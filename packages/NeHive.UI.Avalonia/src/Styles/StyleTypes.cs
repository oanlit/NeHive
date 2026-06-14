using Avalonia;
using Avalonia.Controls.Documents;
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

    public bool? ClipToBounds;
    public IEffect? Effect;
    public List<BoxShadow>? BoxShadows;
    public Cursor? Cursor;
    public FlowDirection? FlowDirection;

    public RelativePoint? RenderTransformOrigin;
    public ITransform? RenderTransform;
    public Transitions? Transitions;

    internal TempStyle? TempStyle;
}

internal class TempStyle
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
    
    public TextDecoration? TextDecoration;

    public TransitionScope? TransitionScope;
    public double? Duration;
    public Easing? Easing;

    public TranslateTransform? TranslateTransform;
    public ScaleTransform? ScaleTransform;
    public RotateTransform? RotateTransform;
    public SkewTransform? SkewTransform;
}

public class StyleSet : BaseStyle
{
    public double? RowSpacing;
    public double? ColumnSpacing;

    public Orientation? Orientation;

    public double? LetterSpacing;
    public double? LineHeight;
    public double? LineSpacing;
    
    public int? MaxLines;
    public TextTrimming? TextTrimming;
    
    public TextAlignment? TextAlignment;
    public VerticalAlignment? VerticalTextAlignment;
    
    public TextWrapping? TextWrapping;
    public TextDecorationCollection? TextDecorations;
    public InlineCollection? Inlines;

    public double? FontSize;
    public FontWeight? FontWeight;
    public FontFamily? FontFamily;
    public FontStretch? FontStretch;
    public FontFeatureCollection? FontFeatures;
    public FontStyle? FontStyle;
    public IBrush? Foreground;
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

public readonly struct StyleDefinitions
{
    public Dictionary<string, Action<string[], bool, StyleSet>> Handlers { get; init; }
    public Dictionary<string, Color> Colors { get; init; }
    public Dictionary<string, string[]> Fonts { get; init; }
}