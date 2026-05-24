using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HUniformGridProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<int>? Rows;
    public readonly Accessor<int>? Columns;
    public readonly Accessor<FullStyle>? Style;

    public HUniformGridProp(
        Accessor<int>? rows = null,
        Accessor<int>? columns = null,
        Accessor<string>? strStyle = null,
        Accessor<HGridStyle>? style = null
    )
    {
        Rows = rows;
        Columns = columns;

        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
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
        var border = new Border
        {
            Child = grid
        };

        // 应用样式（直接复用 HGrid 的样式应用逻辑）
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyStyle(style.Normal);
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

        return new Element<UniformGrid>(uiScope, border, grid);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, grid, border);

            if (style.ColumnSpacing is not null) grid.ColumnSpacing = style.ColumnSpacing.Value;
            if (style.RowSpacing is not null) grid.RowSpacing = style.RowSpacing.Value;
        }
    }
}