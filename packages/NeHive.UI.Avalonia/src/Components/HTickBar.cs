using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

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
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        return Element<TickBar>.WithScope(uiScope =>
        {
            var tickBar = new TickBar();
            var border = new Border
            {
                Child = tickBar
            };

            var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                StrVariants = styleAccessor.Value.Variants,
                Variants = variants
            };
            state.ApplyAccessorStyle(styleAccessor, tickBar, border, ApplyStyle);
            state.ApplyVariantsStyle(tickBar, border, ApplyStyle);

            if (minimum is not null)
            {
                tickBar.Minimum = minimum.Value;
                if (minimum.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Minimum = epoch.Track(minimum));
            }

            if (maximum is not null)
            {
                tickBar.Maximum = maximum.Value;
                if (maximum.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Maximum = epoch.Track(maximum));
            }

            if (isDirectionReversed is not null)
            {
                tickBar.IsDirectionReversed = isDirectionReversed.Value;
                if (isDirectionReversed.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.IsDirectionReversed = epoch.Track(isDirectionReversed));
            }

            if (tickFrequency is not null)
            {
                tickBar.TickFrequency = tickFrequency.Value;
                if (tickFrequency.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.TickFrequency = epoch.Track(tickFrequency));
            }

            if (placement is not null)
            {
                tickBar.Placement = placement.Value;
                if (placement.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Placement = epoch.Track(placement));
            }

            if (ticks is not null)
            {
                tickBar.Ticks = new AvaloniaList<double>(ticks.Value);
                if (ticks.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.Ticks = new AvaloniaList<double>(epoch.Track(ticks)));
            }

            if (reservedSpace is not null)
            {
                tickBar.ReservedSpace = reservedSpace.Value;
                if (reservedSpace.IsReactive)
                    uiScope.CreateEffect(epoch => tickBar.ReservedSpace = epoch.Track(reservedSpace));
            }

            return (tickBar, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) tickBar.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}