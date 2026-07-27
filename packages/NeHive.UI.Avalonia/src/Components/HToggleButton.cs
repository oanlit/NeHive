using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HToggleButtonProps(Scope scope, Border border, ToggleButton button) : HButtonProps(scope, border, button)
{
    public Signal<bool?> IsChecked
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool?>(button.IsChecked);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ToggleButton.IsCheckedProperty)
                    sig.RxValue = (bool?)args.NewValue;
            }
        }
    }

    public Signal<bool> IsThreeState
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(button.IsThreeState);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ToggleButton.IsThreeStateProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
}

public class HToggleButtonArgs(
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
    BaseComponentInteraction? baseInteraction = null) : HButtonArgs(text, clickMode, hotKey, isDefault,
    isCancel, strStyle, style, onClick, baseInteraction)
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
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        Action<RoutedEventArgs>? onIsCheckedChanged = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HToggleButton(out _,
            _ => new(text, isChecked, bindIsChecked, isThreeState, clickMode, hotKey, isDefault, isCancel,
                strStyle, style,
                onClick, onIsCheckedChanged, baseInteraction));

    public static IElement<ToggleButton> HToggleButton(
        out ToggleButton expose,
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
        HToggleButton(out expose,
            _ => new(text, isChecked, bindIsChecked, isThreeState, clickMode, hotKey, isDefault, isCancel,
                strStyle, style,
                onClick, onIsCheckedChanged, baseInteraction));

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
    internal static void SetToggleButtonEffectCore(UiScope uiScope, ToggleButton button, Border border, HToggleButtonArgs args)
    {
        SetButtonEffectCore(uiScope, button, border, args);
        button.Content = border;

        if (args.BindIsChecked is not null)
        {
            uiScope.CreateEffect(epoch => button.IsChecked = epoch.Pull(args.BindIsChecked));
            button.PropertyChanged += UpdateIsChecked;
            uiScope.OnCleanup += () => button.PropertyChanged -= UpdateIsChecked;
        }
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
            button.IsCheckedChanged += IsCheckedChangedHandler;
            uiScope.OnCleanup += () => button.IsCheckedChanged -= IsCheckedChangedHandler;
        }
        
        return;
        
        void IsCheckedChangedHandler(object? _, RoutedEventArgs e)
        {
            args.OnIsCheckedChanged!(e);
        }

        void UpdateIsChecked(object? _, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ToggleButton.IsCheckedProperty)
                args.BindIsChecked!.RxValue = (bool?)e.NewValue;
        }
    }
}