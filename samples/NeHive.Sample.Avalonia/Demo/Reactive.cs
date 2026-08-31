using System.Text;
using NeHive.Model;
using NeHive.UI.Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Components;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;
using static NeHive.UI.Avalonia.Components.AttachComponent;

namespace NeHive.Sample.Avalonia;

public static partial class DemoComponent
{
    #region Show – Conditional Rendering Demo

    private static IElement ShowDemo()
    {
        var showFlag = new MutSignal<bool?>(true);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Show – Conditional Rendering", strStyle: SectionTitleStyle),
            HText("Toggle visibility of content using a boolean signal",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(_ => new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HToggleSwitch(_ => new(bindIsChecked: showFlag) { HText(" Show extra content") }),
                Show(new(new(() => showFlag.RxValue is true))
                {
                    IfTrue = () => HText("✨ Extra content is now visible",
                        strStyle: "fg-matcha-600 italic")
                }) // Show
            }) // HStackPanel
        });
    }

    #endregion

    #region Switch – Integer Branch Demo

    private static IElement SwitchDemo()
    {
        var switchValue = new MutSignal<int>(0);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Switch – Branch by Integer", strStyle: SectionTitleStyle),
            HText("Render different content based on discrete integer values",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(_ => new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Value 0", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 0),
                HButton("Value 1", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 1),
                HButton("Value 2", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 2)
            }), // HStackPanel
            HStackPanel(_ => new(strStyle: "mt-2 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full")
            {
                Switch<int>(new(switchValue)
                {
                    [0] = () => HText("Selected: 0 – default option", strStyle: "fg-matcha-700"),
                    [1] = () => HText("Selected: 1 – alternative option", strStyle: "fg-matcha-600"),
                    [2] = () => HText("Selected: 2 – third option", strStyle: "fg-matcha-500"),
                    Default = () => HText("Unknown value", strStyle: "fg-coffee-400")
                }) // Switch<int>
            }) // HStackPanel
        });
    }

    #endregion

    #region Match – Predicate-based Conditional Demo

    private static IElement MatchDemo()
    {
        var matchValue = new MutSignal<double>(2);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Match – Predicate-based Branching", strStyle: SectionTitleStyle),
            HText("Use lambdas to match complex conditions (range, equality, etc.)",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HSlider(bindValue: matchValue, minimum: 0, maximum: 10, strStyle: "w-64 horizontal"),
            HStackPanel(_ => new(strStyle: "mt-2 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full")
            {
                Match<double>(new(matchValue)
                {
                    [v => v is 0] = () => HText("⭐ Zero", strStyle: "fg-matcha-700"),
                    [v => v is > 0 and <= 5] = () => HText("🔵 Small (1–5)", strStyle: "fg-matcha-600"),
                    [v => v > 5] = () => HText("🟢 Large (6–10)", strStyle: "fg-matcha-500"),
                    Default = () => HText("No match", strStyle: "fg-coffee-400")
                }) // Match<double>
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region ForEach Dynamic Collection Demo

    private static IElement ForEachDemo()
    {
        var items = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("ForEach – Dynamic Collection Rendering", strStyle: SectionTitleStyle),
            HText("Items reactively added/removed; DOM updates only where changed",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(_ => new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Add Item",
                    strStyle: PrimaryBtnBase,
                    onClick: _ =>
                    {
                        var list = items.Value.ToList();
                        list.Add(list.Count + 1);
                        items.RxValue = list;
                    }
                ), // HButton
                HButton("Remove Last",
                    strStyle: SecondaryBtnBase,
                    onClick: _ =>
                    {
                        var list = items.Value.ToList();
                        if (list.Count > 0) list.RemoveAt(list.Count - 1);
                        items.RxValue = list;
                    }
                ), // HButton
                HButton("Remove Index 1",
                    strStyle: PrimaryBtnBase + " bg-matcha-600 hover:bg-matcha-700",
                    onClick: _ =>
                    {
                        var list = items.Value.ToList();
                        if (list.Count > 1) list.RemoveAt(1);
                        items.RxValue = list;
                    }
                ) // HButton
            }),

            HScrollViewer(_ => new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                verticalScrollBarVisibility: ScrollBarVisibility.Visible,
                strStyle: "min-h-60 max-h-75"
            )
            {
                ForEach<int>(new(items)
                {
                    ItemsPanel = HStackPanel(strStyle: ScrollContainerBase + "w-full p-3 gap-3 vertical"),
                    ItemTemplate = (id, index) => HStackPanel(_ => new()
                    {
                        HText(new(() => $"Index: {index.RxValue}"), strStyle: "fg-coffee-700 text-lg"),
                        Counter(id)
                    }) // ForEach<int>.ItemTemplate
                }) // ForEach<int>
            })
        });
    }

    #endregion

    #region Async Loading State Demo

    private static IElement LoadingDemoComp(UiScope uiScope)
    {
        var userId = new MutSignal<int>(1);

        var userMemo = uiScope.CreateReactiveFlow(userId)
            .Debounce(500)
            .Filter(id => id > 0)
            .Map(id => new User(id, $"User {id}"))
            .PushAsyncMemo(async user =>
            {
                await Task.Delay(500); // 模拟网络请求
                return user;
            }, initValue: new User(0, "Unknown User"));

        var rootElement = HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Loading – Asynchronous State Management", strStyle: SectionTitleStyle),
            HText("Debounced ID changes trigger async fetch; shows loading/error/success",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(_ => new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Increase ID",
                    strStyle: PrimaryBtnBase,
                    onClick: _ => userId.RxValue++
                ), // HButton
                HButton("Decrease ID",
                    strStyle: SecondaryBtnBase,
                    onClick: _ => userId.RxValue--
                ) // HButton
            }), // HStackPanel

            Loading<User>(new(userMemo)
            {
                Success = user => HStackPanel(_ => new()
                {
                    HText($"User ID: {user.Id}",
                        strStyle: "mt-2 text-lg fw-medium fg-matcha-800"),
                    HText($"Welcome, {user.Name}",
                        strStyle: "text-xl fw-semibold fg-matcha-600")
                }), // Loading<User>.Success
                Loading = _ =>
                    HStackPanel(_ =>
                        new(strStyle: HorizontalRowBase + " p-3 bg-matcha-50 rounded-lg w-full justify-center")
                        {
                            HText("Fetching user data...",
                                strStyle: "fg-coffee-500 italic animate-pulse")
                        }), // HStackPanel
                // Loading<User>.Loading
                Error = ex =>
                    HButton($"Retry: {ex.Message}",
                        strStyle: PrimaryBtnBase + " mt-2 bg-matcha-600 hover:bg-matcha-700",
                        onClick: _ => userMemo.Refetch()
                    ) // HButton
                // Loading<User>.Error
            })
        });

        return rootElement;
    }

    private static IElement LoadingDemo() => Element.WithScope(LoadingDemoComp);

    #endregion

    #region Component Lifecycle Demo

    private static IElement LifecycleDemoComp(UiScope uiScope)
    {
        var showExtra = new MutSignal<bool>(false);
        var logMessages = new MutSignal<IReadOnlyList<string>>(new List<string>());

        AddLog("Root component initialized");

        var root = HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Component Lifecycle Tracking Demo", strStyle: SectionTitleStyle),

            HButton("Toggle Dynamic Child Visibility",
                strStyle: PrimaryBtnBase + "bg-amber-400 fg-white border-amber-500",
                onClick: _ => showExtra.RxValue = !showExtra.Value),

            Show(new(showExtra)
            {
                IfTrue = () => Element.WithScope(childScope =>
                {
                    AddLog("Dynamic child component initialized");

                    var childRootElement = HStackPanel(_ =>
                        new(strStyle: "p-3 bg-amber-50 border border-amber-200 rounded-lg")
                        {
                            HText(
                                "This child component is dynamically created and destroyed. Toggle the button above to trigger its construction and cleanup callback.")
                        }); // childRootElement

                    childScope.OnMount += () => AddLog("Dynamic child component mounted");
                    childScope.OnCleanup += () => AddLog("Dynamic child component disposed");

                    return childRootElement;
                }),
                IfFalse = () => HText("Dynamic child hidden (no instance alive)", strStyle: "fg-gray-400 italic")
            }),

            HText("Lifecycle Event Log (Latest 10 Entries)", strStyle: "text-md fw-semibold mt-2"),
            HScrollViewer(_ => new(strStyle: ScrollContainerBase + "max-h-48")
            {
                ForEach<string>(new(logMessages)
                {
                    ItemsPanel = HStackPanel(strStyle: "gap-1 vertical"),
                    ItemTemplate = (msg, _) => HText(msg, strStyle: "text-xs fg-gray-700")
                }) // ForEach<string>
            }) // HScrollViewer
        });

        uiScope.OnCleanup += () => AddLog("Root component disposed");

        return root;

        void AddLog(string msg)
        {
            var list = logMessages.Value.ToList();
            list.Insert(0, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}");
            if (list.Count > 10) list.RemoveAt(list.Count - 1);
            logMessages.RxValue = list;
        }
    }

    public static IElement LifecycleDemo() => Element.WithScope(LifecycleDemoComp);

    #endregion

}
