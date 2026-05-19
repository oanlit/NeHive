using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ProgressBar 样式配置
/// </summary>
public class HProgressBarStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    Thickness? padding = null,
    Orientation? orientation = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    IBrush? background = null,
    IBrush? borderBrush = null,
    Thickness? borderThickness = null,
    CornerRadius? cornerRadius = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public int? ZIndex { get; private set; } = zIndex;

    public double? Width { get; private set; } = width;
    public double? Height { get; private set; } = height;
    public double? MinWidth { get; private set; } = minWidth;
    public double? MaxWidth { get; private set; } = maxWidth;
    public double? MinHeight { get; private set; } = minHeight;
    public double? MaxHeight { get; private set; } = maxHeight;

    public Thickness? Padding { get; private set; } = padding;

    public Orientation Orientation { get; private set; } = orientation ?? Orientation.Horizontal;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Left;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Top;

    public IBrush? Background { get; private set; } = background;
    public IBrush? BorderBrush { get; private set; } = borderBrush;
    public Thickness? BorderThickness { get; private set; } = borderThickness;
    public CornerRadius? CornerRadius { get; private set; } = cornerRadius;

    public HProgressBarStyle Merge(
        Thickness? margin = null,
        int? zIndex = null,
        double? width = null,
        double? height = null,
        double? minWidth = null,
        double? maxWidth = null,
        double? minHeight = null,
        double? maxHeight = null,
        Thickness? padding = null,
        Orientation? orientation = null,
        HorizontalAlignment? horizontalAlignment = null,
        VerticalAlignment? verticalAlignment = null,
        IBrush? background = null,
        IBrush? borderBrush = null,
        Thickness? borderThickness = null,
        CornerRadius? cornerRadius = null)
    {
        if (margin is not null) Margin = margin.Value;
        if (zIndex is not null) ZIndex = zIndex.Value;

        if (width is not null) Width = width;
        if (height is not null) Height = height;
        if (minWidth is not null) MinWidth = minWidth;
        if (maxWidth is not null) MaxWidth = maxWidth;
        if (minHeight is not null) MinHeight = minHeight;
        if (maxHeight is not null) MaxHeight = maxHeight;

        if (padding is not null) Padding = padding;

        if (orientation is not null) Orientation = orientation.Value;
        if (horizontalAlignment is not null) HorizontalAlignment = horizontalAlignment.Value;
        if (verticalAlignment is not null) VerticalAlignment = verticalAlignment.Value;

        if (background is not null) Background = background;
        if (borderBrush is not null) BorderBrush = borderBrush;
        if (borderThickness is not null) BorderThickness = borderThickness;
        if (cornerRadius is not null) CornerRadius = cornerRadius;
        return this;
    }

    public HProgressBarStyle Merge(HProgressBarStyle style)
    {
        Margin = style.Margin;
        if (style.ZIndex is not null) ZIndex = style.ZIndex;

        if (style.Width is not null) Width = style.Width;
        if (style.Height is not null) Height = style.Height;
        if (style.MinWidth is not null) MinWidth = style.MinWidth;
        if (style.MaxWidth is not null) MaxWidth = style.MaxWidth;
        if (style.MinHeight is not null) MinHeight = style.MinHeight;
        if (style.MaxHeight is not null) MaxHeight = style.MaxHeight;

        if (style.Padding is not null) Padding = style.Padding;

        Orientation = style.Orientation;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        if (style.Background is not null) Background = style.Background;
        if (style.BorderBrush is not null) BorderBrush = style.BorderBrush;
        if (style.BorderThickness is not null) BorderThickness = style.BorderThickness;
        if (style.CornerRadius is not null) CornerRadius = style.CornerRadius;
        return this;
    }

    public static HProgressBarStyle Default => new();

    public static Accessor<HProgressBarStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HProgressBarStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HProgressBarStyle(
                margin: result.Margin,
                width: result.Width,
                height: result.Height,
                minWidth: result.MinWidth,
                maxWidth: result.MaxWidth,
                minHeight: result.MinHeight,
                maxHeight: result.MaxHeight,
                padding: result.Padding,
                horizontalAlignment: result.HorizontalAlignment,
                verticalAlignment: result.VerticalAlignment
            );
        });
    }
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 ProgressBar 控件
    /// </summary>
    /// <param name="value">当前值（支持响应式）</param>
    /// <param name="minimum">最小值（默认0）</param>
    /// <param name="maximum">最大值（默认100）</param>
    /// <param name="isIndeterminate">是否为不确定进度（响应式）</param>
    /// <param name="strStyle">样式字符串</param>
    /// <param name="style">直接样式对象</param>
    public static IElement HProgressBar(
        Accessor<double>? value = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<bool>? isIndeterminate = null,
        Accessor<string>? strStyle = null,
        Accessor<HProgressBarStyle>? style = null)
    {
        value ??= 0;
        minimum ??= 0;
        maximum ??= 100;
        isIndeterminate ??= false;

        // 样式合并
        if (style is not null && strStyle is not null)
        {
            style = new Computed<HProgressBarStyle>(() =>
                HProgressBarStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle is not null)
        {
            style = HProgressBarStyle.Parse(strStyle);
        }

        var uiScope = new UiScope();
        var progressBar = new ProgressBar();

        // 绑定属性
        uiScope.CreateEffect(() => progressBar.Value = value.RxValue);
        uiScope.CreateEffect(() => progressBar.Minimum = minimum.RxValue);
        uiScope.CreateEffect(() => progressBar.Maximum = maximum.RxValue);
        uiScope.CreateEffect(() => progressBar.IsIndeterminate = isIndeterminate.RxValue);

        // 应用样式
        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                ApplyStyle(progressBar, styleValue);
            });
        }

        return new Element(uiScope, progressBar);

        void ApplyStyle(ProgressBar pb, HProgressBarStyle styleValue)
        {
            pb.Margin = styleValue.Margin;
            if (styleValue.ZIndex is not null) pb.ZIndex = styleValue.ZIndex.Value;

            if (styleValue.Width is not null) pb.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) pb.Height = styleValue.Height.Value;
            if (styleValue.MinWidth is not null) pb.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxWidth is not null) pb.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinHeight is not null) pb.MinHeight = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight is not null) pb.MaxHeight = styleValue.MaxHeight.Value;

            if (styleValue.Padding is not null) pb.Padding = styleValue.Padding.Value;

            pb.Orientation = styleValue.Orientation;
            pb.HorizontalAlignment = styleValue.HorizontalAlignment;
            pb.VerticalAlignment = styleValue.VerticalAlignment;

            if (styleValue.Background is not null) pb.Background = styleValue.Background;
            if (styleValue.BorderBrush is not null) pb.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness is not null) pb.BorderThickness = styleValue.BorderThickness.Value;
            if (styleValue.CornerRadius is not null) pb.CornerRadius = styleValue.CornerRadius.Value;
        }
    }
}