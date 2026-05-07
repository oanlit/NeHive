using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;

namespace NeHive.Sample.Avalonia.Render;

public class StyleSet
{
    public HorizontalAlignment? HorizontalAlignment;
    public VerticalAlignment? VerticalAlignment;
    public Orientation? Orientation;
    public double? RowSpacing;
    public double? ColumnSpacing;
    public Thickness? Margin;
    public Thickness? Padding;
    public IBrush? Background;
    public IBrush? Foreground;
    public double? FontSize;
    public FontWeight? FontWeight;
    public FontStyle? FontStyle;
    public TextAlignment? TextAlignment;
    public VerticalAlignment? VerticalTextAlignment;
    public TextWrapping? TextWrapping;
    public CornerRadius? CornerRadius;
}

public static class StyleParser
{
    // 1单位 = 4px（Tailwind 标准）
    private const double UnitScale = 4;

    // 原子类字典：前缀 → 处理逻辑
    private static readonly Dictionary<string, Action<string, StyleSet>> AtomHandlers = new()
    {
        ["center"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Center,
        ["left"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Left,
        ["right"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Right,
        ["stretch"] = (_, set) => set.HorizontalAlignment = HorizontalAlignment.Stretch,

        ["flex-row"] = (_, set) => set.Orientation = Orientation.Horizontal,
        ["flex-col"] = (_, set) => set.Orientation = Orientation.Vertical,
        ["horizontal"] = (_, set) => set.Orientation = Orientation.Horizontal,
        ["vertical"] = (_, set) => set.Orientation = Orientation.Vertical,

        ["top"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Top,
        ["bottom"] = (_, set) => set.VerticalAlignment = VerticalAlignment.Bottom,

        ["m-"] = ApplyMargin,
        ["mx-"] = ApplyMarginX,
        ["my-"] = ApplyMarginY,
        ["mt-"] = ApplyMarginTop,
        ["mb-"] = ApplyMarginBottom,
        ["ml-"] = ApplyMarginLeft,
        ["mr-"] = ApplyMarginRight,

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

        ["bg-"] = ApplyBackground,
        ["fg-"] = ApplyForeground,

        ["text-"] = ApplyFontSize,
        ["font-bold"] = (_, set) => set.FontWeight = FontWeight.Bold,
        ["font-normal"] = (_, set) => set.FontWeight = FontWeight.Normal,

        ["italic"] = (_, set) => set.FontStyle = FontStyle.Italic,
        ["not-italic"] = (_, set) => set.FontStyle = FontStyle.Normal,

        // 文本对齐
        ["text-center"] = (_, set) => set.TextAlignment = TextAlignment.Center,
        ["text-left"] = (_, set) => set.TextAlignment = TextAlignment.Left,
        ["text-right"] = (_, set) => set.TextAlignment = TextAlignment.Right,

        // 垂直对齐
        ["text-top"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Top,
        ["text-middle"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Center,
        ["text-bottom"] = (_, set) => set.VerticalTextAlignment = VerticalAlignment.Bottom,

        ["wrap"] = (_, set) => set.TextWrapping = TextWrapping.Wrap,
        ["whitespace-nowrap"] = (_, set) => set.TextWrapping = TextWrapping.NoWrap,
        ["wrap-overflow"] = (_, set) => set.TextWrapping = TextWrapping.WrapWithOverflow,

        ["rounded-"] = ApplyCornerRadius,
        ["rounded"] = (_, set) => set.CornerRadius = new CornerRadius(4),
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
                foreach (var (prefix, handler) in AtomHandlers)
                {
                    if (p.StartsWith(prefix))
                    {
                        handler(p[prefix.Length..], set);
                        break;
                    }
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

    private static void ApplyMargin(string v, StyleSet set)
    {
        var val = ToVal(v);
        set.Margin = new Thickness(val);
    }

    private static void ApplyMarginX(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val, t.Top, val, t.Bottom);
    }

    private static void ApplyMarginY(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val, t.Right, val);
    }

    private static void ApplyMarginLeft(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyMarginTop(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val, t.Right, t.Bottom);
    }

    private static void ApplyMarginRight(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, val, t.Bottom);
    }

    private static void ApplyMarginBottom(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, t.Right, val);
    }

    private static void ApplyPadding(string v, StyleSet set)
    {
        var val = ToVal(v);
        set.Padding = new Thickness(val);
    }

    private static void ApplyPaddingX(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val, t.Top, val, t.Bottom);
    }

    private static void ApplyPaddingY(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val, t.Right, val);
    }

    private static void ApplyPaddingLeft(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyPaddingTop(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val, t.Right, t.Bottom);
    }

    private static void ApplyPaddingRight(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, val, t.Bottom);
    }


    private static void ApplyPaddingBottom(string v, StyleSet set)
    {
        var val = ToVal(v);
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, t.Right, val);
    }

    private static void ApplyBackground(string color, StyleSet set)
    {
        try
        {
            set.Background = new SolidColorBrush(Color.Parse(color));
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }
    }

    private static void ApplyForeground(string color, StyleSet set)
    {
        try
        {
            set.Foreground = new SolidColorBrush(Color.Parse(color));
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }
    }

    private static void ApplyGap(string v, StyleSet set)
    {
        var val = ToVal(v);
        set.RowSpacing = val;
        set.ColumnSpacing = val;
    }

    private static void ApplyGapX(string v, StyleSet set)
    {
        set.ColumnSpacing = ToVal(v);
    }

    private static void ApplyGapY(string v, StyleSet set)
    {
        set.RowSpacing = ToVal(v);
    }

    private static void ApplyFontSize(string v, StyleSet set)
    {
        set.FontSize = v switch
        {
            "xs" => 12,
            "sm" => 14,
            "base" => 16,
            "lg" => 18,
            "xl" => 20,
            "2xl" => 24,
            "3xl" => 30,
            _ => ToVal(v)
        };
    }

    private static void ApplyCornerRadius(string v, StyleSet set)
    {
        var r = v switch
        {
            "sm" => 2,
            "lg" => 8,
            "xl" => 12,
            "2xl" => 16,
            "full" => 9999,
            _ => ToVal(v)
        };
        set.CornerRadius = new CornerRadius(r);
    }

    private static double ToVal(string v)
    {
        return double.TryParse(v, out var num) ? num * UnitScale : 0;
    }
}