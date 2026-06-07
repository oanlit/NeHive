using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace NeHive.UI.Avalonia.Styles;

// 定位 → 尺寸 → 内间距 → 布局排版 → 文字 → 颜色(渐变) → 边框 → 特效
public class BaseStyle
{
    public Thickness? Margin;
    public int? ZIndex;

    public double? Width;
    public double? Height;
    public double? MinWidth;
    public double? MaxWidth;
    public double? MinHeight;
    public double? MaxHeight;

    public Thickness? Padding;

    public HorizontalAlignment? HorizontalAlignment;
    public VerticalAlignment? VerticalAlignment;

    public IBrush? Background;
    public IBrush? BorderBrush;
    public Thickness? BorderThickness;
    public BackgroundSizing? BackgroundSizing;
    public CornerRadius? CornerRadius;

    public double? Opacity;
    public bool? IsVisible;

    public AdvancedStyle? Advanced;
}

public sealed class AdvancedStyle
{
    public double? FgGradientDir;
    public Color? FgFromColor;
    public Color? FgToColor;

    public double? BgGradientDir;
    public Color? BgFromColor;
    public Color? BgToColor;

    public double? BorderGradientDir;
    public Color? BorderFromColor;
    public Color? BorderToColor;

    public IEffect? Effect;
    public List<BoxShadow>? BoxShadows;

    public Cursor? Cursor;
    public FlowDirection? FlowDirection;

    public TransitionScope? TransitionScope;
    public double? Duration;
    public Easing? Easing;
    public TransitionBase? Transition;

    public RelativePoint? RelativePoint;

    public ITransform? Transform;
    public TranslateTransform? TranslateTransform;
    public ScaleTransform? ScaleTransform;
    public RotateTransform? RotateTransform;
    public SkewTransform? SkewTransform;
    public MatrixTransform? MatrixTransform;

    public static AdvancedStyle Copy(ref AdvancedStyle style)
    {
        return new AdvancedStyle
        {
            FgGradientDir = style.FgGradientDir,
            FgFromColor = style.FgFromColor,
            FgToColor = style.FgToColor,

            BgGradientDir = style.BgGradientDir,
            BgFromColor = style.BgFromColor,
            BgToColor = style.BgToColor,

            BorderGradientDir = style.BorderGradientDir,
            BorderFromColor = style.BorderFromColor,
            BorderToColor = style.BorderToColor,

            Effect = style.Effect,
            Cursor = style.Cursor,
            FlowDirection = style.FlowDirection,

            TransitionScope = style.TransitionScope,
            Duration = style.Duration,
            RelativePoint = style.RelativePoint,
            Transform = style.Transform,

            TranslateTransform = style.TranslateTransform,
            ScaleTransform = style.ScaleTransform,
            RotateTransform = style.RotateTransform,
            SkewTransform = style.SkewTransform,
            MatrixTransform = style.MatrixTransform,
            Transition = style.Transition
        };
    }
}

public class StyleSet : BaseStyle
{
    public double? RowSpacing;
    public double? ColumnSpacing;

    public OverflowHandle? OverflowHandle;

    public Orientation? Orientation;

    public TextAlignment? TextAlignment;
    public VerticalAlignment? VerticalTextAlignment;
    public TextWrapping? TextWrapping;

    public double? FontSize;
    public FontWeight? FontWeight;
    public FontStyle? FontStyle;
    public IBrush? Foreground;
}

public enum OverflowHandle
{
    Visible,
    Hidden
}

public enum TransitionScope
{
    None,
    All,
    Opacity,
    Transform,
    Colors,
    Shadow
}

public struct FullStyle
{
    public StyleSet Normal;

    public Dictionary<string, List<string>> Variants;
}