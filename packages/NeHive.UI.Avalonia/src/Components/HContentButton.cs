using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using System.Collections;

namespace NeHive.UI.Avalonia.Components;

public class HContentButtonProp : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];
    public readonly Accessor<FullStyle>? Style;
    public readonly Action<RoutedEventArgs>? OnClick;
    
    public HContentButtonProp(
        Accessor<string>? strStyle = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        OnClick = onClick;
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }
    
    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
    {
        _children.Add(element);
    }
}

public static partial class BaseComponent
{
    public static IElement<HButtonExpose> HContentButton(
        HContentButtonProp prop)
    {
        UiScope uiScope = new();
        // 创建基础视觉元素

        var stack = new StackPanel();
        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = stack
        };

        HButtonState state;

        var style = prop.Style;
        if (style is null)
        {
            state = new HButtonState(HButtonStyle.DefaultStyleSet);
            ApplyStyle(state.CurrentStyle); // 应用默认样式
        }
        else
        {
            state = new HButtonState(style.Value.Normal);
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
        
        foreach (var child in prop)
            stack.Children.Add(child.Content);

        return new Element<HButtonExpose>(uiScope, border, expose);

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            prop.OnClick?.Invoke(args);
            expose.Click.Invoke(args);
        }

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, stack, border);
            
            var orientation = styleValue.Orientation ?? Orientation.Vertical;
            stack.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Vertical:
                    if (styleValue.RowSpacing is not null) stack.Spacing = styleValue.RowSpacing.Value;
                    break;
                case Orientation.Horizontal:
                    if (styleValue.ColumnSpacing is not null) stack.Spacing = styleValue.ColumnSpacing.Value;
                    break;
            }

            var overflowHandle = styleValue.OverflowHandle;
            if (overflowHandle is not null)
            {
                if (overflowHandle is OverflowHandle.Visible)
                    stack.ClipToBounds = false;
                else if (overflowHandle is OverflowHandle.Hidden)
                    stack.ClipToBounds = true;
            }

            if (styleValue.Orientation is not null) stack.Orientation = styleValue.Orientation.Value;
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