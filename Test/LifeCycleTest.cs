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
    public void Memo_Dispose_Stop_Reactive_Test()
    {
        Signal<int> s = new(1);
        var memo = new Memo<int>(() => s.Value * 2);

        List<int> values = [];
        var e = new Effect(() => { values.Add(memo.Value); });

        Assert.Equal([2], values);

        s.Value = 2;
        Assert.Equal([2, 4], values);

        memo.Dispose();

        s.Value = 3;

        // memo 不再更新
        Assert.Equal([2, 4], values);

        e.Dispose();
    }

    [Fact]
    public void Memo_Dispose_No_Tracking_Test()
    {
        Signal<int> s = new(1);
        var memo = new Memo<int>(() => s.Value * 2);

        memo.Dispose();

        List<int> values = [];

        var e = new Effect(() => { values.Add(memo.Value); });

        s.Value = 2;

        // 如果还在 track，这里会再触发
        Assert.Single(values);

        e.Dispose();
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

        owner.AddEffect(() => a.Add(s.Value));

        var e = new Effect(() => b.Add(s.Value));

        s.Value = 1;

        Assert.Equal([0, 1], a);
        Assert.Equal([0, 1], b);

        owner.Dispose();

        s.Value = 2;

        // owner 内的停止
        Assert.Equal([0, 1], a);

        // 独立 effect 继续
        Assert.Equal([0, 1, 2], b);

        e.Dispose();

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
    public void No_Zombie_Effect_Test()
    {
        Signal<int> s = new(0);
        List<int> values = [];

        var e = new Effect(() =>
        {
            if (s.Value > 0)
            {
                _ = new Effect(() => { values.Add(s.Value); });
            }
        });

        s.Value = 1; // 创建子 effect
        s.Value = 2;

        e.Dispose();

        s.Value = 3;

        // 如果有 zombie，这里会继续写
        Assert.DoesNotContain(3, values);
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

        var owner = new Owner();

        owner.AddEffect(() => values.Add(s.Value));

        s.Value = 1;
        Assert.Equal([0, 1], values);

        owner.Clean();

        s.Value = 2;

        // ❗旧 effect 不应再触发
        Assert.Equal([0, 1], values);

        // 重新挂
        owner.AddEffect(() => values.Add(s.Value));

        s.Value = 3;

        Assert.Equal([0, 1, 2, 3], values);
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
    public void Owner_Clean_Nested_Effect_Test()
    {
        Signal<int> s = new(0);
        List<int> logs = [];

        var owner = new Owner();

        owner.AddEffect(() =>
        {
            logs.Add(s.Value);

            _ = new Effect(() => { logs.Add(s.Value * 10); });
        });

        s.Value = 1;
        Assert.Equal([0, 0, 1, 10], logs);

        owner.Clean();

        s.Value = 2;

        // ❗所有嵌套 effect 都应停止
        Assert.Equal([0, 0, 1, 10], logs);
    }
    
    [Fact]
    public void Owner_Dispose_Should_Throw_Test()
    {
        var owner = new Owner();

        owner.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
        {
            owner.AddEffect(() => { });
        });
    }
}