using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Input;

namespace NeHive.Sample.Avalonia.Render.Styles;

// 定位 → 尺寸 → 内间距 → 布局排版 → 文字 → 颜色 → 边框 → 特效
public class StyleSet
{
    public Thickness? Margin;

    public double? Width;
    public double? Height;
    public double? MinWidth;
    public double? MaxWidth;
    public double? MinHeight;
    public double? MaxHeight;

    public Thickness? Padding;
    public double? RowSpacing;
    public double? ColumnSpacing;

    public Orientation? Orientation;
    public HorizontalAlignment? HorizontalAlignment;
    public VerticalAlignment? VerticalAlignment;

    public TextAlignment? TextAlignment;
    public VerticalAlignment? VerticalTextAlignment;
    public TextWrapping? TextWrapping;

    public double? FontSize;
    public FontWeight? FontWeight;
    public FontStyle? FontStyle;
    public IBrush? Foreground;

    public IBrush? Background;
    public IBrush? BorderBrush;
    public Thickness? BorderThickness;
    public CornerRadius? CornerRadius;

    public double? Opacity;
    public bool? IsVisible;
    public IEffect? Effect; // 阴影等效果
    public Cursor? Cursor;
    public FlowDirection? FlowDirection;

    public static StyleSet Copy(StyleSet other)
    {
        var result = new StyleSet();
        Copy(ref result, other);
        return result;
    }

    public static void Copy(ref StyleSet target, StyleSet source)
    {
        target.Margin = source.Margin;

        target.Width = source.Width;
        target.Height = source.Height;
        target.MinWidth = source.MinWidth;
        target.MaxWidth = source.MaxWidth;
        target.MinHeight = source.MinHeight;
        target.MaxHeight = source.MaxHeight;

        target.Padding = source.Padding;
        target.RowSpacing = source.RowSpacing;
        target.ColumnSpacing = source.ColumnSpacing;

        target.Orientation = source.Orientation;
        target.HorizontalAlignment = source.HorizontalAlignment;
        target.VerticalAlignment = source.VerticalAlignment;

        target.TextAlignment = source.TextAlignment;
        target.VerticalTextAlignment = source.VerticalTextAlignment;
        target.TextWrapping = source.TextWrapping;

        target.FontSize = source.FontSize;
        target.FontWeight = source.FontWeight;
        target.FontStyle = source.FontStyle;
        target.Foreground = source.Foreground;

        target.Background = source.Background;
        target.BorderBrush = source.BorderBrush;
        target.BorderThickness = source.BorderThickness;
        target.CornerRadius = source.CornerRadius;

        target.Opacity = source.Opacity;
        target.IsVisible = source.IsVisible;
        target.Effect = source.Effect;
        target.Cursor = source.Cursor;
        target.FlowDirection = source.FlowDirection;
    }

