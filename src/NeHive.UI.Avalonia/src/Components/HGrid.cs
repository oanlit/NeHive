using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

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
    {
        return new Computed<HGridStyle>(() =>
        {
            var str = text.RxValue;
            var result = StyleParser.Parse(str);
            return new HGridStyle(
                result.Margin,
                result.ColumnSpacing,
                result.RowSpacing,
                result.HorizontalAlignment,
                result.VerticalAlignment,
                result.Background
            );
        });
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
                HGridStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
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
        uiScope.CreateEffect(() =>
        {
            if (prop.RowDefinitions is not null)
            {
                grid.RowDefinitions.Clear();
                foreach (var rowDef in prop.RowDefinitions.RxValue)
                    grid.RowDefinitions.Add(new RowDefinition(rowDef.Value));
            }

            if (prop.ColumnDefinitions is not null)
            {
                grid.ColumnDefinitions.Clear();
                foreach (var colDef in prop.ColumnDefinitions.RxValue)
                    grid.ColumnDefinitions.Add(new ColumnDefinition(colDef.Value));
            }
        });

        uiScope.CreateEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var style = epochScope.Pull(prop.Style);

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