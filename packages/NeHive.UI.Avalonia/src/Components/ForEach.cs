using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct ForEachProp<T>(Accessor<IReadOnlyList<T>> each)
{
    public readonly Accessor<IReadOnlyList<T>> Each = each;
    public IElement<Panel>? ItemsPanel;
    public required Func<T, ISignal<int>, IElement> ItemTemplate;
}

public static partial class ControlFlow
{
    private static Element ForEachComp<T>(ForEachProp<T> prop, UiScope uiScope) where T : notnull
    {
        var panel = new StackPanel();
        var container = prop.ItemsPanel?.Expose ?? panel;

        // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
        var memo = new ArrayMapMemo<T, IElement, T>(prop.Each, prop.ItemTemplate);

        uiScope.CreateEffect(epochScope =>
        {
            var list = epochScope.Pull(memo);

            // —— UI 最小更新（核心）——
            for (var i = 0; i < list.Count; i++)
            {
                var childrenContent = list[i].Content;

                if (i >= container.Children.Count)
                {
                    // 追加
                    container.Children.Add(childrenContent);
                }
                else if (!ReferenceEquals(container.Children[i], childrenContent))
                {
                    // 位置不一致 → 移动（或替换）
                    container.Children.RemoveAt(i);
                    container.Children.Insert(i, childrenContent);
                }
            }
        });

        if (prop.ItemsPanel is not null)
        {
            panel.Children.Add(prop.ItemsPanel.Content);
        }

        uiScope.OnCleanup += () => memo.Dispose();

        return new Element(uiScope, panel);
    }

    public static IElement ForEach<T>(ForEachProp<T> prop) where T : notnull
        => Element.WithScope(ForEachComp, prop);
}