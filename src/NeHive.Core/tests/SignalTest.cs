namespace NeHive.Core.Tests;

public class SignalTest
{
    [Fact]
    public void Repeat_Read_Signal_Test()
    {
        Signal<int> a = new(0);
        var effect = new Effect(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = a.RxValue;
            }
        });

        Assert.Single(a.ValueSignal.Observers);
        Assert.Single(a.ValueSignal.ObserverSlots);
        Assert.Single(a.ValueSignal.Observers[0].Sources);
        Assert.Single(a.ValueSignal.Observers[0].SourceSlots);

        effect.Dispose();
    }

    [Fact]
    public void Repeat_Alternation_Read_Signal_Test()
    {
        Signal<int> a = new(0);
        Signal<int> b = new(0);

        var effect = new Effect(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = a.RxValue;
                _ = b.RxValue;
            }
        });

        Assert.Single(a.ValueSignal.Observers);
        Assert.Single(a.ValueSignal.ObserverSlots);
        Assert.Equal(2, a.ValueSignal.Observers[0].Sources.Count);
        Assert.Equal(2, a.ValueSignal.Observers[0].SourceSlots.Count);

        Assert.Single(b.ValueSignal.Observers);
        Assert.Single(b.ValueSignal.ObserverSlots);
        Assert.Equal(2, b.ValueSignal.Observers[0].Sources.Count);
        Assert.Equal(2, b.ValueSignal.Observers[0].SourceSlots.Count);

        effect.Dispose();
    }
}