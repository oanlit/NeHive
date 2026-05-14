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

    extension(Scope scope)
    {
        public Effect CreateEffect(Action fn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new Effect(fn, scope);
        }

        public Effect CreateEffect(Action<EpochScope> fn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new Effect(fn, scope);
        }

        public Effect CreateEffect(Func<Scope, Action<EpochScope>> fn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new Effect(fn, scope);
        }

        public Computed<T> CreateComputed<T>(Func<T, T> fn, T? value = default)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new Computed<T>(fn, value, scope);
        }

        public Computed<T> CreateComputed<T>(Func<T> fn, T? value = default)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new Computed<T>(fn, value, scope);
        }

        public AsyncMemo<T> CreateAsyncMemo<T>(Func<Task<T>> executeFn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new AsyncMemo<T>(executeFn, scope);
        }

        public AsyncMemo<T> CreateAsyncMemo<T>(Func<EpochScope, Task<T>> executeFn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new AsyncMemo<T>(executeFn, scope);
        }

        public AsyncMemo<T> CreateAsyncMemo<T>(Func<Scope, Func<EpochScope, Task<T>>> setupFn)
        {
            ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
            return new AsyncMemo<T>(setupFn, scope);
        }
    }
}

public interface ISignal<out T>
{
    public T RxValue { get; }
    public T Value { get; }
}

public interface ISetOnlySignal<T>
{
    public T RxValue { set; }
    public void NotifySet(Func<T, T> value);
}

public class Accessor<T> : ISignal<T>
{
    internal readonly ISignalState<T>? InternalSignal;
    internal readonly Func<T> RxValueGetter;
    internal readonly Func<T> ValueGetter;

    public bool IsReactive;
    public T RxValue => RxValueGetter();

    public T Value => ValueGetter();

    public Accessor(T value)
    {
        InternalSignal = null;
        RxValueGetter = () => value;
        ValueGetter = () => Reactive.Untrack(RxValueGetter);
    }

    public Accessor(Func<T> rxValueGetter)
    {
        InternalSignal = null;
        RxValueGetter = rxValueGetter;
        ValueGetter = () => Reactive.Untrack(RxValueGetter);
        // TODO 设计一个可以发现 RxValue 的API
    }

    public Accessor(Signal<T> signal)
    {
        InternalSignal = signal.InternalSignal;
        RxValueGetter = InternalSignal.ReadSignal;
        ValueGetter = () => Reactive.Untrack(InternalSignal.ReadSignal);
        IsReactive = true;
    }

    public static implicit operator Accessor<T>(T value)
    {
        return new Accessor<T>(value);
    }

    public static implicit operator Accessor<T>(Func<T> getter)
    {
        return new Accessor<T>(getter);
    }

    public static implicit operator Accessor<T>(Signal<T> signal)
    {
        return new Accessor<T>(signal);
    }
}

public abstract class Signal
{
    internal abstract ISignalState GetInternalSignal();
}

public class Signal<T> : Signal, ISignal<T>
{
    internal ISignalState<T> InternalSignal;
    internal override ISignalState<T> GetInternalSignal() => InternalSignal;

    public virtual T RxValue => InternalSignal.ReadSignal();

    public virtual T Value => InternalSignal.Value;

    internal Signal(ISignalState<T> internalSignal)
    {
        InternalSignal = internalSignal;
    }

    internal Signal()
    {
        InternalSignal = new SignalState<T>(default!);
    }
}

public class MutSignal<T>(T value) : Signal<T>(new SignalState<T>(value)),
    ISetOnlySignal<T>
{
    public new T RxValue
    {
        get => InternalSignal.ReadSignal();
        set => InternalSignal.WriteSignal(value);
    }

    public void NotifySet(T value)
    {
        InternalSignal.WriteSignal(value);
    }

    public void NotifySet(Func<T, T> value)
    {
        InternalSignal.WriteSignal(value(InternalSignal.Value));
    }

    internal bool HasObserver => InternalSignal.Observers.Count > 0;
}

public interface IScope : IDisposable
{
    public bool IsDisposed { get; }
    public void OnDispose(Action fn);
}

public class Scope : IScope
{
    private static readonly Dictionary<ScopeNode, Scope> ScopeHolder = [];

