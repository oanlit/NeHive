using NeHive.Core;
using NeHive.Sample.Avalonia.Render;
using Avalonia;
using Avalonia.Media;
using NeHive.Sample.Avalonia.Render.Components;
using static NeHive.Sample.Avalonia.Render.Components.BaseComponent;
using static NeHive.Sample.Avalonia.Render.Components.ControlFlow;

namespace NeHive.Sample.Avalonia;

public static class DemoComponent
{
    private static IElement CounterComp(int id, UiScope uiScope)
    {
        Console.WriteLine($"Counter{id} 组件已创建");
        var count = new Signal<int>(0);
        var countText = () => $"Count: {count.Value}";

        var rootElement = uiScope.RootElement(new()
        {
            // HTextBlock(new($"Id:{id}",
            //     fontSize: 18,
            //     fontWeight: FontWeight.Bold,
            //     foreground: new(Brushes.DarkSlateBlue)
            // )), // HTextBlock
            // HTextBlock(new($"Id:{id}",
            //     style: HTextStyle.Parse(
            //         """
            //             text-18
            //             font-bold
            //             fg-darkslateblue
            //         """)
            // )), // HTextBlock
            HTextBlock(new($"Id:{id}",
                strStyle: """
                              text-lg
                              font-bold
                              fg-darkslateblue
                          """
            )), // HTextBlock
            HTextBlock(new(countText,
                strStyle: "text-2xl fg-darkgreen"
            )), // HTextBlock
            // HButton(new("Add",
            //     background: new(Brushes.ForestGreen),
            //     foreground: new(Brushes.White),
            //     cornerRadius: new CornerRadius(8),
            //     padding: new Thickness(16, 8),
            //     click: _ => count.Value++
            // )), // HButton
            HButton(new("Add",
                strStyle: "px-2 py-1 mt-1 ml-2 bg-forestgreen fg-white rounded-lg",
                click: _ => count.Value++
            )), // HButton
            HButton(new("Sub",
                strStyle: "px-2 py-1 mt-1 ml-2 bg-crimson fg-white rounded-lg",
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
        var visibleText = uiScope.CreateComputed(() => visible.Value ? "ForEachDemo" : "LoadDemo");

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
        var userMemo = uiScope.CreateAsyncMemo<User>(async epoch =>
        {
            var id = epoch.Track(userId);
            await Task.Delay(500);
            if (id <= 0) throw new Exception("Invalid user");
            return new User { Id = id, Name = $"User {id}" };
        });

        var rootElement = uiScope.RootElement(new()
        {
            HStackPanel(new(strStyle:"flex-row mb-2")
            {
                HButton(new("Add User Id",
                    strStyle: "ml-2 mt-2",
                    click: _ => userId.Value++
                )), // HButton
                HButton(new("Sub User Id",
                    strStyle: "ml-2 mt-2",
                    click: _ => userId.Value--
                )) // HButton
            }),

            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock(new($"Id: {user.Id}", strStyle: "mt-1.5")),
                    HTextBlock(new($"Hello, {user.Name}", strStyle: "mt-1.5"))
                ), // Loading<User>.Success
                Loading = () => HTextBlock(new("Fetching user...")),
                Error = ex =>
                    HButton(new($"Retry: {ex.Message}",
                        click: _ => userMemo.Refetch())
                    ) // HButton
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

        // var rootElement = uiScope.RootElement(new(
        //     orientation: Orientation.Vertical,
        //     spacing: 16,
        //     background: new(Brushes.LightGray),
        //     // cornerRadius: new CornerRadius(12),
        //     // padding: new Thickness(16),
        //     margin: new Thickness(20))
        var rootElement = uiScope.RootElement(new(
            strStyle: "flex-col gap-4 m-5 bg-lightgray")
        {
            HStackPanel(new(strStyle: "flex-row center gap-3")
            {
                // HButton(new("显示简单计数器",
                //     background: new(Brushes.SteelBlue),
                //     foreground: new(Brushes.White),
                //     cornerRadius: new CornerRadius(16),
                //     padding: new Thickness(12, 6),
                //     fontSize: 14,
                //     click: _ => currentView.Value = DemoView.SimpleCounter
                // )), // HButton
                HButton(new("显示简单计数器",
                    strStyle: "px-3 py-1.5 text-sm bg-steelblue fg-white rounded-2xl",
                    click: _ => currentView.Value = DemoView.SimpleCounter
                )), // HButton
                HButton(new("显示 ForEach 示例",
                    strStyle: "px-3 py-1.5 text-sm bg-steelblue fg-white rounded-2xl",
                    click: _ => currentView.Value = DemoView.ForEachDemo
                )), // HButton
                HButton(new("显示 Loading 示例",
                    strStyle: "px-3 py-1.5 text-sm bg-steelblue fg-white rounded-2xl",
                    click: _ => currentView.Value = DemoView.LoadingDemo
                )) // HButton
            }), // HStackPanel
            // 根据 currentView 切换内容
            HStackPanel(new(
                strStyle: "gap-3 bg-white")
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
                        strStyle: "text-base fg-gray"
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
        var gapX = new Signal<int>(3);
        var rootElement = uiScope.RootElement(new()
        {
            HGrid(new(
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]), // 第一行高度自适应，第二行占满剩余
                columnDefinitions: new([100, HgLen.Star()]), // 第一列固定100，第二列占满
                strStyle: new(() => $"m-2 gap-x-{gapX.Value} gap-y-3 bg-lightgray"))
            {
                [(row: 0, column: 0)] = HTextBlock(new("左上", style: new HTextStyle(fontSize: 16))),
                [(row: 0, column: 1)] = HTextBlock(new("右上", style: new HTextStyle(fontSize: 16))),
                [(row: 1, column: 0, rowSpan: 1, colSpan: 2)] =
                    HButton(new("点我增加间隔",
                        click: _ => gapX.Value++
                    )) // HButton
                // HGrid.[(row: 1, column: 0, rowSpan: 1, colSpan: 2)]
            }) // HGrid
        }); // rootElement

        return rootElement;
    }

    public static IElement GridDemo()
        => Element.WithScope(GridDemoComp);

    private static IElement AbsoluteDemoComp(UiScope uiScope)
    {
        var rootElement = uiScope.RootElement(new(
            strStyle: "flex-col gap-4 m-5 bg-lightgray")
        {
            HTextBlock(new("Canvas 绝对布局示例",
                strStyle: "text-lg font-bold"
            )), // HTextBlock

            // 创建一个带边框和海蓝色背景的 Canvas 容器
            HAbsolute(new(
                width: 600,
                height: 400,
                margin: new Thickness(0),
                background: new(Brushes.LightBlue))
            {
                // 左上角放置文本
                [new(left: 10, top: 10)] =
                    HTextBlock(new("左上角 (10,10)",
                        strStyle: "text-sm fg-darkslateblue"
                    )), // HTextBlock
                // HAbsolute.[new(450, 10)]

                // 右上角放置按钮
                [new(left: 450, top: 10)] =
                    HButton(new("右上角按钮",
                        strStyle: "px-2 py-1 bg-steelblue fg-white rounded",
                        click: _ => Console.WriteLine("右上角按钮被点击")
                    )), // HButton
                // HAbsolute.[new(450, 10)]

                // 中心偏左位置放置一个圆形文本（通过 Border + 背景模拟）
                [new(left: 150, top: 150)] =
                    HStackPanel(new(
                        strStyle: "flex-col gap-2 bg-white")
                    {
                        HTextBlock(new("中心区域", strStyle: "text-base font-bold")),
                        HTextBlock(new("坐标 (150,150)", strStyle: "text-xs fg-gray"))
                    }), // HStackPanel
                // HAbsolute.[new(left: 150, top: 150)]

                // 右下角放置一个按钮
                [new(left: 450, top: 340)] =
                    HButton(new("右下角按钮",
                        strStyle: "bg-crimson fg-white rounded-lg",
                        click: _ => Console.WriteLine("右下角按钮被点击")
                    )), // HButton
                // HAbsolute.[new(left: 450, top: 340)]

                // 添加一个带半透明背景的浮动面板
                [new(left: 20, top: 300)] =
                    HStackPanel(new(
                        strStyle: "flex-row gap-2 m-5 bg-black")
                    {
                        HTextBlock(new("悬浮信息", style: new HTextStyle(foreground: Brushes.White))),
                        HTextBlock(new("(20,300)", style: new HTextStyle(foreground: Brushes.LightYellow)))
                    }) // HStackPanel
                // HAbsolute.[new(left: 20, top: 300)]
            }), // HAbsolute

            // 说明文字
            HTextBlock(new("提示：Absolute 中元素通过 Left/Top 绝对定位，容器本身有固定宽高 600x400",
                strStyle: "text-xs fg-gray"
            )) // HTextBlock
        }); // rootElement

        return rootElement;
    }

    public static IElement AbsoluteDemo()
        => Element.WithScope(AbsoluteDemoComp);
}

public record User(int? Id = null, string? Name = null);

public enum DemoView
{
    SimpleCounter,
    ForEachDemo,
    LoadingDemo,
    Unknown
}