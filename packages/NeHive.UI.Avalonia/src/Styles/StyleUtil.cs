using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Animation;

namespace NeHive.UI.Avalonia.Styles;

public static class StyleUtil
{
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
            layout.HorizontalAlignment = style.HorizontalAlignment.Value;
        if (style.VerticalAlignment is not null)
            layout.VerticalAlignment = style.VerticalAlignment.Value;

        if (style.Background is not null) border.Background = style.Background;
        if (style.BackgroundSizing is not null) border.BackgroundSizing = style.BackgroundSizing.Value;

        if (style.BorderBrush is not null) border.BorderBrush = style.BorderBrush;
        if (style.BorderThickness.HasValue) border.BorderThickness = style.BorderThickness.Value;
        if (style.CornerRadius is not null) border.CornerRadius = style.CornerRadius.Value;

        if (style.Opacity is not null)
            layout.Opacity = style.Opacity.Value;

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
}