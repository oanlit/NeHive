using Avalonia.Controls;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HSplitViewArgs(
    Accessor<bool>? isPaneOpen = null,
    Accessor<SplitViewDisplayMode>? displayMode = null,
    Accessor<double>? openPaneLength = null,
    Accessor<double>? compactPaneLength = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null
)
{
    public readonly Accessor<bool>? IsPaneOpen = isPaneOpen;
    public readonly Accessor<SplitViewDisplayMode>? DisplayMode = displayMode;
    public readonly Accessor<double>? OpenPaneLength = openPaneLength;
    public readonly Accessor<double>? CompactPaneLength = compactPaneLength;
    
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public IElement? Pane { get; init; }
    public IElement? Content { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<SplitView> HSplitView(HSplitViewArgs args)
    {
        return Element<SplitView>.WithScope(uiScope =>
        {
            var splitView = new SplitView();
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = splitView
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, splitView, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(splitView, border, StyleUtil.ApplyStyle);

            // 绑定属性
            if (args.IsPaneOpen is not null)
            {
                splitView.IsPaneOpen = args.IsPaneOpen.Value;
                if (args.IsPaneOpen.IsReactive)
                    uiScope.CreateEffect(epochScope => splitView.IsPaneOpen = epochScope.Track(args.IsPaneOpen));
            }

            if (args.DisplayMode is not null)
            {
                splitView.DisplayMode = args.DisplayMode.Value;
                if (args.DisplayMode.IsReactive)
                    uiScope.CreateEffect(epochScope => splitView.DisplayMode = epochScope.Track(args.DisplayMode));
            }

            if (args.OpenPaneLength is not null)
            {
                splitView.OpenPaneLength = args.OpenPaneLength.Value;
                if (args.OpenPaneLength.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.OpenPaneLength = epochScope.Track(args.OpenPaneLength));
            }

            if (args.CompactPaneLength is not null)
            {
                splitView.CompactPaneLength = args.CompactPaneLength.Value;
                if (args.CompactPaneLength.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.CompactPaneLength = epochScope.Track(args.CompactPaneLength));
            }

            // 设置 Pane 和 Content
            if (args.Pane is not null)
                splitView.Pane = args.Pane.Content;
            if (args.Content is not null)
                splitView.Content = args.Content.Content;

            return (splitView, border);
        });
    }
}