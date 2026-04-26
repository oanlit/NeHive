namespace NeHive.Core.Tests;

public class ResourceTest
{
    [Fact]
    public async Task Resource_Async_Fetch_Test()
    {
        var source = new Signal<int>(1);

        var res = new Resource<int, int, int>(
            async (s, _, _) =>
            {
                await Task.Delay(10);
                return s * 2;
            },
            source
        );

        Assert.True(res.Loading);

        await Task.Delay(20);

        Assert.Equal(2, res.Value);
        Assert.Equal(ResourceState.Ready, res.State);
    }
    
    [Fact]
    public async Task Resource_Should_Reload_When_Source_Changes()
    {
        var source = new Signal<int>(1);

        var resource = new Resource<int, int, object>(
            async (s, _, _) =>
            {
                await Task.Delay(10);
                return s * 2;
            },
            source
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

        var resource = new Resource<int, int, object>(
            async (s, _, _) =>
            {
                await Task.Delay(50);
                return s;
            },
            source
        );

        await Task.Delay(100);
        Assert.False(resource.Loading);

        source.Value = 2;
        Assert.True(resource.Loading); // 关键点

        await Task.Delay(100);

        Assert.Equal(ResourceState.Ready, resource.State);
    }
    
    [Fact]
    public async Task State_Should_Be_Reactive()
    {
        var source = new Signal<int>(1);

        var resource = new Resource<int, int, object>(
            async (s, _, _) =>
            {
                await Task.Delay(20);
                return s;
            },
            source
        );

        var runs = 0;
        ResourceState last = default;

        using var effect = new Effect(() =>
        {
            runs++;
            last = resource.State;
        });

        source.Value = 2;

        await Task.Delay(100);

        Assert.Equal(2, runs);
        Assert.Equal(ResourceState.Ready, last);
    }
}