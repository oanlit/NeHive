namespace NeHive.Core.Tests;

public class AsyncMemoTest
{
    [Fact]
    public async Task Resource_Async_Fetch_Test()
    {
        var source = new Signal<int>(1);

        var res = new AsyncMemo<int>(async () =>
            {
                var result = source.Value * 2;
                await Task.Delay(10);
                return result;
            }
        );

        Assert.True(res.Loading);

        await Task.Delay(50);
        
        Assert.Equal(AsyncMemoState.Ready, res.State);
        Assert.Equal(2, res.Value);
    }

    [Fact]
    public async Task Resource_Should_Reload_When_Source_Changes()
    {
        var source = new Signal<int>(1);

        var resource = new AsyncMemo<int>(async epochScope =>
            {
                await Task.Delay(10);
                return epochScope.Track(source) * 2;
            }
        );

        await Task.Delay(50);
        Assert.Equal(2, resource.Value);

        source.Value = 2;

        await Task.Delay(50);
        Assert.Equal(4, resource.Value);
    }

    [Fact]
    public async Task Resource_Should_Enter_Pending_When_Source_Changes()
    {
        var source = new Signal<int>(1);

        var resource = new AsyncMemo<int>(async () =>
            {
                var result = source.Value;
                await Task.Delay(50);
                return result;
            }
        );

        await Task.Delay(100);
        Assert.False(resource.Loading);

        source.Value = 2;
        Assert.True(resource.Loading); // 关键点

        await Task.Delay(100);

        Assert.Equal(AsyncMemoState.Ready, resource.State);
    }

    [Fact]
    public async Task State_Should_Be_Reactive()
    {
        var source = new Signal<int>(1);

        var resource = new AsyncMemo<int>(async () =>
            {
                await Task.Delay(20);
                return source.Value;
            }
        );

        var runs = 0;
        AsyncMemoState last = default;

        using var effect = new Effect(() =>
        {
            runs++;
            last = resource.State;
        });

        source.Value = 2;

        await Task.Delay(100);

        Assert.Equal(2, runs);
        Assert.Equal(AsyncMemoState.Ready, last);
    }
}