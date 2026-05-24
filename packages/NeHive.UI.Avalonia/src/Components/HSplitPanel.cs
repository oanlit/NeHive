using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// SplitPanel 配置类
/// </summary>
public class HSplitPanelProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<StyleSet>? Style;

    public Accessor<double>? SplitFraction { get; }
    public Accessor<double>? SplitPosition { get; }

    public HSplitPanelProp(
        Accessor<double>? splitFraction = null, // 第一个面板占比 (0-1)
        Accessor<double>? splitPosition = null, // 绝对像素位置（优先级高于 splitFraction）
        Accessor<string>? strStyle = null
    )
    {
        SplitFraction = splitFraction;
        SplitPosition = splitPosition;
        if (strStyle is null) return;

        var result = StyleUtil.FromDefault();
        result.Orientation = Orientation.Horizontal;
        Style = new Computed<StyleSet>(() =>
        {
            var str = strStyle.RxValue;
            StyleParser.Parse(str, ref result);
            return result;
        });
    }

    // 索引器：按顺序添加面板内容
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

    // 集合初始化器：直接 Add 按顺序添加
    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建可拖拽分割面板（支持两个或更多区域）
    /// </summary>
    public static IElement<Grid> HSplitPanel(HSplitPanelProp prop)
    {
        var uiScope = new UiScope();
        var grid = new Grid();
        var border = new Border
        {
            Child = grid
        };

        // 应用样式
        if (prop.Style is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyStyle(style);
            });
        }

        var children = prop.ToList();
        if (children.Count < 2)
            throw new InvalidOperationException("The SplitPanel requires at least two child elements.");

        Accessor<bool> accessorIsHorizontal;
        if (prop.Style is null)
        {
            accessorIsHorizontal = true;
        }
        else
        {
            accessorIsHorizontal = uiScope.CreateComputed(() =>
            {
                var style = prop.Style.RxValue;
                return style.Orientation is not Orientation.Vertical;
            });
        }

        uiScope.CreateEffect(epochScope =>
        {
            var isHorizontal = epochScope.Track(accessorIsHorizontal);
            double? splitPos = prop.SplitPosition is null ? null : epochScope.Track(prop.SplitPosition);
            double? splitFrac = prop.SplitFraction is null ? null : epochScope.Track(prop.SplitFraction);

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

        return new Element<Grid>(uiScope, border, grid);
        
        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, grid, border);
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