using System.Collections;
using Avalonia.Controls;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HTreeViewItemArgs(
    Accessor<string> header,
    Accessor<bool>? isExpanded = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null
)
{
    public readonly Accessor<string> Header = header;
    public readonly Accessor<bool> IsExpanded = isExpanded ?? false;
    public readonly List<HTreeViewItemArgs> Children = new();

    public void Add(HTreeViewItemArgs child) => Children.Add(child);
}

public class HTreeViewProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : IEnumerable<HTreeViewItemArgs>
{
    private readonly List<HTreeViewItemArgs> _roots = new();

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public void Add(HTreeViewItemArgs root) => _roots.Add(root);

    public IEnumerator<HTreeViewItemArgs> GetEnumerator() => _roots.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<TreeView> HTreeView(HTreeViewProp prop)
    {
        return Element<TreeView>.WithScope(uiScope =>
        {
            var treeView = new TreeView();
            var border = new Border
            {
                Child = treeView
            };

            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants
            };

            state.ApplyAccessorStyle(prop.Style, treeView, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(treeView, border, StyleUtil.ApplyStyle);

            // 递归构建 TreeViewItem
            TreeViewItem BuildItem(HTreeViewItemArgs itemArgs)
            {
                var tvi = new TreeViewItem();

                tvi.Header = itemArgs.Header.Value;
                if (itemArgs.Header.IsReactive)
                    uiScope.CreateEffect(epochScope => tvi.Header = epochScope.Track(itemArgs.Header));

                tvi.IsExpanded = itemArgs.IsExpanded.Value;
                if (itemArgs.IsExpanded.IsReactive)
                    uiScope.CreateEffect(epochScope => tvi.IsExpanded = epochScope.Track(itemArgs.IsExpanded));

                foreach (var childProp in itemArgs.Children)
                {
                    tvi.Items.Add(BuildItem(childProp));
                }

                return tvi;
            }

            foreach (var item in prop)
            {
                treeView.Items.Add(BuildItem(item));
            }

            return (treeView, border);
        });
    }
}