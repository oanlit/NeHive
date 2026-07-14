using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HScrollProp(
    Accessor<bool>? isAllowAutoHide = null,
    Accessor<bool>? isBringIntoViewOnFocusChange = null,
    Accessor<bool>? isDeferredScrollingEnabled = null,
    Accessor<bool>? isScrollChainingEnabled = null,
    Accessor<bool>? isScrollInertiaEnabled = null,
    Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
    Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
    Accessor<SnapPointsType>? horizontalSnapPointsType = null,
    Accessor<SnapPointsType>? verticalSnapPointsType = null,
    Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
    Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsAllowAutoHide = isAllowAutoHide;
    public readonly Accessor<bool>? IsBringIntoViewOnFocusChange = isBringIntoViewOnFocusChange;
    public readonly Accessor<bool>? IsDeferredScrollingEnabled = isDeferredScrollingEnabled;
    public readonly Accessor<bool>? IsScrollChainingEnabled = isScrollChainingEnabled;
    public readonly Accessor<bool>? IsScrollInertiaEnabled = isScrollInertiaEnabled;

    public readonly Accessor<ScrollBarVisibility>? HorizontalScrollBarVisibility = horizontalScrollBarVisibility;
    public readonly Accessor<ScrollBarVisibility>? VerticalScrollBarVisibility = verticalScrollBarVisibility;
    public readonly Accessor<SnapPointsType>? HorizontalSnapPointsType = horizontalSnapPointsType;
    public readonly Accessor<SnapPointsType>? VerticalSnapPointsType = verticalSnapPointsType;
    public readonly Accessor<SnapPointsAlignment>? HorizontalSnapPointsAlignment = horizontalSnapPointsAlignment;
    public readonly Accessor<SnapPointsAlignment>? VerticalSnapPointsAlignment = verticalSnapPointsAlignment;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
        => _children.Add(element);
}

public class HScrollExpose(ScrollViewer scroll)
{
    public Size Extent => scroll.Extent;
    public Size Viewport => scroll.Viewport;

    public Vector Offset
    {
        get => scroll.Offset;
        set => scroll.Offset = value;
    }

    public void LineUp()
        => scroll.LineUp();

    public void LineDown()
        => scroll.LineDown();

    public void LineLeft()
        => scroll.LineLeft();

    public void LineRight()
        => scroll.LineRight();

    public void PageUp()
        => scroll.PageUp();

    public void PageDown()
        => scroll.PageDown();

    public void PageLeft()
        => scroll.PageLeft();

    public void PageRight()
        => scroll.PageRight();

    public void ScrollToHome()
        => scroll.ScrollToHome();

    public void ScrollToEnd()
        => scroll.ScrollToEnd();
}

public static partial class BaseComponent
{
    public static IElement<StackPanel> HScrollViewer(out HScrollExpose expose, HScrollProp prop)
    {
        var scroll = new ScrollViewer();
        expose = new HScrollExpose(scroll);
        var s = new ScrollContentPresenter();
        return Element<StackPanel>.WithScope(uiScope =>
        {
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

            if (prop.IsAllowAutoHide is not null)
            {
                scroll.AllowAutoHide = prop.IsAllowAutoHide.Value;
                if (prop.IsAllowAutoHide.IsReactive)
                    uiScope.CreateEffect(scope => scroll.AllowAutoHide = scope.Track(prop.IsAllowAutoHide));
            }

            if (prop.IsBringIntoViewOnFocusChange is not null)
            {
                scroll.BringIntoViewOnFocusChange = prop.IsBringIntoViewOnFocusChange.Value;
                if (prop.IsBringIntoViewOnFocusChange.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.BringIntoViewOnFocusChange = scope.Track(prop.IsBringIntoViewOnFocusChange));
            }

            if (prop.IsDeferredScrollingEnabled is not null)
            {
                scroll.IsDeferredScrollingEnabled = prop.IsDeferredScrollingEnabled.Value;
                if (prop.IsDeferredScrollingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsDeferredScrollingEnabled = scope.Track(prop.IsDeferredScrollingEnabled));
            }

            if (prop.IsScrollChainingEnabled is not null)
            {
                scroll.IsScrollChainingEnabled = prop.IsScrollChainingEnabled.Value;
                if (prop.IsScrollChainingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsScrollChainingEnabled = scope.Track(prop.IsScrollChainingEnabled));
            }

            if (prop.IsScrollInertiaEnabled is not null)
            {
                scroll.IsScrollInertiaEnabled = prop.IsScrollInertiaEnabled.Value;
                if (prop.IsScrollInertiaEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsScrollInertiaEnabled = scope.Track(prop.IsScrollInertiaEnabled));
            }

            if (prop.HorizontalScrollBarVisibility is not null)
            {
                scroll.HorizontalScrollBarVisibility = prop.HorizontalScrollBarVisibility.Value;
                if (prop.HorizontalScrollBarVisibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalScrollBarVisibility = scope.Track(prop.HorizontalScrollBarVisibility));
            }

            if (prop.VerticalScrollBarVisibility is not null)
            {
                scroll.VerticalScrollBarVisibility = prop.VerticalScrollBarVisibility.Value;
                if (prop.VerticalScrollBarVisibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalScrollBarVisibility = scope.Track(prop.VerticalScrollBarVisibility));
            }

            if (prop.HorizontalSnapPointsType is not null)
            {
                scroll.HorizontalSnapPointsType = prop.HorizontalSnapPointsType.Value;
                if (prop.HorizontalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalSnapPointsType = scope.Track(prop.HorizontalSnapPointsType));
            }

            if (prop.VerticalSnapPointsType is not null)
            {
                scroll.VerticalSnapPointsType = prop.VerticalSnapPointsType.Value;
                if (prop.VerticalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalSnapPointsType = scope.Track(prop.VerticalSnapPointsType));
            }

            if (prop.HorizontalSnapPointsAlignment is not null)
            {
                scroll.HorizontalSnapPointsAlignment = prop.HorizontalSnapPointsAlignment.Value;
                if (prop.HorizontalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalSnapPointsAlignment = scope.Track(prop.HorizontalSnapPointsAlignment));
            }

            if (prop.VerticalSnapPointsAlignment is not null)
            {
                scroll.VerticalSnapPointsAlignment = prop.VerticalSnapPointsAlignment.Value;
                if (prop.VerticalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalSnapPointsAlignment = scope.Track(prop.VerticalSnapPointsAlignment));
            }

            uiScope.OnMount += () =>
            {
                scroll.Content = stack;
                scroll.ScrollToHome();
            };

            return (stack, border);

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
                        break;

                    case Orientation.Vertical:
                        if (styleValue.RowSpacing is not null) stack.Spacing = styleValue.RowSpacing.Value;
                        break;
                }
            }
        });
    }

    public static IElement<StackPanel> HScrollViewer(HScrollProp prop)
    {
        return HScrollViewer(out _, prop);
    }
}