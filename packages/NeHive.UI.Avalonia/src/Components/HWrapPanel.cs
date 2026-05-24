using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HWrapPanelProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];
    public readonly Accessor<StyleSet>? Style;
    public readonly Accessor<double>? ItemWidth;
    public readonly Accessor<double>? ItemHeight;

    public HWrapPanelProp(
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<string>? strStyle = null
    )
    {
        ItemWidth = itemWidth;
        ItemHeight = itemHeight;

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

        // 应用样式
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyStyle(style);
            });
        }

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

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, wrapPanel, border);

            var orientation = style.Orientation;
            if(orientation is null) return;

            if (orientation is Orientation.Horizontal)
            {
                if (style.RowSpacing is not null) wrapPanel.ItemSpacing = style.RowSpacing.Value;
                if (style.ColumnSpacing is not null) wrapPanel.LineSpacing = style.ColumnSpacing.Value;
            }
            else
            {
                if (style.ColumnSpacing is not null) wrapPanel.ItemSpacing = style.ColumnSpacing.Value;
                if (style.RowSpacing is not null) wrapPanel.LineSpacing = style.RowSpacing.Value;
            }

        }
    }
}