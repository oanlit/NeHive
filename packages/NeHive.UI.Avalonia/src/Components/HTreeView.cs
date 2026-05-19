using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// 树节点配置
/// </summary>
public class HTreeViewItemProp(
    Accessor<string> header,
    Accessor<bool>? isExpanded = null
)
{
    public readonly Accessor<string> Header = header;
    public readonly Accessor<bool> IsExpanded = isExpanded ?? false;
    public readonly List<HTreeViewItemProp> Children = new();

    // 添加子节点方法（方便集合初始化器）
    public void Add(HTreeViewItemProp child) => Children.Add(child);
}

/// <summary>
/// TreeView 配置
/// </summary>
public class HTreeViewProp : IEnumerable<HTreeViewItemProp>
{
    private readonly List<HTreeViewItemProp> _roots = new();

    public readonly Accessor<HPanelStyle>? Style;

    public HTreeViewProp(
        Accessor<string>? strStyle = null,
        Accessor<HPanelStyle>? style = null
    )
    {
        // 自动合并规则：strStyle → style 覆盖
        if (style != null && strStyle != null)
        {
            Style = new Computed<HPanelStyle>(() =>
                HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle != null)
        {
            Style = HPanelStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }

    // 集合初始化器支持
    public void Add(HTreeViewItemProp root) => _roots.Add(root);

    public IEnumerator<HTreeViewItemProp> GetEnumerator() => _roots.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    private static readonly Component<HTreeViewProp> CompTreeView = new((prop, uiScope) =>
    {
        var treeView = new TreeView();

        // 应用样式
        // uiScope.CreateEffect(() =>
        // {
        //     if (prop.Width is not null) treeView.Width = prop.Width.RxValue;
        //     if (prop.Height is not null) treeView.Height = prop.Height.RxValue;
        //     if (prop.Margin is not null) treeView.Margin = prop.Margin.RxValue;
        //     treeView.HorizontalAlignment = prop.HorizontalAlignment.RxValue;
        //     treeView.VerticalAlignment = prop.VerticalAlignment.RxValue;
        // });

        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                ApplyStyle(style);
            });
        }

        // 递归构建 TreeViewItem
        TreeViewItem BuildItem(HTreeViewItemProp itemProp)
        {
            var tvi = new TreeViewItem();
            uiScope.CreateEffect(() => tvi.Header = itemProp.Header.RxValue);
            uiScope.CreateEffect(() => tvi.IsExpanded = itemProp.IsExpanded.RxValue);
            foreach (var childProp in itemProp.Children)
            {
                tvi.Items.Add(BuildItem(childProp));
            }

            return tvi;
        }

        foreach (var item in prop)
        {
            treeView.Items.Add(BuildItem(item));
        }

        return new Element(uiScope, treeView);

        void ApplyStyle(HPanelStyle style)
        {
            treeView.Margin = style.Margin;
            if (style.ZIndex is not null) treeView.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) treeView.Width = style.Width.Value;
            if (style.Height is not null) treeView.Height = style.Height.Value;
            if (style.MinWidth is not null) treeView.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) treeView.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) treeView.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) treeView.MaxHeight = style.MaxHeight.Value;

            treeView.HorizontalAlignment = style.HorizontalAlignment;
            treeView.VerticalAlignment = style.VerticalAlignment;

            treeView.Background = style.Background;
        }
    });

    public static IElement HTreeView(HTreeViewProp prop) => CompTreeView.Create(prop);
}