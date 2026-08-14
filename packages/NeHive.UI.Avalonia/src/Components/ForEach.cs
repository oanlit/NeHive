using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public struct ForEachProp<T>(Accessor<IReadOnlyList<T>> each)
{
    public readonly Accessor<IReadOnlyList<T>> Each = each;
    public IElement<Panel>? ItemsPanel { get; init; }
    public required Func<T, ISignal<int>, IElement> ItemTemplate { get; init; }
}

public static partial class ControlFlow
{
    public static IElement ForEach<T>(ForEachProp<T> prop) where T : notnull
    {
        return Element.WithScope(uiScope =>
        {
            var panel = new StackPanel();
            if (prop.ItemsPanel is not null)
            {
                panel.Children.Add(prop.ItemsPanel.Content);
            }

            var container = prop.ItemsPanel?.Expose ?? panel;

            // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
            Func<T, ISignal<int>, IElement> template = (data, i) =>
            {
                IElement result;
                result = prop.ItemTemplate(data, i);
                _ = result.Content;
                // using (new ScopeFrame(uiScope))
                // {
                //     result = prop.ItemTemplate(data, i);
                // }

                return result;
            };
            var memo = new ArrayMapMemo<T, IElement, T>(prop.Each, template);

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

            uiScope.OnCleanup += memo.Dispose;

            return panel;
        });
    }
}