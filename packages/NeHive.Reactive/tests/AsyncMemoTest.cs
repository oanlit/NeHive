namespace NeHive.Reactive.Tests;

public class AsyncMemoTest
{
    [Fact]
    public async Task AsyncMemo_Simple_Use()
    {
        var source = new MutSignal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async () =>
        {
            var result = source.RxValue * 2;
            await Task.Delay(10);
            return result;
        });

        await Task.Delay(50);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.RxState);
        Assert.Equal(2, asyncMemo.RxValue);
    }

    [Fact]
    public async Task AsyncMemo_Should_Resolve_To_Value()
    {
        var source = new MutSignal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(10);
            return epoch.Pull(source) * 2;
        });

        await Task.Delay(50);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.RxState);
        Assert.Equal(2, asyncMemo.RxValue);
    }

    [Fact]
    public async Task AsyncMemo_Should_Reload_When_Dependency_Changes()
    {
        var source = new MutSignal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(10);
            return epoch.Pull(source) * 2;
        });

        await Task.Delay(50);
        Assert.Equal(2, asyncMemo.RxValue);

        source.RxValue = 2;

        await Task.Delay(50);
        Assert.Equal(4, asyncMemo.RxValue);
    }

    [Fact]
    public async Task AsyncMemo_Should_Enter_Loading_When_Dependency_Changes()
    {
        var source = new MutSignal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            var v = epoch.Pull(source);
            await Task.Delay(50);
            return v;
        });

        await Task.Delay(100);
        Assert.Equal(AsyncMemoState.Ready, asyncMemo.RxState);

        source.RxValue = 2;

        Assert.True(asyncMemo.RxLoading);

        await Task.Delay(100);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.RxState);
        Assert.Equal(2, asyncMemo.RxValue);
    }
    
    [Fact]
    public async Task AsyncMemo_State_Should_Be_Reactive()
    {
        var source = new MutSignal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(20);
            return epoch.Pull(source);
        });

        var runs = 0;
        AsyncMemoState last = default;

        using var effect = new Effect(() =>
        {
            runs++;
            last = asyncMemo.RxState;
        });

        await Task.Delay(50);

        source.RxValue = 2;

        await Task.Delay(100);

        Assert.True(runs >= 2);
        Assert.Equal(AsyncMemoState.Ready, last);
    }
    
    [Fact]
    public async Task AsyncMemo_Setup_Should_Execute_Only_Once()
    {
        var setupRuns = 0;

        var source = new MutSignal<int>(1);

        _ = new AsyncMemo<int>(
            _ =>
            {
                setupRuns++;
                return async epoch =>
                {
                    await Task.Delay(10);
                    return epoch.Pull(source) * 2;
                };
            }
        );

        await Task.Delay(50);
        source.RxValue = 2;
        await Task.Delay(50);

        Assert.Equal(1, setupRuns);
    }
    
    [Fact]
    public async Task AsyncMemo_Should_Handle_Burst_Race()
    {
        var signal = new MutSignal<int>(1);
        var memo = new AsyncMemo<int>(async epoch =>
        {
            var v = epoch.Pull(signal);
            switch (v)
            {
                case 0:
                    return v;
                case 1:
                    await Task.Delay(100);
                    return v;
                default:
                    await Task.Delay(10);
                    return v;
            }
        });
        signal.RxValue = 1;
        signal.RxValue = 2;
        await Task.Delay(200);
        Assert.Equal(2, memo.Value);
    }
}