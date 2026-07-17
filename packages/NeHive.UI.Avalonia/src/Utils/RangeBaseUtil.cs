using Avalonia.Controls.Primitives;

using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Utils;

public static class RangeBaseUtil
{
    public static void BindAccessor(UiScope uiScope,
        RangeBase range, 
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null)
    {
        if (bindValue is not null)
        {
            uiScope.CreateEffect(epoch => range.Value = epoch.Pull(bindValue));
            range.PropertyChanged += (_, e) =>
            {
                if (e.Property != RangeBase.ValueProperty) return;
                var newVal = range.Value;
                if (Math.Abs(newVal - bindValue.RxValue) > 0.0001)
                    bindValue.RxValue = newVal;
            };
        }
        else if (value is not null)
        {
            uiScope.CreateEffect(epoch => range.Value = epoch.Track(value));
        }

        if(onValueChanged is not null)
            range.ValueChanged += (_, e) => onValueChanged(e);

        if (minimum is not null)
        {
            range.Minimum = minimum.Value;
            if (minimum.IsReactive)
                uiScope.CreateEffect(epoch => range.Minimum = epoch.Track(minimum));
        }
        
        if (maximum is not null)
        {
            range.Maximum = maximum.Value;
            if (maximum.IsReactive)
                uiScope.CreateEffect(epoch => range.Maximum = epoch.Track(maximum));
        }
        
        if (smallChange is not null)
        {
            range.SmallChange = smallChange.Value;
            if (smallChange.IsReactive)
                uiScope.CreateEffect(epoch => range.SmallChange = epoch.Track(smallChange));
        }
        
        if (largeChange is not null)
        {
            range.LargeChange = largeChange.Value;
            if (largeChange.IsReactive)
                uiScope.CreateEffect(epoch => range.LargeChange = epoch.Track(largeChange));
        }
    }
}