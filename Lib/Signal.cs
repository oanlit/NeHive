namespace Lib;

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
        Func<T, T> fn,
        T init,
        bool pure = false,
        ComputationState state = ComputationState.Stale)
    {
        return new Computation<T>(fn, init, pure: pure, state: state);
    }

    public static void CreateEffect(Action fn)
    {
        CreateEffect(_ =>
        {
            fn();
            return Constant.EmptyObj;
        }, Constant.EmptyObj);
    }

    public static void CreateEffect<T>(
        Func<T, T> fn,
        T? value = default
    )
    {
        var c = new Computation<T>(fn, value!);

        c.User = true;
        if (Context.EffectQueue is null)
        {
            c.UpdateComputation();
        }
        else
        {
            Context.EffectQueue.Add(c);
        }
    }

    public static ReadOnlySignal<T> CreateMemo<T>(Func<T, T> fn)
    {
        return new ReadOnlySignal<T>(fn);
    }

    public static T Batch<T>(Func<T> fn)
    {
        return Common.RunUpdates(fn, false);
    }

    public static T CreateRoot<T>(Func<Action, T> fn, Owner? detachedOwner = null)
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
        Func<T> updateFn;
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
            updateFn = () => fn(() => Untrack(() => root.CleanNode()));
        }

        Context.CurrentOwner = root;
        Context.CurrentComputation = null;

        try
        {
            return Common.RunUpdates(updateFn, true)!;
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

    public static T Untrack<T>(Func<T> fn)
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
}

// public delegate TNext EffectFunction<in TPrev, out TNext>(TPrev v) where TNext : TPrev;
//
// public delegate T EffectFunction<T>(T v);
//
// public delegate void EffectFunction();

// public delegate T Accessor<out T>();
//
// public delegate T RootFunction<out T>(Action dispose);

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
    T WriteSignal(T value);
    T ReadSignal();
}

public class SignalState<T>(T value) : ISignalState<T>
{
    public List<IComputationNode>? Observers { get; set; }
    public List<int>? ObserverSlots { get; set; }
    public Func<T, T, bool>? Comparator { get; set; }

    public virtual T Value { get; set; } = value;

    public T ReadSignal()
    {
        var currentComp = Context.CurrentComputation;
        if (currentComp is null) return Value;
        // 建立Computation与Signal的双向引用
        var sSlot = Observers?.Count ?? 0;
        var oSlot = currentComp.Sources?.Count ?? 0;
        if (currentComp.Sources is not null)
        {
            currentComp.Sources.Add(this); // 当前计算节点自动收集依赖
            currentComp.SourceSlots!.Add(sSlot);
        }
        else
        {
            currentComp.Sources = [this];
            currentComp.SourceSlots = [sSlot];
        }

        if (Observers is not null)
        {
            Observers.Add(currentComp);
            ObserverSlots!.Add(oSlot);
        }
        else
        {
            Observers = [currentComp];
            ObserverSlots = [oSlot];
        }

        return Value;
    }

