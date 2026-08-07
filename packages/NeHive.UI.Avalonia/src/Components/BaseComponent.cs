using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using Avalonia.Interactivity;
using Avalonia.Input.TextInput;
using NeHive.UI.Avalonia.Objects;

namespace NeHive.UI.Avalonia.Components;

public class BaseComponentProps(Scope scope, Border border, Control control)
{
    private readonly Dictionary<AvaloniaProperty, object> _attachProperty = new();

    public readonly StyleProps Style = new(scope, border, control);
    public readonly Control Content = control;

    public MutSignal<T> AttachProperty<T>(AvaloniaProperty<T> property)
    {
        if (_attachProperty.TryGetValue(property, out var value))
            return (MutSignal<T>)value;

        var signal = new MutSignal<T>((T)control.GetValue(property)!);
        BridgeAvalonia.BindPropertySignal(scope, signal, border, property);
        BridgeAvalonia.BindPropertySignal(scope, signal, control, property);
        _attachProperty[property] = signal;
        return signal;
    }

    public Signal<Rect> Bounds
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.BoundsProperty, border.Bounds);
            return field;
        }
    }

    public MutSignal<bool> IsEnabled
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(border.IsEnabled);
            BridgeAvalonia.BindPropertySignal(scope, field, border, InputElement.IsEnabledProperty);
            return field;
        }
    }

    public Signal<bool> IsEffectivelyEnabled
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                InputElement.IsEffectivelyEnabledProperty, border.IsEffectivelyEnabled);
            return field;
        }
    }

    public MutSignal<bool> IsHitTestVisible
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(Content.IsHitTestVisible);
            BridgeAvalonia.BindPropertySignal(scope, field, Content, InputElement.IsHitTestVisibleProperty);
            return field;
        }
    }

    public Signal<bool> IsPointerOver
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                InputElement.IsPointerOverProperty, border.IsPointerOver);
            return field;
        }
    }

    public MutSignal<bool> IsFocusable
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(Content.Focusable);
            BridgeAvalonia.BindPropertySignal(scope, field, Content, InputElement.FocusableProperty);
            return field;
        }
    }

    public Signal<bool> IsFocused
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, Content,
                InputElement.IsFocusedProperty, Content.IsFocused);
            return field;
        }
    }

    public Signal<bool> IsKeyboardFocusWithin
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, Content,
                InputElement.IsKeyboardFocusWithinProperty, Content.IsKeyboardFocusWithin);
            return field;
        }
    }

    public MutSignal<bool> IsTabStop
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(Content.IsTabStop);
            BridgeAvalonia.BindPropertySignal(scope, field, Content, InputElement.IsTabStopProperty);
            return field;
        }
    }

    public Signal<bool> IsContextRequested
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(false);
            field = sig;

            Content.ContextRequested += SetTrueState;
            scope.OnCleanup += () => Content.ContextRequested -= SetTrueState;
            Content.ContextCanceled += SetFalseState;
            scope.OnCleanup += () => Content.ContextCanceled -= SetFalseState;

            return field;

            void SetTrueState(object? _, ContextRequestedEventArgs args)
            {
                sig.RxValue = true;
            }

            void SetFalseState(object? _, RoutedEventArgs args)
            {
                sig.RxValue = false;
            }
        }
    }

    public MutSignal<object?> Tag
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<object?>(Content.Tag);
            BridgeAvalonia.BindPropertySignal(scope, field, Content, Control.TagProperty);
            return field;
        }
    }
}

public interface ISingleChildrenArgs : IEnumerable<IElement>
{
    public void Add(IElement element);
}

public class BaseComponentArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null)
{
    public Accessor<FullStyle> StrStyle { get; protected set; } = StyleParser.ParseFull(strStyle);
    public Signal<StyleSet>? Style { get; protected set; } = style is null ? null : StyleUtil.HStyle2Signal(style);
    public BaseComponentInteraction? BaseInteraction { get; protected set; } = baseInteraction;
    public List<IElement<Popup>>? Popups { internal get; init; }
}

