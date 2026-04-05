namespace Test;

using Lib;

public class BaseTest
{
    [Fact]
    public void Dynamic_Dependency_Switch_Test()
    {
        var a = new Signal<int>(1);
        var b = new Signal<int>(2);
        var flag = new Signal<bool>(true);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = flag.Value ? a.Value : b.Value;
        });

        Assert.Equal(1, runs);

        flag.Value = false;
        Assert.Equal(2, runs);

        a.Value = 10; // ❗ 不应触发
        Assert.Equal(2, runs);

        b.Value = 20; // ✅ 应触发
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Topological_Order_Test()
    {
        var a = new Signal<int>(1);
        var logs = new List<int>();

        using var owner = new Owner();

        var m1 = owner.AddMemo(() => a.Value + 1);
        var m2 = owner.AddMemo(() => m1.Value + 1);

        owner.AddEffect(() => { logs.Add(m2.Value); });

        a.Value = 10;

        Assert.Equal(12, logs[^1]); // 必须是最终值
    }

    [Fact]
    public void No_Glitch_Test()
    {
        var a = new Signal<int>(1);

        using var owner = new Owner();

        var m1 = owner.AddMemo(() => a.Value + 1);
        var m2 = owner.AddMemo(() => m1.Value + 1);

        int observed = 0;

        owner.AddEffect(() => { observed = m2.Value; });

        a.Value = 10;

        Assert.Equal(12, observed); // 不能出现 3、11 等中间值
    }
}