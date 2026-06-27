using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.Sample.Avalonia;

public static class ContextKey
{
    public static readonly ContextKey<MutSignal<int>> LockWindowCount = new();
}