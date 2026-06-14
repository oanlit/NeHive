using System.Collections;
using Avalonia.Controls;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

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

public class HTreeViewProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : IEnumerable<HTreeViewItemProp>
{
    private readonly List<HTreeViewItemProp> _roots = new();

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, treeView, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(treeView, border, StyleUtil.ApplyStyle);

        // 递归构建 TreeViewItem
        TreeViewItem BuildItem(HTreeViewItemProp itemProp)
        {
            var tvi = new TreeViewItem();

            tvi.Header = itemProp.Header.Value;
            if(itemProp.Header.IsReactive)
                uiScope.CreateEffect(epochScope => tvi.Header = epochScope.Track(itemProp.Header));

            tvi.IsExpanded = itemProp.IsExpanded.Value;
            if(itemProp.IsExpanded.IsReactive)
                uiScope.CreateEffect(epochScope => tvi.IsExpanded = epochScope.Track(itemProp.IsExpanded));
            
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