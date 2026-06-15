using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct ShowProp(Accessor<bool> when)
{
    public readonly Accessor<bool> When = when;
    public required Func<IElement> IfTrue { get; init; }
    public Func<IElement>? IfFalse { get; init; }
}

public static partial class ControlFlow
{
    private static Element ShowComp(ShowProp prop, UiScope uiScope)
    {
        var panel = new Panel();

        uiScope.CreateEffect(epochScope =>
        {
            var when = epochScope.Track(prop.When);
            IElement child;
            if (when)
            {
                child = prop.IfTrue();
            }
            else
            {
                if (prop.IfFalse is null) return;
                child = prop.IfFalse();
            }
            
            var content = child.Content;
            panel.Children.Add(content);
            epochScope.OnCleanup += child.Dispose;
        });
        return new Element(uiScope, panel);
    }

    public static IElement Show(ShowProp prop)
        => Element.WithScope(ShowComp, prop);
}