using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HPanelStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    double? spacing = null,
    OverflowHandle? overflowHandle = null,
    Orientation? orientation = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    IBrush? background = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public int? ZIndex { get; private set; } = zIndex;

    public double? Width { get; private set; } = width;
    public double? Height { get; private set; } = height;
    public double? MinWidth { get; private set; } = minWidth;
    public double? MaxWidth { get; private set; } = maxWidth;

    public double? MinHeight { get; private set; } = minHeight;
    public double? MaxHeight { get; private set; } = maxHeight;

    public OverflowHandle? OverflowHandle = overflowHandle;
    public Orientation Orientation { get; private set; } = orientation ?? Orientation.Vertical;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Stretch;
    public double Spacing { get; private set; } = spacing ?? 0;
    public IBrush? Background { get; private set; } = background;
    // 可以继续添加更多样式属性，例如 Shadow, Opacity 等

    public HPanelStyle Merge(HPanelStyle style)
    {
        Margin = style.Margin;
        ZIndex = style.ZIndex;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        Spacing = style.Spacing;

        OverflowHandle = style.OverflowHandle;
        Orientation = style.Orientation;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        Background = style.Background ?? Background;
        return this;
    }

    public static HPanelStyle Default => new();

    public static Accessor<HPanelStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HPanelStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HPanelStyle(
                result.Margin,
                result.ZIndex,
                result.Width,
                result.Height,
                result.MinWidth,
                result.MaxWidth,
                result.MinHeight,
                result.MaxHeight,
                result.ColumnSpacing,
                result.OverflowHandle,
                result.Orientation,
                result.HorizontalAlignment,
                result.VerticalAlignment,
                result.Background
            );
        });
    }
}

public class HStackPanelProp : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    // 布局属性
    public readonly Accessor<FullStyle>? Style;

    // public readonly Accessor<HPanelStyle>? Style = (style, strStyle) switch
    // {
    //     (not null, not null) => new Computed<HPanelStyle>(() =>
    //         HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
    //     (not null, _) => style,
    //     (_, not null) => HPanelStyle.Parse(strStyle),
    //     _ => null
    // };

    public HStackPanelProp(
        Accessor<string>? strStyle = null
    )
    {
        // 自动合并规则：strStyle → style 覆盖
        // if (style != null && strStyle != null)
        // {
        //     Style = new Computed<HPanelStyle>(() =>
        //         HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        // }
        // else if (strStyle != null)
        // {
        //     Style = HPanelStyle.Parse(strStyle);
        // }
        // else
        // {
        //     Style = style;
        // }

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
    public static IElement<StackPanel> HStackPanel(HStackPanelProp prop)
    {
        var uiScope = new UiScope();
        var stack = new StackPanel();
        var border = new Border
        {
            Child = stack
        };

        uiScope.CreateEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var style = epochScope.Track(prop.Style);
            ApplyStyle(style.Normal);
        });

        foreach (var child in prop)
            stack.Children.Add(child.Content);

        return new Element<StackPanel>(uiScope, border, stack);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, stack, border);

            var orientation = style.Orientation ?? Orientation.Vertical;
            stack.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Vertical:
                    if (style.RowSpacing is not null) stack.Spacing = style.RowSpacing.Value;
                    break;
                case Orientation.Horizontal:
                    if (style.ColumnSpacing is not null) stack.Spacing = style.ColumnSpacing.Value;
                    break;
            }

            var overflowHandle = style.OverflowHandle;
            if (overflowHandle is not null)
            {
                if (overflowHandle is OverflowHandle.Visible)
                    stack.ClipToBounds = false;
                else if (overflowHandle is OverflowHandle.Hidden)
                    stack.ClipToBounds = true;
            }

            if (style.Orientation is not null) stack.Orientation = style.Orientation.Value;
        }

        // void ApplyStyle(HPanelStyle style)
        // {
        //     stack.Margin = style.Margin;
        //     if (style.ZIndex is not null) stack.ZIndex = style.ZIndex.Value;
        //
        //     if (style.Width is not null) stack.Width = style.Width.Value;
        //     if (style.Height is not null) stack.Height = style.Height.Value;
        //     if (style.MinWidth is not null) stack.MinWidth = style.MinWidth.Value;
        //     if (style.MaxWidth is not null) stack.MaxWidth = style.MaxWidth.Value;
        //     if (style.MinHeight is not null) stack.MinHeight = style.MinHeight.Value;
        //     if (style.MaxHeight is not null) stack.MaxHeight = style.MaxHeight.Value;
        //     
        //     stack.Spacing = style.Spacing;
        //
        //     var overflowHandle = style.OverflowHandle;
        //     if (overflowHandle is not null)
        //     {
        //         if (overflowHandle is OverflowHandle.Visible)
        //             stack.ClipToBounds = false;
        //         else if (overflowHandle is OverflowHandle.Hidden)
        //             stack.ClipToBounds = true;
        //     }
        //
        //     stack.Orientation = style.Orientation;
        //     stack.HorizontalAlignment = style.HorizontalAlignment;
        //     stack.VerticalAlignment = style.VerticalAlignment;
        //
        //     stack.Background = style.Background;
        // }
    }
}