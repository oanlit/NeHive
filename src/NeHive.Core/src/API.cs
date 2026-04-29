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

public class Accessor<T> : IReadOnlySignal<T>
{
    internal ISignalState<T>? ValueSignal;
    internal Func<T> ValueGetter;
    internal Func<T> UntrackValueGetter;

    public T Value => ValueGetter();

    public T UntrackValue => UntrackValueGetter();

    private Accessor(T value)
    {
        ValueSignal = null;
        ValueGetter = () => value;
        UntrackValueGetter = () => Reactive.Untrack(ValueGetter);
    }

    private Accessor(Func<T> valueGetter)
    {
        ValueSignal = null;
        ValueGetter = valueGetter;
        UntrackValueGetter = () => Reactive.Untrack(ValueGetter);
    }

    internal Accessor(ISignalState<T> valueSignal)
    {
        ValueSignal = valueSignal;
        ValueGetter = valueSignal.ReadSignal;
        UntrackValueGetter = () => valueSignal.Value;
    }

    internal Accessor()
    {
        ValueSignal = null;
        ValueGetter = () => default!;
        UntrackValueGetter = () => Reactive.Untrack(ValueGetter);
    }

    public static implicit operator Accessor<T>(T value)
    {
        return new Accessor<T>(value);
    }

    public static implicit operator Accessor<T>(Func<T> getter)
    {
        return new Accessor<T>(getter);
    }
}

public class Signal<T>(T value) : Accessor<T>(new SignalState<T>(value)),
    ISetOnlySignal<T>
{
    public new T Value
    {
        get => ValueSignal!.ReadSignal();
        set => ValueSignal!.WriteSignal(value);
    }

    public void SetValue(T value)
    {
        ValueSignal!.WriteSignal(value);
    }

    public void SetValue(Func<T, T> value)
    {
        ValueSignal!.WriteSignal(value(ValueSignal.Value));
    }

    internal bool HasObserver => ValueSignal!.Observers.Count > 0;
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
    private readonly ExecuteNode _tracker;

    internal EpochScope(ExecuteNode tracker) : base(tracker)
    {
        _tracker = tracker;
    }

    public T Track<T>(Accessor<T> signal)
    {
        return signal.ValueSignal is null
            ? _tracker.Track(signal.ValueGetter)
            : _tracker.Track(signal.ValueSignal);
    }

    public T Track<T>(Func<T> trackFn)
    {
        return _tracker.Track(trackFn);
    }

    public void Track(Action trackFn)
    {
        _tracker.Track(trackFn);
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

        using (new ReactiveContextHelper(_scope, null))
        {
            _ = new EffectNode<object>((tracker, _) =>
            {
                tracker.Track(executeFn);
                return Constant.EmptyObj;
            }, Constant.EmptyObj);
        }
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

        using (new ReactiveContextHelper(_scope, null))
        {
            var effectScope = Scope.CurrentScope;
            var executeFn = setupFn(effectScope);
            EpochScope? epochScope = null;
            _ = new EffectNode<object>(
                (tracker, _) =>
                {
                    epochScope ??= new EpochScope(tracker);
                    executeFn(epochScope);
                    return Constant.EmptyObj;
                },
                Constant.EmptyObj
            );
        }
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
    }
}

public class Computed<T> : Accessor<T>
{
    private readonly ScopeNode _scope;
    private readonly ComputedNode<T> _computedNode;

    private T _value;

    private readonly Func<T, T> _fn;

    // 缓存是否失效 (不影响依赖建立)
    public bool IsInvalid { get; private set; }

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
        _scope.Cleanups.Add(_afterDisposed);

        using (new ReactiveContextHelper(_scope, null))
        {
            _computedNode = new ComputedNode<T>(
                (tracker, prev) =>
                    tracker.Track(() => fn(prev)),
                comparator: Constant.EqualFn
            )
            {
                Phase = ExecutePhase.Resolved,
                Value = value!
            };
        }

        _initSignalGetter();

        _computedNode.UpdateComputation();
        _value = _computedNode.UntrackValue;
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

