namespace NeHive.Core;

internal interface ISignalState
{
    List<ExecuteNode> Observers { get; }
    List<int> ObserverSlots { get; }
    ExecuteNode? LastTracker { get; set; }
    internal void UpdateIfNeeded(ExecuteNode? ignore = null);
}

internal interface ISignalState<T> : ISignalState
{
    T Value { get; internal set; }
    Func<T, T, bool> Comparator { get; }
    T ReadSignal();
    T WriteSignal(T value);
}

internal static class Common
{
    internal static T BaseReadSignal<T>(ISignalState<T> signal)
    {
        var currentExecute = ReactiveContext.CurrentExecute;
        if (currentExecute is null) return signal.Value;
        return currentExecute.Pull(signal);
    }

    internal static T WriteSignal<T>(ISignalState<T> signal, T value)
    {
        var current = signal.Value;
        if (signal.Comparator(current, value)) return current;

        signal.Value = value;
        if (signal.Observers.Count == 0) return value;

        ExecuteNode.NotifyObservers(signal.Observers);
        return value;
    }
}

internal class SignalState<T>(T value, Func<T, T, bool>? comparator = null) : ISignalState<T>
{
    public List<ExecuteNode> Observers { get; set; } = [];
    public List<int> ObserverSlots { get; } = [];
    public ExecuteNode? LastTracker { get; set; } = null;
    public Func<T, T, bool> Comparator { get; init; } = comparator ?? Constant.EqualFn;
    public T Value { get; set; } = value;

    public T ReadSignal()
        => Common.BaseReadSignal(this);

    public T WriteSignal(T value)
        => Common.WriteSignal(this, value);

    public void UpdateIfNeeded(ExecuteNode? ignore = null)
    {
    }
}

internal class ScopeNode
{
    internal readonly ScopeNode? Parent;
    internal readonly List<ScopeNode> Children;
    internal readonly List<Action> Cleanups;
    internal Dictionary<object, object?>? Context;

    internal ScopeNode(ScopeNode? parent = null,
        List<ScopeNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<object, object?>? context = null)
    {
        Parent = parent ?? ReactiveContext.CurrentScope;
        Children = children ?? [];
        Cleanups = cleanups ?? [];
        Context = context;

        Parent.Children.Add(this);
    }

    internal ScopeNode(bool isRoot)
    {
        _ = isRoot;
        Parent = null;
        Children = [];
        Cleanups = [];
        Context = null;
    }

    internal T RunInScope<T>(Func<T> fn)
        => ReactiveContext.RunInContext(fn, this, null);

    internal void RunInScope(Action fn)
        => ReactiveContext.RunInContext(fn, this, null);

    internal virtual void Dispose()
    {
        var currentComputation = ReactiveContext.CurrentExecute;
        ReactiveContext.CurrentExecute = null;
        try
        {
            DisposeChildren();
            if (Cleanups.Count == 0) return;

            for (var i = Cleanups.Count - 1; i >= 0; i--) Cleanups[i]();
            Cleanups.Clear();
        }
        finally
        {
            ReactiveContext.CurrentExecute = currentComputation;
        }
    }

    internal void DisposeChildren()
    {
        int i;
        for (i = Children.Count - 1; i >= 0; i--)
        {
            var child = Children[i];
            child.Dispose();
        }

        Children.Clear();
    }
}

internal static class ReactiveContext
{
    public static ScopeNode CurrentScope = Constant.RootScopeTree; // 当前正在执行的所有者
    public static ExecuteNode? CurrentExecute; // 当前计算节点
    public static readonly Action BeforeFlush = () => { };
    public static readonly Action AfterFlush = () => { };

    internal static T RunInContext<T>(Func<T> fn, ScopeNode root, ExecuteNode? node)
    {
        var currentComputation = CurrentExecute;
        var currentOwner = CurrentScope;

        CurrentScope = root;
        CurrentExecute = node;

        T result;
        try
        {
            ExecuteNode.StartBatch();
            result = fn();
            ExecuteNode.EndBatch();
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            ExecuteNode.HandleError(err);
            return default!;
        }
        finally
        {
            CurrentExecute = currentComputation;
            CurrentScope = currentOwner;
        }

        return result;
    }

    internal static void RunInContext(Action fn, ScopeNode root, ExecuteNode? node)
    {
        var currentComputation = CurrentExecute;
        var currentOwner = CurrentScope;

        CurrentScope = root;
        CurrentExecute = node;

        try
        {
            ExecuteNode.StartBatch();
            fn();
            ExecuteNode.EndBatch();
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            ExecuteNode.HandleError(err);
        }
        finally
        {
            CurrentExecute = currentComputation;
            CurrentScope = currentOwner;
        }
    }

