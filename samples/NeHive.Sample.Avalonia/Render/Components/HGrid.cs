using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

// 位置参数元组别名，便于阅读
using GridPosition = (int Row, int Column, int RowSpan, int ColSpan);
using SimpleGridPosition = (int Row, int Column);

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

public class HGridProp(
    Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
    Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
    Accessor<double>? rowSpacing = null,
    Accessor<double>? columnSpacing = null,
    Accessor<HorizontalAlignment>? horizontalAlignment = null,
    Accessor<VerticalAlignment>? verticalAlignment = null,
    Accessor<Thickness>? margin = null,
    Accessor<IBrush>? background = null
)
    : IEnumerable<KeyValuePair<GridPosition, IElement>>
{
    private readonly Dictionary<GridPosition, IElement> _children = new();

    // 布局属性（响应式）
    public readonly Accessor<IReadOnlyList<HgLen>>? RowDefinitions = rowDefinitions;
    public readonly Accessor<IReadOnlyList<HgLen>>? ColumnDefinitions = columnDefinitions;
    public readonly Accessor<double> RowSpacing = rowSpacing ?? 0;
    public readonly Accessor<double> ColumnSpacing = columnSpacing ?? 0;

    // 对齐与样式
    public readonly Accessor<HorizontalAlignment> HorizontalAlignment =
        horizontalAlignment ?? global::Avalonia.Layout.HorizontalAlignment.Stretch;

    public readonly Accessor<VerticalAlignment> VerticalAlignment =
        verticalAlignment ?? global::Avalonia.Layout.VerticalAlignment.Stretch;

    public readonly Accessor<Thickness> Margin =
        margin ?? new Thickness(0);

    public readonly Accessor<IBrush>? Background = background;


    // 添加子元素（支持初始化器语法）
    public void Add(GridPosition position, IElement childElement)
    {
        _children.Add(position, childElement);
    }

    public void Add(SimpleGridPosition position, IElement childElement)
    {
        _children.Add((position.Row, position.Column, 1, 1), childElement);
    }

    public IElement this[GridPosition key]
    {
        set => _children[key] = value;
    }

    public IElement this[SimpleGridPosition key]
    {
        set
        {
            GridPosition pos = (key.Row, key.Column, 1, 1);
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
            // 处理行定义（字符串格式如 "Auto,*,100"）
            // if (prop.RowDefinitions is not null)
            // {
            //     var rowDefs = new RowDefinitions();
            //     var lengths = ParseGridLengths(prop.RowDefinitions.Value);
            //     foreach (var length in lengths)
            //     {
            //         rowDefs.Add(new RowDefinition(length));
            //     }
            //
            //     grid.RowDefinitions = rowDefs;
            // }
            //
            // if (prop.ColumnDefinitions is not null)
            // {
            //     var colDefs = new ColumnDefinitions();
            //     var lengths = ParseGridLengths(prop.ColumnDefinitions.Value);
            //     foreach (var length in lengths)
            //     {
            //         colDefs.Add(new ColumnDefinition(length));
            //     }
            //
            //     grid.ColumnDefinitions = colDefs;
            // }
            
            // 设置行定义
            if (prop.RowDefinitions is not null)
            {
                grid.RowDefinitions.Clear();
                foreach (var rowDef in prop.RowDefinitions.Value)
                    grid.RowDefinitions.Add(new RowDefinition(rowDef.Value));
            }

            // 设置列定义
            if (prop.ColumnDefinitions is not null)
            {
                grid.ColumnDefinitions.Clear();
                foreach (var colDef in prop.ColumnDefinitions.Value)
                    grid.ColumnDefinitions.Add(new ColumnDefinition(colDef.Value));
            }

            grid.RowSpacing = prop.RowSpacing.Value;
            grid.ColumnSpacing = prop.ColumnSpacing.Value;
            grid.HorizontalAlignment = prop.HorizontalAlignment.Value;
            grid.VerticalAlignment = prop.VerticalAlignment.Value;
        });

        uiScope.AddEffect(() =>
        {
            if (prop.Background?.Value != null)
                grid.Background = prop.Background.Value;
            grid.Margin = prop.Margin.Value;
        });

        // 添加子元素并应用附加属性
        foreach (var (position, childElement) in prop)
        {
            var child = childElement.Content; // 获取控件的根元素
            Grid.SetRow(child, position.Row);
            Grid.SetColumn(child, position.Column);
            Grid.SetRowSpan(child, position.RowSpan);
            Grid.SetColumnSpan(child, position.ColSpan);
            grid.Children.Add(child);
        }

        return new Element(uiScope, grid);
    });

    public static IElement HGrid(HGridProp prop)
        => CompGrid.Create(prop);
}