    public void Merge(StyleSet other, bool mergeNull = false)
    {
        if (mergeNull)
        {
            Margin = other.Margin;

            Width = other.Width;
            Height = other.Height;
            MinWidth = other.MinWidth;
            MaxWidth = other.MaxWidth;
            MinHeight = other.MinHeight;
            MaxHeight = other.MaxHeight;

            Padding = other.Padding;
            RowSpacing = other.RowSpacing;
            ColumnSpacing = other.ColumnSpacing;

            Orientation = other.Orientation;
            HorizontalAlignment = other.HorizontalAlignment;
            VerticalAlignment = other.VerticalAlignment;

            TextAlignment = other.TextAlignment;
            VerticalTextAlignment = other.VerticalTextAlignment;
            TextWrapping = other.TextWrapping;

            FontSize = other.FontSize;
            FontWeight = other.FontWeight;
            FontStyle = other.FontStyle;
            Foreground = other.Foreground;

            Background = other.Background;
            BorderBrush = other.BorderBrush;
            BorderThickness = other.BorderThickness;
            CornerRadius = other.CornerRadius;

            Opacity = other.Opacity;
            IsVisible = other.IsVisible;
            Effect = other.Effect;
            Cursor = other.Cursor;
            FlowDirection = other.FlowDirection;

            return;
        }

        if (other.Margin is not null) Margin = other.Margin;

        if (other.Width is not null) Width = other.Width;
        if (other.Height is not null) Height = other.Height;
        if (other.MinWidth is not null) MinWidth = other.MinWidth;
        if (other.MaxWidth is not null) MaxWidth = other.MaxWidth;
        if (other.MinHeight is not null) MinHeight = other.MinHeight;
        if (other.MaxHeight is not null) MaxHeight = other.MaxHeight;

        if (other.Padding is not null) Padding = other.Padding;
        if (other.RowSpacing is not null) RowSpacing = other.RowSpacing;
        if (other.ColumnSpacing is not null) ColumnSpacing = other.ColumnSpacing;

        if (other.Orientation is not null) Orientation = other.Orientation;
        if (other.HorizontalAlignment is not null) HorizontalAlignment = other.HorizontalAlignment;
        if (other.VerticalAlignment is not null) VerticalAlignment = other.VerticalAlignment;

        if (other.TextAlignment is not null) TextAlignment = other.TextAlignment;
        if (other.VerticalTextAlignment is not null) VerticalTextAlignment = other.VerticalTextAlignment;
        if (other.TextWrapping is not null) TextWrapping = other.TextWrapping;

        if (other.FontSize is not null) FontSize = other.FontSize;
        if (other.FontWeight is not null) FontWeight = other.FontWeight;
        if (other.FontStyle is not null) FontStyle = other.FontStyle;
        if (other.Foreground is not null) Foreground = other.Foreground;

        if (other.Background is not null) Background = other.Background;
        if (other.BorderBrush is not null) BorderBrush = other.BorderBrush;
        if (other.BorderThickness is not null) BorderThickness = other.BorderThickness;
        if (other.CornerRadius is not null) CornerRadius = other.CornerRadius;

        if (other.Opacity is not null) Opacity = other.Opacity;
        if (other.IsVisible is not null) IsVisible = other.IsVisible;
        if (other.Effect is not null) Effect = other.Effect;
        if (other.Cursor is not null) Cursor = other.Cursor;
        if (other.FlowDirection is not null) FlowDirection = other.FlowDirection;
    }

    public void MergeMany(bool mergeNull, params StyleSet[] styles)
    {
        foreach (var style in styles)
        {
            Merge(style, mergeNull);
        }
    }
}

public struct FullStyle
{
    public StyleSet Base;

