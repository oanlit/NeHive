using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HRepeatButtonProps(Scope scope, Border border, RepeatButton button) : HButtonProps(scope, border, button)
{
    public MutSignal<int> Interval
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(button.Interval);
            BridgeAvalonia.BindPropertySignal(scope, field, button, RepeatButton.IntervalProperty);
            return field;
        }
    }

    public MutSignal<int> Delay
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(button.Delay);
            BridgeAvalonia.BindPropertySignal(scope, field, button, RepeatButton.IntervalProperty);
            return field;
        }
    }
}

public class HRepeatButtonArgs(
    Accessor<string>? text = null,
    Accessor<int>? interval = null,
    Accessor<int>? delay = null,
    Accessor<bool>? isDefault = null,
    Accessor<bool>? isCancel = null,
    Accessor<ClickMode>? clickMode = null,
    Accessor<KeyGesture>? hotKey = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onClick = null
) : HButtonArgs(text, isDefault, isCancel, clickMode, hotKey, strStyle, style, baseInteraction, onClick)
{
    public readonly Accessor<int>? Interval = interval;
    public readonly Accessor<int>? Delay = delay;
}

public static partial class BaseComponent
{
    public static IElement<RepeatButton> HRepeatButton(
        Accessor<string>? text = null,
        Accessor<int>? interval = null,
        Accessor<int>? delay = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HRepeatButton(out _, _ => new(text, interval, delay, isDefault, isCancel,
        clickMode, hotKey, strStyle, style, baseInteraction, onClick));

    public static IElement<RepeatButton> HRadioButton(
        out RepeatButton expose,
        Accessor<string>? text = null,
        Accessor<int>? interval = null,
        Accessor<int>? delay = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HRepeatButton(out expose, _ => new(text, interval, delay, isDefault, isCancel,
        clickMode, hotKey, strStyle, style, baseInteraction, onClick));

    public static IElement<RepeatButton> HRepeatButton(Func<HRepeatButtonProps, HRepeatButtonArgs> fn) =>
        HRepeatButton(out _, fn);

    public static IElement<RepeatButton> HRepeatButton(out RepeatButton expose,
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