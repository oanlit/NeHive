using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HStackPanelProp(
    Accessor<Orientation>? orientation = null,
    Accessor<HorizontalAlignment>? horizontalAlignment = null,
    Accessor<VerticalAlignment>? verticalAlignment = null,
    Accessor<double>? spacing = null,
    Accessor<IBrush>? background = null,
    Accessor<Thickness>? margin = null
) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    // 布局属性
    public readonly Accessor<Orientation> Orientation =
        orientation ?? global::Avalonia.Layout.Orientation.Vertical;

    public readonly Accessor<HorizontalAlignment> HorizontalAlignment =
        horizontalAlignment ?? global::Avalonia.Layout.HorizontalAlignment.Stretch;

    public readonly Accessor<VerticalAlignment> VerticalAlignment =
        verticalAlignment ?? global::Avalonia.Layout.VerticalAlignment.Stretch;

    public readonly Accessor<double> Spacing = spacing ?? 0;

    // 样式属性
    public readonly Accessor<IBrush>? Background = background;

    public readonly Accessor<Thickness> Margin
        = margin ?? new Thickness(0);
    // 可以继续添加更多样式属性，例如 Shadow, Opacity 等

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
    private static readonly Component<HStackPanelProp> CompStackPanel = new((prop, uiScope) =>
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

        foreach (var child in prop)
            stack.Children.Add(child.Content);

        return new Element(uiScope, stack);
    });

    public static IElement HStackPanel(HStackPanelProp prop)
        => CompStackPanel.Create(prop);
}