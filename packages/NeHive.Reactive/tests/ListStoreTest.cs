using NeHive.Model;
namespace NeHive.Reactive.Tests;

public class ListStoreTest
{
    static List<int?> TakeSnapshot(ListStore<int?> store)
    {
        var snapshot = new List<int?>();

        for (int i = 0; i < store.Count; i++)
        {
            store.TryGetValue(i, out var v);
            snapshot.Add(v);
        }

        return snapshot;
    }

    [Fact]
    public void ListStore_ValueType_Test()
    {
        ListStore<int?> store = [10, 20, null];
        List<List<int?>> snapshots = [];
        List<int> doubleCounts = [];

        var scope = new Scope();
        // scope.RunWithOwner(() =>
        // {
        //     _ = new Effect(() => { snapshots.Add(TakeSnapshot(store)); });
        //     var doubleCount = new Computed<int>(() => store.Count * 2);
        //     _ = new Effect(() => { doubleCounts.Add(doubleCount.RxValue); });
        // });
        scope.CreateEffect(() => { snapshots.Add(TakeSnapshot(store)); });
        var doubleCount = scope.CreateComputed(() => store.Count * 2);
        scope.CreateEffect(() => { doubleCounts.Add(doubleCount.RxValue); });

        // 初始
        Assert.Equal([10, 20, null], snapshots[^1]);
        Assert.Equal(6, doubleCounts[^1]);

        // 单个修改
        store[1] = 99;
        Assert.Equal([10, 99, null], snapshots[^1]);
        Assert.Equal(6, doubleCounts[^1]);

        // 批量修改
        store.BatchModify(list =>
        {
            list[0] = 42;
            list.Add(77);
            list.Add(666);
            list.Add(null);
        });

        Assert.Equal(
            [42, 99, null, 77, 666, null],
            snapshots[^1]
        );
        Assert.Equal(12, doubleCounts[^1]);

        // RemoveAt
        store.RemoveAt(2);
        Assert.Equal(
            [42, 99, 77, 666, null],
            snapshots[^1]
        );
        Assert.Equal(10, doubleCounts[^1]);

        // Reverse
        store.Reverse();
        Assert.Equal(
            [null, 666, 77, 99, 42],
            snapshots[^1]
        );
        Assert.Equal(10, doubleCounts[^1]);

        // Sort
        store.Sort();
        Assert.Equal(
            [null, 42, 77, 99, 666],
            snapshots[^1]
        );
        Assert.Equal(10, doubleCounts[^1]);

        // Clear
        store.Clear();
        Assert.Empty(snapshots[^1]);
        Assert.Equal(0, doubleCounts[^1]);

        scope.Dispose();
    }

