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

        FontWeight = FontWeight.Normal,
        BorderThickness = new Thickness(0),

        CornerRadius = new CornerRadius(0),

        Opacity = 1.0,
        IsVisible = true
    };

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

        if(style.ClipToBounds is not null) border.ClipToBounds = style.ClipToBounds.Value;
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

    extension(BaseStyle target)
    {
        internal void InnerCopy(BaseStyle result)
        {
            if (target.BoxShadows is not null) result.BoxShadows = [..target.BoxShadows];
            if (target.RenderTransform is not null)
            {
                var builder = TransformOperations.CreateBuilder(4);
                builder.AppendMatrix(target.RenderTransform.Value);
                result.RenderTransform = builder.Build();
            }

            if (target.Transitions is not null) result.Transitions = [..target.Transitions];
        }

        public BaseStyle Copy()
        {
            var result = new BaseStyle();
            result.Merge(target, true);
            target.InnerCopy(result);
            return result;
        }

        public void Merge(BaseStyle source, bool mergeNull = false)
        {
            if (mergeNull)
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

                target.HorizontalAlignment = source.HorizontalAlignment;
                target.VerticalAlignment = source.VerticalAlignment;

                target.Background = source.Background;
                target.BorderBrush = source.BorderBrush;
                target.BorderThickness = source.BorderThickness;
                target.CornerRadius = source.CornerRadius;

                target.Opacity = source.Opacity;
                target.IsVisible = source.IsVisible;

                target.ClipToBounds = source.ClipToBounds;
                target.Effect = source.Effect;
                target.BoxShadows = source.BoxShadows;
                target.Cursor = source.Cursor;
                target.FlowDirection = source.FlowDirection;

                target.RenderTransformOrigin = source.RenderTransformOrigin;
                target.RenderTransform = source.RenderTransform;
                target.Transitions = source.Transitions;
                return;
            }

            if (source.Margin is not null) target.Margin = source.Margin;
            if (source.ZIndex is not null) target.ZIndex = source.ZIndex;

            if (source.Width is not null) target.Width = source.Width;
            if (source.Height is not null) target.Height = source.Height;
            if (source.MinWidth is not null) target.MinWidth = source.MinWidth;
            if (source.MaxWidth is not null) target.MaxWidth = source.MaxWidth;
            if (source.MinHeight is not null) target.MinHeight = source.MinHeight;
            if (source.MaxHeight is not null) target.MaxHeight = source.MaxHeight;

            if (source.Padding is not null) target.Padding = source.Padding;

            if (source.HorizontalAlignment is not null) target.HorizontalAlignment = source.HorizontalAlignment;
            if (source.VerticalAlignment is not null) target.VerticalAlignment = source.VerticalAlignment;

            if (source.Background is not null) target.Background = source.Background;
            if (source.BorderBrush is not null) target.BorderBrush = source.BorderBrush;
            if (source.BorderThickness is not null) target.BorderThickness = source.BorderThickness;
            if (source.CornerRadius is not null) target.CornerRadius = source.CornerRadius;

            if (source.Opacity is not null) target.Opacity = source.Opacity;
            if (source.IsVisible is not null) target.IsVisible = source.IsVisible;

            if (source.ClipToBounds is not null) target.ClipToBounds = source.ClipToBounds;
            if (source.Effect is not null) target.Effect = source.Effect;
            if (source.BoxShadows is not null) target.BoxShadows = source.BoxShadows;
            if (source.Cursor is not null) target.Cursor = source.Cursor;
            if (source.FlowDirection is not null) target.FlowDirection = source.FlowDirection;

            if (source.RenderTransformOrigin is not null) target.RenderTransformOrigin = source.RenderTransformOrigin;
            if (source.RenderTransform is not null) target.RenderTransform = source.RenderTransform;
            if (source.Transitions is not null) target.Transitions = source.Transitions;
        }

        public void MergeMany(params BaseStyle[] styles)
        {
            foreach (var s in styles)
            {
                target.Merge(s);
            }
        }
    }

    extension(StyleSet target)
    {
        public StyleSet Copy()
        {
            var result = new StyleSet();
            result.Merge(target, true);
            target.InnerCopy(result);
            return result;
        }

        public void Merge(StyleSet source, bool mergeNull = false)
        {
            target.Merge((BaseStyle)source, mergeNull);
            if (mergeNull)
            {
                target.RowSpacing = source.RowSpacing;
                target.ColumnSpacing = source.ColumnSpacing;

                target.Orientation = source.Orientation;

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
                return;
            }

            if (source.RowSpacing is not null) target.RowSpacing = source.RowSpacing;
            if (source.ColumnSpacing is not null) target.ColumnSpacing = source.ColumnSpacing;

            if (source.Orientation is not null) target.Orientation = source.Orientation;

            if (source.LetterSpacing is not null) target.LetterSpacing = source.LetterSpacing;
            if (source.LineHeight is not null) target.LineHeight = source.LineHeight;
            if (source.LineHeight is not null) target.LineHeight = source.LineSpacing;
            if (source.MaxLines is not null) target.MaxLines = source.MaxLines;
            if (source.TextTrimming is not null) target.TextTrimming = source.TextTrimming;
            if (source.TextAlignment is not null) target.TextAlignment = source.TextAlignment;
            if (source.VerticalTextAlignment is not null) target.VerticalTextAlignment = source.VerticalTextAlignment;
            if (source.TextWrapping is not null) target.TextWrapping = source.TextWrapping;
            if (source.TextDecorations is not null) target.TextDecorations = source.TextDecorations;
            if (source.Inlines is not null) target.Inlines = source.Inlines;

            if (source.FontSize is not null) target.FontSize = source.FontSize;
            if (source.FontFamily is not null) target.FontFamily = source.FontFamily;
            if (source.FontWeight is not null) target.FontWeight = source.FontWeight;
            if (source.FontStretch is not null) target.FontStretch = source.FontStretch;
            if (source.FontFeatures is not null) target.FontFeatures = source.FontFeatures;
            if (source.FontStyle is not null) target.FontStyle = source.FontStyle;
            if (source.Foreground is not null) target.Foreground = source.Foreground;
        }

        public void MergeMany(params StyleSet[] styles)
        {
            foreach (var s in styles)
            {
                target.Merge(s);
            }
        }
    }
}