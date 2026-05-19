using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// DockPanel 样式配置
/// </summary>
public class HDockPanelStyle(
    Thickness? margin = null,
    int? zIndex = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    double? columnSpacing = null,
    double? rowSpacing = null,
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

    public double ColumnSpacing { get; private set; } = columnSpacing ?? 0;
    public double RowSpacing { get; private set; } = rowSpacing ?? 0;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Stretch;
    public IBrush? Background { get; private set; } = background;

    public static HDockPanelStyle Default => new();

    public static Accessor<HDockPanelStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HDockPanelStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HDockPanelStyle(
                margin: result.Margin,
                width: result.Width,
                height: result.Height,
                minWidth: result.MinWidth,
                maxWidth: result.MaxWidth,
                minHeight: result.MinHeight,
                maxHeight: result.MaxHeight,
                columnSpacing: result.ColumnSpacing,
                rowSpacing: result.RowSpacing,
                horizontalAlignment: result.HorizontalAlignment,
                verticalAlignment: result.VerticalAlignment,
                background: result.Background
            );
        });
    }

    public HDockPanelStyle Merge(HDockPanelStyle style)
    {
        Margin = style.Margin;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        Background = style.Background ?? Background;
        return this;
    }
}

/// <summary>
/// DockPanel 配置类（构造参数仅样式）
/// </summary>
public class HDockPanelProp : IEnumerable<(Dock Dock, IElement Element)>
{
    private readonly List<(Dock Dock, IElement Element)> _children = [];
    public readonly Accessor<bool> LastChildFill = true;
    public readonly Accessor<HDockPanelStyle>? Style;

    public HDockPanelProp(
        Accessor<bool>? lastChildFill = null,
        Accessor<string>? strStyle = null,
        Accessor<HDockPanelStyle>? style = null
    )
    {
        if (style != null && strStyle != null)
        {
            LastChildFill = lastChildFill ?? true;
            Style = new Computed<HDockPanelStyle>(() =>
                HDockPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle != null)
        {
            Style = HDockPanelStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }

    // 集合初始化器支持：添加子元素并指定停靠方向
    public IElement this[Dock key]
    {
        set => _children.Add((key, value));
    }

    public void Add(IElement element, Dock dock) => _children.Add((dock, element));

    public IEnumerator<(Dock Dock, IElement Element)> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 DockPanel 组件
    /// </summary>
    public static IElement<DockPanel> HDockPanel(HDockPanelProp prop)
    {
        var uiScope = new UiScope();
        var dockPanel = new DockPanel();

        // 应用样式
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyDockPanelStyle(dockPanel, style);
            });
        }

        // 添加子元素并设置 Dock 附加属性
        // var lastItem = prop.LastOrDefault();
        foreach (var (dock, element) in prop)
        {
            var control = element.Content;
            DockPanel.SetDock(control, dock);
            dockPanel.Children.Add(control);
        }

        uiScope.CreateEffect(() => dockPanel.LastChildFill = prop.LastChildFill.RxValue);

        return new Element<DockPanel>(uiScope, dockPanel, dockPanel);

        void ApplyDockPanelStyle(DockPanel panel, HDockPanelStyle style)
        {
            panel.Margin = style.Margin;
            if (style.ZIndex is not null) panel.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) panel.Width = style.Width.Value;
            if (style.Height is not null) panel.Height = style.Height.Value;
            if (style.MinWidth is not null) panel.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) panel.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) panel.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) panel.MaxHeight = style.MaxHeight.Value;

            panel.HorizontalSpacing = style.RowSpacing;
            panel.VerticalSpacing = style.ColumnSpacing;

            panel.HorizontalAlignment = style.HorizontalAlignment;
            panel.VerticalAlignment = style.VerticalAlignment;

            if (style.Background != null) panel.Background = style.Background;
        }
    }
}