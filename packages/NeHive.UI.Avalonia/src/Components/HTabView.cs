using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// TabControl 配置类，支持索引器添加标签页
/// </summary>
public class HTabViewProp : IEnumerable<(Accessor<string> Header, IElement Content)>
{
    private readonly List<(Accessor<string> Header, IElement Content)> _items = [];

    public readonly MutSignal<int>? BindSelectedIndex;
    public readonly Accessor<FullStyle>? Style;

    public HTabViewProp(
        MutSignal<int>? bindSelectedIndex = null,
        Accessor<string>? strStyle = null)
    {
        BindSelectedIndex = bindSelectedIndex;
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

    // 索引器：支持 Accessor<string> 标题（响应式）
    public IElement this[Accessor<string> header]
    {
        set => _items.Add((header, value));
    }

    // 可选：传统 Add 方法（配合集合初始化器）
    public void Add(Accessor<string> header, IElement content) => _items.Add((header, content));
    public void Add(string header, IElement content) => _items.Add((header, content));

    public IEnumerator<(Accessor<string> Header, IElement Content)> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement HTabControl(HTabViewProp prop)
    {
        var uiScope = new UiScope();

        var tabControl = new TabControl();
        var border = new Border
        {
            Child = tabControl
        };

        // 应用布局样式
        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                ApplyStyle(style.Normal);
            });
        }

        // tabControl.ItemTemplate = 

        // 构建 TabItems
        var tabItems = new List<TabItem>();
        foreach (var (headerAccessor, contentElement) in prop)
        {
            var tabItem = new TabItem();
            uiScope.CreateEffect(() => tabItem.Header = headerAccessor.RxValue);
            tabItem.Content = contentElement.Content;
            tabItems.Add(tabItem);
        }

        tabControl.ItemsSource = tabItems;

        if (prop.BindSelectedIndex is null)
        {
            if (tabItems.Count > 0) tabControl.SelectedIndex = 0;
        }
        else
        {
            // 双向绑定 selectedIndex
            // View -> ViewModel
            tabControl.SelectionChanged += (_, _) =>
            {
                if (tabControl.SelectedIndex != prop.BindSelectedIndex.RxValue)
                    prop.BindSelectedIndex.RxValue = tabControl.SelectedIndex;
            };
            // ViewModel -> View
            uiScope.CreateEffect(() =>
            {
                var idx = prop.BindSelectedIndex.RxValue;
                if (idx >= 0 && idx < tabItems.Count && idx != tabControl.SelectedIndex)
                    tabControl.SelectedIndex = idx;
            });
        }

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, tabControl, border);
            
            if (style.VerticalTextAlignment is not null)
                border.VerticalAlignment = style.VerticalTextAlignment.Value;
            if (style.Foreground is not null) tabControl.Foreground = style.Foreground;
            if (style.FontSize is not null) tabControl.FontSize = style.FontSize.Value;
            if (style.FontWeight is not null) tabControl.FontWeight = style.FontWeight.Value;
            if (style.FontStyle is not null) tabControl.FontStyle = style.FontStyle.Value;
            if (style.Foreground is not null) tabControl.Foreground = style.Foreground;
        }
    }
}