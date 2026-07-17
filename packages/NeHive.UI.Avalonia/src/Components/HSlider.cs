using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HSliderPart
{
    internal IElement<Track>? TrackElement { get; private set; }
    internal IElement<Button>? DecreaseButtonElement { get; private set; }
    internal IElement<Button>? IncreaseButtonElement { get; private set; }

    public IElement<Track> Track(IElement<Track> element) => TrackElement = element;
    public IElement<Button> DecreaseButton(IElement<Button> element) => DecreaseButtonElement = element;
    public IElement<Button> IncreaseButton(IElement<Button> element) => IncreaseButtonElement = element;
}

public class HSliderProp(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<bool>? isSnapToTickEnabled = null,
    Accessor<double>? tickFrequency = null,
    Accessor<TickPlacement>? tickPlacement = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;

    public readonly Accessor<double> Minimum = minimum ?? 0.0;
    public readonly Accessor<double> Maximum = maximum ?? 100.0;
    public readonly Accessor<double>? SmallChange = smallChange;
    public readonly Accessor<double>? LargeChange = largeChange;

    public readonly Accessor<bool> IsSnapToTickEnabled = isSnapToTickEnabled ?? false;
    public readonly Accessor<double> TickFrequency = tickFrequency ?? 1.0;

    public readonly Accessor<TickPlacement> TickPlacement =
        tickPlacement ?? global::Avalonia.Controls.TickPlacement.None;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public readonly Action<RangeBaseValueChangedEventArgs>? OnValueChanged = onValueChanged;

    public Func<HSliderPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<Slider> HSlider(HSliderProp prop)
    {
        return Element<Slider>.WithScope(uiScope =>
        {
            var slider = new Slider();
            var border = new Border
            {
                Child = slider
            };

            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants,
                Variants = prop.Variants
            };

            state.ApplyAccessorStyle(prop.Style, slider, border, ApplyStyle);
            state.ApplyVariantsStyle(slider, border, ApplyStyle);

            RangeBaseUtil.BindAccessor(uiScope, slider, prop.Value, prop.BindValue, prop.Minimum, prop.Maximum,
                prop.SmallChange, prop.LargeChange, prop.OnValueChanged);

            slider.IsSnapToTickEnabled = prop.IsSnapToTickEnabled.Value;
            if (prop.IsSnapToTickEnabled.IsReactive)
                uiScope.CreateEffect(epochScope =>
                    slider.IsSnapToTickEnabled = epochScope.Track(prop.IsSnapToTickEnabled));

            slider.TickFrequency = prop.TickFrequency.Value;
            if (prop.TickFrequency.IsReactive)
                uiScope.CreateEffect(epochScope => slider.TickFrequency = epochScope.Track(prop.TickFrequency));

            slider.TickPlacement = prop.TickPlacement.Value;
            if (prop.TickPlacement.IsReactive)
                uiScope.CreateEffect(epochScope => slider.TickPlacement = epochScope.Track(prop.TickPlacement));

            if (prop.Template is not null)
            {
                var part = new HSliderPart();
                var content = prop.Template(part).Content;
                slider.Template = new FuncControlTemplate((_, s) =>
                {
                    if (part.TrackElement is not null)
                    {
                        var __ = part.TrackElement.Content;
                        var trackElement = part.TrackElement.Expose!;
                        trackElement.Name = "PART_Track";
                        s.Register("PART_Track", trackElement);
                    }

                    if (part.DecreaseButtonElement is not null)
                    {
                        var __ = part.DecreaseButtonElement.Content;
                        var decreaseButtonElement = part.DecreaseButtonElement.Expose!;
                        decreaseButtonElement.Name = "PART_DecreaseButton";
                        s.Register("PART_DecreaseButton", decreaseButtonElement);
                    }

                    if (part.IncreaseButtonElement is not null)
                    {
                        var __ = part.IncreaseButtonElement.Content;
                        var increaseButtonElement = part.IncreaseButtonElement.Expose!;
                        increaseButtonElement.Name = "PART_IncreaseButton";
                        s.Register("PART_IncreaseButton", increaseButtonElement);
                    }

                    return content;
                });
            }

            return (slider, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                if (styleValue.Width is not null)
                    slider.Width = styleValue.Width.Value;

                if (styleValue.Height is not null)
                    slider.Height = styleValue.Height.Value;

                if (styleValue.MinWidth is not null)
                    slider.MinWidth = styleValue.MinWidth.Value;

                if (styleValue.MaxWidth is not null)
                    slider.MaxWidth = styleValue.MaxWidth.Value;

                if (styleValue.MinHeight is not null)
                    slider.MinHeight = styleValue.MinHeight.Value;

                if (styleValue.MaxHeight is not null)
                    slider.MaxHeight = styleValue.MaxHeight.Value;

                if (styleValue.Orientation is not null) slider.Orientation = styleValue.Orientation.Value;
            }
        });
    }

    public static IElement HSlider(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Accessor<bool>? isSnapToTickEnabled = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickPlacement>? tickPlacement = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HSlider(new(value, bindValue, minimum, maximum, smallChange, largeChange, isSnapToTickEnabled,
        tickFrequency, tickPlacement, strStyle, style, variants, onValueChanged));
}