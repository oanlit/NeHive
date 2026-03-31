namespace Lib;

using System.Diagnostics;

// 开放的API
public static partial class Reactive
{
    public static Signal<T> CreateSignal<T>(T value)
        => new Signal<T>(value);

    public static ComputationNode<object> CreateComputation(
        Action fn,
        bool pure = false,
        ComputationPhase phase = ComputationPhase.Stale
    ) => new ComputationNode<object>(Util.WrapActionWithArg(fn), Constant.EmptyObj, isPure: pure, phase: phase);

    public static ComputationNode<T> CreateComputation<T>(
        Func<T, T> fn,
        T init,
        bool pure = false,
        ComputationPhase phase = ComputationPhase.Stale
    ) => new ComputationNode<T>(fn, init, isPure: pure, phase: phase);

    public static Effect<T> CreateEffect<T>(
        Func<T, T> fn,
        T value
    ) => new Effect<T>(fn, value);

    public static void CreateEffect(Action fn)
        => CreateEffect(Util.WrapActionWithArg(fn), Constant.EmptyObj);

    public static void CreateEffect<T>(EffectOptions<T> options)
        => Effect<T>.Create(options);

    public static Memo<T> CreateMemo<T>(Func<T> fn)
        => new Memo<T>(fn);

    public static Memo<T> CreateMemo<T>(Func<T, T> fn)
        => new Memo<T>(fn);

    public static void Batch(Action fn)
    {
        ComputationNode.RunUpdates(fn, false);
    }

    public static T Batch<T>(Func<T> fn)
    {
        return ComputationNode.RunUpdates(fn, false);
    }

    public static void CreateRoot(Action fn, Owner? detachedOwner = null)
    {
        var currentComputation = CurrentContext.Computation;
        var currentOwner = CurrentContext.Owner;
        var current = detachedOwner ?? currentOwner;
        var root = new Owner(
            parent: current,
            children: null,
            context: current?.Context,
            cleanups: null
        );

        var updateFn = () =>
        {
            fn();
            Untrack(() => root.CleanNode());
        };

        CurrentContext.Owner = root;
        CurrentContext.Computation = null;

        try
        {
            ComputationNode.RunUpdates(updateFn, true);
        }
        finally
        {
            CurrentContext.Computation = currentComputation;
            CurrentContext.Owner = currentOwner;
        }
    }

    public static T CreateRoot<T>(Func<Action, T> fn, Owner? detachedOwner = null)
    {
        var currentComputation = CurrentContext.Computation;
        var currentOwner = CurrentContext.Owner;
        var current = detachedOwner ?? currentOwner;
        var root = new Owner(
            parent: current,
            children: null,
            context: current?.Context,
            cleanups: null
        );

        var updateFn = () => fn(() => Untrack(() => root.CleanNode()));

        CurrentContext.Owner = root;
        CurrentContext.Computation = null;

        try
        {
            return ComputationNode.RunUpdates(updateFn, true)!;
        }
        finally
        {
            CurrentContext.Computation = currentComputation;
            CurrentContext.Owner = currentOwner;
        }
    }

    public static void OnMount(Action fn)
    {
        CreateEffect(() => Untrack(fn));
    }

    public static Action OnCleanup(Action fn)
    {
        if (CurrentContext.Owner is null)
            Trace.TraceWarning("cleanups created outside a `createRoot` or `render` will never be run");
        else if (CurrentContext.Owner.Cleanups is null) CurrentContext.Owner.Cleanups = [fn];
        else CurrentContext.Owner.Cleanups.Add(fn);
        return fn;
    }

    public static T Untrack<T>(Func<T> fn)
    {
        return ComputationNode.Untrack(fn);
    }

    public static void Untrack(Action fn)
    {
        ComputationNode.Untrack(fn);
    }

    public static T Untrack<T>(IReadOnlySignal<T> source)
    {
        return ComputationNode.Untrack(source);
    }

    public static void BatchUntrack(Action fn)
    {
        ComputationNode.RunUpdates(() =>
        {
            ComputationNode.Untrack(fn);
            return Constant.EmptyObj;
        }, false);
    }

