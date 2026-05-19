using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// Image 组件的样式配置，包含布局属性和 Stretch 等特有属性
/// </summary>
public class HImageStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    Thickness? padding = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    double? opacity = null
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

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Left;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Top;

    public double Opacity { get; private set; } = opacity ?? 1.0;

    /// <summary>
    /// 合并新属性（用于组合样式）
    /// </summary>
    public HImageStyle Merge(
        Thickness? margin = null,
        int? zIndex = null,
        double? width = null,
        double? height = null,
        double? minWidth = null,
        double? maxWidth = null,
        double? minHeight = null,
        double? maxHeight = null,
        Thickness? padding = null,
        HorizontalAlignment? horizontalAlignment = null,
        VerticalAlignment? verticalAlignment = null,
        double? opacity = null
    )
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

        if (horizontalAlignment is not null) HorizontalAlignment = horizontalAlignment.Value;
        if (verticalAlignment is not null) VerticalAlignment = verticalAlignment.Value;

        if (opacity is not null) Opacity = opacity.Value;

        return this;
    }

    /// <summary>
    /// 合并另一个 HImageStyle 对象
    /// </summary>
    public HImageStyle Merge(HImageStyle style)
    {
        Margin = style.Margin;
        ZIndex = style.ZIndex;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;
        Padding = style.Padding;

        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        Opacity = style.Opacity;
        return this;
    }

    public static HImageStyle Default => new();

    /// <summary>
    /// 从样式字符串解析（复用 StyleParser，需扩展支持 stretch 等属性）
    /// </summary>
    public static Accessor<HImageStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HImageStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HImageStyle(
                margin: result.Margin,
                width: result.Width,
                height: result.Height,
                minWidth: result.MinWidth,
                maxWidth: result.MaxWidth,
                minHeight: result.MinHeight,
                maxHeight: result.MaxHeight,
                padding: result.Padding,
                horizontalAlignment: result.HorizontalAlignment,
                verticalAlignment: result.VerticalAlignment,
                opacity: result.Opacity
                // stretch: result.Stretch,      // 需要在 StyleParser 中支持
                // stretchDirection: result.StretchDirection
            );
        });
    }
}

public static partial class BaseComponent
{
    public static IElement HImage(
        Accessor<Bitmap?> source,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<HImageStyle>? style = null
    )
    {
        // 样式优先级：如果同时提供了 strStyle 和 style，则合并
        if (style is not null && strStyle is not null)
        {
            style = new Computed<HImageStyle>(() =>
                HImageStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle is not null)
        {
            style = HImageStyle.Parse(strStyle);
        }

        var uiScope = new UiScope();
        var image = new Image();

        // 绑定 Source

        uiScope.CreateEffect(() => image.Source = source.RxValue);

        // 绑定样式
        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                ApplyImageStyle(image, styleValue);
            });
        }

        if (stretch is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var stretchValue = epochScope.Track(stretch);
                image.Stretch = stretchValue;
            });
        }

        return new Element(uiScope, image);

        void ApplyImageStyle(Image img, HImageStyle styleValue)
        {
            // 布局属性
            img.Margin = styleValue.Margin;
            if (styleValue.ZIndex is not null) img.ZIndex = styleValue.ZIndex.Value;

            if (styleValue.Width is not null) img.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) img.Height = styleValue.Height.Value;
            if (styleValue.MinWidth is not null) img.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxWidth is not null) img.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinHeight is not null) img.MinHeight = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight is not null) img.MaxHeight = styleValue.MaxHeight.Value;

            // if (styleValue.Padding is not null) img.Padding = styleValue.Padding.Value;

            img.HorizontalAlignment = styleValue.HorizontalAlignment;
            img.VerticalAlignment = styleValue.VerticalAlignment;

            img.Opacity = styleValue.Opacity;

            // Image 特有属性
            // img.StretchDirection = styleValue.StretchDirection;
        }
    }

    // 可选：支持从 Uri 或字符串路径加载图像的重载版本
    public static IElement HUriImage(
        Accessor<string?> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<HImageStyle>? style = null
    )
    {
        var sourceSignal = new Computed<Bitmap?>(() =>
        {
            var u = uri.RxValue;
            if (string.IsNullOrEmpty(u)) return null;
            // 这里需要根据你的项目实现实际的图像加载逻辑
            // 例如：new Bitmap(u) 或使用 AssetLoader
            return LoadBitmapFromUri(u);
        });
        return HImage(sourceSignal, stretch, strStyle, style);
    }

    // 示例：简单的 Uri 加载（实际应缓存和异步处理）
    private static Bitmap? LoadBitmapFromUri(string uri)
    {
        try
        {
            return new Bitmap(uri);
        }
        catch
        {
            return null;
        }
    }
}