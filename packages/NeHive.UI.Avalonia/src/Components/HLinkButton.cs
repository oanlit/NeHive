using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HLinkButtonProps(Scope scope, Border border, HyperlinkButton button) : HButtonProps(scope, border, button)
{
    public Signal<bool> IsVisited
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(button.IsVisited);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == HyperlinkButton.IsVisitedProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<Uri?> NavigateUri
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Uri?>(button.NavigateUri);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == HyperlinkButton.NavigateUriProperty)
                    sig.RxValue = (Uri?)args.NewValue;
            }
        }
    }
}

public class HLinkButtonArgs(
    Accessor<string>? text = null,
    Accessor<bool>? isVisited = null,
    MutSignal<bool>? bindIsVisited = null,
    Accessor<Uri?>? navigateUri = null,
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
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HLinkButton(out _,
            _ => new(text, isVisited, bindIsVisited, navigateUri, clickMode, hotKey,
                isDefault, isCancel, strStyle, style, onClick, baseInteraction));
    
    public static IElement<HyperlinkButton> HLinkButton(
        out HyperlinkButton expose,
        Accessor<string>? text = null,
        Accessor<bool>? isVisited = null,
        MutSignal<bool>? bindIsVisited = null,
        Accessor<Uri?>? navigateUri = null,
        Accessor<ClickMode>? clickMode = null,
        Accessor<KeyGesture>? hotKey = null,
        Accessor<bool>? isDefault = null,
        Accessor<bool>? isCancel = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? onClick = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HLinkButton(out expose,
            _ => new(text, isVisited, bindIsVisited, navigateUri, clickMode, hotKey,
                isDefault, isCancel, strStyle, style, onClick, baseInteraction));

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