    public static Context<T> CreateContext<T>(T defaultValue)
        => new Context<T>("context", defaultValue);

    public static T UseContext<T>(Context<T> context)
    {
        var owner = CurrentContext.Owner;
        if (owner?.Context?[context.Id] is T value) return value;
        return context.DefaultValue;
    }

    public static T? RunWithOwner<T>(Owner? o, Func<T> fn)
        => ComputationNode.RunWithOwner(o, fn);

    public static Resource<TSource, TValue, TInfo> CreateResource<TSource, TValue, TInfo>(
        Func<TSource, TValue?, TInfo?, object> fetcher,
        IReadOnlySignal<TSource> signalSource
    ) => new Resource<TSource, TValue, TInfo>(fetcher, signalSource);

    public static Resource<TSource, TValue, TInfo> CreateResource<TSource, TValue, TInfo>(
        Func<TSource, TValue?, TInfo?, object> fetcher,
        TSource? source = default
    ) => new Resource<TSource, TValue, TInfo>(fetcher, source);

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
        var node = new MemoState<T>(fn: computationFn, isPure: true, phase: ComputationPhase.Stale);
        node.UpdateComputation();
        return key =>
        {
            var currentComputation = CurrentContext.Computation;
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

public class Memo<T> : IReadOnlySignal<T>
{
    private readonly MemoState<T> _state;

    public T Value => _state.ReadSignal();
    public T UntrackValue => _state.UntrackValue;

    public Memo(Func<T, T> fn, T? value = default)
    {
        _state = new MemoState<T>(
            fn,
            comparator: Constant.EqualFn,
            isPure: true
        )
        {
            Observers = null,
            ObserverSlots = null,
            Phase = ComputationPhase.Resolved,
            Value = value!
        };
        _state.UpdateComputation();
    }

    public Memo(Func<T> fn, T? value = default)
        : this(_ => fn(), value)
    {
    }
}

// 算法
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

    public static Func<object> WrapAction(Action fn)
    {
        return () =>
        {
            fn();
            return Constant.EmptyObj;
        };
    }

    public static Func<object, object> WrapActionWithArg(Action fn)
    {
        return _ =>
        {
            fn();
            return Constant.EmptyObj;
        };
    }
}

internal static class Common
{
    internal static T BaseReadSignal<T>(ISignalState<T> signal)
    {
        var currentComp = CurrentContext.Computation;
        if (currentComp is null) return signal.Value;
        // 建立Computation与Signal的双向引用
        var sSlot = signal.Observers?.Count ?? 0;
        var oSlot = currentComp.Sources?.Count ?? 0;
        if (currentComp.Sources is not null)
        {
            currentComp.Sources.Add(signal); // 当前计算节点自动收集依赖
            currentComp.SourceSlots!.Add(sSlot);
        }
        else
        {
            currentComp.Sources = [signal];
            currentComp.SourceSlots = [sSlot];
        }

        if (signal.Observers is not null)
        {
            signal.Observers.Add(currentComp);
            signal.ObserverSlots!.Add(oSlot);
        }
        else
        {
            signal.Observers = [currentComp];
            signal.ObserverSlots = [oSlot];
        }

        return signal.Value;
    }

    internal static T WriteSignal<T>(ISignalState<T> signal, T value)
    {
        var current = signal.Value;

        if (signal.Comparator.Invoke(current, value)) return current;
        signal.Value = value;

        if (signal.Observers is null) return value;

        ComputationNode.NotifyObservers(signal.Observers);

        return value;
    }
}

public interface ISignalState
{
    List<ComputationNode>? Observers { get; set; }
    List<int>? ObserverSlots { get; set; }
    internal void UpdateIfNeeded(ComputationNode? ignore = null);
}

public interface ISignalState<T> : ISignalState
{
    T Value { get; internal set; }
    Func<T, T, bool> Comparator { get; }
    T ReadSignal();
    T WriteSignal(T value);
}

public class SignalState<T>(T value, Func<T, T, bool>? comparator = null) : ISignalState<T>
{
    public List<ComputationNode>? Observers { get; set; }
    public List<int>? ObserverSlots { get; set; }
    public Func<T, T, bool> Comparator { get; init; } = comparator ?? Constant.EqualFn;
    public T Value { get; set; } = value;

