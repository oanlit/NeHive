namespace Lib;

// 开放的API
public static partial class Reactive
{
    public static Signal<T> CreateSignal<T>(T value)
    {
        return new Signal<T>(value);
    }

    public static ComputationNode<object> CreateComputation(
        Action fn,
        bool pure = false,
        ComputationPhase phase = ComputationPhase.Stale)
    {
        return new Computation<object>(Util.WrapActionWithArg(fn), Constant.EmptyObj, pure: pure, phase: phase);
    }

    public static ComputationNode<T> CreateComputation<T>(
        Func<T, T> fn,
        T init,
        bool pure = false,
        ComputationPhase phase = ComputationPhase.Stale)
    {
        return new Computation<T>(fn, init, pure: pure, phase: phase);
    }

    public static void CreateEffect(Action fn)
    {
        CreateEffect(Util.WrapActionWithArg(fn), Constant.EmptyObj);
    }

    public static void CreateEffect<T>(EffectOptions<T> options)
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
        CreateEffect(fn, prevInput!);
    }

    public static void CreateEffect<T>(
        Func<T, T> fn,
        T value)
    {
        var c = new Computation<T>(fn, value);

        c.User = true;
        if (CurrentState.EffectQueue is null)
        {
            c.UpdateComputation();
        }
        else
        {
            CurrentState.EffectQueue.Add(c);
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
        var currentComputation = CurrentState.CurrentComputation;
        var currentOwner = CurrentState.CurrentOwner;
        var current = detachedOwner ?? currentOwner;
        var root = new Owner(
            parent: current,
            children: null,
            context: current?.Context,
            cleanups: null
        );

        var updateFn = () => fn(() => Untrack(() => root.CleanNode()));

        CurrentState.CurrentOwner = root;
        CurrentState.CurrentComputation = null;

        try
        {
            return Common.RunUpdates(updateFn, true)!;
        }
        finally
        {
            CurrentState.CurrentComputation = currentComputation;
            CurrentState.CurrentOwner = currentOwner;
        }
    }

    public static void OnMount(Action fn)
    {
        CreateEffect(() => Untrack(fn));
    }

    public static Action OnCleanup(Action fn)
    {
        if (CurrentState.CurrentOwner is null)
            Console.WriteLine("cleanups created outside a `createRoot` or `render` will never be run");
        else if (CurrentState.CurrentOwner.Cleanups is null) CurrentState.CurrentOwner.Cleanups = [fn];
        else CurrentState.CurrentOwner.Cleanups.Add(fn);
        return fn;
    }

    public static T Untrack<T>(Func<T> fn)
    {
        return Common.Untrack(fn);
    }

    public static void Untrack(Action fn)
    {
        Common.Untrack(fn);
    }

    public static T Untrack<T>(IReadOnlySignal<T> source)
    {
        return Common.Untrack(source);
    }

    public static Context<T> CreateContext<T>(T defaultValue)
    {
        return new Context<T>("context", defaultValue);
    }

    public static T UseContext<T>(Context<T> context)
    {
        var owner = CurrentState.CurrentOwner;
        if (owner?.Context?[context.Id] is T value) return value;
        return context.DefaultValue;
    }

    public static T? RunWithOwner<T>(Owner? o, Func<T> fn)
    {
        return Common.RunWithOwner(o, fn);
    }

    public static Resource<TSource, TValue, TInfo> CreateResource<TSource, TValue, TInfo>(
        Func<TSource, TValue?, TInfo?, object> fetcher, IReadOnlySignal<TSource> signalSource)
    {
        return new Resource<TSource, TValue, TInfo>(fetcher, signalSource);
    }

    public static Resource<TSource, TValue, TInfo> CreateResource<TSource, TValue, TInfo>(
        Func<TSource, TValue?, TInfo?, object> fetcher, TSource? source = default)
    {
        return new Resource<TSource, TValue, TInfo>(fetcher, source);
    }


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
                        c.Phase = ComputationPhase.Stale;
                        if (c.Pure) CurrentState.UpdateQueue!.Add(c);
                        else CurrentState.EffectQueue!.Add(c);
                    }
                }

            return nextValue;
        };
        var node = new Memo<T>(fn: computationFn, pure: true, phase: ComputationPhase.Stale);
        node.UpdateComputation();
        return key =>
        {
            var currentComputation = CurrentState.CurrentComputation;
            if (currentComputation is null) return fn(key, node.Value);

            var computations = subs[key];
            if (computations is not null) computations.Add(currentComputation);
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
            Phase = ComputationPhase.Resolved
        };
        _state.UpdateComputation();
    }
}

