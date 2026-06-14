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
public class HSplitPanelProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];
    
    public Accessor<double>? SplitFraction { get; }
    public Accessor<double>? SplitPosition { get; }
    public readonly Accessor<FullStyle> Style;
    public readonly Dictionary<string, StyleSet>? Variants;

    public HSplitPanelProp(
        Accessor<double>? splitFraction = null, // 第一个面板占比 (0-1)
        Accessor<double>? splitPosition = null, // 绝对像素位置（优先级高于 splitFraction）
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        SplitFraction = splitFraction;
        SplitPosition = splitPosition;
        var baseStyle = StyleUtil.FromDefault();
        baseStyle.Orientation = Orientation.Horizontal;
        Style = StyleParser.ParseFull(strStyle, baseStyle, style);
        Variants = variants;
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
        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, grid, border, ApplyStyle);
        state.ApplyVariantsStyle(grid, border, ApplyStyle);

        var children = prop.ToList();
        if (children.Count < 2)
            throw new InvalidOperationException("The SplitPanel requires at least two child elements.");

        Accessor<bool> accessorIsHorizontal = prop.Style.Value.Normal.Orientation is not Orientation.Vertical;
        if (prop.Style.IsReactive)
        {
            accessorIsHorizontal = uiScope.CreateComputed(() =>
            {
                var style = prop.Style.RxValue;
                return style.Normal.Orientation is not Orientation.Vertical;
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
            
            if (styleValue.ColumnSpacing is not null) grid.ColumnSpacing = styleValue.ColumnSpacing.Value;
            if (styleValue.RowSpacing is not null) grid.RowSpacing = styleValue.RowSpacing.Value;
        }
    }
}