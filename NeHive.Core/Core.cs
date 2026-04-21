namespace NeHive.Core;

internal interface ISignalState
{
    List<ComputationNode> Observers { get; }
    List<int> ObserverSlots { get; }
    ComputationNode? LastObserver { get; set; }
    internal void UpdateIfNeeded(ComputationNode? ignore = null);
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
        var currentComp = ReactiveContext.CurrentComputation;
        if (currentComp is null || signal.LastObserver == currentComp) return signal.Value;
        signal.LastObserver = currentComp;

        // 建立 Computation 与 Signal 的双向引用
        var sSlot = signal.Observers.Count;
        var oSlot = currentComp.Sources.Count;

        currentComp.Sources.Add(signal); // 当前计算节点自动收集依赖
        currentComp.SourceSlots.Add(sSlot);

        signal.Observers.Add(currentComp);
        signal.ObserverSlots.Add(oSlot);

        return signal.Value;
    }

    internal static T WriteSignal<T>(ISignalState<T> signal, T value)
    {
        var current = signal.Value;
        if (signal.Comparator(current, value)) return current;

        signal.Value = value;
        if (signal.Observers.Count == 0) return value;

        ComputationNode.NotifyObservers(signal.Observers);
        return value;
    }
}

internal class SignalState<T>(T value, Func<T, T, bool>? comparator = null) : ISignalState<T>
{
    public List<ComputationNode> Observers { get; set; } = [];
    public List<int> ObserverSlots { get; } = [];
    public ComputationNode? LastObserver { get; set; } = null;
    public Func<T, T, bool> Comparator { get; init; } = comparator ?? Constant.EqualFn;
    public T Value { get; set; } = value;

    public T ReadSignal()
        => Common.BaseReadSignal(this);

    public T WriteSignal(T value)
        => Common.WriteSignal(this, value);

    public void UpdateIfNeeded(ComputationNode? ignore = null)
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

    internal virtual void Dispose()
    {
        var currentComputation = ReactiveContext.CurrentComputation;
        ReactiveContext.CurrentComputation = null;
        try
        {
            DisposeChildren();
            if (Cleanups.Count == 0) return;

            for (var i = Cleanups.Count - 1; i >= 0; i--) Cleanups[i]();
            Cleanups.Clear();
        }
        finally
        {
            ReactiveContext.CurrentComputation = currentComputation;
        }
    }
    
    internal virtual void DisposeChildren()
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
    public static ComputationNode? CurrentComputation; // 当前计算节点
    public static readonly Action BeforeFlush = () => { };
    public static readonly Action AfterFlush = () => { };
}

internal enum ComputationPhase
{
    Resolved,
    Stale,
    Pending
}

internal abstract class ComputationNode : ScopeNode
{
    internal ComputationPhase Phase;
    internal readonly List<ISignalState> Sources;
    internal readonly List<int> SourceSlots;
    protected long Version;
    public readonly bool IsPure;
    public readonly bool IsUser;

    protected ComputationNode(
        ComputationPhase phase = ComputationPhase.Stale,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long version = 0,
        bool isPure = false,
        bool isUser = false,
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
        IsUser = isUser;
    }

    internal override void Dispose()
    {
        while (Sources.Count != 0)
        {
            var source = Util.RemoveLast(Sources);
            source.LastObserver = null;
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
        Phase = ComputationPhase.Resolved; // 当前及旧的子Computation 不再执行

        base.Dispose();
    }

    // 核心功能入口点
    internal static void NotifyObservers(IEnumerable<ComputationNode> observers)
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
        if (Phase == ComputationPhase.Resolved)
        {
            if (IsPure) SchedulerContext.UpdateQueue.Add(this);
            else SchedulerContext.EffectQueue.Add(this);

            // MemoNode<T> 的额外行为
            MarkDownstream();
        }

        Phase = ComputationPhase.Stale;
    }

    internal virtual void MarkDownstream()
    {
    }

    // 调度核心
    protected struct SchedulerContext
    {
        public static int BatchCount; // Batch的重入次数

        public static readonly List<ComputationNode> EffectQueue = new(256); // 执行副作用的队列
        public static readonly List<ComputationNode> UpdateQueue = new(256); // 更新值的队列
        public static int UpdateFromIndex; // 队列开始执行的位置

        public static long ExecCount; // 执行计数器，和 ComputationNode.Version 配合使用
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
        if (Phase == ComputationPhase.Resolved) return;
        if (Phase == ComputationPhase.Pending)
        {
            LookUpstream();
            return;
        }

        List<ComputationNode> ancestors = [this];
        var node = this;
        while (true)
        {
            if (node.Parent is not ComputationNode parent || parent.Version > SchedulerContext.ExecCount) break;
            if (parent.Phase != ComputationPhase.Resolved) ancestors.Add(node);
            node = parent;
        }

        // 从根owner开始执行
        for (var i = ancestors.Count - 1; i >= 0; i--)
        {
            node = ancestors[i];

            if (node.Phase == ComputationPhase.Stale)
                node.UpdateComputation();

            else if (node.Phase == ComputationPhase.Pending)
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
        // 由 ComputationNode<T> 完成
        RunComputation();
    }

    protected abstract void RunComputation();

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     */
    protected void LookUpstream(ComputationNode? ignore = null)
    {
        Phase = ComputationPhase.Resolved;
        for (var i = 0; i < Sources.Count; i += 1)
            Sources[i].UpdateIfNeeded();
    }

