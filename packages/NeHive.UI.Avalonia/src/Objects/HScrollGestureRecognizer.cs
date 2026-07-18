using Avalonia;
using Avalonia.Input.GestureRecognizers;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Objects;

public class HScrollGestureRecognizer
{
    public readonly ScrollGestureRecognizer GestureRecognizer;

    public HScrollGestureRecognizer(
        Accessor<bool>? canHorizontallyScroll = null,
        Accessor<bool>? canVerticallyScroll = null,
        Accessor<bool>? isScrollInertiaEnabled = null,
        Accessor<int>? scrollStartDistance = null,
        Accessor<Vector>? offset = null,
        Accessor<Size>? viewport = null,
        Accessor<Size>? extent = null
    )
    {
        GestureRecognizer = new ScrollGestureRecognizer();
        var scope = new Scope();
        
        if (canHorizontallyScroll is not null)
        {
            GestureRecognizer.CanHorizontallyScroll = canHorizontallyScroll.Value;
            if (canHorizontallyScroll.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.CanHorizontallyScroll = epoch.Track(canHorizontallyScroll));
            }
        }
        
        if (canVerticallyScroll is not null)
        {
            GestureRecognizer.CanVerticallyScroll = canVerticallyScroll.Value;
            if (canVerticallyScroll.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.CanVerticallyScroll = epoch.Track(canVerticallyScroll));
            }
        }
        
        if (isScrollInertiaEnabled is not null)
        {
            GestureRecognizer.IsScrollInertiaEnabled = isScrollInertiaEnabled.Value;
            if (isScrollInertiaEnabled.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.IsScrollInertiaEnabled = epoch.Track(isScrollInertiaEnabled));
            }
        }

        if (scrollStartDistance is not null)
        {
            GestureRecognizer.ScrollStartDistance = scrollStartDistance.Value;
            if (scrollStartDistance.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.ScrollStartDistance = epoch.Track(scrollStartDistance));
            }
        }

        if (offset is not null)
        {
            GestureRecognizer.SetValue(ScrollGestureRecognizer.OffsetProperty, offset.Value);
            if (offset.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.SetValue(ScrollGestureRecognizer.OffsetProperty, epoch.Track(offset)));
            }
        }

        if (viewport is not null)
        {
            GestureRecognizer.SetValue(ScrollGestureRecognizer.ViewportProperty, viewport.Value);
            if (viewport.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.SetValue(ScrollGestureRecognizer.ViewportProperty, epoch.Track(viewport)));
            }
        }
        
        if (extent is not null)
        {
            GestureRecognizer.SetValue(ScrollGestureRecognizer.ExtentProperty, extent.Value);
            if (extent.IsReactive)
            {
                scope.CreateEffect(epoch =>
                    GestureRecognizer.SetValue(ScrollGestureRecognizer.ExtentProperty, epoch.Track(extent)));
            }
        }
    }
}