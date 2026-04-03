namespace Test;

using Lib;

public class SignalTest
{
    [Fact]
    public void Repeat_Read_Signal_Test()
    {
        Signal<int> a = new(0);
        var dispose = Reactive.CreateRoot(dispose =>
        {
            Reactive.CreateEffect(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    _ = a.Value;
                }
            });
            return dispose;
        });

        Assert.Equal(1, a.State.Observers!.Count);
        Assert.Equal(1, a.State.ObserverSlots!.Count);
        Assert.Equal(1, a.State.Observers[0].Sources!.Count);
        Assert.Equal(1, a.State.Observers[0].SourceSlots!.Count);

        dispose();
    }
    
    [Fact]
    public void Repeat_Alternation_Read_Signal_Test()
    {
        Signal<int> a = new(0);
        Signal<int> b = new(0);
        var dispose = Reactive.CreateRoot(dispose =>
        {
            Reactive.CreateEffect(() =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    _ = a.Value;
                    _ = b.Value;
                }
            });
            return dispose;
        });

        Assert.Equal(1, a.State.Observers!.Count);
        Assert.Equal(1, a.State.ObserverSlots!.Count);
        Assert.Equal(2, a.State.Observers[0].Sources!.Count);
        Assert.Equal(2, a.State.Observers[0].SourceSlots!.Count);
        
        Assert.Equal(1, b.State.Observers!.Count);
        Assert.Equal(1, b.State.ObserverSlots!.Count);
        Assert.Equal(2, b.State.Observers[0].Sources!.Count);
        Assert.Equal(2, b.State.Observers[0].SourceSlots!.Count);

        dispose();
    }
}