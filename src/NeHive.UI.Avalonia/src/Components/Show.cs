using Avalonia.Controls;
using NeHive.Core;

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

            if (!when)
            {
                if (prop.IfFalse is null) return;
                var fallback = prop.IfFalse();
                panel.Children.Add(fallback.Content);
                epochScope.OnDispose(fallback.Dispose);
                return;
            }

            var child = prop.IfTrue();
            panel.Children.Add(child.Content);
            epochScope.OnDispose(child.Dispose);
        });
        return new Element(uiScope, panel);
    }

    public static IElement Show(ShowProp prop)
        => Element.WithScope(ShowComp, prop);
}