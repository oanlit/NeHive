using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

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
        Accessor<FullStyle>? style = null,
        Action<double>? onValueChanged = null)
    {
        // 默认值
        minimum ??= 0.0;
        maximum ??= 100.0;
        orientation ??= Orientation.Horizontal;
        isSnapToTickEnabled ??= false;
        tickFrequency ??= 1.0;
        tickPlacement ??= TickPlacement.None;
        
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        var uiScope = new UiScope();
        var slider = new Slider();
        var border = new Border
        {
            Child = slider
        };

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

        slider.ValueChanged += (_, e) => { onValueChanged?.Invoke(e.NewValue); };

        // === 单向绑定其他属性 ===
        uiScope.CreateEffect(() => slider.Minimum = minimum.RxValue);
        uiScope.CreateEffect(() => slider.Maximum = maximum.RxValue);
        uiScope.CreateEffect(() => slider.Orientation = orientation.RxValue);
        uiScope.CreateEffect(() => slider.IsSnapToTickEnabled = isSnapToTickEnabled.RxValue);
        uiScope.CreateEffect(() => slider.TickFrequency = tickFrequency.RxValue);
        uiScope.CreateEffect(() => slider.TickPlacement = tickPlacement.RxValue);

        // === 应用样式字符串 ===
        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style).Normal;
                StyleUtil.ApplyStyle(styleValue, slider, border);
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
            });
        }

        return new Element(uiScope, border);
        
    }
}