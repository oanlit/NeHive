using Avalonia.Controls;
using NeHive.Reactive;

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
    Accessor<HPanelStyle>? style = null
)
{
    // 样式合并
    public readonly Accessor<HPanelStyle>? ComputedStyle = (style, strStyle) switch
    {
        (not null, not null) => new Computed<HPanelStyle>(() =>
            HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
        (not null, _) => style,
        (_, not null) => HPanelStyle.Parse(strStyle),
        _ => null
    };

    // SplitView 特有属性
    public readonly Accessor<bool>? IsPaneOpen = isPaneOpen;
    public readonly Accessor<SplitViewDisplayMode>? DisplayMode = displayMode;
    public readonly Accessor<double>? OpenPaneLength = openPaneLength;
    public readonly Accessor<double>? CompactPaneLength = compactPaneLength;

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

        // 应用样式
        if (prop.ComputedStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.ComputedStyle);
                ApplySplitViewStyle(splitView, style);
            });
        }

        // 绑定属性
        if (prop.IsPaneOpen != null)
            uiScope.CreateEffect(() => splitView.IsPaneOpen = prop.IsPaneOpen.RxValue);
        if (prop.DisplayMode != null)
            uiScope.CreateEffect(() => splitView.DisplayMode = prop.DisplayMode.RxValue);
        if (prop.OpenPaneLength != null)
            uiScope.CreateEffect(() => splitView.OpenPaneLength = prop.OpenPaneLength.RxValue);
        if (prop.CompactPaneLength != null)
            uiScope.CreateEffect(() => splitView.CompactPaneLength = prop.CompactPaneLength.RxValue);

        // 设置 Pane 和 Content
        if (prop.Pane != null)
            splitView.Pane = prop.Pane.Content;
        if (prop.Content != null)
            splitView.Content = prop.Content.Content;

        return new Element<SplitView>(uiScope, splitView, splitView);

        void ApplySplitViewStyle(SplitView sv, HPanelStyle style)
        {
            sv.Margin = style.Margin;
            if (style.ZIndex is not null) sv.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) sv.Width = style.Width.Value;
            if (style.Height is not null) sv.Height = style.Height.Value;
            if (style.MinWidth is not null) sv.Width = style.MinWidth.Value;
            if (style.MaxWidth is not null) sv.Width = style.MaxWidth.Value;
            if (style.MinHeight is not null) sv.Height = style.MinHeight.Value;
            if (style.MaxHeight is not null) sv.Height = style.MaxHeight.Value;

            sv.HorizontalAlignment = style.HorizontalAlignment;
            sv.VerticalAlignment = style.VerticalAlignment;

            sv.Background = style.Background;
            // 如需边框等可扩展
        }
    }
}