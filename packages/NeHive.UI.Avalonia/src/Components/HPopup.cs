using System.Collections;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HPopupProps(Scope scope, Border border, Popup popup) : BaseComponentProps(scope, border, popup)
{
}

public class HPopupArgs(
    Accessor<bool>? isOpen = null,
    Accessor<bool>? isLightDismissEnabled = null,
    Accessor<bool>? isTopmost = null,
    Accessor<bool>? shouldUseOverlayLayer = null,
    Accessor<bool>? isInheritsTransform = null,
    Accessor<bool>? isOverlayDismissEventPassThrough = null,
    Accessor<bool>? isTakesFocusFromNativeControl = null,
    Accessor<bool>? isWindowManagerAddShadowHint = null,
    Accessor<Control>? placementTarget = null,
    Accessor<PlacementMode>? placement = null,
    Accessor<PopupAnchor>? placementAnchor = null,
    Accessor<Rect>? placementRect = null,
    Accessor<PopupPositionerConstraintAdjustment>? placementConstraintAdjustment = null,
    Accessor<PopupGravity>? placementGravity = null,
    Accessor<CustomPopupPlacementCallback>? customPopupPlacementCallback = null,
    Accessor<double>? horizontalOffset = null,
    Accessor<double>? verticalOffset = null,
    Accessor<IInputElement>? overlayInputPassThroughElement = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<EventArgs>? onOpened = null,
    Action<EventArgs>? onClosed = null
) : ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsOpen = isOpen;
    public readonly Accessor<bool>? IsLightDismissEnabled = isLightDismissEnabled;
    public readonly Accessor<bool>? IsTopmost = isTopmost;
    public readonly Accessor<bool>? ShouldUseOverlayLayer = shouldUseOverlayLayer;
    public readonly Accessor<bool>? IsInheritsTransform = isInheritsTransform;
    public readonly Accessor<bool>? IsOverlayDismissEventPassThrough = isOverlayDismissEventPassThrough;
    public readonly Accessor<bool>? IsTakesFocusFromNativeControl = isTakesFocusFromNativeControl;
    public readonly Accessor<bool>? IsWindowManagerAddShadowHint = isWindowManagerAddShadowHint;

    public readonly Accessor<Control>? PlacementTarget = placementTarget;
    public readonly Accessor<PlacementMode>? Placement = placement;
    public readonly Accessor<PopupAnchor>? PlacementAnchor = placementAnchor;
    public readonly Accessor<Rect>? PlacementRect = placementRect;
    public readonly Accessor<PopupGravity>? PlacementGravity = placementGravity;

    public readonly Accessor<PopupPositionerConstraintAdjustment>? PlacementConstraintAdjustment =
        placementConstraintAdjustment;

    public readonly Accessor<CustomPopupPlacementCallback>? CustomPopupPlacementCallback = customPopupPlacementCallback;

    public readonly Accessor<double>? HorizontalOffset = horizontalOffset;
    public readonly Accessor<double>? VerticalOffset = verticalOffset;

    public readonly Accessor<IInputElement>? OverlayInputPassThroughElement = overlayInputPassThroughElement;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public readonly Action<EventArgs>? OnOpened = onOpened;
    public readonly Action<EventArgs>? OnClosed = onClosed;

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
    {
        _children.Add(element);
    }
}

