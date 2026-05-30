using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct MatchProp<T>(Accessor<T> key) where T : notnull
{
    public readonly Accessor<T> Key = key;
    public required Dictionary<Func<T, bool>, Func<IElement>> Cases { get; init; }
    public Func<IElement>? Default { get; init; }
}

public static partial class ControlFlow
{
    private static Element MatchComp<T>(MatchProp<T> prop, UiScope uiScope) where T : notnull
    {
        var container = new Panel();
        var cases = new KeyValuePair<Func<T,bool>,Func<IElement>>[prop.Cases.Count];

        var i = 0;
        foreach (var kvp in prop.Cases)
        {
            cases[i] = kvp;
            i++;
        }

        var rxMatchIndex = uiScope.CreateComputed(() =>
        {
            var key = prop.Key.RxValue;
            for (var j = 0; j < cases.Length; j++)
            {
                if (!cases[j].Key(key)) continue;
                return j;
            }
            return -1; // 默认值
        });

        uiScope.CreateEffect(epochScope =>
        {
            var matchIndex = epochScope.Pull(rxMatchIndex);
            Func<IElement> childFactory;
            if (matchIndex == -1)
            {
                if (prop.Default is null) return;
                childFactory = prop.Default;
            }
            else
            {
                childFactory = cases[matchIndex].Value;
            }
            var child = childFactory();
            container.Children.Add(child.Content);
            epochScope.OnDispose += child.Dispose;
        });

        return new Element(uiScope, container);
    }

    public static IElement Match<T>(MatchProp<T> prop) where T : notnull
        => Element.WithScope(MatchComp, prop);
}