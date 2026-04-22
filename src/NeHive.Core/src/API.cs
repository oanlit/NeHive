namespace NeHive.Core;

public static partial class Reactive
{
    public static void Batch(Action fn)
    {
        ComputationNode.StartBatch();
        fn();
        ComputationNode.EndBatch();
    }

    public static T Batch<T>(Func<T> fn)
    {
        ComputationNode.StartBatch();
        var result = fn();
        ComputationNode.EndBatch();
        return result;
    }

    public static void OnDispose(Action fn)
    {
        Scope.CurrentScope.OnDispose(fn);
    }

    public static T Untrack<T>(Func<T> fn)
        => ComputationNode.Untrack(fn);

    public static void Untrack(Action fn)
        => ComputationNode.Untrack(fn);

    public static T Untrack<T>(IReadOnlySignal<T> source)
        => ComputationNode.Untrack(source);

    public static void BatchUntrack(Action fn)
    {
        ComputationNode.StartBatch();
        ComputationNode.Untrack(fn);
        ComputationNode.EndBatch();
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
    internal bool HasObserver => _state.Observers.Count > 0;
}

public class Scope : IDisposable
{
    private static readonly Dictionary<ScopeNode, Scope> OwnerHolder = [];

    private readonly ScopeNode _root;
    public bool IsDisposed { get; private set; }

    public Scope(Scope? parentScope = null)
    {
        var currentScope = ReactiveContext.CurrentScope;
        var current = parentScope?._root ?? currentScope;
        _root = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _root.Cleanups.Add(_onDispose);
        OwnerHolder[_root] = this;
    }

    public void OnDispose(Action fn)
    {
        if (IsDisposed) return;
        _root.Cleanups.Add(fn);
    }

    public void Clean()
    {
        if (IsDisposed) return;
        _root.DisposeChildren();
    }

    public void Dispose()
    {
        if (IsDisposed) return;
        _root.Dispose();
    }

    private void _onDispose()
    {
        IsDisposed = true;
        OwnerHolder.Remove(_root);
    }

    private Scope(bool isRoot)
    {
        _ = isRoot;
        _root = Constant.RootScopeTree;
        OwnerHolder[_root] = this;
    }

    static Scope()
    {
        var root = new Scope(true);
        OwnerHolder[Constant.RootScopeTree] = root;
    }

    public static Scope RootScope
        => OwnerHolder[Constant.RootScopeTree];

    private Scope(ScopeNode scope)
    {
        _root = scope;
    }

    public static Scope CurrentScope
    {
        get
        {
            var currentScopeNode = ReactiveContext.CurrentScope;
            OwnerHolder.TryGetValue(currentScopeNode, out var scope);
            if (scope is not null) return scope;

            scope = new Scope(currentScopeNode);
            OwnerHolder[currentScopeNode] = scope;
            if (currentScopeNode is ComputationNode)
            {
                // ComputationNode 每次运行时都会dispose自己，为了复用，我们绑定到 Parent Scope
                var parent = currentScopeNode.Parent!; // 除了 RootScope 以外都有
                parent.Cleanups.Add(() =>
                {
                    OwnerHolder.Remove(currentScopeNode);
                    scope.IsDisposed = true;
                });
            }
            else
            {
                currentScopeNode.Cleanups.Add(() =>
                {
                    OwnerHolder.Remove(currentScopeNode);
                    scope.IsDisposed = true;
                });
            }

            return scope;
        }
    }

    private T _setContext<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(Scope));
        var computation = ReactiveContext.CurrentComputation;
        return ComputationNode.SetContext(fn, _root, computation);
    }

    public Effect<T> AddEffect<T>(Func<T, T> fn, T value)
        => _setContext(() => new Effect<T>(fn, value));

    public Effect AddEffect(Action fn)
        => _setContext(() => new Effect(fn));

    public Effect AddEffect(Action<Scope> fn)
        => _setContext(() => new Effect(fn));

    public Effect AddEffect(Func<Scope, Action> fn)
        => _setContext(() => new Effect(fn));

    public Effect AddEffect(Func<Scope, Action<Scope>> fn)
        => _setContext(() => new Effect(fn));

    public Memo<T> AddMemo<T>(Func<T, T> fn, T? value = default)
        => _setContext(() => new Memo<T>(fn, value));

    public Memo<T> AddMemo<T>(Func<T> fn, T? value = default)
        => _setContext(() => new Memo<T>(fn, value));

    public Computation<T> AddComputation<T>(Func<T, T> fn,
        T init,
        bool pure = false)
        => _setContext(() => new Computation<T>(fn, init, pure));

    public T RunInScope<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(Scope));
        return ComputationNode.RunInScope(_root, fn)!;
    }

    public void RunInScope(Action fn)
        => RunInScope(Util.WrapAction(fn));
}

public class Computation<T> : IDisposable
{
    private readonly ScopeNode _scope;
    public bool IsInvalid { get; private set; }

    public Computation(Func<T, T> fn,
        T init,
        bool pure = false)
    {
        var current = ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        ComputationNode.RunInScope(_scope, () =>
            new ComputationNode<T>(fn, init, isPure: pure));
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
    }
}

public record struct EffectOptions<T>(
    Func<T, T?, T, T> Fn,
    Func<T> Deps,
    bool Defer = false
);

public class Effect : IDisposable
{
    private readonly ScopeNode _scope;
    public bool IsInvalid { get; private set; }

    public Effect(Action fn)
    {
        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        ComputationNode.RunInScope(_scope, () =>
            new EffectNode<object>(Util.WrapActionWithArg(fn), Constant.EmptyObj)
        );
    }

