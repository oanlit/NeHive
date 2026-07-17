using Avalonia;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HScrollContentPresenterProps(Scope scope, ScrollContentPresenter scrollContentPresenter)
{
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
    Accessor<bool>? isScrollChainingEnabled = null,
    Accessor<bool>? isScrollInertiaEnabled = null,
    Accessor<bool>? canHorizontallyScroll = null,
    Accessor<bool>? canVerticallyScroll = null,
    Accessor<SnapPointsType>? horizontalSnapPointsType = null,
    Accessor<SnapPointsType>? verticalSnapPointsType = null,
    Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
    Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
    ScrollGestureRecognizer? gestureRecognizer = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null)
{
    public readonly Accessor<bool>? IsScrollChainingEnabled = isScrollChainingEnabled;
    public readonly Accessor<bool>? IsScrollInertiaEnabled = isScrollInertiaEnabled;
    public readonly Accessor<bool>? CanHorizontallyScroll = canHorizontallyScroll;
    public readonly Accessor<bool>? CanVerticallyScroll = canVerticallyScroll;
    public readonly Accessor<SnapPointsType>? HorizontalSnapPointsType = horizontalSnapPointsType;
    public readonly Accessor<SnapPointsType>? VerticalSnapPointsType = verticalSnapPointsType;
    public readonly Accessor<SnapPointsAlignment>? HorizontalSnapPointsAlignment = horizontalSnapPointsAlignment;
    public readonly Accessor<SnapPointsAlignment>? VerticalSnapPointsAlignment = verticalSnapPointsAlignment;
    
    public readonly ScrollGestureRecognizer? GestureRecognizer = gestureRecognizer;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;
}

public static partial class BaseComponent
{
    public static IElement<ScrollContentPresenter> HScrollContentPresenter(
        Accessor<bool>? isScrollChainingEnabled = null,
        Accessor<bool>? isScrollInertiaEnabled = null,
        Accessor<bool>? canHorizontallyScroll = null,
        Accessor<bool>? canVerticallyScroll = null,
        Accessor<SnapPointsType>? horizontalSnapPointsType = null,
        Accessor<SnapPointsType>? verticalSnapPointsType = null,
        Accessor<SnapPointsAlignment>? horizontalSnapPointsAlignment = null,
        Accessor<SnapPointsAlignment>? verticalSnapPointsAlignment = null,
        ScrollGestureRecognizer? gestureRecognizer = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        return Element<ScrollContentPresenter>.WithScope(uiScope =>
        {
            var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

            var scrollContentPresenter = new ScrollContentPresenter();
            var border = new Border
            {
                Child = scrollContentPresenter
            };

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                StrVariants = styleAccessor.Value.Variants,
                Variants = variants
            };

            state.ApplyAccessorStyle(styleAccessor, scrollContentPresenter, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(scrollContentPresenter, border, StyleUtil.ApplyStyle);

            if (isScrollChainingEnabled is not null)
            {
                scrollContentPresenter.IsScrollChainingEnabled = isScrollChainingEnabled.Value;
                if (isScrollChainingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.IsScrollChainingEnabled = scope.Track(isScrollChainingEnabled));
            }

            if (isScrollInertiaEnabled is not null)
            {
                ScrollViewer.SetIsScrollInertiaEnabled(scrollContentPresenter, isScrollInertiaEnabled.Value);
                if (isScrollInertiaEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        ScrollViewer.SetIsScrollInertiaEnabled(scrollContentPresenter,
                            scope.Track(isScrollInertiaEnabled)));
            }

            if (canHorizontallyScroll is not null)
            {
                scrollContentPresenter.CanHorizontallyScroll = canHorizontallyScroll.Value;
                if (canHorizontallyScroll.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.CanHorizontallyScroll = scope.Track(canHorizontallyScroll));
            }

            if (canVerticallyScroll is not null)
            {
                scrollContentPresenter.CanVerticallyScroll = canVerticallyScroll.Value;
                if (canVerticallyScroll.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.CanVerticallyScroll = scope.Track(canVerticallyScroll));
            }

            if (horizontalSnapPointsType is not null)
            {
                scrollContentPresenter.HorizontalSnapPointsType = horizontalSnapPointsType.Value;
                if (horizontalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.HorizontalSnapPointsType = scope.Track(horizontalSnapPointsType));
            }

            if (verticalSnapPointsType is not null)
            {
                scrollContentPresenter.VerticalSnapPointsType = verticalSnapPointsType.Value;
                if (verticalSnapPointsType.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.VerticalSnapPointsType = scope.Track(verticalSnapPointsType));
            }

            if (horizontalSnapPointsAlignment is not null)
            {
                scrollContentPresenter.HorizontalSnapPointsAlignment = horizontalSnapPointsAlignment.Value;
                if (horizontalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.HorizontalSnapPointsAlignment =
                            scope.Track(horizontalSnapPointsAlignment));
            }

            if (verticalSnapPointsAlignment is not null)
            {
                scrollContentPresenter.VerticalSnapPointsAlignment = verticalSnapPointsAlignment.Value;
                if (verticalSnapPointsAlignment.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollContentPresenter.VerticalSnapPointsAlignment = scope.Track(verticalSnapPointsAlignment));
            }

            if (gestureRecognizer is not null)
            {
                scrollContentPresenter.GestureRecognizers.Add(gestureRecognizer);
            }

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

            var state = new CommonState(uiScope, args.Style.Value.Normal)
            {
                StrVariants = args.Style.Value.Variants,
                Variants = args.Variants
            };

            state.ApplyAccessorStyle(args.Style, scrollContentPresenter, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(scrollContentPresenter, border, StyleUtil.ApplyStyle);

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
                        scrollContentPresenter.VerticalSnapPointsAlignment = scope.Track(args.VerticalSnapPointsAlignment));
            }

            if (args.GestureRecognizer is not null)
            {
                scrollContentPresenter.GestureRecognizers.Add(args.GestureRecognizer);
            }

            return (scrollContentPresenter, border);
        });
    }
}