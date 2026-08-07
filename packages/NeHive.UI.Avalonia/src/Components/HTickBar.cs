using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HTickBarProps(Scope scope, Border border, TickBar tickBar) : BaseComponentProps(scope, border, tickBar)
{
    public MutSignal<double> Minimum
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(tickBar.Minimum);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.MinimumProperty);
            return field;
        }
    }

    public MutSignal<double> Maximum
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(tickBar.Maximum);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.MaximumProperty);
            return field;
        }
    }

    public MutSignal<bool> IsDirectionReversed
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(tickBar.IsDirectionReversed);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.IsDirectionReversedProperty);
            return field;
        }
    }

    public MutSignal<double> TickFrequency
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(tickBar.TickFrequency);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.TickFrequencyProperty);
            return field;
        }
    }

    public MutSignal<TickBarPlacement> Placement
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<TickBarPlacement>(tickBar.Placement);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.PlacementProperty);
            return field;
        }
    }

    public MutSignal<Rect> ReservedSpace
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<Rect>(tickBar.ReservedSpace);
            BridgeAvalonia.BindPropertySignal(scope, field, tickBar, TickBar.ReservedSpaceProperty);
            return field;
        }
    }
}

public class HTickBarArgs(
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<bool>? isDirectionReversed = null,
    Accessor<double>? tickFrequency = null,
    Accessor<TickBarPlacement>? placement = null,
    Accessor<IEnumerable<double>>? ticks = null,
    Accessor<Rect>? reservedSpace = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<double>? Minimum = minimum;
    public readonly Accessor<double>? Maximum = maximum;
    public readonly Accessor<bool>? IsDirectionReversed = isDirectionReversed;
    public readonly Accessor<double>? TickFrequency = tickFrequency;
    public readonly Accessor<TickBarPlacement>? Placement = placement;
    public readonly Accessor<IEnumerable<double>>? Ticks = ticks;
    public readonly Accessor<Rect>? ReservedSpace = reservedSpace;
}

public static partial class BaseComponent
{
    public static IElement<TickBar> HTickBar(
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<bool>? isDirectionReversed = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickBarPlacement>? placement = null,
        Accessor<IEnumerable<double>>? ticks = null,
        Accessor<Rect>? reservedSpace = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HTickBar(out _, _ => new(minimum, maximum, isDirectionReversed, tickFrequency,
        placement, ticks, reservedSpace, strStyle, style, baseInteraction));
    
    public static IElement<TickBar> HTickBar(
        out TickBar expose,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<bool>? isDirectionReversed = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickBarPlacement>? placement = null,
        Accessor<IEnumerable<double>>? ticks = null,
        Accessor<Rect>? reservedSpace = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HTickBar(out expose, _ => new(minimum, maximum, isDirectionReversed, tickFrequency,
        placement, ticks, reservedSpace, strStyle, style, baseInteraction));

    public static IElement<TickBar> HTickBar(Func<HTickBarProps, HTickBarArgs> fn) => HTickBar(out _, fn);

    public static IElement<TickBar> HTickBar(
        out TickBar expose,
        Func<HTickBarProps, HTickBarArgs> fn
    )
    {
        var tickBar = new TickBar();
        expose = tickBar;
        return Element<TickBar>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = tickBar
            };
            var props = new HTickBarProps(uiScope, border, tickBar);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, tickBar, border, ApplyStyle);
            state.ApplyVariantsStyle(tickBar, border, ApplyStyle);

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(tickBar, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, tickBar);

            if (args.Minimum is not null)
            {
                tickBar.Minimum = args.Minimum.Value;
                if (args.Minimum.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Minimum = epoch.Track(args.Minimum));
            }

            if (args.Maximum is not null)
            {
                tickBar.Maximum = args.Maximum.Value;
                if (args.Maximum.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Maximum = epoch.Track(args.Maximum));
            }

            if (args.IsDirectionReversed is not null)
            {
                tickBar.IsDirectionReversed = args.IsDirectionReversed.Value;
                if (args.IsDirectionReversed.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.IsDirectionReversed = epoch.Track(args.IsDirectionReversed));
            }

            if (args.TickFrequency is not null)
            {
                tickBar.TickFrequency = args.TickFrequency.Value;
                if (args.TickFrequency.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.TickFrequency = epoch.Track(args.TickFrequency));
            }

            if (args.Placement is not null)
            {
                tickBar.Placement = args.Placement.Value;
                if (args.Placement.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Placement = epoch.Track(args.Placement));
            }

            if (args.Ticks is not null)
            {
                tickBar.Ticks = new AvaloniaList<double>(args.Ticks.Value);
                if (args.Ticks.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Ticks = new AvaloniaList<double>(epoch.Track(args.Ticks)));
            }

            if (args.ReservedSpace is not null)
            {
                tickBar.ReservedSpace = args.ReservedSpace.Value;
                if (args.ReservedSpace.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.ReservedSpace = epoch.Track(args.ReservedSpace));
            }

            return (tickBar, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) tickBar.Orientation = styleValue.Orientation.Value;
                if (styleValue.Foreground is not null) tickBar.Fill = styleValue.Foreground;
            }
        });
    }
}