using Avalonia.Controls;
using Avalonia.Interactivity;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render;

public static class Components
{
    public static readonly Component Empty = new(() => new Panel());

    public class ShowProp(Accessor<bool> when)
    {
        public required Component Children;
        public readonly Accessor<bool> When = when;
    }

    public static readonly Component<ShowProp> Show = new((prop, uiScope) =>
    {
        var panel = new Panel();

        uiScope.AddEffect(epochScope =>
        {
            var when = epochScope.Track(prop.When);
            
            if (!when)
                return;

            var child = prop.Children.Create();
            panel.Children.Add(child.Content);
            epochScope.OnDispose(child.Dispose);
        });

        return panel;
    });

    public class HStackPanelProp(Element[]? children = null)
    {
        public Element[] Children = children ?? [];
    }

    public static readonly Component<HStackPanelProp> HStackPanel = new(prop =>
    {
        var stack = new StackPanel();
        foreach (var el in prop.Children)
        {
            stack.Children.Add(el.Content);
        }

        return stack;
    });

    public class HTextBlockProp(Accessor<string>? text = null)
    {
        public readonly Accessor<string> Text = text ?? "";
    }

    public static readonly Component<HTextBlockProp> HTextBlock = new((prop, uiScope) =>
    {
        var text = new TextBlock();
        uiScope.AddEffect(() => { text.Text = prop.Text.Value; });
        return text;
    });

    public struct HButtonProp(Accessor<string>? text = null,EventHandler<RoutedEventArgs>? click = null)
    {
        public readonly Accessor<string> Text = text ?? "";
        public readonly EventHandler<RoutedEventArgs>? Click = click;
    }

    public class HButtonExpose
    {
        public EventHandler<RoutedEventArgs> Click = (_, _) => { };
    }

    public static readonly Component<HButtonProp, HButtonExpose> HButton = new((prop, out expose, uiScope) =>
    {
        var button = new Button();
        uiScope.AddEffect(() => { button.Content = prop.Text.Value; });

        var expose1 = new HButtonExpose();
        uiScope.OnMount(() =>
        {
            button.Click += prop.Click;
            button.Click += expose1.Click;
        });
        expose = expose1;

        return new Element<HButtonExpose>(uiScope, button, expose);
    });
}

public static class Components<T> where T : notnull
{
    public class ForEachProp<T1>(Accessor<IReadOnlyList<T1>> each)
    {
        public required Component<T1> Children;
        public readonly Accessor<IReadOnlyList<T1>> Each = each;
    }

    public static readonly Component<ForEachProp<T>> ForEach = new((prop, uiScope) =>
    {
        var panel = new StackPanel();

        // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
        var memo = new ArrayMapMemo<T, Element, T>(prop.Each, prop.Children.Create);

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