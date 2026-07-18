using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

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

public class HScrollProps(Scope scope, ScrollViewer scrollViewer)
{
    public Signal<bool> IsAllowAutoHide
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollViewer.AllowAutoHide);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.AllowAutoHideProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsBringIntoViewOnFocusChange
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollViewer.BringIntoViewOnFocusChange);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.BringIntoViewOnFocusChangeProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsDeferredScrollingEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollViewer.IsDeferredScrollingEnabled);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.IsDeferredScrollingEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsScrollChainingEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollViewer.IsScrollChainingEnabled);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.IsScrollChainingEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsScrollInertiaEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollViewer.IsScrollInertiaEnabled);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.IsScrollInertiaEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<ScrollBarVisibility> HorizontalScrollBarVisibility
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<ScrollBarVisibility>(scrollViewer.HorizontalScrollBarVisibility);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.HorizontalScrollBarVisibilityProperty)
                    sig.RxValue = (ScrollBarVisibility)args.NewValue!;
            }
        }
    }

    public Signal<ScrollBarVisibility> VerticalScrollBarVisibility
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<ScrollBarVisibility>(scrollViewer.VerticalScrollBarVisibility);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.VerticalScrollBarVisibilityProperty)
                    sig.RxValue = (ScrollBarVisibility)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsType> HorizontalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsType>(scrollViewer.HorizontalSnapPointsType);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.HorizontalSnapPointsAlignmentProperty)
                    sig.RxValue = (SnapPointsType)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsType> VerticalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsType>(scrollViewer.VerticalSnapPointsType);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.VerticalSnapPointsTypeProperty)
                    sig.RxValue = (SnapPointsType)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsAlignment> HorizontalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsAlignment>(scrollViewer.HorizontalSnapPointsAlignment);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.HorizontalSnapPointsAlignmentProperty)
                    sig.RxValue = (SnapPointsAlignment)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsAlignment> VerticalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsAlignment>(scrollViewer.VerticalSnapPointsAlignment);
            field = sig;

            scrollViewer.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollViewer.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.VerticalSnapPointsAlignmentProperty)
                    sig.RxValue = (SnapPointsAlignment)args.NewValue!;
            }
        }
    }
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
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null) : ISingleChildrenProp
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

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public IElement Content
    {
        init => _children.Add(value);
    }

    public Func<HScrollProps, HScrollPart, IElement>? Template { get; init; }

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
        => _children.Add(element);
}

public class HScrollExpose(ScrollViewer scroll)
{
    public Size Extent => scroll.Extent;
    public Size Viewport => scroll.Viewport;

    public Vector Offset
    {
        get => scroll.Offset;
        set => scroll.Offset = value;
    }

    public void LineUp()
        => scroll.LineUp();

    public void LineDown()
        => scroll.LineDown();

    public void LineLeft()
        => scroll.LineLeft();

    public void LineRight()
        => scroll.LineRight();

    public void PageUp()
        => scroll.PageUp();

    public void PageDown()
        => scroll.PageDown();

    public void PageLeft()
        => scroll.PageLeft();

    public void PageRight()
        => scroll.PageRight();

    public void ScrollToHome()
        => scroll.ScrollToHome();

    public void ScrollToEnd()
        => scroll.ScrollToEnd();
}

public static partial class BaseComponent
{
    public static IElement<StackPanel> HScrollViewer(out HScrollExpose expose, HScrollViewerArgs args)
    {
        var scroll = new ScrollViewer();
        expose = new HScrollExpose(scroll);
        return Element<StackPanel>.WithScope(uiScope =>
        {
            var stack = new StackPanel();

            var border = new Border
            {
                Child = scroll
            };

            var state = new CommonState(uiScope, args.Style.Value.Normal)
            {
                StrVariants = args.Style.Value.Variants,
                Variants = args.Variants
            };

            state.ApplyAccessorStyle(args.Style, stack, border, ApplyStyle);
            state.ApplyVariantsStyle(stack, border, ApplyStyle);

            foreach (var child in args)
                stack.Children.Add(child.Content);
            
            // scroll.PointerWheelChanged += (_, e)=>
            // {
            //     Console.WriteLine($"Source = {e.Source}");
            //     if (e.Source is Border b)
            //     {
            //         Console.WriteLine($"Child={b.Child?.GetType()}");
            //         StyledElement? p = b;
            //         while (p != null)
            //         {
            //             Console.WriteLine(p);
            //             p = p.Parent;
            //         }
            //     }
            //     Console.WriteLine($"Offset : {scroll.Offset}");
            // };

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
                var props = new HScrollProps(uiScope, scroll);
                var part = new HScrollPart();
                var content = args.Template(props, part).Content;

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

            scroll.Content = stack;

            uiScope.OnMount += () => { scroll.ScrollToHome(); };

            return (stack, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Padding is not null)
                {
                    border.Padding = new Thickness(0);
                    stack.Margin = styleValue.Padding.Value;
                }

                var orientation = styleValue.Orientation ?? Orientation.Vertical;
                stack.Orientation = orientation;

                switch (orientation)
                {
                    case Orientation.Horizontal:
                        if (styleValue.ColumnSpacing is not null) stack.Spacing = styleValue.ColumnSpacing.Value;
                        break;

                    case Orientation.Vertical:
                        if (styleValue.RowSpacing is not null) stack.Spacing = styleValue.RowSpacing.Value;
                        break;
                }
            }
        });
    }

    public static IElement<StackPanel> HScrollViewer(HScrollViewerArgs args)
    {
        return HScrollViewer(out _, args);
    }
}