using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components.Shapes;

public class HShapeStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    IBrush? background = null,
    IBrush? borderBrush = null,
    Thickness? borderThickness = null,
    CornerRadius? cornerRadius = null,
    double? opacity = null)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public int? ZIndex { get; private set; } = zIndex;

    public double? Width { get; private set; } = width;
    public double? Height { get; private set; } = height;
    public double? MinWidth { get; private set; } = minWidth;
    public double? MaxWidth { get; private set; } = maxWidth;

    public double? MinHeight { get; private set; } = minHeight;
    public double? MaxHeight { get; private set; } = maxHeight;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Stretch;

    public IBrush? Background { get; private set; } = background;
    public IBrush BorderBrush = borderBrush ?? Brushes.Gray;
    public Thickness BorderThickness = borderThickness ?? new Thickness(1);
    public CornerRadius CornerRadius = cornerRadius ?? new CornerRadius(4);

    public double? Opacity { get; private set; } = opacity;
    // 可以继续添加更多样式属性，例如 Shadow, Opacity 等


    public HShapeStyle Merge(HShapeStyle style)
    {
        Margin = style.Margin;
        ZIndex = style.ZIndex;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        Background = style.Background;
        BorderBrush = style.BorderBrush;
        BorderThickness = style.BorderThickness;
        CornerRadius = style.CornerRadius;
        Opacity = style.Opacity;

        return this;
    }

    public static HPanelStyle Default => new();

    public static Accessor<HShapeStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HShapeStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HShapeStyle(
                margin: result.Margin,
                zIndex: result.ZIndex,
                width: result.Width,
                height: result.Height,
                minWidth: result.MinWidth,
                maxWidth: result.MaxWidth,
                minHeight: result.MinHeight,
                maxHeight: result.MaxHeight,
                horizontalAlignment: result.HorizontalAlignment,
                verticalAlignment: result.VerticalAlignment,
                background: result.Background,
                borderBrush: result.BorderBrush,
                borderThickness: result.BorderThickness,
                cornerRadius: result.CornerRadius,
                opacity: result.Opacity
            );
        });
    }
}

public static class Shapes
{
    public static IElement HRectangle()
    {
        var uiScope = new UiScope();
        // 创建 Rectangle 控件
        var rect = new Rectangle();

        Canvas.SetLeft(rect, 10);
        Canvas.SetTop(rect, 10);

        return new Element(uiScope, rect);

        // void ApplyStyle(HShapeStyle style)
        // {
        //     rect.Margin = style.Margin;
        //     if (style.ZIndex is not null) rect.ZIndex = style.ZIndex.Value;
        //
        //     if (style.Width is not null) rect.Width = style.Width.Value;
        //     if (style.Height is not null) rect.Height = style.Height.Value;
        //     if (style.MinWidth is not null) rect.MinWidth = style.MinWidth.Value;
        //     if (style.MaxWidth is not null) rect.MaxWidth = style.MaxWidth.Value;
        //     if (style.MinHeight is not null) rect.MinHeight = style.MinHeight.Value;
        //     if (style.MaxHeight is not null) rect.MaxHeight = style.MaxHeight.Value;
        //
        //     rect.HorizontalAlignment = style.HorizontalAlignment;
        //     rect.VerticalAlignment = style.VerticalAlignment;
        //
        //     rect.Fill = style.Background;
        //     rect.Stroke = style.BorderBrush;
        //     rect.StrokeThickness =
        //         (style.BorderThickness.Top + style.BorderThickness.Bottom +
        //          style.BorderThickness.Left + style.BorderThickness.Right) / 4.0;
        //     
        //     if (style.Opacity is not null) rect.Opacity = style.Opacity.Value;
        //     // rect.RenderTransform = new TranslateTransform
        //     // {
        //     //     X = 1.0,
        //     //     Y = 1.0
        //     // };
        //     // rect.RenderTransformOrigin
        // }
    }
}