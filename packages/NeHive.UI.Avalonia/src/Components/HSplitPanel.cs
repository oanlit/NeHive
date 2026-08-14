using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// SplitPanel 配置类
/// </summary>
public class HSplitPanelArgs : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];
    
    public Accessor<double>? SplitFraction { get; }
    public Accessor<double>? SplitPosition { get; }
    public readonly Accessor<FullStyle> StrStyle;
    public readonly Signal<StyleSet>? Style;

    public HSplitPanelArgs(
        Accessor<double>? splitFraction = null, // 第一个面板占比 (0-1)
        Accessor<double>? splitPosition = null, // 绝对像素位置（优先级高于 splitFraction）
        Accessor<string>? strStyle = null,
        HStyle? style = null
    )
    {
        SplitFraction = splitFraction;
        SplitPosition = splitPosition;
        var baseStyle = StyleUtil.FromDefault();
        baseStyle.Orientation = Orientation.Horizontal;
        StrStyle = StyleParser.ParseFull(strStyle, baseStyle);
        Style = style is null ? null : StyleUtil.HStyle2Signal(style);
    }

    public IElement this[int index]
    {
        set
        {
            // 确保列表足够长
            while (_children.Count <= index)
                _children.Add(null!);
            _children[index] = value;
        }
    }

    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<Grid> HSplitPanel(HSplitPanelArgs args)
    {
        return Element<Grid>.WithScope(uiScope =>
        {
            var grid = new Grid();
            var border = new Border
            {
                Child = grid
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, grid, border, ApplyStyle);
            state.ApplyVariantsStyle(grid, border, ApplyStyle);

            var children = args.ToList();
            if (children.Count < 2)
                throw new InvalidOperationException("The SplitPanel requires at least two child elements.");

            Accessor<bool> accessorIsHorizontal = args.StrStyle.Value.Normal.Orientation is not Orientation.Vertical;
            if (args.StrStyle.IsReactive)
            {
                accessorIsHorizontal = uiScope.CreateComputed(() =>
                {
                    var style = args.StrStyle.RxValue;
                    return style.Normal.Orientation is not Orientation.Vertical;
                });
            }

            uiScope.CreateEffect(epochScope =>
            {
                var isHorizontal = epochScope.Track(accessorIsHorizontal);
                double? splitPos = args.SplitPosition is null ? null : epochScope.Track(args.SplitPosition);
                double? splitFrac = args.SplitFraction is null ? null : epochScope.Track(args.SplitFraction);

                // 动态构建列/行定义
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();

                if (isHorizontal)
                {
                    // 水平分割：列定义
                    if (splitPos.HasValue)
                    {
                        // 绝对像素分割
                        grid.ColumnDefinitions.Add(new ColumnDefinition(splitPos.Value, GridUnitType.Pixel));
                        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                    }
                    else if (splitFrac.HasValue)
                    {
                        var frac = Math.Clamp(splitFrac.Value, 0.05, 0.95);
                        grid.ColumnDefinitions.Add(new ColumnDefinition(frac, GridUnitType.Star));
                        grid.ColumnDefinitions.Add(new ColumnDefinition(1 - frac, GridUnitType.Star));
                    }
                    else
                    {
                        // 默认各占一半
                        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                    }
                }
                else
                {
                    // 垂直分割：行定义
                    if (splitPos.HasValue)
                    {
                        grid.RowDefinitions.Add(new RowDefinition(splitPos.Value, GridUnitType.Pixel));
                        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                    }
                    else if (splitFrac.HasValue)
                    {
                        var frac = Math.Clamp(splitFrac.Value, 0.05, 0.95);
                        grid.RowDefinitions.Add(new RowDefinition(frac, GridUnitType.Star));
                        grid.RowDefinitions.Add(new RowDefinition(1 - frac, GridUnitType.Star));
                    }
                    else
                    {
                        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                    }
                }

                // 添加内容和分割条
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];

                    var control = child.Content;
                    if (isHorizontal)
                        Grid.SetColumn(control, i * 2); // 每个面板占一列，中间留一列给分割条
                    else
                        Grid.SetRow(control, i * 2);

                    grid.Children.Add(control);

                    // 不是最后一个，添加 GridSplitter
                    if (i >= children.Count - 1) continue;

                    var splitter = new GridSplitter
                    {
                        Background = global::Avalonia.Media.Brushes.Gray,
                        ResizeDirection = isHorizontal ? GridResizeDirection.Columns : GridResizeDirection.Rows,
                        Width = isHorizontal ? 4 : double.NaN,
                        Height = isHorizontal ? double.NaN : 4,
                        HorizontalAlignment = isHorizontal ? HorizontalAlignment.Left : HorizontalAlignment.Stretch,
                        VerticalAlignment = isHorizontal ? VerticalAlignment.Stretch : VerticalAlignment.Top
                    };

                    if (isHorizontal)
                    {
                        Grid.SetColumn(splitter, i * 2 + 1);
                        splitter.Width = 4;
                    }
                    else
                    {
                        Grid.SetRow(splitter, i * 2 + 1);
                        splitter.Height = 4;
                    }

                    grid.Children.Add(splitter);
                }
            });

            return (grid, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                if (styleValue.Width is not null)
                    grid.Width = styleValue.Width.Value;

                if (styleValue.Height is not null)
                    grid.Height = styleValue.Height.Value;

                if (styleValue.MinWidth is not null)
                    grid.MinWidth = styleValue.MinWidth.Value;

                if (styleValue.MaxWidth is not null)
                    grid.MaxWidth = styleValue.MaxWidth.Value;

                if (styleValue.MinHeight is not null)
                    grid.MinHeight = styleValue.MinHeight.Value;

                if (styleValue.MaxHeight is not null)
                    grid.MaxHeight = styleValue.MaxHeight.Value;

                if (styleValue.GapX is not null) grid.ColumnSpacing = styleValue.GapX.Value;
                if (styleValue.GapY is not null) grid.RowSpacing = styleValue.GapY.Value;
            }
        });
    }
}