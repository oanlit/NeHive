namespace NeHive.Core;

public static partial class Reactive
{
    public static void Batch(Action fn)
    {
        ExecuteNode.StartBatch();
        fn();
        ExecuteNode.EndBatch();
    }

    public static T Batch<T>(Func<T> fn)
    {
        ExecuteNode.StartBatch();
        var result = fn();
        ExecuteNode.EndBatch();
        return result;
    }

    public static void OnDispose(Action fn)
    {
        Scope.CurrentScope.OnDispose(fn);
    }

    public static T Untrack<T>(Func<T> fn)
        => ReactiveContext.Untrack(fn);

    public static void Untrack(Action fn)
        => ReactiveContext.Untrack(fn);

    public static void BatchUntrack(Action fn)
    {
        ExecuteNode.StartBatch();
        ReactiveContext.Untrack(fn);
        ExecuteNode.EndBatch();
    }
}

public interface IReadOnlySignal<out T>
{
    public T Value { get; }
    public T UntrackValue { get; }
}

public interface ISetOnlySignal<T>
{
    public T Value { set; }
    public void SetValue(Func<T, T> value);
}

public interface ISignal<T> : IReadOnlySignal<T>, ISetOnlySignal<T>
{
    public new T Value { get; set; }
}

public class Signal<T>(T value) : ISignal<T>
{
    internal readonly SignalState<T> State = new(value);

    public T Value
    {
        get => State.ReadSignal();
        set => State.WriteSignal(value);
    }

    public void SetValue(T value)
    {
        State.WriteSignal(value);
    }

    public void SetValue(Func<T, T> value)
    {
        State.WriteSignal(value(State.Value));
    }

    public T UntrackValue => State.Value;
    internal bool HasObserver => State.Observers.Count > 0;
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

    internal Scope(ScopeNode scope)
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
            if (currentScopeNode is ExecuteNode)
            {
                // ExecuteNode 每次运行时都会dispose自己，为了复用，我们绑定到 Parent Scope
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
        var computation = ReactiveContext.CurrentExecute;
        return ReactiveContext.RunInContext(fn, _root, computation);
    }

    public Effect AddEffect(Action fn)
        => _setContext(() => new Effect(fn));

    public Effect AddEffect(Action<Scope> fn)
        => _setContext(() => new Effect(fn));

    public Effect AddEffect(Func<Scope, Action<Scope>> fn)
        => _setContext(() => new Effect(fn));

    public Computed<T> AddComputed<T>(Func<T, T> fn, T? value = default)
        => _setContext(() => new Computed<T>(fn, value));

    public Computed<T> AddComputed<T>(Func<T> fn, T? value = default)
        => _setContext(() => new Computed<T>(fn, value));

    public T RunInScope<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(Scope));
        return _root.RunInScope(fn)!;
    }

    public void RunInScope(Action fn)
        => RunInScope(Util.WrapAction(fn));
}

public class EpochScope : Scope
{
    private readonly ExecuteNode _track;

    internal EpochScope(ExecuteNode track) : base(track)
    {
        _track = track;
    }

    public T Track<T>(Signal<T> signal)
    {
        return _track.Track(signal.State);
    }

    public T Track<T>(Func<T> trackFn)
    {
        return _track.Track(trackFn);
    }

    public void Track(Action trackFn)
    {
        _track.Track(trackFn);
    }
}

public class Effect : IDisposable
{
    private readonly ScopeNode _scope;
    public bool IsInvalid { get; private set; }

    public Effect(Action executeFn)
    {
        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        _scope.RunInScope(() =>
            new EffectNode<object>((trackScope, _) =>
            {
                trackScope.Track(executeFn);
                return Constant.EmptyObj;
            }, Constant.EmptyObj)
        );
    }

    public Effect(Action<EpochScope> fn) : this(_ => fn)
    {
    }

    public Effect(Func<Scope, Action<EpochScope>> setupFn)
    {
        var current = ReactiveContext.CurrentScope;

        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => IsInvalid = true);

