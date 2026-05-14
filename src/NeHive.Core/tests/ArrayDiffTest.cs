namespace NeHive.Core.Tests;

public class ArrayDiffTest
{
    [Fact]
    public void ArrayDiff_NoChange()
    {
        var oldList = new[] { 1, 2, 3 };
        var newList = new[] { 1, 2, 3 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Empty(diff.RemoveItemsIndex);
        Assert.Empty(diff.OldIndex2News);
        Assert.Empty(diff.NewItemsIndex);
    }

    [Fact]
    public void ArrayDiff_AllNew()
    {
        var oldList = Array.Empty<int>();
        var newList = new[] { 1, 2, 3 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Empty(diff.RemoveItemsIndex);
        Assert.Empty(diff.OldIndex2News);
        Assert.Equal([0, 1, 2], diff.NewItemsIndex);
    }

    [Fact]
    public void ArrayDiff_AllRemoved()
    {
        var oldList = new[] { 1, 2, 3 };
        var newList = Array.Empty<int>();

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Equal([0, 1, 2], diff.RemoveItemsIndex);
        Assert.Empty(diff.OldIndex2News);
        Assert.Empty(diff.NewItemsIndex);
    }

    [Fact]
    public void ArrayDiff_InsertMiddle()
    {
        var oldList = new[] { 1, 3 };
        var newList = new[] { 1, 2, 3 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Empty(diff.RemoveItemsIndex);
        Assert.Contains(1, diff.NewItemsIndex);
    }

    [Fact]
    public void ArrayDiff_RemoveMiddle()
    {
        var oldList = new[] { 1, 2, 3 };
        var newList = new[] { 1, 3 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Contains(1, diff.RemoveItemsIndex);
    }

    [Fact]
    public void ArrayDiff_Move()
    {
        var oldList = new[] { 1, 2, 3 };
        var newList = new[] { 3, 1, 2 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.Contains(diff.OldIndex2News, x => x == (0, 1));
        Assert.Contains(diff.OldIndex2News, x => x == (1, 2));
        Assert.Contains(diff.OldIndex2News, x => x == (2, 0));
    }

    [Fact]
    public void ArrayDiff_DuplicateItems()
    {
        var oldList = new[] { 1, 2, 1 };
        var newList = new[] { 1, 1, 2 };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList);

        Assert.NotEmpty(diff.OldIndex2News);
    }

    class Item
    {
        public int Id;
        public string Name = "";
    }

    [Fact]
    public void ArrayDiff_KeySelector()
    {
        var oldList = new[]
        {
            new Item { Id = 1 },
            new Item { Id = 2 }
        };

        var newList = new[]
        {
            new Item { Id = 2 },
            new Item { Id = 1 }
        };

        var diff = ArrayDiffUtil.ArrayDiff(newList, oldList, x => x.Id);

        Assert.Equal(2, diff.OldIndex2News.Count);
    }

    [Fact]
    public void MapList_Initial_ShouldMapCorrectly()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 2, 3],
            x => $"v{x}"
        );

        Assert.Equal(["v1", "v2", "v3"], map.MapList);
    }

    [Fact]
    public void MapList_NoChange_ShouldKeepSameValues()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 2, 3],
            x => $"v{x}"
        );

        var old = map.MapList.ToArray();

        map.SourceList = [1, 2, 3];

        Assert.Equal(old, map.MapList);
    }

    [Fact]
    public void MapList_Insert_ShouldAddNewMappedItem()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 3],
            x => $"v{x}"
        );

        map.SourceList = [1, 2, 3];

        Assert.Equal(
            ["v1", "v2", "v3"],
            map.MapList
        );
    }

    [Fact]
    public void MapList_Remove_ShouldRemoveMappedItem()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 2, 3],
            x => $"v{x}"
        );

        map.SourceList = [1, 3];

        Assert.Equal(
            ["v1", "v3"],
            map.MapList
        );
    }

    [Fact]
    public void MapList_Move_ShouldReorderWithoutRemap()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 2, 3],
            x => $"v{x}"
        );

        map.SourceList = [3, 1, 2];

        Assert.Equal(
            ["v3", "v1", "v2"],
            map.MapList
        );
    }

    [Fact]
    public void MapList_ComplexChange()
    {
        var map = new ArrayMapResult<int, string>(
            [1, 2, 3, 4],
            x => $"v{x}"
        );

        map.SourceList = [3, 5, 1];

        Assert.Equal(
            ["v3", "v5", "v1"],
            map.MapList
        );
    }

    [Fact]
    public void MapList_WithKey_ShouldTrackByKey()
    {
        var map = new ArrayMapResult<Item, string>(
            [
                new Item { Id = 1, Name = "A" },
                new Item { Id = 2, Name = "B" }
            ],
            x => x.Name,
            keyFn: x => x.Id
        );

        map.SourceList =
        [
            new Item { Id = 2, Name = "B2" },
            new Item { Id = 1, Name = "A2" }
        ];

        // 注意：这里值不会更新（因为复用了旧映射）
        Assert.Equal(["B", "A"], map.MapList);
    }

    [Fact]
    public void MapList_Move_ShouldKeepReference()
    {
        var map = new ArrayMapResult<int, object>(
            [1, 2, 3],
            _ => new object()
        );

        var before = map.MapList.ToArray();
        map.SourceList = [3, 1, 2];
        var after = map.MapList;

        Assert.Same(before[0], after[1]);
        Assert.Same(before[1], after[2]);
        Assert.Same(before[2], after[0]);
    }

    [Fact]
    public void ArrayMapMemo_Initial()
    {
        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            x => x * 2
        );
        Assert.Equal([2, 4, 6], memo.RxValue);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_Update()
    {
        var signal = new MutSignal<IReadOnlyList<int>>([1, 2]);

        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            x => x * 2
        );

        signal.RxValue = [3, 4];
        Assert.Equal([6, 8], memo.RxValue);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_NoChange_ShouldReuse()
    {
        var created = 0;

        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var memo = new ArrayMapMemo<int, object, int>(
            signal,
            _ =>
            {
                created++;
                return new object();
            }
        );
        created = 0;

        signal.RxValue = [1, 2, 3];
        Assert.Equal(0, created);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_Move_ShouldReuseReference()
    {
        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var memo = new ArrayMapMemo<int, object, int>(
            signal,
            _ => new object()
        );

        var before = memo.RxValue.ToArray();

        signal.RxValue = [3, 1, 2];

        var after = memo.RxValue;

        Assert.Same(before[0], after[1]);
        Assert.Same(before[1], after[2]);
        Assert.Same(before[2], after[0]);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_Insert_ShouldOnlyCreateNew()
    {
        var created = 0;
        var signal = new MutSignal<IReadOnlyList<int>>([1, 2]);

        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            x =>
            {
                created++;
                return x;
            }
        );

        created = 0;
        signal.RxValue = [1, 2, 3];

        Assert.Equal(1, created);
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_Remove_ShouldDispose()
    {
        var disposed = 0;

        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            x =>
            {
                Reactive.OnDispose(() => disposed++);
                return x;
            }
        );
        disposed = 0;

        signal.RxValue = [2];

        Assert.Equal(2, disposed);

        signal.RxValue = [6];
        Assert.Equal(3, disposed);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_ClearAll()
    {
        var disposed = 0;

        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            x =>
            {
                Reactive.OnDispose(() => disposed++);
                return x;
            }
        );

        signal.RxValue = [];

        Assert.Equal(3, disposed);
        Assert.Empty(memo.RxValue);
        
        memo.Dispose();
    }

    [Fact]
    public void ArrayMapMemo_Key_ShouldPreventRecreate()
    {
        var created = 0;

        var signal = new MutSignal<IReadOnlyList<Item>>([
            new Item { Id = 1, Name = "A" },
            new Item { Id = 2, Name = "B" }
        ]);

        var memo = new ArrayMapMemo<Item, string, int>(
            signal,
            x =>
            {
                created++;
                return x.Name;
            },
            keyFn: x => x.Id
        );

        created = 0;

        signal.RxValue =
        [
            new Item { Id = 2, Name = "B2" },
            new Item { Id = 1, Name = "A2" }
        ];

        var result = memo.RxValue;

        Assert.Equal(0, created); // 没有重新 map
        Assert.Equal(["B", "A"], result); // 仍是旧值
    }

    [Fact]
    public void ArrayMapMemo_IndexSignal_ShouldUpdate()
    {
        var signal = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        ISignal<int> iSignal = new MutSignal<int>(-1);
        var memo = new ArrayMapMemo<int, int, int>(
            signal,
            (item, indexSignal) =>
            {
                iSignal = indexSignal;
                return item;
            }
        );

        Assert.Equal(2, iSignal.RxValue);

        var effectRuns = 0;
        using var effect = new Effect(() =>
        {
            effectRuns++;
            _ = iSignal.RxValue;
        });
        effectRuns = 0;
        signal.RxValue = [3, 1, 2];
        Assert.Equal(0, iSignal.RxValue);
        Assert.Equal(1, effectRuns);

        memo.Dispose();
    }
}