using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;

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
        if (ReferenceEquals(target, source)) return;

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

        target.LetterSpacing = source.LetterSpacing;
        target.LineHeight = source.LineHeight;
        target.LineSpacing = source.LineSpacing;
        target.MaxLines = source.MaxLines;
        target.TextTrimming = source.TextTrimming;
        target.TextAlignment = source.TextAlignment;
        target.VerticalTextAlignment = source.VerticalTextAlignment;
        target.TextWrapping = source.TextWrapping;
        target.TextDecorations = source.TextDecorations;
        target.Inlines = source.Inlines;

        target.FontSize = source.FontSize;
        target.FontFamily = source.FontFamily;
        target.FontWeight = source.FontWeight;
        target.FontStretch = source.FontStretch;
        target.FontFeatures = source.FontFeatures;
        target.FontStyle = source.FontStyle;
        target.Foreground = source.Foreground;

        target.Background = source.Background;
        target.BorderBrush = source.BorderBrush;
        target.BorderThickness = source.BorderThickness;
        target.CornerRadius = source.CornerRadius;

        target.Opacity = source.Opacity;
        target.IsVisible = source.IsVisible;

        target.Effect = source.Effect;
        if (source.BoxShadows is not null) target.BoxShadows = [..source.BoxShadows];
        target.Cursor = source.Cursor;
        target.FlowDirection = source.FlowDirection;

        target.RenderTransformOrigin = source.RenderTransformOrigin;

        if (source.RenderTransform is not null)
        {
            var builder = TransformOperations.CreateBuilder(4);
            builder.AppendMatrix(source.RenderTransform.Value);
            target.RenderTransform = builder.Build();
        }

        if (source.Transitions is null) return;
        target.Transitions = [..source.Transitions];
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
        if (style.Opacity is not null) border.Opacity = style.Opacity.Value;

        if (style.Effect is not null) border.Effect = style.Effect;
        var boxShadows = style.BoxShadows;
        if (boxShadows is not null)
        {
            if (boxShadows.Count == 1)
            {
                border.BoxShadow = new(boxShadows[0]);
            }
            else if (boxShadows.Count > 1)
            {
                var rest = new BoxShadow[boxShadows.Count - 1];
                for (var i = 1; i < boxShadows.Count; i++)
                {
                    rest[i - 1] = boxShadows[i];
                }

                border.BoxShadow = new(boxShadows[0], rest);
            }
        }

        if (style.Cursor is not null) border.Cursor = style.Cursor;
        if (style.FlowDirection is not null) border.FlowDirection = style.FlowDirection.Value;

        border.RenderTransformOrigin = style.RenderTransformOrigin ?? RelativePoint.Center;
        border.RenderTransform = style.RenderTransform;
        border.Transitions ??= style.Transitions;
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

            if (other.Effect is not null) style.Effect = other.Effect;
            if (other.Cursor is not null) style.Cursor = other.Cursor;
            if (other.FlowDirection is not null) style.FlowDirection = other.FlowDirection;

            if (other.RenderTransformOrigin is not null) style.RenderTransformOrigin = other.RenderTransformOrigin;
            if (other.RenderTransform is not null) style.RenderTransform = other.RenderTransform;
            if (other.Transitions is not null) style.Transitions = other.Transitions;
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