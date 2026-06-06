namespace NeHive.Reactive.Tests;

public class AccessorTest
{
    [Fact]
    public void Accessor_FromValue_Should_Be_NonReactive()
    {
        Accessor<int> acc = 10;

        var rx = Rx.Track(() => { _ = acc.RxValue; });

        var value = Rx.Track(() => { _ = acc.Value; });

        Assert.Empty(rx);
        Assert.Empty(value);
        Assert.False(acc.IsReactive);
    }

    [Fact]
    public void Accessor_FromFunc_Should_Detect_Reactive_When_SignalUsed()
    {
        var signal = new MutSignal<int>(1);

        Accessor<int> acc = new(() => signal.RxValue);

        Assert.True(acc.IsReactive);

        var deps = Rx.Track(() => { _ = acc.RxValue; });

        Assert.Contains(signal, deps);
    }

    [Fact]
    public void Accessor_FromFunc_Should_Be_NonReactive_When_NoSignalUsed()
    {
        const int local = 42;

        Accessor<int> acc = new(() => local + 1);

        Assert.False(acc.IsReactive);

        var deps = Rx.Track(() => { _ = acc.RxValue; });

        Assert.Empty(deps);
    }
    
    [Fact]
    public void Accessor_FromSignal_Should_Be_Reactive()
    {
        var signal = new MutSignal<int>(1);

        Accessor<int> acc = signal;

        Assert.True(acc.IsReactive);

        var deps = Rx.Track(() =>
        {
            _ = acc.RxValue;
        });

        Assert.Single(deps);
        Assert.Same(signal, deps[0]);
    }
    
    [Fact]
    public void Accessor_Value_Should_Not_Track()
    {
        var signal = new MutSignal<int>(1);

        Accessor<int> acc = signal;

        var deps = Rx.Track(() =>
        {
            _ = acc.Value;
        });

        Assert.Empty(deps);
    }
    
    [Fact]
    public void Accessor_RxValue_And_Value_Should_Be_Separated()
    {
        var signal = new MutSignal<int>(1);

        Accessor<int> acc = signal;

        var rxDeps = Rx.Track(() =>
        {
            _ = acc.RxValue;
        });

        var valueDeps = Rx.Track(() =>
        {
            _ = acc.Value;
        });

        Assert.Single(rxDeps);
        Assert.Empty(valueDeps);
    }
}