using NeHive.Core;

namespace NeHive.Generator.Tests;

[Store]
public class A
{
    public int Value { get; set; } = 0;
}
[Store]
public class B(AStore a)
{
    public AStore A { get; } = a;

    public int Double => A.Value * 2;
}

[Store]
public class C(BStore b)
{
    public BStore B { get; } = b;

    public int Value => B.Double + 1;
}

public class CombinationStoreTest
{
    [Fact]
    public void StoreGraph_Effect_ShouldReactAcrossStores()
    {
        var a = new AStore();
        var b = new BStore(a);

        var run = 0;

        using var effect = new Effect(() =>
        {
            run++;
            _ = b.Double;
        });

        Assert.Equal(1, run);

        a.Value = 10;
        Assert.Equal(2, run);

        a.Value = 20;
        Assert.Equal(3, run);
    }
    
    [Fact]
    public void StoreGraph_Chain_ShouldPropagate()
    {
        var a = new AStore();
        var b = new BStore(a);
        var c = new CStore(b);

        var run = 0;

        using var effect = new Effect(() =>
        {
            run++;
            _ = c.Value;
        });

        Assert.Equal(1, run);

        a.Value = 10;

        Assert.Equal(2, run);
    }
    
    [Fact]
    public void StoreGraph_Memo_ShouldTrackCrossStore()
    {
        var a = new AStore();
        var b = new BStore(a);

        var memo = new Memo<int>(() => a.Value + b.Double);

        var v1 = memo.Value;

        a.Value = 10;

        var v2 = memo.Value;

        Assert.NotEqual(v1, v2);
    }
    
    [Fact]
    public void StoreGraph_Batch_ShouldMergeUpdates()
    {
        var a = new AStore();
        var b = new BStore(a);

        var run = 0;

        using var effect = new Effect(() =>
        {
            run++;
            _ = b.Double;
        });

        Reactive.Batch(() =>
        {
            a.Value = 1;
            a.Value = 2;
            a.Value = 3;
        });

        Assert.Equal(2, run); // 只 rerun 一次
    }
    
    [Fact]
    public void StoreGraph_FanOut_ShouldTrackMultipleDeps()
    {
        var a = new AStore();
        var b1 = new BStore(a);
        var b2 = new BStore(a);

        var run = 0;

        using var effect = new Effect(() =>
        {
            run++;
            _ = b1.Double + b2.Double;
        });

        Assert.Equal(1, run);

        a.Value = 5;

        Assert.Equal(2, run);
    }
}