using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class AbsPosition(
    Accessor<double>? left = null,
    Accessor<double>? top = null,
    Accessor<double>? right = null,
    Accessor<double>? bottom = null
)
{
    public readonly Accessor<double>? Left = left;
    public readonly Accessor<double>? Top = top;
    public readonly Accessor<double>? Right = right;
    public readonly Accessor<double>? Bottom = bottom;
}

public class HAbsoluteProp : IEnumerable<KeyValuePair<AbsPosition, IElement>>
{
    private readonly Dictionary<AbsPosition, IElement> _children = new();

    public readonly Accessor<FullStyle>? Style;

    public HAbsoluteProp(
        Accessor<string>? strStyle = null
    )
    {
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

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
        
        var border = new Border
        {
            Child = canvas
        };

        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal,canvas, border);
            });
        }

        foreach (var (pos, element) in prop)
        {
            var control = element.Content;
            uiScope.CreateEffect(() =>
            {
                if (pos.Left is not null) Canvas.SetLeft(control, pos.Left.RxValue);
                if (pos.Top is not null) Canvas.SetTop(control, pos.Top.RxValue);
                if (pos.Right is not null) Canvas.SetRight(control, pos.Right.RxValue);
                if (pos.Bottom is not null) Canvas.SetBottom(control, pos.Bottom.RxValue);
            });

            canvas.Children.Add(control);
        }

        return new Element(uiScope, border);
    });

    public static IElement HAbsolute(HAbsoluteProp prop) => CompAbsolute.Create(prop);
}