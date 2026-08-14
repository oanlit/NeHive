using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HLinkButtonProps(Scope scope, Border border, HyperlinkButton button) : HButtonProps(scope, border, button)
{
    public MutSignal<bool> IsVisited
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(button.IsVisited);
            BridgeAvalonia.BindPropertySignal(scope, field, button, HyperlinkButton.IsVisitedProperty);
            return field;
        }
    }

    public MutSignal<Uri?> NavigateUri
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<Uri?>(button.NavigateUri);
            BridgeAvalonia.BindPropertySignal(scope, field, button, HyperlinkButton.NavigateUriProperty);
            return field;
        }
    }
}

public class HLinkButtonArgs(
    Accessor<string>? text = null,
    Accessor<bool>? isVisited = null,
    MutSignal<bool>? bindIsVisited = null,
    Accessor<Uri?>? navigateUri = null,
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
    public readonly MutSignal<bool>? BindIsVisited = bindIsVisited;
    public readonly Accessor<bool>? IsVisited = bindIsVisited ?? isVisited;
    public readonly Accessor<Uri?>? NavigateUri = navigateUri;
}

public static partial class BaseComponent
{
    public static IElement<HyperlinkButton> HLinkButton(
        Accessor<string>? text = null,
        Accessor<bool>? isVisited = null,
        MutSignal<bool>? bindIsVisited = null,
        Accessor<Uri?>? navigateUri = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HLinkButton(out _, _ => new(text, isVisited, bindIsVisited, navigateUri, isDefault, 
        isCancel, clickMode, hotKey, strStyle, style, baseInteraction, onClick));

    public static IElement<HyperlinkButton> HLinkButton(
        out HyperlinkButton expose,
        Accessor<string>? text = null,
        Accessor<bool>? isVisited = null,
        MutSignal<bool>? bindIsVisited = null,
        Accessor<Uri?>? navigateUri = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onClick = null
    ) => HLinkButton(out expose, _ => new(text, isVisited, bindIsVisited, navigateUri, isDefault, 
        isCancel, clickMode, hotKey, strStyle, style, baseInteraction, onClick));

    public static IElement<HyperlinkButton> HLinkButton(Func<HLinkButtonProps, HLinkButtonArgs> fn) =>
        HLinkButton(out _, fn);

    public static IElement<HyperlinkButton> HLinkButton(out HyperlinkButton expose,
        Func<HLinkButtonProps, HLinkButtonArgs> fn)
    {
        var button = new HyperlinkButton();
        expose = button;
        return Element<HyperlinkButton>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HLinkButtonProps(uiScope, border, button);
            var args = fn(props);
            InternalButtonUtil.SetButtonEffectCore(uiScope, button, border, args);
            button.Template = new FuncControlTemplate((_, _) => border);

            if (args.BindIsVisited is not null)
            {
                button.IsVisited = args.BindIsVisited.Value;
                uiScope.CreateEffect(epoch => button.IsVisited = epoch.Pull(args.BindIsVisited));
                button.PropertyChanged += UpdateIsVisited;
                uiScope.OnCleanup += () => button.PropertyChanged -= UpdateIsVisited;
            }
            else if (args.IsVisited is not null)
            {
                button.IsVisited = args.IsVisited.Value;
                if (args.IsVisited.IsReactive)
                    uiScope.CreateEffect(epoch => button.IsVisited = epoch.Track(args.IsVisited));
            }

            if (args.NavigateUri is not null)
            {
                button.NavigateUri = args.NavigateUri.Value;
                if (args.NavigateUri.IsReactive)
                    uiScope.CreateEffect(epoch => button.NavigateUri = epoch.Track(args.NavigateUri));
            }

            return (button, button);

            void UpdateIsVisited(object? _, AvaloniaPropertyChangedEventArgs e)
            {
                if (e.Property == HyperlinkButton.IsVisitedProperty)
                    args.BindIsVisited!.RxValue = (bool)e.NewValue!;
            }
        });
    }
}