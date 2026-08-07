using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HUniformGridProps(Scope scope, Border border, UniformGrid uniformGrid)
    : BaseComponentProps(scope, border, uniformGrid)
{
    public Signal<int> Rows
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(uniformGrid.Rows);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == UniformGrid.RowsProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<int> Columns
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(uniformGrid.Columns);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == UniformGrid.ColumnsProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<int> FirstColumn
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(uniformGrid.FirstColumn);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == UniformGrid.FirstColumnProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }
}

public class HUniformGridArgs(
    Accessor<int>? rows = null,
    Accessor<int>? columns = null,
    Accessor<int>? firstColumn = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : HPanelArgs(strStyle, style, baseInteraction)
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<int>? Rows = rows;
    public readonly Accessor<int>? Columns = columns;
    public readonly Accessor<int>? FirstColumn = firstColumn;

    public IElement this[int index]
    {
        set
        {
            while (_children.Count <= index)
                _children.Add(null!);
            _children[index] = value;
        }
    }
}

public static partial class BaseComponent
{
    public static IElement<UniformGrid> HUniformGrid(
        Accessor<int>? rows = null,
        Accessor<int>? columns = null,
        Accessor<int>? firstColumn = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HUniformGrid(out _, _ => new(rows, columns, firstColumn, strStyle, style, baseInteraction));
    
    public static IElement<UniformGrid> HUniformGrid(
        out UniformGrid expose,
        Accessor<int>? rows = null,
        Accessor<int>? columns = null,
        Accessor<int>? firstColumn = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HUniformGrid(out expose, _ => new(rows, columns, firstColumn, strStyle, style, baseInteraction));

    public static IElement<UniformGrid> HUniformGrid(Func<HUniformGridProps, HUniformGridArgs> fn
    ) => HUniformGrid(out _, fn);

    public static IElement<UniformGrid> HUniformGrid(out UniformGrid expose,
        Func<HUniformGridProps, HUniformGridArgs> fn)
    {
        var grid = new UniformGrid();
        expose = grid;
        return Element<UniformGrid>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = grid
            };

            var props = new HUniformGridProps(uiScope, border, grid);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, grid, border, ApplyStyle);
            state.ApplyVariantsStyle(grid, border, ApplyStyle);

            if (args.Rows is not null)
            {
                grid.Rows = args.Rows.Value;
                if (args.Rows.IsReactive)
                    uiScope.CreateEffect(epochScope => grid.Rows = epochScope.Track(args.Rows));
            }

            if (args.Columns is not null)
            {
                grid.Columns = args.Columns.Value;
                if (args.Columns.IsReactive)
                    uiScope.CreateEffect(epochScope => grid.Columns = epochScope.Track(args.Columns));
            }

            if (args.FirstColumn is not null)
            {
                grid.Columns = args.FirstColumn.Value;
                if (args.FirstColumn.IsReactive)
                    uiScope.CreateEffect(epochScope => grid.FirstColumn = epochScope.Track(args.FirstColumn));
            }

            foreach (var childElement in args)
            {
                grid.Children.Add(childElement.Content);
            }

            return (grid, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, grid, border);

                if (styleValue.GapX is not null) grid.ColumnSpacing = styleValue.GapX.Value;
                if (styleValue.GapY is not null) grid.RowSpacing = styleValue.GapY.Value;
            }
        });
    }
}