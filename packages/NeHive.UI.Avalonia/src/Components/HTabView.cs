using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// TabControl 配置类，支持索引器添加标签页
/// </summary>
public class HTabViewProp(
    MutSignal<int>? bindSelectedIndex = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null)
    : IEnumerable<(Accessor<string> Header, IElement Content)>
{
    private readonly List<(Accessor<string> Header, IElement Content)> _items = [];

    public readonly MutSignal<int>? BindSelectedIndex = bindSelectedIndex;
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;
    
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

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, tabControl, border, ApplyStyle);
        state.ApplyVariantsStyle(tabControl, border, ApplyStyle);

        // tabControl.ItemTemplate = 

        // 构建 TabItems
        var tabItems = new List<TabItem>();
        foreach (var (headerAccessor, contentElement) in prop)
        {
            var tabItem = new TabItem();
            
            tabItem.Header = headerAccessor.Value;
            if(headerAccessor.IsReactive)
                uiScope.CreateEffect(epochScope => tabItem.Header = epochScope.Track(headerAccessor));
            
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

        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, layout, bord);

            if (styleValue.VerticalTextAlignment is not null)
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.Foreground is not null) tabControl.Foreground = styleValue.Foreground;
            if (styleValue.FontSize is not null) tabControl.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) tabControl.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) tabControl.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) tabControl.Foreground = styleValue.Foreground;
        }
    }
}