        _scope.RunInScope(() =>
        {
            var effectScope = Scope.CurrentScope;
            var executeFn = setupFn(effectScope);
            EpochScope? epochScope = null;

            return new EffectNode<object>(
                (trackScope, _) =>
                {
                    epochScope ??= new EpochScope(trackScope);
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

public class Computed<T> : IReadOnlySignal<T>
{
    private readonly ScopeNode _scope;
    private readonly ComputedNode<T> _computedNode;

    private T _value;

    private readonly Func<T, T> _fn;

    // 缓存是否失效 (不影响依赖建立)
    public bool IsInvalid { get; private set; }

    public T Value
    {
        get
        {
            if (IsInvalid) return _fn(_value);
            _value = _computedNode.ReadSignal();
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

            _value = _computedNode.UntrackValue;
            return _value;
        }
    }

    public Computed(Func<T, T> fn, T? value = default)
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

        _computedNode = _scope.RunInScope(() =>
            new ComputedNode<T>(
                (trackScope, prev) =>
                    trackScope.Track(() => fn(prev)),
                comparator: Constant.EqualFn
            )
            {
                Phase = ExecutePhase.Resolved,
                Value = value!
            });

        _computedNode.UpdateComputation();
        _value = _computedNode.UntrackValue;

        _scope.Cleanups.Add(_afterDisposed);
    }

    public Computed(Func<T> fn, T? value = default)
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
        var observers = _computedNode.Observers;
        if (observers.Count == 0) return;
        ExecuteNode.NotifyObservers(observers);
        _computedNode.Observers.Clear();
    }
}

public class Selector<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<ExecuteNode>> _subs;
    private readonly Computed<T> _computed;
    private readonly Signal<bool> _logicSignal = new(true);

    public readonly Func<T, T, bool> CompareFn;

    public Selector(IReadOnlySignal<T> source, Func<T, T, bool>? compareFn = null)
    {
        CompareFn = compareFn ?? Constant.EqualFn;
        _subs = new Dictionary<T, HashSet<ExecuteNode>>();

        _computed = new Computed<T>(fn: ComputationFn);
        return;

        T ComputationFn(T? prevValue)
        {
            var nextValue = source.Value;
            foreach (var (key, observers) in _subs.AsEnumerable())
            {
                // 异或比较
                if (CompareFn(key, nextValue) == CompareFn(key, prevValue!)) continue;
                ExecuteNode.NotifyObservers(observers);
            }

            return nextValue;
        }
    }

    public bool Select(T key)
    {
        _ = _logicSignal.Value; // 逻辑依赖，不会导致外部观察者误以为没有信号而失效

        var value = _computed.UntrackValue;
        var currentComputation = ReactiveContext.CurrentExecute;
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
        var scope = ReactiveContext.CurrentScope;
        if (scope is null) throw new Exception("CurrentOwner is None!");
        scope.Context ??= new Dictionary<object, object?>();
        scope.Context[Id] = value;
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

public enum AsyncMemoState
{
    Unresolved,
    Pending,
    Ready,
    Refreshing,
    Errored
}

public class AsyncMemo<TSource, TValue, TInfo> : ISignal<TValue?>
{
    private readonly Scope _scope = new();
    private readonly IReadOnlySignal<TSource?> _source;
    private readonly Func<TSource, TValue?, TInfo?, Task<TValue>> _fetcher;
    private readonly Signal<TValue?> _value = new(default);
    private readonly Signal<Exception?> _error = new(null);
    private readonly Signal<AsyncMemoState> _state = new(AsyncMemoState.Unresolved);

    private Task<TValue>? _result;
    private bool _scheduled;
    private bool _resolved;

    public AsyncMemoState State => _state.Value;

    public bool Loading
    {
        get
        {
            var state = _state.Value;
            return state is AsyncMemoState.Pending or AsyncMemoState.Refreshing;
        }
    }

    public TValue? Value
    {
        get
        {
            var v = _value.Value;
            var err = _error.Value;
            if (err is not null && _result is null) throw err;
            return v;
        }
        set => _value.Value = value;
    }

    public TValue? UntrackValue
    {
        get
        {
            var v = _value.UntrackValue;
            var err = _error.UntrackValue;
            if (err is not null && _result is null) throw new Exception(err.ToString());
            return v;
        }
    }

    public void SetValue(Func<TValue?, TValue?> value)
    {
        _value.SetValue(value);
    }

    public TValue? Latest
    {
        get
        {
            if (!_resolved) return Value;
            var err = _error.Value;
            if (err is not null && _result is null) throw err;
            return _value.Value;
        }
    }

    public Exception? Error => _error.Value;

    public AsyncMemo(Func<TSource, TValue?, TInfo?, Task<TValue>> fetcher, IReadOnlySignal<TSource> signalSource)
    {
        _fetcher = fetcher;
        _source = signalSource;
        _scope.AddEffect(() => _load());
    }

    public AsyncMemo(Func<TSource, TValue?, TInfo?, Task<TValue>> fetcher, TSource? source = default)
    {
        _fetcher = fetcher;
        _source = new SimpleReadOnlySignal<TSource?>(source);
        _scope.AddEffect(() => _load());
    }

    public Task<TValue?> Refetch(TInfo info)
    {
        return _scope.RunInScope(() => _load(true, info));
    }

    private Task<TValue?> _load(bool isRefetch = false, TInfo? info = default)
    {
        if (isRefetch && _scheduled) return Task.FromResult<TValue?>(default);
        _scheduled = false;
        var source = _source.Value;
        if (source is null)
        {
            _loadEnd(_result, _value.UntrackValue);
            return Task.FromResult<TValue?>(default);
        }

        Exception? error = null;

        var result = ReactiveContext.Untrack(() =>
        {
            try
            {
                return _fetcher(source, _value.Value, info);
            }
            catch (Exception fetcherError)
            {
                error = fetcherError;
            }

            return null;
        });

        if (error is not null)
        {
            _loadEnd(_result, default, error, source);
            return Task.FromResult<TValue?>(default);
        }

        if (result is null) return Task.FromResult<TValue?>(default);

        _result = result;
        _scheduled = true;
        _ = ResetScheduledAsync();

        ExecuteNode.StartBatch();
        _state.Value = _resolved ? AsyncMemoState.Refreshing : AsyncMemoState.Pending;
        ExecuteNode.EndBatch();

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
                var v = await result;
                return _loadEnd(_result, v, null, source);
            }
            catch (Exception err)
            {
                return _loadEnd(_result, default, err, source);
            }
        }
    }

    private TValue? _loadEnd(Task<TValue>? result, TValue? value, Exception? error = null, TSource? key = default)
    {
        if (_result != result) return value;
        if (_result == result)
            _result = null;
        if (key is not null) _resolved = true;

        _completeLoad(value, error);

        return value;
    }

    private void _completeLoad(TValue? v, Exception? err)
    {
        ExecuteNode.StartBatch();
        if (err is null)
        {
            _value.Value = v;
            _state.Value = _resolved ? AsyncMemoState.Ready : AsyncMemoState.Unresolved;
        }
        else _state.Value = AsyncMemoState.Errored;

        _error.Value = err;
        ExecuteNode.EndBatch();
    }
}