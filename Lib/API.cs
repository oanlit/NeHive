namespace Lib;

using System.Diagnostics;

public static partial class Reactive
{
    public static void Batch(Action fn)
        => ComputationNode.RunBatch(fn, false);

    public static T Batch<T>(Func<T> fn)
        => ComputationNode.RunBatch(fn, false);

    public static void OnMount(Action fn)
    {
        if (ReactiveContext.CurrentOwner is null)
        {
            Trace.TraceWarning("cleanups created outside a `createRoot` or `render` will never be run");
            return;
        }

        _ = new EffectNode<object>(_ =>
        {
            Untrack(fn);
            return Constant.EmptyObj;
        }, Constant.EmptyObj);
    }

    public static void OnCleanup(Action fn)
    {
        if (ReactiveContext.CurrentOwner is null)
            Trace.TraceWarning("cleanups created outside a `createRoot` or `render` will never be run");
        else if (ReactiveContext.CurrentOwner.Cleanups is null) ReactiveContext.CurrentOwner.Cleanups = [fn];
        else ReactiveContext.CurrentOwner.Cleanups.Add(fn);
    }

    public static T Untrack<T>(Func<T> fn)
        => ComputationNode.Untrack(fn);

    public static void Untrack(Action fn)
        => ComputationNode.Untrack(fn);

    public static T Untrack<T>(IReadOnlySignal<T> source)
        => ComputationNode.Untrack(source);

    public static void BatchUntrack(Action fn)
    {
        ComputationNode.RunBatch(() =>
        {
            ComputationNode.Untrack(fn);
            return Constant.EmptyObj;
        }, false);
    }

    public static Func<T, bool> CreateSelector<T>(Func<T> source, Func<T, T, bool>? fn = null) where T : notnull
    {
        fn ??= Constant.EqualFn;
        var subs = new Dictionary<T, HashSet<ComputationNode>>();
        var computationFn = (T? prevValue) =>
        {
            var nextValue = source();
            foreach (var (key, val) in subs.AsEnumerable())
                if (fn(key, nextValue) != fn(key, prevValue!)) // 异或比较
                {
                    foreach (var c in val)
                    {
                        // c.Phase = ComputationPhase.Stale;
                        // if (c.IsPure) CurrentState.UpdateQueue!.Add(c);
                        // else CurrentState.EffectQueue!.Add(c);
                        c.AddQueue(); // 以上是源码，用这个会不会有问题?
                    }
                }

            return nextValue;
        };
        var node = new MemoNode<T>(fn: computationFn, isPure: true, phase: ComputationPhase.Stale);
        node.UpdateComputation();
        return key =>
        {
            var currentComputation = ReactiveContext.CurrentComputation;
            if (currentComputation is null) return fn(key, node.Value);

            if (subs.TryGetValue(key, out var computations))
                computations.Add(currentComputation);
            else
            {
                computations = [currentComputation];
                subs.Add(key, computations);
            }

            OnCleanup(() =>
            {
                computations.Remove(currentComputation);
                if (computations.Count == 0) subs.Remove(key);
            });
            return fn(key, node.Value);
        };
    }
}

public interface IReadOnlySignal<out T>
{
    public T Value { get; }
    public T UntrackValue { get; }
}

public interface ISetOnlySignal<in T>
{
    public T Value { set; }
}

public interface ISignal<T> : IReadOnlySignal<T>, ISetOnlySignal<T>
{
    public new T Value { get; set; }
}

public class Signal<T>(T value) : ISignal<T>
{
    private readonly SignalState<T> _state = new(value);

#if DEBUG
    internal SignalState<T> State => _state;
#endif

    public T Value
    {
        get => _state.ReadSignal();
        set => _state.WriteSignal(value);
    }

    public void SetValue(T value)
    {
        _state.WriteSignal(value);
    }

    public void SetValue(Func<T, T> value)
    {
        _state.WriteSignal(value(_state.Value));
    }

