using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HContext<T>(ContextKey<T> contextKey, T value, Func<IElement> child) where T : notnull
    {
        var uiScope = new UiScope();
        uiScope.SetContext(contextKey, value);
        var element = uiScope.RunInScope(child);
        return new Element(uiScope, element);
    }

    public static IElement HContext(Action<IContextSetter> buildContext, Func<IElement> child)
    {
        var uiScope = new UiScope();
        buildContext(uiScope);
        var element = uiScope.RunInScope(child);
        return new Element(uiScope, element);
    }
}