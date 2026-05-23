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
    public TransformOperationsTransition? Transition;

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

    public static StyleSet Copy(StyleSet other)
    {
        var result = new StyleSet();
        Copy(ref result, other);
        return result;
    }

    public static void Copy(ref StyleSet target, StyleSet source)
    {
        target.Margin = source.Margin;
        target.ZIndex = source.ZIndex;

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

        if (source.Advanced is null) target.Advanced = null;
        else target.Advanced = AdvancedStyle.Copy(ref source.Advanced);
    }

    public void Merge(StyleSet other, bool mergeNull = false)
    {
        if (mergeNull)
        {
            Margin = other.Margin;
            ZIndex = other.ZIndex;

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

            Advanced = other.Advanced;

            return;
        }

        if (other.Margin is not null) Margin = other.Margin;
        if (other.ZIndex is not null) ZIndex = other.ZIndex;

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

        if (other.Advanced is null) return;

        if (Advanced is null)
        {
            Advanced = other.Advanced;
            return;
        }

        var otherAdvanced = other.Advanced;

        if (otherAdvanced.RelativePoint is not null) Advanced.RelativePoint = otherAdvanced.RelativePoint;

        if (otherAdvanced.ScaleTransform is not null) Advanced.ScaleTransform = otherAdvanced.ScaleTransform;
        if (otherAdvanced.RotateTransform is not null) Advanced.RotateTransform = otherAdvanced.RotateTransform;
        if (otherAdvanced.SkewTransform is not null) Advanced.SkewTransform = otherAdvanced.SkewTransform;
        if (otherAdvanced.MatrixTransform is not null) Advanced.MatrixTransform = otherAdvanced.MatrixTransform;

        if (otherAdvanced.Transform is not null) Advanced.Transform = otherAdvanced.Transform;
        if (otherAdvanced.Transition is not null) Advanced.Transition = otherAdvanced.Transition;

        if (otherAdvanced.Effect is not null) Advanced.Effect = otherAdvanced.Effect;
        if (otherAdvanced.Cursor is not null) Advanced.Cursor = otherAdvanced.Cursor;
        if (otherAdvanced.FlowDirection is not null) Advanced.FlowDirection = otherAdvanced.FlowDirection;
    }

    public void MergeMany(bool mergeNull, params StyleSet[] styles)
    {
        foreach (var style in styles)
        {
            Merge(style, mergeNull);
        }
    }
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