    public Effect(Action<Scope> fn) : this(_ => fn)
    {
    }

    public Effect(Func<Scope, Action> setupFn) : this(selfScope =>
    {
        var executeFn = setupFn(selfScope);
        return _ => executeFn();
    })
    {
    }

    public Effect(Func<Scope, Action<Scope>> setupFn)
    {
        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        ComputationNode.RunInScope(_scope, () =>
        {
            var effectScope = Scope.CurrentScope;
            var executeFn = setupFn(effectScope);
            return new EffectNode<object>(
                _ =>
                {
                    var epochScope = Scope.CurrentScope;
                    executeFn(epochScope);
                    return Constant.EmptyObj;
                },
                Constant.EmptyObj
            );
        });
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
    }
}

public class Effect<T> : IDisposable
{
    private readonly ScopeNode _scope;

    public bool IsInvalid { get; private set; }

    public Effect(Func<T, T> fn, T value)
    {
        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        ComputationNode.RunInScope(_scope,
            () => new EffectNode<T>(fn, value)
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

        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        ComputationNode.RunInScope(_scope,
            () => new EffectNode<T>(fn, prevInput!)
        );
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
    }
}

public class Memo<T> : IReadOnlySignal<T>
{
    private readonly ScopeNode _scope;
    private readonly MemoNode<T> _memoNode;

    private T _value;

    private readonly Func<T, T> _fn;

    // 缓存是否失效 (不影响依赖建立)
    public bool IsInvalid { get; private set; }

    public T Value
    {
        get
        {
            if (IsInvalid) return _fn(_value);
            _value = _memoNode.ReadSignal();
            return _value;
        }
    }

    public T UntrackValue
    {
        get
        {
            if (IsInvalid)
            {
                _value = Reactive.Untrack(() => _fn(_value));
                return _value;
            }

            _value = _memoNode.UntrackValue;
            return _value;
        }
    }

    public Memo(Func<T, T> fn, T? value = default)
    {
        _fn = fn;

        var current = ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        _memoNode = ComputationNode.RunInScope(_scope, () =>
            new MemoNode<T>(
                fn,
                comparator: Constant.EqualFn,
                isPure: true
            )
            {
                Phase = ComputationPhase.Resolved,
                Value = value!
            });

        _memoNode.UpdateComputation();
        _value = _memoNode.UntrackValue;

        _scope.Cleanups.Add(_afterDisposed);
    }

    public Memo(Func<T> fn, T? value = default)
        : this(_ => fn(), value)
    {
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
    }

    private void _afterDisposed()
    {
        IsInvalid = true;
        var observers = _memoNode.Observers;
        if (observers.Count == 0) return;
        ComputationNode.NotifyObservers(observers);
        _memoNode.Observers.Clear();
    }
}

public class Selector<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<ComputationNode>> _subs;
    private readonly Memo<T> _memo;
    private readonly Signal<bool> _logicSignal = new(true);

    public readonly Func<T, T, bool> CompareFn;

    public Selector(IReadOnlySignal<T> source, Func<T, T, bool>? compareFn = null)
    {
        CompareFn = compareFn ?? Constant.EqualFn;
        _subs = new Dictionary<T, HashSet<ComputationNode>>();

        _memo = new Memo<T>(fn: ComputationFn);
        return;

        T ComputationFn(T? prevValue)
        {
            var nextValue = source.Value;
            foreach (var (key, observers) in _subs.AsEnumerable())
            {
                // 异或比较
                if (CompareFn(key, nextValue) == CompareFn(key, prevValue!)) continue;
                ComputationNode.NotifyObservers(observers);
            }

            return nextValue;
        }
    }

    public bool Select(T key)
    {
        _ = _logicSignal.Value; // 逻辑依赖，不会导致外部观察者误以为没有信号而失效

        var value = _memo.UntrackValue;
        var currentComputation = ReactiveContext.CurrentComputation;
        if (currentComputation is null) return CompareFn(key, value);

        // 建立一个逻辑依赖
        if (_subs.TryGetValue(key, out var computations))
            computations.Add(currentComputation);
        else
        {
            computations = [currentComputation];
            _subs.Add(key, computations);
        }

        Reactive.OnDispose(() =>
        {
            computations.Remove(currentComputation);
            if (computations.Count == 0) _subs.Remove(key);
        });
        return CompareFn(key, value);
    }
}

public class Context<T>(string id = "context", T defaultValue = default!)
{
    public readonly string Id = id;
    public readonly T DefaultValue = defaultValue;

    public void Provider(T value, Action fn)
    {
        var owner = ReactiveContext.CurrentScope;
        if (owner is null) throw new Exception("CurrentOwner is None!");
        owner.Context ??= new Dictionary<object, object?>();
        owner.Context[Id] = value;
        fn();
    }

    public T UseContext()
    {
        var owner = ReactiveContext.CurrentScope;
        if (owner.Context?[Id] is T value) return value;
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
    private readonly ScopeNode _owner = ReactiveContext.CurrentScope;
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
        return ComputationNode.RunInScope(_owner, () => _load(info));
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

        ComputationNode.StartBatch();
        _state.Value = _resolved ? ResourcePhase.Refreshing : ResourcePhase.Pending;
        ComputationNode.EndBatch();

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
        ComputationNode.StartBatch();
        if (err is null)
        {
            _value.Value = v;
            _state.Value = _resolved ? ResourcePhase.Ready : ResourcePhase.Unresolved;
        }
        else _state.Value = ResourcePhase.Errored;

        _error.Value = err;
        ComputationNode.EndBatch();
    }
}