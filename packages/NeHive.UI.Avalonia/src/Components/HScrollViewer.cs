using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HScrollViewerProps(Scope scope, Border border, ScrollViewer scrollViewer)
    : BaseComponentProps(scope, border, scrollViewer)
{
    public Signal<bool> IsAllowAutoHide
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.AllowAutoHideProperty, scrollViewer.AllowAutoHide);
            return field;
        }
    }

    public Signal<bool> IsBringIntoViewOnFocusChange
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.BringIntoViewOnFocusChangeProperty, scrollViewer.BringIntoViewOnFocusChange);
            return field;
        }
    }

    public Signal<bool> IsDeferredScrollingEnabled
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.IsDeferredScrollingEnabledProperty, scrollViewer.IsDeferredScrollingEnabled);
            return field;
        }
    }

    public Signal<bool> IsScrollChainingEnabled
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.IsScrollChainingEnabledProperty, scrollViewer.IsScrollChainingEnabled);
            return field;
        }
    }

    public Signal<bool> IsScrollInertiaEnabled
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.IsScrollInertiaEnabledProperty, scrollViewer.IsScrollInertiaEnabled);
            return field;
        }
    }

    public Signal<ScrollBarVisibility> HorizontalScrollBarVisibility
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer.HorizontalScrollBarVisibility);
            return field;
        }
    }

    public Signal<ScrollBarVisibility> VerticalScrollBarVisibility
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer.VerticalScrollBarVisibility);
            return field;
        }
    }

    public Signal<SnapPointsType> HorizontalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.HorizontalSnapPointsTypeProperty, scrollViewer.HorizontalSnapPointsType);
            return field;
        }
    }

    public Signal<SnapPointsType> VerticalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.VerticalSnapPointsTypeProperty, scrollViewer.VerticalSnapPointsType);
            return field;
        }
    }

    public Signal<SnapPointsAlignment> HorizontalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.HorizontalSnapPointsAlignmentProperty, scrollViewer.HorizontalSnapPointsAlignment);
            return field;
        }
    }

    public Signal<SnapPointsAlignment> VerticalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, scrollViewer,
                ScrollViewer.VerticalSnapPointsAlignmentProperty, scrollViewer.VerticalSnapPointsAlignment);
            return field;
        }
    }
}

public class HScrollPart
{
    internal IElement<ScrollBar>? HorizontalScrollBarElement { get; private set; }
    internal IElement<ScrollBar>? VerticalScrollBarElement { get; private set; }
    internal IElement<ScrollContentPresenter>? ContentPresenterElement { get; private set; }

    public IElement<ScrollBar> HorizontalScrollBar(IElement<ScrollBar> element) => HorizontalScrollBarElement = element;
    public IElement<ScrollBar> VerticalScrollBar(IElement<ScrollBar> element) => VerticalScrollBarElement = element;

    public IElement<ScrollContentPresenter> ContentPresenter(IElement<ScrollContentPresenter> element) =>
        ContentPresenterElement = element;
}

public class HScrollViewerArgs(
    Accessor<bool>? isAllowAutoHide = null,
    Accessor<bool>? isBringIntoViewOnFocusChange = null,
    Accessor<bool>? isDeferredScrollingEnabled = null,
    Accessor<bool>? isScrollChainingEnabled = null,
    Accessor<bool>? isScrollInertiaEnabled = null,
    Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
    Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
    Accessor<SnapPointsType>? horizontalSnapPointsType = null,
    Accessor<SnapPointsType>? verticalSnapPointsType = null,
    Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
    Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsAllowAutoHide = isAllowAutoHide;
    public readonly Accessor<bool>? IsBringIntoViewOnFocusChange = isBringIntoViewOnFocusChange;
    public readonly Accessor<bool>? IsDeferredScrollingEnabled = isDeferredScrollingEnabled;
    public readonly Accessor<bool>? IsScrollChainingEnabled = isScrollChainingEnabled;
    public readonly Accessor<bool>? IsScrollInertiaEnabled = isScrollInertiaEnabled;

    public readonly Accessor<ScrollBarVisibility>? HorizontalScrollBarVisibility = horizontalScrollBarVisibility;
    public readonly Accessor<ScrollBarVisibility>? VerticalScrollBarVisibility = verticalScrollBarVisibility;
    public readonly Accessor<SnapPointsType>? HorizontalSnapPointsType = horizontalSnapPointsType;
    public readonly Accessor<SnapPointsType>? VerticalSnapPointsType = verticalSnapPointsType;
    public readonly Accessor<SnapPointsAlignment>? HorizontalSnapPointsAlignment = horizontalSnapPointsAlignment;
    public readonly Accessor<SnapPointsAlignment>? VerticalSnapPointsAlignment = verticalSnapPointsAlignment;

    public IElement Content
    {
        init => _children.Add(value);
    }

    public Func<HScrollPart, IElement>? Template { get; init; }

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
        => _children.Add(element);
}

