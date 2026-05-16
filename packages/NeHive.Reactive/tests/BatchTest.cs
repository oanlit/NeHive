namespace NeHive.Reactive.Tests;

public class BatchTest
{
    [Fact]
    public void Nested_Batch_Should_Behave_As_One_Batch()
    {
        var a = new MutSignal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.RxValue;
        });

        Rx.Batch(() =>
        {
            a.RxValue = 1;

            Rx.Batch(() =>
            {
                a.RxValue = 2;
                a.RxValue = 3;
            });

            a.RxValue = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Dynamic_Nested_Batch_Should_Work()
    {
        var a = new MutSignal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.RxValue == 1)
            {
                Rx.Batch(() =>
                {
                    a.RxValue = 2;
                    a.RxValue = 3;
                });
            }
        });
        Assert.Equal(1, runs);

        Rx.Batch(() => { a.RxValue = 1; });

        // Assert.Equal(2, runs);
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Effect_Chain_Should_Batch()
    {
        var a = new MutSignal<int>(0);
        var b = new MutSignal<int>(0);

        int runsA = 0;
        int runsB = 0;

        using var effectA = new Effect(() =>
        {
            runsA++;
            if (a.RxValue > 0)
                b.RxValue = a.RxValue;
        });

        using var effectB = new Effect(() =>
        {
            runsB++;
            _ = b.RxValue;
        });

        Rx.Batch(() =>
        {
            a.RxValue = 1;
            a.RxValue = 2;
            a.RxValue = 3;
        });

        Assert.Equal(2, runsA);
        Assert.Equal(2, runsB);
    }

    [Fact]
    public void Memo_And_Effect_Should_Batch_Together()
    {
        var a = new MutSignal<int>(1);
        using var owner = new Scope();

        var m = owner.CreateComputed(() => a.RxValue + 1);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = m.RxValue;
        });

        Rx.Batch(() =>
        {
            a.RxValue = 2;
            a.RxValue = 3;
            a.RxValue = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Read_Inside_Batch_Should_Not_Trigger_Extra_Run()
    {
        var a = new MutSignal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.RxValue;
        });

        Rx.Batch(() =>
        {
            a.RxValue = 1;
            _ = a.RxValue; // read
            a.RxValue = 2;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Large_Batch_Should_Run_Once()
    {
        var a = new MutSignal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.RxValue;
        });

        Rx.Batch(() =>
        {
            for (int i = 0; i < 10000; i++)
                a.RxValue = i;
        });

        Assert.Equal(2, runs);
    }
}
