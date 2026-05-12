using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render.Styles;

namespace NeHive.Sample.Avalonia.Render.Components;

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
        return new Computed<HScrollStyle>(() =>
        {
            var str = text.Value;
            var result = StyleParser.Parse(str);
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

/// <summary>
/// ScrollViewer 配置属性（内容通过初始化器传入）
/// </summary>
public class HScrollProp : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    // 滚动行为参数（与 Element 无关）
    public readonly Accessor<ScrollBarVisibility> HorizontalScrollBarVisibility;
    public readonly Accessor<ScrollBarVisibility> VerticalScrollBarVisibility;
    // public readonly Accessor<bool>? AllowInertia;

    // 样式（外观）—— 直接使用 StyleSet，通过 strStyle 或单独样式对象传入
    public readonly Accessor<HScrollStyle>? Style;

    /// <param name="horizontalScrollBarVisibility">水平滚动条可见策略</param>
    /// <param name="verticalScrollBarVisibility">垂直滚动条可见策略</param>
    /// <param name="strStyle">字符串样式，如 "m-2 p-2 bg-lightgray rounded-lg"</param>
    /// <param name="style">强类型样式对象（StyleSet）</param>
    public HScrollProp(
        Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
        Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
        // Accessor<bool>? allowInertia = null, // 是否允许惯性滚动（触摸屏）
        Accessor<string>? strStyle = null,
        Accessor<HScrollStyle>? style = null)
    {
        HorizontalScrollBarVisibility = horizontalScrollBarVisibility ?? ScrollBarVisibility.Auto;
        VerticalScrollBarVisibility = verticalScrollBarVisibility ?? ScrollBarVisibility.Auto;
        // AllowInertia = allowInertia;

        // 样式合并规则：strStyle -> style
        if (style != null && strStyle != null)
        {
            Style = new Computed<HScrollStyle>(() =>
            {
                var parsed = HScrollStyle.Parse(strStyle.Value);
                return parsed.Value.Merge(style.Value);
            });
        }
        else if (strStyle != null)
        {
            Style = HScrollStyle.Parse(strStyle.Value);
        }
        else
        {
            Style = style;
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

public static partial class BaseComponent
{
    private static readonly Component<HScrollProp> CompScrollViewer = new((prop, uiScope) =>
    {
        var scroll = new ScrollViewer();
        
        // 设置唯一的内容
        var stack = new StackPanel();

        uiScope.CreateEffect(scope =>
        {
            if (prop.Style == null) return;
            var style = scope.Pull(prop.Style);

            var ori = style.Orientation;
            if (ori == Orientation.Horizontal)
            {
                scroll.HorizontalScrollBarVisibility =
                    prop.HorizontalScrollBarVisibility.Value;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else // Vertical
            {
                scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scroll.VerticalScrollBarVisibility =
                    prop.VerticalScrollBarVisibility.Value;
            }

            scroll.Margin = style.Margin;
            // scroll.Padding = style.Padding;
            stack.Margin = style.Padding;
            scroll.Background = style.Background;
            scroll.BorderBrush = style.BorderBrush;
            scroll.BorderThickness = style.BorderThickness;
            if (style.CornerRadius.HasValue)
                scroll.CornerRadius = style.CornerRadius.Value;
            if (style.Width.HasValue)
            {
                scroll.Width = style.Width.Value;
                // stack.Width = style.Width.Value;
            }
            if (style.Height.HasValue)
            {
                scroll.Height = style.Height.Value;
                // stack.Height = style.Height.Value;
            }
            if (style.MinWidth.HasValue)
            {
                scroll.MinWidth = style.MinWidth.Value;
                // stack.MinWidth = style.MinWidth.Value;
            }
            if (style.MaxWidth.HasValue)
            {
                scroll.MaxWidth = style.MaxWidth.Value;
                // stack.MaxWidth = style.MaxWidth.Value;
            }
            if (style.MinHeight.HasValue)
            {
                scroll.MinHeight = style.MinHeight.Value;
                // stack.MinHeight = style.MinHeight.Value;
            }
            if (style.MaxHeight.HasValue)
            {
                scroll.MaxHeight = style.MaxHeight.Value;
                // stack.MaxHeight = style.MaxHeight.Value;
            }
            if (style.Opacity.HasValue)
                scroll.Opacity = style.Opacity.Value;
            if (style.IsVisible.HasValue)
                scroll.IsVisible = style.IsVisible.Value;
            // Cursor 和 Effect 按需添加
        });
        
        foreach (var child in prop)
            stack.Children.Add(child.Content);

        scroll.Content = stack;

        return new Element(uiScope, scroll);
    });

    /// <summary>
    /// 创建滚动容器。示例：
    /// <code>
    /// HScrollViewer(new(orientation: Orientation.Vertical, strStyle: "m-4 p-2 bg-lightgray rounded-lg border-slate-500 border-w-1"))
    /// {
    ///     HTextBlock(longText, strStyle: "text-base")
    /// }
    /// </code>
    /// </summary>
    public static IElement HScrollViewer(HScrollProp prop)
        => CompScrollViewer.Create(prop);
}