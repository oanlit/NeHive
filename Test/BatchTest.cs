namespace Test;

using Lib;

public class BatchTest
{
    [Fact]
    public void Nested_Batch_Should_Behave_As_One_Batch()
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

            Reactive.Batch(() =>
            {
                a.Value = 2;
                a.Value = 3;
            });

            a.Value = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Dynamic_Nested_Batch_Should_Work()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.Value == 1)
            {
                Reactive.Batch(() =>
                {
                    a.Value = 2;
                    a.Value = 3;
                });
            }
        });
        Assert.Equal(1, runs);

        Reactive.Batch(() => { a.Value = 1; });

        // Assert.Equal(2, runs);
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Effect_Chain_Should_Batch()
    {
        var a = new Signal<int>(0);
        var b = new Signal<int>(0);

        int runsA = 0;
        int runsB = 0;

        using var effectA = new Effect(() =>
        {
            runsA++;
            if (a.Value > 0)
                b.Value = a.Value;
        });

        using var effectB = new Effect(() =>
        {
            runsB++;
            _ = b.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;
            a.Value = 2;
            a.Value = 3;
        });

        Assert.Equal(2, runsA);
        Assert.Equal(2, runsB);
    }

    [Fact]
    public void Memo_And_Effect_Should_Batch_Together()
    {
        var a = new Signal<int>(1);
        using var owner = new Owner();

        var m = owner.AddMemo(() => a.Value + 1);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = m.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 2;
            a.Value = 3;
            a.Value = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Read_Inside_Batch_Should_Not_Trigger_Extra_Run()
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
            _ = a.Value; // read
            a.Value = 2;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Large_Batch_Should_Run_Once()
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
            for (int i = 0; i < 10000; i++)
                a.Value = i;
        });

        Assert.Equal(2, runs);
    }
}

public class ReactiveLoopDetectionTests
{
    [Fact]
    public void Effect_DirectlyModifiesOwnDependency_ThrowsInfiniteReactiveLoopException()
    {
        var signal = new Signal<int>(0);
        Assert.Throws<InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                signal.Value++; // 读取并修改同一个信号，形成直接循环
            });
        });
    }

    [Fact]
    public void Effect_IndirectlyModifiesOwnDependency_ThrowsInfiniteReactiveLoopException()
    {
        var a = new Signal<int>(0);
        var b = new Signal<int>(1);

        Assert.Throws<InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                // Effect 依赖 a 和 b
                var x = a.Value + b.Value;
                // 修改 a 会影响 Effect 自身
                a.Value = x;
            });
        });
    }

    [Fact]
    public void BatchUntrack_PreventsInfiniteLoopByNotTrackingDependency()
    {
        var signal = new Signal<int>(0);
        var effectRunCount = 0;

        // 使用 BatchUntrack 包裹修改逻辑，避免建立依赖
        using var effect = new Effect(() =>
        {
            effectRunCount++;
            Reactive.BatchUntrack(() =>
            {
                // 这里的读取不会建立依赖，修改也不会触发自身重入
                signal.Value++;
                signal.Value++;
            });
        });

        // Effect 首次运行：读取 0，写入 2
        Assert.Equal(1, effectRunCount);
        Assert.Equal(2, signal.Value);

        // 由于没有依赖，后续手动修改信号不会触发 Effect
        signal.Value = 10;
        Assert.Equal(1, effectRunCount);
        Assert.Equal(10, signal.Value);

        // 当然，也可以让 Effect 继续安全地自增（但需要外部触发）
        // 例如通过另一个信号来触发它，但在这个测试里不必要。
    }

    [Fact]
    public void Batch_WithoutUntrack_StillTriggersLoopDetection()
    {
        var signal = new Signal<int>(0);
        Assert.Throws<InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                Reactive.Batch(() =>
                {
                    // Batch 只延迟通知，但依赖关系仍然建立
                    signal.Value++;
                });
            });
        });
    }
}