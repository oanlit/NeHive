using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HSplitViewProps(Scope scope, Border border, SplitView splitView)
    : BaseComponentProps(scope, border, splitView)
{
    public MutSignal<bool> IsPaneOpen
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(splitView.IsPaneOpen);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.IsPaneOpenProperty);
            return field;
        }
    }

    public MutSignal<double> CompactPaneLength
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(splitView.CompactPaneLength);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.CompactPaneLengthProperty);
            return field;
        }
    }

    public MutSignal<double> OpenPaneLength
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(splitView.OpenPaneLength);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.OpenPaneLengthProperty);
            return field;
        }
    }

    public MutSignal<bool> IsUseLightDismissOverlayMode
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(splitView.UseLightDismissOverlayMode);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.UseLightDismissOverlayModeProperty);
            return field;
        }
    }

    public MutSignal<SplitViewDisplayMode> DisplayMode
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<SplitViewDisplayMode>(splitView.DisplayMode);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.DisplayModeProperty);
            return field;
        }
    }

    public MutSignal<SplitViewPanePlacement> PanePlacement
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<SplitViewPanePlacement>(splitView.PanePlacement);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.PanePlacementProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> PaneBackground
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(splitView.PaneBackground);
            BridgeAvalonia.BindPropertySignal(scope, field, splitView, SplitView.PaneBackgroundProperty);
            return field;
        }
    }
}

public class HSplitViewArgs(
    Accessor<bool>? isPaneOpen = null,
    Accessor<bool>? isUseLightDismissOverlayMode = null,
    Accessor<double>? openPaneLength = null,
    Accessor<double>? compactPaneLength = null,
    Accessor<SplitViewDisplayMode>? displayMode = null,
    Accessor<SplitViewPanePlacement>? panePlacement = null,
    Accessor<IBrush?>? paneBackground = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onPaneOpened = null,
    Action<CancelRoutedEventArgs>? onPaneOpening = null,
    Action<RoutedEventArgs>? onPaneClosed = null,
    Action<CancelRoutedEventArgs>? onPaneClosing = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<bool>? IsPaneOpen = isPaneOpen;
    public readonly Accessor<bool>? IsUseLightDismissOverlayMode = isUseLightDismissOverlayMode;

    public readonly Accessor<double>? CompactPaneLength = compactPaneLength;
    public readonly Accessor<double>? OpenPaneLength = openPaneLength;

    public readonly Accessor<SplitViewDisplayMode>? DisplayMode = displayMode;
    public readonly Accessor<SplitViewPanePlacement>? PanePlacement = panePlacement;

    public readonly Accessor<IBrush?>? PaneBackground = paneBackground;

    public IElement? Pane { get; init; }
    public IElement? Content { get; init; }

    public readonly Action<RoutedEventArgs>? OnPaneOpened = onPaneOpened;
    public readonly Action<CancelRoutedEventArgs>? OnPaneOpening = onPaneOpening;
    public readonly Action<RoutedEventArgs>? OnPaneClosed = onPaneClosed;
    public readonly Action<CancelRoutedEventArgs>? OnPaneClosing = onPaneClosing;
}

public static partial class BaseComponent
{
    public static IElement<SplitView> HSplitView(
        Accessor<bool>? isPaneOpen = null,
        Accessor<bool>? isUseLightDismissOverlayMode = null,
        Accessor<double>? openPaneLength = null,
        Accessor<double>? compactPaneLength = null,
        Accessor<SplitViewDisplayMode>? displayMode = null,
        Accessor<SplitViewPanePlacement>? panePlacement = null,
        Accessor<IBrush?>? paneBackground = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onPaneOpened = null,
        Action<CancelRoutedEventArgs>? onPaneOpening = null,
        Action<RoutedEventArgs>? onPaneClosed = null,
        Action<CancelRoutedEventArgs>? onPaneClosing = null
    ) => HSplitView(out _, _ => new(isPaneOpen, isUseLightDismissOverlayMode, openPaneLength, compactPaneLength,
        displayMode, panePlacement, paneBackground, strStyle, style,
        baseInteraction, onPaneOpened, onPaneOpening, onPaneClosed, onPaneClosing));

