using Avalonia;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HScrollContentPresenterProps(Scope scope, Border border, ScrollContentPresenter scrollContentPresenter)
    : BaseComponentProps(scope, border, scrollContentPresenter)
{
    public Signal<Size> Extent
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Size>(scrollContentPresenter.Extent);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.ExtentProperty)
                    sig.RxValue = (Size)args.NewValue!;
            }
        }
    }

    public Signal<Size> Viewport
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Size>(scrollContentPresenter.Viewport);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.ViewportProperty)
                    sig.RxValue = (Size)args.NewValue!;
            }
        }
    }

    public Signal<Vector> Offset
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Vector>(scrollContentPresenter.Offset);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.OffsetProperty)
                    sig.RxValue = (Vector)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsScrollChainingEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollContentPresenter.IsScrollChainingEnabled);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.IsScrollChainingEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsScrollInertiaEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(ScrollViewer.GetIsScrollInertiaEnabled(scrollContentPresenter));
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.IsScrollInertiaEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> CanHorizontallyScroll
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollContentPresenter.CanHorizontallyScroll);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.CanHorizontallyScrollProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> CanVerticallyScroll
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollContentPresenter.CanVerticallyScroll);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.CanVerticallyScrollProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsType>? HorizontalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsType>(scrollContentPresenter.HorizontalSnapPointsType);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.HorizontalSnapPointsTypeProperty)
                    sig.RxValue = (SnapPointsType)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsType> VerticalSnapPointsType
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsType>(scrollContentPresenter.VerticalSnapPointsType);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.VerticalSnapPointsTypeProperty)
                    sig.RxValue = (SnapPointsType)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsAlignment> HorizontalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsAlignment>(scrollContentPresenter.HorizontalSnapPointsAlignment);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.HorizontalSnapPointsAlignmentProperty)
                    sig.RxValue = (SnapPointsAlignment)args.NewValue!;
            }
        }
    }

    public Signal<SnapPointsAlignment> VerticalSnapPointsAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<SnapPointsAlignment>(scrollContentPresenter.VerticalSnapPointsAlignment);
            field = sig;

            scrollContentPresenter.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollContentPresenter.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollContentPresenter.VerticalSnapPointsAlignmentProperty)
                    sig.RxValue = (SnapPointsAlignment)args.NewValue!;
            }
        }
    }
}

public class HScrollContentPresenterArgs(
    Accessor<Vector>? offset = null,
    Accessor<bool>? isScrollChainingEnabled = null,
    Accessor<bool>? isScrollInertiaEnabled = null,
    Accessor<bool>? canHorizontallyScroll = null,
    Accessor<bool>? canVerticallyScroll = null,
    Accessor<SnapPointsType>? horizontalSnapPointsType = null,
    Accessor<SnapPointsType>? verticalSnapPointsType = null,
    Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
    Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
    HScrollGestureRecognizer? gestureRecognizer = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null)
{
    public readonly Accessor<Vector>? Offset = offset;

    public readonly Accessor<bool>? IsScrollChainingEnabled = isScrollChainingEnabled;
    public readonly Accessor<bool>? IsScrollInertiaEnabled = isScrollInertiaEnabled;
    public readonly Accessor<bool>? CanHorizontallyScroll = canHorizontallyScroll;
    public readonly Accessor<bool>? CanVerticallyScroll = canVerticallyScroll;
    public readonly Accessor<SnapPointsType>? HorizontalSnapPointsType = horizontalSnapPointsType;
    public readonly Accessor<SnapPointsType>? VerticalSnapPointsType = verticalSnapPointsType;
    public readonly Accessor<SnapPointsAlignment>? HorizontalSnapPointsAlignment = horizontalSnapPointsAlignment;
    public readonly Accessor<SnapPointsAlignment>? VerticalSnapPointsAlignment = verticalSnapPointsAlignment;

    public readonly ScrollGestureRecognizer? GestureRecognizer = gestureRecognizer?.GestureRecognizer;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);
}

public static partial class BaseComponent
{
    public static IElement<ScrollContentPresenter> HScrollContentPresenter(
        Func<HScrollContentPresenterProps, HScrollContentPresenterArgs> fn)
    {
        return Element<ScrollContentPresenter>.WithScope(uiScope =>
        {
            var scrollContentPresenter = new ScrollContentPresenter();
            var border = new Border
            {
                Child = scrollContentPresenter
            };

            var props = new HScrollContentPresenterProps(uiScope, border, scrollContentPresenter);
            var args = fn(props);

            HScrollContentPresenterCore(uiScope, scrollContentPresenter, border, args);

            return (scrollContentPresenter, border);
        });
    }


