using NeHive.Core;
using NeHive.Sample.Avalonia.Render;
using static NeHive.Sample.Avalonia.Render.Components;

namespace NeHive.Sample.Avalonia;

public static class CounterComponent
{
    private static readonly Component<int> CompCounter = new((id, uiScope) =>
    {
        Console.WriteLine($"Counter{id} 组件已创建");
        var count = new Signal<int>(0);
        var countText = () => $"Count: {count.Value}";

        var rootView = HStackPanel(new()
        {
            Children =
            [
                HTextBlock(new($"Id:{id}")),
                HTextBlock(new(countText)),
                HButton(new("Add",
                    click: _ => count.Value++
                )), // HButton
                HButton(new("Sub",
                    click: _ => count.Value--
                )) // HButton
            ] // HStackPanel.Children
        }); // HStackPanel
        uiScope.OnMount(() => Console.WriteLine(rootView.Content.Bounds.Size));
        // Console.WriteLine(rootView.Text.Bounds.Size);
        uiScope.OnDispose(() => Console.WriteLine($"Counter{id} 组件已移除"));

        return rootView;
    });

    public static IElement Counter(int prop)
        => CompCounter.Create(prop);

    private static readonly Component CompShowDemo = new(uiScope =>
    {
        var visible = new Signal<bool>(true);
        // var visibleText = () => visible.Value ? "Visible" : "Hidden";
        var visibleText = uiScope.AddComputed(() => visible.Value ? "Visible" : "Hidden");

        var rootView = HStackPanel(new()
        {
            Children =
            [
                HButton(new(visibleText,
                    click: _ => visible.Value = !visible.Value
                )), // HButton
                Show(new(visible)
                {
                    Children = new Component(() => Counter(0))
                }) // HButton
            ] // HStackPanel.Children
        }); // HStackPanel

        return rootView;
    });

    public static IElement ShowDemo()
        => CompShowDemo.Create();

    private static readonly Component CompForEachDemo = new(() =>
    {
        var items = new Signal<IReadOnlyList<int>>([1, 2, 3]);

        var rootView = HStackPanel(new()
        {
            Children =
            [
                HButton(new("Add Item"),
                    out var addBtn
                ), // HButton
                HButton(new("Remove Last"),
                    out var removeBtn
                ), // HButton
                HButton(new("Remove Second Last"),
                    out var removeSecBtn
                ), // HButton
                ForEach<int>(new(items)
                {
                    Children = Counter
                }) // ForEach<int>
            ] // HStackPanel.Children
        }); // HStackPanel

        addBtn.Expose.Click += _ =>
        {
            var arr = items.Value.ToList();
            arr.Add(arr.Count + 1);
            items.Value = arr;
        };

        removeBtn.Expose.Click += _ =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 0)
                arr.RemoveAt(arr.Count - 1);
            items.Value = arr;
        };

        removeSecBtn.Expose.Click += _ =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 1)
                arr.RemoveAt(1);
            items.Value = arr;
        };

        return rootView;
    });

    public static IElement ForEachDemo()
        => CompForEachDemo.Create();
}