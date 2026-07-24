using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
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

public class HGridProps(Scope scope, Border border, Grid grid) : BaseComponentProps(scope, border, grid);
public class HGridArgs(
    Accessor<bool>? showGridLines = null,
    Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
    Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null
) : IEnumerable<KeyValuePair<GridPosition, IElement>>
{
    private readonly Dictionary<GridPosition, IElement> _children = new();

    public readonly Accessor<bool>? ShowGridLines = showGridLines;
    public readonly Accessor<IReadOnlyList<HgLen>>? RowDefinitions = rowDefinitions;
    public readonly Accessor<IReadOnlyList<HgLen>>? ColumnDefinitions = columnDefinitions;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

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
    public static IElement HGrid(Func<HGridProps,HGridArgs>  fn)
    {
        return Element.WithScope(uiScope =>
        {
            var grid = new Grid();

            var border = new Border
            {
                Child = grid
            };

            var props = new HGridProps(uiScope, border, grid);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, grid, border, ApplyStyle);
            state.ApplyVariantsStyle(grid, border, ApplyStyle);

            var showGridLines = args.ShowGridLines;
            if (showGridLines is not null)
            {
                grid.ShowGridLines = showGridLines.Value;
                if (showGridLines.IsReactive)
                {
                    uiScope.CreateEffect(epochScope => { grid.ShowGridLines = epochScope.Track(showGridLines); });
                }
            }

            var rowDefinitions = args.RowDefinitions;
            if (rowDefinitions is not null)
            {
                ApplyRowDefinitions(rowDefinitions.Value);
                if (rowDefinitions.IsReactive)
                {
                    uiScope.CreateEffect(epochScope => { ApplyRowDefinitions(epochScope.Track(rowDefinitions)); });
                }
            }

            var columnDefinitions = args.ColumnDefinitions;
            if (columnDefinitions is not null)
            {
                ApplyColumnDefinitions(columnDefinitions.Value);
                if (columnDefinitions.IsReactive)
                {
                    uiScope.CreateEffect(epochScope => { ApplyRowDefinitions(epochScope.Track(columnDefinitions)); });
                }
            }

            foreach (var (position, childElement) in args)
            {
                var child = childElement.Content;
                Grid.SetRow(child, position.row);
                Grid.SetColumn(child, position.column);
                Grid.SetRowSpan(child, position.rowSpan);
                Grid.SetColumnSpan(child, position.colSpan);
                grid.Children.Add(child);
            }

            return border;

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

                if (style.GapX is not null) grid.ColumnSpacing = style.GapX.Value;
                if (style.GapY is not null) grid.RowSpacing = style.GapY.Value;
            }

            void ApplyColumnDefinitions(IEnumerable<HgLen> lens)
            {
                grid.ColumnDefinitions.Clear();
                foreach (var len in lens)
                    grid.ColumnDefinitions.Add(new ColumnDefinition(len.Value));
            }

            void ApplyRowDefinitions(IEnumerable<HgLen> lens)
            {
                grid.RowDefinitions.Clear();
                foreach (var len in lens)
                    grid.RowDefinitions.Add(new RowDefinition(len.Value));
            }
        });
    }
}