public record struct EffectOptions<T>(
    Func<T, T?, T, T> Fn,
    Func<T> Deps,
    bool Defer = false
);

// 算法
public enum ComputationPhase
{
    Resolved,
    Stale,
    Pending
}

internal static class CurrentState
{
    public static Owner? CurrentOwner; // 当前正在执行的所有者
    public static ComputationNode? CurrentComputation; // 当前计算节点
    public static List<ComputationNode>? UpdateQueue; // 更新值的队列
    public static List<ComputationNode>? EffectQueue; // 执行副作用的队列
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
        var currentComp = CurrentState.CurrentComputation;
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

        if (signal.Comparator?.Invoke(current, value) ?? false) return default!;
        signal.Value = value;

        if (signal.Observers is null) return default!;
        RunUpdates(() =>
        {
            foreach (var observer in signal.Observers)
            {
                observer.AddQueue();
            }

            if (CurrentState.UpdateQueue!.Count > 10e5)
            {
                CurrentState.UpdateQueue = [];
                throw new Exception("Potential Infinite Loop Detected.");
            }

            return Constant.EmptyObj;
        }, false);
        return value;
    }

    internal static void RunUpdates(Action fn, bool init)
    {
        RunUpdates(Util.WrapAction(fn), init);
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

    private static void RunQueue(ICollection<ComputationNode> queues)
    {
        foreach (var queue in queues)
        {
            queue.RunTop();
        }
    }

    private static void RunUserEffects(List<ComputationNode> queue)
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

    internal static T Untrack<T>(Func<T> fn)
    {
        if (CurrentState.CurrentComputation is null) return fn();

        var currentComputation = CurrentState.CurrentComputation;
        CurrentState.CurrentComputation = null;
        try
        {
            return fn();
        }
        finally
        {
            CurrentState.CurrentComputation = currentComputation;
        }
    }

    internal static void Untrack(Action fn)
    {
        Untrack(Util.WrapAction(fn));
    }

    internal static T Untrack<T>(IReadOnlySignal<T> source)
    {
        return Untrack(() => source.Value);
    }

    internal static T? RunWithOwner<T>(Owner? o, Func<T> fn)
    {
        var tempOwner = CurrentState.CurrentOwner;
        var tempComputation = CurrentState.CurrentComputation;
        CurrentState.CurrentOwner = o;
        CurrentState.CurrentComputation = null;
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
            CurrentState.CurrentOwner = tempOwner;
            CurrentState.CurrentComputation = tempComputation;
        }

        return default;
    }

    internal static void HandleError(Exception err, Owner? owner = null)
    {
        owner ??= CurrentState.CurrentOwner;
        ICollection<Action<object>>? fns = null;
        if (CurrentState.Error is not null)
        {
            fns = owner?.Context?[CurrentState.Error] as ICollection<Action<object>>;
        }

        var error = CastError(err);
        if (fns is null) throw error;

        if (CurrentState.EffectQueue is not null)
        {
            var handler = new Computation<object>(Util.WrapActionWithArg(() => RunErrors(error, fns, owner)),
                Constant.EmptyObj, pure: false, phase: ComputationPhase.Stale);
            handler.Phase = ComputationPhase.Stale;
            CurrentState.EffectQueue.Add(handler);
        }
        else RunErrors(error, fns, owner);
    }

    internal static Exception CastError(object err)
    {
        if (err is Exception exception) return exception;
        return new Exception(err is string errorStr ? errorStr : "Unknown error");
    }

    private static void RunErrors(object err, ICollection<Action<object>> fns, Owner? owner)
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

public interface ISignalState
{
    List<ComputationNode>? Observers { get; set; }
    List<int>? ObserverSlots { get; set; }
    internal void UpdateIfNeeded(ComputationNode? ignore = null);
}

public interface ISignalState<T> : ISignalState
{
    T Value { get; internal set; }
    Func<T, T, bool>? Comparator { get; }
    T ReadSignal();
    T WriteSignal(T value);
}

