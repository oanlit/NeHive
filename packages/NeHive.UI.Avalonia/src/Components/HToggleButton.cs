using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HToggleButtonProps(Scope scope, Border border, ToggleButton button) : HButtonProps(scope, border, button)
{
    public MutSignal<bool?> IsChecked
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool?>(button.IsChecked);
            BridgeAvalonia.BindPropertySignal(scope, field, button, ToggleButton.IsCheckedProperty);
            return field;
        }
    }

    public MutSignal<bool> IsThreeState
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(button.IsThreeState);
            BridgeAvalonia.BindPropertySignal(scope, field, button, ToggleButton.IsThreeStateProperty);
            return field;
        }
    }
}

public class HToggleButtonArgs(
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
) : HButtonArgs(text, isDefault, isCancel, clickMode, hotKey, strStyle, style, baseInteraction, onClick)
{
    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = bindIsChecked ?? isChecked;
    public readonly Accessor<bool>? IsThreeState = isThreeState;
    public readonly Action<RoutedEventArgs>? OnIsCheckedChanged = onIsCheckedChanged;
}

public static partial class BaseComponent
{
    public static IElement<ToggleButton> HToggleButton(
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
    ) => HToggleButton(out _, _ => new(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel,
        clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));

    public static IElement<ToggleButton> HToggleButton(
        out ToggleButton expose,
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
    ) => HToggleButton(out expose, _ => new(text, isChecked, bindIsChecked, isThreeState, isDefault, isCancel,
        clickMode, hotKey, strStyle, style, baseInteraction, onClick, onIsCheckedChanged));

    public static IElement<ToggleButton> HToggleButton(Func<HToggleButtonProps, HToggleButtonArgs> fn) =>
        HToggleButton(out _, fn);

    public static IElement<ToggleButton> HToggleButton(
        out ToggleButton expose,
        Func<HToggleButtonProps, HToggleButtonArgs> fn)
    {
        var button = new ToggleButton();
        expose = button;
        return Element<ToggleButton>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HToggleButtonProps(uiScope, border, button);
            var args = fn(props);
            InternalButtonUtil.SetToggleButtonEffectCore(uiScope, button, border, args);
            button.Content = border;
            return (button, button);
        });
    }
}

internal static partial class InternalButtonUtil
{
    internal static void SetToggleButtonEffectCore(UiScope uiScope, ToggleButton button, Border border,
        HToggleButtonArgs args)
    {
        SetButtonEffectCore(uiScope, button, border, args);
        button.Content = border;

        if (args.BindIsChecked is not null)
            BridgeAvalonia.BindPropertySignal(uiScope, args.BindIsChecked, button, ToggleButton.IsCheckedProperty);
        else if (args.IsChecked is not null)
        {
            button.IsChecked = args.IsChecked.Value;
            if (args.IsChecked.IsReactive)
                uiScope.CreateEffect(epoch => button.IsChecked = epoch.Track(args.IsChecked));
        }

        if (args.IsThreeState is not null)
        {
            button.IsThreeState = args.IsThreeState.Value;
            if (args.IsThreeState.IsReactive)
                uiScope.CreateEffect(epoch => button.IsThreeState = epoch.Track(args.IsThreeState));
        }

        if (args.OnIsCheckedChanged is not null)
        {
            uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                add: h => button.IsCheckedChanged += h,
                remove: h => button.IsCheckedChanged -= h,
                handler: (_, e) => args.OnIsCheckedChanged(e)
            );
        }
    }
}