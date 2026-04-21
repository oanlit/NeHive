using NeHive.Core;

namespace NeHive.Test;

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
                _ = a.Value;
            }
        });

        Assert.Single(a.State.Observers);
        Assert.Single(a.State.ObserverSlots);
        Assert.Single(a.State.Observers[0].Sources);
        Assert.Single(a.State.Observers[0].SourceSlots);

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
                _ = a.Value;
                _ = b.Value;
            }
        });

        Assert.Single(a.State.Observers);
        Assert.Single(a.State.ObserverSlots);
        Assert.Equal(2, a.State.Observers[0].Sources.Count);
        Assert.Equal(2, a.State.Observers[0].SourceSlots.Count);

        Assert.Single(b.State.Observers);
        Assert.Single(b.State.ObserverSlots);
        Assert.Equal(2, b.State.Observers[0].Sources.Count);
        Assert.Equal(2, b.State.Observers[0].SourceSlots.Count);

        effect.Dispose();
    }
}