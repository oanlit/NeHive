using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct SwitchProp<T>(Accessor<T> key) where T : notnull
{
    public readonly Accessor<T> Key = key;
    internal readonly Dictionary<T, Func<IElement>> Cases = new();

    public Func<IElement> this[params T[] cases]
    {
        set
        {
            foreach (var item in cases)
            {
                Cases[item] = value;
            }
        }
    }

    public Func<IElement>? Default { get; init; }
}

public static partial class ControlFlow
{
    public static IElement Switch<T>(SwitchProp<T> prop) where T : notnull
    {
        return Element.WithScope(uiScope =>
        {
            var container = new Panel();

            uiScope.CreateEffect(epochScope =>
            {
                var currentKey = epochScope.Track(prop.Key);

                // 根据当前 key 获取对应的子元素工厂，否则使用 Default
                if (!prop.Cases.TryGetValue(currentKey, out var childFactory))
                {
                    if (prop.Default is null) return;
                    childFactory = prop.Default;
                }

                IElement child;
                using (new ScopeFrame(uiScope))
                {
                    child = childFactory();
                    _ = child.Content;
                }

                container.Children.Add(child.Content);
                epochScope.OnCleanup += child.Dispose;
            });

            return container;
        });
    }
}