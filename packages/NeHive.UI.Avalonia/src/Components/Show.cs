using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
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
    public static IElement Show(ShowProp prop)
    {
        return Element.WithScope(uiScope =>
        {
            var panel = new Panel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            uiScope.CreateEffect(epochScope =>
            {
                var when = epochScope.Track(prop.When);
                IElement child;
                if (when)
                {
                    using (new ScopeFrame(uiScope))
                    {
                        child = prop.IfTrue();
                        _ =  child.Content;
                    }
                }
                else
                {
                    if (prop.IfFalse is null) return;
                    using (new ScopeFrame(uiScope))
                    {
                        child = prop.IfFalse();
                        _ = child.Content;
                    }
                }

                var content = child.Content;
                panel.Children.Add(content);
                epochScope.OnCleanup += child.Dispose;
            });
            return panel;
        });
    }
}