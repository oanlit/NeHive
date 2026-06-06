namespace NeHive.Reactive.Tests;

public class TrackTest
{
    [Fact]
    public void RxTrack_Should_Capture_RxValue_Dependencies()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(2);
        var c = new MutSignal<int>(3);

        var signals = Rx.Track(() =>
        {
            _ = a.RxValue;
            _ = b.RxValue;
            _ = c.RxValue;
        });

        Assert.Equal(3, signals.Count);
        Assert.Contains(a, signals);
        Assert.Contains(b, signals);
        Assert.Contains(c, signals);
    }
    
    [Fact]
    public void RxTrack_Should_Not_Capture_Value_Access()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(2);

        var signals = Rx.Track(() =>
        {
            _ = a.Value;
            _ = b.RxValue;
        });

        Assert.Single(signals);
        Assert.Same(b, signals[0]);
    }
    
    [Fact]
    public void RxTrack_Should_Not_Duplicate_Signals()
    {
        var a = new MutSignal<int>(1);

        var signals = Rx.Track(() =>
        {
            _ = a.RxValue;
            _ = a.RxValue;
            _ = a.RxValue;
        });

        Assert.Single(signals);
    }
    
    [Fact]
    public void RxTrack_Should_Handle_Mixed_Access()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(2);

        var signals = Rx.Track(() =>
        {
            var x = a.RxValue + 10;
            if (x > 5)
            {
                _ = b.RxValue;
            }
        });

        Assert.Equal(2, signals.Count);
        Assert.Contains(a, signals);
        Assert.Contains(b, signals);
    }
    
    [Fact]
    public void RxTrack_Should_Be_Isolated_Per_Call()
    {
        var a = new MutSignal<int>(1);

        var r1 = Rx.Track(() => _ = a.RxValue);
        var r2 = Rx.Track(() => { });

        Assert.Single(r1);
        Assert.Empty(r2);
    }
}