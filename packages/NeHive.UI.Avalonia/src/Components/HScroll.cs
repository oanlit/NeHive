using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HScrollProp(
    Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
    Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
    // Accessor<bool>? allowInertia = null, // 是否允许惯性滚动（触摸屏）
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<ScrollBarVisibility> HorizontalScrollBarVisibility =
        horizontalScrollBarVisibility ?? ScrollBarVisibility.Auto;

    public readonly Accessor<ScrollBarVisibility> VerticalScrollBarVisibility =
        verticalScrollBarVisibility ?? ScrollBarVisibility.Auto;
    // public readonly Accessor<bool>? AllowInertia;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    // 支持初始化器语法：{ 子元素 }
    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
    {
        _children.Add(element);
    }
}

public class HScrollExpose(ScrollViewer scroll)
{
    public void ScrollToHome()
        => scroll.ScrollToHome();

    public void ScrollToEnd()
        => scroll.ScrollToEnd();
}

public static partial class BaseComponent
{
    public static IElement<StackPanel> HScrollViewer(out HScrollExpose expose, HScrollProp prop)
    {
        var uiScope = new UiScope();
        var scroll = new ScrollViewer();
        var stack = new StackPanel();

        var border = new Border
        {
            Child = scroll
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, stack, border, ApplyStyle);
        state.ApplyVariantsStyle(stack, border, ApplyStyle);

        foreach (var child in prop)
            stack.Children.Add(child.Content);

        uiScope.OnMount += () =>
        {
            scroll.Content = stack;
            scroll.ScrollToHome();
        };

        expose = new HScrollExpose(scroll);

        return new Element<StackPanel>(uiScope, border, stack);

        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, layout, bord);

            if (styleValue.Padding is not null)
            {
                border.Padding = new Thickness(0);
                stack.Margin = styleValue.Padding.Value;
            }

            var orientation = styleValue.Orientation ?? Orientation.Vertical;
            stack.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    if (styleValue.ColumnSpacing is not null) stack.Spacing = styleValue.ColumnSpacing.Value;
                    scroll.HorizontalScrollBarVisibility =
                        prop.HorizontalScrollBarVisibility.Value;
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    break;

                case Orientation.Vertical:
                    if (styleValue.RowSpacing is not null) stack.Spacing = styleValue.RowSpacing.Value;
                    scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scroll.VerticalScrollBarVisibility =
                        prop.VerticalScrollBarVisibility.Value;
                    break;
            }
        }
    }

    public static IElement<StackPanel> HScrollViewer(HScrollProp prop)
    {
        return HScrollViewer(out _, prop);
    }
}