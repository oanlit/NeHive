using NeHive.Core;
using NeHive.Sample.Avalonia.Render;
using static NeHive.Sample.Avalonia.Render.Components.Base;

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
            HTextBlock(new($"Id:{id}")),
            HTextBlock(new(countText)),
            HButton(new("Add",
                click: _ => count.Value++
            )), // HButton
            HButton(new("Sub",
                click: _ => count.Value--
            )) // HButton
        }); // HStackPanel
        uiScope.OnMount(() => Console.WriteLine(rootView.Content.Bounds.Size));
        // Console.WriteLine(rootView.Text.Bounds.Size);
        uiScope.OnDispose(() => Console.WriteLine($"Counter{id} 组件已移除"));

        return rootView;
    });

    private static readonly Component CompForEachDemo = new(() =>
    {
        var items = new Signal<IReadOnlyList<int>>([1, 2, 3]);

        var rootView = HStackPanel(new()
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
                Children = Counter
            }) // ForEach<int>
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

    public static IElement Counter(int prop)
        => CompCounter.Create(prop);

    private static readonly Component CompShowDemo = new(uiScope =>
    {
        var visible = new Signal<bool>(true);
        // var visibleText = () => visible.Value ? "Visible" : "Hidden";
        var visibleText = uiScope.AddComputed(() => visible.Value ? "Visible" : "Hidden");

        var rootView = HStackPanel(new()
        {
            HButton(new(visibleText,
                click: _ => visible.Value = !visible.Value
            )), // HButton
            Show(new(visible)
            {
                Fallback = () => Counter(0),
                Children = ForEachDemo
            }) // HButton
        }); // HStackPanel

        return rootView;
    });

    public static IElement ShowDemo()
        => CompShowDemo.Create();

    private static readonly Component CompLoadingDemo = new(uiScope =>
    {
        var userId = new Signal<int>(1);
        var userMemo = uiScope.AddAsyncMemo<User>(async epoch =>
        {
            var id = epoch.Track(userId);
            await Task.Delay(500);
            if (id <= 0) throw new Exception("Invalid user");
            return new User { Id = id, Name = $"User {id}" };
        });

        var rootView = HStackPanel(new()
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
                Error = ex =>
                    HButton(new($"Retry: {ex.Message}", click: _ => { userMemo.Refetch(); })) // Loading<User>.Error
            }) // Loading<User>
        }); // HStackPanel

        return rootView;
    });

    public static IElement LoadDemo()
        => CompLoadingDemo.Create();
}

public record User(int? Id = null, string? Name = null);