    [Fact]
    public void ListStore_ReferenceType_Test()
    {
        ListStore<string?> store = ["D", "B", "C", null, "A"];

        var slot04Snapshots = new List<(string?, string?)>();
        var slot5Snapshots = new List<string?>();
        var countSnapshots = new List<int>();
        var linqSnapshots = new List<List<string>>();
        var linqRuns = 0;

        var scope = new Scope();
        scope.RunInScope(() =>
        {
            // slot[0] & slot[4]
            _ = new Effect(() =>
            {
                store.TryGetValue(0, out var v0);
                store.TryGetValue(4, out var v4);
                slot04Snapshots.Add((v0, v4));
            });

            // slot[5]
            _ = new Effect(() =>
            {
                store.TryGetValue(5, out var v);
                slot5Snapshots.Add(v);
            });

            // Count
            _ = new Effect(() => { countSnapshots.Add(store.Count); });

            // LINQ 查询
            _ = new Effect(() =>
            {
                var result = store
                    .Where(x => x != null)
                    .Select((v, i) => $"{i}:{v}")
                    .ToList();

                linqSnapshots.Add(result);
                linqRuns++;
            });
        });

        // ✅ 初始状态
        Assert.Equal(("D", "A"), slot04Snapshots[^1]);
        Assert.Null(slot5Snapshots[^1]);
        Assert.Equal(5, countSnapshots[^1]);
        Assert.Equal(["0:D", "1:B", "2:C", "3:A"], linqSnapshots[^1]);
        Assert.Equal(1, linqRuns);

        // === 排序 ===
        store.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        Assert.Equal((null, "D"), slot04Snapshots[^1]);
        Assert.Equal(["0:A", "1:B", "2:C", "3:D"], linqSnapshots[^1]);
        Assert.Equal(2, linqRuns);

        // === 反转 ===
        store.Reverse();

        Assert.Equal(("D", null), slot04Snapshots[^1]);
        Assert.Equal(["0:D", "1:C", "2:B", "3:A"], linqSnapshots[^1]);
        Assert.Equal(3, linqRuns);

        // === 批量修改 ===
        store.BatchModify(list =>
        {
            list[0] = "Z";
            list[4] = "M";
        });

        Assert.Equal(("Z", "M"), slot04Snapshots[^1]);
        Assert.Equal(["0:Z", "1:C", "2:B", "3:A", "4:M"], linqSnapshots[^1]);
        Assert.Equal(4, linqRuns);

        // === Add ===
        store.Add("Tom");

        Assert.Equal("Tom", slot5Snapshots[^1]);
        Assert.Equal(6, countSnapshots[^1]);
        Assert.Equal(
            ["0:Z", "1:C", "2:B", "3:A", "4:M", "5:Tom"],
            linqSnapshots[^1]
        );
        Assert.Equal(5, linqRuns);

        // 什么也不干
        store.BatchModify(_ => { });

        Assert.Equal("Tom", slot5Snapshots[^1]);
        Assert.Equal(6, countSnapshots[^1]);
        Assert.Equal(
            ["0:Z", "1:C", "2:B", "3:A", "4:M", "5:Tom"],
            linqSnapshots[^1]
        );
        Assert.Equal(5, linqRuns);

        // === Clear ===
        store.Clear();

        Assert.Equal((null, null), slot04Snapshots[^1]);
        Assert.Null(slot5Snapshots[^1]);
        Assert.Equal(0, countSnapshots[^1]);
        Assert.Empty(linqSnapshots[^1]);
        Assert.Equal(6, linqRuns);

        scope.Dispose();
    }

    [Fact]
    public void Something_Batch_ShouldNotTrigger()
    {
        var store = new ListStore<int> { 1, 2, 3 };

        int runs = 0;
        var effect = new Effect(() =>
        {
            _ = store.GetEnumerator();
            runs++;
        });

        Assert.Equal(1, runs);

        // Do Nothing
        store.BatchModify(_ => { });
        Assert.Equal(1, runs);

        // same value not trigger
        store[0] = 1;
        Assert.Equal(1, runs);
        store[1] = 2;
        Assert.Equal(1, runs);
        store[2] = 3;
        Assert.Equal(1, runs);
        store[1] = 2;
        Assert.Equal(1, runs);

        Assert.Equal(1, runs);

        // none item for clear/sort/reverse should not trigger
        store.Clear();
        Assert.Equal(2, runs);
        store.Clear();
        Assert.Equal(2, runs);
        store.Sort();
        Assert.Equal(2, runs);
        store.Reverse();
        Assert.Equal(2, runs);

        store.Add(100);
        Assert.Equal(3, runs);

        // single item for sort/reverse should not trigger
        store.Sort();
        Assert.Equal(3, runs);
        store.Reverse();
        Assert.Equal(3, runs);

        store.BatchModify(items =>
        {
            items.Add(200);
            items.Add(300);
        });
        Assert.Equal(4, runs);

        // count value is 0 or 1 for sort/reverse should not trigger
        store.Sort(1, 0);
        Assert.Equal(4, runs);
        store.Sort(1, 1);
        Assert.Equal(4, runs);
        store.Reverse(1, 0);
        Assert.Equal(4, runs);
        store.Reverse(1, 1);
        Assert.Equal(4, runs);

        effect.Dispose();
    }

    [Fact]
    public void ReverseTest()
    {
        ListStore<int> store = [1, 2, 3];
        var runs = 0;
        var effect = new Effect(() =>
        {
            _ = store;
            runs++;
        });

        Assert.Equal(1, runs);
        store.Reverse();
        Assert.Equal(1, runs);
        effect.Dispose();
    }
}
