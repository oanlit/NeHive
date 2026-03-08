using Lib;

var counter = Reactive.CreateRoot(dispose =>
{
    var count = Reactive.CreateSignal(10);
    var doubleCount = Reactive.CreateMemo<int>(_ => count.Value * 2);
    Reactive.CreateEffect(() => Console.WriteLine($"count: {count.Value}"));
    Reactive.CreateEffect(() => Console.WriteLine($"doubleCount: {doubleCount.Value}"));
    Reactive.OnCleanup(() => Console.WriteLine("Counter已销毁"));
    return new
    {
        count,
        doubleCount,
        dispose
    };
});

counter.count.SetValue(prev => prev + 1);
counter.count.SetValue(prev => prev + 1);
counter.count.Value = 30;

counter.dispose();