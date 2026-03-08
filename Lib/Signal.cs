namespace Lib;

public delegate TNext EffectFunction<in TPrev, out TNext>(TPrev v) where TNext : TPrev;

public delegate T EffectFunction<T>(T v);

public delegate void EffectFunction();

public delegate T Accessor<out T>();

public delegate T RootFunction<out T>(Action dispose);

public enum ComputationState
{
    Resolved,
    Stale,
    Pending
}

public interface ISignalState
{
    List<IComputationNode>? Observers { get; set; }
    List<int>? ObserverSlots { get; set; }
}

public interface ISignalState<T> : ISignalState
{
    T Value { get; internal set; }
    Func<T, T, bool>? Comparator { get; }
}

public class SignalState<T>(T value) : ISignalState<T>
{
    public List<IComputationNode>? Observers { get; set; }
    public List<int>? ObserverSlots { get; set; }
    public Func<T, T, bool>? Comparator { get; set; }

    public virtual T Value { get; set; } = value;
}

public interface IOwner
{
    IOwner? Parent { get; set; }
    List<IComputationNode>? Children { get; set; }
    List<Action>? Cleanups { get; set; }
    Dictionary<string, object>? Context { get; set; }
}

public class Owner(
    IOwner? parent,
    List<IComputationNode>? children,
    List<Action>? cleanups,
    Dictionary<string, object>? context
) : IOwner
{
    public IOwner? Parent { get; set; } = parent;
    public List<IComputationNode>? Children { get; set; } = children;
    public List<Action>? Cleanups { get; set; } = cleanups;
    public Dictionary<string, object>? Context { get; set; } = context;
}

public interface IComputationNode : IOwner
{
    EffectFunction Fn { get; }
    ComputationState State { get; set; }
    List<ISignalState>? Sources { get; set; }
    List<int>? SourceSlots { get; set; }
    long UpdatedAt { get; set; }
    bool Pure { get; }
    bool User { get; }
}

public interface IComputationNode<TInit> : IComputationNode<TInit, TInit>;

public interface IComputationNode<TInit, TNext> : IComputationNode where TNext : TInit
{
    public new EffectFunction<TInit, TNext> Fn { get; }
    public TInit Value { get; set; }
}

public class ComputationNode(
    EffectFunction fn,
    ComputationState state = ComputationState.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    IOwner? parent = null,
    List<IComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object>? context = null
) : IComputationNode
{
    public EffectFunction Fn { get; } = fn;
    public ComputationState State { get; set; } = state;
    public List<ISignalState>? Sources { get; set; } = sources;
    public List<int>? SourceSlots { get; set; } = sourceSlots;
    public long UpdatedAt { get; set; } = updatedAt;
    public bool Pure { get; } = pure;
    public bool User { get; set; } = user;
    public IOwner? Parent { get; set; } = parent;
    public List<IComputationNode>? Children { get; set; } = children;
    public List<Action>? Cleanups { get; set; } = cleanups;
    public Dictionary<string, object>? Context { get; set; } = context;
}

public class ComputationNode<T>(
    EffectFunction<T, T> fn,
    T value,
    ComputationState state = ComputationState.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    IOwner? parent = null,
    List<IComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object>? context = null
) : ComputationNode(Constant.EmptyEffectFunction, state, null, sourceSlots,
        updatedAt, pure, user, parent, children, cleanups, context),
    IComputationNode<T>
{
    public new EffectFunction<T, T> Fn { get; } = fn;
    public T Value { get; set; } = value;
    public new List<ISignalState>? Sources { get; set; } = sources;
}

public class Computation<T> : ComputationNode<T>
{
    public Computation(
        EffectFunction<T, T> fn,
        T value,
        ComputationState state = ComputationState.Stale,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long updatedAt = 0,
        bool pure = false,
        bool user = false,
        IOwner? parent = null,
        List<IComputationNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<string, object>? context = null
    ) : base(fn, value, state, sources, sourceSlots, updatedAt, pure, user, parent, children, cleanups, context)
    {
        if (Lib.Context.CurrentOwner is null)
            Console.WriteLine(
                "computations created outside a `createRoot` or `render` will never be disposed"
            );
        else if (Lib.Context.CurrentOwner != Constant.UnOwned)
        {
            if (Lib.Context.CurrentOwner.Children is null)
                Lib.Context.CurrentOwner.Children = [this];
            else Lib.Context.CurrentOwner.Children.Add(this);
        }
    }
}

// public delegate V UntrackFunc<V>();

public interface IMemo : ISignalState, IComputationNode;

public interface IMemo<TPrev, TNext> : IMemo, ISignalState<TNext>, IComputationNode<TNext> where TNext : TPrev;

