using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using Avalonia.Interactivity;
using Avalonia.Input.TextInput;

namespace NeHive.UI.Avalonia.Components;

public interface ISingleChildrenArgs : IEnumerable<IElement>
{
    public void Add(IElement element);
}

public class BaseComponentProps(Scope scope, Border border, Control control)
{
    public readonly StyleProps Style = new(scope, border, control);
    public readonly Control Content = control;

    public Signal<bool> IsEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsEnabled);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsEffectivelyEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsEffectivelyEnabled);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsEffectivelyEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsHitTestVisible
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsHitTestVisible);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsHitTestVisibleProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsPointerOver
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(border.IsPointerOver);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsPointerOverProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsFocusable
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.Focusable);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.FocusableProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsFocused
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsFocused);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsFocusedProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsKeyboardFocusWithin
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsKeyboardFocusWithin);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsKeyboardFocusWithinProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsTabStop
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsTabStop);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsTabStopProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
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

    public Signal<object?> Tag
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<object?>(Content.Tag);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Control.TagProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
}

public class BaseComponentArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null)
{
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);
    public readonly BaseComponentInteraction? BaseInteraction = baseInteraction;
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
            if(IsFocusable.IsReactive)
                scope.CreateEffect(epoch=> element.Focusable = epoch.Track(IsFocusable));
        }
        
        if (IsTabStop is not null)
        {
            element.IsTabStop = IsTabStop.Value;
            if(IsTabStop.IsReactive)
                scope.CreateEffect(epoch=> element.IsTabStop = epoch.Track(IsTabStop));
        }
        
        if (IsEnabled is not null)
        {
            element.IsEnabled = IsEnabled.Value;
            if(IsEnabled.IsReactive)
                scope.CreateEffect(epoch=> element.IsEnabled = epoch.Track(IsEnabled));
        }
        
        if (IsHitTestVisible is not null)
        {
            element.IsHitTestVisible = IsHitTestVisible.Value;
            if(IsHitTestVisible.IsReactive)
                scope.CreateEffect(epoch=> element.IsHitTestVisible = epoch.Track(IsHitTestVisible));
        }
        
        if (OnPointerEntered is not null)
        {
            element.PointerEntered += PointerEnteredHandler;
            scope.OnCleanup += () => element.PointerEntered -= PointerEnteredHandler;
        }

        if (OnPointerExited is not null)
        {
            element.PointerExited += PointerExitedHandler;
            scope.OnCleanup += () => element.PointerExited -= PointerExitedHandler;
        }

        if (OnPointerMoved is not null)
        {
            element.PointerMoved += PointerMovedHandler;
            scope.OnCleanup += () => element.PointerMoved -= PointerMovedHandler;
        }

        if (OnPointerPressed is not null)
        {
            element.PointerPressed += PointerPressedHandler;
            scope.OnCleanup += () => element.PointerPressed -= PointerPressedHandler;
        }

        if (OnPointerReleased is not null)
        {
            element.PointerReleased += PointerReleasedHandler;
            scope.OnCleanup += () => element.PointerReleased -= PointerReleasedHandler;
        }

        if (OnPointerCaptureLost is not null)
        {
            element.PointerCaptureLost += PointerCaptureLostHandler;
            scope.OnCleanup += () => element.PointerCaptureLost -= PointerCaptureLostHandler;
        }

        if (OnPointerWheelChanged is not null)
        {
            element.PointerWheelChanged += PointerWheelChangedHandler;
            scope.OnCleanup += () => element.PointerWheelChanged -= PointerWheelChangedHandler;
        }

        if (OnGotFocus is not null)
        {
            element.GotFocus += GotFocusHandler;
            scope.OnCleanup += () => element.GotFocus -= GotFocusHandler;
        }

        if (OnGettingFocus is not null)
        {
            element.GettingFocus += GettingFocusHandler;
            scope.OnCleanup += () => element.GettingFocus -= GettingFocusHandler;
        }

        if (OnLostFocus is not null)
        {
            element.LostFocus += LostFocusHandler;
            scope.OnCleanup += () => element.LostFocus -= LostFocusHandler;
        }

        if (OnLosingFocus is not null)
        {
            element.LosingFocus += LosingFocusHandler;
            scope.OnCleanup += () => element.LosingFocus -= LosingFocusHandler;
        }

        if (OnKeyDown is not null)
        {
            element.KeyDown += KeyDownHandler;
            scope.OnCleanup += () => element.KeyDown -= KeyDownHandler;
        }

        if (OnKeyUp is not null)
        {
            element.KeyUp += KeyUpHandler;
            scope.OnCleanup += () => element.KeyUp -= KeyUpHandler;
        }

        if (OnTextInput is not null)
        {
            element.TextInput += TextInputHandler;
            scope.OnCleanup += () => element.TextInput -= TextInputHandler;
        }

        if (OnTextInputMethodClientRequested is not null)
        {
            element.TextInputMethodClientRequested += TextInputMethodClientRequestedHandler;
            scope.OnCleanup += () => element.TextInputMethodClientRequested -= TextInputMethodClientRequestedHandler;
        }
    }

    private void PointerEnteredHandler(object? _, PointerEventArgs args)
        => OnPointerEntered!.Invoke(args);

    private void PointerExitedHandler(object? _, PointerEventArgs args)
        => OnPointerExited!.Invoke(args);

    private void PointerMovedHandler(object? _, PointerEventArgs args)
        => OnPointerMoved!.Invoke(args);

    private void PointerPressedHandler(object? _, PointerPressedEventArgs args)
        => OnPointerPressed!.Invoke(args);

    private void PointerReleasedHandler(object? _, PointerReleasedEventArgs args)
        => OnPointerReleased!.Invoke(args);

    private void PointerCaptureLostHandler(object? _, PointerCaptureLostEventArgs args)
        => OnPointerCaptureLost!.Invoke(args);

    private void PointerWheelChangedHandler(object? _, PointerWheelEventArgs args)
        => OnPointerWheelChanged!.Invoke(args);

    private void GotFocusHandler(object? _, FocusChangedEventArgs args)
        => OnGotFocus!.Invoke(args);

    private void GettingFocusHandler(object? _, FocusChangingEventArgs args)
        => OnGettingFocus!.Invoke(args);

    private void LostFocusHandler(object? _, FocusChangedEventArgs args)
        => OnLostFocus!.Invoke(args);

    private void LosingFocusHandler(object? _, FocusChangingEventArgs args)
        => OnLosingFocus!.Invoke(args);

    private void KeyDownHandler(object? _, KeyEventArgs args)
        => OnKeyDown!.Invoke(args);

    private void KeyUpHandler(object? _, KeyEventArgs args)
        => OnKeyUp!.Invoke(args);

    private void TextInputHandler(object? _, TextInputEventArgs args)
        => OnTextInput!.Invoke(args);

    private void TextInputMethodClientRequestedHandler(object? _, TextInputMethodClientRequestedEventArgs args)
        => OnTextInputMethodClientRequested!.Invoke(args);
}