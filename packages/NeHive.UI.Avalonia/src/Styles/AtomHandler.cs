using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Animation.Easings;

namespace NeHive.UI.Avalonia.Styles;

public class AtomHandler
{
    // 1单位 = 4px（Tailwind 标准）
    private const double UnitScale = 4;

    // 原子类字典：前缀 → 处理逻辑
    internal static readonly Dictionary<string, Action<string[], bool, StyleSet>> DefaultHandlers = new()
    {
        ["m-"] = ApplyMargin,
        ["mx-"] = ApplyMarginX,
        ["my-"] = ApplyMarginY,
        ["mt-"] = ApplyMarginTop,
        ["mb-"] = ApplyMarginBottom,
        ["ml-"] = ApplyMarginLeft,
        ["mr-"] = ApplyMarginRight,
        ["z-"] = ApplyZIndex,
        ["mx-auto"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Center,
        ["my-auto"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Center,

        // 尺寸
        ["w-"] = ApplyWidth,
        ["w-full"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Stretch,
        ["h-"] = ApplyHeight,
        ["h-full"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Stretch,
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

        ["overflow-visible"] = (_, _, set) => set.ClipToBounds = false,
        ["overflow-hidden"] = (_, _, set) => set.ClipToBounds = true,

        ["flex-row"] = (_, _, set) => set.Orientation = Orientation.Horizontal,
        ["flex-col"] = (_, _, set) => set.Orientation = Orientation.Vertical,
        ["horizontal"] = (_, _, set) => set.Orientation = Orientation.Horizontal,
        ["vertical"] = (_, _, set) => set.Orientation = Orientation.Vertical,

        ["start"] = (_, _, set) =>
        {
            set.HorizontalAlignment = HorizontalAlignment.Left;
            set.VerticalAlignment = VerticalAlignment.Top;
        },
        ["center"] = (_, _, set) =>
        {
            set.HorizontalAlignment = HorizontalAlignment.Center;
            set.VerticalAlignment = VerticalAlignment.Center;
        },
        ["end"] = (_, _, set) =>
        {
            set.HorizontalAlignment = HorizontalAlignment.Right;
            set.VerticalAlignment = VerticalAlignment.Bottom;
        },
        ["stretch"] = (_, _, set) =>
        {
            set.HorizontalAlignment = HorizontalAlignment.Stretch;
            set.VerticalAlignment = VerticalAlignment.Stretch;
        },
        ["justify-start"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Left,
        ["justify-center"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Center,
        ["justify-end"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Right,
        ["justify-stretch"] = (_, _, set) => set.HorizontalAlignment = HorizontalAlignment.Stretch,
        ["items-start"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Top,
        ["items-center"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Center,
        ["items-end"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Bottom,
        ["items-stretch"] = (_, _, set) => set.VerticalAlignment = VerticalAlignment.Stretch,

        // 行与字间距
        ["tracking-"] = ApplyLetterSpacing,
        ["leading-"] = ApplyLineHeight,

        // 文本布局与对齐
        ["text-clip-char"] = (_, _, set) => set.TextTrimming = TextTrimming.CharacterEllipsis,
        ["text-clip-start"] = (_, _, set) => set.TextTrimming = TextTrimming.LeadingCharacterEllipsis,
        ["text-clip-prefix"] = (_, _, set) => set.TextTrimming = TextTrimming.PrefixCharacterEllipsis,
        ["text-clip-path"] = (_, _, set) => set.TextTrimming = TextTrimming.PathSegmentEllipsis,
        ["text-clip-end"] = (_, _, set) => set.TextTrimming = TextTrimming.WordEllipsis,
        ["text-clip-none"] = (_, _, set) => set.TextTrimming = TextTrimming.None,
        ["truncate"] = (_, _, set) => set.TextTrimming = TextTrimming.WordEllipsis,
        ["line-clamp-"] = ApplyMaxLine,
        
        ["text-left"] = (_, _, set) => set.TextAlignment = TextAlignment.Left,
        ["text-x-center"] = (_, _, set) => set.TextAlignment = TextAlignment.Center,
        ["text-right"] = (_, _, set) => set.TextAlignment = TextAlignment.Right,
        ["text-top"] = (_, _, set) => set.VerticalTextAlignment = VerticalAlignment.Top,
        ["text-y-center"] = (_, _, set) => set.VerticalTextAlignment = VerticalAlignment.Center,
        ["text-bottom"] = (_, _, set) => set.VerticalTextAlignment = VerticalAlignment.Bottom,
        ["text-center"] = (_, _, set) =>
        {
            set.TextAlignment = TextAlignment.Center;
            set.VerticalTextAlignment = VerticalAlignment.Center;
        },

        ["wrap"] = (_, _, set) => set.TextWrapping = TextWrapping.Wrap,
        ["whitespace-nowrap"] = (_, _, set) => set.TextWrapping = TextWrapping.NoWrap,
        ["wrap-overflow"] = (_, _, set) => set.TextWrapping = TextWrapping.WrapWithOverflow,

        ["underline"] = (_, _, set) => EnsureDecoration(set).Location = TextDecorationLocation.Underline,
        ["overline"] = (_, _, set) => EnsureDecoration(set).Location = TextDecorationLocation.Overline,
        ["baseline"] = (_, _, set) => EnsureDecoration(set).Location = TextDecorationLocation.Baseline,
        ["line-through"] = (_, _, set) => EnsureDecoration(set).Location = TextDecorationLocation.Strikethrough,
        ["decoration-none"] = (_, _, set) => set.TextDecorations = null,

        ["decoration-w-"] = ApplyDecorationWidth,
        ["decoration-"] = ApplyDecorationColor,
        ["decoration-solid"] = (_, _, set) => set.TempStyle?.TextDecoration?.StrokeDashArray = null,
        ["decoration-dashed"] = (_, _, set) => EnsureDecoration(set).StrokeDashArray = [4, 2],
        ["decoration-dotted"] = (_, _, set) => EnsureDecoration(set).StrokeDashArray = [1, 2],

        // 文本样式
        ["text-"] = ApplyText,
        ["text-xs"] = (_, _, set) => set.FontSize = 12,
        ["text-sm"] = (_, _, set) => set.FontSize = 14,
        ["text-base"] = (_, _, set) => set.FontSize = 16,
        ["text-lg"] = (_, _, set) => set.FontSize = 18,
        ["text-xl"] = (_, _, set) => set.FontSize = 20,
        ["text-2xl"] = (_, _, set) => set.FontSize = 24,
        ["text-3xl"] = (_, _, set) => set.FontSize = 30,

        ["fw-thin"] = (_, _, set) => set.FontWeight = FontWeight.Thin,
        ["fw-extralight"] = (_, _, set) => set.FontWeight = FontWeight.ExtraLight,
        ["fw-light"] = (_, _, set) => set.FontWeight = FontWeight.Light,
        ["fw-normal"] = (_, _, set) => set.FontWeight = FontWeight.Normal,
        ["fw-medium"] = (_, _, set) => set.FontWeight = FontWeight.Medium,
        ["fw-semibold"] = (_, _, set) => set.FontWeight = FontWeight.SemiBold,
        ["fw-bold"] = (_, _, set) => set.FontWeight = FontWeight.Bold,
        ["fw-extrabold"] = (_, _, set) => set.FontWeight = FontWeight.ExtraBold,
        ["fw-black"] = (_, _, set) => set.FontWeight = FontWeight.Black,
        ["fw-extrablack"] = (_, _, set) => set.FontWeight = FontWeight.ExtraBlack,

        ["italic"] = (_, _, set) => set.FontStyle = FontStyle.Italic,
        ["oblique"] = (_, _, set) => set.FontStyle = FontStyle.Oblique,
        ["not-italic"] = (_, _, set) => set.FontStyle = FontStyle.Normal,

        ["font-"] = ApplyFont,

        ["fg-"] = ApplyForeground,
        ["fg-gradient-"] = (vals, _, set) => EnsureTemp(set).FgGradientDir = TryGetDir(vals),
        ["fg-from-"] = (vals, _, set) => EnsureTemp(set).FgFromColor = ParseColor(vals),
        ["fg-to-"] = (vals, _, set) => EnsureTemp(set).FgToColor = ParseColor(vals),

        // 背景
        ["bg-"] = ApplyBackground,
        ["bg-gradient-"] = (vals, _, set) => EnsureTemp(set).BgGradientDir = TryGetDir(vals),
        ["bg-from-"] = (vals, _, set) => EnsureTemp(set).BgFromColor = ParseColor(vals),
        ["bg-to-"] = (vals, _, set) => EnsureTemp(set).BgToColor = ParseColor(vals),
        ["gradient-"] = (vals, _, set) => EnsureTemp(set).BgGradientDir = TryGetDir(vals),
        ["from-"] = (vals, _, set) => EnsureTemp(set).BgFromColor = ParseColor(vals),
        ["to-"] = (vals, _, set) => EnsureTemp(set).BgToColor = ParseColor(vals),
        ["bg-center"] = (_, _, set) => set.BackgroundSizing = BackgroundSizing.CenterBorder,
        ["bg-inner"] = (_, _, set) => set.BackgroundSizing = BackgroundSizing.InnerBorderEdge,
        ["bg-outer"] = (_, _, set) => set.BackgroundSizing = BackgroundSizing.OuterBorderEdge,

        // 边框
        ["border"] = (_, _, set) => set.BorderThickness = new Thickness(1),
        ["border-"] = ApplyBorderBrush,
        ["border-gradient-"] = (vals, _, set) => EnsureTemp(set).BorderGradientDir = TryGetDir(vals),
        ["border-from-"] = (vals, _, set) => EnsureTemp(set).BorderFromColor = ParseColor(vals),
        ["border-to-"] = (vals, _, set) => EnsureTemp(set).BorderToColor = ParseColor(vals),
        ["border-w-"] = ApplyBorderWidth, // 如 border-w-2
        ["border-t-"] = ApplyBorderTopWidth,
        ["border-r-"] = ApplyBorderRightWidth,
        ["border-b-"] = ApplyBorderBottomWidth,
        ["border-l-"] = ApplyBorderLeftWidth,
        ["rounded"] = (_, _, set) => set.CornerRadius = new CornerRadius(4),
        ["rounded-"] = ApplyCornerRadius,
        ["rounded-sm"] = (_, _, set) => set.CornerRadius = new CornerRadius(2),
        ["rounded-md"] = (_, _, set) => set.CornerRadius = new CornerRadius(4),
        ["rounded-lg"] = (_, _, set) => set.CornerRadius = new CornerRadius(8),
        ["rounded-xl"] = (_, _, set) => set.CornerRadius = new CornerRadius(12),
        ["rounded-2xl"] = (_, _, set) => set.CornerRadius = new CornerRadius(16),
        ["rounded-full"] = (_, _, set) => set.CornerRadius = new CornerRadius(9999),

        // 透明度 & 可见性
        ["opacity-"] = ApplyOpacity,
        ["visible"] = (_, _, set) => set.IsVisible = true,
        ["hidden"] = (_, _, set) => set.IsVisible = false,

        // 模糊
        ["blur-sm"] = (_, _, set) => set.Effect = new BlurEffect { Radius = 4 },
        ["blur"] = (_, _, set) => set.Effect = new BlurEffect { Radius = 8 },
        ["blur-md"] = (_, _, set) => set.Effect = new BlurEffect { Radius = 8 },
        ["blur-lg"] = (_, _, set) => set.Effect = new BlurEffect { Radius = 16 },
        ["blur-xl"] = (_, _, set) => set.Effect = new BlurEffect { Radius = 32 },
        ["blur-none"] = (_, _, set) => set.Effect = null,

        // 阴影
        ["shadow-sm"] = (_, _, set) => ApplyShadow(set, 2, new Vector(0, 1)),
        ["shadow"] = (_, _, set) => ApplyShadow(set, 4, new Vector(0, 2)),
        ["shadow-md"] = (_, _, set) => ApplyShadow(set, 6, new Vector(0, 3)),
        ["shadow-lg"] = (_, _, set) => ApplyShadow(set, 10, new Vector(0, 6)),
        ["shadow-xl"] = (_, _, set) => ApplyShadow(set, 18, new Vector(0, 10)),
        ["shadow-none"] = (_, _, set) => EnsureTemp(set).HasShadow = false,

        ["ring-"] = ApplyRingColor,
        ["ring-w-"] = ApplyRingWidth,
        ["ring-offset-"] = ApplyRingOffset,

        // 光标
        ["cursor-default"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Arrow),
        ["cursor-text"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Ibeam),
        ["cursor-wait"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Wait),
        ["cursor-crosshair"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Cross),
        ["cursor-up-arrow"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.UpArrow),
        ["cursor-ew-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.SizeWestEast),
        ["cursor-ns-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.SizeNorthSouth),
        ["cursor-move"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.SizeAll),
        ["cursor-not-allowed"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.No),
        ["cursor-pointer"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Hand),
        ["cursor-progress"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.AppStarting),
        ["cursor-help"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.Help),
        ["cursor-n-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.TopSide),
        ["cursor-s-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.BottomSide),
        ["cursor-w-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.LeftSide),
        ["cursor-e-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.RightSide),
        ["cursor-nw-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.TopLeftCorner),
        ["cursor-ne-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.TopRightCorner),
        ["cursor-sw-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.BottomLeftCorner),
        ["cursor-se-resize"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.BottomRightCorner),
        ["cursor-drag-move"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.DragMove),
        ["cursor-drag-copy"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.DragCopy),
        ["cursor-drag-link"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.DragLink),
        ["cursor-none"] = (_, _, set) => set.Cursor = new Cursor(StandardCursorType.None),

        ["ltr"] = (_, _, set) => set.FlowDirection = FlowDirection.LeftToRight,
        ["rtl"] = (_, _, set) => set.FlowDirection = FlowDirection.RightToLeft,

        // 过渡
        ["transition-none"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.None,
        ["transition-all"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.All,
        ["transition-opacity"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.Opacity,
        ["transition-transform"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.Transform,
        ["transition-colors"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.Colors,
        ["transition-shadow"] = (_, _, set) => EnsureTemp(set).TransitionScope = TransitionScope.Shadow,
        ["duration-"] = ApplyDuration,
        ["linear"] = (_, _, set) => EnsureTemp(set).Easing = new LinearEasing(),
        ["ease"] = (_, _, set) => EnsureTemp(set).Easing = new QuadraticEaseInOut(),
        ["ease-in"] = (_, _, set) => EnsureTemp(set).Easing = new QuadraticEaseIn(),
        ["ease-out"] = (_, _, set) => EnsureTemp(set).Easing = new QuadraticEaseOut(),
        ["ease-in-out"] = (_, _, set) => EnsureTemp(set).Easing = new QuadraticEaseInOut(),

        // 几何变换
        ["origin-top-left"] = (_, _, set) => SetRelativePoint(0, 0, set),
        ["origin-left"] = (_, _, set) => SetRelativePoint(0, 0.5, set),
        ["origin-bottom-left"] = (_, _, set) => SetRelativePoint(0, 1, set),
        ["origin-top"] = (_, _, set) => SetRelativePoint(0.5, 0, set),
        ["origin-center"] = (_, _, set) => SetRelativePoint(0.5, 0.5, set),
        ["origin-bottom"] = (_, _, set) => SetRelativePoint(0.5, 1, set),
        ["origin-top-right"] = (_, _, set) => SetRelativePoint(1, 0, set),
        ["origin-right"] = (_, _, set) => SetRelativePoint(1, 0.5, set),
        ["origin-bottom-right"] = (_, _, set) => SetRelativePoint(1, 1, set),

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

    private static void ApplyMargin(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        set.Margin = new Thickness(val.Value);
    }

    private static void ApplyMarginX(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val.Value, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyMarginY(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val.Value, t.Right, val.Value);
    }

    private static void ApplyMarginLeft(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(val.Value, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyMarginTop(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, val.Value, t.Right, t.Bottom);
    }

    private static void ApplyMarginRight(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyMarginBottom(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        var t = set.Margin ?? new Thickness(0);
        set.Margin = new Thickness(t.Left, t.Top, t.Right, val.Value);
    }

    private static void ApplyZIndex(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        set.ZIndex = (int)val.Value;
    }

    private static void ApplyWidth(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.Width = TryParseLength(val[0]);
    }

    private static void ApplyHeight(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.Height = TryParseLength(val[0]);
    }

    private static void ApplyMinWidth(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.MinWidth = TryParseLength(val[0]);
    }

    private static void ApplyMaxWidth(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.MaxWidth = TryParseLength(val[0]);
    }

    private static void ApplyMinHeight(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.MinHeight = TryParseLength(val[0]);
    }

    private static void ApplyMaxHeight(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        set.MaxHeight = TryParseLength(val[0]);
    }

    private static void ApplyGap(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        set.RowSpacing = val.Value;
        set.ColumnSpacing = val.Value;
    }

    private static void ApplyGapX(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        set.ColumnSpacing = val.Value;
    }

    private static void ApplyGapY(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        if (isNegative) val = -val;
        set.RowSpacing = val.Value;
    }

    private static void ApplyPadding(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.Padding = new Thickness(val.Value);
    }

    private static void ApplyPaddingX(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val.Value, t.Top, val.Value, t.Bottom);
    }

    private static void ApplyPaddingY(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val.Value, t.Right, val.Value);
    }

    private static void ApplyPaddingLeft(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(val.Value, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyPaddingTop(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, val.Value, t.Right, t.Bottom);
    }

    private static void ApplyPaddingRight(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, val.Value, t.Bottom);
    }


    private static void ApplyPaddingBottom(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        var t = set.Padding ?? new Thickness(0);
        set.Padding = new Thickness(t.Left, t.Top, t.Right, val.Value);
    }

    private static void ApplyLetterSpacing(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var value = v[0];
        set.LetterSpacing = value switch
        {
            "tighter" => -1,
            "tight" => -0.5,
            "normal" => 0,
            "wide" => 0.5,
            "wider" => 1,
            "widest" => 2,
            _ => TryParseValue(value)
        };
        if (isNegative) set.LetterSpacing = -set.LetterSpacing;
    }

    private static void ApplyLineHeight(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var value = v[0];
        double? letterSpacing = value switch
        {
            "none" => 1,
            "tight" => 1.25,
            "snug" => 1.375,
            "normal" => 1.5,
            "relaxed" => 1.625,
            "loose" => 2,
            _ => null
        };
        if (letterSpacing is not null)
        {
            set.LineHeight = letterSpacing.Value * 4 * UnitScale;
            return;
        }

        letterSpacing = TryParseValue(value);
        if (letterSpacing is null) return;
        set.LineHeight = letterSpacing.Value * UnitScale;
    }

    private static void ApplyMaxLine(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var value = TryParseValue(v[0]);
        if (value is null) return;
        set.MaxLines = (int)value;
    }

    private static void ApplyText(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        val *= UnitScale;
        set.FontSize = val.Value;
    }

    private static void ApplyFont(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        Fonts.FinalFonts ??= Fonts.ToFrozen();
        set.FontFamily = Fonts.FinalFonts[v[0]];
    }

    private static void ApplyDecorationWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        EnsureDecoration(set).StrokeThickness = val.Value;
    }

    private static void ApplyDecorationColor(string[] color, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        var c = ParseColor(color);
        if (c is null) return;
        EnsureDecoration(set).Stroke = new SolidColorBrush(c.Value);
    }

    private static void ApplyForeground(string[] color, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        var c = ParseColor(color);
        if (c is null) return;
        set.Foreground = new SolidColorBrush(c.Value);
    }

    private static void ApplyBackground(string[] color, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        var c = ParseColor(color);
        if (c is null) return;
        set.Background = new SolidColorBrush(c.Value);
    }

    private static void ApplyBorderBrush(string[] color, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        var c = ParseColor(color);
        if (c is null) return;
        set.BorderBrush = new SolidColorBrush(c.Value);
    }

    private static void ApplyBorderWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;
        set.BorderThickness = new Thickness(val.Value);
    }

    private static void ApplyBorderLeftWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;

        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(w, t.Top, t.Right, t.Bottom);
    }

    private static void ApplyBorderTopWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;

        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, w, t.Right, t.Bottom);
    }

    private static void ApplyBorderRightWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;

        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, w, t.Bottom);
    }

    private static void ApplyBorderBottomWidth(string[] widthStr, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (widthStr.Length != 1) return;
        var val = TryParseValue(widthStr[0]);
        if (val is null) return;

        var w = val.Value;
        var t = set.BorderThickness ?? new Thickness(0);
        set.BorderThickness = new Thickness(t.Left, t.Top, t.Right, w);
    }

    private static void ApplyCornerRadius(string[] v, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;
        set.CornerRadius = new CornerRadius(val.Value);
    }

    private static void ApplyOpacity(string[] val, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (double.TryParse(val[0], out var op))
            set.Opacity = op / 100.0; // opacity-50 -> 0.5
        else if (val[0] == "0") set.Opacity = 0;
        else if (val[0] == "100") set.Opacity = 1;
    }

    private static void ApplyShadow(StyleSet set, double blurRadius, Vector offset)
    {
        var temp = EnsureTemp(set);
        temp.BoxShadow = new BoxShadow
        {
            Blur = blurRadius,
            OffsetX = offset.X,
            OffsetY = offset.Y,
            Color = Color.Parse("#40000000")
        };
        temp.HasShadow = true;
    }

    private static void ApplyRingWidth(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;

        var w = val.Value;
        if (isNegative) w = -w;

        var temp = EnsureTemp(set);
        temp.RingWidth = w;
        temp.HasShadow = true;
    }

    private static void ApplyRingOffset(string[] v, bool isNegative, StyleSet set)
    {
        if (v.Length != 1) return;
        var val = TryParseValue(v[0]);
        if (val is null) return;

        var w = val.Value;
        if (isNegative) w = -w;

        var temp = EnsureTemp(set);
        temp.RingOffset = w;
        temp.HasShadow = true;
    }

    private static void ApplyRingColor(string[] color, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        var c = ParseColor(color);
        if (c is null) return;
        var temp = EnsureTemp(set);
        temp.RingColor = c.Value;
        temp.HasShadow = true;
    }

    private static TempStyle EnsureTemp(StyleSet set)
    {
        set.TempStyle ??= new TempStyle();
        return set.TempStyle;
    }

    private static TextDecoration EnsureDecoration(StyleSet set)
    {
        set.TempStyle ??= new TempStyle();
        set.TempStyle.TextDecoration ??= new TextDecoration();
        return set.TempStyle.TextDecoration;
    }

    private static void ApplyDuration(string[] vals, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (vals.Length != 1) return;
        var val = TryParseValue(vals[0]);
        if (val is null) return;

        EnsureTemp(set).Duration = val.Value;
    }

    private static void SetRelativePoint(double x, double y, StyleSet set)
    {
        set.RenderTransformOrigin = new RelativePoint(x, y, RelativeUnit.Relative);
    }

    private static void ApplyTranslate(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;
        translate *= UnitScale;
        if (isNegative) translate = -translate;
        EnsureTemp(set).TranslateTransform = new TranslateTransform { X = translate.Value, Y = translate.Value };
    }

    private static void ApplyTranslateX(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;

        translate *= UnitScale;
        if (isNegative) translate = -translate;
        var advanced = EnsureTemp(set);
        var translateTransform = advanced.TranslateTransform ?? new TranslateTransform();
        translateTransform.X = translate.Value;
        advanced.TranslateTransform = translateTransform;
    }

    private static void ApplyTranslateY(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var translate = TryParseValue(val);
        if (translate is null) return;

        translate *= UnitScale;
        if (isNegative) translate = -translate;
        var advanced = EnsureTemp(set);
        var translateTransform = advanced.TranslateTransform ?? new TranslateTransform();
        translateTransform.Y = translate.Value;
        advanced.TranslateTransform = translateTransform;
    }

    private static void ApplyScale(string[] vals, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        EnsureTemp(set).ScaleTransform = new ScaleTransform { ScaleX = scale.Value, ScaleY = scale.Value };
    }

    private static void ApplyScaleX(string[] vals, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        var advanced = EnsureTemp(set);
        var scaleTransform = advanced.ScaleTransform ?? new ScaleTransform();
        scaleTransform.ScaleX = scale.Value;
        advanced.ScaleTransform = scaleTransform;
    }

    private static void ApplyScaleY(string[] vals, bool isNegative, StyleSet set)
    {
        if (isNegative) return;
        if (vals.Length > 1) return;
        var val = vals[0];
        var scale = TryParseValue(val);
        if (scale is null) return;

        scale /= 100.0;
        var advanced = EnsureTemp(set);
        var scaleTransform = advanced.ScaleTransform ?? new ScaleTransform();
        scaleTransform.ScaleY = scale.Value;
        advanced.ScaleTransform = scaleTransform;
    }

    private static void ApplyRotate(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var rotate = TryParseValue(val);
        if (rotate is null) return;

        var r = rotate.Value / 180 * Math.PI;
        if (isNegative) r = -r;
        EnsureTemp(set).RotateTransform = new RotateTransform { Angle = r };
    }

    private static void ApplySkew(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        var sk = skew.Value / 180 * Math.PI;
        if (isNegative) sk = -sk;
        EnsureTemp(set).SkewTransform = new SkewTransform { AngleX = sk, AngleY = sk };
    }

    private static void ApplySkewX(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        var advanced = EnsureTemp(set);
        var scaleTransform = advanced.SkewTransform ?? new SkewTransform();
        var sk = skew.Value / 180 * Math.PI;
        if (isNegative) sk = -sk;
        scaleTransform.AngleX = sk;
        advanced.SkewTransform = scaleTransform;
    }

    private static void ApplySkewY(string[] vals, bool isNegative, StyleSet set)
    {
        if (vals.Length > 1) return;
        var val = vals[0];
        var skew = TryParseValue(val);
        if (skew is null) return;

        var advanced = EnsureTemp(set);
        var scaleTransform = advanced.SkewTransform ?? new SkewTransform();
        var sk = skew.Value / 180 * Math.PI;
        if (isNegative) sk = -sk;
        scaleTransform.AngleY = sk;
        advanced.SkewTransform = scaleTransform;
    }

    private static readonly Dictionary<string, double> ParseLengthResultCache = new()
    {
        ["auto"] = double.NaN,
        ["full"] = double.NaN,
        ["xs"] = 20 * UnitScale,
        ["sm"] = 24 * UnitScale,
        ["md"] = 28 * UnitScale,
        ["lg"] = 32 * UnitScale,
        ["xl"] = 36 * UnitScale,
        ["2xl"] = 42 * UnitScale,
        ["3xl"] = 48 * UnitScale,
        ["4xl"] = 56 * UnitScale,
        ["5xl"] = 64 * UnitScale,
        ["6xl"] = 72 * UnitScale,
        ["7xl"] = 80 * UnitScale,
    };

    private static double? TryParseLength(string s)
    {
        if (ParseLengthResultCache.TryGetValue(s, out var cacheValue)) return cacheValue;

        double? result = null;
        if (s.EndsWith("px") && double.TryParse(s[..^2], out var px)) result = px;
        else if (double.TryParse(s, out var val)) result = val * UnitScale;
        if (result is not null) ParseLengthResultCache[s] = result.Value;
        return result;
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
}