public interface IMemo<TPrev> : IMemo<TPrev, TPrev>;

public class Memo<T>(
    EffectFunction<T, T> fn,
    ComputationState state = ComputationState.Resolved,
    T? value = default,
    Func<T, T, bool>? comparator = null,
    List<IComputationNode>? observers = null,
    List<int>? observerSlots = null,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    IOwner? parent = null,
    List<IComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object>? context = null
) : Computation<T>(fn, value!, state, sources, sourceSlots, updatedAt, pure, user, parent, children, cleanups, context),
    IMemo<T>
{
    EffectFunction IComputationNode.Fn { get; } = Constant.EmptyEffectFunction;
    public Func<T, T, bool>? Comparator { get; } = comparator;
    public List<IComputationNode>? Observers { get; set; } = observers;
    public List<int>? ObserverSlots { get; set; } = observerSlots;
}

internal static class Context
{
    public static IOwner? CurrentOwner; // 当前正在执行的所有者
    public static IComputationNode? CurrentComputation; // 当前计算节点
    public static List<IComputationNode>? UpdateQueue; // 更新值的队列
    public static List<IComputationNode>? EffectQueue; // 执行副作用的队列
    public static long ExecCount; // 执行计数器，和ComputationNode.updatedAt 配合使用
    public static string? Error = null;
}

internal static class Constant
{
    public static readonly Owner UnOwned = new(
        parent: null,
        children: null,
        context: null,
        cleanups: null
    );

    public static bool EqualFn<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    public static readonly object EmptyObj = new { };
    public static readonly EffectFunction EmptyEffectFunction = () => { };
}

internal static class Util
{
    public static T RemoveLast<T>(List<T> list)
    {
        var count = list.Count;
        var value = list[count - 1];
        list.RemoveAt(count - 1);
        return value;
    }
}

public interface IReadOnlySignal<T>
{
    public T Value { get; }
}

public interface ISignal<T> : IReadOnlySignal<T>
{
    public new T Value { get; set; }
}

public class Signal<T>(T value) : ISignal<T>
{
    private readonly SignalState<T> _state = new(value);

    public T Value
    {
        get => Reactive.ReadSignal(_state);
        set => Reactive.WriteSignal(_state, value);
    }

    public void SetValue(T value)
    {
        Reactive.WriteSignal(_state, value);
    }

    public void SetValue(Func<T, T> value)
    {
        Reactive.WriteSignal(_state, value(_state.Value));
    }
}

public class ReadOnlySignal<T> : IReadOnlySignal<T>
{
    private readonly Memo<T> _state;

    public T Value => Reactive.ReadSignal(_state);

    public ReadOnlySignal(EffectFunction<T, T> fn)
    {
        _state = new Memo<T>(
            fn,
            comparator: Constant.EqualFn
        )
        {
            Observers = null,
            ObserverSlots = null,
        };
        Reactive.UpdateComputation(_state);
    }
}

public static partial class Reactive
{
    public static Signal<T> CreateSignal<T>(T value)
    {
        return new Signal<T>(value);
    }


    public static IComputationNode<object> CreateComputation(
        Action fn,
        bool pure = false,
        ComputationState state = ComputationState.Stale)
    {
        return new Computation<object>(_ =>
        {
            fn();
            return Constant.EmptyObj;
        }, Constant.EmptyObj, pure: pure, state: state);
    }

    public static IComputationNode<T> CreateComputation<T>(
        EffectFunction<T, T> fn,
        T init,
        bool pure = false,
        ComputationState state = ComputationState.Stale)
    {
        return new Computation<T>(fn, init, pure: pure, state: state);
    }

    public static void CreateEffect(EffectFunction fn)
    {
        CreateEffect(_ =>
        {
            fn();
            return Constant.EmptyObj;
        }, Constant.EmptyObj);
    }

    public static void CreateEffect<T>(
        EffectFunction<T, T> fn,
        T? value = default
    )
    {
        var c = new Computation<T>(fn, value!);

        c.User = true;
        if (Context.EffectQueue is null)
        {
            UpdateComputation(c);
        }
        else
        {
            Context.EffectQueue.Add(c);
        }
    }

    public static ReadOnlySignal<T> CreateMemo<T>(EffectFunction<T, T> fn)
    {
        return new ReadOnlySignal<T>(fn);
    }

