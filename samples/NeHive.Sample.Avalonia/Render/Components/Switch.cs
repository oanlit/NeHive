using Avalonia.Controls;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

// 泛型参数 T 表示 key 的类型，需要支持作为 Dictionary 的键
public struct SwitchProp<T>(Accessor<T> key) where T : notnull
{
    public readonly Accessor<T> Key = key;
    public required Dictionary<T, Func<IElement>> Cases { get; init; }
    public Func<IElement>? Default { get; init; }
}

public static partial class ControlFlow
{
    private static Element SwitchComp<T>(SwitchProp<T> prop, UiScope uiScope) where T : notnull
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

            var child = childFactory();
            container.Children.Add(child.Content);
            epochScope.OnDispose(child.Dispose);
        });

        return new Element(uiScope, container);
    }

    public static IElement Switch<T>(SwitchProp<T> prop) where T : notnull
        => Element.WithScope(SwitchComp, prop);
}