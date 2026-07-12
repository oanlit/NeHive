using System.Collections;
using NeHive.Model;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HContextProp(Action<IContextSetter> contextSetter) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];
    internal readonly Action<IContextSetter> ContextSetter = contextSetter;

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
    public static IElement HContext(HContextProp prop)
    {
        return Element.WithScope(uiScope =>
        {
            prop.ContextSetter(uiScope);
            return ElementUtil.WrapSingleContainerContent(prop);
        });
    }
}