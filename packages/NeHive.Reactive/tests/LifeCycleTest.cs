using NeHive.Model;
namespace NeHive.Reactive.Tests;

public class LifeCycleTest
{
    [Fact]
    public void Effect_Dispose_Unsubscribe_Test()
    {
        MutSignal<int> s = new(0);
        List<int> values = [];

        var e = new Effect(() => { values.Add(s.RxValue); });

        s.RxValue = 1;
        Assert.Equal([0, 1], values);

        e.Dispose();
        Assert.True(e.IsInvalid);

        s.RxValue = 2;

        // 不应再触发
        Assert.Equal([0, 1], values);
    }

    [Fact]
    public void OnDispose_Should_Belong_To_EffectScope_Not_EpochScope()
    {
        var cleaned = false;

        var effect = new Effect(selfScope =>
        {
            selfScope.OnCleanup += () => cleaned = true;

            return _ =>
            {
                // epoch 上不注册 cleanup
            };
        });

        Assert.False(cleaned);

        effect.Dispose();

        Assert.True(cleaned);
    }

    [Fact]
    public void EpochScope_Cleanup_Should_Run_On_ReExecution()
    {
        var signal = new MutSignal<int>(0);

        var cleanupCount = 0;

        using var effect = new Effect(epoch =>
        {
            _ = epoch.Pull(signal);
            epoch.OnCleanup += () => cleanupCount++;
        });

        Assert.Equal(0, cleanupCount);

        signal.RxValue = 1;
        Assert.Equal(1, cleanupCount);
    }

    [Fact]
    public void Setup_Should_Run_Only_Once()
    {
        var signal = new MutSignal<int>(0);

        var setupCount = 0;
        var executeCount = 0;

        using var effect = new Effect(scope =>
        {
            setupCount++;

            return epochScope =>
            {
                _ = epochScope.Pull(signal);
                executeCount++;
            };
        });

        Assert.Equal(1, setupCount);
        Assert.Equal(1, executeCount);

        signal.RxValue = 1;

        Assert.Equal(1, setupCount);
        Assert.Equal(2, executeCount);
    }

    [Fact]
    public void Setup_Should_Not_Track_Dependencies()
    {
        var signal = new MutSignal<int>(0);

        var setupCount = 0;

        using var effect = new Effect(selfScope =>
        {
            setupCount++;

            _ = signal.RxValue; // ❗如果被 tracking 就会出问题

            return _ => { };
        });

        Assert.Equal(1, setupCount);

        signal.RxValue = 1;

        // 不应重新执行 setup
        Assert.Equal(1, setupCount);
    }

    [Fact]
    public void Cleanup_Should_Run_Before_Next_Execution()
    {
        var signal = new MutSignal<int>(0);

        var log = new List<string>();

        using var effect = new Effect(selfScope =>
        {
            return epochScope =>
            {
                _ = epochScope.Pull(signal);
                epochScope.OnCleanup += () => log.Add("cleanup");
                log.Add("run");
            };
        });

        Assert.Equal(["run"], log);

        signal.RxValue = 1;

        Assert.Equal(["run", "cleanup", "run"], log);
    }

    [Fact]
    public void Cleanup_Should_Not_Accumulate()
    {
        var signal = new MutSignal<int>(0);
        var cleanupCount = 0;

        using var effect = new Effect(selfScope =>
        {
            return epochScope =>
            {
                _ = epochScope.Pull(signal);
                epochScope.OnCleanup += () => cleanupCount++;
            };
        });

        signal.RxValue = 1;
        Assert.Equal(1, cleanupCount);

        signal.RxValue = 2;
        Assert.Equal(2, cleanupCount); // 不是 3！
    }

    [Fact]
    public void Final_Cleanup_Should_Run_On_Dispose()
    {
        var cleaned = false;

        var effect = new Effect(epoch => { epoch.OnCleanup += () => cleaned = true; });

        Assert.False(cleaned);

        effect.Dispose();

        Assert.True(cleaned);
    }

