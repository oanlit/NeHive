using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public readonly struct HgLen
{
    public readonly GridLength Value;

    private HgLen(GridLength value) => Value = value;

    public static implicit operator HgLen(GridLength length) => new(length);

    public static implicit operator HgLen(int pixels) => new(new GridLength(pixels, GridUnitType.Pixel));
    public static implicit operator HgLen(double pixels) => new(new GridLength(pixels, GridUnitType.Pixel));

    public static HgLen Auto => new(GridLength.Auto);
    public static HgLen Star(double value = 1) => new(new GridLength(value, GridUnitType.Star));
}

public record GridPosition(
    Accessor<int>? Row = null,
    Accessor<int>? Column = null,
    Accessor<int>? RowSpan = null,
    Accessor<int>? ColSpan = null
);

public class HGridProps(Scope scope, Border border, Grid grid) : BaseComponentProps(scope, border, grid);

public class HGridArgs(
    Accessor<bool>? showGridLines = null,
    Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
    Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), IEnumerable<(GridPosition?, IElement)>
{
    private readonly List<(GridPosition? GridPos, IElement Element)> _children = [];

    public readonly Accessor<bool>? ShowGridLines = showGridLines;
    public readonly Accessor<IReadOnlyList<HgLen>>? RowDefinitions = rowDefinitions;
    public readonly Accessor<IReadOnlyList<HgLen>>? ColumnDefinitions = columnDefinitions;

    public IElement this[GridPosition? key]
    {
        set => _children.Add((key, value));
    }

    public IElement this[Accessor<int>? row = null,
        Accessor<int>? column = null,
        Accessor<int>? rowSpan = null,
        Accessor<int>? colSpan = null]
    {
        set
        {
            var key = new GridPosition(row, column, rowSpan, colSpan);
            _children.Add((key, value));
        }
    }

    public void Add(IElement element)
    {
        _children.Add((new(), element));
    }

    public IEnumerator<(GridPosition?, IElement)> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<Grid> HGrid(
        Accessor<bool>? showGridLines = null,
        Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
        Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HGrid(out _, _ => new(showGridLines, rowDefinitions, columnDefinitions,
        strStyle, style, baseInteraction));
    
    public static IElement<Grid> HGrid(
        out Grid expose, 
        Accessor<bool>? showGridLines = null,
        Accessor<IReadOnlyList<HgLen>>? rowDefinitions = null,
        Accessor<IReadOnlyList<HgLen>>? columnDefinitions = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HGrid(out expose, _ => new(showGridLines, rowDefinitions, columnDefinitions,
        strStyle, style, baseInteraction));

    public static IElement<Grid> HGrid(Func<HGridProps, HGridArgs> fn) => HGrid(out _, fn);

    public static IElement<Grid> HGrid(out Grid expose, Func<HGridProps, HGridArgs> fn)
    {
        var grid = new Grid();
        expose = grid;
        return Element<Grid>.WithScope(uiScope =>
        {
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

            args.BaseInteraction?.ApplyInteractions(uiScope, grid);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            var showGridLines = args.ShowGridLines;
            if (showGridLines is not null)
            {
                grid.ShowGridLines = showGridLines.Value;
                if (showGridLines.IsReactive)
                    uiScope.CreateEffect(epoch => grid.ShowGridLines = epoch.Track(showGridLines));
            }

            var rowDefinitions = args.RowDefinitions;
            if (rowDefinitions is not null)
            {
                ApplyRowDefinitions(rowDefinitions.Value);
                if (rowDefinitions.IsReactive)
                    uiScope.CreateEffect(epoch => ApplyRowDefinitions(epoch.Track(rowDefinitions)));
            }

            var columnDefinitions = args.ColumnDefinitions;
            if (columnDefinitions is not null)
            {
                ApplyColumnDefinitions(columnDefinitions.Value);
                if (columnDefinitions.IsReactive)
                    uiScope.CreateEffect(epoch => ApplyRowDefinitions(epoch.Track(columnDefinitions)));
            }

            foreach (var (pos, childElement) in args)
            {
                var child = childElement.Content;
                if (pos is not null)
                {
                    SetPos(child, pos.Row?.Value, pos.Column?.Value, pos.RowSpan?.Value, pos.ColSpan?.Value);
                    if (pos.Row?.IsReactive is true ||
                        pos.Column?.IsReactive is true ||
                        pos.RowSpan?.IsReactive is true ||
                        pos.ColSpan?.IsReactive is true
                       )
                        uiScope.CreateEffect(() =>
                        {
                            SetPos(child, pos.Row?.RxValue, pos.Column?.RxValue, pos.RowSpan?.RxValue,
                                pos.ColSpan?.RxValue);
                        });
                }

                grid.Children.Add(child);
            }

            return (grid, border);

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

            void SetPos(Control control, int? row, int? column, int? rowSpan, int? colSpan)
            {
                if (row is not null) Grid.SetRow(control, row.Value);
                if (column is not null) Grid.SetColumn(control, column.Value);
                if (rowSpan is not null) Grid.SetRowSpan(control, rowSpan.Value);
                if (colSpan is not null) Grid.SetColumnSpan(control, colSpan.Value);
            }
        });
    }
}