    public T UntrackValue => _state.Value;
    internal bool HasObserver => _state.Observers?.Count > 0;
}

public class Owner : IDisposable
{
    private readonly OwnerTree _root;
    private bool _isDisposed;

    public Owner(Owner? parentOwner = null)
    {
        var currentOwner = ReactiveContext.CurrentOwner;
        var current = parentOwner?._root ?? currentOwner;
        _root = new OwnerTree(
            parent: current,
            children: null,
            context: current?.Context,
            cleanups: null
        );
    }

    public void AddMount(Action fn)
        => AddEffect(() => ComputationNode.Untrack(fn));

    public void AddCleanup(Action fn)
    {
        if (_root.Cleanups is null) _root.Cleanups = [fn];
        else _root.Cleanups.Add(fn);
    }

    public void Clean()
    {
        if (_isDisposed) return;
        ComputationNode.Untrack(() => _root.CleanNode());
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        Clean();
        _isDisposed = true;
    }

    private T _setContext<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Owner));
        var computation = ReactiveContext.CurrentComputation;
        return ComputationNode.SetContext(fn, _root, computation);
    }

    public Effect<T> AddEffect<T>(Func<T, T> fn, T value)
        => _setContext(() => new Effect<T>(fn, value));

    public Effect<object> AddEffect(Action fn)
        => AddEffect(Util.WrapActionWithArg(fn), Constant.EmptyObj);

    public Memo<T> AddMemo<T>(Func<T, T> fn, T? value = default)
        => _setContext(() => new Memo<T>(fn, value));

    public Memo<T> AddMemo<T>(Func<T> fn, T? value = default)
        => AddMemo(_ => fn(), value);

    public Computation<T> AddComputation<T>(Func<T, T> fn,
        T init,
        bool pure = false)
        => _setContext(() => new Computation<T>(fn, init, pure));

    public T RunWithOwner<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(Owner));
        return ComputationNode.RunWithOwner(_root, fn)!;
    }

    public void RunWithOwner(Action fn)
        => RunWithOwner(Util.WrapAction(fn));
}

public class Computation<T> : IDisposable
{
    private readonly ComputationNode<T> _node;
    private bool _isDisposed;

    public Computation(Func<T, T> fn,
        T init,
        bool pure = false)
    {
        _node = new(fn, init, isPure: pure);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        ComputationNode.Untrack(() => _node.CleanNode());
        _isDisposed = true;
    }
}

public record struct EffectOptions<T>(
    Func<T, T?, T, T> Fn,
    Func<T> Deps,
    bool Defer = false
);

public class Effect : IDisposable
{
    private readonly EffectNode<object> _node;
    private bool _isDisposed;

    public bool IsDisposed
    {
        get
        {
            if (_isDisposed) return true;
            if ((_node.Sources?.Count ?? 0) != 0) return false;

            _isDisposed = true;
            return true;
        }
    }

    public Effect(Action fn)
    {
        var owner = ReactiveContext.CurrentOwner ?? Constant.UnOwned;
        var computation = ReactiveContext.CurrentComputation;

        _node = ComputationNode.SetContext(
            () => new EffectNode<object>(Util.WrapActionWithArg(fn), Constant.EmptyObj),
            owner, computation
        );
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        ComputationNode.Untrack(() => _node.CleanNode());
        _isDisposed = true;
    }
}

public class Effect<T> : IDisposable
{
    private readonly EffectNode<T> _node;
    private bool _isDisposed;

    public bool IsDisposed
    {
        get
        {
            if (_isDisposed) return true;
            if ((_node.Sources?.Count ?? 0) != 0) return false;

            _isDisposed = true;
            return true;
        }
    }

    public Effect(Func<T, T> fn, T value)
    {
        var owner = ReactiveContext.CurrentOwner ?? Constant.UnOwned;
        var computation = ReactiveContext.CurrentComputation;

        _node = ComputationNode.SetContext(
            () => new EffectNode<T>(fn, value),
            owner, computation
        );
    }

