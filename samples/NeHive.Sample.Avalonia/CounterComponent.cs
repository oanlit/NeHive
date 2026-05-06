using NeHive.Core;
using NeHive.Sample.Avalonia.Render;
using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using NeHive.Sample.Avalonia.Render.Components;
using static NeHive.Sample.Avalonia.Render.Components.BaseComponent;
using static NeHive.Sample.Avalonia.Render.Components.ControlFlow;

namespace NeHive.Sample.Avalonia;

public static class CounterComponent
{
    private static IElement CounterComp(int id, UiScope uiScope)
    {
        Console.WriteLine($"Counter{id} 组件已创建");
        var count = new Signal<int>(0);
        var countText = () => $"Count: {count.Value}";

        var rootElement = uiScope.RootElement(new()
        {
            HTextBlock(new($"Id:{id}",
                fontSize: 18,
                fontWeight: FontWeight.Bold,
                foreground: new(Brushes.DarkSlateBlue)
            )), // HTextBlock
            HTextBlock(new(countText,
                fontSize: 24,
                foreground: new(Brushes.DarkGreen)
            )), // HTextBlock
            HButton(new("Add",
                background: new(Brushes.ForestGreen),
                foreground: new(Brushes.White),
                cornerRadius: new CornerRadius(6),
                padding: new Thickness(16, 8),
                click: _ => count.Value++
            )), // HButton
            HButton(new("Sub",
                background: new(Brushes.Crimson),
                foreground: new(Brushes.White),
                cornerRadius: new CornerRadius(6),
                padding: new Thickness(16, 8),
                click: _ => count.Value--
            )) // HButton
        }); // rootElement

        uiScope.OnMount(() => Console.WriteLine(rootElement.Content.Bounds.Size));
        // Console.WriteLine(rootElement.Text.Bounds.Size);
        uiScope.OnDispose(() => Console.WriteLine($"Counter{id} 组件已移除"));

        return rootElement;
    }

    public static IElement Counter(int prop)
        => Element.WithScope(CounterComp, prop);

    private static IElement ShowDemoComp(UiScope uiScope)
    {
        var visible = new Signal<bool>(true);
        // var visibleText = () => visible.Value ? "ForEachDemo" : "LoadDemo";
        var visibleText = uiScope.AddComputed(() => visible.Value ? "ForEachDemo" : "LoadDemo");

        var rootElement = uiScope.RootElement(new()
        {
            HButton(new(visibleText,
                click: _ => visible.Value = !visible.Value
            )), // HButton
            Show(new(visible)
            {
                IfFalse = LoadingDemo,
                IfTrue = ForEachDemo
            }) // HButton
        }); // rootElement

        return rootElement;
    }

    public static IElement ShowDemo()
        => Element.WithScope(ShowDemoComp);

