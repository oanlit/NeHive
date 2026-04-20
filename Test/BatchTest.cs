namespace Test;

using Lib;

public class BatchTest
{
    [Fact]
    public void Nested_Batch_Should_Behave_As_One_Batch()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;

            Reactive.Batch(() =>
            {
                a.Value = 2;
                a.Value = 3;
            });

            a.Value = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Dynamic_Nested_Batch_Should_Work()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            if (a.Value == 1)
            {
                Reactive.Batch(() =>
                {
                    a.Value = 2;
                    a.Value = 3;
                });
            }
        });
        Assert.Equal(1, runs);

        Reactive.Batch(() => { a.Value = 1; });

        // Assert.Equal(2, runs);
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Effect_Chain_Should_Batch()
    {
        var a = new Signal<int>(0);
        var b = new Signal<int>(0);

        int runsA = 0;
        int runsB = 0;

        using var effectA = new Effect(() =>
        {
            runsA++;
            if (a.Value > 0)
                b.Value = a.Value;
        });

        using var effectB = new Effect(() =>
        {
            runsB++;
            _ = b.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;
            a.Value = 2;
            a.Value = 3;
        });

        Assert.Equal(2, runsA);
        Assert.Equal(2, runsB);
    }

    [Fact]
    public void Memo_And_Effect_Should_Batch_Together()
    {
        var a = new Signal<int>(1);
        using var owner = new Scope();

        var m = owner.AddMemo(() => a.Value + 1);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = m.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 2;
            a.Value = 3;
            a.Value = 4;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Read_Inside_Batch_Should_Not_Trigger_Extra_Run()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.Value;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;
            _ = a.Value; // read
            a.Value = 2;
        });

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Large_Batch_Should_Run_Once()
    {
        var a = new Signal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.Value;
        });

        Reactive.Batch(() =>
        {
            for (int i = 0; i < 10000; i++)
                a.Value = i;
        });

        Assert.Equal(2, runs);
    }
}
