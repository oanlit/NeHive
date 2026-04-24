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
    
    private static void DisposeControl(UiScope scope, Control control)
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

                epochScope.OnDispose(child.Dispose);
            });

            return panel;
        });
    }
    
    public static Component ForEach<T>(
        IReadOnlySignal<IReadOnlyList<T>> source,
        Func<T, Component> children)
        where T : notnull
    {
        return Create(uiScope =>
        {
            var panel = new StackPanel();

            // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
            var memo = new ArrayMapMemo<T, Component, T>(source, children);

            uiScope.AddEffect(() =>
            {
                var list = memo.Value;

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
            
            uiScope.OnDispose(memo.Dispose);

            return panel;
        });
    }
}