    public Effect(EffectOptions<T> options)
    {
        T? prevInput = default;
        Func<T, T> fn = prevValue =>
        {
            var input = options.Deps();
            if (options.Defer)
            {
                options.Defer = false;
                return prevValue;
            }

            var result = ComputationNode.Untrack(() => options.Fn(input, prevInput, prevValue));
            prevInput = input;
            return result;
        };

        var owner = ReactiveContext.CurrentOwner ?? Constant.UnOwned;
        var computation = ReactiveContext.CurrentComputation;
        _node = ComputationNode.SetContext(
            () => new EffectNode<T>(fn, prevInput!),
            owner, computation
        );
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        ComputationNode.Untrack(() => _node.CleanNode());
        _isDisposed = true;
    }
}

public class Memo<T> : IReadOnlySignal<T>
{
    private readonly MemoNode<T> _memoNode;
    private bool _isDisposed;
    private T _value;

    public bool IsDisposed
    {
        get
        {
            if (_isDisposed) return true;
            if ((_memoNode.Sources?.Count ?? 0) != 0) return false;

            _isDisposed = true;
            return true;
        }
    }

    public T Value
    {
        get
        {
            if (_isDisposed) return _value;
            if ((_memoNode.Sources?.Count ?? 0) == 0)
            {
                _isDisposed = true;
                return _value;
            }

            _value = _memoNode.ReadSignal();
            return _value;
        }
    }

    public T UntrackValue
    {
        get
        {
            if (_isDisposed) return _value;
            if ((_memoNode.Sources?.Count ?? 0) == 0)
            {
                _isDisposed = true;
                return _value;
            }

            _value = _memoNode.UntrackValue;
            return _value;
        }
    }

    public Memo(Func<T, T> fn, T? value = default)
    {
        var owner = ReactiveContext.CurrentOwner ?? Constant.UnOwned;
        var computation = ReactiveContext.CurrentComputation;
        _memoNode = ComputationNode.SetContext(() =>
            new MemoNode<T>(
                fn,
                comparator: Constant.EqualFn,
                isPure: true
            )
            {
                Observers = null,
                ObserverSlots = null,
                Phase = ComputationPhase.Resolved,
                Value = value!
            }, owner, computation);

        _memoNode.UpdateComputation();
        _value = _memoNode.UntrackValue;
    }

    public Memo(Func<T> fn, T? value = default)
        : this(_ => fn(), value)
    {
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        ComputationNode.Untrack(() => _memoNode.CleanNode());
        _isDisposed = true;
    }
}

public class Context<T>(string id = "context", T defaultValue = default!)
{
    public readonly string Id = id;
    public readonly T DefaultValue = defaultValue;

    public void Provider(T value, Action fn)
    {
        var owner = ReactiveContext.CurrentOwner;
        if (owner is null) throw new Exception("CurrentOwner is None!");
        owner.Context ??= new Dictionary<object, object?>();
        owner.Context[Id] = value;
        fn();
    }

    public T UseContext()
    {
        var owner = ReactiveContext.CurrentOwner;
        if (owner?.Context?[Id] is T value) return value;
        return DefaultValue;
    }
}

internal class SimpleReadOnlySignal<T>(T source) : IReadOnlySignal<T>
{
    public T Value { get; } = source;
    public T UntrackValue { get; } = source;
}

public enum ResourcePhase
{
    Unresolved,
    Pending,
    Ready,
    Refreshing,
    Errored
}

public class Resource<TSource, TValue, TInfo>
{
    public ResourcePhase Phase => _state.Value;

    public bool Loading
    {
        get
        {
            var s = _state.Value;
            return s == ResourcePhase.Pending || s == ResourcePhase.Refreshing;
        }
    }

    public object? Error => _error.Value;

    public TValue? Latest
    {
        get
        {
            if (!_resolved) return Read();
            var err = _error.Value;
            if (err is not null && _task is null) throw new Exception(err.ToString());
            return _value.Value;
        }
    }

