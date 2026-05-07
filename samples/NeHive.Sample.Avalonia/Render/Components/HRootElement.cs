using Avalonia.Controls;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public static partial class BaseComponent
{
    public static IElement RootElement(SingleChildrenProp prop)
    {
        var scope = Scope.CurrentScope;
        if (scope is not UiScope uiScope)
            throw new InvalidOperationException("CurrentScope is not UiScope");

        var stack = new StackPanel();
        foreach (var el in prop)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    }

    public static IElement RootElement(HStackPanelProp prop, UiScope uiScope)
    {
        var stack = new StackPanel();
        uiScope.AddEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var s = epochScope.Track(prop.Style);

            stack.Orientation = s.Orientation;
            stack.Spacing = s.Spacing;
            stack.HorizontalAlignment = s.HorizontalAlignment;
            stack.VerticalAlignment = s.VerticalAlignment;
            stack.Margin = s.Margin;
            stack.Background = s.Background;
        });

        foreach (var el in prop)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    }
}