using Avalonia;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Styling;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public class HFlyoutProp(
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

    public required Func<Control, Flyout, IElement> Host { get; init; }
    public Func<Flyout, IElement>? Content { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<Flyout> HFlyout(HFlyoutProp prop)
    {
        var uiScope = new UiScope();

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

        if (prop.Content is not null)
        {
            var content = prop.Content(flyout).Content;
            flyout.Content = content;
        }

        // var flyoutPresenter = new FlyoutPresenter();
        // var presenter = flyout.FlyoutPresenterClasses;

        var horizontalOffset = prop.HorizontalOffset;
        if (horizontalOffset is not null)
        {
            flyout.HorizontalOffset = horizontalOffset.Value;
            if (horizontalOffset.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.HorizontalOffset = scope.Track(horizontalOffset));
            }
        }

        var verticalOffset = prop.VerticalOffset;
        if (verticalOffset is not null)
        {
            flyout.VerticalOffset = verticalOffset.Value;
            if (verticalOffset.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.HorizontalOffset = scope.Track(verticalOffset));
            }
        }

        var showMode = prop.ShowMode;
        if (showMode is not null)
        {
            flyout.ShowMode = showMode.Value;
            if (showMode.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.ShowMode = scope.Track(showMode));
            }
        }

        var placement = prop.Placement;
        if (placement is not null)
        {
            flyout.Placement = placement.Value;
            if (placement.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.Placement = scope.Track(placement));
            }
        }

        var placementGravity = prop.PlacementGravity;
        if (placementGravity is not null)
        {
            flyout.PlacementGravity = placementGravity.Value;
            if (placementGravity.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.PlacementGravity = scope.Track(placementGravity));
            }
        }

        var placementAnchor = prop.PlacementAnchor;
        if (placementAnchor is not null)
        {
            flyout.PlacementAnchor = placementAnchor.Value;
            if (placementAnchor.IsReactive)
            {
                uiScope.CreateEffect(scope => flyout.PlacementAnchor = scope.Track(placementAnchor));
            }
        }

        var customPopupPlacementCallback = prop.CustomPopupPlacementCallback;
        if (customPopupPlacementCallback is not null)
        {
            flyout.CustomPopupPlacementCallback = customPopupPlacementCallback.Value;
            if (customPopupPlacementCallback.IsReactive)
            {
                uiScope.CreateEffect(scope =>
                    flyout.CustomPopupPlacementCallback = scope.Track(customPopupPlacementCallback));
            }
        }

        var overlayDismissEventPassThrough = prop.OverlayDismissEventPassThrough;
        if (overlayDismissEventPassThrough is not null)
        {
            flyout.OverlayDismissEventPassThrough = overlayDismissEventPassThrough.Value;
            if (overlayDismissEventPassThrough.IsReactive)
            {
                uiScope.CreateEffect(scope =>
                    flyout.OverlayDismissEventPassThrough = scope.Track(overlayDismissEventPassThrough));
            }
        }

        var overlayInputPassThroughElement = prop.OverlayInputPassThroughElement;
        if (overlayInputPassThroughElement is not null)
        {
            flyout.OverlayInputPassThroughElement = overlayInputPassThroughElement.Value;
            if (overlayInputPassThroughElement.IsReactive)
            {
                uiScope.CreateEffect(scope =>
                    flyout.OverlayInputPassThroughElement = scope.Track(overlayInputPassThroughElement));
            }
        }

        var placementConstraintAdjustment = prop.PlacementConstraintAdjustment;
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
        var hostContent = prop.Host(host, flyout).Content;
        host.Child = hostContent;

        return new Element<Flyout>(uiScope, host, flyout);
    }
}