    public static IElement<ScrollContentPresenter> HScrollContentPresenter(HScrollContentPresenterArgs args)
    {
        return Element<ScrollContentPresenter>.WithScope(uiScope =>
        {
            var scrollContentPresenter = new ScrollContentPresenter();
            var border = new Border
            {
                Child = scrollContentPresenter
            };

            HScrollContentPresenterCore(uiScope, scrollContentPresenter, border, args);

            return (scrollContentPresenter, border);
        });
    }

    public static IElement<ScrollContentPresenter> HScrollContentPresenter(
        Accessor<Vector>? offset = null,
        Accessor<bool>? isScrollChainingEnabled = null,
        Accessor<bool>? isScrollInertiaEnabled = null,
        Accessor<bool>? canHorizontallyScroll = null,
        Accessor<bool>? canVerticallyScroll = null,
        Accessor<SnapPointsType>? horizontalSnapPointsType = null,
        Accessor<SnapPointsType>? verticalSnapPointsType = null,
        Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
        Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
        HScrollGestureRecognizer? gestureRecognizer = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null)
    {
        return HScrollContentPresenter(new HScrollContentPresenterArgs(offset,
            isScrollChainingEnabled, isScrollInertiaEnabled,
            canHorizontallyScroll, canVerticallyScroll, horizontalSnapPointsType, verticalSnapPointsType,
            horizontalSnapPointsAlignment, verticalSnapPointsAlignment, gestureRecognizer,
            strStyle, style));
    }

    private static void HScrollContentPresenterCore(
        UiScope uiScope,
        ScrollContentPresenter scrollContentPresenter,
        Border border,
        HScrollContentPresenterArgs args)
    {
        var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
        {
            PriorityStyle = args.Style,
            StrVariants = args.StrStyle.Value.Variants
        };

        state.ApplyAccessorStyle(args.StrStyle, scrollContentPresenter, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(scrollContentPresenter, border, StyleUtil.ApplyStyle);

        if (args.Offset is not null)
        {
            scrollContentPresenter.Offset = args.Offset.Value;
            if (args.Offset.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.Offset = scope.Track(args.Offset));
        }

        if (args.IsScrollChainingEnabled is not null)
        {
            scrollContentPresenter.IsScrollChainingEnabled = args.IsScrollChainingEnabled.Value;
            if (args.IsScrollChainingEnabled.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.IsScrollChainingEnabled = scope.Track(args.IsScrollChainingEnabled));
        }

        if (args.IsScrollInertiaEnabled is not null)
        {
            ScrollViewer.SetIsScrollInertiaEnabled(scrollContentPresenter, args.IsScrollInertiaEnabled.Value);
            if (args.IsScrollInertiaEnabled.IsReactive)
                uiScope.CreateEffect(scope =>
                    ScrollViewer.SetIsScrollInertiaEnabled(scrollContentPresenter,
                        scope.Track(args.IsScrollInertiaEnabled)));
        }

        if (args.CanHorizontallyScroll is not null)
        {
            scrollContentPresenter.CanHorizontallyScroll = args.CanHorizontallyScroll.Value;
            if (args.CanHorizontallyScroll.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.CanHorizontallyScroll = scope.Track(args.CanHorizontallyScroll));
        }

        if (args.CanVerticallyScroll is not null)
        {
            scrollContentPresenter.CanVerticallyScroll = args.CanVerticallyScroll.Value;
            if (args.CanVerticallyScroll.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.CanVerticallyScroll = scope.Track(args.CanVerticallyScroll));
        }

        if (args.HorizontalSnapPointsType is not null)
        {
            scrollContentPresenter.HorizontalSnapPointsType = args.HorizontalSnapPointsType.Value;
            if (args.HorizontalSnapPointsType.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.HorizontalSnapPointsType = scope.Track(args.HorizontalSnapPointsType));
        }

        if (args.VerticalSnapPointsType is not null)
        {
            scrollContentPresenter.VerticalSnapPointsType = args.VerticalSnapPointsType.Value;
            if (args.VerticalSnapPointsType.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.VerticalSnapPointsType = scope.Track(args.VerticalSnapPointsType));
        }

        if (args.HorizontalSnapPointsAlignment is not null)
        {
            scrollContentPresenter.HorizontalSnapPointsAlignment = args.HorizontalSnapPointsAlignment.Value;
            if (args.HorizontalSnapPointsAlignment.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.HorizontalSnapPointsAlignment =
                        scope.Track(args.HorizontalSnapPointsAlignment));
        }

        if (args.VerticalSnapPointsAlignment is not null)
        {
            scrollContentPresenter.VerticalSnapPointsAlignment = args.VerticalSnapPointsAlignment.Value;
            if (args.VerticalSnapPointsAlignment.IsReactive)
                uiScope.CreateEffect(scope =>
                    scrollContentPresenter.VerticalSnapPointsAlignment =
                        scope.Track(args.VerticalSnapPointsAlignment));
        }

        if (args.GestureRecognizer is not null)
        {
            scrollContentPresenter.GestureRecognizers.Add(args.GestureRecognizer);
        }
    }
}