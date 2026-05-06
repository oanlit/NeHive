using Avalonia.Controls;

namespace NeHive.Sample.Avalonia.Render.Components;

public static partial class BaseComponent
{
    public static IElement HChildren(params IEnumerable<IElement> children)
    {
        var uiScope = new UiScope();
        var stack = new StackPanel();
        foreach (var el in children)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    }
}