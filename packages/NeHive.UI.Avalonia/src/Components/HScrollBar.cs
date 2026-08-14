using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HScrollBarPart
{
    internal IElement<Button>? LineDownButtonElement { get; private set; }
    internal IElement<Button>? LineUpButtonElement { get; private set; }
    internal IElement<Button>? PageDownButtonElement { get; private set; }
    internal IElement<Button>? PageUpButtonElement { get; private set; }

    public IElement<Button> LineDownButton(IElement<Button> element) => LineDownButtonElement = element;
    public IElement<Button> LineUpButton(IElement<Button> element) => LineUpButtonElement = element;
    public IElement<Button> PageDownButton(IElement<Button> element) => PageDownButtonElement = element;
    public IElement<Button> PageUpButton(IElement<Button> element) => PageUpButtonElement = element;
}

public class HScrollBarProps(Scope scope, Border border, ScrollBar scrollBar)
    : HRangeBaseProps(scope, border, scrollBar)
{
    public Signal<bool> IsExpanded
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.IsExpandedProperty, scrollBar.IsExpanded);
            return field;
        }
    }

    public Signal<double> ViewportSize
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.ViewportSizeProperty, scrollBar.ViewportSize);
            return field;
            // field = new MutSignal<double>(scrollBar.ViewportSize);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollBar.ViewportSizeProperty);
            // return field;
        }
    }

    public Signal<bool> IsAllowAutoHide
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.AllowAutoHideProperty, scrollBar.AllowAutoHide);
            return field;
            // field = new MutSignal<bool>(scrollBar.AllowAutoHide);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollBar.AllowAutoHideProperty);
            // return field;
        }
    }

    public Signal<bool> IsDeferredScrollingEnabled
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollViewer.IsDeferredScrollingEnabledProperty, ScrollViewer.GetIsDeferredScrollingEnabled(scrollBar));
            return field;
            // field = new MutSignal<bool>(ScrollViewer.GetIsDeferredScrollingEnabled(scrollBar));
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollViewer.IsDeferredScrollingEnabledProperty);
            // return field;
        }
    }

    public Signal<ScrollBarVisibility> Visibility
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.VisibilityProperty, scrollBar.Visibility);
            return field;
            // field = new MutSignal<ScrollBarVisibility>(scrollBar.Visibility);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollBar.VisibilityProperty);
            // return field;
        }
    }

    public Signal<TimeSpan> HideDelay
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.HideDelayProperty, scrollBar.HideDelay);
            return field;
            // field = new MutSignal<TimeSpan>(scrollBar.HideDelay);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollBar.HideDelayProperty);
            // return field;
        }
    }

    public Signal<TimeSpan> ShowDelay
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                ScrollBar.ShowDelayProperty, scrollBar.ShowDelay);
            return field;
            // field = new MutSignal<TimeSpan>(scrollBar.ShowDelay);
            // BridgeAvalonia.BindPropertySignal(scope, field, scrollBar, ScrollBar.ShowDelayProperty);
            // return field;
        }
    }
}

