using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Styles;

public static class StyleParser
{
    // 1单位 = 4px（Tailwind 标准）
    private const double UnitScale = 4;

    // 原子类字典：前缀 → 处理逻辑
    private static readonly Dictionary<string, Action<string[], StyleSet>> AtomHandlers = new()
    {
        ["m-"] = ApplyMargin,
        ["mx-"] = ApplyMarginX,
        ["my-"] = ApplyMarginY,
        ["mt-"] = ApplyMarginTop,
        ["mb-"] = ApplyMarginBottom,
        ["ml-"] = ApplyMarginLeft,
        ["mr-"] = ApplyMarginRight,
        ["z-"] = ApplyZIndex,

        // 尺寸
        ["w-"] = ApplyWidth,
        ["w-full"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Stretch,
        ["h-"] = ApplyHeight,
        ["h-full"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Stretch,
        ["min-w-"] = ApplyMinWidth,
        ["max-w-"] = ApplyMaxWidth,
        ["min-h-"] = ApplyMinHeight,
        ["max-h-"] = ApplyMaxHeight,

        ["p-"] = ApplyPadding,
        ["px-"] = ApplyPaddingX,
        ["py-"] = ApplyPaddingY,
        ["pt-"] = ApplyPaddingTop,
        ["pb-"] = ApplyPaddingBottom,
        ["pl-"] = ApplyPaddingLeft,
        ["pr-"] = ApplyPaddingRight,

        ["gap-x-"] = ApplyGapX,
        ["gap-y-"] = ApplyGapY,
        ["gap-"] = ApplyGap,

        ["overflow-visible"] = (_, set) => set.OverflowHandle = OverflowHandle.Visible,
        ["overflow-hidden"] = (_, set) => set.OverflowHandle = OverflowHandle.Hidden,

        ["flex-row"] = (_, set) => set.Orientation = Orientation.Horizontal,
        ["flex-col"] = (_, set) => set.Orientation = Orientation.Vertical,
        ["horizontal"] = (_, set) => set.Orientation = Orientation.Horizontal,
        ["vertical"] = (_, set) => set.Orientation = Orientation.Vertical,
        ["justify-start"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Left,
        ["justify-center"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Center,
        ["justify-end"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Right,
        ["justify-stretch"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Stretch,
        ["items-start"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Top,
        ["items-center"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Center,
        ["items-end"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Bottom,
        ["items-stretch"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Stretch,

        // 文本对齐
        ["text-center"] = (_, set) => set.TextAlignment = TextAlignment.Center,
        ["text-left"] = (_, set) => set.TextAlignment = TextAlignment.Left,
        ["text-right"] = (_, set) => set.TextAlignment = TextAlignment.Right,
        ["text-top"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Top,
        ["text-middle"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Center,
        ["text-bottom"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Bottom,
        ["wrap"] = (_, set) => set.TextWrapping = TextWrapping.Wrap,
        ["whitespace-nowrap"] = (_, set) => set.TextWrapping = TextWrapping.NoWrap,
        ["wrap-overflow"] = (_, set) => set.TextWrapping = TextWrapping.WrapWithOverflow,

        // 文本样式
        ["text-"] = ApplyText,
        ["text-xs"] = (_, set) => set.FontSize = 12,
        ["text-sm"] = (_, set) => set.FontSize = 14,
        ["text-base"] = (_, set) => set.FontSize = 16,
        ["text-lg"] = (_, set) => set.FontSize = 18,
        ["text-xl"] = (_, set) => set.FontSize = 20,
        ["text-2xl"] = (_, set) => set.FontSize = 24,
        ["text-3xl"] = (_, set) => set.FontSize = 30,

        ["font-thin"] = (_, set) => set.FontWeight = FontWeight.Thin,
        ["font-extralight"] = (_, set) => set.FontWeight = FontWeight.ExtraLight,
        ["font-light"] = (_, set) => set.FontWeight = FontWeight.Light,
        ["font-normal"] = (_, set) => set.FontWeight = FontWeight.Normal,
        ["font-medium"] = (_, set) => set.FontWeight = FontWeight.Medium,
        ["font-semibold"] = (_, set) => set.FontWeight = FontWeight.SemiBold,
        ["font-bold"] = (_, set) => set.FontWeight = FontWeight.Bold,
        ["font-extrabold"] = (_, set) => set.FontWeight = FontWeight.ExtraBold,
        ["font-black"] = (_, set) => set.FontWeight = FontWeight.Black,
        ["font-extrablack"] = (_, set) => set.FontWeight = FontWeight.ExtraBlack,

        ["italic"] = (_, set) => set.FontStyle = FontStyle.Italic,
        ["not-italic"] = (_, set) => set.FontStyle = FontStyle.Normal,

        ["fg-"] = ApplyForeground,
        ["fg-gradient-"] = (vals, set) => EnsureAdvanced(set).FgGradientDir = TryGetDir(vals),
        ["fg-from-"] = (vals, set) => EnsureAdvanced(set).FgFromColor = ParseColor(vals),
        ["fg-to-"] = (vals, set) => EnsureAdvanced(set).FgToColor = ParseColor(vals),

        // 背景
        ["bg-"] = ApplyBackground,
        ["bg-gradient-"] = (vals, set) => EnsureAdvanced(set).BgGradientDir = TryGetDir(vals),
        ["bg-from-"] = (vals, set) => EnsureAdvanced(set).BgFromColor = ParseColor(vals),
        ["bg-to-"] = (vals, set) => EnsureAdvanced(set).BgToColor = ParseColor(vals),
        ["gradient-"] = (vals, set) => EnsureAdvanced(set).BgGradientDir = TryGetDir(vals),
        ["from-"] = (vals, set) => EnsureAdvanced(set).BgFromColor = ParseColor(vals),
        ["to-"] = (vals, set) => EnsureAdvanced(set).BgToColor = ParseColor(vals),
        ["bg-center"] = (_, set) => set.BackgroundSizing = BackgroundSizing.CenterBorder,
        ["bg-inner"] = (_, set) => set.BackgroundSizing = BackgroundSizing.InnerBorderEdge,
        ["bg-outer"] = (_, set) => set.BackgroundSizing = BackgroundSizing.OuterBorderEdge,
        
        // 边框
        ["border-"] = ApplyBorderBrush,
        ["border-gradient-"] = (vals, set) => EnsureAdvanced(set).BorderGradientDir = TryGetDir(vals),
        ["border-from-"] = (vals, set) => EnsureAdvanced(set).BorderFromColor = ParseColor(vals),
        ["border-to-"] = (vals, set) => EnsureAdvanced(set).BorderToColor = ParseColor(vals),
        ["border-w-"] = ApplyBorderWidth, // 如 border-w-2
        ["border-t-"] = ApplyBorderTopWidth,
        ["border-r-"] = ApplyBorderRightWidth,
        ["border-b-"] = ApplyBorderBottomWidth,
        ["border-l-"] = ApplyBorderLeftWidth,
        ["rounded"] = (_, set) => set.CornerRadius = new CornerRadius(4),
        ["rounded-"] = ApplyCornerRadius,
        ["rounded-sm"] = (_, set) => set.CornerRadius = new CornerRadius(2),
        ["rounded-md"] = (_, set) => set.CornerRadius = new CornerRadius(4),
        ["rounded-lg"] = (_, set) => set.CornerRadius = new CornerRadius(8),
        ["rounded-xl"] = (_, set) => set.CornerRadius = new CornerRadius(12),
        ["rounded-2xl"] = (_, set) => set.CornerRadius = new CornerRadius(16),
        ["rounded-full"] = (_, set) => set.CornerRadius = new CornerRadius(9999),

        // 透明度 & 可见性
        ["opacity-"] = ApplyOpacity,
        ["visible"] = (_, set) => set.IsVisible = true,
        ["hidden"] = (_, set) => set.IsVisible = false,

        // 模糊
        ["blur-sm"] = (_, set) => EnsureAdvanced(set).Effect = new BlurEffect { Radius = 4 },
        ["blur"] = (_, set) => EnsureAdvanced(set).Effect = new BlurEffect { Radius = 8 },
        ["blur-md"] = (_, set) => EnsureAdvanced(set).Effect = new BlurEffect { Radius = 8 },
        ["blur-lg"] = (_, set) => EnsureAdvanced(set).Effect = new BlurEffect { Radius = 16 },
        ["blur-xl"] = (_, set) => EnsureAdvanced(set).Effect = new BlurEffect { Radius = 32 },
        ["blur-none"] = (_, set) => set.Advanced?.Effect = null,

        // 阴影
        ["shadow-sm"] = (_, set) => ApplyShadow(set, 2, new Vector(0, 1)),
        ["shadow"] = (_, set) => ApplyShadow(set, 4, new Vector(0, 2)),
        ["shadow-md"] = (_, set) => ApplyShadow(set, 6, new Vector(0, 3)),
        ["shadow-lg"] = (_, set) => ApplyShadow(set, 10, new Vector(0, 6)),
        ["shadow-xl"] =(_, set) => ApplyShadow(set, 18, new Vector(0, 10)),
        ["shadow-none"] = (_, set) => set.Advanced?.BoxShadows = null,

        // 光标
        ["cursor-pointer"] = (_, set) => EnsureAdvanced(set).Cursor = new Cursor(StandardCursorType.Hand),
        ["cursor-default"] = (_, set) => EnsureAdvanced(set).Cursor = new Cursor(StandardCursorType.Arrow),

        // 过渡
        ["transition-none"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.None,
        ["transition-all"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.All,
        ["transition-opacity"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.Opacity,
        ["transition-transform"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.Transform,
        ["transition-colors"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.Colors,
        ["transition-shadow"] = (_, set) => EnsureAdvanced(set).TransitionScope = TransitionScope.Shadow,
        ["duration-"] = ApplyDuration,
        ["linear"] = (_, set) => EnsureAdvanced(set).Easing = new LinearEasing(),
        ["ease"] = (_, set) => EnsureAdvanced(set).Easing = new QuadraticEaseInOut(),
        ["ease-in"] = (_, set) => EnsureAdvanced(set).Easing = new QuadraticEaseIn(),
        ["ease-out"] = (_, set) => EnsureAdvanced(set).Easing = new QuadraticEaseOut(),
        ["ease-in-out"] = (_, set) => EnsureAdvanced(set).Easing = new QuadraticEaseInOut(),

        // 几何变换
        ["origin-top-left"] = (_, set) => SetRelativePoint(0, 0, set),
        ["origin-left"] = (_, set) => SetRelativePoint(0, 0.5, set),
        ["origin-bottom-left"] = (_, set) => SetRelativePoint(0, 1, set),
        ["origin-top"] = (_, set) => SetRelativePoint(0.5, 0, set),
        ["origin-center"] = (_, set) => SetRelativePoint(0.5, 0.5, set),
        ["origin-bottom"] = (_, set) => SetRelativePoint(0.5, 1, set),
        ["origin-top-right"] = (_, set) => SetRelativePoint(1, 0, set),
        ["origin-right"] = (_, set) => SetRelativePoint(1, 0.5, set),
        ["origin-bottom-right"] = (_, set) => SetRelativePoint(1, 1, set),

        ["translate-"] = ApplyTranslate,
        ["translate-x-"] = ApplyTranslateX,
        ["translate-y-"] = ApplyTranslateY,
        ["scale-"] = ApplyScale,
        ["scale-x-"] = ApplyScaleX,
        ["scale-y-"] = ApplyScaleY,
        ["rotate-"] = ApplyRotate,
        ["skew-"] = ApplySkew,
        ["skew-x-"] = ApplySkewX,
        ["skew-y-"] = ApplySkewY
    };

    // 样式缓存
    private static readonly Dictionary<string, StyleSet> StyleCache = new(StringComparer.Ordinal);

    public static void ParsePart(string part, ref StyleSet set)
    {
        try
        {
            var p = part.Trim();
            if (AtomHandlers.TryGetValue(p, out var handler))
            {
                handler([p], set);
                return;
            }

            var items = part.Split('-');
            if (items.Length == 1) return;

            for (var i = 1; i < items.Length; i++)
            {
                var key = string.Join('-', items[..^i]) + '-';
                if (!AtomHandlers.TryGetValue(key, out handler)) continue;

                handler(items[^i..], set);
                break;
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }
    }

    public static void Parse(IEnumerable<string>? parts, ref StyleSet set)
    {
        if (parts is null) return;
        foreach (var part in parts)
        {
            ParsePart(part, ref set);
        }

        MergeAdvanced(ref set);
    }

    public static void Parse(string strStyle, ref StyleSet set)
    {
        var parts = strStyle.Split(
            [' ', '\n', '\r', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Parse(parts, ref set);
    }

    public static StyleSet Parse(string strStyle)
    {
        if (string.IsNullOrWhiteSpace(strStyle))
            return new StyleSet();

        lock (StyleCache)
        {
            if (StyleCache.TryGetValue(strStyle, out var cached))
                return cached;
        }

        var set = new StyleSet();

        Parse(strStyle, ref set);

        MergeAdvanced(ref set);

        lock (StyleCache)
        {
            StyleCache[strStyle] = set;
        }

        return set;
    }

    // 实现伪类
    public static void ParseFullStyle(string strStyle, ref FullStyle fullStyle)
    {
        var baseStyle = fullStyle.Normal;
        var variants = fullStyle.Variants;

        var parts = strStyle.Split(
            [' ', '\n', '\r', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            var variantPrefixes = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (variantPrefixes.Length == 1)
            {
                ParsePart(part, ref baseStyle);
                continue;
            }

            var value = variantPrefixes[^1];
            variantPrefixes = variantPrefixes[..^1];

            // TODO 按平台:响应式:状态:主题 排序
            Array.Sort(variantPrefixes);

            var key = string.Join(':', variantPrefixes);

            if (variants.TryGetValue(key, out var variant))
            {
                variant.Add(value);
                continue;
            }

            variants[key] = [value];
        }

        MergeAdvanced(ref baseStyle);
    }


    public static Accessor<FullStyle> ParseFull(Accessor<string> text)
    {
        var fullStyle = new FullStyle();

        return new Computed<FullStyle>(() =>
        {
            var str = text.RxValue;
            // fullStyle.Normal = new StyleSet();
            fullStyle.Normal = StyleUtil.FromDefault();
            fullStyle.Variants = [];
            ParseFullStyle(str, ref fullStyle);

            return fullStyle;
        });
    }

    public static void MergeAdvanced(ref StyleSet styles)
    {
        var advanced = styles.Advanced;
        if (advanced is null) return;

        var gradientDir = advanced.FgGradientDir;
        var fromColor = advanced.FgFromColor;
        var toColor = advanced.FgToColor;
        var foreground = GetGradientBrush(gradientDir, fromColor, toColor);
        if (foreground is not null) styles.Foreground = foreground;

        gradientDir = advanced.BgGradientDir;
        fromColor = advanced.BgFromColor;
        toColor = advanced.BgToColor;
        var background = GetGradientBrush(gradientDir, fromColor, toColor);
        if (background is not null) styles.Background = background;

        gradientDir = advanced.BorderGradientDir;
        fromColor = advanced.BorderFromColor;
        toColor = advanced.BorderToColor;
        var borderBrush = GetGradientBrush(gradientDir, fromColor, toColor);
        if (borderBrush is not null) styles.BorderBrush = borderBrush;

        var builder = TransformOperations.CreateBuilder(4);

        if (advanced.TranslateTransform is not null)
            builder.AppendTranslate(advanced.TranslateTransform.X, advanced.TranslateTransform.Y);
        if (advanced.ScaleTransform is not null)
            builder.AppendScale(advanced.ScaleTransform.ScaleX, advanced.ScaleTransform.ScaleY);
        if (advanced.RotateTransform is not null)
            builder.AppendRotate(advanced.RotateTransform.Angle);
        if (advanced.SkewTransform is not null)
            builder.AppendSkew(advanced.SkewTransform.AngleX, advanced.SkewTransform.AngleY);

        // if (advanced.MatrixTransform is not null) 
        //     builder.AppendMatrix(advanced.MatrixTransform);

        var ops = builder.Build();
        if (ops.Operations.Count > 0)
        {
            advanced.Transform = ops;
        }

        var kind = advanced.TransitionScope;
        if (kind is null) return;

        if (advanced.Transform is null)
        {
            builder.AppendScale(1, 1);
            advanced.Transform = builder.Build();
        }

        var duration = advanced.Duration;
        if (duration is null) return;

        advanced.Transition ??= new TransformOperationsTransition();
        advanced.Transition.Duration = TimeSpan.FromMilliseconds(duration.Value);

        if (advanced.Easing is null) return;
        advanced.Transition.Easing = advanced.Easing;
    }

    private static void ApplyMargin(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.Margin = new Thickness(val.Value);
    }

    private static void ApplyMarginX(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val.Value, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyMarginY(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val.Value, t.Right, val.Value);
    }

    private static void ApplyMarginLeft(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val.Value, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyMarginTop(string[] v, StyleSet set)
    {
        // var val = ToVal(v[0]);
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val.Value, t.Right, t.Bottom);
    }

    private static void ApplyMarginRight(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyMarginBottom(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, t.Right, val.Value);
    }

    private static void ApplyZIndex(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.ZIndex = (int)val.Value;
    }

    private static void ApplyWidth(string[] val, StyleSet set)
    {
        set.Width = TryParseLength(val[0]);
    }

    private static void ApplyHeight(string[] val, StyleSet set)
    {
        set.Height = TryParseLength(val[0]);
    }

    private static void ApplyMinWidth(string[] val, StyleSet set)
    {
        set.MinWidth = TryParseLength(val[0]);
    }

    private static void ApplyMaxWidth(string[] val, StyleSet set)
    {
        set.MaxWidth = TryParseLength(val[0]);
    }

    private static void ApplyMinHeight(string[] val, StyleSet set)
    {
        set.MinHeight = TryParseLength(val[0]);
    }

    private static void ApplyMaxHeight(string[] val, StyleSet set)
    {
        set.MaxHeight = TryParseLength(val[0]);
    }

    private static void ApplyGap(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.RowSpacing = val.Value;
        set.ColumnSpacing = val.Value;
    }

    private static void ApplyGapX(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.ColumnSpacing = val.Value;
    }

    private static void ApplyGapY(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.RowSpacing = val.Value;
    }

    private static void ApplyPadding(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.Padding = new Thickness(val.Value);
    }

    private static void ApplyPaddingX(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val.Value, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyPaddingY(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val.Value, t.Right, val.Value);
    }

    private static void ApplyPaddingLeft(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val.Value, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyPaddingTop(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val.Value, t.Right, t.Bottom);
    }

    private static void ApplyPaddingRight(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, val.Value, t.Bottom);
    }


    private static void ApplyPaddingBottom(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, t.Right, val.Value);
    }

    private static void ApplyText(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.FontSize = val.Value;
    }

    private static void ApplyForeground(string[] color, StyleSet set)
    {
        var c = ParseColor(color);
        if (c is null) return;
        set.Foreground = new SolidColorBrush(c.Value);
    }

    private static void ApplyBackground(string[] color, StyleSet set)
    {
        var c = ParseColor(color);
        if (c is null) return;
        set.Background = new SolidColorBrush(c.Value);
    }

    private static void ApplyBorderBrush(string[] color, StyleSet set)
    {
        var c = ParseColor(color);
        if (c is null) return;
        set.BorderBrush = new SolidColorBrush(c.Value);
    }

    private static void ApplyBorderWidth(string[] widthStr, StyleSet set)
    {
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        set.BorderThickness = new Thickness(val.Value);
    }

    private static void ApplyBorderLeftWidth(string[] widthStr, StyleSet set)
    {
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        val *= UnitScale;
        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(w, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyBorderTopWidth(string[] widthStr, StyleSet set)
    {
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        val *= UnitScale;
        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, w, t.Right, t.Bottom);
    }

    private static void ApplyBorderRightWidth(string[] widthStr, StyleSet set)
    {
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        val *= UnitScale;
        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, w, t.Bottom);
    }

    private static void ApplyBorderBottomWidth(string[] widthStr, StyleSet set)
    {
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        val *= UnitScale;
        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, t.Right, w);
    }

    private static void ApplyCornerRadius(string[] v, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        set.CornerRadius = new CornerRadius(val.Value);
    }

    private static void ApplyOpacity(string[] val, StyleSet set)
    {
        if (double.TryParse(val[0], out var op))
            set.Opacity = op / 100.0; // opacity-50 -> 0.5
        else if (val[0] == "0") set.Opacity = 0;
        else if (val[0] == "100") set.Opacity = 1;
    }

    private static void ApplyShadow(StyleSet set, double blurRadius, Vector offset)
    {
        var advanced = EnsureAdvanced(set);

        advanced.BoxShadows ??= [];
        advanced.BoxShadows.Add(new BoxShadow
        {
            Blur = blurRadius,
            OffsetX = offset.X,
            OffsetY = offset.Y,
            Color = Color.Parse("#40000000")
        });
    }

    private static AdvancedStyle EnsureAdvanced(StyleSet set)
    {
        set.Advanced ??= new AdvancedStyle();
        return set.Advanced;
    }

    private static void ApplyDuration(string[] vals, StyleSet set)
    {
        if (vals.Length != 1) return;
        var val = TryParseValue(vals[0]);
        if (val is null) return;

        EnsureAdvanced(set).Duration = val.Value;
    }

    private static void SetRelativePoint(double x, double y, StyleSet set)
    {
        var advanced = EnsureAdvanced(set);
        advanced.RelativePoint = new RelativePoint(x, y, RelativeUnit.Relative);
    }

    private static void ApplyTranslate(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;

        translate *= UnitScale;
        EnsureAdvanced(set).TranslateTransform = new TranslateTransform { X = translate.Value, Y = translate.Value };
    }

    private static void ApplyTranslateX(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;

        translate *= UnitScale;
        var advanced = EnsureAdvanced(set);
        var translateTransform = advanced.TranslateTransform ?? new TranslateTransform();
        translateTransform.X = translate.Value;
        advanced.TranslateTransform = translateTransform;
    }

    private static void ApplyTranslateY(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;

        translate *= UnitScale;
        var advanced = EnsureAdvanced(set);
        var translateTransform = advanced.TranslateTransform ?? new TranslateTransform();
        translateTransform.Y = translate.Value;
        advanced.TranslateTransform = translateTransform;
    }

    private static void ApplyScale(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        EnsureAdvanced(set).ScaleTransform = new ScaleTransform { ScaleX = scale.Value, ScaleY = scale.Value };
    }

    private static void ApplyScaleX(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        var advanced = EnsureAdvanced(set);
        var scaleTransform = advanced.ScaleTransform ?? new ScaleTransform();
        scaleTransform.ScaleX = scale.Value;
        advanced.ScaleTransform = scaleTransform;
    }

    private static void ApplyScaleY(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        var advanced = EnsureAdvanced(set);
        var scaleTransform = advanced.ScaleTransform ?? new ScaleTransform();
        scaleTransform.ScaleY = scale.Value;
        advanced.ScaleTransform = scaleTransform;
    }

    private static void ApplyRotate(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var rotate = TryParseValue(val);
        if (rotate is null) return;

        EnsureAdvanced(set).RotateTransform = new RotateTransform { Angle = rotate.Value };
    }

    private static void ApplySkew(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        EnsureAdvanced(set).SkewTransform = new SkewTransform { AngleX = skew.Value, AngleY = skew.Value };
    }

    private static void ApplySkewX(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        var advanced = EnsureAdvanced(set);
        var scaleTransform = advanced.SkewTransform ?? new SkewTransform();
        scaleTransform.AngleX = skew.Value;
        advanced.SkewTransform = scaleTransform;
    }

    private static void ApplySkewY(string[] vals, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        var advanced = EnsureAdvanced(set);
        var scaleTransform = advanced.SkewTransform ?? new SkewTransform();
        scaleTransform.AngleY = skew.Value;
        advanced.SkewTransform = scaleTransform;
    }

    private static double? TryParseLength(string s)
    {
        if (s is "auto" or "full") return double.NaN;
        if (s.EndsWith("px") && double.TryParse(s[..^2], out var px)) return px;
        if (double.TryParse(s, out var val)) return val * UnitScale;
        return null;
    }

    private static readonly Dictionary<string, double> ValueResultCache = new();

    private static double? TryParseValue(string val)
    {
        if (ValueResultCache.TryGetValue(val, out var cacheValue)) return cacheValue;

        var cacheKey = new string(val);
        if (val.StartsWith('['))
        {
            if (!val.EndsWith(']')) return null;
            val = val[1..^1];
        }

        if (!double.TryParse(val, out var result)) return null;
        ValueResultCache[cacheKey] = result;
        return result;
    }

    private static double? TryGetDir(string[] vals)
    {
        if (vals.Length != 1) return null;
        return vals[0] switch
        {
            "r" => 180,
            "l" => 0,
            "t" => 90,
            "b" => 270,
            "tr" => 135,
            "tl" => 45,
            "br" => 225,
            "bl" => 315,
            _ => TryParseValue(vals[0])
        };
    }

    private static readonly Dictionary<string, Color> ColorResultCache = new();

    private static Color? ParseColor(string[] colors)
    {
        try
        {
            if (colors.Length > 2) return null;

            var cacheKey = string.Join("-", colors);
            if (ColorResultCache.TryGetValue(cacheKey, out var cacheValue)) return cacheValue;

            var values = colors[^1].Split('/'); // 500/50
            if (values.Length > 2) return null;

            // [#8a2be2]/20
            Color result;
            var resultAssign = false;

            byte a = 100;
            if (values.Length == 2)
            {
                a = byte.Parse(values[1]);
                if (a > 100) return null;
            }

            var color = colors.Length == 1 ? values[0] : colors[0]; // blue or [#...]

            if (Colors.ColorDict.TryGetValue(color, out result))
            {
                result = Color.FromArgb((byte)(255 * a / 100), result.R, result.G, result.B);
                resultAssign = true;
            }

            // 可能是任意值
            else if (color.StartsWith("[#") && color.EndsWith(']'))
            {
                color = color[1..^1];
                if (!Color.TryParse(color, out result)) return null;
                result = Color.FromArgb((byte)(255 * a / 100), result.R, result.G, result.B);
                resultAssign = true;
                if (colors.Length == 2) return null; // 不允许诸如 [#8a2be2]-300 等
            }

            if (colors.Length == 1)
            {
                ColorResultCache[cacheKey] = result;
                return resultAssign ? result : null;
            }

            var key = $"{color}-{values[0]}";
            if (!Colors.ColorDict.TryGetValue(key, out result)) return null;

            result = Color.FromArgb((byte)(255 * a / 100), result.R, result.G, result.B);
            ColorResultCache[cacheKey] = result;
            return result;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
            return null;
        }
    }

    private static readonly Dictionary<double, (RelativePoint Start, RelativePoint End)> GradientPointsResultCache =
        new();

    private static IBrush? GetGradientBrush(double? gradientDir, Color? fromColor, Color? toColor)
    {
        if (gradientDir is null || fromColor is null || toColor is null) return null;

        var (start, end) = GetGradientPoints(gradientDir.Value);

        return new LinearGradientBrush
        {
            StartPoint = start,
            EndPoint = end,
            GradientStops =
            {
                new GradientStop(fromColor.Value, 0.0f),
                new GradientStop(toColor.Value, 1.0f)
            }
        };
    }

    private static (RelativePoint Start, RelativePoint End) GetGradientPoints(double angle)
    {
        if (GradientPointsResultCache.TryGetValue(angle, out var cacheValue)) return cacheValue;
        // 标准渐变数学计算：角度 → 方向向量
        var radians = angle * Math.PI / 180.0;
        var cos = Math.Cos(radians);
        var sin = Math.Sin(radians);

        // 中心点 (0.5, 0.5) 发射渐变（标准模式）
        var startX = 0.5 + cos * 0.5;
        var startY = 0.5 + sin * 0.5;
        var endX = 0.5 - cos * 0.5;
        var endY = 0.5 - sin * 0.5;

        var result = (
            new RelativePoint(startX, startY, RelativeUnit.Relative),
            new RelativePoint(endX, endY, RelativeUnit.Relative)
        );

        GradientPointsResultCache[angle] = result;

        return result;
    }
}