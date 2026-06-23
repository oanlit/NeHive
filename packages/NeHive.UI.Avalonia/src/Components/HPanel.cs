using System.Collections;

using Avalonia.Controls;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HPanelProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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
    public static IElement<Panel> HPanel(HPanelProp prop)
    {
        var uiScope = new UiScope();
        var panel = new Panel();
        var border = new Border
        {
            Child = panel
        };

        foreach (var child in prop)
            panel.Children.Add(child.Content);
        
        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, panel, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(panel, border, StyleUtil.ApplyStyle);

        return new Element<Panel>(uiScope, border, panel);
    }
}