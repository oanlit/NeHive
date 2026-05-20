using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct ForEachProp<T>(Accessor<IReadOnlyList<T>> each)
{
    public readonly Accessor<IReadOnlyList<T>> Each = each;
    public IElement<Panel>? Container;
    public required Func<T, ISignal<int>, IElement> ItemTemplate;
}

public static partial class ControlFlow
{
    private static Element ForEachComp<T>(ForEachProp<T> prop, UiScope uiScope) where T : notnull
    {
        var panel = prop.Container?.Expose ?? new StackPanel();

        // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
        var memo = new ArrayMapMemo<T, IElement, T>(prop.Each, prop.ItemTemplate);

        uiScope.CreateEffect(epochScope =>
        {
            var list = epochScope.Pull(memo);

            // —— UI 最小更新（核心）——
            for (var i = 0; i < list.Count; i++)
            {
                var childrenContent = list[i].Content;

                if (i >= panel.Children.Count)
                {
                    // 追加
                    panel.Children.Add(childrenContent);
                }
                else if (!ReferenceEquals(panel.Children[i], childrenContent))
                {
                    // 位置不一致 → 移动（或替换）
                    panel.Children.RemoveAt(i);
                    panel.Children.Insert(i, childrenContent);
                }
            }
        });

        uiScope.OnDispose += () => memo.Dispose();

        return new Element(uiScope, panel);
    }

    public static IElement ForEach<T>(ForEachProp<T> prop) where T : notnull
        => Element.WithScope(ForEachComp, prop);
}