public class SignalState<T>(T value) : ISignalState<T>
{
    public List<ComputationNode>? Observers { get; set; }
    public List<int>? ObserverSlots { get; set; }
    public Func<T, T, bool>? Comparator { get; set; }
    public virtual T Value { get; set; } = value;

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
    public Owner? Parent = parent;
    public List<ComputationNode>? Children = children;
    public List<Action>? Cleanups = cleanups;
    public Dictionary<string, object?>? Context = context;

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

public class ComputationNode : Owner
{
    public ComputationPhase Phase;
    public List<ISignalState>? Sources;
    public List<int>? SourceSlots;
    protected long UpdatedAt;
    public readonly bool Pure;
    public bool User;

    public ComputationNode(
        ComputationPhase phase = ComputationPhase.Stale,
        List<ISignalState>? sources = null,
        List<int>? sourceSlots = null,
        long updatedAt = 0,
        bool pure = false,
        bool user = false,
        Owner? parent = null,
        List<ComputationNode>? children = null,
        List<Action>? cleanups = null,
        Dictionary<string, object?>? context = null
    ) : base(parent, children, cleanups, context)
    {
        Phase = phase;
        Sources = sources;
        SourceSlots = sourceSlots;
        UpdatedAt = updatedAt;
        Pure = pure;
        User = user;

        if (CurrentState.CurrentOwner is null)
            Console.WriteLine(
                "computations created outside a `createRoot` or `render` will never be disposed"
            );
        else if (CurrentState.CurrentOwner != Constant.UnOwned)
        {
            if (CurrentState.CurrentOwner.Children is null)
                CurrentState.CurrentOwner.Children = [this];
            else CurrentState.CurrentOwner.Children.Add(this);
        }
    }

    public void AddQueue()
    {
        if (Phase == ComputationPhase.Resolved)
        {
            if (Pure) CurrentState.UpdateQueue!.Add(this);
            else CurrentState.EffectQueue!.Add(this);
            MarkDownstream();
        }

        Phase = ComputationPhase.Stale;
    }

    internal virtual void MarkDownstream()
    {
    }

    public void UpdateComputation()
    {
        CleanNode(); // 动态依赖切换，先断开旧依赖
        var time = CurrentState.ExecCount;
        RunComputation(time);
    }

    public virtual void RunComputation(long time)
    {
    }

    public void RunTop()
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
            if (!(Parent is ComputationNode parent && parent.UpdatedAt <= CurrentState.ExecCount)) break;
            if (parent.Phase != ComputationPhase.Resolved) ancestors.Add(node);
            node = parent;
        }

