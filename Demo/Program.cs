using Demo.Store;
using Lib;

// 创建一个 NullableListStore
ListStore<string?> store = ["D", "B", "C", null, "A"];

var dispose = Reactive.CreateRoot(dispose =>
{
    // 订阅 slot[0] 和 slot[4]
    Reactive.CreateEffect(() =>
    {
        store.TryGetValue(0, out var value1);
        value1 ??= "null";
        store.TryGetValue(4, out var value2);
        value2 ??= "null";
        Console.WriteLine($"Effect slot[0]: {value1}");
        Console.WriteLine($"Effect slot[4]: {value2}");
    });

    Reactive.CreateEffect(() =>
    {
        store.TryGetValue(5, out var value);
        value ??= "null";
        Console.WriteLine($"Effect slot[5]: {value}");
    });

    // 创建一个 effect，订阅 count
    Reactive.CreateEffect(() => { Console.WriteLine($"Effect Count: {store.Count}"); });

    Reactive.CreateEffect(() =>
    {
        var query = store
            .Where(x => x != null)
            .Select((v, i) => $"{i}:{v}");

        Console.WriteLine("LINQ 查询结果:");
        foreach (var s in query)
            Console.WriteLine(s);
    });

    return dispose;
});

Console.WriteLine("=== 排序 ===");
store.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));
// slot[0] 和 slot[4] 会触发 effect，如果值变化

Console.WriteLine("=== 反转 ===");
store.Reverse();

Console.WriteLine("=== 再次批量修改 ===");
store.BatchModify(list =>
{
    list[0] = "Z"; // slot[0] 改变
    list[4] = "M"; // slot[4] 改变
});

store.Add("Lin");

store.Clear();

dispose();

// 创建一个 int 类型的 ListStore
// ListStore<int?> store = [10, 20, null];
//
// // 创建一个 Reactive 根上下文
// var dispose = Reactive.CreateRoot(dispose =>
// {
//     // 创建 Effect，观察整个列表变化
//     Reactive.CreateEffect(() =>
//     {
//         Console.WriteLine("Effect triggered! store contents:");
//         for (var i = 0; i < store.Count; i++)
//         {
//             store.TryGetValue(i, out var v);
//             Console.WriteLine($"  index {i}: {(v.HasValue ? v.Value.ToString() : "null")}");
//         }
//     });
//
//     Memo<int> doubleCount = new(() => store.Count * 2);
//     Reactive.CreateEffect(() => { Console.WriteLine($"Track DoubleCount: {doubleCount.Value}"); });
//     Reactive.CreateEffect(() => { Console.WriteLine($"Untrack DoubleCount: {doubleCount.UntrackValue}"); });
//
//     Reactive.CreateEffect(() =>
//     {
//         store.TryGetValue(4, out var v);
//         Console.WriteLine($"  store[4]: {(v.HasValue ? v.Value.ToString() : "null")}");
//     });
//
//     return dispose;
// });
//
// Console.WriteLine("store[1]单个修改");
// store[1] = 99;
//
// Console.WriteLine("批量修改");
// store.BatchModify(list =>
// {
//     list[0] = 42;
//     list.Add(77); // 增加新元素
//     list.Add(666);
//     list.Add(null); // 再增加一个 null
// });
//
// Console.WriteLine("执行RemoveAt(2)");
// store.RemoveAt(2); // 删除原来的 null
//
// Console.WriteLine("对整个 store 反转");
// store.Reverse();
//
// Console.WriteLine("排序");
// store.Sort();
//
// Console.WriteLine("清空");
// store.Clear();
//
// dispose();