    private readonly Signal<ResourcePhase> _state = new(ResourcePhase.Unresolved);
    private readonly Signal<object?> _error = new(null);
    private bool _resolved;
    private readonly Signal<TValue?> _value = new(default);
    private Task<TValue>? _task;
    private object _initTask = Constant.EmptyObj;
    private readonly OwnerTree? _owner = ReactiveContext.CurrentOwner;
    private bool _scheduled;
    private readonly IReadOnlySignal<TSource?> _source;

    // object 的类型为 TValue | Task<TValue>
    private readonly Func<TSource, TValue?, TInfo?, object> _fetcher;

    public Resource(Func<TSource, TValue?, TInfo?, object> fetcher, IReadOnlySignal<TSource> signalSource)
    {
        _fetcher = fetcher;
        _source = new Memo<TSource?>(_ => signalSource.Value);
    }

    public Resource(Func<TSource, TValue?, TInfo?, object> fetcher, TSource? source = default)
    {
        _fetcher = fetcher;
        _source = new SimpleReadOnlySignal<TSource?>(source);
    }

    public TValue? Read()
    {
        var v = _value.Value;
        var err = _error.Value;
        if (err is not null && _task is null) throw new Exception(err.ToString());
        return v;
    }

    // object 的类型为 TInfo | Task<TInfo>
    public object? Refetch(TInfo info)
    {
        return ComputationNode.RunWithOwner(_owner, () => _load(info));
    }

    public void Mutate(TValue value)
    {
        _value.SetValue(value);
    }

    public void Mutate(Func<TValue?, TValue?> value)
    {
        _value.SetValue(value);
    }

    private object? _load(TInfo? refetching = default, bool isRefetch = false)
    {
        if (isRefetch && _scheduled) return null;
        _scheduled = false;
        var lookup = _source.Value;
        if (lookup is null || lookup is false)
        {
            _loadEnd(_task, ComputationNode.Untrack(_value));
            return null;
        }

        object? error = null;

        object? p;
        if (_initTask == Constant.EmptyObj)
        {
            p = ComputationNode.Untrack(() =>
            {
                try
                {
                    return _fetcher(lookup, _value.Value, refetching);
                }
                catch (Exception fetcherError)
                {
                    error = fetcherError;
                }

                return null;
            });
        }
        else p = _initTask;

        if (error is not null)
        {
            _loadEnd(_task, default, ComputationNode.CastError(error), lookup);
            return null;
        }

        if (p is not Task<TValue> && p is TValue pValue)
        {
            _loadEnd(_task, pValue, null, lookup);
            return p;
        }

        if (p is not Task<TValue> pTask) return null;

        _task = pTask;
        _scheduled = true;
        _ = ResetScheduledAsync();
        ComputationNode.RunBatch(
            () => { _state.Value = _resolved ? ResourcePhase.Refreshing : ResourcePhase.Pending; },
            false);
        return HandleAsync();

        async Task ResetScheduledAsync()
        {
            await Task.Yield();
            _scheduled = false;
        }

        async Task<TValue?> HandleAsync()
        {
            try
            {
                var v = await pTask;
                return _loadEnd(_task, v, null, lookup);
            }
            catch (Exception e)
            {
                return _loadEnd(_task, default, ComputationNode.CastError(e), lookup);
            }
        }
    }

    private TValue? _loadEnd(Task<TValue>? p, TValue? v, object? error = null, TSource? key = default)
    {
        if (_task != p) return v;
        if (_task == p)
            _task = null;
        if (key is not null) _resolved = true;

        _initTask = Constant.EmptyObj;
        _completeLoad(v, error);

        return v;
    }

    private void _completeLoad(TValue? v, object? err)
    {
        ComputationNode.RunBatch(() =>
        {
            if (err is null)
            {
                _value.Value = v;
                _state.Value = _resolved ? ResourcePhase.Ready : ResourcePhase.Unresolved;
            }
            else _state.Value = ResourcePhase.Errored;

            _error.Value = err;
        }, false);
    }
}