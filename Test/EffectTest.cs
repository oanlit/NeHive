namespace Test;

using Lib;

public class EffectTest
{
    [Fact]
    public void Effect_Self_Update_Test()
    {
        var a = new Signal<int>(1);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.Value < 3)
                a.Value++;
        });

        Assert.Equal(3, a.Value);
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Effect_Should_Not_Recurse_Immediately()
    {
        var a = new Signal<int>(1);
        int depth = 0;
        int maxDepth = 0;

        using var effect = new Effect(() =>
        {
            depth++;
            maxDepth = Math.Max(maxDepth, depth);

            if (a.Value < 10)
                a.Value++;

            depth--;
        });

        Assert.Equal(2, maxDepth); // ❗ 不能递归执行
    }

    [Fact]
    public void Cross_Effect_Update_Order_Test()
    {
        var a = new Signal<int>(1);
        var b = new Signal<int>(0);

        var logs = new List<string>();

        using var e1 = new Effect(() =>
        {
            logs.Add($"e1:{a.Value}");
            b.Value = a.Value * 2;
        });

        using var e2 = new Effect(() => { logs.Add($"e2:{b.Value}"); });

        a.Value = 10;

        Assert.Equal("e1:10", logs[^2]);
        Assert.Equal("e2:20", logs[^1]);
    }

    [Fact]
    public void Effect_Multiple_Writes_Should_Batch()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;
            a.Value = 2;
            a.Value = 3;
        });

        Assert.Equal(2, runs);
        // 初始 1 次 + batch 后 1 次
    }

    [Fact]
    public void Effect_Memo_Effect_Chain_Test()
    {
        var a = new Signal<int>(1);

        using var owner = new Owner();

        var m = owner.AddMemo(() => a.Value + 1);

        int result = 0;

        owner.AddEffect(() => { result = m.Value; });

        a.Value = 10;

        Assert.Equal(11, result);
    }

    [Fact]
    public void Conditional_Write_Should_Not_Trigger_Extra()
    {
        var a = new Signal<int>(1);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.Value < 1)
                a.Value = 1;
        });

        Assert.Equal(1, runs);
    }

    [Fact]
    public void Nested_Effect_With_Write_Test()
    {
        var a = new Signal<int>(1);

        var outer = 0;
        var inner = 0;

        using var effect = new Effect(() =>
        {
            outer++;
            _ = a.Value;

            _ = new Effect(() =>
            {
                inner++;
                if (a.Value < 3)
                    a.Value++;
            });
        });

        Assert.True(a.Value >= 3);
        Assert.Equal(3, outer);
        Assert.Equal(3, inner);
    }

    // [Fact]
    // public void Infinite_Loop_Should_Throw()
    // {
    //     var a = new Signal<int>(0);
    //
    //     var effect = new Effect(() =>
    //     {
    //         if (a.Value < 10)
    //             a.Value++;
    //     });
    //     effect.Dispose();
    //     Assert.Throws<InfiniteReactiveLoopException>(() =>
    //     {
    //         using var effect2 = new Effect(() =>
    //         {
    //             a.Value++;
    //         });
    //     });
    // }
}