using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    /// <summary>
    /// 创建滑块控件
    /// </summary>
    /// <param name="bindValue">当前值（支持双向绑定，传入 MutSignal 即可双向）</param>
    /// <param name="minimum">最小值</param>
    /// <param name="maximum">最大值</param>
    /// <param name="orientation">方向（水平/垂直）</param>
    /// <param name="isSnapToTickEnabled">是否对齐刻度</param>
    /// <param name="tickFrequency">刻度间隔</param>
    /// <param name="tickPlacement">刻度位置</param>
    /// <param name="strStyle">样式字符串（如 "w-200 h-20"）</param>
    public static IElement HSlider(
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<Orientation>? orientation = null,
        Accessor<bool>? isSnapToTickEnabled = null,
        Accessor<double>? tickFrequency = null,
        Accessor<TickPlacement>? tickPlacement = null,
        Accessor<string>? strStyle = null)
    {
        // 默认值
        minimum ??= 0.0;
        maximum ??= 100.0;
        orientation ??= Orientation.Horizontal;
        isSnapToTickEnabled ??= false;
        tickFrequency ??= 1.0;
        tickPlacement ??= TickPlacement.None;

        var uiScope = new UiScope();
        var slider = new Slider();

        // === 双向绑定 bindValue ===
        if (bindValue != null)
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

        // === 单向绑定其他属性 ===
        uiScope.CreateEffect(() => slider.Minimum = minimum.RxValue);
        uiScope.CreateEffect(() => slider.Maximum = maximum.RxValue);
        uiScope.CreateEffect(() => slider.Orientation = orientation.RxValue);
        uiScope.CreateEffect(() => slider.IsSnapToTickEnabled = isSnapToTickEnabled.RxValue);
        uiScope.CreateEffect(() => slider.TickFrequency = tickFrequency.RxValue);
        uiScope.CreateEffect(() => slider.TickPlacement = tickPlacement.RxValue);

        // === 应用样式字符串 ===
        if (strStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var styleStr = scope.Track(strStyle);
                ApplySliderStyle(slider, styleStr);
            });
        }

        return new Element(uiScope, slider);
    }

    // 样式解析辅助函数（复用你现有的 StyleParser）
    private static void ApplySliderStyle(Slider slider, string styleStr)
    {
        var result = new StyleSet();
        StyleParser.Parse(styleStr, ref result);

        // 布局属性
        if (result.Margin is not null) slider.Margin = result.Margin.Value;
        if (result.ZIndex is not null) slider.ZIndex = result.ZIndex.Value;

        if (result.Width is not null) slider.Width = result.Width.Value;
        if (result.Height is not null) slider.Height = result.Height.Value;
        if (result.MinWidth is not null) slider.MinWidth = result.MinWidth.Value;
        if (result.MaxWidth is not null) slider.MaxWidth = result.MaxWidth.Value;
        if (result.MinHeight is not null) slider.MinHeight = result.MinHeight.Value;
        if (result.MaxHeight is not null) slider.MaxHeight = result.MaxHeight.Value;

        if (result.Padding is not null) slider.Padding = result.Padding.Value;

        if (result.HorizontalAlignment is not null) slider.HorizontalAlignment = result.HorizontalAlignment.Value;
        if (result.VerticalAlignment is not null) slider.VerticalAlignment = result.VerticalAlignment.Value;

        if (result.Background is not null) slider.Background = result.Background;

        // Slider 特有样式（可通过 StyleParser 扩展支持，如 tick-placement, orientation 等）
        // 此处保留扩展点
    }
}