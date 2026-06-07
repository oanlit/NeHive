using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static class HButtonStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        Padding = new Thickness(8, 4),
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontSize = 12,
        Foreground = Brushes.Black,
        Background = Brushes.LightGray,
        BorderBrush = Brushes.Gray,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(4)
    };
}

public class HButtonExpose
{
    public Action<RoutedEventArgs> Click = _ => { };
}

public static partial class BaseComponent
{
    private class HButtonState(StyleSet baseStyle)
    {
        public StyleSet BaseStyle = baseStyle;
        public StyleSet CurrentStyle = StyleUtil.Copy(baseStyle);
        public bool CurrentIsBase { get; private set; } = true;
        public Dictionary<string, List<string>>? Variants;

        // 鼠标交互状态（悬停、按下等）
        public bool IsHover;
        public bool IsClicked;

        public void ResetSetStyle()
        {
            if(CurrentIsBase) return;
            CurrentStyle.Merge(BaseStyle);
            CurrentIsBase = true;
        }

        public void SetCurrentStyle()
        {
            if (Variants == null) return;
            if (IsHover && Variants.TryGetValue("hover", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
                CurrentIsBase = false;
            }

            if (IsClicked && Variants.TryGetValue("click", out strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
                CurrentIsBase = false;
            }
        }

        public void SetHoverStyle()
        {
            if (Variants == null) return;
            if (IsHover && Variants.TryGetValue("hover", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
                CurrentIsBase = false;
            }
        }

        public void SetClickStyle()
        {
            if (Variants == null) return;
            if (IsClicked && Variants.TryGetValue("click", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
                CurrentIsBase = false;
            }
        }
    }

    public static IElement<HButtonExpose> HButton(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        text ??= "";
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        UiScope uiScope = new();

        // 创建基础视觉元素

        var textBlock = new TextBlock();
        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = textBlock
        };

        textBlock.Text = text.Value;
        if (text.IsReactive)
            uiScope.CreateEffect(() => textBlock.Text = text.RxValue);

        HButtonState state;

        if (style is null)
        {
            state = new HButtonState(HButtonStyle.DefaultStyleSet);
            ApplyStyle(state.CurrentStyle); // 应用默认样式
        }
        else
        {
            state = new HButtonState(style.Value.Normal);
            ApplyStyle(state.CurrentStyle);

            if (style.IsReactive)
            {
                uiScope.CreateEffect(epochScope =>
                {
                    var styleValue = epochScope.Track(style);
                    state.BaseStyle = styleValue.Normal;
                    state.Variants = styleValue.Variants;
                    state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
                    ApplyStyle(state.CurrentStyle);
                });
            }
        }

        var expose = new HButtonExpose();
        // 事件挂载
        uiScope.OnMount += () =>
        {
            border.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                    return;

                state.IsClicked = true;
                state.SetClickStyle();
                ApplyStyle(state.CurrentStyle);
                e.Handled = true;
            };

            border.PointerReleased += (_, e) =>
            {
                if (state.IsClicked && border.IsPointerOver)
                {
                    RaiseClick();
                }

                state.IsClicked = false;
                state.ResetSetStyle();
                state.SetCurrentStyle();
                ApplyStyle(state.CurrentStyle);
                e.Handled = true;
            };

            border.PointerExited += (_, _) =>
            {
                state.IsHover = false;
                state.IsClicked = false; // 移出区域时取消按下状态
                state.ResetSetStyle();
                ApplyStyle(state.CurrentStyle);
            };

            border.PointerEntered += (_, _) =>
            {
                state.IsHover = true;
                state.SetHoverStyle();
                ApplyStyle(state.CurrentStyle);
            };
        };

        return new Element<HButtonExpose>(uiScope, border, expose);

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            onClick?.Invoke(args);
            expose.Click.Invoke(args);
        }

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, textBlock, border);

            if (styleValue.TextAlignment is not null) textBlock.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.VerticalTextAlignment is not null)
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.TextWrapping is not null) textBlock.TextWrapping = styleValue.TextWrapping.Value;
            if (styleValue.Foreground is not null) textBlock.Foreground = styleValue.Foreground;

            if (styleValue.FontSize is not null) textBlock.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) textBlock.Foreground = styleValue.Foreground;
        }
    }

    public static IElement<HButtonExpose> HButton(
        out HButtonExpose expose,
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null,
        Action<RoutedEventArgs>? click = null)
    {
        var el = HButton(text, strStyle, style, click);
        expose = el.Expose;
        return el;
    }
}