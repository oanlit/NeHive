using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HCheckBoxProps(Scope scope, Border border, CheckBox checkBox)
    : HToggleButtonProps(scope, border, checkBox);

public class HCheckBoxArgs(
    Accessor<string>? text = null,
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isThreeState = null,
    Accessor<ClickMode>? clickMode = null,
    Accessor<KeyGesture>? hotKey = null,
    Accessor<bool>? isDefault = null,
    Accessor<bool>? isCancel = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<RoutedEventArgs>? onClick = null,
    Action<RoutedEventArgs>? onIsCheckedChanged = null,
    BaseComponentInteraction? baseInteraction = null
) : HToggleButtonArgs(text, isChecked, bindIsChecked, isThreeState, clickMode, hotKey, isDefault, isCancel,
    strStyle, style, onClick, onIsCheckedChanged, baseInteraction);

public static partial class BaseComponent
{
    public static IElement<CheckBox> HCheckBox(
        Accessor<string>? text = null,
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isThreeState = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        Action<RoutedEventArgs>? onIsCheckedChanged = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HCheckBox(out _,
            _ => new(text, isChecked, bindIsChecked, isThreeState, clickMode, hotKey, isDefault, isCancel,
                strStyle, style,
                onClick, onIsCheckedChanged, baseInteraction));

    public static IElement<CheckBox> HCheckBox(
        out CheckBox expose,
        Accessor<string>? text = null,
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isThreeState = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        Action<RoutedEventArgs>? onIsCheckedChanged = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HCheckBox(out expose,
            _ => new(text, isChecked, bindIsChecked, isThreeState, clickMode, hotKey, isDefault, isCancel,
                strStyle, style,
                onClick, onIsCheckedChanged, baseInteraction));

    public static IElement<CheckBox> HCheckBox(Func<HCheckBoxProps, HCheckBoxArgs> fn) =>
        HCheckBox(out _, fn);

    public static IElement<CheckBox> HCheckBox(
        out CheckBox expose,
        Func<HCheckBoxProps, HCheckBoxArgs> fn)
    {
        var checkBox = new CheckBox();
        expose = checkBox;
        return Element<CheckBox>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HCheckBoxProps(uiScope, border, checkBox);
            var args = fn(props);
            InternalButtonUtil.SetToggleButtonEffectCore(uiScope, checkBox, border, args);
            checkBox.Content = border;
            return (checkBox, checkBox);
        });
    }
}