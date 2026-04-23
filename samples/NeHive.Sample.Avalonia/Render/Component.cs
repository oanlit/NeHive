using Avalonia.Controls;
using Avalonia.Threading;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render;

public class Component
{
    private readonly UiScope _scope;

    public readonly Control Content;

    private Component(UiScope scope, Control content)
    {
        _scope = scope;
        Content = content;
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public static Component Create(Func<UiScope, Control> builder)
    {
        var uiScope = new UiScope();
        var control = uiScope.RunInScope(() => builder(uiScope));
        control.AttachedToVisualTree += (_, _) => { Dispatcher.UIThread.Post(uiScope.RunMount); };
        DisposeControl(uiScope, control);
        return new Component(uiScope, control);
    }

    public static Component Create(Func<Control> builder)
    {
        var uiOwner = new UiScope();
        var control = uiOwner.RunInScope(builder);
        control.AttachedToVisualTree += (_, _) => { Dispatcher.UIThread.Post(uiOwner.RunMount); };
        DisposeControl(uiOwner, control);
        return new Component(uiOwner, control);
    }

    public static Component Show(
        IReadOnlySignal<bool> when,
        Func<Component> children)
    {
        return Create(uiScope =>
        {
            var panel = new Panel();

            uiScope.AddEffect(epochScope =>
            {
                if (!when.Value)
                    return;

                var child = children();

                panel.Children.Add(child.Content);

                epochScope.OnDispose(() =>
                {
                    panel.Children.Remove(child.Content);
                    child.Dispose();
                });
            });

            return panel;
        });
    }

    private static void DisposeControl(Scope scope, Control control)
    {
        scope.OnDispose(() =>
        {
            var parent = control.Parent;
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(control);
                    break;

                case ContentControl contentControl:
                    if (contentControl.Content == control)
                        contentControl.Content = null;
                    break;

                case Decorator decorator: // 比如 Border
                    if (decorator.Child == control)
                        decorator.Child = null;
                    break;
            }
        });
    }
}