    internal static T Untrack<T>(Func<T> fn)
    {
        return CurrentExecute is null
            ? fn()
            : RunInContext(fn, CurrentScope, null);
    }

    internal static void Untrack(Action fn)
    {
        if (CurrentExecute is null) fn();
        RunInContext(fn, CurrentScope, null);
    }
}

internal class ReactiveContextHelper : IDisposable
{
    private readonly ExecuteNode? _tempExecute;
    private readonly ScopeNode _tempScope;

    public ReactiveContextHelper(ScopeNode scope, ExecuteNode? tracker)
    {
        _tempExecute = ReactiveContext.CurrentExecute;
        _tempScope = ReactiveContext.CurrentScope;
        ReactiveContext.CurrentScope = scope;
        ReactiveContext.CurrentExecute = tracker;
        ExecuteNode.StartBatch();
    }

    public void Dispose()
    {
        ExecuteNode.EndBatch();
        ReactiveContext.CurrentExecute = _tempExecute;
        ReactiveContext.CurrentScope = _tempScope;
    }
}

internal enum ExecutePhase
{
    Resolved,
    Stale,
    Pending
}

internal interface ITrack
{
    T Pull<T>(ISignalState<T> signal);
    T Track<T>(Func<T> trackFn);
    void Track(Action trackFn);
}

internal abstract class ExecuteNode : ScopeNode, ITrack
{
    internal ExecutePhase Phase;
    internal readonly List<ISignalState> Sources;
    internal readonly List<int> SourceSlots;
    protected long Version;
    public readonly bool IsPure;

    protected ExecuteNode(
        ExecutePhase phase = ExecutePhase.Stale,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long version = 0,
        bool isPure = false,
        ScopeNode? parent = null,
        List<ScopeNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<object, object?>? context = null
    ) : base(parent, children, cleanups, context)
    {
        Phase = phase;
        Sources = sources ?? [];
        SourceSlots = sourceSlots ?? [];
        Version = version;
        IsPure = isPure;
    }

    internal override void Dispose()
    {
        while (Sources.Count != 0)
        {
            var source = Util.RemoveLast(Sources);
            source.LastTracker = null;
            var index = Util.RemoveLast(SourceSlots);
            var obs = source.Observers;

            if (obs.Count == 0) continue;
            var n = Util.RemoveLast(obs);
            var s = Util.RemoveLast(source.ObserverSlots);

            if (index >= obs.Count) continue;
            n.SourceSlots[s] = index;
            obs[index] = n;
            source.ObserverSlots[index] = s;
        }

        Phase = ExecutePhase.Resolved; // 当前及旧的子Computation 不再执行

        base.Dispose();
    }

    // 核心功能入口点
    internal static void NotifyObservers(IEnumerable<ExecuteNode> observers)
    {
        StartBatch();
        foreach (var observer in observers)
        {
            observer.AddQueue();
        }

        if (SchedulerContext.UpdateQueue.Count > 10e5)
        {
            ResetScheduler();
            throw new InfiniteReactiveLoopException("CRITICAL: UpdateQueue Potential Infinite Loop Detected.");
        }

        if (SchedulerContext.EffectQueue.Count > 10e5)
        {
            ResetScheduler();
            throw new InfiniteReactiveLoopException("CRITICAL: EffectQueue Potential Infinite Loop Detected.");
        }

        EndBatch();
    }

    private void AddQueue()
    {
        if (Phase == ExecutePhase.Resolved)
        {
            if (IsPure) SchedulerContext.UpdateQueue.Add(this);
            else SchedulerContext.EffectQueue.Add(this);

            // ComputedNode<T> 的额外行为
            MarkDownstream();
        }

        Phase = ExecutePhase.Stale;
    }

    internal virtual void MarkDownstream()
    {
    }

    public T Pull<T>(ISignalState<T> signal)
    {
        if (signal.LastTracker == this) return signal.Value;
        signal.LastTracker = this;

        // 建立 Computation 与 Signal 的双向引用
        var sSlot = signal.Observers.Count;
        var oSlot = Sources.Count;

        Sources.Add(signal); // 当前计算节点自动收集依赖
        SourceSlots.Add(sSlot);

        signal.Observers.Add(this);
        signal.ObserverSlots.Add(oSlot);
        return signal.Value;
    }

    public void Track(Action trackFn)
        => ReactiveContext.RunInContext(trackFn, this, this);

    public T Track<T>(Func<T> trackFn)
        => ReactiveContext.RunInContext(trackFn, this, this);

    // 调度核心
    protected struct SchedulerContext
    {
        public static int BatchCount; // Batch的重入次数

