using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HTrackProp(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? viewportSize = null,
    Accessor<bool>? isDirectionReversed = null,
    Accessor<bool>? isDeferThumbDrag = null,
    Accessor<bool>? isIgnoreThumbDrag = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;
    public readonly Accessor<double> Minimum = minimum ?? 0.0;
    public readonly Accessor<double> Maximum = maximum ?? 100.0;
    public readonly Accessor<double>? ViewportSize = viewportSize;
    
    public readonly Accessor<bool>? IsDirectionReversed = isDirectionReversed;
    public readonly Accessor<bool>? IsDeferThumbDrag = isDeferThumbDrag;
    public readonly Accessor<bool>? IsIgnoreThumbDrag = isIgnoreThumbDrag;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public IElement<Button>? IncreaseButton;
    public IElement<Button>? DecreaseButton;
    public required IElement<Thumb> Thumb;
}

public static partial class BaseComponent
{
    public static IElement<Track> HTrack(HTrackProp prop)
    {
        return Element<Track>.WithScope(uiScope =>
        {
            var track = new Track();
            var border = new Border
            {
                Child = track
            };
            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants,
                Variants = prop.Variants
            };

            state.ApplyAccessorStyle(prop.Style, track, border, ApplyStyle);
            state.ApplyVariantsStyle(track, border, ApplyStyle);
            
            if (prop.BindValue is not null)
            {
                uiScope.CreateEffect(() => track.Value = prop.BindValue.RxValue);
                track.PropertyChanged += (_, e) =>
                {
                    if (e.Property != RangeBase.ValueProperty) return;
                    var newVal = track.Value;
                    if (Math.Abs(newVal - prop.BindValue.RxValue) > 0.0001)
                        prop.BindValue.RxValue = newVal;
                };
            }
            else if (prop.Value is not null)
            {
                uiScope.CreateEffect(() => track.Value = prop.Value.RxValue);
            }
            
            track.Minimum = prop.Minimum.Value;
            if (prop.Minimum.IsReactive)
                uiScope.CreateEffect(epochScope => track.Minimum = epochScope.Track(prop.Minimum));

            track.Maximum = prop.Maximum.Value;
            if (prop.Maximum.IsReactive)
                uiScope.CreateEffect(epochScope => track.Maximum = epochScope.Track(prop.Maximum));

            if (prop.ViewportSize is not null)
            {
                track.ViewportSize = prop.ViewportSize.Value;
                if (prop.ViewportSize.IsReactive)
                    uiScope.CreateEffect(epochScope => track.ViewportSize = epochScope.Track(prop.ViewportSize));
            }
            
            if (prop.IsDirectionReversed is not null)
            {
                track.IsDirectionReversed = prop.IsDirectionReversed.Value;
                if (prop.IsDirectionReversed.IsReactive)
                    uiScope.CreateEffect(epochScope => track.IsDirectionReversed = epochScope.Track(prop.IsDirectionReversed));
            }
            
            if (prop.IsDeferThumbDrag is not null)
            {
                track.DeferThumbDrag = prop.IsDeferThumbDrag.Value;
                if (prop.IsDeferThumbDrag.IsReactive)
                    uiScope.CreateEffect(epochScope => track.DeferThumbDrag = epochScope.Track(prop.IsDeferThumbDrag));
            }
            
            if (prop.IsIgnoreThumbDrag is not null)
            {
                track.IgnoreThumbDrag = prop.IsIgnoreThumbDrag.Value;
                if (prop.IsIgnoreThumbDrag.IsReactive)
                    uiScope.CreateEffect(epochScope => track.IgnoreThumbDrag = epochScope.Track(prop.IsIgnoreThumbDrag));
            }

            _ = prop.IncreaseButton?.Content;
            track.IncreaseButton = prop.IncreaseButton?.Expose;
            
            _ = prop.DecreaseButton?.Content;
            track.DecreaseButton = prop.DecreaseButton?.Expose;
            
            _ = prop.Thumb.Content;
            track.Thumb = prop.Thumb.Expose;
            
            return (track, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                if (styleValue.Orientation is not null) track.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}