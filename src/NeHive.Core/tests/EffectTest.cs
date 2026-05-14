namespace NeHive.Core.Tests;

public class EffectTest
{
    [Fact]
    public void Effect_Self_Update_Test()
    {
        var a = new MutSignal<int>(1);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.RxValue < 3)
                a.RxValue++;
        });

        Assert.Equal(3, a.RxValue);
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Effect_Should_Not_Recurse_Immediately()
    {
        var a = new MutSignal<int>(1);
        var depth = 0;
        var maxDepth = 0;

        using var effect = new Effect(() =>
        {
            depth++;
            maxDepth = Math.Max(maxDepth, depth);

            if (a.RxValue < 10)
                a.RxValue++;

            depth--;
        });

        Assert.Equal(1, maxDepth); // ❗ 不能递归执行
    }

    [Fact]
    public void Cross_Effect_Update_Order_Test()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(0);

        var logs = new List<string>();

        using var e1 = new Effect(() =>
        {
            logs.Add($"e1:{a.RxValue}");
            b.RxValue = a.RxValue * 2;
        });

        using var e2 = new Effect(() => { logs.Add($"e2:{b.RxValue}"); });

        a.RxValue = 10;

        Assert.Equal("e1:10", logs[^2]);
        Assert.Equal("e2:20", logs[^1]);
    }

    [Fact]
    public void Conditional_Write_Should_Not_Trigger_Extra()
    {
        var a = new MutSignal<int>(1);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.RxValue < 1)
                a.RxValue = 1;
        });

        Assert.Equal(1, runs);
    }
}