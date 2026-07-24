using Avalonia;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Styling;
using NeHive.Reactive;
using NeHive.Model;

namespace NeHive.UI.Avalonia.Components;

public class HFlyoutProps(Scope scope, Flyout flyout)
{
    public Signal<bool> IsOpen
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(flyout.IsOpen);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == FlyoutBase.IsOpenProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<double> HorizontalOffset
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(flyout.HorizontalOffset);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.HorizontalOffsetProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> VerticalOffset
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(flyout.VerticalOffset);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.VerticalOffsetProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<FlyoutShowMode> ShowMode
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FlyoutShowMode>(flyout.ShowMode);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.ShowModeProperty)
                    sig.RxValue = (FlyoutShowMode)args.NewValue!;
            }
        }
    }

    public Signal<PlacementMode> Placement
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<PlacementMode>(flyout.Placement);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.PlacementAnchorProperty)
                    sig.RxValue = (PlacementMode)args.NewValue!;
            }
        }
    }

    public Signal<PopupGravity> PlacementGravity
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<PopupGravity>(flyout.PlacementGravity);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.PlacementGravityProperty)
                    sig.RxValue = (PopupGravity)args.NewValue!;
            }
        }
    }

    public Signal<PopupAnchor> PlacementAnchor
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<PopupAnchor>(flyout.PlacementAnchor);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.PlacementAnchorProperty)
                    sig.RxValue = (PopupAnchor)args.NewValue!;
            }
        }
    }

    public Signal<CustomPopupPlacementCallback?> CustomPopupPlacementCallback
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<CustomPopupPlacementCallback?>(flyout.CustomPopupPlacementCallback);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.CustomPopupPlacementCallbackProperty)
                    sig.RxValue = (CustomPopupPlacementCallback?)args.NewValue;
            }
        }
    }

    public Signal<bool> OverlayDismissEventPassThrough
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(flyout.OverlayDismissEventPassThrough);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.OverlayDismissEventPassThroughProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<IInputElement?> OverlayInputPassThroughElement
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IInputElement?>(flyout.OverlayInputPassThroughElement);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.OverlayInputPassThroughElementProperty)
                    sig.RxValue = (IInputElement?)args.NewValue;
            }
        }
    }

    public Signal<PopupPositionerConstraintAdjustment> PlacementConstraintAdjustment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<PopupPositionerConstraintAdjustment>(flyout.PlacementConstraintAdjustment);
            field = sig;

            flyout.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => flyout.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == PopupFlyoutBase.PlacementConstraintAdjustmentProperty)
                    sig.RxValue = (PopupPositionerConstraintAdjustment)args.NewValue!;
            }
        }
    }

    public void ShowAt(Control placementTarget) => flyout.ShowAt(placementTarget);

    public void ShowAt(Control placementTarget, bool showAtPointer) => flyout.ShowAt(placementTarget, showAtPointer);

    public void Hide() => flyout.Hide();
}

