using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HWrapPanelProps(Scope scope, Border border, WrapPanel wrapPanel)
    : BaseComponentProps(scope, border, wrapPanel)
{
    public Signal<double> ItemWidth
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(wrapPanel.ItemWidth);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == WrapPanel.ItemWidthProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<double> ItemHeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(wrapPanel.ItemHeight);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == WrapPanel.ItemHeightProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<WrapPanelItemsAlignment> ItemsAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<WrapPanelItemsAlignment>(wrapPanel.ItemsAlignment);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == WrapPanel.ItemsAlignmentProperty)
                    sig.RxValue = (WrapPanelItemsAlignment)args.NewValue!;
            }
        }
    }
}

public class HWrapPanelArgs : HPanelArgs
{
    public readonly Accessor<double>? ItemWidth;
    public readonly Accessor<double>? ItemHeight;
    public readonly Accessor<WrapPanelItemsAlignment>? ItemsAlignment;

    public HWrapPanelArgs(
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<WrapPanelItemsAlignment>? itemsAlignment = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) : base(style: style, baseInteraction: baseInteraction)
    {
        ItemWidth = itemWidth;
        ItemHeight = itemHeight;
        ItemsAlignment = itemsAlignment;

        var baseStyle = StyleUtil.FromDefault();
        baseStyle.Orientation = Orientation.Horizontal;
        StrStyle = StyleParser.ParseFull(strStyle, baseStyle);
    }
}

public static partial class BaseComponent
{
    public static IElement<WrapPanel> HWrapPanel(
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<WrapPanelItemsAlignment>? itemsAlignment = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HWrapPanel(out _, _ => new(itemWidth, itemHeight, itemsAlignment,
        strStyle, style, baseInteraction));

    public static IElement<WrapPanel> HWrapPanel(
        out WrapPanel expose,
        Accessor<double>? itemWidth = null,
        Accessor<double>? itemHeight = null,
        Accessor<WrapPanelItemsAlignment>? itemsAlignment = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HWrapPanel(out expose, _ => new(itemWidth, itemHeight, itemsAlignment,
        strStyle, style, baseInteraction));

    public static IElement<WrapPanel> HWrapPanel(Func<HWrapPanelProps, HWrapPanelArgs> fn) => HWrapPanel(out _, fn);

    public static IElement<WrapPanel> HWrapPanel(out WrapPanel expose, Func<HWrapPanelProps, HWrapPanelArgs> fn)
    {
        var wrapPanel = new WrapPanel();
        expose = wrapPanel;
        return Element<WrapPanel>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = wrapPanel
            };

            var props = new HWrapPanelProps(uiScope, border, wrapPanel);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, wrapPanel, border, ApplyStyle);
            state.ApplyVariantsStyle(wrapPanel, border, ApplyStyle);

            args.BaseInteraction?.ApplyInteractions(uiScope, wrapPanel);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            if (args.ItemWidth is not null)
            {
                uiScope.CreateEffect(scope =>
                {
                    var itemWidth = scope.Track(args.ItemWidth);
                    wrapPanel.ItemWidth = itemWidth;
                });
            }

            if (args.ItemHeight is not null)
            {
                uiScope.CreateEffect(scope =>
                {
                    var itemHeight = scope.Track(args.ItemHeight);
                    wrapPanel.ItemHeight = itemHeight;
                });
            }

            if (args.ItemsAlignment is not null)
            {
                uiScope.CreateEffect(scope =>
                {
                    var itemsAlignment = scope.Track(args.ItemsAlignment);
                    wrapPanel.ItemsAlignment = itemsAlignment;
                });
            }

            foreach (var child in args)
            {
                wrapPanel.Children.Add(child.Content);
            }

            return (wrapPanel, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                var orientation = styleValue.Orientation;
                if (orientation is null) return;

                if (orientation is Orientation.Horizontal)
                {
                    if (styleValue.GapY is not null) wrapPanel.ItemSpacing = styleValue.GapY.Value;
                    if (styleValue.GapX is not null) wrapPanel.LineSpacing = styleValue.GapX.Value;
                }
                else
                {
                    if (styleValue.GapX is not null) wrapPanel.ItemSpacing = styleValue.GapX.Value;
                    if (styleValue.GapY is not null) wrapPanel.LineSpacing = styleValue.GapY.Value;
                }
            }
        });
    }
}