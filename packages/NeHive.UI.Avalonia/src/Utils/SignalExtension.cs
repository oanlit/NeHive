using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Utils;

public static class SignalExtension
{
    public static MutSignal<T?> AsNullable<T>(this MutSignal<T> signal)
    {
        var result = new MutSignal<T?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<bool?> AsNullable(this MutSignal<bool> signal)
    {
        var result = new MutSignal<bool?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        scope.CreateEffect(epoch => signal.RxValue = epoch.Pull(result) is true);
        return result;
    }

    public static MutSignal<char?> AsNullable(this MutSignal<char> signal)
    {
        var result = new MutSignal<char?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<byte?> AsNullable(this MutSignal<byte> signal)
    {
        var result = new MutSignal<byte?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<short?> AsNullable(this MutSignal<short> signal)
    {
        var result = new MutSignal<short?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<ushort?> AsNullable(this MutSignal<ushort> signal)
    {
        var result = new MutSignal<ushort?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<int?> AsNullable(this MutSignal<int> signal)
    {
        var result = new MutSignal<int?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<uint?> AsNullable(this MutSignal<uint> signal)
    {
        var result = new MutSignal<uint?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<long?> AsNullable(this MutSignal<long> signal)
    {
        var result = new MutSignal<long?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<ulong?> AsNullable(this MutSignal<ulong> signal)
    {
        var result = new MutSignal<ulong?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<float?> AsNullable(this MutSignal<float> signal)
    {
        var result = new MutSignal<float?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }

    public static MutSignal<double?> AsNullable(this MutSignal<double> signal)
    {
        var result = new MutSignal<double?>(signal.Value);
        var scope = NeHiveContext.CurrentScope;
        scope.CreateEffect(epoch => result.RxValue = epoch.Pull(signal));
        return result;
    }
}