using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;

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

// public class HAbsoluteProp(
//     Accessor<double>? width = null,
//     Accessor<double>? height = null,
//     Accessor<IBrush>? background = null,
//     Accessor<Thickness>? margin = null,
//     Accessor<HorizontalAlignment>? horizontalAlignment = null,
//     Accessor<VerticalAlignment>? verticalAlignment = null
// ) : IEnumerable<KeyValuePair<AbsPosition, IElement>>
public class HAbsoluteProp: IEnumerable<KeyValuePair<AbsPosition, IElement>>
{
    private readonly Dictionary<AbsPosition, IElement> _children = new();
    
    public readonly Accessor<HPanelStyle>? Style;
    
    public HAbsoluteProp(
        Accessor<string>? strStyle = null,
        Accessor<HPanelStyle>? style = null
    )
    {
        // 自动合并规则：strStyle → style 覆盖
        if (style != null && strStyle != null)
        {
            Style = new Computed<HPanelStyle>(() =>
                HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
        }
        else if (strStyle != null)
        {
            Style = HPanelStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
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

        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                ApplyStyle(style);
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

        return new Element(uiScope, canvas);

        void ApplyStyle(HPanelStyle style)
        {
            canvas.Margin = style.Margin;
            if (style.ZIndex is not null) canvas.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) canvas.Width = style.Width.Value;
            if (style.Height is not null) canvas.Height = style.Height.Value;
            if (style.MinWidth is not null) canvas.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) canvas.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) canvas.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) canvas.MaxHeight = style.MaxHeight.Value;

            canvas.HorizontalAlignment = style.HorizontalAlignment;
            canvas.VerticalAlignment = style.VerticalAlignment;

            canvas.Background = style.Background;
        }
    });

    public static IElement HAbsolute(HAbsoluteProp prop) => CompAbsolute.Create(prop);
}