    internal static T ReadSignal<T>(ISignalState<T> signalState)
    {
        if (signalState is IMemo<T> memo)
        {
            if (memo.State == ComputationState.Stale)
            {
                UpdateComputation(memo);
            }
            else
            {
                var updates = Context.UpdateQueue;
                Context.UpdateQueue = null;
                RunUpdates(() =>
                {
                    LookUpstream(memo);
                    return Constant.EmptyObj;
                }, false);
                Context.UpdateQueue = updates;
            }
        }

        var currentComp = Context.CurrentComputation;
        if (currentComp is null) return signalState.Value;
        // 建立Computation与Signal的双向引用
        var sSlot = signalState.Observers?.Count ?? 0;
        var oSlot = currentComp.Sources?.Count ?? 0;
        if (currentComp.Sources is not null)
        {
            currentComp.Sources.Add(signalState); // 当前计算节点自动收集依赖
            currentComp.SourceSlots!.Add(sSlot);
        }
        else
        {
            currentComp.Sources = [signalState];
            currentComp.SourceSlots = [sSlot];
        }

        if (signalState.Observers is not null)
        {
            signalState.Observers.Add(currentComp);
            signalState.ObserverSlots!.Add(oSlot);
        }
        else
        {
            signalState.Observers = [currentComp];
            signalState.ObserverSlots = [oSlot];
        }

        return signalState.Value;
    }

    internal static T WriteSignal<T>(ISignalState<T> node, T value)
    {
        var current = node.Value;

        if (node.Comparator?.Invoke(current, value) ?? false) return default!;
        node.Value = value;

        if (node.Observers is null) return default!;
        RunUpdates(() =>
        {
            foreach (var observer in node.Observers)
            {
                if (observer.State == ComputationState.Resolved)
                {
                    if (observer.Pure) Context.UpdateQueue!.Add(observer);
                    else Context.EffectQueue!.Add(observer);
                    if (observer is IMemo memo) MarkDownstream(memo);
                }

                observer.State = ComputationState.Stale;
            }

            if (Context.UpdateQueue!.Count > 10e5)
            {
                Context.UpdateQueue = [];
                throw new Exception("Potential Infinite Loop Detected.");
            }

            return Constant.EmptyObj;
        }, false);
        return value;
    }

    public static T Batch<T>(Accessor<T> fn)
    {
        return RunUpdates(fn, false);
    }

    public static T CreateRoot<T>(RootFunction<T> fn, Owner? detachedOwner = null)
    {
        var currentComputation = Context.CurrentComputation;
        var currentOwner = Context.CurrentOwner;
        var unDispose = fn.Method.GetParameters().Length == 0;
        var current = detachedOwner ?? currentOwner;
        var root = unDispose
            ? Constant.UnOwned
            : new Owner(
                parent: current,
                children: null,
                context: current?.Context,
                cleanups: null
            );
        Accessor<T> updateFn;
        if (unDispose)
        {
            updateFn = () =>
                fn(() =>
                {
                    throw new Exception("Dispose method must be an explicit argument to createRoot function");
                });
        }
        else
        {
            updateFn = () => fn(() => Untrack(() => CleanNode(root)));
        }

        Context.CurrentOwner = root;
        Context.CurrentComputation = null;

        try
        {
            return RunUpdates(updateFn, true)!;
        }
        finally
        {
            Context.CurrentComputation = currentComputation;
            Context.CurrentOwner = currentOwner;
        }
    }

    public static void OnMount(Action fn)
    {
        CreateEffect(() => Untrack(fn));
    }

    public static Action OnCleanup(Action fn)
    {
        if (Context.CurrentOwner is null)
            Console.WriteLine("cleanups created outside a `createRoot` or `render` will never be run");
        else if (Context.CurrentOwner.Cleanups is null) Context.CurrentOwner.Cleanups = [fn];
        else Context.CurrentOwner.Cleanups.Add(fn);
        return fn;
    }

    private static void RunUpdates(Action fn, bool init)
    {
        RunUpdates(() =>
        {
            fn();
            return Constant.EmptyObj;
        }, init);
    }

    private static T RunUpdates<T>(Accessor<T> fn, bool init)
    {
        if (Context.UpdateQueue is not null) return fn();
        var wait = false;
        if (!init) Context.UpdateQueue = [];
        if (Context.EffectQueue is not null) wait = true;
        else Context.EffectQueue = [];
        Context.ExecCount++;
        try
        {
            var res = fn();
            CompleteUpdates(wait);
            return res;
        }
        catch (Exception err)
        {
            if (!wait) Context.EffectQueue = null;
            Context.UpdateQueue = null;
            HandleError(err);
            return default!;
        }
    }