    public T ReadSignal()
    {
        return Common.BaseReadSignal(this);
    }

    public T WriteSignal(T value)
    {
        return Common.WriteSignal(this, value);
    }

    public void UpdateIfNeeded(ComputationNode? ignore = null)
    {
    }
}

public class Owner(
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
)
{
    internal readonly Owner? Parent = parent;
    internal List<ComputationNode>? Children = children;
    internal List<Action>? Cleanups = cleanups;
    internal Dictionary<string, object?>? Context = context;

    internal virtual void CleanNode()
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

public enum ComputationPhase
{
    Resolved,
    Stale,
    Pending
}

internal static class CurrentContext
{
    public static Owner? Owner; // 当前正在执行的所有者
    public static ComputationNode? Computation; // 当前计算节点
}

public abstract class ComputationNode : Owner
{
    protected struct CurrentState
    {
        public static List<ComputationNode>? UpdateQueue; // 更新值的队列
        public static List<ComputationNode>? EffectQueue; // 执行副作用的队列
        public static long ExecCount; // 执行计数器，和ComputationNode.Version 配合使用
        public static string? Error = null;
    }

    internal ComputationPhase Phase;
    internal List<ISignalState>? Sources;
    internal List<int>? SourceSlots;
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
        Owner? parent = null,
        List<ComputationNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<string, object?>? context = null
    ) : base(parent, children, cleanups, context)
    {
        Phase = phase;
        Sources = sources;
        SourceSlots = sourceSlots;
        Version = version;
        IsPure = isPure;
        IsUser = isUser;

        if (CurrentContext.Owner is null)
            Trace.TraceWarning(
                "computations created outside a `createRoot` or `render` will never be disposed"
            );
        else if (CurrentContext.Owner != Constant.UnOwned)
        {
            if (CurrentContext.Owner.Children is null)
                CurrentContext.Owner.Children = [this];
            else CurrentContext.Owner.Children.Add(this);
        }
    }

    internal override void CleanNode()
    {
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

        base.CleanNode();

        Phase = ComputationPhase.Resolved; // 当前及旧的子Computation已解决，旧的子Computation不再执行
    }

    // 核心功能入口点
    internal static void NotifyObservers(List<ComputationNode> observers)
    {
        RunUpdates(() =>
        {
            for (var i = 0; i < observers.Count; i++)
            {
                var observer = observers[i];
                observer.AddQueue();
            }

            if (CurrentState.UpdateQueue!.Count > 10e5)
            {
                CurrentState.UpdateQueue = [];
                throw new Exception("Potential Infinite Loop Detected.");
            }

            return Constant.EmptyObj;
        }, false);
    }

    internal void AddQueue()
    {
        if (Phase == ComputationPhase.Resolved)
        {
            if (IsPure) CurrentState.UpdateQueue!.Add(this);
            else CurrentState.EffectQueue!.Add(this);

            // MemoState<T> 的额外行为
            MarkDownstream();
        }

        Phase = ComputationPhase.Stale;
    }

    internal virtual void MarkDownstream()
    {
    }

    internal static T RunUpdates<T>(Func<T> fn, bool init)
    {
        if (CurrentState.UpdateQueue is not null) return fn();
        var wait = false;
        if (!init) CurrentState.UpdateQueue = [];
        if (CurrentState.EffectQueue is not null) wait = true;
        else CurrentState.EffectQueue = [];
        CurrentState.ExecCount++;
        try
        {
            var res = fn();
            CompleteUpdates(wait);
            return res;
        }
        catch (Exception err)
        {
            if (!wait) CurrentState.EffectQueue = null;
            CurrentState.UpdateQueue = null;
            HandleError(err);
            return default!;
        }
    }

    internal static void RunUpdates(Action fn, bool init)
        => RunUpdates(Util.WrapAction(fn), init);

    private static void CompleteUpdates(bool wait)
    {
        if (CurrentState.UpdateQueue is not null)
        {
            RunQueue(CurrentState.UpdateQueue);
            CurrentState.UpdateQueue = null;
        }

        if (wait) return;
        var e = CurrentState.EffectQueue!;
        CurrentState.EffectQueue = null;
        if (e.Count != 0)
            RunUpdates(() =>
            {
                RunUserEffects(e);
                return Constant.EmptyObj;
            }, false);
    }

    private static void RunQueue(List<ComputationNode> queues)
    {
        for (var i = 0; i < queues.Count; i++)
        {
            var queue = queues[i];
            queue.RunTop();
        }
    }

    private static void RunUserEffects(List<ComputationNode> queue)
    {
        var userLength = 0;
        for (var i = 0; i < queue.Count; i++)
        {
            var e = queue[i];
            if (!e.IsUser) e.RunTop();
            else queue[userLength++] = e;
        }

        for (var i = 0; i < userLength; i++) queue[i].RunTop();
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
            if (!(Parent is ComputationNode parent && parent.Version <= CurrentState.ExecCount)) break;
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
            {
                var updates = CurrentState.UpdateQueue;
                CurrentState.UpdateQueue = null;
                RunUpdates(() => LookUpstream(ancestors[0]), false);
                CurrentState.UpdateQueue = updates;
            }
        }
    }

