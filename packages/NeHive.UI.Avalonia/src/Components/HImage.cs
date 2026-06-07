using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Platform;
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
    double? opacity = null,
    AdvancedStyle? advanced = null
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

    public VerticalAlignment VerticalAlignment { get; private set; } =
        verticalAlignment ?? VerticalAlignment.Top;

    public double Opacity { get; private set; } = opacity ?? 1.0;

    public AdvancedStyle? Advanced { get; private set; } = advanced;

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
        double? opacity = null,
        AdvancedStyle? advanced = null
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

        Advanced = style.Advanced;
        return this;
    }

    public static HImageStyle Default => new();

    public static StyleSet DefaultStyleSet()
    {
        return new StyleSet
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
    }

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
                opacity: result.Opacity,
                advanced: result.Advanced
            );
        });
    }
}

public static partial class BaseComponent
{
    private class HImageState
    {
        // private StyleSet _currentStyle;
        // public bool ResetCurrentStyle;
        public StyleSet BaseStyle;

        public StyleSet CurrentStyle;

        public Dictionary<string, List<string>>? Variants;

        // 鼠标交互状态（悬停、按下等）
        public bool IsHover;

        public HImageState(StyleSet baseStyle)
        {
            BaseStyle = baseStyle;
            CurrentStyle = StyleUtil.Copy(BaseStyle);
        }

        public void ResetSetStyle()
        {
            CurrentStyle.Merge(BaseStyle);
            // CurrentStyle = StyleUtil.Copy(BaseStyle);
        }

        // public void SetCurrentStyle()
        // {
        //     if (Variants == null) return;
        //     List<string>? strs;
        //     if (IsHover && Variants.TryGetValue("hover", out strs))
        //     {
        //         StyleParser.Parse(strs, ref CurrentStyle);
        //     }
        // }

        public void SetHoverStyle()
        {
            if (Variants == null) return;
            if (IsHover && Variants.TryGetValue("hover", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
            }
        }
    }

    public static IElement HImage(
        Accessor<Bitmap?> source,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null
    )
    {
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        var uiScope = new UiScope();

        var image = new Image();

        var border = new Border
        {
            Child = image,
            ClipToBounds = true
        };

        // 绑定 Source
        uiScope.CreateEffect(() => image.Source = source.RxValue);

        HImageState state;

        if (style is null)
        {
            state = new HImageState(new StyleSet());
            ApplyStyle(state.BaseStyle);
        }
        // 绑定样式
        else
        {
            state = new HImageState(style.Value.Normal);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                state.BaseStyle = styleValue.Normal;
                state.Variants = styleValue.Variants;
                ApplyStyle(styleValue.Normal);
                state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
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

        // 事件挂载
        uiScope.OnMount += () =>
        {
            image.PointerExited += (_, _) =>
            {
                state.IsHover = false;
                state.ResetSetStyle();
                ApplyStyle(state.CurrentStyle);
            };

            image.PointerEntered += (_, _) =>
            {
                state.IsHover = true;
                state.SetHoverStyle();
                ApplyStyle(state.CurrentStyle);
            };
        };

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, image, border);
        }
    }

    // 可选：支持从 Uri 或字符串路径加载图像的重载版本
    public static IElement HUriImage(
        Accessor<string?> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null
    )
    {
        var sourceSignal = new Computed<Bitmap?>(() =>
        {
            var u = uri.RxValue;
            if (string.IsNullOrEmpty(u)) return null;

            if (!u.StartsWith("avares://")) return LoadBitmapFromUri(u);

            var avaresUri = new Uri(u);

            using var stream = AssetLoader.Open(avaresUri);
            return new Bitmap(stream);
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