    public T WriteSignal(T value)
    {
        var current = Value;

        if (Comparator?.Invoke(current, value) ?? false) return default!;
        Value = value;

        if (Observers is null) return default!;
        Common.RunUpdates(() =>
        {
            foreach (var observer in Observers)
            {
                observer.AddQueue();
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
}

public interface IOwner
{
    IOwner? Parent { get; set; }
    List<IComputationNode>? Children { get; set; }
    List<Action>? Cleanups { get; set; }
    Dictionary<string, object>? Context { get; set; }
    void CleanNode();
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

    public void CleanNode()
    {
        int i;
        if (Children is not null)
        {
            for (i = Children.Count - 1; i >= 0; i--) Children[i].CleanNode();
            Children = null;
        }

        if ((Cleanups?.Count ?? 0) == 0) return;

        for (i = Cleanups!.Count - 1; i >= 0; i--) Cleanups[i]();
        Cleanups = null;
    }
}

public interface IComputationNode : IOwner
{
    Action Fn { get; }
    ComputationState State { get; set; }
    List<ISignalState>? Sources { get; set; }
    List<int>? SourceSlots { get; set; }
    long UpdatedAt { get; set; }
    bool Pure { get; }
    bool User { get; }
    void RunComputation(long time);
    internal void AddQueue();
    internal void RunTop();
    internal void UpdateComputation();
    internal void LookUpstream(IComputationNode? ignore = null);
}

public interface IComputationNode<TInit> : IComputationNode<TInit, TInit>;

public interface IComputationNode<TInit, TNext> : IComputationNode where TNext : TInit
{
    public new Func<TInit, TNext> Fn { get; }
    public TInit Value { get; set; }
}

public class ComputationNode(
    Action fn,
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
    public Action Fn { get; } = fn;
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

    public void AddQueue()
    {
        if (State == ComputationState.Resolved)
        {
            if (Pure) Lib.Context.UpdateQueue!.Add(this);
            else Lib.Context.EffectQueue!.Add(this);
            MarkDownstream();
        }

        State = ComputationState.Stale;
    }

    protected virtual void MarkDownstream() { }

    public void UpdateComputation()
    {
        CleanNode(); // 动态依赖切换，先断开旧依赖
        var time = Lib.Context.ExecCount;
        RunComputation(time);
    }

    public virtual void RunComputation(long time)
    {
        var tempOwner = Lib.Context.CurrentOwner;
        var tempComputation = Lib.Context.CurrentComputation;
        Lib.Context.CurrentOwner = Lib.Context.CurrentComputation = this;
        try
        {
            Fn();
        }
        catch (Exception err)
        {
            if (Pure)
            {
                State = ComputationState.Stale;
                Children?.ForEach(node => node.CleanNode());
                Children = null;
            }

            // won't be picked up until next update
            UpdatedAt = time + 1;
            Common.HandleError(err);
            return;
        }
        finally
        {
            Lib.Context.CurrentComputation = tempComputation;
            Lib.Context.CurrentOwner = tempOwner;
        }

        if (UpdatedAt > time) return;
        UpdatedAt = time;
    }

    public void RunTop()
    {
        if (State == ComputationState.Resolved) return;
        if (State == ComputationState.Pending)
        {
            LookUpstream();
            return;
        }

        List<IComputationNode> ancestors = [this];
        IComputationNode node = this;
        while (true)
        {
            if (!(Parent is IComputationNode parent && parent.UpdatedAt <= Lib.Context.ExecCount)) break;
            if (parent.State != ComputationState.Resolved) ancestors.Add(node);
            node = parent;
        }

        // 从根owner开始执行
        for (var i = ancestors.Count - 1; i >= 0; i--)
        {
            node = ancestors[i];

            if (node.State == ComputationState.Stale)
            {
                node.UpdateComputation();
            }
            else if (node.State == ComputationState.Pending)
            {
                var updates = Lib.Context.UpdateQueue;
                Lib.Context.UpdateQueue = null;
                Common.RunUpdates(() => LookUpstream(ancestors[0]), false);
                Lib.Context.UpdateQueue = updates;
            }
        }
    }

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     * @param node
     * @param ignore
     */
    public void LookUpstream(IComputationNode? ignore = null)
    {
        State = ComputationState.Resolved;
        for (var i = 0; i < Sources!.Count; i += 1)
        {
            if (Sources![i] is not IMemo source) continue;
            var state = source.State;
            switch (state)
            {
                case ComputationState.Stale:
                    if (source != ignore && (source.UpdatedAt < Lib.Context.ExecCount))
                        source.RunTop();
                    break;
                case ComputationState.Pending:
                    LookUpstream(ignore);
                    break;
            }
        }
    }

    public void CleanNode()
    {
        int i;
        while ((Sources?.Count ?? 0) != 0)
        {
            var source = Util.RemoveLast(Sources!);
            var index = Util.RemoveLast(SourceSlots!);
            var obs = source.Observers;

            if (obs is null) continue;
            var n = Util.RemoveLast(obs);
            var s = Util.RemoveLast(source.ObserverSlots!);

            if (index >= obs.Count) continue;
            n.SourceSlots![s] = index;
            obs[index] = n;
            source.ObserverSlots![index] = s;
        }

        if (Children is not null)
        {
            for (i = Children.Count - 1; i >= 0; i--) Children[i].CleanNode();
            Children = null;
        }

        if ((Cleanups?.Count ?? 0) != 0)
        {
            for (i = Cleanups!.Count - 1; i >= 0; i--) Cleanups[i]();
            Cleanups = null;
        }

        State = ComputationState.Resolved; // 当前及旧的子Computation已解决，旧的子Computation不再执行
    }
}

public class ComputationNode<T>(
    Func<T, T> fn,
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
) : ComputationNode(Constant.EmptyEffectFunction, state, sources, sourceSlots,
        updatedAt, pure, user, parent, children, cleanups, context),
    IComputationNode<T>
{
    public Func<T, T> Fn { get; } = fn;
    public T Value { get; set; } = value;

    protected virtual void UpdateValue(T value)
    {
        Value = value;
    }

    public override void RunComputation(long time)
    {
        T nextValue;
        var tempOwner = Lib.Context.CurrentOwner;
        var tempComputation = Lib.Context.CurrentComputation;
        Lib.Context.CurrentOwner = Lib.Context.CurrentComputation = this;
        try
        {
            nextValue = Fn(Value);
        }
        catch (Exception err)
        {
            if (Pure)
            {
                State = ComputationState.Stale;
                Children?.ForEach(node => node.CleanNode());
                Children = null;
            }

            // won't be picked up until next update
            UpdatedAt = time + 1;
            Common.HandleError(err);
            return;
        }
        finally
        {
            Lib.Context.CurrentComputation = tempComputation;
            Lib.Context.CurrentOwner = tempOwner;
        }

        if (UpdatedAt > time) return;
        UpdateValue(nextValue);
        UpdatedAt = time;
    }
}

public class Computation<T> : ComputationNode<T>
{
    public Computation(
        Func<T, T> fn,
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
    Func<T, T> fn,
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
    Action IComputationNode.Fn { get; } = Constant.EmptyEffectFunction;
    public Func<T, T, bool>? Comparator { get; } = comparator;
    public List<IComputationNode>? Observers { get; set; } = observers;
    public List<int>? ObserverSlots { get; set; } = observerSlots;

    protected override void UpdateValue(T value)
    {
        WriteSignal(value);
    }

    /**
     * 递归向下传播脏状态
     */
    protected override void MarkDownstream()
    {
        foreach (var observer in Observers!)
        {
            if (observer.State != ComputationState.Resolved) continue;
            observer.State = ComputationState.Pending;
            if (observer.Pure) Lib.Context.UpdateQueue!.Add(observer);
            else Lib.Context.EffectQueue!.Add(observer);
            if (observer is not IMemo memo) continue;
            MarkDownstream();
        }
    }

    public T ReadSignal()
    {
        if (State == ComputationState.Stale)
        {
            UpdateComputation();
        }
        else
        {
            var updates = Lib.Context.UpdateQueue;
            Lib.Context.UpdateQueue = null;
            Common.RunUpdates(() =>
            {
                LookUpstream();
                return Constant.EmptyObj;
            }, false);
            Lib.Context.UpdateQueue = updates;
        }

        var currentComp = Lib.Context.CurrentComputation;
        if (currentComp is null) return Value;
        // 建立Computation与Signal的双向引用
        var sSlot = Observers?.Count ?? 0;
        var oSlot = currentComp.Sources?.Count ?? 0;
        if (currentComp.Sources is not null)
        {
            currentComp.Sources.Add(this); // 当前计算节点自动收集依赖
            currentComp.SourceSlots!.Add(sSlot);
        }
        else
        {
            currentComp.Sources = [this];
            currentComp.SourceSlots = [sSlot];
        }

        if (Observers is not null)
        {
            Observers.Add(currentComp);
            ObserverSlots!.Add(oSlot);
        }
        else
        {
            Observers = [currentComp];
            ObserverSlots = [oSlot];
        }

        return Value;
    }

    public T WriteSignal(T value)
    {
        var current = Value;

        if (Comparator?.Invoke(current, value) ?? false) return default!;
        Value = value;

        if (Observers is null) return default!;
        Common.RunUpdates(() =>
        {
            foreach (var observer in Observers)
            {
                observer.AddQueue();
            }

            if (Lib.Context.UpdateQueue!.Count > 10e5)
            {
                Lib.Context.UpdateQueue = [];
                throw new Exception("Potential Infinite Loop Detected.");
            }

            return Constant.EmptyObj;
        }, false);
        return value;
    }
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
    public static readonly Action EmptyEffectFunction = () => { };
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

internal static class Common
{
    internal static void RunUpdates(Action fn, bool init)
    {
        RunUpdates(() =>
        {
            fn();
            return Constant.EmptyObj;
        }, init);
    }

    internal static T RunUpdates<T>(Func<T> fn, bool init)
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
            queue.RunTop();
        }
    }

    private static void RunUserEffects(List<IComputationNode> queue)
    {
        var userLength = 0;
        for (var i = 0; i < queue.Count; i++)
        {
            var e = queue[i];
            if (!e.User) e.RunTop();
            else queue[userLength++] = e;
        }

        for (var i = 0; i < userLength; i++) queue[i].RunTop();
    }

    internal static void HandleError(Exception err, IOwner? owner = null)
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
            var handler = Reactive.CreateComputation(() => RunErrors(error, fns, owner));
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
}

public class ReadOnlySignal<T> : IReadOnlySignal<T>
{
    private readonly Memo<T> _state;

    public T Value => _state.ReadSignal();

    public ReadOnlySignal(Func<T, T> fn)
    {
        _state = new Memo<T>(
            fn,
            comparator: Constant.EqualFn,
            pure: true
        )
        {
            Observers = null,
            ObserverSlots = null,
            State = ComputationState.Resolved
        };
        _state.UpdateComputation();
    }
}