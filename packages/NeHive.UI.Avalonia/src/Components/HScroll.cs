using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HScrollStyle(
    Thickness? margin = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? minHeight = null,
    double? maxWidth = null,
    double? maxHeight = null,
    Orientation? orientation = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    Thickness? padding = null,
    bool? isVisible = null,
    double? spacing = null,
    IBrush? background = null,
    IBrush? borderBrush = null,
    Thickness? borderThickness = null,
    double? opacity = null,
    CornerRadius? cornerRadius = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public double? Width { get; private set; } = width;
    public double? Height { get; private set; } = height;
    public double? MinWidth { get; private set; } = minWidth;
    public double? MinHeight { get; private set; } = minHeight;
    public double? MaxWidth { get; private set; } = maxWidth;
    public double? MaxHeight { get; private set; } = maxHeight;
    public Orientation Orientation { get; private set; } = orientation ?? Orientation.Vertical;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } =
        verticalAlignment ?? VerticalAlignment.Stretch;

    public Thickness Padding { get; private set; } = padding ?? new Thickness(0);

    public bool? IsVisible { get; private set; } = isVisible;
    public double Spacing { get; private set; } = spacing ?? 0;
    public IBrush? Background { get; private set; } = background;
    public IBrush? BorderBrush { get; private set; } = borderBrush;

    public Thickness BorderThickness { get; private set; } = borderThickness ?? new Thickness(0);

    public double? Opacity { get; private set; } = opacity;
    public CornerRadius? CornerRadius { get; private set; } = cornerRadius;

    public HScrollStyle Merge(HScrollStyle style)
    {
        Margin = style.Margin;
        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MinHeight = style.MinHeight;
        MaxWidth = style.MaxWidth;
        MaxHeight = style.MaxHeight;
        Orientation = style.Orientation;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;
        Padding = style.Padding;
        IsVisible = style.IsVisible;
        Spacing = style.Spacing;
        Background = style.Background ?? Background;
        BorderBrush = style.BorderBrush;
        BorderThickness = style.BorderThickness;
        Opacity = style.Opacity;
        CornerRadius = style.CornerRadius;
        return this;
    }

    public static HScrollStyle Default => new();

    public static Accessor<HScrollStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HScrollStyle>(() =>
        {
            var str = text.RxValue;

            StyleParser.Parse(str, ref result);
            return new HScrollStyle(
                result.Margin,
                result.Width,
                result.Height,
                result.MinWidth,
                result.MinHeight,
                result.MaxWidth,
                result.MaxHeight,
                result.Orientation,
                result.HorizontalAlignment,
                result.VerticalAlignment,
                result.Padding,
                result.IsVisible,
                result.ColumnSpacing,
                result.Background,
                result.BorderBrush
            );
        });
    }
}

public class HScrollProp : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<ScrollBarVisibility> HorizontalScrollBarVisibility;
    public readonly Accessor<ScrollBarVisibility> VerticalScrollBarVisibility;
    // public readonly Accessor<bool>? AllowInertia;

    public readonly Accessor<FullStyle>? Style;

    /// <param name="horizontalScrollBarVisibility">水平滚动条可见策略</param>
    /// <param name="verticalScrollBarVisibility">垂直滚动条可见策略</param>
    /// <param name="strStyle">字符串样式，如 "m-2 p-2 bg-lightgray rounded-lg"</param>
    public HScrollProp(
        Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
        Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
        // Accessor<bool>? allowInertia = null, // 是否允许惯性滚动（触摸屏）
        Accessor<string>? strStyle = null)
    {
        HorizontalScrollBarVisibility = horizontalScrollBarVisibility ?? ScrollBarVisibility.Auto;
        VerticalScrollBarVisibility = verticalScrollBarVisibility ?? ScrollBarVisibility.Auto;
        // AllowInertia = allowInertia;

        // 样式合并规则：strStyle -> style
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

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

        if (prop.Style is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyStyle(style.Normal);
            });
        }

        foreach (var child in prop)
            stack.Children.Add(child.Content);

        uiScope.OnMount += () =>
        {
            scroll.Content = stack;
            scroll.ScrollToHome();
        };

        expose = new HScrollExpose(scroll);

        return new Element<StackPanel>(uiScope, border, stack);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, stack, border);

            if (style.Padding is not null)
            {
                border.Padding = new Thickness(0);
                stack.Margin = style.Padding.Value;
            }

            var orientation = style.Orientation ?? Orientation.Vertical;
            stack.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    if (style.ColumnSpacing is not null) stack.Spacing = style.ColumnSpacing.Value;
                    scroll.HorizontalScrollBarVisibility =
                        prop.HorizontalScrollBarVisibility.Value;
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    break;

                case Orientation.Vertical:
                    if (style.RowSpacing is not null) stack.Spacing = style.RowSpacing.Value;
                    scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scroll.VerticalScrollBarVisibility =
                        prop.VerticalScrollBarVisibility.Value;
                    break;
            }

            var overflowHandle = style.OverflowHandle;
            if (overflowHandle is  null) return;
            
            if (overflowHandle is OverflowHandle.Visible)
                stack.ClipToBounds = false;
            else if (overflowHandle is OverflowHandle.Hidden)
                stack.ClipToBounds = true;
        }
    }

    public static IElement<StackPanel> HScrollViewer(HScrollProp prop)
    {
        return HScrollViewer(out _, prop);
    }
}