public static partial class BaseComponent
{
    public static IElement<ScrollViewer> HScrollViewer(
        Accessor<bool>? isAllowAutoHide = null,
        Accessor<bool>? isBringIntoViewOnFocusChange = null,
        Accessor<bool>? isDeferredScrollingEnabled = null,
        Accessor<bool>? isScrollChainingEnabled = null,
        Accessor<bool>? isScrollInertiaEnabled = null,
        Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
        Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
        Accessor<SnapPointsType>? horizontalSnapPointsType = null,
        Accessor<SnapPointsType>? verticalSnapPointsType = null,
        Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
        Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HScrollViewer(out _, _ => new(isAllowAutoHide, isBringIntoViewOnFocusChange,
        isDeferredScrollingEnabled, isScrollChainingEnabled, isScrollInertiaEnabled,
        horizontalScrollBarVisibility, verticalScrollBarVisibility,
        horizontalSnapPointsType, verticalSnapPointsType, horizontalSnapPointsAlignment,
        verticalSnapPointsAlignment, strStyle, style, baseInteraction)
    );
    
    public static IElement<ScrollViewer> HScrollViewer(
        out ScrollViewer expose,
        Accessor<bool>? isAllowAutoHide = null,
        Accessor<bool>? isBringIntoViewOnFocusChange = null,
        Accessor<bool>? isDeferredScrollingEnabled = null,
        Accessor<bool>? isScrollChainingEnabled = null,
        Accessor<bool>? isScrollInertiaEnabled = null,
        Accessor<ScrollBarVisibility>? horizontalScrollBarVisibility = null,
        Accessor<ScrollBarVisibility>? verticalScrollBarVisibility = null,
        Accessor<SnapPointsType>? horizontalSnapPointsType = null,
        Accessor<SnapPointsType>? verticalSnapPointsType = null,
        Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
        Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HScrollViewer(out expose, _ => new(isAllowAutoHide, isBringIntoViewOnFocusChange,
        isDeferredScrollingEnabled, isScrollChainingEnabled, isScrollInertiaEnabled,
        horizontalScrollBarVisibility, verticalScrollBarVisibility,
        horizontalSnapPointsType, verticalSnapPointsType, horizontalSnapPointsAlignment,
        verticalSnapPointsAlignment, strStyle, style, baseInteraction)
    );

    public static IElement<ScrollViewer> HScrollViewer(Func<HScrollViewerProps, HScrollViewerArgs> fn)
        => HScrollViewer(out _, fn);

    public static IElement<ScrollViewer> HScrollViewer(out ScrollViewer expose,
        Func<HScrollViewerProps, HScrollViewerArgs> fn)
    {
        var scroll = new ScrollViewer();
        expose = scroll;
        return Element<ScrollViewer>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = scroll
            };

            var props = new HScrollViewerProps(uiScope, border, scroll);
            var args = fn(props);

            var control = ElementUtil.WrapSingleContainerContent(args).Content;

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, control, border, ApplyStyle);
            state.ApplyVariantsStyle(control, border, ApplyStyle);
            args.BaseInteraction?.ApplyInteractions(uiScope, scroll);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            if (args.IsAllowAutoHide is not null)
            {
                scroll.AllowAutoHide = args.IsAllowAutoHide.Value;
                if (args.IsAllowAutoHide.IsReactive)
                    uiScope.CreateEffect(scope => scroll.AllowAutoHide = scope.Track(args.IsAllowAutoHide));
            }

            if (args.IsBringIntoViewOnFocusChange is not null)
            {
                scroll.BringIntoViewOnFocusChange = args.IsBringIntoViewOnFocusChange.Value;
                if (args.IsBringIntoViewOnFocusChange.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.BringIntoViewOnFocusChange = scope.Track(args.IsBringIntoViewOnFocusChange));
            }

            if (args.IsDeferredScrollingEnabled is not null)
            {
                scroll.IsDeferredScrollingEnabled = args.IsDeferredScrollingEnabled.Value;
                if (args.IsDeferredScrollingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsDeferredScrollingEnabled = scope.Track(args.IsDeferredScrollingEnabled));
            }

            if (args.IsScrollChainingEnabled is not null)
            {
                scroll.IsScrollChainingEnabled = args.IsScrollChainingEnabled.Value;
                if (args.IsScrollChainingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsScrollChainingEnabled = scope.Track(args.IsScrollChainingEnabled));
            }

            if (args.IsScrollInertiaEnabled is not null)
            {
                scroll.IsScrollInertiaEnabled = args.IsScrollInertiaEnabled.Value;
                if (args.IsScrollInertiaEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.IsScrollInertiaEnabled = scope.Track(args.IsScrollInertiaEnabled));
            }

            if (args.HorizontalScrollBarVisibility is not null)
            {
                scroll.HorizontalScrollBarVisibility = args.HorizontalScrollBarVisibility.Value;
                if (args.HorizontalScrollBarVisibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalScrollBarVisibility = scope.Track(args.HorizontalScrollBarVisibility));
            }

            if (args.VerticalScrollBarVisibility is not null)
            {
                scroll.VerticalScrollBarVisibility = args.VerticalScrollBarVisibility.Value;
                if (args.VerticalScrollBarVisibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalScrollBarVisibility = scope.Track(args.VerticalScrollBarVisibility));
            }

            if (args.HorizontalSnapPointsType is not null)
            {
                scroll.HorizontalSnapPointsType = args.HorizontalSnapPointsType.Value;
                if (args.HorizontalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalSnapPointsType = scope.Track(args.HorizontalSnapPointsType));
            }

            if (args.VerticalSnapPointsType is not null)
            {
                scroll.VerticalSnapPointsType = args.VerticalSnapPointsType.Value;
                if (args.VerticalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalSnapPointsType = scope.Track(args.VerticalSnapPointsType));
            }

            if (args.HorizontalSnapPointsAlignment is not null)
            {
                scroll.HorizontalSnapPointsAlignment = args.HorizontalSnapPointsAlignment.Value;
                if (args.HorizontalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.HorizontalSnapPointsAlignment = scope.Track(args.HorizontalSnapPointsAlignment));
            }

            if (args.VerticalSnapPointsAlignment is not null)
            {
                scroll.VerticalSnapPointsAlignment = args.VerticalSnapPointsAlignment.Value;
                if (args.VerticalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scroll.VerticalSnapPointsAlignment = scope.Track(args.VerticalSnapPointsAlignment));
            }

            if (args.Template is not null)
            {
                var part = new HScrollPart();
                var content = args.Template(part).Content;

                ScrollBar? horizontalScrollBarElement = null;
                ScrollBar? verticalScrollBarElement = null;
                ScrollContentPresenter? scrollContentPresenter = null;

                if (part.HorizontalScrollBarElement is not null)
                {
                    _ = part.HorizontalScrollBarElement.Content;
                    horizontalScrollBarElement = part.HorizontalScrollBarElement.Expose!;
                }

                if (part.VerticalScrollBarElement is not null)
                {
                    _ = part.VerticalScrollBarElement.Content;
                    verticalScrollBarElement = part.VerticalScrollBarElement.Expose!;
                }

                if (part.ContentPresenterElement is not null)
                {
                    _ = part.ContentPresenterElement.Content;
                    scrollContentPresenter = part.ContentPresenterElement.Expose!;
                }

                scroll.Template = new FuncControlTemplate((_, s) =>
                {
                    if (horizontalScrollBarElement is not null)
                    {
                        horizontalScrollBarElement.Name = "PART_HorizontalScrollBar";
                        s.Register("PART_HorizontalScrollBar", horizontalScrollBarElement);
                        scroll.PropertyChanged += (_, e) =>
                        {
                            if (e.Property == ScrollViewer.OffsetProperty)
                            {
                                var vec = (Vector)e.NewValue!;
                                horizontalScrollBarElement.Value = vec.X;
                            }
                        };
                    }

                    if (verticalScrollBarElement is not null)
                    {
                        verticalScrollBarElement.Name = "PART_VerticalScrollBar";
                        s.Register("PART_VerticalScrollBar", verticalScrollBarElement);
                        scroll.PropertyChanged += (_, e) =>
                        {
                            if (e.Property == ScrollViewer.OffsetProperty)
                            {
                                var vec = (Vector)e.NewValue!;
                                verticalScrollBarElement.Value = vec.Y;
                            }
                        };
                    }

                    if (scrollContentPresenter is not null)
                    {
                        scrollContentPresenter.Name = "PART_ContentPresenter";
                        s.Register("PART_ContentPresenter", scrollContentPresenter);
                    }

                    return content;
                });
            }

            // scroll.Content = stack;
            scroll.Content = control;

            uiScope.OnMount += scroll.ScrollToHome;

            // return (stack, border);
            return (scroll, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                if (styleValue.Padding is not null)
                {
                    border.Padding = new Thickness(0);
                    // stack.Margin = styleValue.Padding.Value;
                }
            }
        });
    }
}