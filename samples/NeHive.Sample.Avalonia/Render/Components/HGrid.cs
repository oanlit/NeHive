using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

// 位置参数元组别名，便于阅读
using GridPosition = (int row, int column, int rowSpan, int colSpan);
using SimpleGridPosition = (int row, int column);

public readonly struct HgLen
{
    public readonly GridLength Value;

    private HgLen(GridLength value) => Value = value;

    // 隐式转换：从 GridLength 直接转换
    public static implicit operator HgLen(GridLength length) => new(length);

    // 隐式转换：从数值（像素）转换
    public static implicit operator HgLen(int pixels) => new(new GridLength(pixels, GridUnitType.Pixel));
    public static implicit operator HgLen(double pixels) => new(new GridLength(pixels, GridUnitType.Pixel));

    // 静态辅助方法（也可以放到单独的 Grid 类中）
    public static HgLen Auto => new(GridLength.Auto);
    public static HgLen Star(double value = 1) => new(new GridLength(value, GridUnitType.Star));
}

public class HGridStyle(
    Thickness? margin = null,
    double? columnSpacing = null,
    double? rowSpacing = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    IBrush? background = null
)
{
    // 边距
    public Thickness Margin { get; private set; } = margin ?? new(0);
    public double ColumnSpacing { get; private set; } = columnSpacing ?? 0;
    public double RowSpacing { get; private set; } = rowSpacing ?? 0;

    // 对齐
    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Stretch;

    public VerticalAlignment VerticalAlignment { get; private set; } =
        verticalAlignment ?? VerticalAlignment.Stretch;


    // 背景
    public IBrush? Background { get; private set; } = background;

    // 样式默认值
    public static HGridStyle Default => new();

    public HGridStyle Merge(HGridStyle style)
    {
        Margin = style.Margin;
        ColumnSpacing = style.ColumnSpacing;
        RowSpacing = style.RowSpacing;
        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;
        Background = style.Background ?? Background;
        return this;
    }

    public static Accessor<HGridStyle> Parse(Accessor<string> text)
        => new Computed<HGridStyle>(() => PureParse(text.Value));

    public static HGridStyle PureParse(string text)
    {
        // Thickness? margin = null;
        // double? columnSpacing = null;
        // double? rowSpacing = null;
        //
        // HorizontalAlignment? horizontalAlignment = null;
        // VerticalAlignment? verticalAlignment = null;
        // IBrush? background = null;
        //
        // var tokens = text.Split(
        //     [' ', '\n', '\r', '\t'],
        //     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //
        // foreach (var token in tokens)
        // {
        //     // 定位
        //
        //     // 外边距 m-10
        //     // if (token.StartsWith("m-"))
        //     // {
        //     //     var val = token["m-".Length..];
        //     //     if (double.TryParse(val, out var m)) margin = new Thickness(m);
        //     //     continue;
        //     // }
        //     //
        //     // // 间距 spacing-8
        //     // if (token.StartsWith("spacing-"))
        //     // {
        //     //     var val = token["spacing-".Length..];
        //     //     if (val.StartsWith("x-"))
        //     //     {
        //     //         val = val["x-".Length..];
        //     //         if (double.TryParse(val, out var s1))
        //     //         {
        //     //             rowSpacing = s1;
        //     //             continue;
        //     //         }
        //     //     }
        //     //
        //     //     if (val.StartsWith("y-"))
        //     //     {
        //     //         val = val["y-".Length..];
        //     //         if (double.TryParse(val, out var s1))
        //     //         {
        //     //             columnSpacing = s1;
        //     //             continue;
        //     //         }
        //     //     }
        //     //
        //     //     if (double.TryParse(val, out var s)) rowSpacing = columnSpacing = s;
        //     //     continue;
        //     // }
        //     //
        //     // if (token.StartsWith("gap-"))
        //     // {
        //     //     var val = token["gap-".Length..];
        //     //     if (val.StartsWith("x-"))
        //     //     {
        //     //         val = val["x-".Length..];
        //     //         if (double.TryParse(val, out var s1))
        //     //         {
        //     //             columnSpacing = s1;
        //     //             continue;
        //     //         }
        //     //     }
        //     //
        //     //     if (val.StartsWith("y-"))
        //     //     {
        //     //         val = val["y-".Length..];
        //     //         if (double.TryParse(val, out var s1))
        //     //         {
        //     //             rowSpacing = s1;
        //     //             continue;
        //     //         }
        //     //     }
        //     //
        //     //     if (double.TryParse(val, out var s)) rowSpacing = columnSpacing = s;
        //     //     continue;
        //     // }
        //     //
        //     // // 对齐
        //     // if (token == "center")
        //     // {
        //     //     horizontalAlignment = HorizontalAlignment.Center;
        //     //     continue;
        //     // }
        //     //
        //     // if (token == "left")
        //     // {
        //     //     horizontalAlignment = HorizontalAlignment.Left;
        //     //     continue;
        //     // }
        //     //
        //     // if (token == "right")
        //     // {
        //     //     horizontalAlignment = HorizontalAlignment.Right;
        //     //     continue;
        //     // }
        //     //
        //     // if (token == "stretch")
        //     // {
        //     //     horizontalAlignment = HorizontalAlignment.Stretch;
        //     //     continue;
        //     // }
        //     //
        //     // if (token == "top")
        //     // {
        //     //     verticalAlignment = VerticalAlignment.Top;
        //     //     continue;
        //     // }
        //     //
        //     // if (token == "bottom")
        //     // {
        //     //     verticalAlignment = VerticalAlignment.Bottom;
        //     //     continue;
        //     // }
        //     //
        //     // // 背景 bg-gray
        //     // if (token.StartsWith("bg-"))
        //     // {
        //     //     var color = token["bg-".Length..];
        //     //     background = color.ToLowerInvariant() switch
        //     //     {
        //     //         "white" => Brushes.White,
        //     //         "black" => Brushes.Black,
        //     //         "gray" => Brushes.LightGray,
        //     //         "lightgray" => Brushes.LightGray,
        //     //         "darkgray" => Brushes.DarkGray,
        //     //         _ => background
        //     //     };
        //     // }
        //
        //     // var result = StyleParser.Parse(text);
        //     // margin =  result.Margin;
        // }
        var result = StyleParser.Parse(text);
        return new HGridStyle(
            result.Margin,
            result.ColumnSpacing,
            result.RowSpacing,
            result.HorizontalAlignment,
            result.VerticalAlignment,
            result.Background
        );
    }
}

