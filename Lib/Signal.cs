namespace Lib;
//
// using Transition = (Func<bool>, Func<Action, Task>);

public delegate TNext EffectFunction<in TPrev, out TNext>(TPrev v) where TNext : TPrev;

public delegate TPrev EffectFunction<TPrev>(TPrev v);

public delegate T Accessor<out T>();

public delegate T RootFunction<out T>(Action dispose);

public enum ComputationState
{
    Resolved,
    Stale,
    Pending
}

public interface ISignalState<T>
{
    T Value { get; internal set; }
    List<IComputationNode<object>>? Observers { get; set; }
    List<int>? ObserverSlots { get; set; }
    Func<T, T, bool>? Comparator { get; set; }
}

public class SignalState<T>(T value) : ISignalState<T>
{
    public List<IComputationNode<object>>? Observers { get; set; }
    public List<int>? ObserverSlots { get; set; }
    public Func<T, T, bool>? Comparator { get; set; }

    public virtual T Value { get; set; } = value;
}

public interface IOwner
{
    IOwner? Parent { get; set; }
    List<IComputationNode<object>>? Children { get; set; }
    List<Action>? Cleanups { get; set; }
    Dictionary<string, object>? Context { get; set; }
}

public class Owner(
    IOwner? parent,
    List<IComputationNode<object>>? children,
    List<Action>? cleanups,
    Dictionary<string, object>? context
) : IOwner
{
    public IOwner? Parent { get; set; } = parent;
    public List<IComputationNode<object>>? Children { get; set; } = children;
    public List<Action>? Cleanups { get; set; } = cleanups;
    public Dictionary<string, object>? Context { get; set; } = context;
}

public interface IComputationNode<TInit> : IComputationNode<TInit, TInit>;

public interface IComputationNode<TInit, TNext> : IOwner where TNext : TInit
{
    public EffectFunction<TInit, TNext> Fn { get; }
    public ComputationState State { get; set; }
    public List<ISignalState<TInit>>? Sources { get; set; }
    public List<int>? SourceSlots { get; set; }
    public TInit Value { get; set; }
    public long UpdatedAt { get; set; }
    public bool Pure { get; }
    public bool User { get; }
}

public class ComputationNode<T>(
    EffectFunction<T, T> fn,
    T value,
    ComputationState state = ComputationState.Stale,
    List<ISignalState<T>>? sources = null,
    List<int>? sourceSlots = null,
    int updatedAt = 0,
    bool pure = false,
    bool user = false,
    IOwner? parent = null,
    List<IComputationNode<object>>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object>? context = null
) : IComputationNode<T>
{
    public EffectFunction<T, T> Fn { get; } = fn;
    public ComputationState State { get; set; } = state;
    public List<ISignalState<T>>? Sources { get; set; } = sources;
    public List<int>? SourceSlots { get; set; } = sourceSlots;
    public long UpdatedAt { get; set; } = updatedAt;
    public bool Pure { get; } = pure;
    public bool User { get; set; } = user;
    public IOwner? Parent { get; set; } = parent;
    public List<IComputationNode<object>>? Children { get; set; } = children;
    public List<Action>? Cleanups { get; set; } = cleanups;
    public Dictionary<string, object>? Context { get; set; } = context;
    public T Value { get; set; } = value;
}

public class Computation<T> : ComputationNode<T>
{
    public Computation(
        EffectFunction<T, T> fn,
        T init,
        bool pure = false,
        ComputationState state = ComputationState.Stale
    ) : base(fn: fn, value: init, state: state, pure: pure, parent: Lib.Context.CurrentOwner,
        context: Lib.Context.CurrentOwner?.Context)
    {
        if (Lib.Context.CurrentOwner is null)
            Console.WriteLine(
                "computations created outside a `createRoot` or `render` will never be disposed"
            );
        else if (Lib.Context.CurrentOwner != Constant.UnOwned)
        {
            if (Lib.Context.CurrentOwner.Children is null)
                Lib.Context.CurrentOwner.Children = [(IComputationNode<object>)this];
            else Lib.Context.CurrentOwner.Children.Add((IComputationNode<object>)this);
        }
    }
}