    public Dictionary<string, List<string>> Variants;
}

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

        // 尺寸
        ["w-"] = ApplyWidth,
        ["h-"] = ApplyHeight,
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
        ["fg-"] = ApplyForeground,
        ["text-"] = ApplyText,
        ["font-bold"] = (_, set) => set.FontWeight = FontWeight.Bold,
        ["font-normal"] = (_, set) => set.FontWeight = FontWeight.Normal,
        ["italic"] = (_, set) => set.FontStyle = FontStyle.Italic,
        ["not-italic"] = (_, set) => set.FontStyle = FontStyle.Normal,

        // 背景边框
        ["bg-"] = ApplyBackground,
        ["border-"] = ApplyBorderBrush,
        ["border-w-"] = ApplyBorderWidth, // 如 border-w-2
        ["border-t-"] = ApplyBorderTopWidth,
        ["border-r-"] = ApplyBorderRightWidth,
        ["border-b-"] = ApplyBorderBottomWidth,
        ["border-l-"] = ApplyBorderLeftWidth,
        ["rounded"] = (_, set) => set.CornerRadius = new CornerRadius(4),
        ["rounded-"] = ApplyCornerRadius,

        // 透明度 & 可见性
        ["opacity-"] = ApplyOpacity,
        ["visible"] = (_, set) => set.IsVisible = true,
        ["hidden"] = (_, set) => set.IsVisible = false,

        // 阴影
        ["shadow"] = ApplyShadow, // 默认 md
        ["shadow-sm"] = ApplyShadow,
        ["shadow-lg"] = ApplyShadow,
        ["shadow-xl"] = ApplyShadow,
        ["shadow-none"] = (_, set) => set.Effect = null,

        // 光标
        ["cursor-pointer"] = (_, set) => set.Cursor = new Cursor(StandardCursorType.Hand),
        ["cursor-default"] = (_, set) => set.Cursor = new Cursor(StandardCursorType.Arrow),
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

        lock (StyleCache)
        {
            StyleCache[strStyle] = set;
        }

        return set;
    }

    // 实现伪类
    public static void ParseFullStyle(string strStyle, ref FullStyle fullStyle)
    {
        var baseStyle = fullStyle.Base;
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
    }

    private static void ApplyMargin(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        set.Margin = new Thickness(val);
    }

    private static void ApplyMarginX(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val, t.Top, val, t.Bottom);
    }

    private static void ApplyMarginY(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val, t.Right, val);
    }

    private static void ApplyMarginLeft(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyMarginTop(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val, t.Right, t.Bottom);
    }

    private static void ApplyMarginRight(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, val, t.Bottom);
    }

    private static void ApplyMarginBottom(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, t.Right, val);
    }

    private static void ApplyPadding(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        set.Padding = new Thickness(val);
    }

    private static void ApplyPaddingX(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val, t.Top, val, t.Bottom);
    }

    private static void ApplyPaddingY(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val, t.Right, val);
    }

    private static void ApplyPaddingLeft(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyPaddingTop(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val, t.Right, t.Bottom);
    }

    private static void ApplyPaddingRight(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, val, t.Bottom);
    }


    private static void ApplyPaddingBottom(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, t.Right, val);
    }

    private static void ApplyBackground(string[] color, StyleSet set)
    {
        var c = ParseColor(color);
        if (c is null) return;
        set.Background = new SolidColorBrush(c.Value);
    }

    private static void ApplyForeground(string[] color, StyleSet set)
    {
        try
        {
            var c = ParseColor(color);
            if (c is null) return;
            set.Foreground = new SolidColorBrush(c.Value);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }
    }

    private static void ApplyGap(string[] v, StyleSet set)
    {
        var val = ToVal(v[0]);
        set.RowSpacing = val;
        set.ColumnSpacing = val;
    }

    private static void ApplyGapX(string[] v, StyleSet set)
    {
        set.ColumnSpacing = ToVal(v[0]);
    }

    private static void ApplyGapY(string[] v, StyleSet set)
    {
        set.RowSpacing = ToVal(v[0]);
    }

    private static void ApplyText(string[] v, StyleSet set)
    {
        set.FontSize = v[0] switch
        {
            "xs" => 12,
            "sm" => 14,
            "base" => 16,
            "lg" => 18,
            "xl" => 20,
            "2xl" => 24,
            "3xl" => 30,
            _ => ToVal(v[0])
        };
    }

    private static void ApplyCornerRadius(string[] v, StyleSet set)
    {
        var r = v[0] switch
        {
            "sm" => 2,
            "lg" => 8,
            "xl" => 12,
            "2xl" => 16,
            "full" => 9999,
            _ => ToVal(v[0])
        };
        set.CornerRadius = new CornerRadius(r);
    }

    private static void ApplyBorderBrush(string[] color, StyleSet set)
    {
        var c = ParseColor(color);
        if (c is null) return;
        set.BorderBrush = new SolidColorBrush(c.Value);
    }

    private static void ApplyBorderWidth(string[] widthStr, StyleSet set)
    {
        var w = ToVal(widthStr[0]);
        set.BorderThickness = new Thickness(w);
    }

    private static void ApplyBorderLeftWidth(string[] widthStr, StyleSet set)
    {
        var w = ToVal(widthStr[0]);
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(w, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyBorderTopWidth(string[] widthStr, StyleSet set)
    {
        var w = ToVal(widthStr[0]);
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, w, t.Right, t.Bottom);
    }

    private static void ApplyBorderRightWidth(string[] widthStr, StyleSet set)
    {
        var w = ToVal(widthStr[0]);
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, w, t.Bottom);
    }

    private static void ApplyBorderBottomWidth(string[] widthStr, StyleSet set)
    {
        var w = ToVal(widthStr[0]);
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, t.Right, w);
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

    private static void ApplyOpacity(string[] val, StyleSet set)
    {
        if (double.TryParse(val[0], out var op))
            set.Opacity = op / 100.0; // opacity-50 -> 0.5
        else if (val[0] == "0") set.Opacity = 0;
        else if (val[0] == "100") set.Opacity = 1;
    }

    private static void ApplyShadow(string[] level, StyleSet set)
    {
        var blur = level[0] switch
        {
            "sm" => 2,
            "lg" => 8,
            "xl" => 12,
            _ => 4 // 默认 shadow 或 shadow-md
        };
        set.Effect = new DropShadowEffect { BlurRadius = blur, Opacity = 0.3 };
    }

    private static double ToVal(string v)
    {
        return double.TryParse(v, out var num) ? num * UnitScale : 0;
    }

    private static double? TryParseLength(string s)
    {
        if (s is "auto" or "full") return double.NaN;
        if (s.EndsWith("px") && double.TryParse(s[..^2], out var px)) return px;
        if (double.TryParse(s, out var val)) return val * UnitScale;
        return null;
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
}