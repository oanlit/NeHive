using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HRangeBaseProps(Scope scope, Border border, RangeBase rangeBase)
    : BaseComponentProps(scope, border, rangeBase)
{
    public MutSignal<double> Value
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(rangeBase.Value);
            BridgeAvalonia.BindPropertySignal(scope, field, rangeBase, RangeBase.ValueProperty);
            return field;
        }
    }

    public Signal<double> Minimum
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, rangeBase,
                RangeBase.MinimumProperty, rangeBase.Minimum);
            return field;
            // field = new MutSignal<double>(scrollBar.Minimum);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, RangeBase.MinimumProperty);
            // return field;
        }
    }

    public Signal<double> Maximum
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, rangeBase,
                RangeBase.MaximumProperty, rangeBase.Maximum);
            return field;
            // field = new MutSignal<double>(scrollBar.Maximum);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, RangeBase.MaximumProperty);
            // return field;
        }
    }

    public Signal<double> SmallChange
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, rangeBase,
                RangeBase.SmallChangeProperty, rangeBase.SmallChange);
            return field;
            // field = new MutSignal<double>(scrollBar.SmallChange);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, RangeBase.SmallChangeProperty);
            // return field;
        }
    }

    public Signal<double> LargeChange
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, rangeBase,
                RangeBase.LargeChangeProperty, rangeBase.LargeChange);
            return field;
            // field = new MutSignal<double>(scrollBar.SmallChange);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, RangeBase.SmallChangeProperty);
            // return field;
        }
    }
}

public class HRangeBaseArgs(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;
    public readonly Accessor<double>? Minimum = minimum;
    public readonly Accessor<double>? Maximum = maximum;
    public readonly Accessor<double>? SmallChange = smallChange;
    public readonly Accessor<double>? LargeChange = largeChange;
    public readonly Action<RangeBaseValueChangedEventArgs>? OnValueChanged = onValueChanged;
}