    internal void UpdateComputation()
    {
        CleanNode(); // 动态更新依赖，先断开旧依赖
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
        for (var i = 0; i < Sources?.Count; i += 1)
            Sources[i].UpdateIfNeeded();
    }

    protected static void HandleError(Exception err, Owner? owner = null)
    {
        owner ??= CurrentContext.Owner;
        List<Action<object>>? fns = null;
        if (CurrentState.Error is not null)
        {
            fns = owner?.Context?[CurrentState.Error] as List<Action<object>>;
        }

        var error = CastError(err);
        if (fns is null) throw error;

        if (CurrentState.EffectQueue is not null)
        {
            ComputationNode<object> handler = new(Util.WrapActionWithArg(() => RunErrors(error, fns, owner)),
                Constant.EmptyObj, isPure: false, phase: ComputationPhase.Stale);
            handler.Phase = ComputationPhase.Stale;
            CurrentState.EffectQueue.Add(handler);
        }
        else RunErrors(error, fns, owner);
    }

    internal static Exception CastError(object err)
    {
        if (err is Exception exception) return exception;
        return new Exception(err as string ?? "Unknown error");
    }

    private static void RunErrors(object err, List<Action<object>> fns, Owner? owner)
    {
        try
        {
            for (var i = 0; i < fns.Count; i++)
            {
                var f = fns[i];
                f(err);
            }
        }
        catch (Exception e)
        {
            HandleError(e, owner?.Parent);
        }
    }

    // 额外API，非核心算法
    internal static T Untrack<T>(Func<T> fn)
    {
        if (CurrentContext.Computation is null) return fn();

        var currentComputation = CurrentContext.Computation;
        CurrentContext.Computation = null;
        try
        {
            return fn();
        }
        finally
        {
            CurrentContext.Computation = currentComputation;
        }
    }

    internal static void Untrack(Action fn)
        => Untrack(Util.WrapAction(fn));

    internal static T Untrack<T>(IReadOnlySignal<T> source)
        => Untrack(() => source.Value);

    internal static T? RunWithOwner<T>(Owner? o, Func<T> fn)
    {
        var tempOwner = CurrentContext.Owner;
        var tempComputation = CurrentContext.Computation;
        CurrentContext.Owner = o;
        CurrentContext.Computation = null;
        try
        {
            return RunUpdates(fn, true)!;
        }
        catch (Exception err)
        {
            HandleError(err);
        }
        finally
        {
            CurrentContext.Owner = tempOwner;
            CurrentContext.Computation = tempComputation;
        }

        return default;
    }
}

