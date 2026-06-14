using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using Avalonia.Media;
using Avalonia.Input;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static class HSliderStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        Height = 20,
        Orientation = Orientation.Horizontal,

        Foreground = Brushes.DodgerBlue,
        Background = Brushes.Gray,
        BorderThickness = new Thickness(0)
    };
}

internal class HCustomSliderState(StyleSet baseStyle)
{
    internal StyleSet BaseStyle = baseStyle;
    internal StyleSet CurrentStyle = StyleUtil.Copy(baseStyle);
    internal Dictionary<string, List<string>>? Variants;

    // public bool IsHover;
    // public bool IsDragging;
    internal required HCustomSliderGroup Group;

    internal MutSignal<double> Value = new(0);
    internal Accessor<double> Min = 0;
    internal Accessor<double> Max = 100;

    internal void Reset()
    {
        CurrentStyle.Merge(BaseStyle);
    }

    internal void ApplyVariants()
    {
        if (Variants == null) return;

        if (Group.IsHover.Value && Variants.TryGetValue("hover", out var hover))
            StyleParser.Parse(hover, ref CurrentStyle);

        if (Group.IsDragging.Value && Variants.TryGetValue("drag", out var click))
            StyleParser.Parse(click, ref CurrentStyle);
    }

    internal double Clamp(double v)
        => Math.Clamp(v, Min.RxValue, Max.RxValue);

    internal double RxRatio() => (Value.RxValue - Min.RxValue) / (Max.RxValue - Min.RxValue);
}

public class HCustomSliderGroup : IGroupState
{
    private MutSignal<bool>? _isHover;
    private MutSignal<bool>? _isFocus;
    private MutSignal<bool>? _isDragging;

    public Signal<bool> IsHover
    {
        get
        {
            _isHover ??= new MutSignal<bool>(false);
            return _isHover;
        }
    }

    internal void SetHover(bool value) => _isHover?.RxValue = value;

    public Signal<bool> IsFocus
    {
        get
        {
            _isFocus ??= new MutSignal<bool>(false);
            return _isFocus;
        }
    }

    internal void SetFocus(bool value) => _isFocus?.RxValue = value;

    public Signal<bool> IsDragging
    {
        get
        {
            _isDragging ??= new MutSignal<bool>(false);
            return _isDragging;
        }
    }

    internal void SetDragging(bool value) => _isDragging?.RxValue = value;
}

public class HCustomSliderProp
{
    public readonly Accessor<double>? Value;
    public readonly MutSignal<double>? BindValue;
    public readonly Accessor<double>? Minimum;
    public readonly Accessor<double>? Maximum;
    public readonly Accessor<FullStyle>? Style;
    public readonly Action<double>? OnValueChanged;
    public Func<HCustomSliderGroup, IElement>? Thumb { get; init; }

    public HCustomSliderProp(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<string>? strStyle = null,
        Action<double>? onValueChanged = null)
    {
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle, HSliderStyle.DefaultStyleSet);
        }

        Value = bindValue ?? value;
        BindValue = bindValue;
        Minimum = minimum;
        Maximum = maximum;
        OnValueChanged = onValueChanged;
    }
}

