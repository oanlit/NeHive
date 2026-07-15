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
    Accessor<Orientation>? orientation = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;
    
    public readonly Accessor<double> Minimum = minimum ?? 0.0;
    public readonly Accessor<double> Maximum = maximum ?? 100.0;
    public readonly Accessor<Orientation> Orientation = orientation ?? global::Avalonia.Layout.Orientation.Horizontal;
    
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

            state.ApplyAccessorStyle(prop.Style, track, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(track, border, StyleUtil.ApplyStyle);
            
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
            
            track.Orientation = prop.Orientation.Value;
            if (prop.Orientation.IsReactive)
                uiScope.CreateEffect(epochScope => track.Orientation = epochScope.Track(prop.Orientation));

            _ = prop.IncreaseButton?.Content;
            track.IncreaseButton = prop.IncreaseButton?.Expose;
            
            _ = prop.DecreaseButton?.Content;
            track.DecreaseButton = prop.DecreaseButton?.Expose;
            
            _ = prop.Thumb.Content;
            track.Thumb = prop.Thumb.Expose;
            
            return (track, border);
        });
    }
}