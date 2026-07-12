using NeHive.Model;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HContext<T>(ContextKey<T> contextKey, T value, IElement child) where T : notnull
    {
        return Element.WithScope(uiScope =>
        {
            uiScope.SetContext(contextKey, value);
            return child;
        });
    }

    public static IElement HContext(Action<IContextSetter> contextSetter, IElement child)
    {
        return Element.WithScope(uiScope =>
        {
            contextSetter(uiScope);
            return child;
        });
    }
}