    internal readonly ScopeNode InnerScopeNode;
    public bool IsDisposed { get; private set; }

    public Scope(Scope? parentScope = null)
    {
        var currentScope = ReactiveContext.CurrentScope;
        var current = parentScope?.InnerScopeNode ?? currentScope;
        InnerScopeNode = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        InnerScopeNode.Cleanups.Add(_onDispose);
        ScopeHolder[InnerScopeNode] = this;
    }

    public void OnDispose(Action fn)
    {
        if (IsDisposed) return;
        InnerScopeNode.Cleanups.Add(fn);
    }

    public void Clean()
    {
        if (IsDisposed) return;
        InnerScopeNode.DisposeChildren();
    }

    public void Dispose()
    {
        if (IsDisposed) return;
        InnerScopeNode.Dispose();
    }

    private void _onDispose()
    {
        IsDisposed = true;
        ScopeHolder.Remove(InnerScopeNode);
    }

    private Scope(bool isRoot)
    {
        _ = isRoot;
        InnerScopeNode = Constant.RootScopeTree;
        ScopeHolder[InnerScopeNode] = this;
    }

    static Scope()
    {
        var root = new Scope(true);
        ScopeHolder[Constant.RootScopeTree] = root;
    }

    public static Scope RootScope
        => ScopeHolder[Constant.RootScopeTree];

    internal Scope(ScopeNode scope)
        => InnerScopeNode = scope;

    public static Scope CurrentScope
    {
        get
        {
            var currentScopeNode = ReactiveContext.CurrentScope;
            ScopeHolder.TryGetValue(currentScopeNode, out var scope);
            if (scope is not null) return scope;

            scope = new Scope(currentScopeNode);
            ScopeHolder[currentScopeNode] = scope;
            if (currentScopeNode is ExecuteNode)
            {
                // ExecuteNode 每次运行时都会dispose自己，为了复用，我们绑定到 Parent Scope
                var parent = currentScopeNode.Parent!; // 除了 RootScope 以外都有
                parent.Cleanups.Add(() =>
                {
                    ScopeHolder.Remove(currentScopeNode);
                    scope.IsDisposed = true;
                });
            }
            else
            {
                currentScopeNode.Cleanups.Add(() =>
                {
                    ScopeHolder.Remove(currentScopeNode);
                    scope.IsDisposed = true;
                });
            }

            return scope;
        }
    }

    public T RunInScope<T>(Func<T> fn)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(Scope));
        return InnerScopeNode.RunInScope(fn)!;
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

    public T Pull<T>(Signal<T> signal)
    {
        return _tracker.Pull(signal.InternalSignal);
    }

    public void Pull(IEnumerable<Signal> signals)
    {
        foreach (var signal in signals)
        {
            _tracker.Pull(signal.GetInternalSignal());
        }
    }

    public T Track<T>(Func<T> trackFn)
    {
        return _tracker.Track(trackFn);
    }

    public void Track(Action trackFn)
    {
        _tracker.Track(trackFn);
    }

    public T Track<T>(Accessor<T> accessor)
    {
        return accessor.InternalSignal is null
            ? _tracker.Track(accessor.RxValueGetter)
            : _tracker.Pull(accessor.InternalSignal);
    }
}

public class Effect : IDisposable
{
    private readonly ScopeNode _scope;
    public bool IsInvalid { get; private set; }

