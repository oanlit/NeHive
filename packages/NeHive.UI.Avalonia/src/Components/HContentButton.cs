using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HContentButtonProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<RoutedEventArgs>? onClick = null)
{
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;
    public readonly Action<RoutedEventArgs>? OnClick = onClick;

    public IElement? Content { get; init; }
    public IElement? Flyout { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<HButtonExpose> HContentButton(
        HContentButtonProp prop)
    {
        UiScope uiScope = new();
        // 创建基础视觉元素

        var content = prop.Content?.Content ?? new Control();
        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = content
        };

        if (prop.Flyout is not null)
        {
            var flyout = new Flyout
            {
                Content = prop.Flyout
            };
            border.ContextFlyout = flyout;
        }

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, content, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);

        var expose = new HButtonExpose();
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

        return new Element<HButtonExpose>(uiScope, border, expose);

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            prop.OnClick?.Invoke(args);
            expose.Click.Invoke(args);
        }
    }

    public static IElement<HButtonExpose> HContentButton(
        out HButtonExpose expose,
        HContentButtonProp prop)
    {
        var el = HContentButton(prop);
        expose = el.Expose;
        return el;
    }
}