    private static IElement ForEachDemoComp()
    {
        var items = new Signal<IReadOnlyList<int>>([1, 2, 3]);

        var rootElement = RootElement(new()
        {
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
                ComponentItem = Counter
            }) // ForEach<int>
        }); // rootElement

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

        return rootElement;
    }

    public static IElement ForEachDemo()
        => Element.WithScope(ForEachDemoComp);

    private static IElement LoadingDemoComp(UiScope uiScope)
    {
        var userId = new Signal<int>(1);
        var userMemo = uiScope.AddAsyncMemo<User>(async epoch =>
        {
            var id = epoch.Track(userId);
            await Task.Delay(500);
            if (id <= 0) throw new Exception("Invalid user");
            return new User { Id = id, Name = $"User {id}" };
        });

        var rootElement = uiScope.RootElement(new()
        {
            HButton(new("Add User Id",
                click: _ => userId.Value++
            )), // HButton
            HButton(new("Sub User Id",
                click: _ => userId.Value--
            )), // HButton
            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock(new($"Id: {user.Id}")),
                    HTextBlock(new($"Hello, {user.Name}"))
                ), // Loading<User>.Success
                Loading = () => HTextBlock(new("Fetching user...")),
                Error = ex => HButton(
                    new($"Retry: {ex.Message}",
                        click: _ => userMemo.Refetch()
                    )) // HButton
                // Loading<User>.Error
            }) // Loading<User>
        }); // rootElement

        return rootElement;
    }

    public static IElement LoadingDemo()
        => Element.WithScope(LoadingDemoComp);

    private static IElement SwitchDemoComp(UiScope uiScope)
    {
        var currentView = new Signal<DemoView>(DemoView.Unknown);

        var rootElement = uiScope.RootElement(new(
            orientation: Orientation.Vertical,
            spacing: 16,
            background: new(Brushes.LightGray),
            // cornerRadius: new CornerRadius(12),
            // padding: new Thickness(16),
            margin: new Thickness(20))
        {
            HStackPanel(new(
                orientation: Orientation.Horizontal,
                spacing: 12,
                horizontalAlignment: HorizontalAlignment.Center)
            {
                HButton(new("显示简单计数器",
                    background: new(Brushes.SteelBlue),
                    foreground: new(Brushes.White),
                    cornerRadius: new CornerRadius(20),
                    padding: new Thickness(12, 6),
                    fontSize: 14,
                    click: _ => currentView.Value = DemoView.SimpleCounter
                )), // HButton
                HButton(new("显示 ForEach 示例",
                    background: new(Brushes.SteelBlue),
                    foreground: new(Brushes.White),
                    cornerRadius: new CornerRadius(20),
                    padding: new Thickness(12, 6),
                    fontSize: 14,
                    click: _ => currentView.Value = DemoView.ForEachDemo
                )), // HButton
                HButton(new("显示 Loading 示例",
                    background: new(Brushes.SteelBlue),
                    foreground: new(Brushes.White),
                    cornerRadius: new CornerRadius(20),
                    padding: new Thickness(12, 6),
                    fontSize: 14,
                    click: _ => currentView.Value = DemoView.LoadingDemo
                )) // HButton
            }), // HStackPanel
            // 根据 currentView 切换内容
            HStackPanel(new(
                background: new(Brushes.White),
                // cornerRadius: new CornerRadius(8),
                // padding: new Thickness(16),
                spacing: 12)
            {
                Switch<DemoView>(new(currentView)
                {
                    Cases = new()
                    {
                        [DemoView.SimpleCounter] = () => Counter(999), // 独立计数器，id=999
                        [DemoView.ForEachDemo] = ForEachDemo,
                        [DemoView.LoadingDemo] = LoadingDemo
                    }, // Switch<DemoView>.Cases
                    Default = () => HTextBlock(new("未知视图",
                        foreground: new(Brushes.Gray),
                        fontSize: 16
                    )) // Switch<DemoView>.Default
                }) // Switch<DemoView>
            }) // HStackPanel
        }); // rootElement

        return rootElement;
    }

    public static IElement SwitchDemo()
        => Element.WithScope(SwitchDemoComp);

    private static IElement GridDemoComp(UiScope uiScope)
    {
        var rootElement = uiScope.RootElement(new()
        {
            HGrid(new(
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]), // 第一行高度自适应，第二行占满剩余
                columnDefinitions: new([100, HgLen.Star()]), // 第一列固定100，第二列占满
                rowSpacing: 12,
                columnSpacing: 8,
                margin: new Thickness(10),
                background: new(Brushes.LightGray)
            )
            {
                [(Row: 0, Column: 0)] = HTextBlock(new("左上", fontSize: 16)),
                [(Row: 0, Column: 1)] = HTextBlock(new("右上", fontSize: 16)),
                [(Row: 1, Column: 0, RowSpan: 1, ColSpan: 2)] =
                    HButton(new("底部按钮横跨两列",
                        click: _ => Console.WriteLine("Clicked")
                    )) // HButton
            }) // HGrid
        }); // rootElement

        return rootElement;
    }

    public static IElement GridDemo()
        => Element.WithScope(GridDemoComp);
}

public record User(int? Id = null, string? Name = null);

public enum DemoView
{
    SimpleCounter,
    ForEachDemo,
    LoadingDemo,
    Unknown
}