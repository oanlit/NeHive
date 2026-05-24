using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Animation;

namespace NeHive.UI.Avalonia.Styles;

public static class StyleUtil
{
    public static StyleSet FromDefault() => new()
    {
        Margin = new Thickness(0),
        Padding = new Thickness(0),

        Orientation = Orientation.Vertical,
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,

        FontWeight = global::Avalonia.Media.FontWeight.Normal,
        BorderThickness = new Thickness(0),

        CornerRadius = new CornerRadius(0),

        Opacity = 1.0,
        IsVisible = true
    };

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
        // else target.Advanced = source.AdvancedStyle.Copy(ref source.Advanced);
    }

    public static void ApplyStyle(BaseStyle style, Layoutable layout, Border border)
    {
        if (style.Margin is not null) border.Margin = style.Margin.Value;
        if (style.ZIndex is not null) border.ZIndex = style.ZIndex.Value;

        if (style.Width is not null)
            border.Width = style.Width.Value;

        if (style.Height is not null)
            border.Height = style.Height.Value;

        if (style.MinWidth is not null)
            border.MinWidth = style.MinWidth.Value;

        if (style.MaxWidth is not null)
            border.MaxWidth = style.MaxWidth.Value;

        if (style.MinHeight is not null)
            border.MinHeight = style.MinHeight.Value;

        if (style.MaxHeight is not null)
            border.MaxHeight = style.MaxHeight.Value;

        if (style.Padding.HasValue) border.Padding = style.Padding.Value;

        if (style.HorizontalAlignment is not null)
        {
            border.HorizontalAlignment = style.HorizontalAlignment.Value;
            layout.HorizontalAlignment = style.HorizontalAlignment.Value;
        }

        if (style.VerticalAlignment is not null)
        {
            border.VerticalAlignment = style.VerticalAlignment.Value;
            layout.VerticalAlignment = style.VerticalAlignment.Value;
        }

        if (style.Background is not null) border.Background = style.Background;
        if (style.BackgroundSizing is not null) border.BackgroundSizing = style.BackgroundSizing.Value;

        if (style.BorderBrush is not null) border.BorderBrush = style.BorderBrush;
        if (style.BorderThickness.HasValue) border.BorderThickness = style.BorderThickness.Value;
        if (style.CornerRadius is not null) border.CornerRadius = style.CornerRadius.Value;

        if (style.Opacity is not null)
            border.Opacity = style.Opacity.Value;

        if (style.Advanced is null) return;
        var advanced = style.Advanced;

        if (advanced.Effect is not null) layout.Effect = advanced.Effect;
        if (advanced.Cursor is not null) border.Cursor = advanced.Cursor;
        if (advanced.FlowDirection is not null) layout.FlowDirection = advanced.FlowDirection.Value;

        layout.RenderTransformOrigin = advanced.RelativePoint ?? RelativePoint.Center;

        if (advanced.Transform is not null) layout.RenderTransform = advanced.Transform;

        var scope = advanced.TransitionScope;
        if (scope is not null)
        {
            advanced.Transition ??= new TransformOperationsTransition();
            AvaloniaProperty? prop = scope switch
            {
                TransitionScope.Transform => Visual.RenderTransformProperty,
                TransitionScope.Opacity => Visual.OpacityProperty,
                TransitionScope.Colors => Border.BackgroundProperty,
                _ => null
            };
            if (prop is not null)
                advanced.Transition.Property = prop;
        }

        if (advanced.Transition is not null)
        {
            layout.Transitions =
            [
                advanced.Transition
            ];
        }
    }

    extension(StyleSet style)
    {
        public void Merge(StyleSet other)
        {
            if (other.Margin is not null) style.Margin = other.Margin;
            if (other.ZIndex is not null) style.ZIndex = other.ZIndex;

            if (other.Width is not null) style.Width = other.Width;
            if (other.Height is not null) style.Height = other.Height;
            if (other.MinWidth is not null) style.MinWidth = other.MinWidth;
            if (other.MaxWidth is not null) style.MaxWidth = other.MaxWidth;
            if (other.MinHeight is not null) style.MinHeight = other.MinHeight;
            if (other.MaxHeight is not null) style.MaxHeight = other.MaxHeight;

            if (other.Padding is not null) style.Padding = other.Padding;
            if (other.RowSpacing is not null) style.RowSpacing = other.RowSpacing;
            if (other.ColumnSpacing is not null) style.ColumnSpacing = other.ColumnSpacing;

            if (other.Orientation is not null) style.Orientation = other.Orientation;
            if (other.HorizontalAlignment is not null) style.HorizontalAlignment = other.HorizontalAlignment;
            if (other.VerticalAlignment is not null) style.VerticalAlignment = other.VerticalAlignment;

            if (other.TextAlignment is not null) style.TextAlignment = other.TextAlignment;
            if (other.VerticalTextAlignment is not null) style.VerticalTextAlignment = other.VerticalTextAlignment;
            if (other.TextWrapping is not null) style.TextWrapping = other.TextWrapping;

            if (other.FontSize is not null) style.FontSize = other.FontSize;
            if (other.FontWeight is not null) style.FontWeight = other.FontWeight;
            if (other.FontStyle is not null) style.FontStyle = other.FontStyle;
            if (other.Foreground is not null) style.Foreground = other.Foreground;

            if (other.Background is not null) style.Background = other.Background;
            if (other.BorderBrush is not null) style.BorderBrush = other.BorderBrush;
            if (other.BorderThickness is not null) style.BorderThickness = other.BorderThickness;
            if (other.CornerRadius is not null) style.CornerRadius = other.CornerRadius;

            if (other.Opacity is not null) style.Opacity = other.Opacity;
            if (other.IsVisible is not null) style.IsVisible = other.IsVisible;

            if (other.Advanced is null) return;

            if (style.Advanced is null)
            {
                style.Advanced = other.Advanced;
                return;
            }

            var otherAdvanced = other.Advanced;

            if (otherAdvanced.Effect is not null) style.Advanced.Effect = otherAdvanced.Effect;
            if (otherAdvanced.Cursor is not null) style.Advanced.Cursor = otherAdvanced.Cursor;
            if (otherAdvanced.FlowDirection is not null) style.Advanced.FlowDirection = otherAdvanced.FlowDirection;

            if (otherAdvanced.RelativePoint is not null) style.Advanced.RelativePoint = otherAdvanced.RelativePoint;

            if (otherAdvanced.ScaleTransform is not null) style.Advanced.ScaleTransform = otherAdvanced.ScaleTransform;
            if (otherAdvanced.RotateTransform is not null)
                style.Advanced.RotateTransform = otherAdvanced.RotateTransform;
            if (otherAdvanced.SkewTransform is not null) style.Advanced.SkewTransform = otherAdvanced.SkewTransform;
            if (otherAdvanced.MatrixTransform is not null)
                style.Advanced.MatrixTransform = otherAdvanced.MatrixTransform;

            if (otherAdvanced.Transform is not null) style.Advanced.Transform = otherAdvanced.Transform;
            if (otherAdvanced.Transition is not null) style.Advanced.Transition = otherAdvanced.Transition;
        }

        public void MergeMany(params StyleSet[] styles)
        {
            foreach (var s in styles)
            {
                style.Merge(s);
            }
        }
    }
}