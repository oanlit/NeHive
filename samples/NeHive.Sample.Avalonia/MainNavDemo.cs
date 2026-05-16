using System.Text;
using NeHive.Reactive;
using NeHive.UI.Avalonia;
using Avalonia.Controls.Primitives;
using NeHive.UI.Avalonia.Components;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;

namespace NeHive.Sample.Avalonia;

public static class DemoComponent
{
    #region Counter Component

    private static IElement CounterComp(int id, UiScope uiScope)
    {
        Console.WriteLine($"Counter {id} is creating");
        var count = new MutSignal<int>(0);
        var countText = () => $"Count: {count.RxValue}";

        var rootElement = uiScope.RootElement(new()
        {
            HTextBlock($"Id: {id}",
                strStyle: "text-lg font-bold fg-sky-200"
            ), // HTextBlock

            HTextBlock(countText,
                strStyle: "text-2xl fg-lime-300"
            ), // HTextBlock

            HButton("Add",
                strStyle: """
                          mt-1 ml-2 px-2 py-1 fg-white 
                          bg-green-300 hover:bg-green-400 click:bg-green-500
                          border-green-400 rounded-lg
                          """,
                click: _ => count.RxValue++
            ), // HButton

            HButton("Sub",
                strStyle: """
                          mt-1 ml-2 px-2 py-1 fg-white 
                          bg-pink-300 hover:bg-pink-400 click:bg-pink-500
                          border-pink-400 rounded-lg
                          """,
                click: _ => count.RxValue--
            ) // HButton
        }); // rootElement

        uiScope.OnDispose(() => Console.WriteLine($"Counter {id} disposed"));
        return rootElement;
    }

    public static IElement Counter(int prop)
        => Element.WithScope(CounterComp, prop);

    #endregion

    #region ForEach Demo

