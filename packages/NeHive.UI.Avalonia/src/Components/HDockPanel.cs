using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HDockPanelProp : IEnumerable<(Dock Dock, IElement Element)>
{
    private readonly List<(Dock Dock, IElement Element)> _children = [];
    public readonly Accessor<bool> LastChildFill;
    public readonly Accessor<FullStyle>? Style;

    public HDockPanelProp(
        Accessor<bool>? lastChildFill = null,
        Accessor<string>? strStyle = null
    )
    {
        LastChildFill = lastChildFill ?? true;
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
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
        var border = new Border
        {
            Child = dockPanel
        };

        // 应用样式
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyStyle(style.Normal);
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

        return new Element<DockPanel>(uiScope, border, dockPanel);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, dockPanel, border);
            if (style.Width is not null)
                dockPanel.Width = style.Width.Value;

            if (style.Height is not null)
                dockPanel.Height = style.Height.Value;

            if (style.MinWidth is not null)
                dockPanel.MinWidth = style.MinWidth.Value;

            if (style.MaxWidth is not null)
                dockPanel.MaxWidth = style.MaxWidth.Value;

            if (style.MinHeight is not null)
                dockPanel.MinHeight = style.MinHeight.Value;

            if (style.MaxHeight is not null)
                dockPanel.MaxHeight = style.MaxHeight.Value;

            if (style.RowSpacing is not null) dockPanel.HorizontalSpacing = style.RowSpacing.Value;
            if (style.ColumnSpacing is not null) dockPanel.VerticalSpacing = style.ColumnSpacing.Value;
        }
    }
}