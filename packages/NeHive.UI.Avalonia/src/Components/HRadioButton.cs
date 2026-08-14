using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HRadioButtonProps(Scope scope, Border border, RadioButton button)
    : HToggleButtonProps(scope, border, button)
{
    public MutSignal<string?> GroupName
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(button.GroupName);
            BridgeAvalonia.BindPropertySignal(scope, field, button, RadioButton.GroupNameProperty);
            return field;
        }
    }
}

public class HRadioButtonArgs(
    Accessor<string>? text = null,
    Accessor<string>? groupName = null,
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
) : HToggleButtonArgs(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel, clickMode, hotKey,
    strStyle, style, baseInteraction, onClick, onIsCheckedChanged)
{
    public readonly Accessor<string>? GroupName = groupName;
}

public static partial class BaseComponent
{
    public static IElement<RadioButton> HRadioButton(
        Accessor<string>? text = null,
        Accessor<string>? groupName = null,
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
    ) => HRadioButton(out _, _ => new(text, groupName, isChecked, bindIsChecked, isThreeState, isDefault, isCancel,
            clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));

    public static IElement<RadioButton> HRadioButton(
        out RadioButton expose,
        Accessor<string>? text = null,
        Accessor<string>? groupName = null,
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
    ) => HRadioButton(out expose, _ => new(text, groupName, isChecked, bindIsChecked, isThreeState, isDefault, isCancel,
        clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));

    public static IElement<RadioButton> HRadioButton(Func<HRadioButtonProps, HRadioButtonArgs> fn) =>
        HRadioButton(out _, fn);

    public static IElement<RadioButton> HRadioButton(out RadioButton expose,
        Func<HRadioButtonProps, HRadioButtonArgs> fn)
    {
        var radio = new RadioButton();
        expose = radio;
        return Element<RadioButton>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HRadioButtonProps(uiScope, border, radio);
            var args = fn(props);
            InternalButtonUtil.SetToggleButtonEffectCore(uiScope, radio, border, args);
            radio.Content = border;

            if (args.GroupName is not null)
            {
                radio.GroupName = args.GroupName.Value;
                if (args.GroupName.IsReactive)
                    uiScope.CreateEffect(epoch => radio.GroupName = epoch.Track(args.GroupName));
            }

            return (radio, radio);
        });
    }
}