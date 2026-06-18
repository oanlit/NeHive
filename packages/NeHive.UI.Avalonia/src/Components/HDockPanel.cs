using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HDockPanelProp(
    bool? lastChildFill = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
) : IEnumerable<(Dock? Dock, IElement Element)>
{
    private readonly List<(Dock? Dock, IElement Element)> _children = [];
    public readonly bool LastChildFill = lastChildFill ?? true;
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    // 集合初始化器支持：添加子元素并指定停靠方向
    public IElement this[Dock? key]
    {
        set => _children.Add((key, value));
    }

    public void Add(IElement element, Dock dock) => _children.Add((dock, element));

    public IEnumerator<(Dock? Dock, IElement Element)> GetEnumerator() => _children.GetEnumerator();
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
        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, dockPanel, border, ApplyStyle);
        state.ApplyVariantsStyle(dockPanel, border, ApplyStyle);

        // 添加子元素并设置 Dock 附加属性
        Control? lastItem = null;
        foreach (var (dock, element) in prop)
        {
            var control = element.Content;
            if (dock is null)
            {
                lastItem = control;
                continue;
            }

            DockPanel.SetDock(control, dock.Value);
            dockPanel.Children.Add(control);
        }

        dockPanel.LastChildFill = prop.LastChildFill;
        if(lastItem is not null) dockPanel.Children.Add(lastItem);

        return new Element<DockPanel>(uiScope, border, dockPanel);

        void ApplyStyle(StyleSet style, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(style, layout, bord);
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