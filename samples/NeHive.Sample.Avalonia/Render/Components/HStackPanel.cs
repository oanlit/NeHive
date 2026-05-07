using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HPanelStyle(
    Thickness? margin = null,
    Orientation? orientation = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    double? spacing = null,
    IBrush? background = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public Orientation Orientation { get; private set; } = orientation ?? Orientation.Vertical;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } = verticalAlignment ?? VerticalAlignment.Stretch;
    public double Spacing { get; private set; } = spacing ?? 0;
    public IBrush? Background { get; private set; } = background;
    // 可以继续添加更多样式属性，例如 Shadow, Opacity 等

    public HPanelStyle Merge(HPanelStyle style)
    {
        Margin = style.Margin;
        Orientation = style.Orientation;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;
        Spacing = style.Spacing;
        Background = style.Background ?? Background;
        return this;
    }

    public static HPanelStyle Default => new();

    public static Accessor<HPanelStyle> Parse(Accessor<string> text)
        => new Computed<HPanelStyle>(() => PureParse(text.Value));

    public static HPanelStyle PureParse(string text)
    {
        // Thickness? margin = null;
        // Orientation? orientation = null;
        // HorizontalAlignment? horizontalAlignment = null;
        // VerticalAlignment? verticalAlignment = null;
        // double? spacing = null;
        // IBrush? background = null;
        //
        // var tokens = text.Split(
        //     [' ', '\n', '\r', '\t'],
        //     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //
        // foreach (var token in tokens)
        // {
        //     // 方向
        //     if (token == "vertical")
        //     {
        //         orientation = Orientation.Vertical;
        //         continue;
        //     }
        //
        //     if (token == "horizontal")
        //     {
        //         orientation = Orientation.Horizontal;
        //         continue;
        //     }
        //
        //     // 对齐
        //     if (token == "center")
        //     {
        //         horizontalAlignment = HorizontalAlignment.Center;
        //         continue;
        //     }
        //
        //     if (token == "left")
        //     {
        //         horizontalAlignment = HorizontalAlignment.Left;
        //         continue;
        //     }
        //
        //     if (token == "right")
        //     {
        //         horizontalAlignment = HorizontalAlignment.Right;
        //         continue;
        //     }
        //
        //     if (token == "stretch")
        //     {
        //         horizontalAlignment = HorizontalAlignment.Stretch;
        //         continue;
        //     }
        //
        //     if (token == "top")
        //     {
        //         verticalAlignment = VerticalAlignment.Top;
        //         continue;
        //     }
        //
        //     if (token == "bottom")
        //     {
        //         verticalAlignment = VerticalAlignment.Bottom;
        //         continue;
        //     }
        //
        //     // 间距 spacing-8
        //     if (token.StartsWith("spacing-"))
        //     {
        //         var val = token["spacing-".Length..];
        //         if (double.TryParse(val, out var s)) spacing = s;
        //         continue;
        //     }
        //
        //     if (token.StartsWith("gap-"))
        //     {
        //         var val = token["gap-".Length..];
        //         if (double.TryParse(val, out var s)) spacing = s;
        //         continue;
        //     }
        //
        //     // 外边距 m-10
        //     if (token.StartsWith("m-"))
        //     {
        //         var val = token["m-".Length..];
        //         if (double.TryParse(val, out var m)) margin = new Thickness(m);
        //         continue;
        //     }
        //
        //     // 背景 bg-gray
        //     if (token.StartsWith("bg-"))
        //     {
        //         var color = token["bg-".Length..];
        //         background = color.ToLowerInvariant() switch
        //         {
        //             "white" => Brushes.White,
        //             "black" => Brushes.Black,
        //             "gray" => Brushes.LightGray,
        //             "lightgray" => Brushes.LightGray,
        //             "darkgray" => Brushes.DarkGray,
        //             _ => background
        //         };
        //     }
        // }
        
        var result = StyleParser.Parse(text);

        return new HPanelStyle(
            result.Margin,
            result.Orientation,
            result.HorizontalAlignment,
            result.VerticalAlignment,
            result.ColumnSpacing,
            result.Background
        );
    }
}

public class HStackPanelProp: ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    // 布局属性
    public readonly Accessor<HPanelStyle>? Style;
    
    public HStackPanelProp(
        Accessor<string>? strStyle = null,
        Accessor<HPanelStyle>? style = null
    )
    {
        // 自动合并规则：strStyle → style 覆盖
        if (style != null && strStyle != null)
        {
            Style = new Computed<HPanelStyle>(() =>
                HPanelStyle.Parse(strStyle).Value.Merge(style.Value));
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

        uiScope.AddEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var style = epochScope.Track(prop.Style);

            stack.Orientation = style.Orientation;
            stack.Spacing = style.Spacing;
            stack.HorizontalAlignment = style.HorizontalAlignment;
            stack.VerticalAlignment = style.VerticalAlignment;
            stack.Margin = style.Margin;
            stack.Background = style.Background;
        });
        
        foreach (var child in prop)
            stack.Children.Add(child.Content);

        return new Element(uiScope, stack);
    });

    public static IElement HStackPanel(HStackPanelProp prop)
        => CompStackPanel.Create(prop);
}