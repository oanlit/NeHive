using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using NeHive.Model;
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

public class HSliderProps(Scope scope, Border border, Slider slider) : BaseComponentProps(scope, border, slider)
{
    public MutSignal<double> Value
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.Value);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;
            scope.CreateEffect(epoch => slider.Value = epoch.Pull(sig));

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.ValueProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> Minimum
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.Minimum);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.MinimumProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> Maximum
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.Maximum);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.MaximumProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> SmallChange
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.SmallChange);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.SmallChangeProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> LargeChange
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.SmallChange);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.LargeChangeProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsDirectionReversed
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(slider.IsDirectionReversed);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Slider.IsDirectionReversedProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsSnapToTickEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(slider.IsSnapToTickEnabled);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Slider.IsSnapToTickEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<double> TickFrequency
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(slider.TickFrequency);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Slider.TickFrequencyProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<TickPlacement> TickPlacement
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TickPlacement>(slider.TickPlacement);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Slider.TickPlacementProperty)
                    sig.RxValue = (TickPlacement)args.NewValue!;
            }
        }
    }
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
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;

    public readonly Accessor<double> Minimum = minimum ?? 0.0;
    public readonly Accessor<double> Maximum = maximum ?? 100.0;
    public readonly Accessor<double>? SmallChange = smallChange;
    public readonly Accessor<double>? LargeChange = largeChange;

    public readonly Accessor<bool> IsDirectionReversed = isDirectionReversed ?? false;
    public readonly Accessor<bool> IsSnapToTickEnabled = isSnapToTickEnabled ?? false;
    public readonly Accessor<double> TickFrequency = tickFrequency ?? 1.0;

    public readonly Accessor<TickPlacement> TickPlacement =
        tickPlacement ?? global::Avalonia.Controls.TickPlacement.None;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public readonly Action<RangeBaseValueChangedEventArgs>? OnValueChanged = onValueChanged;

    public Func<HSliderPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<Slider> HSlider(Func<HSliderProps, HSliderArgs> fn)
    {
        return Element<Slider>.WithScope(uiScope =>
        {
            var slider = new Slider();
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

            RangeBaseUtil.BindAccessor(uiScope, slider, args.Value, args.BindValue, args.Minimum, args.Maximum,
                args.SmallChange, args.LargeChange, args.OnValueChanged);

            slider.IsDirectionReversed = args.IsDirectionReversed.Value;
            if (args.IsDirectionReversed.IsReactive)
                uiScope.CreateEffect(epochScope =>
                    slider.IsDirectionReversed = epochScope.Track(args.IsDirectionReversed));

            slider.IsSnapToTickEnabled = args.IsSnapToTickEnabled.Value;
            if (args.IsSnapToTickEnabled.IsReactive)
                uiScope.CreateEffect(epochScope =>
                    slider.IsSnapToTickEnabled = epochScope.Track(args.IsSnapToTickEnabled));

            slider.TickFrequency = args.TickFrequency.Value;
            if (args.TickFrequency.IsReactive)
                uiScope.CreateEffect(epochScope => slider.TickFrequency = epochScope.Track(args.TickFrequency));

            slider.TickPlacement = args.TickPlacement.Value;
            if (args.TickPlacement.IsReactive)
                uiScope.CreateEffect(epochScope => slider.TickPlacement = epochScope.Track(args.TickPlacement));

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
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HSlider(_ => new(value, bindValue, minimum, maximum, smallChange, largeChange, isDirectionReversed,
        isSnapToTickEnabled, tickFrequency, tickPlacement, strStyle, style, onValueChanged));
}