    [Fact]
    public void Cleanup_Order_Test()
    {
        List<string> logs = [];

        var scope = new Scope();

        scope.CreateEffect(() =>
        {
            Rx.OnDispose(() => logs.Add("cleanup"));

            _ = new Effect(() =>
            {
                logs.Add("child run");
                Rx.OnDispose(() => logs.Add("child dispose"));
            });
        });

        Assert.Equal(
            ["child run"],
            logs
        );
        logs.Clear();

        scope.Dispose();

        Assert.Equal(
            ["child dispose", "cleanup"],
            logs
        );
    }

    [Fact]
    public void Owner_And_Standalone_Effect_Test()
    {
        MutSignal<int> s = new(0);

        List<int> a = [];
        List<int> b = [];

        var scope = new Scope();

        var e1 = scope.CreateEffect(() => a.Add(s.RxValue));

        var e2 = new Effect(() => b.Add(s.RxValue));

        s.RxValue = 1;

        Assert.Equal([0, 1], a);
        Assert.Equal([0, 1], b);

        scope.Dispose();
        Assert.True(scope.IsDisposed);
        Assert.True(e1.IsInvalid);
        Assert.False(e2.IsInvalid);

        s.RxValue = 2;

        // scope 内的停止
        Assert.Equal([0, 1], a);

        // 独立 effect 继续
        Assert.Equal([0, 1, 2], b);

        e2.Dispose();
        Assert.True(e2.IsInvalid);

        s.RxValue = 3;

        Assert.Equal([0, 1, 2], b);
    }

    [Fact]
    public void Dispose_Idempotent_Test()
    {
        var e = new Effect(() => { });

        e.Dispose();
        e.Dispose(); // 不应抛异常
    }

    [Fact]
    public void Dynamic_Dependency_Switch_Test()
    {
        MutSignal<int> a = new(1);
        MutSignal<int> b = new(10);
        MutSignal<bool> flag = new(true);

        List<int> values = [];

        using var e = new Effect(() => { values.Add(flag.RxValue ? a.RxValue : b.RxValue); });

        a.RxValue = 2;
        Assert.Equal([1, 2], values);

        flag.RxValue = false;

        b.RxValue = 20;
        Assert.Equal([1, 2, 10, 20], values);

        a.RxValue = 3;

        // ❗不能再触发（如果触发说明旧依赖没清）
        Assert.Equal([1, 2, 10, 20], values);
    }

    [Fact]
    public void Owner_Clean_Reuse_Test()
    {
        MutSignal<int> s = new(0);
        List<int> values = [];

        using var scope = new Scope();

        var e = scope.CreateEffect(() => values.Add(s.RxValue));
        Assert.False(e.IsInvalid);

        s.RxValue = 1;
        Assert.Equal([0, 1], values);

        scope.DisposeChildren();
        Assert.False(scope.IsDisposed);
        Assert.True(e.IsInvalid);

        s.RxValue = 2;

        // ❗旧 effect 不应再触发
        Assert.Equal([0, 1], values);

        // 重新挂
        scope.CreateEffect(() => values.Add(s.RxValue));

        s.RxValue = 3;

        Assert.Equal([0, 1, 2, 3], values);
    }
    //
    // [Fact]
    // public void Clean_Root_Test()
    // {
    //     MutSignal<int> s = new(0);
    //     List<int> values = [];
    //
    //     using var scope = new Scope();
    //
    //     var e1 = scope.CreateEffect(() => values.Add(s.RxValue));
    //     Assert.False(e1.IsInvalid);
    //
    //     s.RxValue = 1;
    //     Assert.Equal([0, 1], values);
    //
    //     var root = Scope.RootScope;
    //
    //     root.Dispose();
    //     Assert.True(scope.IsDisposed);
    //     Assert.True(e1.IsInvalid);
    //
    //     s.RxValue = 2;
    //     Assert.Equal([0, 1], values);
    // }

    [Fact]
    public void Owner_Dispose_Should_Throw_Test()
    {
        var scope = new Scope();

        scope.Dispose();

        Assert.Throws<ObjectDisposedException>(() => { scope.CreateEffect(() => { }); });
    }
}