public class HScrollBarArgs(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<bool>? isAllowAutoHide = null,
    Accessor<bool>? isDeferredScrollingEnabled = null,
    Accessor<double>? viewportSize = null,
    Accessor<ScrollBarVisibility>? visibility = null,
    Accessor<TimeSpan>? showDelay = null,
    Accessor<TimeSpan>? hideDelay = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null,
    Action<ScrollEventArgs>? onScroll = null
) : HRangeBaseArgs(value, bindValue, minimum, maximum, smallChange,
    largeChange, strStyle, style, baseInteraction, onValueChanged)
{
    public readonly Accessor<double>? ViewportSize = viewportSize;

    public readonly Accessor<bool>? IsAllowAutoHide = isAllowAutoHide;
    public readonly Accessor<bool>? IsDeferredScrollingEnabled = isDeferredScrollingEnabled;
    public readonly Accessor<ScrollBarVisibility>? Visibility = visibility;
    public readonly Accessor<TimeSpan>? HideDelay = hideDelay;
    public readonly Accessor<TimeSpan>? ShowDelay = showDelay;
    public readonly Action<ScrollEventArgs>? OnScroll = onScroll;

    public Func<HScrollBarPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<ScrollBar> HScrollBar(Func<HScrollBarProps, HScrollBarArgs> fn)
    {
        return Element<ScrollBar>.WithScope(uiScope =>
        {
            var scrollBar = new ScrollBar();
            var border = new Border();

            var props = new HScrollBarProps(uiScope, border, scrollBar);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, scrollBar, border, ApplyStyle);
            state.ApplyVariantsStyle(scrollBar, border, ApplyStyle);

            RangeBaseUtil.BindAccessor(uiScope, scrollBar, args);

            if (args.IsAllowAutoHide is not null)
            {
                scrollBar.AllowAutoHide = args.IsAllowAutoHide.Value;
                if (args.IsAllowAutoHide.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.AllowAutoHide = scope.Track(args.IsAllowAutoHide));
            }

            if (args.IsDeferredScrollingEnabled is not null)
            {
                ScrollViewer.SetIsDeferredScrollingEnabled(scrollBar, args.IsDeferredScrollingEnabled.Value);
                if (args.IsDeferredScrollingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        ScrollViewer.SetIsDeferredScrollingEnabled(scrollBar,
                            scope.Track(args.IsDeferredScrollingEnabled)));
            }

            if (args.ViewportSize is not null)
            {
                scrollBar.ViewportSize = args.ViewportSize.Value;
                if (args.ViewportSize.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.ViewportSize = scope.Track(args.ViewportSize));
            }

            if (args.Visibility is not null)
            {
                scrollBar.Visibility = args.Visibility.Value;
                if (args.Visibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.Visibility = scope.Track(args.Visibility));
            }

            if (args.ShowDelay is not null)
            {
                scrollBar.ShowDelay = args.ShowDelay.Value;
                if (args.ShowDelay.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.ShowDelay = scope.Track(args.ShowDelay));
            }

            if (args.HideDelay is not null)
            {
                scrollBar.HideDelay = args.HideDelay.Value;
                if (args.HideDelay.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.HideDelay = scope.Track(args.HideDelay));
            }

            if (args.OnScroll is not null)
                scrollBar.Scroll += (_, e) => args.OnScroll(e);

            if (args.Template is null)
            {
                scrollBar.Template = new FuncControlTemplate((_, _) => border);
            }
            else
            {
                var part = new HScrollBarPart();
                var content = args.Template(part).Content;
                border.Child = content;
                scrollBar.Template = new FuncControlTemplate((_, s) =>
                {
                    if (part.LineDownButtonElement is not null)
                    {
                        var __ = part.LineDownButtonElement.Content;
                        var lineDownButtonElement = part.LineDownButtonElement.Expose!;
                        lineDownButtonElement.Name = "PART_LineDownButton";
                        s.Register("PART_LineDownButton", lineDownButtonElement);
                    }

                    if (part.LineUpButtonElement is not null)
                    {
                        var __ = part.LineUpButtonElement.Content;
                        var lineUpButtonElement = part.LineUpButtonElement.Expose!;
                        lineUpButtonElement.Name = "PART_LineUpButton";
                        s.Register("PART_LineUpButton", lineUpButtonElement);
                    }

                    if (part.PageDownButtonElement is not null)
                    {
                        var __ = part.PageDownButtonElement.Content;
                        var pageDownButtonElement = part.PageDownButtonElement.Expose!;
                        pageDownButtonElement.Name = "PART_PageDownButton";
                        s.Register("PART_PageDownButton", pageDownButtonElement);
                    }

                    if (part.PageUpButtonElement is not null)
                    {
                        var __ = part.PageUpButtonElement.Content;
                        var pageUpButtonElement = part.PageUpButtonElement.Expose!;
                        pageUpButtonElement.Name = "PART_PageUpButton";
                        s.Register("PART_PageUpButton", pageUpButtonElement);
                    }

                    return border;
                });
            }

            return (scrollBar, scrollBar);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) scrollBar.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}