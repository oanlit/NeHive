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
        uiScope.AddEffect(() =>
        {
            stack.Orientation = prop.Orientation.Value;
            stack.Spacing = prop.Spacing.Value;
            stack.HorizontalAlignment = prop.HorizontalAlignment.Value;
            stack.VerticalAlignment = prop.VerticalAlignment.Value;
        });

        uiScope.AddEffect(() =>
        {
            if (prop.Background?.Value != null)
                stack.Background = prop.Background.Value;
            stack.Margin = prop.Margin.Value;
        });

        foreach (var el in prop)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    }
}