    public Effect(Action executeFn, Scope? scope = null)
    {
        var current = scope?.InnerScopeNode ?? ReactiveContext.CurrentScope;

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

    public Effect(Action<EpochScope> fn, Scope? scope = null) : this(_ => fn, scope)
    {
    }

    public Effect(Func<Scope, Action<EpochScope>> setupFn, Scope? scope = null)
    {
        var current = scope?.InnerScopeNode ?? ReactiveContext.CurrentScope;

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

public class Computed<T> : Signal<T>
{
    private readonly ScopeNode _scope;
    private readonly ComputedNode<T> _computedNode;

    private T _value;

    private readonly Func<T, T> _fn;

    // 缓存是否失效 (不影响依赖建立)
    public bool IsInvalid { get; private set; }

    public override T RxValue
    {
        get
        {
            if (IsInvalid) return _fn(_value!);
            _value = _computedNode.ReadSignal();
            return _value;
        }
    }

    public override T Value
    {
        get
        {
            if (IsInvalid)
            {
                _value = Reactive.Untrack(() => _fn(_value!));
                return _value;
            }

            _value = _computedNode.UntrackValue;
            return _value;
        }
    }

    public Computed(Func<T, T> fn, T? value = default, Scope? scope = null)
    {
        _fn = fn;

        var current = scope?.InnerScopeNode ?? ReactiveContext.CurrentScope;
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

        InternalSignal = _computedNode;

        _computedNode.UpdateComputation();
        _value = _computedNode.UntrackValue;
    }

    public Computed(Func<T> fn, T? value = default, Scope? scope = null)
        : this(_ => fn(), value, scope)
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

public enum AsyncMemoState
{
    Unresolved,
    Pending,
    Ready,
    Refreshing,
    Errored,
    IsInvalid
}

public class AsyncMemo<T> : Signal<T?>
{
    private readonly ScopeNode _scope;
    private EpochScope? _epochScope;
    private readonly bool _isSimpleUse;

    private readonly Func<EpochScope, Task<T>> _executeFn;
    private readonly MutSignal<Exception?> _error = new(null);
    private readonly MutSignal<AsyncMemoState> _state = new(AsyncMemoState.Unresolved);

    private Task<T>? _result;
    private bool _scheduled;
    private bool _resolved;

    public AsyncMemoState RxState => _state.RxValue;

    public override T? RxValue
    {
        get
        {
            var v = InternalSignal.ReadSignal();
            var err = _error.RxValue;
            if (err is not null && _result is null) throw err;
            return v;
        }
    }

    public override T? Value
    {
        get
        {
            var v = InternalSignal.Value;
            var err = _error.Value;
            if (err is not null && _result is null) throw err;
            return v;
        }
    }

    public bool RxLoading
    {
        get
        {
            var state = _state.RxValue;
            return state is AsyncMemoState.Pending or AsyncMemoState.Refreshing;
        }
    }

    public T? RxLatest
    {
        get
        {
            if (!_resolved) return RxValue;
            var err = _error.RxValue;
            if (err is not null && _result is null) throw err;
            return InternalSignal.ReadSignal();
        }
    }

    public Exception? RxError => _error.RxValue;

    public AsyncMemo(Func<Task<T>> executeFn, Scope? scope = null)
    {
        InternalSignal = new SignalState<T?>(default);

        var current = scope?.InnerScopeNode ?? ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => _state.RxValue = AsyncMemoState.IsInvalid);
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

    public AsyncMemo(Func<EpochScope, Task<T>> executeFn, Scope? scope = null) : this(_ => executeFn, scope)
    {
    }

    public AsyncMemo(Func<Scope, Func<EpochScope, Task<T>>> setupFn, Scope? scope = null)
    {
        InternalSignal = new SignalState<T?>(default);

        var current = scope?.InnerScopeNode ?? ReactiveContext.CurrentScope;
        _scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        _scope.Cleanups.Add(() => _state.RxValue = AsyncMemoState.IsInvalid);
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
        _state.RxValue = _resolved ? AsyncMemoState.Refreshing : AsyncMemoState.Pending;
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
            // _value.RxValue = v;
            InternalSignal.WriteSignal(v);
            _state.RxValue = _resolved ? AsyncMemoState.Ready : AsyncMemoState.Unresolved;
        }
        else _state.RxValue = AsyncMemoState.Errored;

        _error.RxValue = err;
        ExecuteNode.EndBatch();
    }
}

public class Selector<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<ExecuteNode>> _subs;
    private readonly Computed<T> _computed;
    private readonly MutSignal<bool> _logicMutSignal = new(true);

    public readonly Func<T, T, bool> CompareFn;

    public Selector(ISignal<T> source, Func<T, T, bool>? compareFn = null)
    {
        CompareFn = compareFn ?? Constant.EqualFn;
        _subs = new Dictionary<T, HashSet<ExecuteNode>>();

        _computed = new Computed<T>(fn: ComputationFn);
        return;

        T ComputationFn(T? prevValue)
        {
            var nextValue = source.RxValue;
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
        _ = _logicMutSignal.RxValue; // 逻辑依赖，不会导致外部观察者误以为没有信号而失效

        var value = _computed.Value;
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