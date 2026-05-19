using System.Collections;
using Avalonia.Controls.Primitives;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public class HUniformGridProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<int>? Rows;
    public readonly Accessor<int>? Columns;
    public readonly Accessor<HGridStyle>? Style;

    public HUniformGridProp(
        Accessor<int>? rows = null,
        Accessor<int>? columns = null,
        Accessor<string>? strStyle = null,
        Accessor<HGridStyle>? style = null
    )
    {
        Rows = rows;
        Columns = columns;

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

    // 索引器（可按整数位置添加，但通常用集合初始化器）
    public IElement this[int index]
    {
        set
        {
            while (_children.Count <= index)
                _children.Add(null!);
            _children[index] = value;
        }
    }

    // 集合初始化器
    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 UniformGrid 控件
    /// </summary>
    public static IElement<UniformGrid> HUniformGrid(HUniformGridProp prop)
    {
        var uiScope = new UiScope();
        var grid = new UniformGrid();

        // 应用样式（直接复用 HGrid 的样式应用逻辑）
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyGridStyleToControl(style);
            });
        }

        // 绑定行数和列数
        if (prop.Rows != null)
            uiScope.CreateEffect(() => grid.Rows = prop.Rows.RxValue);
        if (prop.Columns != null)
            uiScope.CreateEffect(() => grid.Columns = prop.Columns.RxValue);

        // 添加子元素
        foreach (var childElement in prop)
        {
            grid.Children.Add(childElement.Content);
        }

        return new Element<UniformGrid>(uiScope, grid, grid);

        void ApplyGridStyleToControl(HGridStyle style)
        {
            grid.Margin = style.Margin;
            if (style.ZIndex.HasValue) grid.ZIndex = style.ZIndex.Value;

            if (style.Width.HasValue) grid.Width = style.Width.Value;
            if (style.Height.HasValue) grid.Height = style.Height.Value;
            if (style.MinWidth.HasValue) grid.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth.HasValue) grid.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight.HasValue) grid.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight.HasValue) grid.MaxHeight = style.MaxHeight.Value;

            grid.ColumnSpacing = style.ColumnSpacing;
            grid.RowSpacing = style.RowSpacing;

            grid.HorizontalAlignment = style.HorizontalAlignment;
            grid.VerticalAlignment = style.VerticalAlignment;

            if (style.Background != null)
                grid.Background = style.Background;
        }
    }
}