public class HFlyoutArgs(
    Accessor<double>? horizontalOffset = null,
    Accessor<double>? verticalOffset = null,
    Accessor<FlyoutShowMode>? showMode = null,
    Accessor<PlacementMode>? placement = null,
    Accessor<PopupGravity>? placementGravity = null,
    Accessor<PopupAnchor>? placementAnchor = null,
    Accessor<CustomPopupPlacementCallback>? customPopupPlacementCallback = null,
    Accessor<bool>? overlayDismissEventPassThrough = null,
    Accessor<IInputElement>? overlayInputPassThroughElement = null,
    Accessor<PopupPositionerConstraintAdjustment>? placementConstraintAdjustment = null
)
{
    public readonly Accessor<double>? HorizontalOffset = horizontalOffset;
    public readonly Accessor<double>? VerticalOffset = verticalOffset;
    public readonly Accessor<FlyoutShowMode>? ShowMode = showMode;
    public readonly Accessor<PlacementMode>? Placement = placement;
    public readonly Accessor<PopupGravity>? PlacementGravity = placementGravity;
    public readonly Accessor<PopupAnchor>? PlacementAnchor = placementAnchor;
    public readonly Accessor<CustomPopupPlacementCallback>? CustomPopupPlacementCallback = customPopupPlacementCallback;
    public readonly Accessor<bool>? OverlayDismissEventPassThrough = overlayDismissEventPassThrough;
    public readonly Accessor<IInputElement>? OverlayInputPassThroughElement = overlayInputPassThroughElement;

    public readonly Accessor<PopupPositionerConstraintAdjustment>? PlacementConstraintAdjustment =
        placementConstraintAdjustment;

    public required Func<Control, IElement> Host { get; init; }
    public IElement? Content { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<Flyout> HFlyout(Func<HFlyoutProps,HFlyoutArgs> fn)
    {
        return Element<Flyout>.WithScope(uiScope =>
        {
            var theme = new ControlTheme(typeof(FlyoutPresenter));
            theme.Setters.Add(new Setter(
                TemplatedControl.BorderThicknessProperty, new Thickness(0)));
            theme.Setters.Add(new Setter(
                TemplatedControl.BackgroundProperty, Brushes.Transparent));
            theme.Setters.Add(new Setter(
                TemplatedControl.PaddingProperty, new Thickness(8)));

            var flyout = new Flyout
            {
                FlyoutPresenterTheme = theme
            };

            var props = new HFlyoutProps(uiScope, flyout);
            var args = fn(props);

            if (args.Content is not null)
            {
                var content = args.Content.Content;
                flyout.Content = content;
            }

            // var flyoutPresenter = new FlyoutPresenter();
            // var presenter = flyout.FlyoutPresenterClasses;

            var horizontalOffset = args.HorizontalOffset;
            if (horizontalOffset is not null)
            {
                flyout.HorizontalOffset = horizontalOffset.Value;
                if (horizontalOffset.IsReactive)
                {
                    uiScope.CreateEffect(epoch => flyout.HorizontalOffset = epoch.Track(horizontalOffset));
                }
            }

            var verticalOffset = args.VerticalOffset;
            if (verticalOffset is not null)
            {
                flyout.VerticalOffset = verticalOffset.Value;
                if (verticalOffset.IsReactive)
                {
                    uiScope.CreateEffect(epoch => flyout.HorizontalOffset = epoch.Track(verticalOffset));
                }
            }

            var showMode = args.ShowMode;
            if (showMode is not null)
            {
                flyout.ShowMode = showMode.Value;
                if (showMode.IsReactive)
                {
                    uiScope.CreateEffect(epoch => flyout.ShowMode = epoch.Track(showMode));
                }
            }

            var placement = args.Placement;
            if (placement is not null)
            {
                flyout.Placement = placement.Value;
                if (placement.IsReactive)
                {
                    uiScope.CreateEffect(epoch => flyout.Placement = epoch.Track(placement));
                }
            }

            var placementGravity = args.PlacementGravity;
            if (placementGravity is not null)
            {
                flyout.PlacementGravity = placementGravity.Value;
                if (placementGravity.IsReactive)
                {
                    uiScope.CreateEffect(scope => flyout.PlacementGravity = scope.Track(placementGravity));
                }
            }

            var placementAnchor = args.PlacementAnchor;
            if (placementAnchor is not null)
            {
                flyout.PlacementAnchor = placementAnchor.Value;
                if (placementAnchor.IsReactive)
                {
                    uiScope.CreateEffect(scope => flyout.PlacementAnchor = scope.Track(placementAnchor));
                }
            }

            var customPopupPlacementCallback = args.CustomPopupPlacementCallback;
            if (customPopupPlacementCallback is not null)
            {
                flyout.CustomPopupPlacementCallback = customPopupPlacementCallback.Value;
                if (customPopupPlacementCallback.IsReactive)
                {
                    uiScope.CreateEffect(scope =>
                        flyout.CustomPopupPlacementCallback = scope.Track(customPopupPlacementCallback));
                }
            }

            var overlayDismissEventPassThrough = args.OverlayDismissEventPassThrough;
            if (overlayDismissEventPassThrough is not null)
            {
                flyout.OverlayDismissEventPassThrough = overlayDismissEventPassThrough.Value;
                if (overlayDismissEventPassThrough.IsReactive)
                {
                    uiScope.CreateEffect(scope =>
                        flyout.OverlayDismissEventPassThrough = scope.Track(overlayDismissEventPassThrough));
                }
            }

            var overlayInputPassThroughElement = args.OverlayInputPassThroughElement;
            if (overlayInputPassThroughElement is not null)
            {
                flyout.OverlayInputPassThroughElement = overlayInputPassThroughElement.Value;
                if (overlayInputPassThroughElement.IsReactive)
                {
                    uiScope.CreateEffect(scope =>
                        flyout.OverlayInputPassThroughElement = scope.Track(overlayInputPassThroughElement));
                }
            }

            var placementConstraintAdjustment = args.PlacementConstraintAdjustment;
            if (placementConstraintAdjustment is not null)
            {
                flyout.PlacementConstraintAdjustment = placementConstraintAdjustment.Value;
                if (placementConstraintAdjustment.IsReactive)
                {
                    uiScope.CreateEffect(scope =>
                        flyout.PlacementConstraintAdjustment = scope.Track(placementConstraintAdjustment));
                }
            }

            var host = new Border();
            var hostContent = args.Host(host).Content;
            host.Child = hostContent;

            return (flyout, host);
        });
    }
}