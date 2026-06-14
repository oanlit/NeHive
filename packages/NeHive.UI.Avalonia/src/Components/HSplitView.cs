using Avalonia.Controls;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// SplitView 配置类（构造参数仅布局/样式/行为属性）
/// </summary>
public class HSplitViewProp(
    Accessor<bool>? isPaneOpen = null,
    Accessor<SplitViewDisplayMode>? displayMode = null,
    Accessor<double>? openPaneLength = null,
    Accessor<double>? compactPaneLength = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
)
{
    public readonly Accessor<bool>? IsPaneOpen = isPaneOpen;
    public readonly Accessor<SplitViewDisplayMode>? DisplayMode = displayMode;
    public readonly Accessor<double>? OpenPaneLength = openPaneLength;
    public readonly Accessor<double>? CompactPaneLength = compactPaneLength;
    
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    // 两个固定区域
    public IElement? Pane { get; init; }
    public IElement? Content { get; init; }
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 SplitView 组件（侧边栏面板 + 内容区域）
    /// </summary>
    public static IElement<SplitView> HSplitView(HSplitViewProp prop)
    {
        var uiScope = new UiScope();
        var splitView = new SplitView();
        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = splitView
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, splitView, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(splitView, border, StyleUtil.ApplyStyle);

        // 绑定属性
        if (prop.IsPaneOpen is not null)
        {
            splitView.IsPaneOpen = prop.IsPaneOpen.Value;
            if(prop.IsPaneOpen.IsReactive)
                uiScope.CreateEffect(epochScope => splitView.IsPaneOpen = epochScope.Track(prop.IsPaneOpen));
        }
        
        if (prop.DisplayMode is not null)
        {
            splitView.DisplayMode = prop.DisplayMode.Value;
            if(prop.DisplayMode.IsReactive)
                uiScope.CreateEffect(epochScope => splitView.DisplayMode = epochScope.Track(prop.DisplayMode));
        }
        
        if (prop.OpenPaneLength is not null)
        {
            splitView.OpenPaneLength = prop.OpenPaneLength.Value;
            if(prop.OpenPaneLength.IsReactive)
                uiScope.CreateEffect(epochScope => splitView.OpenPaneLength = epochScope.Track(prop.OpenPaneLength));
        }
        
        if (prop.CompactPaneLength is not null)
        {
            splitView.CompactPaneLength = prop.CompactPaneLength.Value;
            if(prop.CompactPaneLength.IsReactive)
                uiScope.CreateEffect(epochScope => splitView.CompactPaneLength = epochScope.Track(prop.CompactPaneLength));
        }

        // 设置 Pane 和 Content
        if (prop.Pane is not null)
            splitView.Pane = prop.Pane.Content;
        if (prop.Content is not null)
            splitView.Content = prop.Content.Content;

        return new Element<SplitView>(uiScope, border, splitView);
    }
}