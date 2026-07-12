using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static class HButtonStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontSize = 12,
        Foreground = Brushes.Black,
        Background = Brushes.LightGray,
        FontWeight = FontWeight.Normal,
        BorderThickness = new Thickness(0),
        CornerRadius = new CornerRadius(0),
        Opacity = 1.0,
        IsVisible = true
    };
}

public class HButtonExpose
{
    public readonly Action<RoutedEventArgs> Click = _ => { };
}

public static partial class BaseComponent
{
    public static IElement<HButtonExpose> HButton(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        var el = HButton(out _, text, strStyle, style, variants, onClick);
        return el;
    }
    
    public static IElement<HButtonExpose> HButton(
        out HButtonExpose exp,
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        var expose = new HButtonExpose();
        exp = expose;
        return Element<HButtonExpose>.WithScope(uiScope =>
        {
            text ??= "";
            var styleAccessor = StyleParser.ParseFull(strStyle, HButtonStyle.DefaultStyleSet, style);

            // 创建基础视觉元素
            var textBlock = new TextBlock();
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = textBlock
            };

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                StrVariants = styleAccessor.Value.Variants,
                Variants = variants
            };

            state.ApplyAccessorStyle(styleAccessor, textBlock, border, ApplyStyle);
            state.ApplyVariantsStyle(textBlock, border, ApplyStyle);

            textBlock.Text = text.Value;
            if (text.IsReactive)
                uiScope.CreateEffect(() => textBlock.Text = text.RxValue);

            // 事件挂载
            uiScope.OnMount += () =>
            {
                border.PointerReleased += (_, e) =>
                {
                    if (border.IsPointerOver)
                    {
                        RaiseClick();
                    }

                    e.Handled = true;
                };
            };

            return (expose, border);

            // 统一触发点击的方法
            void RaiseClick()
            {
                var args = new RoutedEventArgs(Button.ClickEvent);
                onClick?.Invoke(args);
                expose.Click.Invoke(args);
            }

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, textBlock, bord);

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
        });
    }
}