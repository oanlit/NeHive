using System.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HToggleSwitchProps(Scope scope, Border border, ToggleSwitch button)
    : HToggleButtonProps(scope, border, button);

public class HToggleSwitchArgs(
    Accessor<string>? text = null,
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isThreeState = null,
    Accessor<bool>? isDefault = null,
    Accessor<bool>? isCancel = null,
    Accessor<ClickMode>? clickMode = null,
    Accessor<KeyGesture>? hotKey = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onClick = null,
    Action<RoutedEventArgs>? onIsCheckedChanged = null
) : HToggleButtonArgs(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel, clickMode, hotKey, strStyle,
    style, baseInteraction, onClick, onIsCheckedChanged);

public static partial class BaseComponent
{
    public static IElement<ToggleSwitch> HToggleSwitch(
        Accessor<string>? text = null,
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isThreeState = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null,
        Action<RoutedEventArgs>? onIsCheckedChanged = null
    ) => HToggleSwitch(out _, _ => new(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel, 
        clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));
    
    public static IElement<ToggleSwitch> HToggleSwitch(
        out ToggleSwitch expose,
        Accessor<string>? text = null,
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isThreeState = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null,
        Action<RoutedEventArgs>? onIsCheckedChanged = null
    ) => HToggleSwitch(out expose, _ => new(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel, 
        clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));

    public static IElement<ToggleSwitch> HToggleSwitch(Func<HToggleSwitchProps, HToggleSwitchArgs> fn) =>
        HToggleSwitch(out _, fn);

    public static IElement<ToggleSwitch> HToggleSwitch(
        out ToggleSwitch expose,
        Func<HToggleSwitchProps, HToggleSwitchArgs> fn)
    {
        var toggle = new ToggleSwitch();
        expose = toggle;
        return Element<ToggleSwitch>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HToggleSwitchProps(uiScope, border, toggle);
            var args = fn(props);
            InternalButtonUtil.SetToggleButtonEffectCore(uiScope, toggle, border, args);
            toggle.Content = border;

            return (toggle, toggle);
        });
    }
}