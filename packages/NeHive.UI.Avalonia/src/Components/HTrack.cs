using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HTrackArgs(
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
    public readonly Accessor<double>? Minimum = minimum;
    public readonly Accessor<double>? Maximum = maximum;
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
    public static IElement<Track> HTrack(HTrackArgs args)
    {
        return Element<Track>.WithScope(uiScope =>
        {
            var track = new Track();
            var border = new Border
            {
                Child = track
            };
            var state = new CommonState(uiScope, args.Style.Value.Normal)
            {
                StrVariants = args.Style.Value.Variants
            };

            state.ApplyAccessorStyle(args.Style, track, border, ApplyStyle);
            state.ApplyVariantsStyle(track, border, ApplyStyle);
            
            if (args.BindValue is not null)
            {
                uiScope.CreateEffect(() => track.Value = args.BindValue.RxValue);
                track.PropertyChanged += (_, e) =>
                {
                    if (e.Property != RangeBase.ValueProperty) return;
                    var newVal = track.Value;
                    if (Math.Abs(newVal - args.BindValue.RxValue) > 0.0001)
                        args.BindValue.RxValue = newVal;
                };
            }
            else if (args.Value is not null)
                uiScope.CreateEffect(epochScope => track.Value = epochScope.Track(args.Value));

            if (args.Minimum is not null)
            {
                track.Minimum = args.Minimum.Value;
                if (args.Minimum.IsReactive)
                    uiScope.CreateEffect(epochScope => track.Minimum = epochScope.Track(args.Minimum));
            }
            
            if (args.Maximum is not null)
            {
                track.Maximum = args.Maximum.Value;
                if (args.Maximum.IsReactive)
                    uiScope.CreateEffect(epochScope => track.Maximum = epochScope.Track(args.Maximum));
            }
            
            if (args.ViewportSize is not null)
            {
                track.ViewportSize = args.ViewportSize.Value;
                if (args.ViewportSize.IsReactive)
                    uiScope.CreateEffect(epochScope => track.ViewportSize = epochScope.Track(args.ViewportSize));
            }
            
            if (args.IsDirectionReversed is not null)
            {
                track.IsDirectionReversed = args.IsDirectionReversed.Value;
                if (args.IsDirectionReversed.IsReactive)
                    uiScope.CreateEffect(epochScope => track.IsDirectionReversed = epochScope.Track(args.IsDirectionReversed));
            }
            
            if (args.IsDeferThumbDrag is not null)
            {
                track.DeferThumbDrag = args.IsDeferThumbDrag.Value;
                if (args.IsDeferThumbDrag.IsReactive)
                    uiScope.CreateEffect(epochScope => track.DeferThumbDrag = epochScope.Track(args.IsDeferThumbDrag));
            }
            
            if (args.IsIgnoreThumbDrag is not null)
            {
                track.IgnoreThumbDrag = args.IsIgnoreThumbDrag.Value;
                if (args.IsIgnoreThumbDrag.IsReactive)
                    uiScope.CreateEffect(epochScope => track.IgnoreThumbDrag = epochScope.Track(args.IsIgnoreThumbDrag));
            }

            _ = args.IncreaseButton?.Content;
            track.IncreaseButton = args.IncreaseButton?.Expose;
            
            _ = args.DecreaseButton?.Content;
            track.DecreaseButton = args.DecreaseButton?.Expose;
            
            _ = args.Thumb.Content;
            track.Thumb = args.Thumb.Expose;
            
            return (track, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                if (styleValue.Orientation is not null) track.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}