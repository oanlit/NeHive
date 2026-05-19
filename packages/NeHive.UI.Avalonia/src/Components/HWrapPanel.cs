using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// WrapPanel 样式配置
/// </summary>
public class HWrapPanelStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    double? rowSpacing = null,
    double? columnSpacing = null,
    Orientation? orientation = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    IBrush? background = null
)
{
    public Thickness Margin { get; set; } = margin ?? new Thickness(0);
    public int? ZIndex { get; set; } = zIndex;

    public double? Width { get; set; } = width;
    public double? Height { get; set; } = height;
    public double? MinWidth { get; set; } = minWidth;
    public double? MaxWidth { get; set; } = maxWidth;
    public double? MinHeight { get; set; } = minHeight;
    public double? MaxHeight { get; set; } = maxHeight;

    public double? RowSpacing { get; set; } = rowSpacing;
    public double? ColumnSpacing { get; set; } = columnSpacing;

    public HorizontalAlignment HorizontalAlignment { get; set; } = horizontalAlignment ?? HorizontalAlignment.Stretch;
    public VerticalAlignment VerticalAlignment { get; set; } = verticalAlignment ?? VerticalAlignment.Stretch;
    public IBrush? Background { get; set; } = background;
    public Orientation Orientation { get; set; } = orientation ?? Orientation.Horizontal;

    public static HWrapPanelStyle Default => new();

    public HWrapPanelStyle Merge(HWrapPanelStyle style)
    {
        Margin = style.Margin;
        ZIndex = style.ZIndex;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        ColumnSpacing = style.ColumnSpacing;
        RowSpacing = style.RowSpacing;

        Orientation = style.Orientation;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        Background = style.Background ?? Background;
        return this;
    }

    public static Accessor<HWrapPanelStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HWrapPanelStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HWrapPanelStyle(
                margin: result.Margin,
                zIndex: result.ZIndex,
                width: result.Width,
                height: result.Height,
                minWidth: result.MinWidth,
                maxWidth: result.MaxWidth,
                minHeight: result.MinHeight,
                maxHeight: result.MaxHeight,
                rowSpacing: result.RowSpacing,
                columnSpacing: result.ColumnSpacing,
                orientation: result.Orientation,
                horizontalAlignment: result.HorizontalAlignment,
                verticalAlignment: result.VerticalAlignment,
                background: result.Background
            );
        });
    }
}

/// <summary>
/// WrapPanel 配置类（构造参数仅样式/布局属性）
/// </summary>
public class HWrapPanelProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];
    public readonly Accessor<HWrapPanelStyle>? Style;
    public readonly Accessor<double>? ItemWidth;
    public readonly Accessor<double>? ItemHeight;

    public HWrapPanelProp(
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<string>? strStyle = null,
        Accessor<HWrapPanelStyle>? style = null
    )
    {
        ItemWidth = itemWidth;
        ItemHeight = itemHeight;

        if (style != null && strStyle != null)
        {
            Style = new Computed<HWrapPanelStyle>(() =>
                HWrapPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle != null)
        {
            Style = HWrapPanelStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }

    // 集合初始化器添加子元素
    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 WrapPanel 组件（自动换行面板）
    /// </summary>
    public static IElement<WrapPanel> HWrapPanel(HWrapPanelProp prop)
    {
        var uiScope = new UiScope();
        var wrapPanel = new WrapPanel();

        // 应用样式
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyWrapPanelStyle(wrapPanel, style);
            });
        }

        if (prop.ItemWidth is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var itemWidth = scope.Track(prop.ItemWidth);
                wrapPanel.ItemWidth = itemWidth;
            });
        }

        if (prop.ItemHeight is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var itemHeight = scope.Track(prop.ItemHeight);
                wrapPanel.ItemHeight = itemHeight;
            });
        }

        // 添加子元素
        foreach (var child in prop)
        {
            wrapPanel.Children.Add(child.Content);
        }

        return new Element<WrapPanel>(uiScope, wrapPanel, wrapPanel);

        void ApplyWrapPanelStyle(WrapPanel panel, HWrapPanelStyle style)
        {
            panel.Margin = style.Margin;
            if (style.ZIndex is not null) panel.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) panel.Width = style.Width.Value;
            if (style.Height is not null) panel.Height = style.Height.Value;
            if (style.MinWidth is not null) panel.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) panel.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) panel.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) panel.MaxHeight = style.MaxHeight.Value;

            var orientation = style.Orientation;
            panel.Orientation = orientation;
            if (orientation is Orientation.Horizontal)
            {
                if (style.RowSpacing is not null) panel.ItemSpacing = style.RowSpacing.Value;
                if (style.ColumnSpacing is not null) panel.LineSpacing = style.ColumnSpacing.Value;
            }
            else
            {
                if (style.ColumnSpacing is not null) panel.ItemSpacing = style.ColumnSpacing.Value;
                if (style.RowSpacing is not null) panel.LineSpacing = style.RowSpacing.Value;
            }

            panel.HorizontalAlignment = style.HorizontalAlignment;
            panel.VerticalAlignment = style.VerticalAlignment;

            if (style.Background != null) panel.Background = style.Background;
        }
    }
}