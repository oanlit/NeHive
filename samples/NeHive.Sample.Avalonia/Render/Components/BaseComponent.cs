using System.Collections;

namespace NeHive.Sample.Avalonia.Render.Components;

public interface ISingleChildrenProp : IEnumerable<IElement>
{
    public void Add(IElement element);
}

public readonly struct SingleChildrenProp() : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

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
    public static readonly Component Empty = new(() => Element.Empty);
}