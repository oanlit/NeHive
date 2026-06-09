using NeHive.Model;
using NeHive.Reactive;

using var scope = new Scope();

var signal = new MutSignal<int>(0);

var effect = scope.CreateReactiveFlow(signal)
    // .ThrottleLatest(TimeSpan.FromMilliseconds(500))
    .ThrottleLatest(500)
    .Map(v => v * 2)
    .PushEffect(v =>
    {
        Console.WriteLine(
            $"{DateTime.Now:HH:mm:ss.fff} -> {v}");
    });

for (var i = 1; i <= 10; i++)
{
    signal.RxValue = i;

    await Task.Delay(100);
}

await Task.Delay(2000);

effect.Dispose();
