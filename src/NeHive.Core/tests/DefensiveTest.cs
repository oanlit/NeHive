namespace NeHive.Core.Tests;

public class ReactiveLoopDetectionTests
{
    [Fact]
    public void Infinite_Loop_Should_Throw()
    {
        var signal = new MutSignal<int>(0);
        Assert.Throws<Core.InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                signal.RxValue++; // 读取并修改同一个信号，形成直接循环
            });
        });
    }

    [Fact]
    public void Effect_IndirectlyModifiesOwnDependency_ThrowsInfiniteReactiveLoopException()
    {
        var a = new MutSignal<int>(0);
        var b = new MutSignal<int>(1);

        Assert.Throws<Core.InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                // Effect 依赖 a 和 b
                var x = a.RxValue + b.RxValue;
                // 修改 a 会影响 Effect 自身
                a.RxValue = x;
            });
        });
    }
    
    [Fact]
    public void Batch_WithoutUntrack_StillTriggersLoopDetection()
    {
        var signal = new MutSignal<int>(0);
        Assert.Throws<Core.InfiniteReactiveLoopException>(() =>
        {
            using var effect = new Effect(() =>
            {
                Reactive.Batch(() =>
                {
                    // Batch 只延迟通知，但依赖关系仍然建立
                    signal.RxValue++;
                });
            });
        });
    }

    [Fact]
    public void BatchUntrack_PreventsInfiniteLoopByNotTrackingDependency()
    {
        var signal = new MutSignal<int>(0);
        var effectRunCount = 0;

        // 使用 BatchUntrack 包裹修改逻辑，避免建立依赖
        using var effect = new Effect(() =>
        {
            effectRunCount++;
            Reactive.BatchUntrack(() =>
            {
                // 这里的读取不会建立依赖，修改也不会触发自身重入
                signal.RxValue++;
                signal.RxValue++;
            });
        });

        // Effect 首次运行：读取 0，写入 2
        Assert.Equal(1, effectRunCount);
        Assert.Equal(2, signal.RxValue);

        // 由于没有依赖，后续手动修改信号不会触发 Effect
        signal.RxValue = 10;
        Assert.Equal(1, effectRunCount);
        Assert.Equal(10, signal.RxValue);
    }
}

public class NestedEffectTest
{
    [Fact]
    public void Nested_Effect_With_Write_Test()
    {
        var a = new MutSignal<int>(1);
    
        var outer = 0;
        var inner = 0;
    
        using var effect = new Effect(() =>
        {
            outer++;
            _ = a.RxValue;
    
            _ = new Effect(() =>
            {
                inner++;
                if (a.RxValue < 3)
                    a.RxValue++;
            });
        });
    
        Assert.True(a.RxValue >= 3);
        Assert.Equal(3, outer);
        Assert.Equal(3, inner);
    }

    [Fact]
    public void Owner_Clean_Nested_Effect_Test()
    {
        MutSignal<int> s = new(0);
        List<int> logs = [];
    
        var scope = new Scope();
    
        scope.CreateEffect(() =>
        {
            logs.Add(s.RxValue);
    
            _ = new Effect(() => { logs.Add(s.RxValue * 10); });
        });
    
        s.RxValue = 1;
        Assert.Equal([0, 0, 1, 10], logs);
    
        scope.Clean();
    
        s.RxValue = 2;
    
        // ❗所有嵌套 effect 都应停止
        Assert.Equal([0, 0, 1, 10], logs);
    }

    // [Fact]
    // public void No_Zombie_Effect_Test()
    // {
    //     MutSignal<int> s = new(0);
    //     List<int> values = [];
    //
    //     var e = new Effect(() =>
    //     {
    //         if (s.RxValue > 0)
    //         {
    //             _ = new Effect(() => { values.Add(s.RxValue); });
    //         }
    //     });
    //
    //     s.RxValue = 1; // 创建子 effect
    //     s.RxValue = 2;
    //
    //     e.Dispose();
    //
    //     s.RxValue = 3;
    //
    //     // 如果有 zombie，这里会继续写
    //     Assert.DoesNotContain(3, values);
    // }
}