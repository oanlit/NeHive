using Lib;

// var counter = Reactive.CreateRoot(dispose =>
// {
//     var count = Reactive.CreateSignal(10);
//     var doubleCount = Reactive.CreateMemo<int>(_ => count.Value * 2);
//     Reactive.CreateEffect(() => Console.WriteLine($"count: {count.Value}"));
//     Reactive.CreateEffect(() => Console.WriteLine($"doubleCount: {doubleCount.Value}"));
//     Reactive.OnCleanup(() => Console.WriteLine("Counter已销毁"));
//     return new
//     {
//         count,
//         doubleCount,
//         dispose
//     };
// });
//
// counter.count.SetValue(prev => prev + 1);
// counter.count.SetValue(prev => prev + 1);
// counter.count.Value = 30;
//
// counter.dispose();

// using Lib;

// var map = new ArrayMapResult<int, int>(
//     [1, 2, 3],
//     (item, _) => item * 2
// );
// foreach (var v in map.MapList)
// {
//     Console.WriteLine(v);
// }
//
// Console.WriteLine("--------------");
// map.SourceList = [3, 2];
//
// foreach (var v in map.MapList)
// {
//     Console.WriteLine(v);
// }
//
// Console.WriteLine("--------------");
// map.SourceList = [3, 2, 8, 1];
// foreach (var v in map.MapList)
// {
//     Console.WriteLine(v);
// }

var test = Reactive.CreateRoot(dispose =>
{
    Signal<List<int>> source = new([1, 2, 3]);
    Package effectRunCount = new();
    effectRunCount.Value = 0;

    var arrayMapMemo = new ArrayMapMemo<int, int, int>(
        source,
        (item, index) =>
        {
            Reactive.CreateEffect(() =>
            {
                _ = index.Value;
                effectRunCount.Value += 1;
            });
            return item * 2;
        }
    );
    Reactive.CreateEffect(() =>
    {
        Console.WriteLine("arrayMapMemo更新");
        var values = arrayMapMemo.Value;
        foreach (var value in values)
        {
            Console.WriteLine(value);
        }

        Console.WriteLine("--------------");
    });
    return new
    {
        source,
        dispose,
        effectRunCount
    };
});

Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.source.Value = [1, 2, 3, 4];
Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.source.Value = [5, 9, 3, 2, 8];
Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.source.Value = [1, 1, 4];
Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.source.Value = [];
Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.source.Value = [1, 2, 3];
Console.WriteLine($"effectRunCount的值为:{test.effectRunCount.Value}");
test.dispose();

class Package
{
    public int Value;
}

// var a= Reactive.CreateRoot(dispose =>
// {
//     var source = new Signal<List<(int id, string name)>>(
//         [(1, "A"), (2, "B"), (3, "C")]
//     );
//     Console.WriteLine("source创建成功");
//     var memo = new ArrayMapMemo<(int id, string name), object, int>(
//         source,
//         (item) => new {},
//         item => item.id
//     );
//     Console.WriteLine("memo创建成功");
//
//     var first = memo.Value;
//     Console.WriteLine("first获取成功");
//     source.Value =
//     [
//         (3, "C2"),
//         (1, "A2"),
//         (2, "B2")
//     ];
//     Console.WriteLine("source更新成功");
//     var second = memo.Value;
//     if (ReferenceEquals(first[2], second[0])
//         && ReferenceEquals(first[0], second[1])
//         && ReferenceEquals(first[1], second[2]))
//     {
//         Console.WriteLine("复用正常");
//     }
//     else
//     {
//         Console.WriteLine("复用异常");
//     }
//
//     return new
//     {
//         dispose
//     };
// });
//
// a.dispose();