public static partial class BaseComponent
{
    public static IElement HCustomSlider(HCustomSliderProp prop)
    {
        var uiScope = new UiScope();
        HCustomSliderState state;
        var group = new HCustomSliderGroup();

        var track = new Border
        {
            CornerRadius = new CornerRadius(2),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var fill = new Border
        {
            CornerRadius = new CornerRadius(2),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        // var thumb = new Border
        // {
        //     Width = 12,
        //     Height = 12,
        //     CornerRadius = new CornerRadius(6),
        //     Background = Brushes.DodgerBlue,
        //     HorizontalAlignment = HorizontalAlignment.Left,
        //     VerticalAlignment = VerticalAlignment.Stretch
        // };

        Control thumb;

        if (prop.Thumb is null)
        {
            thumb = new Border
            {
                Width = 12,
                Height = 12,
                CornerRadius = new CornerRadius(6),
                Background = Brushes.DodgerBlue,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
        else
        {
            thumb = prop.Thumb(group).Content;
            thumb.HorizontalAlignment = HorizontalAlignment.Center;
            thumb.VerticalAlignment = VerticalAlignment.Center;
            thumb = new Border
            {
                Width = thumb.Width,
                Height = thumb.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = thumb
            };
        }

        var touch = new Border
        {
            Background = Brushes.Transparent,
            // Background = Brushes.Beige,
            Width = Math.Max(thumb.Width, track.Width),
            Height = Math.Max(thumb.Height, track.Height),
        };

        var panel = new Panel();
        panel.Children.Add(track);
        panel.Children.Add(fill);
        panel.Children.Add(thumb);
        panel.Children.Add(touch);

        if (prop.Style is null)
        {
            state = new HCustomSliderState(HSliderStyle.DefaultStyleSet)
            {
                Group = group,
                Min = prop.Minimum ?? 0,
                Max = prop.Maximum ?? 100,
                Value = prop.BindValue ?? new MutSignal<double>(0)
            };
            ApplyStyle(state.CurrentStyle); // 应用默认样式
        }
        else
        {
            state = new HCustomSliderState(prop.Style.Value.Normal)
            {
                Group = group,
                Min = prop.Minimum ?? 0,
                Max = prop.Maximum ?? 100,
                Value = prop.BindValue ?? new MutSignal<double>(0)
            };
            ApplyStyle(state.CurrentStyle);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(prop.Style);
                state.BaseStyle = styleValue.Normal;
                state.Variants = styleValue.Variants;
                state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
                ApplyStyle(state.CurrentStyle);
            });
        }

        if (prop.Value is not null && prop.BindValue is null)
        {
            uiScope.CreateEffect(scope =>
            {
                var v = scope.Track(prop.Value);
                state.Value.RxValue = state.Clamp(v);
            });
        }

        uiScope.OnMount += () =>
        {
            uiScope.CreateEffect(epochScope =>
            {
                var ratio = epochScope.Track(state.RxRatio);
                double fillValue;
                if (state.CurrentStyle.Orientation is Orientation.Vertical)
                {
                    fillValue = panel.Bounds.Height;
                }
                else
                {
                    fillValue = panel.Bounds.Width;
                }

                if (fillValue <= 0) return;

                fillValue *= ratio;

                if (state.CurrentStyle.Orientation is Orientation.Vertical)
                {
                    thumb.Margin = new Thickness(0, 0, 0, fillValue - thumb.Bounds.Width / 2);
                    fill.Height = fillValue;
                }
                else
                {
                    thumb.Margin = new Thickness(fillValue - thumb.Bounds.Width / 2, 0, 0, 0);
                    fill.Width = fillValue;
                }
            });

            touch.PointerPressed += (_, e) =>
            {
                state.Group.SetDragging(true);
                touch.Height = 9999;
                touch.Width = 9999;
                DraggingThumb(e);
            };

            touch.PointerMoved += (_, e) =>
            {
                if (!state.Group.IsDragging.Value) return;
                UpdateFromPointer(e);
            };

            touch.PointerReleased += (_, e) =>
            {
                state.Group.SetDragging(false);
                e.Handled = true;

                // if (state.Group.IsHover.Value)
                // {
                //     touch.Width = Math.Max(thumb.Bounds.Width, track.Bounds.Width);
                //     touch.Height = Math.Max(thumb.Bounds.Height, track.Bounds.Height);
                // }
                // else
                // {
                //     touch.Height = 0;
                //     touch.Width = 0;
                // }
                touch.Width = Math.Max(thumb.Bounds.Width, track.Bounds.Width);
                touch.Height = Math.Max(thumb.Bounds.Height, track.Bounds.Height);
            };

            touch.PointerEntered += (_, _) =>
            {
                state.Group.SetHover(true);

                touch.Width = Math.Max(thumb.Bounds.Height, Math.Max(thumb.Bounds.Width, track.Bounds.Width));
                touch.Height = Math.Max(thumb.Bounds.Width, Math.Max(thumb.Bounds.Height, track.Bounds.Height));

                state.ApplyVariants();
            };

            touch.PointerExited += (_, _) =>
            {
                state.Group.SetHover(false);
                state.Reset();
            };
        };

        return new Element(uiScope, panel);

        void DraggingThumb(PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(panel).Properties.IsLeftButtonPressed)
                return;


            state.Group.SetDragging(true);
            UpdateFromPointer(e);
            e.Handled = true;
        }

        void UpdateFromPointer(PointerEventArgs e)
        {
            var p = e.GetPosition(panel);

            double ratio;

            if (state.CurrentStyle.Orientation is Orientation.Vertical)
            {
                ratio = panel.Bounds.Height <= 0
                    ? 0
                    : 1 - p.Y / panel.Bounds.Height;
            }
            else
            {
                ratio = panel.Bounds.Width <= 0
                    ? 0
                    : p.X / panel.Bounds.Width;
            }

            ratio = Math.Clamp(ratio, 0, 1);

            state.Value.RxValue = state.Min.RxValue + ratio * (state.Max.RxValue - state.Min.RxValue);

            // prop.BindValue?.RxValue = state.Value;

            prop.OnValueChanged?.Invoke(state.Value.Value);
            // UpdateThumb();
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

            if (styleSet.Orientation is Orientation.Vertical)
            {
                fill.HorizontalAlignment = HorizontalAlignment.Stretch;
                fill.VerticalAlignment = VerticalAlignment.Bottom;

                thumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                thumb.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                fill.HorizontalAlignment = HorizontalAlignment.Left;
                fill.VerticalAlignment = VerticalAlignment.Stretch;

                thumb.HorizontalAlignment = HorizontalAlignment.Left;
                thumb.VerticalAlignment = VerticalAlignment.Stretch;
            }

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