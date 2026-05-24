using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

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

public class HTreeViewProp : IEnumerable<HTreeViewItemProp>
{
    private readonly List<HTreeViewItemProp> _roots = new();

    public readonly Accessor<FullStyle>? Style;

    public HTreeViewProp(
        Accessor<string>? strStyle = null
    )
    {
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

    // 集合初始化器支持
    public void Add(HTreeViewItemProp root) => _roots.Add(root);

    public IEnumerator<HTreeViewItemProp> GetEnumerator() => _roots.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<TreeView> HTreeView(HTreeViewProp prop)
    {
        var uiScope = new UiScope();
        var treeView = new TreeView();
        var border = new Border
        {
            Child = treeView
        };

        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal, treeView, border);
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

        return new Element<TreeView>(uiScope, border, treeView);
    }
}