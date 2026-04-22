using System;
using Avalonia.Controls;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public static class CounterComponent
{
    public static Component Counter()
    {
        return Component.Create(uiScope =>
        {
            Console.WriteLine("Counter 组件已创建");
            var count = new Signal<int>(0);

            var stack = new StackPanel();

            var text = new TextBlock();
            var button = new Button { Content = "Add" };

            stack.Children.Add(text);
            stack.Children.Add(button);

            uiScope.AddEffect(() => { text.Text = $"Count: {count.Value}"; });

            button.Click += (_, _) => count.Value++;

            uiScope.OnMount(() => { Console.WriteLine(stack.Bounds.Size); });
            uiScope.OnDispose(() => { Console.WriteLine("Counter 组件已移除"); });

            return stack;
        });
    }

    public static Component Demo()
    {
        var counter = CounterComponent.Counter;

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

            var show = Component.Show(visible, counter);

            stack.Children.Add(show.Content);

            toggleBtn.Click += (_, _) => { visible.Value = !visible.Value; };

            return stack;
        });
    }
}