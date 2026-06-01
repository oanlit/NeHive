using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using Avalonia.Media;
using Avalonia.Input;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static class HSliderStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        Height = 20,
        Orientation = Orientation.Horizontal,

        Foreground = Brushes.White,
        Background = Brushes.Gray,
        BorderThickness = new Thickness(0)
    };
}

public class HCustomSliderState(StyleSet baseStyle)
{
    public StyleSet BaseStyle = baseStyle;
    public StyleSet CurrentStyle = StyleUtil.Copy(baseStyle);
    public Dictionary<string, List<string>>? Variants;

    public bool IsHover;
    public bool IsDragging;

    public double Value;
    public double Min;
    public double Max = 100;

    public void Reset()
    {
        CurrentStyle.Merge(BaseStyle);
    }

    public void ApplyVariants()
    {
        if (Variants == null) return;

        if (IsHover && Variants.TryGetValue("hover", out var hover))
            StyleParser.Parse(hover, ref CurrentStyle);

        if (IsDragging && Variants.TryGetValue("click", out var click))
            StyleParser.Parse(click, ref CurrentStyle);
    }

    public double Clamp(double v)
        => Math.Clamp(v, Min, Max);

    public double Ratio => (Value - Min) / (Max - Min);
}

public static partial class BaseComponent
{
    public static IElement HCustomSlider(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        double minimum = 0,
        double maximum = 100,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null,
        Action<double>? onValueChanged = null)
    {
        if (strStyle != null)
            style = StyleParser.ParseFull(strStyle);

        var uiScope = new UiScope();
        HCustomSliderState state;

        var track = new Border
        {
            // Height = 4,
            CornerRadius = new CornerRadius(2),
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var fill = new Border
        {
            // Height = 4,
            CornerRadius = new CornerRadius(2),
            Background = Brushes.DodgerBlue,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var thumb = new Border
        {
            Width = 12,
            Height = 12,
            CornerRadius = new CornerRadius(6),
            Background = Brushes.DodgerBlue,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var touch = new Border
        {
            Background = Brushes.Transparent,
            Width = 0,
            ZIndex = 100
        };

        var panel = new Panel();
        panel.Children.Add(track);
        panel.Children.Add(fill);
        panel.Children.Add(thumb);
        panel.Children.Add(touch);

        if (style is null)
        {
            state = new HCustomSliderState(HSliderStyle.DefaultStyleSet)
            {
                Min = minimum,
                Max = maximum
            };
            ApplyStyle(state.CurrentStyle); // 应用默认样式
        }
        else
        {
            state = new HCustomSliderState(style.Value.Normal)
            {
                Min = minimum,
                Max = maximum
            };
            ApplyStyle(state.CurrentStyle);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                state.BaseStyle = styleValue.Normal;
                state.Variants = styleValue.Variants;
                state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
                ApplyStyle(state.CurrentStyle);
            });
        }

        value = bindValue ?? value;
        if (value is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var v = scope.Track(value);
                state.Value = state.Clamp(v);
            });
        }
        else
        {
            state.Value = state.Clamp(0);
        }


        uiScope.OnMount += () =>
        {
            UpdateThumb();
            touch.PointerPressed += (_, e) =>
            {
                touch.Height = 9999;
                touch.Width = 9999;
                DraggingThumb(e);
            };

            touch.PointerMoved += (_, e) =>
            {
                if (!state.IsDragging) return;
                UpdateFromPointer(e);
            };

            touch.PointerReleased += (_, e) =>
            {
                state.IsDragging = false;
                e.Handled = true;
                
                touch.Width = track.Bounds.Width;
                touch.Height = thumb.Bounds.Height;
            };

            panel.PointerEntered += (_, _) =>
            {
                state.IsHover = true;
                
                touch.Width = track.Bounds.Width;
                touch.Height = thumb.Bounds.Height;

                state.ApplyVariants();
            };

            panel.PointerExited += (_, _) =>
            {
                state.IsHover = false;
                state.IsDragging = false;
                touch.Height = 0;
                touch.Width = 0;
                state.Reset();
            };
        };

        return new Element(uiScope, panel);

        void UpdateThumb()
        {
            var ratio = state.Ratio;
            var fillValue = panel.Bounds.Width;

            if (fillValue <= 0) return;

            fillValue *= ratio;

            thumb.Margin = new Thickness(fillValue - thumb.Bounds.Width / 2, 0, 0, 0);
            fill.Width = fillValue;
        }

        void DraggingThumb(PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(panel).Properties.IsLeftButtonPressed)
                return;

            state.IsDragging = true;
            UpdateFromPointer(e);
            e.Handled = true;
        }

        void UpdateFromPointer(PointerEventArgs e)
        {
            var p = e.GetPosition(panel);

            var ratio = panel.Bounds.Width <= 0
                ? 0
                : p.X / panel.Bounds.Width;

            ratio = Math.Clamp(ratio, 0, 1);

            state.Value = state.Min + ratio * (state.Max - state.Min);

            bindValue?.RxValue = state.Value;

            onValueChanged?.Invoke(state.Value);
            UpdateThumb();
        }

        void ApplyStyle(StyleSet styleSet)
        {
            if (styleSet.Margin is not null) panel.Margin = styleSet.Margin.Value;
            if (styleSet.ZIndex is not null) panel.ZIndex = styleSet.ZIndex.Value;
            if (styleSet.HorizontalAlignment is not null)
                panel.HorizontalAlignment = styleSet.HorizontalAlignment.Value;
            if (styleSet.VerticalAlignment is not null)
                panel.VerticalAlignment = styleSet.VerticalAlignment.Value;

            if (styleSet.Width is not null)
                panel.Width = styleSet.Width.Value;

            if (styleSet.Height is not null)
                panel.Height = styleSet.Height.Value;

            if (styleSet.MinWidth is not null)
                panel.MinWidth = styleSet.MinWidth.Value;

            if (styleSet.MaxWidth is not null)
                panel.MaxWidth = styleSet.MaxWidth.Value;

            if (styleSet.MinHeight is not null)
                panel.MinHeight = styleSet.MinHeight.Value;

            if (styleSet.MaxHeight is not null)
                panel.MaxHeight = styleSet.MaxHeight.Value;

            if (styleSet.Foreground is not null) fill.Background = styleSet.Foreground;
            if (styleSet.Background is not null) track.Background = styleSet.Background;
            if (styleSet.BackgroundSizing is not null) track.BackgroundSizing = styleSet.BackgroundSizing.Value;

            if (styleSet.BorderBrush is not null) track.BorderBrush = styleSet.BorderBrush;
            if (styleSet.BorderThickness.HasValue) track.BorderThickness = styleSet.BorderThickness.Value;
            if (styleSet.CornerRadius is not null) track.CornerRadius = styleSet.CornerRadius.Value;

            if (styleSet.Opacity is not null)
                panel.Opacity = styleSet.Opacity.Value;
        }
    }
}