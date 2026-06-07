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
            var style = prop.Style.Value;
            StyleUtil.ApplyStyle(style.Normal, canvas, border);

            if (prop.Style.IsReactive)
            {
                uiScope.CreateEffect(epochScope =>
                {
                    var style2 = epochScope.Track(prop.Style);
                    StyleUtil.ApplyStyle(style2.Normal, canvas, border);
                });
            }
        }
        else
        {
            StyleUtil.ApplyStyle(StyleUtil.FromDefault(), canvas, border);
        }

        foreach (var (pos, element) in prop)
        {
            var control = element.Content;
            SetPos(control, pos.Left?.Value, pos.Top?.Value, pos.Right?.Value, pos.Bottom?.Value);

            if (pos.Left?.IsReactive is true ||
                pos.Top?.IsReactive is true ||
                pos.Right?.IsReactive is true ||
                pos.Bottom?.IsReactive is true
               )
                uiScope.CreateEffect(() =>
                {
                    SetPos(control, pos.Left?.RxValue, pos.Top?.RxValue, pos.Right?.RxValue, pos.Bottom?.RxValue);
                });

            canvas.Children.Add(control);
        }

        return new Element(uiScope, border);

        void SetPos(Control control, double? left, double? top, double? right, double? bottom)
        {
            if (left is not null) Canvas.SetLeft(control, left.Value);
            if (top is not null) Canvas.SetTop(control, top.Value);
            if (right is not null) Canvas.SetRight(control, right.Value);
            if (bottom is not null) Canvas.SetBottom(control, bottom.Value);
        }
    });

    public static IElement HAbsolute(HAbsoluteProp prop) => CompAbsolute.Create(prop);
}