public class ComputationNode<T>(
    Func<T, T> fn,
    T value,
    ComputationPhase phase = ComputationPhase.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long version = 0,
    bool isPure = false,
    bool isUser = false,
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
) : ComputationNode(phase, sources, sourceSlots,
    version, isPure, isUser,
    parent, children, cleanups, context)
{
    public readonly Func<T, T> Fn = fn;
    public virtual T Value { get; set; } = value;

    protected virtual void UpdateValue(T value) => Value = value;

    protected override void RunComputation()
    {
        T nextValue;
        var execCount = CurrentState.ExecCount;
        var tempOwner = CurrentContext.Owner;
        var tempComputation = CurrentContext.Computation;
        CurrentContext.Owner = CurrentContext.Computation = this;
        try
        {
            nextValue = Fn(Value);
        }
        catch (Exception err)
        {
            if (IsPure)
            {
                Phase = ComputationPhase.Stale;
                Children?.ForEach(node => node.CleanNode());
                Children = null;
            }

            // won't be picked up until next update
            Version = execCount + 1;
            HandleError(err);
            return;
        }
        finally
        {
            CurrentContext.Computation = tempComputation;
            CurrentContext.Owner = tempOwner;
        }

        if (Version > execCount) return;
        UpdateValue(nextValue);
        Version = execCount;
    }
}

public record struct EffectOptions<T>(
    Func<T, T?, T, T> Fn,
    Func<T> Deps,
    bool Defer = false
);

public class Effect<T> : ComputationNode<T>
{
    public Effect(Func<T, T> fn,
        T value) : base(fn, value, isUser: true)
    {
        if (CurrentState.EffectQueue is null)
            UpdateComputation();
        else
            CurrentState.EffectQueue.Add(this);
    }

    public static Effect<T> Create(EffectOptions<T> options)
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

            var result = Untrack(() => options.Fn(input, prevInput, prevValue));
            prevInput = input;
            return result;
        };
        return new(fn, prevInput!);
    }
}

internal class MemoState<T>(
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
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
) : ComputationNode<T>(fn, value!, phase,
        sources, sourceSlots, version, isPure, isUser,
        parent, children, cleanups, context),
    ISignalState<T>
{
    public Func<T, T, bool> Comparator { get; } = comparator ?? Constant.EqualFn;
    public List<ComputationNode>? Observers { get; set; } = observers;
    public List<int>? ObserverSlots { get; set; } = observerSlots;

    public override T Value { get; set; } = value!;

    protected override void UpdateValue(T value) => WriteSignal(value);

    /**
     * 递归向下传播脏状态
     */
    internal override void MarkDownstream()
    {
        for (var i = 0; i < Observers?.Count; i++)
        {
            var observer = Observers[i];
            if (observer.Phase != ComputationPhase.Resolved) continue;
            observer.Phase = ComputationPhase.Pending;
            if (observer.IsPure) CurrentState.UpdateQueue!.Add(observer);
            else CurrentState.EffectQueue!.Add(observer);
            observer.MarkDownstream();
        }
    }

    public void UpdateIfNeeded(ComputationNode? ignore = null)
    {
        switch (Phase)
        {
            case ComputationPhase.Stale:
                if (this != ignore && Version < CurrentState.ExecCount)
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
            var updates = CurrentState.UpdateQueue;
            CurrentState.UpdateQueue = null;
            RunUpdates(() =>
            {
                LookUpstream();
                return Constant.EmptyObj;
            }, false);
            CurrentState.UpdateQueue = updates;
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

public class Context<T>(string id = "context", T defaultValue = default!)
{
    public string Id = id;
    public T DefaultValue = defaultValue;

    public void Provider(T value, Action fn)
    {
        var owner = CurrentContext.Owner;
        if (owner is null) throw new Exception("CurrentOwner is None!");
        owner.Context ??= new Dictionary<string, object?>();
        owner.Context[Id] = value;
        fn();
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
    private Signal<TValue?> _value = new(default);
    private Task<TValue>? _task;
    private object _initTask = Constant.EmptyObj;
    private Owner? _owner = CurrentContext.Owner;
    private bool _scheduled;
    private readonly IReadOnlySignal<TSource?> _source;

    // object 的类型为 TValue | Task<TValue>
    private Func<TSource, TValue?, TInfo?, object> _fetcher;

    public Resource(Func<TSource, TValue?, TInfo?, object> fetcher, IReadOnlySignal<TSource> signalSource)
    {
        _fetcher = fetcher;
        _source = Reactive.CreateMemo<TSource?>(_ => signalSource.Value);
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
        ComputationNode.RunUpdates(
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
        ComputationNode.RunUpdates(() =>
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