        public static readonly List<ExecuteNode> EffectQueue = new(256); // 执行副作用的队列
        public static readonly List<ExecuteNode> UpdateQueue = new(256); // 更新值的队列
        public static int UpdateFromIndex; // 队列开始执行的位置

        public static long ExecCount; // 执行计数器，和 ExecuteNode.Version 配合使用
        public static string? Error = null;
    }

    private static void Flush()
    {
        if (SchedulerContext.BatchCount != 1) return;

        var effectFromIndex = 0;
        try
        {
            ReactiveContext.BeforeFlush();
            while (true)
            {
                RunUpdates();

                var e = SchedulerContext.EffectQueue;
                var effectToIndex = e.Count;
                var count = effectToIndex - effectFromIndex;
                if (count == 0) break;

                RunEffectQueue(effectFromIndex, effectToIndex);
                effectFromIndex = effectToIndex;
            }

            ReactiveContext.AfterFlush();
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            HandleError(err);
        }
        finally
        {
            ResetScheduler();
        }
    }

    internal static void StartBatch()
        => SchedulerContext.BatchCount++;

    internal static void EndBatch()
    {
        var batchCount = SchedulerContext.BatchCount;
        if (batchCount <= 0)
        {
            SchedulerContext.BatchCount = 0;
            return;
        }

        if (batchCount > 1)
        {
            SchedulerContext.BatchCount--;
            return;
        }

        // batchCount == 1
        Flush();
        SchedulerContext.BatchCount = 0;
    }

    private static void RunUpdates()
    {
        var queues = SchedulerContext.UpdateQueue;
        var fromIndex = SchedulerContext.UpdateFromIndex;

        if (fromIndex == queues.Count) return;
        SchedulerContext.ExecCount++;

        try
        {
            RunUpdateQueue(fromIndex);
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            HandleError(err);
        }
        finally
        {
            Util.RemoveRangeFrom(queues, fromIndex);
            SchedulerContext.UpdateFromIndex = fromIndex;
        }
    }

    private static void RunUpdateQueue(int fromIndex)
    {
        var queue = SchedulerContext.UpdateQueue;
        for (var i = fromIndex; i < queue.Count; i++)
        {
            var node = queue[i];
            node.RunTop();
        }
    }

    private static void RunEffectQueue(int fromIndex, int toIndex)
    {
        var queue = SchedulerContext.EffectQueue;
        for (var i = fromIndex; i < toIndex; i++)
        {
            var node = queue[i];
            node.RunTop();
        }
    }

    protected void RunTop()
    {
        if (Phase == ExecutePhase.Resolved) return;
        if (Phase == ExecutePhase.Pending)
        {
            LookUpstream();
            return;
        }

        List<ExecuteNode> ancestors = [this];
        var node = this;
        while (true)
        {
            if (node.Parent is not ExecuteNode parent || parent.Version > SchedulerContext.ExecCount) break;
            if (parent.Phase != ExecutePhase.Resolved) ancestors.Add(node);
            node = parent;
        }

        // 从根owner开始执行
        for (var i = ancestors.Count - 1; i >= 0; i--)
        {
            node = ancestors[i];

            if (node.Phase == ExecutePhase.Stale)
                node.UpdateComputation();

            else if (node.Phase == ExecutePhase.Pending)
                RunIsolatedUpdates(() => LookUpstream(ancestors[0]));
        }
    }

    protected static void RunIsolatedUpdates(Action fn)
    {
        var prevIndex = SchedulerContext.UpdateFromIndex;
        SchedulerContext.UpdateFromIndex = SchedulerContext.UpdateQueue.Count;

        try
        {
            fn();
            RunUpdates();
        }
        finally
        {
            SchedulerContext.UpdateFromIndex = prevIndex;
        }
    }

    internal void UpdateComputation()
    {
        Dispose(); // 动态更新依赖，先断开旧依赖
        // 由 ExecuteNode<T> 完成
        RunComputation();
    }

    protected abstract void RunComputation();

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     */
    protected void LookUpstream(ExecuteNode? ignore = null)
    {
        Phase = ExecutePhase.Resolved;
        for (var i = 0; i < Sources.Count; i += 1)
            Sources[i].UpdateIfNeeded();
    }

    internal static void HandleError(Exception err, ScopeNode? owner = null)
    {
        owner ??= ReactiveContext.CurrentScope;
        List<Action<object>>? fns = null;
        if (SchedulerContext.Error is not null)
        {
            fns = owner.Context?[SchedulerContext.Error] as List<Action<object>>;
        }

        var error = CastError(err);
        if (fns is null) throw error;

        if (SchedulerContext.BatchCount > 0)
        {
            ExecuteNode<object> handler = new((_, _) =>
                {
                    RunErrors(error, fns, owner);
                    return Constant.EmptyObj;
                },
                Constant.EmptyObj, isPure: false, phase: ExecutePhase.Stale);
            handler.Phase = ExecutePhase.Stale;
            SchedulerContext.EffectQueue.Add(handler);
        }
        else RunErrors(error, fns, owner);
    }

