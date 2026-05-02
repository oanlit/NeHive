using Avalonia.Controls;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

// using HStackPanelProp = Components.HStackPanelProp;
// using HTextBlockProp = Components.HTextBlockProp;
// using HTextButtonProp = Components.HButtonProp;
// using HTextButtonExpose = Components.HButtonExpose;

public static class CounterComponent
{
    // static readonly Component<HStackPanelProp> HStackPanel = Components.HStackPanel;
    // static readonly Component<HTextBlockProp> HTextBlock = Components.HTextBlock;
    // static readonly Component<HTextButtonProp, HTextButtonExpose> HButton = Components.HButton;

    public static readonly Component<int> Counter = new((id, uiScope) =>
    {
        // def hStackPanel = Components.HStackPanel.Create;
        // def hTextBlock = Components.HTextBlock.Create;
		// def hButton = Components.HButton.Create;
        var hStackPanel = Components.HStackPanel;
        var hTextBlock = Components.HTextBlock;
        var hButton = Components.HButton;

        Console.WriteLine($"Counter{id} 组件已创建");
        var count = new Signal<int>(0);
        var countText = () => $"Count: {count.Value}";

        var rootView = hStackPanel.Create(new()
        {
            Children =
            [
                hTextBlock.Create(new($"Id:{id}")),
                hTextBlock.Create(new(countText)),
                hButton.Create(new("Add"), out var button)
            ]
        });

        button.Click += (_, _) => { count.Value++; };

        uiScope.OnMount(() => { Console.WriteLine(rootView.Content.Bounds.Size); });
        // Console.WriteLine(rootView.Content.Bounds.Size);
        uiScope.OnDispose(() => { Console.WriteLine($"Counter{id} 组件已移除"); });

        return rootView;
    });

    public static Element Demo()
    {
        return Element.Create(demoScope =>
        {
            var visible = new Signal<bool>(true);

            var stack = new StackPanel();
            var toggleBtn = new Button { Content = "Toggle" };

            demoScope.AddEffect(() =>
            {
                var str = visible.Value ? "Visible" : "Hidden";
                toggleBtn.Content = str;
            });

            stack.Children.Add(toggleBtn);

            var show = Element.Show(visible, new Component(() => Counter.Create(0)));

            stack.Children.Add(show.Content);

            toggleBtn.Click += (_, _) => { visible.Value = !visible.Value; };

            return stack;
        });
    }

    public static Element ForEachDemo()
    {
        return Element.Create(_ =>
        {
            var items = new Signal<IReadOnlyList<int>>([1, 2, 3]);

            var stack = new StackPanel();

            var addBtn = new Button { Content = "Add Item" };
            var removeBtn = new Button { Content = "Remove Last" };
            var removeSecBtn = new Button { Content = "Remove Second Last" };

            stack.Children.Add(addBtn);
            stack.Children.Add(removeBtn);
            stack.Children.Add(removeSecBtn);

            // 👇 关键：For
            var list = Element.ForEach(
                items,
                Counter
            );

            stack.Children.Add(list.Content);

            addBtn.Click += (_, _) =>
            {
                var arr = items.Value.ToList();
                arr.Add(arr.Count + 1);
                items.Value = arr;
            };

            removeBtn.Click += (_, _) =>
            {
                var arr = items.Value.ToList();
                if (arr.Count > 0)
                    arr.RemoveAt(arr.Count - 1);
                items.Value = arr;
            };

            removeSecBtn.Click += (_, _) =>
            {
                var arr = items.Value.ToList();
                if (arr.Count > 1)
                    arr.RemoveAt(1);
                items.Value = arr;
            };

            return stack;
        });
    }
}