using Avalonia.Controls;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public static class CounterComponent
{
    public static Component Counter(int id = 0)
    {
        return Component.Create(uiScope =>
        {
            Console.WriteLine($"Counter{id} 组件已创建");
            var count = new Signal<int>(0);

            var stack = new StackPanel();

            var text = new TextBlock();
            var button = new Button { Content = "Add" };

            stack.Children.Add(new TextBlock()
            {
                Text = $"Id:{id}"
            });
            stack.Children.Add(text);
            stack.Children.Add(button);

            uiScope.AddEffect(() => { text.Text = $"Count: {count.Value}"; });

            button.Click += (_, _) => count.Value++;

            uiScope.OnMount(() => { Console.WriteLine(stack.Bounds.Size); });
            uiScope.OnDispose(() => { Console.WriteLine($"Counter{id} 组件已移除"); });

            return stack;
        });
    }

    public static Component Demo()
    {
        return Component.Create(demoScope =>
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

            var show = Component.Show(visible, () => Counter());

            stack.Children.Add(show.Content);

            toggleBtn.Click += (_, _) => { visible.Value = !visible.Value; };

            return stack;
        });
    }

    public static Component ForEachDemo()
    {
        return Component.Create(_ =>
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
            var list = Component.ForEach(
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