using Lib;

var counter = Reactive.CreateRoot(dispose =>
{
    var count = Reactive.CreateSignal(10);
    Reactive.CreateEffect<object?>(_ =>
    {
        Console.WriteLine($"count: {count.Value}");
        return null;
    });
    Reactive.OnCleanup(() => { Console.WriteLine("Counter已销毁"); });
    return new
    {
        count,
        dispose
    };
});

counter.count.SetValue(prev => prev + 1);
counter.count.SetValue(prev => prev + 1);
counter.dispose();