        // 从根owner开始执行
        for (var i = ancestors.Count - 1; i >= 0; i--)
        {
            node = ancestors[i];

            if (node.Phase == ComputationPhase.Stale)
            {
                node.UpdateComputation();
            }
            else if (node.Phase == ComputationPhase.Pending)
            {
                var updates = CurrentState.UpdateQueue;
                CurrentState.UpdateQueue = null;
                Common.RunUpdates(() => LookUpstream(ancestors[0]), false);
                CurrentState.UpdateQueue = updates;
            }
        }
    }

    /**
     * 在真正执行前，向上修复所有脏依赖，保证拓扑顺序正确
     */
    internal void LookUpstream(ComputationNode? ignore = null)
    {
        Phase = ComputationPhase.Resolved;
        for (var i = 0; i < Sources!.Count; i += 1)
        {
            Sources![i].UpdateIfNeeded();
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
}

public class ComputationNode<T>(
    Func<T, T> fn,
    T value,
    ComputationPhase phase = ComputationPhase.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
) : ComputationNode(phase, sources, sourceSlots,
    updatedAt, pure, user, parent, children, cleanups, context)
{
    public readonly Func<T, T> Fn = fn;
    public T Value = value;

    protected virtual void UpdateValue(T value)
    {
        Value = value;
    }

    public override void RunComputation(long time)
    {
        T nextValue;
        var tempOwner = CurrentState.CurrentOwner;
        var tempComputation = CurrentState.CurrentComputation;
        CurrentState.CurrentOwner = CurrentState.CurrentComputation = this;
        try
        {
            nextValue = Fn(Value);
        }
        catch (Exception err)
        {
            if (Pure)
            {
                Phase = ComputationPhase.Stale;
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
            CurrentState.CurrentComputation = tempComputation;
            CurrentState.CurrentOwner = tempOwner;
        }

        if (UpdatedAt > time) return;
        UpdateValue(nextValue);
        UpdatedAt = time;
    }
}

public class Computation<T>(
    Func<T, T> fn,
    T value,
    ComputationPhase phase = ComputationPhase.Stale,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
) : ComputationNode<T>(fn, value, phase, sources, sourceSlots, updatedAt, pure, user,
    parent, children, cleanups, context);

public class Memo<T>(
    Func<T, T> fn,
    ComputationPhase phase = ComputationPhase.Resolved,
    T? value = default,
    Func<T, T, bool>? comparator = null,
    List<ComputationNode>? observers = null,
    List<int>? observerSlots = null,
    List<ISignalState>? sources = null,
    List<int>? sourceSlots = null,
    long updatedAt = 0,
    bool pure = false,
    bool user = false,
    Owner? parent = null,
    List<ComputationNode>? children = null,
    List<Action>? cleanups = null,
    Dictionary<string, object?>? context = null
) : Computation<T>(fn, value!, phase, sources, sourceSlots, updatedAt, pure, user, parent, children, cleanups, context),
    ISignalState<T>
{
    public Func<T, T, bool>? Comparator { get; } = comparator;
    public List<ComputationNode>? Observers { get; set; } = observers;
    public List<int>? ObserverSlots { get; set; } = observerSlots;

    public new T Value { get; set; } = value!;

    protected override void UpdateValue(T value)
    {
        WriteSignal(value);
    }

    /**
     * 递归向下传播脏状态
     */
    internal override void MarkDownstream()
    {
        foreach (var observer in Observers!)
        {
            if (observer.Phase != ComputationPhase.Resolved) continue;
            observer.Phase = ComputationPhase.Pending;
            if (observer.Pure) CurrentState.UpdateQueue!.Add(observer);
            else CurrentState.EffectQueue!.Add(observer);
            observer.MarkDownstream();
        }
    }

    public void UpdateIfNeeded(ComputationNode? ignore = null)
    {
        var state = Phase;
        switch (state)
        {
            case ComputationPhase.Stale:
                if (this != ignore && (UpdatedAt < CurrentState.ExecCount))
                    RunTop();
                break;
            case ComputationPhase.Pending:
                LookUpstream(ignore);
                break;
        }
    }

    public T ReadSignal()
    {
        if (Phase == ComputationPhase.Stale) UpdateComputation();
        else
        {
            var updates = CurrentState.UpdateQueue;
            CurrentState.UpdateQueue = null;
            Common.RunUpdates(() =>
            {
                LookUpstream();
                return Constant.EmptyObj;
            }, false);
            CurrentState.UpdateQueue = updates;
        }

        return Common.BaseReadSignal(this);
    }

    public T WriteSignal(T value)
    {
        return Common.WriteSignal(this, value);
    }
}

public class Context<T>(string id = "context", T defaultValue = default!)
{
    public string Id = id;
    public T DefaultValue = defaultValue;

    public void Provider(T value, Action fn)
    {
        var owner = CurrentState.CurrentOwner;
        if (owner is null) throw new Exception("CurrentOwner is None!");
        owner.Context ??= new Dictionary<string, object?>();
        owner.Context[Id] = value;
        fn();
    }
}

internal class SimpleReadOnlySignal<T>(T source) : IReadOnlySignal<T>
{
    public T Value { get; } = source;
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
    private Owner? _owner = CurrentState.CurrentOwner;
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
        return Common.RunWithOwner(_owner, () => _load(info));
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
            _loadEnd(_task, Common.Untrack(_value));
            return null;
        }

        object? error = null;

        object? p;
        if (_initTask == Constant.EmptyObj)
        {
            p = Common.Untrack(() =>
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
            _loadEnd(_task, default, Common.CastError(error), lookup);
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
        Common.RunUpdates(() => { _state.Value = _resolved ? ResourcePhase.Refreshing : ResourcePhase.Pending; },
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
                return _loadEnd(_task, default, Common.CastError(e), lookup);
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
        Common.RunUpdates(() =>
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