using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

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

public class HGridProp(
    Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
    Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
) : IEnumerable<KeyValuePair<GridPosition, IElement>>
{
    private readonly Dictionary<GridPosition, IElement> _children = new();

    // 布局属性（响应式）
    public readonly Accessor<IReadOnlyList<HgLen>>? RowDefinitions = rowDefinitions;
    public readonly Accessor<IReadOnlyList<HgLen>>? ColumnDefinitions = columnDefinitions;

    // 布局属性
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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
    public static IElement HGrid(HGridProp prop)
    {
        var uiScope = new UiScope();

        var grid = new Grid();

        var border = new Border
        {
            Child = grid
        };
        
        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, grid, border, ApplyStyle);
        state.ApplyVariantsStyle(grid, border, ApplyStyle);

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

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet style, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(style, layout, bord);
            if (style.Width is not null)
                grid.Width = style.Width.Value;

            if (style.Height is not null)
                grid.Height = style.Height.Value;

            if (style.MinWidth is not null)
                grid.MinWidth = style.MinWidth.Value;

            if (style.MaxWidth is not null)
                grid.MaxWidth = style.MaxWidth.Value;

            if (style.MinHeight is not null)
                grid.MinHeight = style.MinHeight.Value;

            if (style.MaxHeight is not null)
                grid.MaxHeight = style.MaxHeight.Value;

            if (style.ColumnSpacing is not null) grid.ColumnSpacing = style.ColumnSpacing.Value;
            if (style.RowSpacing is not null) grid.RowSpacing = style.RowSpacing.Value;
        }
    }
}