public sealed class BaseComponentInteraction(
    Accessor<bool>? isFocusable = null,
    Accessor<bool>? isTabStop = null,
    Accessor<bool>? isEnabled = null,
    Accessor<bool>? isHitTestVisible = null,
    Action<RoutedEventArgs>? onPointerEntered = null,
    Action<RoutedEventArgs>? onPointerExited = null,
    Action<PointerEventArgs>? onPointerMoved = null,
    Action<PointerPressedEventArgs>? onPointerPressed = null,
    Action<PointerReleasedEventArgs>? onPointerReleased = null,
    Action<PointerCaptureLostEventArgs>? onPointerCaptureLost = null,
    Action<PointerWheelEventArgs>? onPointerWheelChanged = null,
    Action<FocusChangedEventArgs>? onGotFocus = null,
    Action<FocusChangingEventArgs>? onGettingFocus = null,
    Action<FocusChangedEventArgs>? onLostFocus = null,
    Action<FocusChangingEventArgs>? onLosingFocus = null,
    Action<KeyEventArgs>? onKeyDown = null,
    Action<KeyEventArgs>? onKeyUp = null,
    Action<TextInputEventArgs>? onTextInput = null,
    Action<TextInputMethodClientRequestedEventArgs>? onTextInputMethodClientRequested = null)
{
    public readonly Accessor<bool>? IsFocusable = isFocusable;
    public readonly Accessor<bool>? IsTabStop = isTabStop;
    public readonly Accessor<bool>? IsEnabled = isEnabled;
    public readonly Accessor<bool>? IsHitTestVisible = isHitTestVisible;

    public readonly Action<PointerEventArgs>? OnPointerEntered = onPointerEntered;
    public readonly Action<PointerEventArgs>? OnPointerExited = onPointerExited;
    public readonly Action<PointerEventArgs>? OnPointerMoved = onPointerMoved;

    public readonly Action<PointerPressedEventArgs>? OnPointerPressed = onPointerPressed;
    public readonly Action<PointerReleasedEventArgs>? OnPointerReleased = onPointerReleased;
    public readonly Action<PointerCaptureLostEventArgs>? OnPointerCaptureLost = onPointerCaptureLost;
    public readonly Action<PointerWheelEventArgs>? OnPointerWheelChanged = onPointerWheelChanged;

    public readonly Action<FocusChangedEventArgs>? OnGotFocus = onGotFocus;
    public readonly Action<FocusChangingEventArgs>? OnGettingFocus = onGettingFocus;
    public readonly Action<FocusChangedEventArgs>? OnLostFocus = onLostFocus;
    public readonly Action<FocusChangingEventArgs>? OnLosingFocus = onLosingFocus;

    public readonly Action<KeyEventArgs>? OnKeyDown = onKeyDown;
    public readonly Action<KeyEventArgs>? OnKeyUp = onKeyUp;

    public readonly Action<TextInputEventArgs>? OnTextInput = onTextInput;

    public readonly Action<TextInputMethodClientRequestedEventArgs>? OnTextInputMethodClientRequested =
        onTextInputMethodClientRequested;

    public void ApplyInteractions(Scope scope, InputElement element)
    {
        if (IsFocusable is not null)
        {
            element.Focusable = IsFocusable.Value;
            if (IsFocusable.IsReactive)
                scope.CreateEffect(epoch => element.Focusable = epoch.Track(IsFocusable));
        }

        if (IsTabStop is not null)
        {
            element.IsTabStop = IsTabStop.Value;
            if (IsTabStop.IsReactive)
                scope.CreateEffect(epoch => element.IsTabStop = epoch.Track(IsTabStop));
        }

        if (IsEnabled is not null)
        {
            element.IsEnabled = IsEnabled.Value;
            if (IsEnabled.IsReactive)
                scope.CreateEffect(epoch => element.IsEnabled = epoch.Track(IsEnabled));
        }

        if (IsHitTestVisible is not null)
        {
            element.IsHitTestVisible = IsHitTestVisible.Value;
            if (IsHitTestVisible.IsReactive)
                scope.CreateEffect(epoch => element.IsHitTestVisible = epoch.Track(IsHitTestVisible));
        }

        if (OnPointerEntered is not null)
        {
            scope.AddHandler<EventHandler<PointerEventArgs>>(
                add: h => element.PointerEntered += h,
                remove: h => element.PointerEntered -= h,
                handler: (_, args) => OnPointerEntered(args)
            );
        }

        if (OnPointerExited is not null)
        {
            scope.AddHandler<EventHandler<PointerEventArgs>>(
                add: h => element.PointerExited += h,
                remove: h => element.PointerExited -= h,
                handler: (_, args) => OnPointerExited(args)
            );
        }

        if (OnPointerMoved is not null)
        {
            scope.AddHandler<EventHandler<PointerEventArgs>>(
                add: h => element.PointerMoved += h,
                remove: h => element.PointerMoved -= h,
                handler: (_, args) => OnPointerMoved(args)
            );
        }

        if (OnPointerPressed is not null)
        {
            scope.AddHandler<EventHandler<PointerPressedEventArgs>>(
                add: h => element.PointerPressed += h,
                remove: h => element.PointerPressed -= h,
                handler: (_, args) => OnPointerPressed(args)
            );
        }

        if (OnPointerReleased is not null)
        {
            scope.AddHandler<EventHandler<PointerReleasedEventArgs>>(
                add: h => element.PointerReleased += h,
                remove: h => element.PointerReleased -= h,
                handler: (_, args) => OnPointerReleased(args)
            );
        }

        if (OnPointerCaptureLost is not null)
        {
            scope.AddHandler<EventHandler<PointerCaptureLostEventArgs>>(
                add: h => element.PointerCaptureLost += h,
                remove: h => element.PointerCaptureLost -= h,
                handler: (_, args) => OnPointerCaptureLost(args)
            );
        }

        if (OnPointerWheelChanged is not null)
        {
            scope.AddHandler<EventHandler<PointerWheelEventArgs>>(
                add: h => element.PointerWheelChanged += h,
                remove: h => element.PointerWheelChanged -= h,
                handler: (_, args) => OnPointerWheelChanged(args)
            );
        }

        if (OnGotFocus is not null)
        {
            scope.AddHandler<EventHandler<FocusChangedEventArgs>>(
                add: h => element.GotFocus += h,
                remove: h => element.GotFocus -= h,
                handler: (_, args) => OnGotFocus(args)
            );
        }

        if (OnGettingFocus is not null)
        {
            scope.AddHandler<EventHandler<FocusChangingEventArgs>>(
                add: h => element.GettingFocus += h,
                remove: h => element.GettingFocus -= h,
                handler: (_, args) => OnGettingFocus(args)
            );
        }

        if (OnLostFocus is not null)
        {
            scope.AddHandler<EventHandler<FocusChangedEventArgs>>(
                add: h => element.LostFocus += h,
                remove: h => element.LostFocus -= h,
                handler: (_, args) => OnLostFocus(args)
            );
        }

        if (OnLosingFocus is not null)
        {
            scope.AddHandler<EventHandler<FocusChangingEventArgs>>(
                add: h => element.LosingFocus += h,
                remove: h => element.LosingFocus -= h,
                handler: (_, args) => OnLosingFocus(args)
            );
        }

        if (OnKeyDown is not null)
        {
            scope.AddHandler<EventHandler<KeyEventArgs>>(
                add: h => element.KeyDown += h,
                remove: h => element.KeyDown -= h,
                handler: (_, args) => OnKeyDown(args)
            );
        }

        if (OnKeyUp is not null)
        {
            scope.AddHandler<EventHandler<KeyEventArgs>>(
                add: h => element.KeyUp += h,
                remove: h => element.KeyUp -= h,
                handler: (_, args) => OnKeyUp(args)
            );
        }

        if (OnTextInput is not null)
        {
            scope.AddHandler<EventHandler<TextInputEventArgs>>(
                add: h => element.TextInput += h,
                remove: h => element.TextInput -= h,
                handler: (_, args) => OnTextInput(args)
            );
        }

        if (OnTextInputMethodClientRequested is not null)
        {
            scope.AddHandler<EventHandler<TextInputMethodClientRequestedEventArgs>>(
                add: h => element.TextInputMethodClientRequested += h,
                remove: h => element.TextInputMethodClientRequested -= h,
                handler: (_, args) => OnTextInputMethodClientRequested(args)
            );
        }
    }
}