public static partial class BaseComponent
{
    public static IElement<Popup> HPopup(HPopupArgs args)
    {
        return Element<Popup>.WithScope(uiScope =>
        {
            var popup = new Popup
            {
                Child = ElementUtil.WrapSingleContainerContent(args).Content
            };
            
            var border = new Border
            {
                Background = Brushes.Transparent,
                Child = popup
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, popup, border, ApplyStyle);
            state.ApplyVariantsStyle(popup, border, ApplyStyle);

            if (args.IsOpen is not null)
            {
                popup.IsOpen = args.IsOpen.Value;
                if (args.IsOpen.IsReactive)
                    uiScope.CreateEffect(epoch => popup.IsOpen = epoch.Track(args.IsOpen));
            }

            if (args.IsLightDismissEnabled is not null)
            {
                popup.IsLightDismissEnabled = args.IsLightDismissEnabled.Value;
                if (args.IsLightDismissEnabled.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.IsLightDismissEnabled = epoch.Track(args.IsLightDismissEnabled));
            }

            if (args.IsTopmost is not null)
            {
                popup.Topmost = args.IsTopmost.Value;
                if (args.IsTopmost.IsReactive)
                    uiScope.CreateEffect(epoch => popup.Topmost = epoch.Track(args.IsTopmost));
            }

            if (args.ShouldUseOverlayLayer is not null)
            {
                popup.ShouldUseOverlayLayer = args.ShouldUseOverlayLayer.Value;
                if (args.ShouldUseOverlayLayer.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.ShouldUseOverlayLayer = epoch.Track(args.ShouldUseOverlayLayer));
            }

            if (args.IsInheritsTransform is not null)
            {
                popup.InheritsTransform = args.IsInheritsTransform.Value;
                if (args.IsInheritsTransform.IsReactive)
                    uiScope.CreateEffect(epoch => popup.InheritsTransform = epoch.Track(args.IsInheritsTransform));
            }

            if (args.IsOverlayDismissEventPassThrough is not null)
            {
                popup.OverlayDismissEventPassThrough = args.IsOverlayDismissEventPassThrough.Value;
                if (args.IsOverlayDismissEventPassThrough.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.OverlayDismissEventPassThrough = epoch.Track(args.IsOverlayDismissEventPassThrough));
            }

            if (args.IsTakesFocusFromNativeControl is not null)
            {
                popup.TakesFocusFromNativeControl = args.IsTakesFocusFromNativeControl.Value;
                if (args.IsTakesFocusFromNativeControl.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.TakesFocusFromNativeControl = epoch.Track(args.IsTakesFocusFromNativeControl));
            }

            if (args.IsWindowManagerAddShadowHint is not null)
            {
                popup.WindowManagerAddShadowHint = args.IsWindowManagerAddShadowHint.Value;
                if (args.IsWindowManagerAddShadowHint.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.WindowManagerAddShadowHint = epoch.Track(args.IsWindowManagerAddShadowHint));
            }

            if (args.PlacementTarget is not null)
            {
                popup.PlacementTarget = args.PlacementTarget.Value;
                if (args.PlacementTarget.IsReactive)
                    uiScope.CreateEffect(epoch => popup.PlacementTarget = epoch.Track(args.PlacementTarget));
            }

            if (args.Placement is not null)
            {
                popup.Placement = args.Placement.Value;
                if (args.Placement.IsReactive)
                    uiScope.CreateEffect(epoch => popup.Placement = epoch.Track(args.Placement));
            }

            if (args.PlacementAnchor is not null)
            {
                popup.PlacementAnchor = args.PlacementAnchor.Value;
                if (args.PlacementAnchor.IsReactive)
                    uiScope.CreateEffect(epoch => popup.PlacementAnchor = epoch.Track(args.PlacementAnchor));
            }

            if (args.PlacementRect is not null)
            {
                popup.PlacementRect = args.PlacementRect.Value;
                if (args.PlacementRect.IsReactive)
                    uiScope.CreateEffect(epoch => popup.PlacementRect = epoch.Track(args.PlacementRect));
            }

            if (args.PlacementGravity is not null)
            {
                popup.PlacementGravity = args.PlacementGravity.Value;
                if (args.PlacementGravity.IsReactive)
                    uiScope.CreateEffect(epoch => popup.PlacementGravity = epoch.Track(args.PlacementGravity));
            }

            if (args.PlacementConstraintAdjustment is not null)
            {
                popup.PlacementConstraintAdjustment = args.PlacementConstraintAdjustment.Value;
                if (args.PlacementConstraintAdjustment.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.PlacementConstraintAdjustment = epoch.Track(args.PlacementConstraintAdjustment));
            }

            if (args.CustomPopupPlacementCallback is not null)
            {
                popup.CustomPopupPlacementCallback = args.CustomPopupPlacementCallback.Value;
                if (args.CustomPopupPlacementCallback.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.CustomPopupPlacementCallback = epoch.Track(args.CustomPopupPlacementCallback));
            }

            if (args.HorizontalOffset is not null)
            {
                popup.HorizontalOffset = args.HorizontalOffset.Value;
                if (args.HorizontalOffset.IsReactive)
                    uiScope.CreateEffect(epoch => popup.HorizontalOffset = epoch.Track(args.HorizontalOffset));
            }

            if (args.VerticalOffset is not null)
            {
                popup.VerticalOffset = args.VerticalOffset.Value;
                if (args.VerticalOffset.IsReactive)
                    uiScope.CreateEffect(epoch => popup.VerticalOffset = epoch.Track(args.VerticalOffset));
            }

            if (args.OverlayInputPassThroughElement is not null)
            {
                popup.OverlayInputPassThroughElement = args.OverlayInputPassThroughElement.Value;
                if (args.OverlayInputPassThroughElement.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        popup.OverlayInputPassThroughElement = epoch.Track(args.OverlayInputPassThroughElement));
            }

            if (args.OnOpened is not null)
            {
                popup.Opened += OnOpened;
                uiScope.OnCleanup += () => popup.Opened -= OnOpened;
            }

            if (args.OnClosed is not null)
            {
                popup.Closed += OnClosed;
                uiScope.OnCleanup += () => popup.Opened -= OnClosed;
            }

            return (popup, popup);
            
            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                
                if(styleValue.Opacity is not null) popup.Opacity = styleValue.Opacity.Value;
                if(styleValue.OpacityMask is not null) popup.OpacityMask = styleValue.OpacityMask;
            }

            void OnOpened(object? _, EventArgs e)
            {
                args.OnOpened?.Invoke(e);
            }

            void OnClosed(object? _, EventArgs e)
            {
                args.OnClosed?.Invoke(e);
            }
        });
    }
}