    private void _initSignalGetter()
    {
        ValueSignal = _computedNode;
        ValueGetter = () =>
        {
            if (IsInvalid) return _fn(_value!);
            _value = _computedNode.ReadSignal();
            return _value;
        };
        UntrackValueGetter = () =>
        {
            if (IsInvalid)
            {
                _value = Reactive.Untrack(() => _fn(_value!));
                return _value;
            }

            _value = _computedNode.UntrackValue;
            return _value;
        };
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

public enum AsyncMemoState
{
    Unresolved,
    Pending,
    Ready,
    Refreshing,
    Errored,
    IsInvalid
}

public class AsyncMemo<T> : Accessor<T?>
{
    private readonly ScopeNode _scope;
    private EpochScope? _epochScope;
    private readonly bool _isSimpleUse;

    private readonly Func<EpochScope, Task<T>> _executeFn;

    // private readonly Signal<T?> _value = new(default);
    private readonly Signal<Exception?> _error = new(null);
    private readonly Signal<AsyncMemoState> _state = new(AsyncMemoState.Unresolved);

    private Task<T>? _result;
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

    public T? Latest
    {
        get
        {
            if (!_resolved) return Value;
            var err = _error.Value;
            if (err is not null && _result is null) throw err;
            return ValueSignal!.ReadSignal();
        }
    }

    public Exception? Error => _error.Value;

    public AsyncMemo(Func<Task<T>> executeFn)
    {
        _initSignalGetter();

        var current = ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => _state.Value = AsyncMemoState.IsInvalid);
        _isSimpleUse = true;

        _executeFn = _ => executeFn();

        using (new ReactiveContextHelper(_scope, null))
        {
            _ = new EffectNode<object>((tracker, _) =>
            {
                _epochScope ??= new EpochScope(tracker);
                _load(false);
                return Constant.EmptyObj;
            }, Constant.EmptyObj);
        }
    }

    public AsyncMemo(Func<EpochScope, Task<T>> fn) : this(_ => fn)
    {
    }

    public AsyncMemo(Func<Scope, Func<EpochScope, Task<T>>> setupFn)
    {
        _initSignalGetter();

        var current = ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => _state.Value = AsyncMemoState.IsInvalid);
        _isSimpleUse = false;

        using (new ReactiveContextHelper(_scope, null))
        {
            var currentScope = Scope.CurrentScope;
            _executeFn = setupFn(currentScope);
            _ = new EffectNode<object>(
                (tracker, _) =>
                {
                    _epochScope ??= new EpochScope(tracker);
                    _load(false);
                    return Constant.EmptyObj;
                },
                Constant.EmptyObj
            );
        }
    }

    public Task<T?> Refetch()
    {
        return _scope.RunInScope(() => _load(true));
    }

    private void _initSignalGetter()
    {
        ValueSignal = new SignalState<T?>(default);
        ValueGetter = () =>
        {
            var v = ValueSignal!.ReadSignal();
            var err = _error.Value;
            if (err is not null && _result is null) throw err;
            return v;
        };
        UntrackValueGetter = () =>
        {
            var v = ValueSignal!.Value;
            var err = _error.UntrackValue;
            if (err is not null && _result is null) throw err;
            return v;
        };
    }

    private Task<T?> _load(bool isRefetch)
    {
        if (isRefetch && _scheduled) return Task.FromResult<T?>(default);
        _scheduled = false;

        Exception? error = null;
        Task<T>? result = null;
        var epochScope = _epochScope!;

        try
        {
            result = _isSimpleUse
                ? epochScope.Track(() => _executeFn(epochScope))
                : _executeFn(epochScope);
        }
        catch (Exception fetcherError)
        {
            error = fetcherError;
        }

        if (error is not null)
        {
            _loadEnd(_result, default, error);
            return Task.FromResult<T?>(default);
        }

        if (result is null) return Task.FromResult<T?>(default);

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

        async Task<T?> HandleAsync()
        {
            try
            {
                var v = await result;
                return _loadEnd(_result, v);
            }
            catch (Exception err)
            {
                return _loadEnd(_result, default, err);
            }
        }
    }

    private T? _loadEnd(Task<T>? result, T? value, Exception? error = null)
    {
        if (_result != result) return value;
        if (_result == result)
            _result = null;
        _resolved = true;

        _completeLoad(value, error);

        return value;
    }

    private void _completeLoad(T? v, Exception? err)
    {
        ExecuteNode.StartBatch();
        if (err is null)
        {
            // _value.Value = v;
            ValueSignal!.WriteSignal(v);
            _state.Value = _resolved ? AsyncMemoState.Ready : AsyncMemoState.Unresolved;
        }
        else _state.Value = AsyncMemoState.Errored;

        _error.Value = err;
        ExecuteNode.EndBatch();
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