    protected static void HandleError(Exception err, ScopeNode? owner = null)
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
            ComputationNode<object> handler = new(Util.WrapActionWithArg(() => RunErrors(error, fns, owner)),
                Constant.EmptyObj, isPure: false, phase: ComputationPhase.Stale);
            handler.Phase = ComputationPhase.Stale;
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
    internal static T SetContext<T>(Func<T> fn, ScopeNode root, ComputationNode? node)
    {
        var currentComputation = ReactiveContext.CurrentComputation;
        var currentOwner = ReactiveContext.CurrentScope;

        ReactiveContext.CurrentScope = root;
        ReactiveContext.CurrentComputation = node;

        T result;
        try
        {
            result = fn();
        }
        finally
        {
            ReactiveContext.CurrentComputation = currentComputation;
            ReactiveContext.CurrentScope = currentOwner;
        }

        return result;
    }

    internal static T Untrack<T>(Func<T> fn)
    {
        if (ReactiveContext.CurrentComputation is null) return fn();

        var currentComputation = ReactiveContext.CurrentComputation;
        ReactiveContext.CurrentComputation = null;
        try
        {
            return fn();
        }
        finally
        {
            ReactiveContext.CurrentComputation = currentComputation;
        }
    }

    internal static void Untrack(Action fn)
        => Untrack(Util.WrapAction(fn));

    internal static T Untrack<T>(IReadOnlySignal<T> source)
        => Untrack(() => source.Value);

    internal static T RunInScope<T>(ScopeNode scope, Func<T> fn)
    {
        var tempOwner = ReactiveContext.CurrentScope;
        var tempComputation = ReactiveContext.CurrentComputation;
        ReactiveContext.CurrentScope = scope;
        ReactiveContext.CurrentComputation = null;
        try
        {
            StartBatch();
            var result = fn();
            EndBatch();
            return result;
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            HandleError(err);
            return default!;
        }
        finally
        {
            ReactiveContext.CurrentScope = tempOwner;
            ReactiveContext.CurrentComputation = tempComputation;
        }
    }
}

internal class ComputationNode<T>(
    Func<T, T> fn,
    T value,
    ComputationPhase phase = ComputationPhase.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long version = 0,
    bool isPure = false,
    bool isUser = false,
    ScopeNode? parent = null,
    List<ScopeNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<object, object?>? context = null
) : ComputationNode(phase, sources, sourceSlots,
    version, isPure, isUser,
    parent, children, cleanups, context)
{
    public readonly Func<T, T> Fn = fn;
    public T Value { get; set; } = value;

    protected virtual void UpdateValue(T value) => Value = value;

    protected override void RunComputation()
    {
        T nextValue;
        var execCount = SchedulerContext.ExecCount;
        var tempOwner = ReactiveContext.CurrentScope;
        var tempComputation = ReactiveContext.CurrentComputation;
        ReactiveContext.CurrentScope = ReactiveContext.CurrentComputation = this;
        try
        {
            nextValue = Fn(Value);
        }
        catch (InfiniteReactiveLoopException)
        {
            throw;
        }
        catch (Exception err)
        {
            if (IsPure)
            {
                Phase = ComputationPhase.Stale;
                Children.ForEach(node => node.DisposeChildren());
                Children.Clear();
            }

            // won't be picked up until next update
            Version = execCount + 1;
            HandleError(err);
            return;
        }
        finally
        {
            ReactiveContext.CurrentComputation = tempComputation;
            ReactiveContext.CurrentScope = tempOwner;
        }

        if (Version > execCount) return;
        UpdateValue(nextValue);
        Version = execCount;
    }
}

internal class EffectNode<T> : ComputationNode<T>
{
    public EffectNode(Func<T, T> fn,
        T value) : base(fn, value, isUser: true)
    {
        if (SchedulerContext.BatchCount > 0)
            SchedulerContext.EffectQueue.Add(this);
        else
            UpdateComputation();
    }
}

internal class MemoNode<T> : ComputationNode<T>,
    ISignalState<T>
{
    public Func<T, T, bool> Comparator { get; }
    public List<ComputationNode> Observers { get; }
    public List<int> ObserverSlots { get; }
    public ComputationNode? LastObserver { get; set; }

    // private T _value;

    protected override void UpdateValue(T value) => WriteSignal(value);

    internal MemoNode(
        Func<T, T> fn,
        ComputationPhase phase = ComputationPhase.Resolved,
        T? value = default,
        Func<T, T, bool>? comparator = null,
        List<ComputationNode>? observers = null,
        List<int>? observerSlots = null,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long version = 0,
        bool isPure = true,
        bool isUser = false,
        ScopeNode? parent = null,
        List<ScopeNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<object, object?>? context = null
    ) : base(fn, value!, phase,
        sources, sourceSlots, version, isPure, isUser,
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
            if (observer.Phase != ComputationPhase.Resolved) continue;
            observer.Phase = ComputationPhase.Pending;
            if (observer.IsPure) SchedulerContext.UpdateQueue.Add(observer);
            else SchedulerContext.EffectQueue.Add(observer);
            observer.MarkDownstream();
        }
    }

    public void UpdateIfNeeded(ComputationNode? ignore = null)
    {
        switch (Phase)
        {
            case ComputationPhase.Stale:
                if (this != ignore && Version < SchedulerContext.ExecCount)
                    RunTop();
                break;
            case ComputationPhase.Pending:
                LookUpstream(ignore);
                break;
        }
    }

    private void FlushResult()
    {
        if (Phase == ComputationPhase.Stale) UpdateComputation();
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