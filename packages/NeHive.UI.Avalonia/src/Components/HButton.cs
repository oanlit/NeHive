using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public static class HButtonStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontSize = 12,
        Foreground = Brushes.Black,
        Background = Brushes.LightGray,
        FontWeight = FontWeight.Normal,
        BorderThickness = new Thickness(0),
        CornerRadius = new CornerRadius(0),
        Opacity = 1.0,
        IsVisible = true
    };
}

public class HButtonProps(Scope scope, Border border, Button button) : BaseComponentProps(scope, border, button)
{
    public Signal<bool> IsPressed
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, button,
                Button.IsPressedProperty, button.IsPressed);
            return field;
        }
    }

    public MutSignal<bool> IsDefault
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(button.IsDefault);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.IsDefaultProperty);
            return field;
        }
    }

    public MutSignal<bool> IsCancel
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(button.IsCancel);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.IsCancelProperty);
            return field;
        }
    }

    public MutSignal<ClickMode> ClickMode
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<ClickMode>(button.ClickMode);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.ClickModeProperty);
            return field;
        }
    }

    public MutSignal<KeyGesture?> HotKey
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<KeyGesture?>(button.HotKey);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.HotKeyProperty);
            return field;
        }
    }

    public MutSignal<ICommand?> Command
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<ICommand?>(button.Command);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.CommandProperty);
            return field;
        }
    }

    public MutSignal<object?> CommandParameter
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<object?>(button.Command);
            BridgeAvalonia.BindPropertySignal(scope, field, button, Button.CommandParameterProperty);
            return field;
        }
    }
}

public class HButtonArgs(
    Accessor<string>? text = null,
    Accessor<bool>? isDefault = null,
    Accessor<bool>? isCancel = null,
    Accessor<ClickMode>? clickMode = null,
    Accessor<KeyGesture>? hotKey = null,
    Accessor<ICommand>? command = null,
    Accessor<object>? commandParameter = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onClick = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<string>? Text = text;

    public readonly Accessor<bool>? IsDefault = isDefault;
    public readonly Accessor<bool>? IsCancel = isCancel;

    public readonly Accessor<ClickMode>? ClickMode = clickMode;
    public readonly Accessor<KeyGesture>? HotKey = hotKey;

    public readonly Accessor<ICommand>? Command = command;
    public readonly Accessor<object>? CommandParameter = commandParameter;

    public readonly Action<RoutedEventArgs>? OnClick = onClick;

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
    public static IElement<Button> HButton(
        Accessor<string>? text = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<ICommand>? command = null,
        Accessor<object>? commandParameter = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HButton(out _, _ => new(text, isDefault, isCancel, clickMode, hotKey, command, commandParameter,
        strStyle, style, baseInteraction, onClick));

    public static IElement<Button> HButton(
        out Button expose,
        Accessor<string>? text = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<ICommand>? command = null,
        Accessor<object>? commandParameter = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HButton(out expose, _ => new(text, isDefault, isCancel, clickMode, hotKey, command, commandParameter,
        strStyle, style, baseInteraction, onClick));

    public static IElement<Button> HButton(Func<HButtonProps, HButtonArgs> fn)
        => HButton(out _, fn);

    public static IElement<Button> HButton(
        out Button expose,
        Func<HButtonProps, HButtonArgs> fn)
    {
        var button = new Button();
        expose = button;
        return Element<Button>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HButtonProps(uiScope, border, button);
            var args = fn(props);
            InternalButtonUtil.SetButtonEffectCore(uiScope, button, border, args);
            button.Template = new FuncControlTemplate((_, _) => border);
            return (button, button);
        });
    }
}

internal static partial class InternalButtonUtil
{
    internal static void SetButtonEffectCore(UiScope uiScope, Button button, Border border, HButtonArgs args)
    {
        var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
        {
            PriorityStyle = args.Style,
            StrVariants = args.StrStyle.Value.Variants
        };

        if (args.Count() is 0)
        {
            var textBlock = new TextBlock();

            border.Child = textBlock;

            state.ApplyAccessorStyle(args.StrStyle, textBlock, border,
                (styleValue, _, bord) => ApplyTextStyle(styleValue, textBlock, bord));
            state.ApplyVariantsStyle(textBlock, border,
                (styleValue, _, bord) => ApplyTextStyle(styleValue, textBlock, bord));

            if (args.Text is not null)
            {
                textBlock.Text = args.Text.Value;
                button.Content = args.Text.Value;
                if (args.Text.IsReactive)
                    uiScope.CreateEffect(epoch =>
                    {
                        var t = epoch.Track(args.Text);
                        textBlock.Text = t;
                        button.Content = t;
                    });
            }
        }
        else
        {
            var content = ElementUtil.WrapSingleContainerContent(args).Content;
            border.Child = content;

            state.ApplyAccessorStyle(args.StrStyle, content, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);
        }

        if (args.Popups is not null)
            ElementUtil.ApplyPopups(button, args.Popups);

        args.BaseInteraction?.ApplyInteractions(uiScope, button);

        if (args.IsDefault is not null)
        {
            button.IsDefault = args.IsDefault.Value;
            if (args.IsDefault.IsReactive)
                uiScope.CreateEffect(epoch => button.IsDefault = epoch.Track(args.IsDefault));
        }

        if (args.IsCancel is not null)
        {
            button.IsCancel = args.IsCancel.Value;
            if (args.IsCancel.IsReactive)
                uiScope.CreateEffect(epoch => button.IsCancel = epoch.Track(args.IsCancel));
        }

        if (args.ClickMode is not null)
        {
            button.ClickMode = args.ClickMode.Value;
            if (args.ClickMode.IsReactive)
                uiScope.CreateEffect(epoch => button.ClickMode = epoch.Track(args.ClickMode));
        }

        if (args.HotKey is not null)
        {
            button.HotKey = args.HotKey.Value;
            if (args.HotKey.IsReactive)
                uiScope.CreateEffect(epoch => button.HotKey = epoch.Track(args.HotKey));
        }

        if (args.Command is not null)
        {
            ICommand command;

            if (args.Command.IsReactive)
            {
                uiScope.CreateEffect(epoch =>
                {
                    command = epoch.Track(args.Command);
                    if (command is INeHiveCommand neHiveCommand && !neHiveCommand.IsPrototype)
                        epoch.OnCleanup += neHiveCommand.Dispose;

                    button.Command = command;
                });
            }
            else
            {
                command = args.Command.Value;
                button.Command = command;
                if (command is INeHiveCommand neHiveCommand && !neHiveCommand.IsPrototype)
                    uiScope.OnCleanup += neHiveCommand.Dispose;
            }
        }

        if (args.CommandParameter is not null)
        {
            button.CommandParameter = args.CommandParameter.Value;
            if (args.CommandParameter.IsReactive)
                uiScope.CreateEffect(epoch => button.CommandParameter = epoch.Track(args.CommandParameter));
        }

        if (args.OnClick is not null)
        {
            uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                add: h => button.Click += h,
                remove: h => button.Click -= h,
                handler: (_, e) => args.OnClick(e)
            );
        }

        return;

        void ApplyTextStyle(StyleSet styleValue, TextBlock tb, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, tb, bord);

            if (styleValue.TextAlignment is not null) tb.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.VerticalTextAlignment is not null)
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.TextWrapping is not null) tb.TextWrapping = styleValue.TextWrapping.Value;
            if (styleValue.Foreground is not null) tb.Foreground = styleValue.Foreground;

            if (styleValue.FontSize is not null) tb.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) tb.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) tb.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) tb.Foreground = styleValue.Foreground;
        }
    }
}