    private static void CompleteUpdates(bool wait)
    {
        if (Context.UpdateQueue is not null)
        {
            RunQueue(Context.UpdateQueue);
            Context.UpdateQueue = null;
        }

        if (wait) return;
        var e = Context.EffectQueue!;
        Context.EffectQueue = null;
        if (e.Count != 0)
            RunUpdates(() =>
            {
                RunUserEffects(e);
                return Constant.EmptyObj;
            }, false);
    }

    private static void RunQueue(ICollection<IComputationNode> queues)
    {
        foreach (var queue in queues)
        {
            RunTop(queue);
        }
    }

    private static void RunUserEffects(List<IComputationNode> queue)
    {
        var userLength = 0;
        for (var i = 0; i < queue.Count; i++)
        {
            var e = queue[i];
            if (!e.User) RunTop(e);
            else queue[userLength++] = e;
        }

        for (var i = 0; i < userLength; i++) RunTop(queue[i]);
    }

    private static void RunTop(IComputationNode node)
    {
        if (node.State == ComputationState.Resolved) return;
        if (node.State == ComputationState.Pending)
        {
            LookUpstream(node);
            return;
        }

        List<IComputationNode> ancestors = [node];
        while (true)
        {
            if (!(node.Parent is IComputationNode parent && parent.UpdatedAt <= Context.ExecCount)) break;
            if (parent.State != ComputationState.Resolved) ancestors.Add(node);
            node = parent;
        }

        // 从根owner开始执行
        for (var i = ancestors.Count - 1; i >= 0; i--)
        {
            node = ancestors[i];

            if (node.State == ComputationState.Stale)
            {
                UpdateComputation(node);
            }
            else if (node.State == ComputationState.Pending)
            {
                var updates = Context.UpdateQueue;
                Context.UpdateQueue = null;
                RunUpdates(() => LookUpstream(node, ancestors[0]), false);
                Context.UpdateQueue = updates;
            }
        }
    }

    internal static void UpdateComputation(IComputationNode node)
    {
        CleanNode(node); // 动态依赖切换，先断开旧依赖
        var time = Context.ExecCount;
        RunComputation(time, node);
    }

    private static void RunComputation(long time, IComputationNode node)
    {
        object? nextValue;
        var tempOwner = Context.CurrentOwner;
        var tempComputation = Context.CurrentComputation;
        Context.CurrentOwner = Context.CurrentComputation = node;
        try
        {
            nextValue = node switch
            {
                IComputationNode<object> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<long> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<int> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<short> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<byte> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<char> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<string> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<double> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<float> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<decimal> nodeV => nodeV.Fn(nodeV.Value),
                IComputationNode<bool> nodeV => nodeV.Fn(nodeV.Value),
                _ => null
            };
        }
        catch (Exception err)
        {
            if (node.Pure)
            {
                node.State = ComputationState.Stale;
                node.Children?.ForEach(CleanNode);
                node.Children = null;
            }

            // won't be picked up until next update
            node.UpdatedAt = time + 1;
            HandleError(err);
            return;
        }
        finally
        {
            Context.CurrentComputation = tempComputation;
            Context.CurrentOwner = tempOwner;
        }

        if (node.UpdatedAt > time) return;
        switch (node)
        {
            case IComputationNode<object> nodeV:
                if (node is IMemo<object> memo1) WriteSignal(memo1, nextValue!);
                else nodeV.Value = nextValue!;
                break;
            case IComputationNode<long> nodeV:
                if (node is IMemo<long> memo2) WriteSignal(memo2, (long)nextValue!);
                else nodeV.Value = (long)nextValue!;
                break;
            case IComputationNode<int> nodeV:
                if (node is IMemo<int> memo3) WriteSignal(memo3, (int)nextValue!);
                else nodeV.Value = (int)nextValue!;
                break;
            case IComputationNode<short> nodeV:
                if (node is IMemo<short> memo4) WriteSignal(memo4, (short)nextValue!);
                else nodeV.Value = (short)nextValue!;
                break;
            case IComputationNode<byte> nodeV:
                if (node is IMemo<byte> memo5) WriteSignal(memo5, (byte)nextValue!);
                else nodeV.Value = (byte)nextValue!;
                break;
            case IComputationNode<char> nodeV:
                if (node is IMemo<char> memo6) WriteSignal(memo6, (char)nextValue!);
                else nodeV.Value = (char)nextValue!;
                break;
            case IComputationNode<string> nodeV:
                if (node is IMemo<string> memo7) WriteSignal(memo7, (string)nextValue!);
                else nodeV.Value = (string)nextValue!;
                break;
            case IComputationNode<double> nodeV:
                if (node is IMemo<double> memo8) WriteSignal(memo8, (double)nextValue!);
                else nodeV.Value = (double)nextValue!;
                break;
            case IComputationNode<float> nodeV:
                if (node is IMemo<float> memo9) WriteSignal(memo9, (float)nextValue!);
                else nodeV.Value = (float)nextValue!;
                break;
            case IComputationNode<decimal> nodeV:
                if (node is IMemo<decimal> memo10) WriteSignal(memo10, (decimal)nextValue!);
                else nodeV.Value = (decimal)nextValue!;
                break;
            case IComputationNode<bool> nodeV:
                if (node is IMemo<bool> memo11) WriteSignal(memo11, (bool)nextValue!);
                else nodeV.Value = (bool)nextValue!;
                break;
        }

        node.UpdatedAt = time;
    }