    internal static Exception CastError(object err)
    {
        if (err is Exception exception) return exception;
        return new Exception(err as string ?? "Unknown error");
    }

    private static void RunErrors(object err, List<Action<object>> fns, ScopeNode? owner)
    {
        try
        {
            foreach (var f in fns)
            {
                f(err);
            }
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception e)
        {
            HandleError(e, owner?.Parent);
        }
    }

    private static void ResetScheduler()
    {
        SchedulerContext.UpdateQueue.Clear();
        SchedulerContext.EffectQueue.Clear();
        SchedulerContext.UpdateFromIndex = 0;
        SchedulerContext.BatchCount = 0;
    }

    // 额外API，非核心算法
}

internal class ExecuteNode<T>(
    Func<ExecuteNode, T, T> fn,
    T value,
    ExecutePhase phase = ExecutePhase.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long version = 0,
    bool isPure = false,
    ScopeNode? parent = null,
    List<ScopeNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<object, object?>? context = null
) : ExecuteNode(phase, sources, sourceSlots,
    version, isPure,
    parent, children, cleanups, context)
{
    public readonly Func<ExecuteNode, T, T> Fn = fn;
    public T Value { get; set; } = value;

    protected virtual void UpdateValue(T value) => Value = value;

    protected override void RunComputation()
    {
        T nextValue;
        var execCount = SchedulerContext.ExecCount;
        try
        {
            nextValue = Fn(this, Value);
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            if (IsPure)
            {
                Phase = ExecutePhase.Stale;
                Children.ForEach(node => node.DisposeChildren());
                Children.Clear();
            }

            // won't be picked up until next update
            Version = execCount + 1;
            HandleError(err);
            return;
        }

        if (Version > execCount) return;
        UpdateValue(nextValue);
        Version = execCount;
    }
}

internal class EffectNode<T> : ExecuteNode<T>
{
    public EffectNode(Func<ExecuteNode, T, T> fn,
        T value) : base(fn, value)
    {
        if (SchedulerContext.BatchCount > 0)
            SchedulerContext.EffectQueue.Add(this);
        else
            UpdateComputation();
    }
}

internal class ComputedNode<T> : ExecuteNode<T>,
    ISignalState<T>
{
    public Func<T, T, bool> Comparator { get; }
    public List<ExecuteNode> Observers { get; }
    public List<int> ObserverSlots { get; }
    public ExecuteNode? LastTracker { get; set; }

    protected override void UpdateValue(T value) => WriteSignal(value);

    internal ComputedNode(
        Func<ITrack, T, T> fn,
        ExecutePhase phase = ExecutePhase.Resolved,
        T? value = default,
        Func<T, T, bool>? comparator = null,
        List<ExecuteNode>? observers = null,
        List<int>? observerSlots = null,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long version = 0,
        ScopeNode? parent = null,
        List<ScopeNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<object, object?>? context = null
    ) : base(fn, value!, phase,
        sources, sourceSlots, version, true,
        parent, children, cleanups, context)
    {
        Comparator = comparator ?? Constant.EqualFn;
        Observers = observers ?? [];
        ObserverSlots = observerSlots ?? [];
        Value = value!;
    }

    /**
     * 递归向下传播脏状态
     */
    internal override void MarkDownstream()
    {
        for (var i = 0; i < Observers.Count; i++)
        {
            var observer = Observers[i];
            if (observer.Phase != ExecutePhase.Resolved) continue;
            observer.Phase = ExecutePhase.Pending;
            if (observer.IsPure) SchedulerContext.UpdateQueue.Add(observer);
            else SchedulerContext.EffectQueue.Add(observer);
            observer.MarkDownstream();
        }
    }

    public void UpdateIfNeeded(ExecuteNode? ignore = null)
    {
        switch (Phase)
        {
            case ExecutePhase.Stale:
                if (this != ignore && Version < SchedulerContext.ExecCount)
                    RunTop();
                break;
            case ExecutePhase.Pending:
                LookUpstream(ignore);
                break;
        }
    }

    private void FlushResult()
    {
        if (Phase == ExecutePhase.Stale) UpdateComputation();
        else
        {
            RunIsolatedUpdates(() => LookUpstream());
        }
    }

    public T ReadSignal()
    {
        FlushResult();
        return Common.BaseReadSignal(this);
    }

    public T WriteSignal(T value)
    {
        return Common.WriteSignal(this, value);
    }

    internal T UntrackValue
    {
        get
        {
            FlushResult();
            return Value;
        }
    }
}