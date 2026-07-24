using System.Collections;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HBorderArgs(
    Accessor<bool>? isAllowDrop = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
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
    Action<TextInputMethodClientRequestedEventArgs>? onTextInputMethodClientRequested = null,
    Action<DragEventArgs>? onDragEnter = null,
    Action<DragEventArgs>? onDragOver = null,
    Action<DragEventArgs>? onDragLeave = null,
    Action<DragEventArgs>? onDrop = null
) : ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsAllowDrop = isAllowDrop;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

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

    public readonly Action<DragEventArgs>? OnDragEnter = onDragEnter;
    public readonly Action<DragEventArgs>? OnDragOver = onDragOver;
    public readonly Action<DragEventArgs>? OnDragLeave = onDragLeave;
    public readonly Action<DragEventArgs>? OnDrop = onDrop;


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
    public static IElement HBorder(HBorderArgs args)
    {
        return Element.WithScope(uiScope =>
        {
            var child = ElementUtil.WrapSingleContainerContent(args);

            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = child.Content
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, border, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(border, border, StyleUtil.ApplyStyle);

            if (args.OnPointerEntered is not null) border.PointerEntered += (_, e) => args.OnPointerEntered(e);
            if (args.OnPointerExited is not null) border.PointerExited += (_, e) => args.OnPointerExited(e);
            if (args.OnPointerMoved is not null) border.PointerMoved += (_, e) => args.OnPointerMoved(e);

            if (args.OnPointerPressed is not null) border.PointerPressed += (_, e) => args.OnPointerPressed(e);
            if (args.OnPointerReleased is not null) border.PointerReleased += (_, e) => args.OnPointerReleased(e);
            if (args.OnPointerCaptureLost is not null)
                border.PointerCaptureLost += (_, e) => args.OnPointerCaptureLost(e);
            if (args.OnPointerWheelChanged is not null)
                border.PointerWheelChanged += (_, e) => args.OnPointerWheelChanged(e);

            if (args.OnGotFocus is not null) border.GotFocus += (_, e) => args.OnGotFocus(e);
            if (args.OnGettingFocus is not null) border.GettingFocus += (_, e) => args.OnGettingFocus(e);
            if (args.OnLostFocus is not null) border.LostFocus += (_, e) => args.OnLostFocus(e);
            if (args.OnLosingFocus is not null) border.LosingFocus += (_, e) => args.OnLosingFocus(e);

            if (args.OnKeyDown is not null) border.KeyDown += (_, e) => args.OnKeyDown(e);
            if (args.OnKeyUp is not null) border.KeyUp += (_, e) => args.OnKeyUp(e);
            if (args.OnTextInput is not null) border.TextInput += (_, e) => args.OnTextInput(e);
            if (args.OnTextInputMethodClientRequested is not null)
                border.TextInputMethodClientRequested += (_, e) => args.OnTextInputMethodClientRequested(e);

            if (args.IsAllowDrop is not null)
            {
                DragDrop.SetAllowDrop(border, args.IsAllowDrop.Value);
                if (args.IsAllowDrop.IsReactive)
                    uiScope.CreateEffect(scope => DragDrop.SetAllowDrop(border, scope.Track(args.IsAllowDrop)));
            }

            if (args.OnDragEnter is not null) DragDrop.AddDragEnterHandler(border, (_, e) => args.OnDragEnter(e));
            if (args.OnDragOver is not null) DragDrop.AddDragOverHandler(border, (_, e) => args.OnDragOver(e));
            if (args.OnDragLeave is not null) DragDrop.AddDragLeaveHandler(border, (_, e) => args.OnDragLeave(e));
            if (args.OnDrop is not null) DragDrop.AddDropHandler(border, (_, e) => args.OnDrop(e));

            return border;
        });
    }
}