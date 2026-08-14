using Avalonia.Controls.Primitives;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Components;
using NeHive.UI.Avalonia.Objects;

namespace NeHive.UI.Avalonia.Utils;

public static class RangeBaseUtil
{
    public static void BindAccessor(UiScope uiScope, RangeBase range, HRangeBaseArgs args)
    {
        if (args.BindValue is not null)
            BridgeAvalonia.BindPropertySignal(uiScope, args.BindValue, range, RangeBase.ValueProperty, true);
        else if (args.Value is not null)
        {
            range.Value = args.Value.Value;
            if (args.Value.IsReactive)
                uiScope.CreateEffect(epoch => range.Value = epoch.Track(args.Value));
        }

        if (args.OnValueChanged is not null)
        {
            uiScope.AddHandler<EventHandler<RangeBaseValueChangedEventArgs>>(
                add: h => range.ValueChanged += h,
                remove: h => range.ValueChanged -= h,
                handler: (_, e) => args.OnValueChanged(e)
            );
        }

        if (args.Minimum is not null)
        {
            range.Minimum = args.Minimum.Value;
            if (args.Minimum.IsReactive)
                uiScope.CreateEffect(epoch => range.Minimum = epoch.Track(args.Minimum));
        }

        if (args.Maximum is not null)
        {
            range.Maximum = args.Maximum.Value;
            if (args.Maximum.IsReactive)
                uiScope.CreateEffect(epoch => range.Maximum = epoch.Track(args.Maximum));
        }

        if (args.SmallChange is not null)
        {
            range.SmallChange = args.SmallChange.Value;
            if (args.SmallChange.IsReactive)
                uiScope.CreateEffect(epoch => range.SmallChange = epoch.Track(args.SmallChange));
        }

        if (args.LargeChange is not null)
        {
            range.LargeChange = args.LargeChange.Value;
            if (args.LargeChange.IsReactive)
                uiScope.CreateEffect(epoch => range.LargeChange = epoch.Track(args.LargeChange));
        }
    }
}