    public static IElement<SplitView> HSplitView(
        out SplitView expose,
        Accessor<bool>? isPaneOpen = null,
        Accessor<bool>? isUseLightDismissOverlayMode = null,
        Accessor<double>? openPaneLength = null,
        Accessor<double>? compactPaneLength = null,
        Accessor<SplitViewDisplayMode>? displayMode = null,
        Accessor<SplitViewPanePlacement>? panePlacement = null,
        Accessor<IBrush?>? paneBackground = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onPaneOpened = null,
        Action<CancelRoutedEventArgs>? onPaneOpening = null,
        Action<RoutedEventArgs>? onPaneClosed = null,
        Action<CancelRoutedEventArgs>? onPaneClosing = null
    ) => HSplitView(out expose, _ => new(isPaneOpen, isUseLightDismissOverlayMode, openPaneLength, compactPaneLength,
        displayMode, panePlacement, paneBackground, strStyle, style,
        baseInteraction, onPaneOpened, onPaneOpening, onPaneClosed, onPaneClosing));

    public static IElement<SplitView> HSplitView(Func<HSplitViewProps, HSplitViewArgs> fn) => HSplitView(out _, fn);

    public static IElement<SplitView> HSplitView(out SplitView expose, Func<HSplitViewProps, HSplitViewArgs> fn)
    {
        var splitView = new SplitView();
        expose = splitView;
        return Element<SplitView>.WithScope(uiScope =>
        {
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = splitView
            };

            var props = new HSplitViewProps(uiScope, border, splitView);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, splitView, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(splitView, border, StyleUtil.ApplyStyle);

            args.BaseInteraction?.ApplyInteractions(uiScope, splitView);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            if (args.IsPaneOpen is not null)
            {
                splitView.IsPaneOpen = args.IsPaneOpen.Value;
                if (args.IsPaneOpen.IsReactive)
                    uiScope.CreateEffect(epochScope => splitView.IsPaneOpen = epochScope.Track(args.IsPaneOpen));
            }

            if (args.IsUseLightDismissOverlayMode is not null)
            {
                splitView.UseLightDismissOverlayMode = args.IsUseLightDismissOverlayMode.Value;
                if (args.IsUseLightDismissOverlayMode.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.UseLightDismissOverlayMode = epochScope.Track(args.IsUseLightDismissOverlayMode));
            }

            if (args.CompactPaneLength is not null)
            {
                splitView.CompactPaneLength = args.CompactPaneLength.Value;
                if (args.CompactPaneLength.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.CompactPaneLength = epochScope.Track(args.CompactPaneLength));
            }

            if (args.OpenPaneLength is not null)
            {
                splitView.OpenPaneLength = args.OpenPaneLength.Value;
                if (args.OpenPaneLength.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.OpenPaneLength = epochScope.Track(args.OpenPaneLength));
            }

            if (args.DisplayMode is not null)
            {
                splitView.DisplayMode = args.DisplayMode.Value;
                if (args.DisplayMode.IsReactive)
                    uiScope.CreateEffect(epochScope => splitView.DisplayMode = epochScope.Track(args.DisplayMode));
            }

            if (args.PanePlacement is not null)
            {
                splitView.PanePlacement = args.PanePlacement.Value;
                if (args.PanePlacement.IsReactive)
                    uiScope.CreateEffect(epochScope => splitView.PanePlacement = epochScope.Track(args.PanePlacement));
            }

            if (args.PaneBackground is not null)
            {
                splitView.PaneBackground = args.PaneBackground.Value;
                if (args.PaneBackground.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        splitView.PaneBackground = epochScope.Track(args.PaneBackground));
            }

            if (args.Pane is not null)
                splitView.Pane = args.Pane.Content;
            if (args.Content is not null)
                splitView.Content = args.Content.Content;

            if (args.OnPaneOpened is not null)
            {
                uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                    add: h => splitView.PaneOpened += h,
                    remove: h => splitView.PaneOpened -= h,
                    handler: (_, e) => args.OnPaneOpened(e)
                );
            }

            if (args.OnPaneOpening is not null)
            {
                uiScope.AddHandler<EventHandler<CancelRoutedEventArgs>>(
                    add: h => splitView.PaneOpening += h,
                    remove: h => splitView.PaneOpening -= h,
                    handler: (_, e) => args.OnPaneOpening(e)
                );
            }

            if (args.OnPaneClosed is not null)
            {
                uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                    add: h => splitView.PaneClosed += h,
                    remove: h => splitView.PaneClosed -= h,
                    handler: (_, e) => args.OnPaneClosed(e)
                );
            }

            if (args.OnPaneClosing is not null)
            {
                uiScope.AddHandler<EventHandler<CancelRoutedEventArgs>>(
                    add: h => splitView.PaneClosing += h,
                    remove: h => splitView.PaneClosing -= h,
                    handler: (_, e) => args.OnPaneClosing(e)
                );
            }

            return (splitView, border);
        });
    }
}