    /**
     * 递归向下传播脏状态
     * @param node
     */
    private static void MarkDownstream(IMemo node)
    {
        foreach (var observer in node.Observers!)
        {
            if (observer.State != ComputationState.Resolved) continue;
            observer.State = ComputationState.Pending;
            if (observer.Pure) Context.UpdateQueue!.Add(observer);
            else Context.EffectQueue!.Add(observer);
            if (observer is not IMemo memo) continue;
            MarkDownstream(memo);
        }
    }

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     * @param node
     * @param ignore
     */
    private static void LookUpstream(IComputationNode node, IComputationNode? ignore = null)
    {
        node.State = ComputationState.Resolved;
        for (var i = 0; i < node.Sources!.Count; i += 1)
        {
            if (node.Sources![i] is not IMemo source) continue;
            var state = source.State;
            switch (state)
            {
                case ComputationState.Stale:
                    if (source != ignore && (source.UpdatedAt < Context.ExecCount))
                        RunTop(source);
                    break;
                case ComputationState.Pending:
                    LookUpstream(source, ignore);
                    break;
            }
        }
    }

    public static T Untrack<T>(Accessor<T> fn)
    {
        if (Context.CurrentComputation is null) return fn();

        var currentComputation = Context.CurrentComputation;
        Context.CurrentComputation = null;
        try
        {
            return fn();
        }
        finally
        {
            Context.CurrentComputation = currentComputation;
        }
    }

    public static void Untrack(Action fn)
    {
        Untrack(() =>
        {
            fn();
            return Constant.EmptyObj;
        });
    }

    private static void CleanNode(IOwner node)
    {
        int i;
        if (node is IComputationNode computationNode)
        {
            CleanComputationNode(computationNode);
            CommonCode();
            computationNode.State = ComputationState.Resolved; // 当前及旧的子Computation已解决，旧的子Computation不再执行
            return;
        }

        CommonCode();
        return;

        void CommonCode()
        {
            if (node.Children is not null)
            {
                for (i = node.Children.Count - 1; i >= 0; i--) CleanNode(node.Children[i]);
                node.Children = null;
            }

            if ((node.Cleanups?.Count ?? 0) == 0) return;

            for (i = node.Cleanups!.Count - 1; i >= 0; i--) node.Cleanups[i]();
            node.Cleanups = null;
        }
    }

    private static void CleanComputationNode(IComputationNode node)
    {
        while ((node.Sources?.Count ?? 0) != 0)
        {
            var source = Util.RemoveLast(node.Sources!);
            var index = Util.RemoveLast(node.SourceSlots!);
            var obs = source.Observers;

            if (obs is null) continue;
            var n = Util.RemoveLast(obs);
            var s = Util.RemoveLast(source.ObserverSlots!);

            if (index >= obs.Count) continue;
            n.SourceSlots![s] = index;
            obs[index] = n;
            source.ObserverSlots![index] = s;
        }
    }

    private static void HandleError(Exception err, IOwner? owner = null)
    {
        owner ??= Context.CurrentOwner;
        ICollection<Action<object>>? fns = null;
        if (Context.Error is not null)
        {
            fns = owner?.Context?[Context.Error] as ICollection<Action<object>>;
        }

        var error = CastError(err);
        if (fns is null) throw error;

        if (Context.EffectQueue is not null)
        {
            var handler = CreateComputation(() => RunErrors(error, fns, owner));
            handler.State = ComputationState.Stale;
            Context.EffectQueue.Add(handler);
        }
        else RunErrors(error, fns, owner);
    }

    private static Exception CastError(object err)
    {
        if (err is Exception exception) return exception;
        return new Exception(err is string errorStr ? errorStr : "Unknown error");
    }

    private static void RunErrors(object err, ICollection<Action<object>> fns, IOwner? owner)
    {
        try
        {
            foreach (var f in fns)
            {
                f(err);
            }
        }
        catch (Exception e)
        {
            HandleError(e, owner?.Parent);
        }
    }
}