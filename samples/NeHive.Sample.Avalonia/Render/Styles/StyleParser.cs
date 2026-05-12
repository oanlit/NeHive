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

    public StyleSet Merge(StyleSet other)
    {
        return new StyleSet
        {
            Margin = other.Margin ?? Margin,

            Width = other.Width ?? Width,
            Height = other.Height ?? Height,
            MinWidth = other.MinWidth ?? MinWidth,
            MaxWidth = other.MaxWidth ?? MaxWidth,
            MinHeight = other.MinHeight ?? MinHeight,
            MaxHeight = other.MaxHeight ?? MaxHeight,

            Padding = other.Padding ?? Padding,
            RowSpacing = other.RowSpacing ?? RowSpacing,
            ColumnSpacing = other.ColumnSpacing ?? ColumnSpacing,

            Orientation = other.Orientation ?? Orientation,
            HorizontalAlignment = other.HorizontalAlignment ?? HorizontalAlignment,
            VerticalAlignment = other.VerticalAlignment ?? VerticalAlignment,

            TextAlignment = other.TextAlignment ?? TextAlignment,
            VerticalTextAlignment = other.VerticalTextAlignment ?? VerticalTextAlignment,
            TextWrapping = other.TextWrapping ?? TextWrapping,

            FontSize = other.FontSize ?? FontSize,
            FontWeight = other.FontWeight ?? FontWeight,
            FontStyle = other.FontStyle ?? FontStyle,
            Foreground = other.Foreground ?? Foreground,

            Background = other.Background ?? Background,
            BorderBrush = other.BorderBrush ?? BorderBrush,
            BorderThickness = other.BorderThickness ?? BorderThickness,
            CornerRadius = other.CornerRadius ?? CornerRadius,

            Opacity = other.Opacity ?? Opacity,
            IsVisible = other.IsVisible ?? IsVisible,
            Effect = other.Effect ?? Effect,
            Cursor = other.Cursor ?? Cursor,
            FlowDirection = other.FlowDirection ?? FlowDirection
        };
    }
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
        var parts = strStyle.Split(
            [' ', '\n', '\r', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            try
            {
                var p = part.Trim();
                if (AtomHandlers.TryGetValue(p, out var handler))
                {
                    handler([p], set);
                    continue;
                }

                var items = part.Split('-');
                if (items.Length == 1) continue;

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

        lock (StyleCache)
        {
            StyleCache[strStyle] = set;
        }

        return set;
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

    private static Color? ParseColor(string[] colors)
    {
        try
        {
            if (colors.Length > 2) return null;
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

            if (colors.Length == 1) return resultAssign ? result : null;

            var key = $"{color}-{values[0]}";
            if (!Colors.ColorDict.TryGetValue(key, out result)) return null;
            //
            // if (values.Length == 1) return result;
            // if (values.Length != 2) return null;
            //
            // a = byte.Parse(values[1]);
            // if (a > 100) return null;
            return Color.FromArgb((byte)(255 * a / 100), result.R, result.G, result.B);
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