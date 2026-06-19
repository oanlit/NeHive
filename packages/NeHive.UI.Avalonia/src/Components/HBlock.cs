using System.Runtime.InteropServices.ComTypes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HBlockProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
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
    Action<TextInputMethodClientRequestedEventArgs>? onTextInputMethodClientRequested = null
)
{
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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

    public IElement? Child { get; init; }
}

public static partial class BaseComponent
{
    public static IElement HBlock(HBlockProp prop)
    {
        var uiScope = new UiScope();

        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = prop.Child?.Content
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, border, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(border, border, StyleUtil.ApplyStyle);

        if (prop.OnPointerEntered is not null) border.PointerEntered += (_, e) => prop.OnPointerEntered(e);
        if (prop.OnPointerExited is not null) border.PointerExited += (_, e) => prop.OnPointerExited(e);
        if (prop.OnPointerMoved is not null) border.PointerMoved += (_, e) => prop.OnPointerMoved(e);

        if (prop.OnPointerPressed is not null) border.PointerPressed += (_, e) => prop.OnPointerPressed(e);
        if (prop.OnPointerReleased is not null) border.PointerReleased += (_, e) => prop.OnPointerReleased(e);
        if (prop.OnPointerCaptureLost is not null) border.PointerCaptureLost += (_, e) => prop.OnPointerCaptureLost(e);
        if (prop.OnPointerWheelChanged is not null)
            border.PointerWheelChanged += (_, e) => prop.OnPointerWheelChanged(e);

        if (prop.OnGotFocus is not null) border.GotFocus += (_, e) => prop.OnGotFocus(e);
        if (prop.OnGettingFocus is not null) border.GettingFocus += (_, e) => prop.OnGettingFocus(e);
        if (prop.OnLostFocus is not null) border.LostFocus += (_, e) => prop.OnLostFocus(e);
        if (prop.OnLosingFocus is not null) border.LosingFocus += (_, e) => prop.OnLosingFocus(e);

        if (prop.OnKeyDown is not null) border.KeyDown += (_, e) => prop.OnKeyDown(e);
        if (prop.OnKeyUp is not null) border.KeyUp += (_, e) => prop.OnKeyUp(e);
        if (prop.OnTextInput is not null) border.TextInput += (_, e) => prop.OnTextInput(e);
        if (prop.OnTextInputMethodClientRequested is not null)
            border.TextInputMethodClientRequested += (_, e) => prop.OnTextInputMethodClientRequested(e);

        return new Element(uiScope, border);
    }
}