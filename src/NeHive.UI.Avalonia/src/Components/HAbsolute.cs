using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;

namespace NeHive.UI.Avalonia.Components;

public class AbsPosition(
    Accessor<double>? left = null,
    Accessor<double>? top = null,
    Accessor<double>? right = null,
    Accessor<double>? bottom = null,
    Accessor<int>? zIndex = null
)
{
    public readonly Accessor<double>? Left = left;
    public readonly Accessor<double>? Top = top;
    public readonly Accessor<double>? Right = right;
    public readonly Accessor<double>? Bottom = bottom;
    public readonly Accessor<int>? ZIndex = zIndex;
}

public class HAbsoluteProp(
    Accessor<double>? width = null,
    Accessor<double>? height = null,
    Accessor<IBrush>? background = null,
    Accessor<Thickness>? margin = null,
    Accessor<HorizontalAlignment>? horizontalAlignment = null,
    Accessor<VerticalAlignment>? verticalAlignment = null
) : IEnumerable<KeyValuePair<AbsPosition, IElement>>
{
    private readonly Dictionary<AbsPosition, IElement> _children = new();

    public readonly Accessor<double>? Width = width;
    public readonly Accessor<double>? Height = height;
    public readonly Accessor<IBrush>? Background = background;
    public readonly Accessor<Thickness>? Margin = margin;

    public readonly Accessor<HorizontalAlignment> HorizontalAlignment =
        horizontalAlignment ?? global::Avalonia.Layout.HorizontalAlignment.Stretch;

    public readonly Accessor<VerticalAlignment> VerticalAlignment =
        verticalAlignment ?? global::Avalonia.Layout.VerticalAlignment.Stretch;

    // 添加子元素的便捷方法
    public IElement this[AbsPosition key]
    {
        set => _children[key] = value;
    }

    public IEnumerator<KeyValuePair<AbsPosition, IElement>> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    private static readonly Component<HAbsoluteProp> CompAbsolute = new((prop, uiScope) =>
    {
        var canvas = new Canvas();

        uiScope.CreateEffect(() =>
        {
            if (prop.Width is not null) canvas.Width = prop.Width.RxValue;
            if (prop.Height is not null) canvas.Height = prop.Height.RxValue;
            if (prop.Background is not null) canvas.Background = prop.Background.RxValue;
            if (prop.Margin is not null) canvas.Margin = prop.Margin.RxValue;
            canvas.HorizontalAlignment = prop.HorizontalAlignment.RxValue;
            canvas.VerticalAlignment = prop.VerticalAlignment.RxValue;
        });

        foreach (var (pos, element) in prop)
        {
            var control = element.Content;
            uiScope.CreateEffect(() =>
            {
                if (pos.Left is not null) Canvas.SetLeft(control, pos.Left.RxValue);
                if (pos.Top is not null) Canvas.SetTop(control, pos.Top.RxValue);
                if (pos.Right is not null) Canvas.SetRight(control, pos.Right.RxValue);
                if (pos.Bottom is not null) Canvas.SetBottom(control, pos.Bottom.RxValue);
                // if (pos.ZIndex is not null) Canvas.SetZIndex(control, pos.ZIndex.RxValue);
            });

            canvas.Children.Add(control);
        }

        return new Element(uiScope, canvas);
    });

    public static IElement HAbsolute(HAbsoluteProp prop) => CompAbsolute.Create(prop);
}