public delegate V UntrackFunc<V>();

public interface IMemo<TPrev> : IMemo<TPrev, TPrev>;

public interface IMemo<TPrev, TNext> : ISignalState<TNext>, IComputationNode<TNext> where TNext : TPrev;

internal static class Context
{
    public static IOwner? CurrentOwner; // 当前正在执行的所有者
    public static IComputationNode<object>? CurrentComputation; // 当前计算节点
    public static Action<List<IComputationNode<object>>> RunEffects = Reactive.RunQueue; // 副作用队列执行函数
    public static List<IComputationNode<object>>? UpdateQueue; // 更新值的队列
    public static List<IComputationNode<object>>? EffectQueue; // 执行副作用的队列
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
}

internal static class Extension
{
    extension(List<object>)
    {
        public static T RemoveLast<T>(List<T> list)
        {
            var count = list.Count;
            var value = list[count - 1];
            list.RemoveAt(count - 1);
            return value;
        }
    }
}

public class Signal<T>(T value)
{
    private readonly ISignalState<T> _state = new SignalState<T>(value);

    public T Value
    {
        get => Reactive.ReadSignal(_state);
        set { Reactive.WriteSignal(_state, value); }
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

public static partial class Reactive
{
    public static Signal<T> CreateSignal<T>(T value)
    {
        return new Signal<T>(value);
    }

    public static IComputationNode<T> CreateComputation<T>(
        EffectFunction<T, T> fn,
        T init,
        bool pure = false,
        ComputationState state = ComputationState.Stale)
    {
        return new Computation<T>(fn, init, pure, state);
    }

    public static void CreateEffect<T>(
        EffectFunction<T, T> fn,
        T? value = default
    )
    {
        Context.RunEffects = RunUserEffects; // 当用户第一次使用createEffect时切换
        var c = new Computation<T>(fn, value!);

        c.User = true;
        if (Context.EffectQueue is null)
        {
            UpdateComputation((IComputationNode<object>)c);
        }
        else
        {
            Context.EffectQueue.Add((IComputationNode<object>)c);
        }
    }

    internal static T ReadSignal<T>(ISignalState<T> signalState)
    {
        if (signalState is IMemo<T> memo)
        {
            if (memo.State == ComputationState.Stale)
            {
                UpdateComputation((IMemo<object>)memo);
            }
            else
            {
                var updates = Context.UpdateQueue;
                Context.UpdateQueue = null;
                RunUpdates<object>(() =>
                {
                    LookUpstream((IMemo<object>)memo);
                    return new { };
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
            currentComp.Sources.Add((ISignalState<object>)signalState); // 当前计算节点自动收集依赖
            currentComp.SourceSlots!.Add(sSlot);
        }
        else
        {
            currentComp.Sources = [(ISignalState<object>)signalState];
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

        if (node.Comparator?.Invoke(current, value) ?? false) return default;
        node.Value = value;

        if (node.Observers is null) return default;
        RunUpdates(() =>
        {
            foreach (var observer in node.Observers)
            {
                if (observer.State == ComputationState.Resolved)
                {
                    if (observer.Pure) Context.UpdateQueue!.Add(observer);
                    else Context.EffectQueue!.Add(observer);
                    if (observer is IMemo<object> memo) MarkDownstream(memo);
                }

                observer.State = ComputationState.Stale;
            }

            if (Context.UpdateQueue!.Count > 10e5)
            {
                Context.UpdateQueue = [];
                throw new Exception("Potential Infinite Loop Detected.");
            }

            return new { };
        }, false);
        return value;
    }

    public static T Batch<T>(Accessor<T> fn)
    {
        return RunUpdates(fn, false);
    }

    internal static T RunUpdates<T>(Accessor<T> fn, bool init)
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
            return default;
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
            RunUpdates<object>(() =>
            {
                Context.RunEffects(e);
                return new { };
            }, false);
    }

    internal static readonly Action<ICollection<IComputationNode<object>>> RunQueue = queues =>
    {
        foreach (var queue in queues)
        {
            RunTop(queue);
        }
    };

    internal static readonly Action<List<IComputationNode<object>>> RunUserEffects = queue =>
    {
        var userLength = 0;
        for (var i = 0; i < queue.Count; i++)
        {
            var e = queue[i];
            if (!e.User) RunTop(e);
            else queue[userLength++] = e;
        }

        for (var i = 0; i < userLength; i++) RunTop(queue[i]);
    };

    private static void RunTop(IComputationNode<object> node)
    {
        if (node.State == ComputationState.Resolved) return;
        if (node.State == ComputationState.Pending)
        {
            LookUpstream(node);
            return;
        }

        List<IComputationNode<object>> ancestors = [node];
        while (true)
        {
            if (!(node.Parent is IComputationNode<object> parent && parent.UpdatedAt <= Context.ExecCount)) break;
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
                RunUpdates<object>(() =>
                {
                    LookUpstream(node, ancestors[0]);
                    return new { };
                }, false);
                Context.UpdateQueue = updates;
            }
        }
    }

    internal static void UpdateComputation(IComputationNode<object> node)
    {
        CleanNode(node); // 动态依赖切换，先断开旧依赖
        var time = Context.ExecCount;
        RunComputation(node, node.Value, time);
    }

    private static void RunComputation<T>(IComputationNode<T> node, T prevValue, long time)
    {
        T nextValue;
        var tempOwner = Context.CurrentOwner;
        var tempComputation = Context.CurrentComputation;
        Context.CurrentOwner = Context.CurrentComputation = (IComputationNode<object>)node;
        try
        {
            nextValue = node.Fn(prevValue);
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
        if (node is IMemo<object> memo)
        {
            WriteSignal(memo, nextValue);
        }
        else node.Value = nextValue;

        node.UpdatedAt = time;
    }

    /**
     * 递归向下传播脏状态
     * @param node
     */
    internal static void MarkDownstream(IMemo<object> node)
    {
        foreach (var observer in node.Observers!)
        {
            if (observer.State != ComputationState.Resolved) continue;
            observer.State = ComputationState.Pending;
            if (observer.Pure) Context.UpdateQueue!.Add(observer);
            else Context.EffectQueue!.Add(observer);
            if (observer is not IMemo<object> memo) continue;
            MarkDownstream(memo);
        }
    }

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     * @param node
     * @param ignore
     */
    private static void LookUpstream(IComputationNode<object> node, IComputationNode<object>? ignore = null)
    {
        node.State = ComputationState.Resolved;
        for (var i = 0; i < node.Sources!.Count; i += 1)
        {
            if (!(node.Sources![i] is IMemo<object> source)) continue;
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

    private static void CleanNode(IOwner node)
    {
        int i;
        if (node is IComputationNode<object> computationNode)
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

    private static void CleanComputationNode(IComputationNode<object> node)
    {
        while ((node.Sources?.Count ?? 0) != 0)
        {
            var source = List<object>.RemoveLast(node.Sources!);
            var index = List<object>.RemoveLast(node.SourceSlots!);
            var obs = source.Observers;

            if (obs is null) continue;
            var n = List<object>.RemoveLast(obs);
            var s = List<object>.RemoveLast(source.ObserverSlots!);

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
            Context.EffectQueue.Add(new ComputationNode<object>(o =>
                {
                    RunErrors(error, fns, owner);
                    return o;
                },
                new { }
            ));

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
            updateFn = () => fn(() => Untrack<object>(() =>
            {
                CleanNode(root);
                return new { };
            }));
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
        CreateEffect<object>(_ =>
        {
            Untrack(() =>
            {
                fn();
                return new { };
            });
            return new { };
        });
    }

    public static Action OnCleanup(Action fn)
    {
        if (Context.CurrentOwner is null)
            Console.WriteLine("cleanups created outside a `createRoot` or `render` will never be run");
        else if (Context.CurrentOwner.Cleanups is null) Context.CurrentOwner.Cleanups = [fn];
        else Context.CurrentOwner.Cleanups.Add(fn);
        return fn;
    }
}