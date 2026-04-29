namespace NeHive.Core.Tests;

public class AsyncMemoTest
{
    [Fact]
    public async Task AsyncMemo_Simple_Use()
    {
        var source = new Signal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async () =>
        {
            var result = source.Value * 2;
            await Task.Delay(10);
            return result;
        });

        await Task.Delay(50);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.State);
        Assert.Equal(2, asyncMemo.Value);
    }

    [Fact]
    public async Task AsyncMemo_Should_Resolve_To_Value()
    {
        var source = new Signal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(10);
            return epoch.Track(source) * 2;
        });

        await Task.Delay(50);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.State);
        Assert.Equal(2, asyncMemo.Value);
    }

    [Fact]
    public async Task AsyncMemo_Should_Reload_When_Dependency_Changes()
    {
        var source = new Signal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(10);
            return epoch.Track(source) * 2;
        });

        await Task.Delay(50);
        Assert.Equal(2, asyncMemo.Value);

        source.Value = 2;

        await Task.Delay(50);
        Assert.Equal(4, asyncMemo.Value);
    }

    [Fact]
    public async Task AsyncMemo_Should_Enter_Loading_When_Dependency_Changes()
    {
        var source = new Signal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            var v = epoch.Track(source);
            await Task.Delay(50);
            return v;
        });

        await Task.Delay(100);
        Assert.Equal(AsyncMemoState.Ready, asyncMemo.State);

        source.Value = 2;

        Assert.True(asyncMemo.Loading);

        await Task.Delay(100);

        Assert.Equal(AsyncMemoState.Ready, asyncMemo.State);
        Assert.Equal(2, asyncMemo.Value);
    }
    
    [Fact]
    public async Task AsyncMemo_State_Should_Be_Reactive()
    {
        var source = new Signal<int>(1);

        var asyncMemo = new AsyncMemo<int>(async epoch =>
        {
            await Task.Delay(20);
            return epoch.Track(source);
        });

        var runs = 0;
        AsyncMemoState last = default;

        using var effect = new Effect(() =>
        {
            runs++;
            last = asyncMemo.State;
        });

        await Task.Delay(50);

        source.Value = 2;

        await Task.Delay(100);

        Assert.True(runs >= 2);
        Assert.Equal(AsyncMemoState.Ready, last);
    }
    
    [Fact]
    public async Task AsyncMemo_Setup_Should_Execute_Only_Once()
    {
        var setupRuns = 0;

        var source = new Signal<int>(1);

        _ = new AsyncMemo<int>(
            _ =>
            {
                setupRuns++;
                return async epoch =>
                {
                    await Task.Delay(10);
                    return epoch.Track(source) * 2;
                };
            }
        );

        await Task.Delay(50);
        source.Value = 2;
        await Task.Delay(50);

        Assert.Equal(1, setupRuns);
    }
}