using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HRepeatButtonProps(Scope scope, Border border, RepeatButton button) : HButtonProps(scope, border, button)
{
    public Signal<int> Interval
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(button.Interval);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RepeatButton.IntervalProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<int> Delay
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(button.Delay);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RepeatButton.DelayProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }
}

public class HRepeatButtonArgs(
    Accessor<string>? text = null,
    Accessor<int>? interval = null,
    Accessor<int>? delay = null,
    Accessor<ClickMode>? clickMode = null,
    Accessor<KeyGesture>? hotKey = null,
    Accessor<bool>? isDefault = null,
    Accessor<bool>? isCancel = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<RoutedEventArgs>? onClick = null,
    BaseComponentInteraction? baseInteraction = null) : HButtonArgs(text, clickMode, hotKey, isDefault,
    isCancel, strStyle, style, onClick, baseInteraction)
{
    public readonly Accessor<int>? Interval = interval;
    public readonly Accessor<int>? Delay = delay;
}

public static partial class BaseComponent
{
    public static IElement<RepeatButton> HRadioButton(
        Accessor<string>? text = null,
        Accessor<int>? interval = null,
        Accessor<int>? delay = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HRadioButton(out _,
            _ => new(text, interval, delay, clickMode, hotKey, 
                isDefault, isCancel, strStyle, style, onClick, baseInteraction));
    
    public static IElement<RepeatButton> HRadioButton(
        out RepeatButton expose,
        Accessor<string>? text = null,
        Accessor<int>? interval = null,
        Accessor<int>? delay = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HRadioButton(out expose,
            _ => new(text, interval, delay, clickMode, hotKey, 
                isDefault, isCancel, strStyle, style, onClick, baseInteraction));

    public static IElement<RepeatButton> HRadioButton(Func<HRepeatButtonProps, HRepeatButtonArgs> fn) =>
        HRadioButton(out _, fn);

    public static IElement<RepeatButton> HRadioButton(out RepeatButton expose,
        Func<HRepeatButtonProps, HRepeatButtonArgs> fn)
    {
        var button = new RepeatButton();
        expose = button;
        return Element<RepeatButton>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HRepeatButtonProps(uiScope, border, button);
            var args = fn(props);
            InternalButtonUtil.SetButtonEffectCore(uiScope, button, border, args);
            button.Template = new FuncControlTemplate((_, _) => border);
            if (args.Interval is not null)
            {
                button.Interval = args.Interval.Value;
                if (args.Interval.IsReactive)
                    uiScope.CreateEffect(epoch => button.Interval = epoch.Track(args.Interval));
            }
            if (args.Delay is not null)
            {
                button.Delay = args.Delay.Value;
                if (args.Delay.IsReactive)
                    uiScope.CreateEffect(epoch => button.Delay = epoch.Track(args.Delay));
            }

            return (button, button);
        });
    }
}