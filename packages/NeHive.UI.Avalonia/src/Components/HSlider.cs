using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HSliderProps(Scope scope, Border border, Slider slider) : HRangeBaseProps(scope, border, slider)
{
    public MutSignal<bool> IsDirectionReversed
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(slider.IsDirectionReversed);
            BridgeAvalonia.BindPropertySignal(scope, field, slider, Slider.IsDirectionReversedProperty);
            return field;
        }
    }

    public MutSignal<bool> IsSnapToTickEnabled
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(slider.IsSnapToTickEnabled);
            BridgeAvalonia.BindPropertySignal(scope, field, slider, Slider.IsSnapToTickEnabledProperty);
            return field;
        }
    }

    public MutSignal<double> TickFrequency
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(slider.TickFrequency);
            BridgeAvalonia.BindPropertySignal(scope, field, slider, Slider.TickFrequencyProperty);
            return field;
        }
    }

    public MutSignal<TickPlacement> TickPlacement
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<TickPlacement>(slider.TickPlacement);
            BridgeAvalonia.BindPropertySignal(scope, field, slider, Slider.TickPlacementProperty);
            return field;
        }
    }
}

public class HSliderPart
{
    internal IElement<Track>? TrackElement { get; private set; }
    internal IElement<Button>? DecreaseButtonElement { get; private set; }
    internal IElement<Button>? IncreaseButtonElement { get; private set; }

    public IElement<Track> Track(IElement<Track> element) => TrackElement = element;
    public IElement<Button> DecreaseButton(IElement<Button> element) => DecreaseButtonElement = element;
    public IElement<Button> IncreaseButton(IElement<Button> element) => IncreaseButtonElement = element;
}

public class HSliderArgs(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<bool>? isDirectionReversed = null,
    Accessor<bool>? isSnapToTickEnabled = null,
    Accessor<double>? tickFrequency = null,
    Accessor<TickPlacement>? tickPlacement = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
) : HRangeBaseArgs(value, bindValue, minimum, maximum, smallChange, largeChange, strStyle, style, baseInteraction,
    onValueChanged)
{
    public readonly Accessor<bool> IsDirectionReversed = isDirectionReversed ?? false;
    public readonly Accessor<bool> IsSnapToTickEnabled = isSnapToTickEnabled ?? false;
    public readonly Accessor<double> TickFrequency = tickFrequency ?? 1.0;

    public readonly Accessor<TickPlacement> TickPlacement =
        tickPlacement ?? global::Avalonia.Controls.TickPlacement.None;

    public Func<HSliderPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement HSlider(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Accessor<bool>? isDirectionReversed = null,
        Accessor<bool>? isSnapToTickEnabled = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickPlacement>? tickPlacement = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HSlider(out _, _ => new(value, bindValue, minimum, maximum, smallChange, largeChange, isDirectionReversed,
        isSnapToTickEnabled, tickFrequency, tickPlacement, strStyle, style, baseInteraction, onValueChanged));

    public static IElement HSlider(
        out Slider expose,
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Accessor<bool>? isDirectionReversed = null,
        Accessor<bool>? isSnapToTickEnabled = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickPlacement>? tickPlacement = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HSlider(out expose, _ => new(value, bindValue, minimum, maximum, smallChange, largeChange, isDirectionReversed,
        isSnapToTickEnabled, tickFrequency, tickPlacement, strStyle, style, baseInteraction, onValueChanged));

    public static IElement<Slider> HSlider(Func<HSliderProps, HSliderArgs> fn) => HSlider(out _, fn);

    public static IElement<Slider> HSlider(out Slider expose, Func<HSliderProps, HSliderArgs> fn)
    {
        var slider = new Slider();
        expose = slider;
        return Element<Slider>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = slider
            };

            var props = new HSliderProps(uiScope, border, slider);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, slider, border, ApplyStyle);
            state.ApplyVariantsStyle(slider, border, ApplyStyle);

            args.BaseInteraction?.ApplyInteractions(uiScope, slider);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            RangeBaseUtil.BindAccessor(uiScope, slider, args);

            slider.IsDirectionReversed = args.IsDirectionReversed.Value;
            if (args.IsDirectionReversed.IsReactive)
                uiScope.CreateEffect(epoch =>
                    slider.IsDirectionReversed = epoch.Track(args.IsDirectionReversed));

            slider.IsSnapToTickEnabled = args.IsSnapToTickEnabled.Value;
            if (args.IsSnapToTickEnabled.IsReactive)
                uiScope.CreateEffect(epoch =>
                    slider.IsSnapToTickEnabled = epoch.Track(args.IsSnapToTickEnabled));

            slider.TickFrequency = args.TickFrequency.Value;
            if (args.TickFrequency.IsReactive)
                uiScope.CreateEffect(epoch => slider.TickFrequency = epoch.Track(args.TickFrequency));

            slider.TickPlacement = args.TickPlacement.Value;
            if (args.TickPlacement.IsReactive)
                uiScope.CreateEffect(epoch => slider.TickPlacement = epoch.Track(args.TickPlacement));

            if (args.Template is not null)
            {
                var part = new HSliderPart();
                var content = args.Template(part).Content;
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
}