    private static IElement ForEachDemoComp()
    {
        var items = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        var rootElement = RootElement(new()
        {
            HStackPanel(new(strStyle: "mb-4 gap-3 flex-row")
            {
                HButton(out var addBtn, text: "Add Item", strStyle: "px-3 py-1 bg-blue-400 fg-white rounded-lg"),
                HButton(out var removeBtn, text: "Remove Last", strStyle: "px-3 py-1 bg-rose-400 fg-white rounded-lg"),
                HButton(out var removeSecBtn, text: "Remove Index 1",
                    strStyle: "px-3 py-1 bg-amber-400 fg-white rounded-lg"
                ) // HButton
            }), // HStackPanel
            ForEach<int>(new(items)
            {
                ComponentItem = Counter
            }) // ForEach<int>
        }); // rootElement

        addBtn.Click += _ =>
        {
            var arr = items.Value.ToList();
            arr.Add(arr.Count + 1);
            items.RxValue = arr;
        };

        removeBtn.Click += _ =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 0)
                arr.RemoveAt(arr.Count - 1);
            items.RxValue = arr;
        };

        removeSecBtn.Click += _ =>
        {
            var arr = items.Value.ToList();
            if (arr.Count > 1)
                arr.RemoveAt(1);
            items.RxValue = arr;
        };

        return rootElement;
    }

    public static IElement ForEachDemo()
        => Element.WithScope(ForEachDemoComp);

    #endregion

    #region Loading Demo

    private static IElement LoadingDemoComp(UiScope uiScope)
    {
        var userId = new MutSignal<int>(1);

        var userMemo = uiScope.CreateReactiveFlow(userId)
            .Debounce(500)
            .Filter(id => id > 0)
            .Map(id => new User(id, $"User {id}"))
            .PushAsyncMemo(async user =>
            {
                await Task.Delay(500);
                return user;
            }, initValue: new User(0, "Unknown"));

        var rootElement = uiScope.RootElement(new()
        {
            HStackPanel(new(strStyle: "mb-4 flex-row gap-3")
            {
                HButton("Increase User Id",
                    strStyle: "px-3 py-1 bg-blue-400 fg-white rounded-lg",
                    click: _ => userId.RxValue++
                ), // HButton
                HButton("Decrease User Id",
                    strStyle: "px-3 py-1 bg-slate-400 fg-white rounded-lg",
                    click: _ => userId.RxValue--
                ) // HButton
            }), // HStackPanel

            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock($"User Id: {user.Id}", strStyle: "mt-1.5 text-lg"),
                    HTextBlock($"Hello, {user.Name}", strStyle: "mt-1.5 fg-sky-600")
                ), // Loading<User>.Success
                Loading = () => HTextBlock("Fetching user data...", strStyle: "text-gray-500"),
                Error = ex =>
                    HButton($"Retry: {ex.Message}",
                        strStyle: "mt-2 px-3 py-1 bg-rose-400 fg-white rounded-lg",
                        click: _ => userMemo.Refetch()
                    ) // HButton
                // Loading<User>.Error
            }) // Loading<User>
        }); // rootElement

        return rootElement;
    }

    public static IElement LoadingDemo()
        => Element.WithScope(LoadingDemoComp);

    #endregion

    #region Grid Demo

    private static IElement GridDemoComp(UiScope uiScope)
    {
        var gapX = new MutSignal<int>(3);
        var rootElement = uiScope.RootElement(new()
        {
            HGrid(new(
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]),
                columnDefinitions: new([100, HgLen.Star()]),
                strStyle: new(() => $"m-2 gap-x-{gapX.RxValue} gap-y-3 bg-gray-100 rounded-xl p-4"))
            {
                [(row: 0, column: 0)] = HTextBlock("Top Left", strStyle: "text-base font-bold fg-slate-700"),
                [(row: 0, column: 1)] = HTextBlock("Top Right", strStyle: "text-base font-bold fg-slate-700"),
                [(row: 1, column: 0, rowSpan: 1, colSpan: 2)] =
                    HButton("Increase Horizontal Gap",
                        strStyle: "mt-2 px-3 py-1 bg-indigo-400 fg-white rounded-lg",
                        click: _ => gapX.RxValue++
                    ) // HButton
            }) // HGrid
        }); // rootElement
        return rootElement;
    }

    public static IElement GridDemo()
        => Element.WithScope(GridDemoComp);

    #endregion

    #region Absolute Demo

    private static IElement AbsoluteDemoComp(UiScope uiScope)
    {
        var rootElement = uiScope.RootElement(new(
            strStyle: "m-5 gap-4 flex-col bg-gray-50 rounded-xl p-4")
        {
            HTextBlock("Absolute Layout Demo",
                strStyle: "text-lg font-bold fg-slate-800"
            ), // HTextBlock

            HAbsolute(new(
                width: 600,
                height: 400)
            {
                [new(left: 10, top: 10)] =
                    HTextBlock("Top Left (10,10)", strStyle: "text-sm fg-blue-700"),

                [new(left: 450, top: 10)] =
                    HButton("Top Right Button",
                        strStyle: "px-2 py-1 fg-white bg-blue-300 rounded-lg",
                        click: _ => Console.WriteLine("Top Right Button Clicked")
                    ), // HButton

                [new(left: 150, top: 150)] =
                    HStackPanel(new(strStyle: "gap-2 flex-col bg-white p-3 rounded-lg shadow")
                    {
                        HTextBlock("Center Area", strStyle: "text-base font-bold"),
                        HTextBlock("Position (150,150)", strStyle: "text-xs fg-gray-500")
                    }), // HStackPanel

                [new(left: 450, top: 340)] =
                    HButton("Bottom Right Button",
                        strStyle: "fg-white bg-pink-300 rounded-lg px-2 py-1",
                        click: _ => Console.WriteLine("Bottom Right Button Clicked")
                    ), // HButton

                [new(left: 20, top: 300)] =
                    HStackPanel(new(strStyle: "gap-2 flex-row bg-slate-800/70 p-2 rounded-lg")
                    {
                        HTextBlock("Floating Panel", strStyle: "fg-white"),
                        HTextBlock("(20,300)", strStyle: "fg-yellow-200")
                    }) // HStackPanel
            }), // HAbsolute

            HTextBlock("Absolute layout uses fixed Left/Top positioning inside container",
                strStyle: "text-xs fg-gray-500"
            ) // HTextBlock
        }); // rootElement
        return rootElement;
    }

    public static IElement AbsoluteDemo()
        => Element.WithScope(AbsoluteDemoComp);

    #endregion

    #region Scroll Demo

    private static IElement ScrollDemoComp(UiScope uiScope)
    {
        var sb = new StringBuilder();
        for (var i = 1; i <= 40; i++)
        {
            sb.AppendLine($"Line {i}: Long scrollable content sample for NeHive UI framework.");
        }

        var longText = sb.ToString();

        var rootElement = uiScope.RootElement(new(strStyle: "m-4 flex-col")
        {
            HTextBlock("Scroll Viewer Demo", strStyle: "text-lg font-bold"),

            HScrollViewer(out var scroll, new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                strStyle: "m-2 h-60 p-3 vertical bg-gray-100 rounded-xl border border-gray-300")
            {
                HTextBlock(longText, strStyle: "text-base")
            }), // HScrollViewer

            HStackPanel(new(strStyle: "my-4 gap-x-16 flex-row justify-center")
            {
                HButton("⬆️ Top",
                    strStyle: "px-3 py-1 font-bold fg-sky-600 bg-sky-200 border-sky-400 rounded",
                    click: _ => ScrollToHome()
                ), // HButton
                HButton("⬇️ Bottom",
                    strStyle: "px-3 py-1 font-bold fg-green-600 bg-green-200 border-green-400 rounded",
                    click: _ => ScrollToEnd()
                ) // HButton
            }) // HStackPanel
        }); // rootElement
        return rootElement;

        void ScrollToHome()
        {
            scroll.ScrollToHome();
        }

        void ScrollToEnd()
        {
            scroll.ScrollToEnd();
        }
    }

    public static IElement ScrollDemo()
        => Element.WithScope(ScrollDemoComp);

    #endregion

    #region TextBox Demo

    private static IElement TextBoxDemoComp(UiScope uiScope)
    {
        var textSignal = new MutSignal<string>("Initial text");
        var log = new MutSignal<string>("");

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 flex-col")
        {
            HTextBox(
                bindText: textSignal,
                watermark: "Type something...",
                strStyle: "w-100 p-3 font-bold border border-blue-400 rounded-lg",
                textChanged: newText => log.RxValue = $"Input changed: {newText}"
            ), // HTextBox
            HTextBlock(new(() => $"Realtime Content: {textSignal.RxValue}"), strStyle: "mt-2 text-base"),
            HTextBlock(new(() => log.RxValue), strStyle: "text-sm fg-gray-500")
        }); // rootElement
        return rootElement;
    }

    public static IElement TextBoxDemo() => Element.WithScope(TextBoxDemoComp);

    #endregion

    #region Global Main Nav (Switch Demo As Entry)

    private static IElement MainNavDemoComp(UiScope uiScope)
    {
        var currentView = new MutSignal<DemoView>(DemoView.SimpleCounter);

        var rootElement = uiScope.RootElement(new(
            strStyle: "m-5 flex-col bg-gray-100 rounded-xl p-5")
        {
            // Top global navigation bar
            HStackPanel(new(strStyle: "my-4 gap-3 flex-row flex-wrap justify-center")
            {
                HButton("Simple Counter",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-blue-300 hover:bg-blue-400 border-blue-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.SimpleCounter
                ), // HButton

                HButton("ForEach List",
                    strStyle:
                    "px-3 py-1.5 text-sm fg-white bg-green-300 hover:bg-green-400 border-green-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.ForEachDemo
                ), // HButton

                HButton("Async Loading",
                    strStyle:
                    "px-3 py-1.5 text-sm fg-white bg-amber-300 hover:bg-amber-400 border-amber-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.LoadingDemo
                ), // HButton

                HButton("Grid Layout",
                    strStyle:
                    "px-3 py-1.5 text-sm fg-white bg-indigo-300 hover:bg-indigo-400 border-indigo-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.GridDemo
                ), // HButton

                HButton("Absolute Layout",
                    strStyle:
                    "px-3 py-1.5 text-sm fg-white bg-slate-300 hover:bg-slate-400 border-slate-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.AbsoluteDemo
                ), // HButton

                HButton("Scroll Viewer",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-teal-300 hover:bg-teal-400 border-teal-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.ScrollDemo
                ), // HButton

                HButton("TextBox Input",
                    strStyle: "px-3 py-1.5 text-sm fg-white bg-rose-300 hover:bg-rose-400 border-rose-400 rounded-2xl",
                    click: _ => currentView.RxValue = DemoView.TextBoxDemo
                ) // HButton
            }), // HStackPanel

            // Page content container
            HStackPanel(new(strStyle: "gap-3 bg-white rounded-xl p-4 shadow-sm")
            {
                Switch<DemoView>(new(currentView)
                {
                    Cases = new()
                    {
                        [DemoView.SimpleCounter] = () => Counter(999),
                        [DemoView.ForEachDemo] = ForEachDemo,
                        [DemoView.LoadingDemo] = LoadingDemo,
                        [DemoView.GridDemo] = GridDemo,
                        [DemoView.AbsoluteDemo] = AbsoluteDemo,
                        [DemoView.ScrollDemo] = ScrollDemo,
                        [DemoView.TextBoxDemo] = TextBoxDemo
                    }, // Switch<DemoView>.Cases
                    Default = () => HTextBlock("Select a demo from navigation buttons",
                        strStyle: "text-base fg-gray-500"
                    ) // Switch<DemoView>.Default
                }) // Switch<DemoView>
            }) // HStackPanel
        }); // rootElement

        return rootElement;
    }

    public static IElement MainNavDemo()
        => Element.WithScope(MainNavDemoComp);

    #endregion
}

public record User(int? Id = null, string? Name = null);

public enum DemoView
{
    SimpleCounter,
    ForEachDemo,
    LoadingDemo,
    GridDemo,
    AbsoluteDemo,
    ScrollDemo,
    TextBoxDemo,
    Unknown
}