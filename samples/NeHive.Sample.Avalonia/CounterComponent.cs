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
        var hStackPanel = Components.HStackPanel.Create;
        var hTextBlock = Components.HTextBlock.Create;
        var hButton = Components.HButton.Create;

        Console.WriteLine($"Counter{id} 组件已创建");
        var count = new Signal<int>(0);
        var countText = () => $"Count: {count.Value}";

        var rootView = hStackPanel(new()
        {
            Children =
            [
                hTextBlock(new($"Id:{id}")),
                hTextBlock(new(countText)),
                hButton(new("Add",
                    click: (_, _) => count.Value++
                )), // hButton.Create
                hButton(new("Sub",
                    click: (_, _) => count.Value--
                )) // hButton.Create
            ] // hStackPanel.Create.Children
        });
        uiScope.OnMount(() => { Console.WriteLine(rootView.Content.Bounds.Size); });
        // Console.WriteLine(rootView.Text.Bounds.Size);
        uiScope.OnDispose(() => { Console.WriteLine($"Counter{id} 组件已移除"); });

        return rootView;
    });

    public static Component ShowDemo = new(uiScope =>
    {
        var hStackPanel = Components.HStackPanel.Create;
        var hButton = Components.HButton.Create;
        var show = Components.Show.Create;

        var visible = new Signal<bool>(true);
        // var visibleText = () => visible.Value ? "Visible" : "Hidden";
        var visibleText = uiScope.AddComputed(() => visible.Value ? "Visible" : "Hidden");

        var rootView = hStackPanel(new()
        {
            Children =
            [
                hButton(new(visibleText,
                    click: (_, _) => visible.Value = !visible.Value
                )), // hButton.Create
                show(new(visible)
                {
                    Children = new Component(() => Counter.Create(0))
                }) // hButton.Create
            ] // hStackPanel.Create.Children
        });

        return rootView;
    });

    public static readonly Component ForEachDemo = new(() =>
    {
        var hStackPanel = Components.HStackPanel.Create;
        var hButton = Components.HButton;

        var items = new Signal<IReadOnlyList<int>>([1, 2, 3]);
        
        var rootView = hStackPanel(new()
        {
            Children = [
                hButton.CreateRef(new("Add Item"),
                    out var addBtn
                ), // hButton.Create
                hButton.CreateRef(new("Remove Last"),
                    out var removeBtn
                ), // hButton.Create
                hButton.CreateRef(new("Remove Second Last"),
                    out var removeSecBtn
                ), // hButton.Create
                Components<int>.ForEach.Create(
                    new(each: items)
                    {
                        Children = Counter
                    }
                ) // Components<int>.ForEach.Create
            ] // hStackPanel.Create.Children
        });

        addBtn.Expose.Click += (_, _) =>
        {
            var arr = items.Value.ToList();
            arr.Add(arr.Count + 1);
            items.Value = arr;
        };

        removeBtn.Expose.Click += (_, _) =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 0)
                arr.RemoveAt(arr.Count - 1);
            items.Value = arr;
        };

        removeSecBtn.Expose.Click += (_, _) =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 1)
                arr.RemoveAt(1);
            items.Value = arr;
        };

        return rootView;
    });
}