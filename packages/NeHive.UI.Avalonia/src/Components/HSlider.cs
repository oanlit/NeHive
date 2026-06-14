using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HSlider(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<Orientation>? orientation = null,
        Accessor<bool>? isSnapToTickEnabled = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickPlacement>? tickPlacement = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<double>? onValueChanged = null)
    {
        // 默认值
        minimum ??= 0.0;
        maximum ??= 100.0;
        orientation ??= Orientation.Horizontal;
        isSnapToTickEnabled ??= false;
        tickFrequency ??= 1.0;
        tickPlacement ??= TickPlacement.None;

        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

        var uiScope = new UiScope();
        var slider = new Slider();
        var border = new Border
        {
            Child = slider
        };
        
        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, slider, border, ApplyStyle);
        state.ApplyVariantsStyle(slider, border, ApplyStyle);

        // === 双向绑定 bindValue ===
        if (bindValue is not null)
        {
            // 信号 -> 控件
            uiScope.CreateEffect(() => slider.Value = bindValue.RxValue);
            // 控件 -> 信号
            slider.PropertyChanged += (_, e) =>
            {
                if (e.Property != Slider.ValueProperty) return;
                var newVal = slider.Value;
                if (Math.Abs(newVal - bindValue.RxValue) > 0.0001)
                    bindValue.RxValue = newVal;
            };
        }
        else if (value is not null)
        {
            uiScope.CreateEffect(() => slider.Value = value.RxValue);
        }

        slider.ValueChanged += (_, e) => onValueChanged?.Invoke(e.NewValue);

        // === 单向绑定其他属性 ===
        slider.Minimum = minimum.Value;
        if(minimum.IsReactive)
            uiScope.CreateEffect(epochScope => slider.Minimum = epochScope.Track(minimum));
        
        slider.Maximum = maximum.Value;
        if(maximum.IsReactive)
            uiScope.CreateEffect(epochScope => slider.Maximum = epochScope.Track(maximum));

        slider.Orientation = orientation.Value;
        if(orientation.IsReactive)
            uiScope.CreateEffect(epochScope => slider.Orientation = epochScope.Track(orientation));
        
        slider.IsSnapToTickEnabled = isSnapToTickEnabled.Value;
        if(isSnapToTickEnabled.IsReactive)
            uiScope.CreateEffect(epochScope => slider.IsSnapToTickEnabled = epochScope.Track(isSnapToTickEnabled));
        
        slider.TickFrequency = tickFrequency.Value;
        if(isSnapToTickEnabled.IsReactive)
            uiScope.CreateEffect(epochScope => slider.TickFrequency = epochScope.Track(tickFrequency));
        
        slider.TickPlacement = tickPlacement.Value;
        if(isSnapToTickEnabled.IsReactive)
            uiScope.CreateEffect(epochScope => slider.TickPlacement = epochScope.Track(tickPlacement));

        return new Element(uiScope, border);

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
        }
    }
}