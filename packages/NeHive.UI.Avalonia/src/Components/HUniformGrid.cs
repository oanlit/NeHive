using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HUniformGridProp(
    Accessor<int>? rows = null,
    Accessor<int>? columns = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<int>? Rows = rows;
    public readonly Accessor<int>? Columns = columns;
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, grid, border, ApplyStyle);
        state.ApplyVariantsStyle(grid, border, ApplyStyle);

        // 绑定行数和列数
        if (prop.Rows is not null)
        {
            grid.Rows = prop.Rows.Value;
            if (prop.Rows.IsReactive)
                uiScope.CreateEffect(epochScope => grid.Rows = epochScope.Track(prop.Rows));
        }

        if (prop.Columns is not null)
        {
            grid.Columns = prop.Columns.Value;
            if (prop.Columns.IsReactive)
                uiScope.CreateEffect(epochScope => grid.Columns = epochScope.Track(prop.Columns));
        }

        // 添加子元素
        foreach (var childElement in prop)
        {
            grid.Children.Add(childElement.Content);
        }

        return new Element<UniformGrid>(uiScope, border, grid);

        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, grid, border);

            if (styleValue.ColumnSpacing is not null) grid.ColumnSpacing = styleValue.ColumnSpacing.Value;
            if (styleValue.RowSpacing is not null) grid.RowSpacing = styleValue.RowSpacing.Value;
        }
    }
}