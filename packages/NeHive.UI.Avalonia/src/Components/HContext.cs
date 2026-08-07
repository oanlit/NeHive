using System.Collections;
using NeHive.Model;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HContextArgs(Action<IContextSetter> contextSetter) : ISingleChildrenArgs
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
    public static IElement HContext(HContextArgs args)
    {
        return Element.WithScope(uiScope =>
        {
            args.ContextSetter(uiScope);
            return ElementUtil.WrapSingleContainerContent(args);
        });
    }
}