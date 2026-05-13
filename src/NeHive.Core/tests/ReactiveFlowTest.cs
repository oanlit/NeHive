namespace NeHive.Core.Tests;

public class ReactiveFlowTest
{
    [Fact]
    public void PushEffect_Should_Create_Effect_And_Trigger_Value()
    {
        // ARRANGE
        using var root = Scope.RootScope;
        var signal = new Signal<int>(100);
        var effectValue = 0;

        // ACT
        root.CreateReactiveFlow(signal)
            .PushEffect(v => effectValue = v);

        // ASSERT
        // ✅ Effect 已执行，拿到初始值
        Assert.Equal(100, effectValue);
    }

    [Fact]
    public void Map_Should_Transform_Value()
    {
        // ARRANGE
        using var root = Scope.RootScope;
        var signal = new Signal<int>(5);
        int result = 0;

        // ACT
        root.CreateReactiveFlow(signal)
            .Map(v => v * 10)
            .PushEffect(v => result = v);

        // ASSERT
        // ✅ 5 * 10 = 50
        Assert.Equal(50, result);
    }

    [Fact]
    public void Filter_Should_Skip_Invalid_Values()
    {
        // ARRANGE
        using var root = Scope.RootScope;
        var signal = new Signal<int>(3);
        int effectCount = 0;

        // ACT
        root.CreateReactiveFlow(signal)
            .Filter(v => v > 5)
            .PushEffect(_ => effectCount++);

        // ASSERT
        // ✅ 3 不满足 >5，所以从未进入 effect
        Assert.Equal(0, effectCount);
    }

    [Fact]
    public void Signal_Change_Should_Trigger_Flow_Chain()
    {
        // ARRANGE
        using var root = Scope.RootScope;
        var signal = new Signal<int>(1);
        var last = 0;

        root.CreateReactiveFlow(signal)
            .Map(v => v * 2)
            .Filter(v => v > 5)
            .PushEffect(v => last = v);

        // ACT
        signal.RxValue = 4; // 4*2=8 → 通过
        signal.RxValue = 2; // 2*2=4 → 不通过

        // ASSERT
        Assert.Equal(8, last);
    }

    [Fact]
    public void Dispose_Scope_Should_Stop_Effect()
    {
        // ARRANGE
        var scope = Scope.RootScope.RunInScope(() => Scope.CurrentScope);
        var signal = new Signal<int>(10);
        var count = 0;

        scope.CreateReactiveFlow(signal)
            .PushEffect(_ => count++);

        // ACT
        scope.Dispose();
        signal.RxValue = 20;

        // ASSERT
        // ✅ 销毁后不再触发
        Assert.Equal(1, count);
    }
    
    [Fact]
    public void PushComputed_Should_Update_On_Input_Change()
    {
        using var scope = new Scope();

        var signal = new Signal<int>(1);

        var computed = scope.CreateReactiveFlow(signal)
            .Map(x => x + 10)
            .PushComputed(v => v * 2);

        Assert.Equal(22, computed.RxValue);

        signal.RxValue = 2;

        Assert.Equal(24, computed.RxValue);
    }
    
    [Fact]
    public async Task PushAsyncMemo_Should_Return_Async_Result()
    {
        using var scope = new Scope();

        var signal = new Signal<int>(1);

        var memo = scope.CreateReactiveFlow(signal)
            .PushAsyncMemo(async v =>
            {
                await Task.Delay(10);
                return v * 10;
            });

        await Task.Delay(50);

        Assert.Equal(10, memo.RxValue);
    }
    //
    // [Fact]
    // public async Task PushAsyncMemo_Should_Handle_Multiple_Updates()
    // {
    //     using var scope = new Scope();
    //
    //     var signal = new Signal<int>(1);
    //
    //     var memo = scope.CreateReactiveFlow(signal)
    //         .PushAsyncMemo(async v =>
    //         {
    //             await Task.Delay(20);
    //             return v;
    //         });
    //
    //     signal.RxValue = 2;
    //     signal.RxValue = 3;
    //
    //     await Task.Delay(100);
    //
    //     Assert.True(memo.RxValue is 2 or 3);
    // }
}