using System.Text;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
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
            // HTextBlock($"Id:{id}",
            //     style: new HTextStyle(fontSize: 18,
            //         fontWeight: FontWeight.Bold,
            //         foreground: Brushes.DarkSlateBlue
            //     )
            // ), // HTextBlock
            // HTextBlock($"Id:{id}",
            //     style: HTextStyle.Parse(
            //         """
            //             text-lg
            //             font-bold
            //             fg-blue-700
            //         """)
            // ), // HTextBlock
            HTextBlock($"Id:{id}",
                strStyle: """
                              text-lg
                              font-bold
                              fg-sky-200
                          """), // HTextBlock
            HTextBlock(countText,
                strStyle: "text-2xl fg-lime-200"
            ), // HTextBlock
            HButton("Add",
                strStyle: "mt-1 ml-2 px-2 py-1 fg-white bg-green-300 border-green-400 rounded-lg",
                click: _ => count.Value++
            ), // HButton
            HButton("Sub",
                strStyle: "mt-1 ml-2 px-2 py-1 fg-white bg-pink-300 border-pink-400 rounded-lg",
                click: _ => count.Value--) // HButton
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
            HButton(visibleText,
                click: _ => visible.Value = !visible.Value
            ), // HButton
            Show(new(visible)
            {
                IfFalse = LoadingDemo,
                IfTrue = ForEachDemo
            }) // Show
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
            HButton(out var addBtn, text: "Add Item"),
            HButton(out var removeBtn, text: "Remove Last"),
            HButton(out var removeSecBtn, text: "Remove Second Last"),
            ForEach<int>(new(items)
            {
                ComponentItem = Counter
            }) // ForEach<int>
        }); // rootElement

        addBtn.Click += _ =>
        {
            var arr = items.Value.ToList();
            arr.Add(arr.Count + 1);
            items.Value = arr;
        };

        removeBtn.Click += _ =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 0)
                arr.RemoveAt(arr.Count - 1);
            items.Value = arr;
        };

        removeSecBtn.Click += _ =>
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
        // var userMemo = uiScope.CreateAsyncMemo<User>(async epoch =>
        // {
        //     var id = epoch.Track(userId);
        //     await Task.Delay(500);
        //     if (id <= 0) throw new Exception("Invalid user");
        //     return new User { Id = id, Name = $"User {id}" };
        // });
        var userMemo = uiScope.CreateReactiveFlow(userId)
            .Debounce(500)
            // .ThrottleLatest(500)
            .Filter(id => id > 0)
            .Map(id => new User { Id = id, Name = $"User {id}" })
            .PushAsyncMemo(async user =>
            {
                await Task.Delay(500);
                return user;
            }, initValue: new User(0, "Unknown"));

        var rootElement = uiScope.RootElement(new()
        {
            HStackPanel(new(strStyle: "mb-2 flex-row")
            {
                HButton("Add User Id",
                    strStyle: "ml-2 mt-2",
                    click: _ => userId.Value++
                ), // HButton
                HButton("Sub User Id",
                    strStyle: "ml-2 mt-2",
                    click: _ => userId.Value--
                ) // HButton
            }), // HStackPanel

            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock($"Id: {user.Id}", strStyle: "mt-1.5"),
                    HTextBlock($"Hello, {user.Name}", strStyle: "mt-1.5")
                ), // Loading<User>.Success
                Loading = () => HTextBlock(new("Fetching user...")),
                Error = ex =>
                    HButton($"Retry: {ex.Message}",
                        click: _ => userMemo.Refetch()
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
        //         style: new HPanelStyle(
        //             orientation: Orientation.Vertical,
        //             spacing: 16,
        //             background: Brushes.LightGray,
        //             // cornerRadius: new CornerRadius(12),
        //             // padding: new Thickness(16),
        //             margin: new Thickness(20))
        //     )
        var rootElement = uiScope.RootElement(new(
            strStyle: "m-5 gap-4 flex-col bg-gray-100")
        {
            HStackPanel(new(strStyle: "gap-3 flex-row justify-center")
            {
                // HButton("显示简单计数器",
                //     style: new HButtonStyle(
                //         background: Brushes.SteelBlue,
                //         foreground: Brushes.White,
                //         cornerRadius: new CornerRadius(16),
                //         padding: new Thickness(12, 6),
                //         fontSize: 14),
                //     click: _ => currentView.Value = DemoView.SimpleCounter
                // ), // HButton
                HButton("显示简单计数器",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-blue-300 border-blue-400 rounded-2xl",
                    click: _ => currentView.Value = DemoView.SimpleCounter
                ), // HButton
                HButton("显示 ForEach 示例",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-blue-300 border-blue-400 rounded-2xl",
                    click: _ => currentView.Value = DemoView.ForEachDemo
                ), // HButton
                HButton("显示 Loading 示例",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-blue-300 border-blue-400 rounded-2xl",
                    click: _ => currentView.Value = DemoView.LoadingDemo
                ) // HButton
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
                    Default = () => HTextBlock("未知视图",
                        strStyle: "text-base fg-gray-500") // Switch<DemoView>.Default
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
                strStyle: new(() => $"m-2 gap-x-{gapX.Value} gap-y-3 bg-gray-100"))
            {
                [(row: 0, column: 0)] = HTextBlock("左上", style: new HTextStyle(fontSize: 16)),
                [(row: 0, column: 1)] = HTextBlock("右上", style: new HTextStyle(fontSize: 16)),
                [(row: 1, column: 0, rowSpan: 1, colSpan: 2)] =
                    HButton("点我增加间隔",
                        click: _ => gapX.Value++
                    ) // HButton
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
            strStyle: "m-5 gap-4 flex-col bg-gray-100")
        {
            HTextBlock("Canvas 绝对布局示例",
                strStyle: "text-lg font-bold"
            ), // HTextBlock

            // 创建一个带边框和海蓝色背景的 Canvas 容器
            HAbsolute(new(
                width: 600,
                height: 400,
                margin: new Thickness(0),
                background: new(Brushes.LightBlue))
            {
                // 左上角放置文本
                [new(left: 10, top: 10)] =
                    HTextBlock("左上角 (10,10)",
                        strStyle: "text-sm fg-blue-700"
                    ), // HTextBlock
                // HAbsolute.[new(450, 10)]

                // 右上角放置按钮
                [new(left: 450, top: 10)] =
                    HButton("右上角按钮",
                        strStyle: "px-2 py-1 fg-white bg-blue-300 rounded",
                        click: _ => Console.WriteLine("右上角按钮被点击")
                    ), // HButton
                // HAbsolute.[new(450, 10)]

                // 中心偏左位置放置一个圆形文本（通过 Border + 背景模拟）
                [new(left: 150, top: 150)] =
                    HStackPanel(new(
                        strStyle: "gap-2 flex-col bg-white")
                    {
                        HTextBlock("中心区域", strStyle: "text-base font-bold"),
                        HTextBlock("坐标 (150,150)", strStyle: "text-xs fg-gray-500")
                    }), // HStackPanel
                // HAbsolute.[new(left: 150, top: 150)]

                // 右下角放置一个按钮
                [new(left: 450, top: 340)] =
                    HButton("右下角按钮",
                        strStyle: "fg-white bg-pink-300 rounded-lg",
                        click: _ => Console.WriteLine("右下角按钮被点击")
                    ), // HButton
                // HAbsolute.[new(left: 450, top: 340)]

                // 添加一个带半透明背景的浮动面板
                [new(left: 20, top: 300)] =
                    HStackPanel(new(
                        strStyle: "m-5 gap-2 flex-row bg-black")
                    {
                        HTextBlock("悬浮信息", style: new HTextStyle(foreground: Brushes.White)),
                        HTextBlock("(20,300)", style: new HTextStyle(foreground: Brushes.LightYellow))
                    }) // HStackPanel
                // HAbsolute.[new(left: 20, top: 300)]
            }), // HAbsolute

            // 说明文字
            HTextBlock("提示：Absolute 中元素通过 Left/Top 绝对定位，容器本身有固定宽高 600x400",
                strStyle: "text-xs fg-gray-500"
            ) // HTextBlock
        }); // rootElement

        return rootElement;
    }

    public static IElement AbsoluteDemo()
        => Element.WithScope(AbsoluteDemoComp);

    private static IElement ScrollDemoComp(UiScope uiScope)
    {
        var sb = new StringBuilder();
        for (var i = 1; i <= 40; i++)
        {
            sb.Append($"{i}: 这是一段很长的文本，用于测试垂直滚动。");
            if (i < 40) sb.Append('\n');
        }

        var longText = sb.ToString();

        var rootElement = uiScope.RootElement(new(strStyle: "horizontal")
        {
            HScrollViewer(new(
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                strStyle: "m-4 h-60 p-3 vertical bg-gray-500 rounded-xl border-blue border-w-4 shadow-sm")
            {
                HTextBlock(longText, strStyle: "text-base")
            }), // HScrollViewer
            HScrollViewer(new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Visible,
                strStyle: "mt-4 p-2 horizontal bg-white rounded-lg")
            {
                HStackPanel(new(strStyle: "gap-3 flex-row")
                {
                    HButton("按钮1"),
                    HButton("按钮2"),
                    HButton("按钮3"),
                    HButton("按钮4"),
                    HButton("按钮5")
                }) // HStackPanel
            }) // HScrollViewer
        }); // rootElement

        return rootElement;
    }

    public static IElement ScrollDemo()
        => Element.WithScope(ScrollDemoComp);

    // 在 DemoComponent 中添加一个演示
    private static IElement TextBoxDemoComp(UiScope uiScope)
    {
        var textSignal = new Signal<string>("初始文本");
        var log = new Signal<string>("");

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 flex-col")
        {
            HTextBox(
                bindText: textSignal,
                watermark: "输入点什么...",
                strStyle: "w-100 p-3 font-bold border-blue rounded-lg",
                textChanged: newText => log.Value = $"输入: {newText}"
            ), // HTextBox
            HTextBlock(new(() => $"实时内容: {textSignal.Value}"), strStyle: "mt-2"),
            HTextBlock(new(() => log.Value), strStyle: "fg-gray-500")
        }); // rootElement
        return rootElement;
    }

    public static IElement TextBoxDemo() => Element.WithScope(TextBoxDemoComp);
}

public record User(int? Id = null, string? Name = null);

public enum DemoView
{
    SimpleCounter,
    ForEachDemo,
    LoadingDemo,
    Unknown
}