namespace Test;

using Lib;

public class LifeCycleTest
{
    [Fact]
    public void Effect_Dispose_Unsubscribe_Test()
    {
        Signal<int> s = new(0);
        List<int> values = [];

        var e = new Effect(() => { values.Add(s.Value); });

        s.Value = 1;
        Assert.Equal([0, 1], values);

        e.Dispose();

        s.Value = 2;

        // 不应再触发
        Assert.Equal([0, 1], values);
    }

    [Fact]
    public void Cleanup_Order_Test()
    {
        List<string> logs = [];

        var owner = new Owner();

        owner.AddEffect(() =>
        {
            Reactive.OnCleanup(() => logs.Add("cleanup"));

            _ = new Effect(() =>
            {
                logs.Add("child run");
                Reactive.OnCleanup(() => logs.Add("child dispose"));
            });
        });

        Assert.Equal(
            ["child run"],
            logs
        );
        logs.Clear();

        owner.Dispose();

        Assert.Equal(
            ["child dispose", "cleanup"],
            logs
        );
    }

    [Fact]
    public void Owner_And_Standalone_Effect_Test()
    {
        Signal<int> s = new(0);

        List<int> a = [];
        List<int> b = [];

        var owner = new Owner();

        var e1 = owner.AddEffect(() => a.Add(s.Value));

        var e2 = new Effect(() => b.Add(s.Value));

        s.Value = 1;

        Assert.Equal([0, 1], a);
        Assert.Equal([0, 1], b);

        owner.Dispose();
        Assert.True(owner.IsDisposed);
        Assert.True(e1.IsInvalid);
        Assert.False(e2.IsInvalid);

        s.Value = 2;

        // owner 内的停止
        Assert.Equal([0, 1], a);

        // 独立 effect 继续
        Assert.Equal([0, 1, 2], b);

        e2.Dispose();
        Assert.True(e2.IsInvalid);

        s.Value = 3;

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
        Signal<int> a = new(1);
        Signal<int> b = new(10);
        Signal<bool> flag = new(true);

        List<int> values = [];

        using var e = new Effect(() => { values.Add(flag.Value ? a.Value : b.Value); });

        a.Value = 2;
        Assert.Equal([1, 2], values);

        flag.Value = false;

        b.Value = 20;
        Assert.Equal([1, 2, 10, 20], values);

        a.Value = 3;

        // ❗不能再触发（如果触发说明旧依赖没清）
        Assert.Equal([1, 2, 10, 20], values);
    }

    [Fact]
    public void Owner_Clean_Reuse_Test()
    {
        Signal<int> s = new(0);
        List<int> values = [];

        using var owner = new Owner();

        var e = owner.AddEffect(() => values.Add(s.Value));
        Assert.False(e.IsInvalid);

        s.Value = 1;
        Assert.Equal([0, 1], values);

        owner.Clean();
        Assert.False(owner.IsDisposed);
        Assert.True(e.IsInvalid);

        s.Value = 2;

        // ❗旧 effect 不应再触发
        Assert.Equal([0, 1], values);

        // 重新挂
        owner.AddEffect(() => values.Add(s.Value));

        s.Value = 3;

        Assert.Equal([0, 1, 2, 3], values);
    }

    [Fact]
    public void Clean_Root_Test()
    {
        Signal<int> s = new(0);
        List<int> values = [];
    
        using var owner = new Owner();
    
        var e1 = owner.AddEffect(() => values.Add(s.Value));
        Assert.False(e1.IsInvalid);
    
        s.Value = 1;
        Assert.Equal([0, 1], values);
    
        var root = Reactive.RootOwner;
    
        root.Dispose();
        Assert.True(owner.IsDisposed);
        Assert.True(e1.IsInvalid);
    
        s.Value = 2;
        Assert.Equal([0, 1], values);
    }

    [Fact]
    public void Owner_Clean_No_Double_Cleanup_Test()
    {
        List<int> logs = [];
        var owner = new Owner();

        owner.AddCleanup(() => logs.Add(1));

        owner.Clean();
        owner.Clean();

        Assert.Equal([1], logs);
    }

    [Fact]
    public void Owner_Dispose_Should_Throw_Test()
    {
        var owner = new Owner();

        owner.Dispose();

        Assert.Throws<ObjectDisposedException>(() => { owner.AddEffect(() => { }); });
    }
}