public class HGridProp : IEnumerable<KeyValuePair<GridPosition, IElement>>
{
    private readonly Dictionary<GridPosition, IElement> _children = new();

    // 布局属性（响应式）
    public readonly Accessor<IReadOnlyList<HgLen>>? RowDefinitions;
    public readonly Accessor<IReadOnlyList<HgLen>>? ColumnDefinitions;

    // 布局属性
    public readonly Accessor<HGridStyle>? Style;

    public HGridProp(
        Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
        Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
        Accessor<string>? strStyle = null,
        Accessor<HGridStyle>? style = null
    )
    {
        RowDefinitions = rowDefinitions;
        ColumnDefinitions = columnDefinitions;
        // 自动合并规则：strStyle → style 覆盖
        if (style != null && strStyle != null)
        {
            Style = new Computed<HGridStyle>(() =>
                HGridStyle.Parse(strStyle).Value.Merge(style.Value));
        }
        else if (strStyle != null)
        {
            Style = HGridStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }

    public IElement this[GridPosition key]
    {
        set => _children[key] = value;
    }

    public IElement this[SimpleGridPosition key]
    {
        set
        {
            GridPosition pos = (key.row, key.column, 1, 1);
            _children[pos] = value;
        }
    }

    public IEnumerator<KeyValuePair<GridPosition, IElement>> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    private static readonly Component<HGridProp> CompGrid = new((prop, uiScope) =>
    {
        var grid = new Grid();

        // 应用响应式属性
        uiScope.AddEffect(() =>
        {
            if (prop.RowDefinitions is not null)
            {
                grid.RowDefinitions.Clear();
                foreach (var rowDef in prop.RowDefinitions.Value)
                    grid.RowDefinitions.Add(new RowDefinition(rowDef.Value));
            }

            if (prop.ColumnDefinitions is not null)
            {
                grid.ColumnDefinitions.Clear();
                foreach (var colDef in prop.ColumnDefinitions.Value)
                    grid.ColumnDefinitions.Add(new ColumnDefinition(colDef.Value));
            }
        });

        uiScope.AddEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var style = epochScope.Track(prop.Style);

            grid.Margin = style.Margin;
            grid.ColumnSpacing = style.ColumnSpacing;
            grid.RowSpacing = style.RowSpacing;

            grid.HorizontalAlignment = style.HorizontalAlignment;
            grid.VerticalAlignment = style.VerticalAlignment;

            if (style.Background != null)
                grid.Background = style.Background;
        });

        // 添加子元素并应用附加属性
        foreach (var (position, childElement) in prop)
        {
            var child = childElement.Content; // 获取控件的根元素
            Grid.SetRow(child, position.row);
            Grid.SetColumn(child, position.column);
            Grid.SetRowSpan(child, position.rowSpan);
            Grid.SetColumnSpan(child, position.colSpan);
            grid.Children.Add(child);
        }

        return new Element(uiScope, grid);
    });

    public static IElement HGrid(HGridProp prop)
        => CompGrid.Create(prop);
}