using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HWrapPanelProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<double>? ItemWidth;
    public readonly Accessor<double>? ItemHeight;
    public readonly Accessor<FullStyle> Style;
    public readonly Dictionary<string, StyleSet>? Variants;

    public HWrapPanelProp(
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        ItemWidth = itemWidth;
        ItemHeight = itemHeight;

        var baseStyle = StyleUtil.FromDefault();
        baseStyle.Orientation = Orientation.Horizontal;

        Style = StyleParser.ParseFull(strStyle, baseStyle, style);
        Variants = variants;
    }

    // 集合初始化器添加子元素
    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<WrapPanel> HWrapPanel(HWrapPanelProp prop)
    {
        var uiScope = new UiScope();
        var wrapPanel = new WrapPanel();
        var border = new Border
        {
            Child = wrapPanel
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, wrapPanel, border, ApplyStyle);
        state.ApplyVariantsStyle(wrapPanel, border, ApplyStyle);

        if (prop.ItemWidth is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var itemWidth = scope.Track(prop.ItemWidth);
                wrapPanel.ItemWidth = itemWidth;
            });
        }

        if (prop.ItemHeight is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var itemHeight = scope.Track(prop.ItemHeight);
                wrapPanel.ItemHeight = itemHeight;
            });
        }

        // 添加子元素
        foreach (var child in prop)
        {
            wrapPanel.Children.Add(child.Content);
        }

        return new Element<WrapPanel>(uiScope, border, wrapPanel);

        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, layout, bord);

            var orientation = styleValue.Orientation;
            if (orientation is null) return;

            if (orientation is Orientation.Horizontal)
            {
                if (styleValue.RowSpacing is not null) wrapPanel.ItemSpacing = styleValue.RowSpacing.Value;
                if (styleValue.ColumnSpacing is not null) wrapPanel.LineSpacing = styleValue.ColumnSpacing.Value;
            }
            else
            {
                if (styleValue.ColumnSpacing is not null) wrapPanel.ItemSpacing = styleValue.ColumnSpacing.Value;
                if (styleValue.RowSpacing is not null) wrapPanel.LineSpacing = styleValue.RowSpacing.Value;
            }
        }
    }
}