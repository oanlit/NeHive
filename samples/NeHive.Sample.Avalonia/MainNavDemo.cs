using System.Text;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
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
                strStyle: "text-lg fw-bold fg-sky-200"
            ), // HTextBlock

            HTextBlock(countText,
                strStyle: "text-2xl fg-lime-300"
            ), // HTextBlock

            HButton("Add",
                strStyle: """
                          mt-1 ml-2 px-2 py-1 fg-white
                          bg-green-300 hover:bg-green-400 click:bg-green-500
                          border-w-1 border-green-400 rounded-lg
                          """,
                onClick: _ => count.RxValue++
            ), // HButton

            HButton("Sub",
                strStyle: """
                          mt-1 ml-2 px-2 py-1 fg-white 
                          bg-pink-300 hover:bg-pink-400 click:bg-pink-500
                          border-w-1 border-pink-400 rounded-lg
                          """,
                onClick: _ => count.RxValue--
            ) // HButton
        }); // rootElement

        uiScope.OnCleanup += () => Console.WriteLine($"Counter {id} disposed");
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
            HStackPanel(new(strStyle: "mb-4 gap-3 horizontal")
            {
                HButton(out var addBtn, text: "Add Item", strStyle: "px-3 py-1 bg-blue-400 fg-white rounded-lg"),
                HButton(out var removeBtn, text: "Remove Last", strStyle: "px-3 py-1 bg-rose-400 fg-white rounded-lg"),
                HButton(out var removeSecBtn, text: "Remove Index 1",
                    strStyle: "px-3 py-1 bg-amber-400 fg-white rounded-lg"
                ) // HButton
            }), // HStackPanel
            ForEach<int>(new(items)
            {
                Container = HScrollViewer(new(
                    horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                    verticalScrollBarVisibility: ScrollBarVisibility.Visible,
                    strStyle:
                    "m-2 min-h-60 max-h-80 w-100 gap-8 p-3 vertical bg-gray-100 rounded-xl border border-gray-300"
                )), // ForEach<int>.Container
                ItemTemplate = (id, index) =>
                {
                    Console.WriteLine($"index:{index.Value}");
                    return Counter(id);
                }
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
            HStackPanel(new(strStyle: "mb-4 horizontal gap-3")
            {
                HButton("Increase User Id",
                    strStyle: "px-3 py-1 bg-blue-400 fg-white rounded-lg",
                    onClick: _ => userId.RxValue++
                ), // HButton
                HButton("Decrease User Id",
                    strStyle: "px-3 py-1 bg-slate-400 fg-white rounded-lg",
                    onClick: _ => userId.RxValue--
                ) // HButton
            }), // HStackPanel

            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock($"User Id: {user.Id}", strStyle: "mt-1.5 text-lg"),
                    HTextBlock($"Hello, {user.Name}", strStyle: "mt-1.5 fg-sky-600")
                ), // Loading<User>.Success
                Loading = _ => HTextBlock("Fetching user data...", strStyle: "fg-gray-500"),
                Error = ex =>
                    HButton($"Retry: {ex.Message}",
                        strStyle: "mt-2 px-3 py-1 bg-rose-400 fg-white rounded-lg",
                        onClick: _ => userMemo.Refetch()
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
                [(row: 0, column: 0)] = HTextBlock("Top Left", strStyle: "text-base fw-bold fg-slate-700"),
                [(row: 0, column: 1)] = HTextBlock("Top Right", strStyle: "text-base fw-bold fg-slate-700"),
                [(row: 1, column: 0, rowSpan: 1, colSpan: 2)] =
                    HButton("Increase Horizontal Gap",
                        strStyle: "mt-2 px-3 py-1 bg-indigo-400 fg-white rounded-lg",
                        onClick: _ => gapX.RxValue++
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
            strStyle: "m-5 gap-4 vertical bg-gray-50 rounded-xl p-4")
        {
            HTextBlock("Absolute Layout Demo",
                strStyle: "text-lg fw-bold fg-slate-800"
            ), // HTextBlock

            HAbsolute(new(strStyle: "w-150 h-100 bg-gray-100")
            {
                [new(left: 10, top: 10)] =
                    HTextBlock("Top Left (10,10)", strStyle: "text-sm fg-blue-700"),

                [new(left: 450, top: 10)] =
                    HButton("Top Right Button",
                        strStyle: "px-2 py-1 fg-white bg-blue-300 rounded-lg",
                        onClick: _ => Console.WriteLine("Top Right Button Clicked")
                    ), // HButton

                [new(left: 150, top: 150)] =
                    HStackPanel(new(strStyle: "gap-2 vertical bg-white p-3 rounded-lg shadow")
                    {
                        HTextBlock("Center Area", strStyle: "text-base fw-bold"),
                        HTextBlock("Position (150,150)", strStyle: "text-xs fg-gray-500")
                    }), // HStackPanel

                [new(left: 450, top: 340)] =
                    HButton("Bottom Right Button",
                        strStyle: "fg-white bg-pink-300 rounded-lg px-2 py-1",
                        onClick: _ => Console.WriteLine("Bottom Right Button Clicked")
                    ), // HButton

                [new(left: 20, top: 300)] =
                    HStackPanel(new(strStyle: "gap-2 horizontal bg-slate-800/70 p-2 rounded-lg")
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

        var rootElement = uiScope.RootElement(new(strStyle: "w-full m-4 vertical items-center")
        {
            HTextBlock("Scroll Viewer Demo", strStyle: "text-lg fw-bold"),

            HScrollViewer(out var scroll, new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                strStyle: "m-2 w-full h-60 p-3 vertical bg-gray-100 rounded-xl border border-gray-300")
            {
                HTextBlock(longText, strStyle: "text-base")
            }), // HScrollViewer

            HStackPanel(new(strStyle: "my-4 gap-x-16 horizontal justify-center")
            {
                HContentButton(new(
                    strStyle:
                    "w-24 h-10 gap-4 px-3 py-1 horizontal justify-center items-center bg-sky-200 border-sky-400 rounded",
                    onClick: _ => ScrollToHome())
                {
                    HSvgImage("~/Assets/arrow-big-up-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-sky-600"),
                    HTextBlock("Top", strStyle: "fw-bold fg-sky-600 ")
                }), // HContentButton

                HContentButton(new(
                    strStyle:
                    "w-24 h-10 gap-2 px-3 py-1 horizontal justify-center items-center bg-green-200 border-green-400 rounded",
                    onClick: _ => ScrollToEnd())
                {
                    HSvgImage("~/Assets/arrow-big-down-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-green-600"),
                    HTextBlock("Bottom", strStyle: "fw-bold fg-green-600 ")
                }) // HContentButton
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

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HTextBox(
                bindText: textSignal,
                watermark: "Type something...",
                strStyle: "w-100 p-3 fw-bold border border-blue-400 rounded-lg",
                onTextChanged: newText => log.RxValue = $"Input changed: {newText}"
            ), // HTextBox
            HTextBlock(new(() => $"Realtime Content: {textSignal.RxValue}"), strStyle: "mt-2 text-base"),
            HTextBlock(new(() => log.RxValue), strStyle: "text-sm fg-gray-500")
        }); // rootElement
        return rootElement;
    }

    public static IElement TextBoxDemo() => Element.WithScope(TextBoxDemoComp);

    #endregion

    #region HCheckBox Demo

    private static IElement CheckBoxComp(UiScope uiScope)
    {
        var threeState = new MutSignal<bool?>(null);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HCheckBox(new(
                bindIsChecked: threeState,
                onClick: isChecked => Console.WriteLine($"Accept: {isChecked}"))
            {
                HTextBlock("I accept the terms")
            }), // HCheckBox
            HButton("Click Me!", onClick: _ => threeState.NotifySet(prev => prev is false))
        }); // rootElement
        return rootElement;
    }

    public static IElement CheckBoxDemo() => Element.WithScope(CheckBoxComp);

    #endregion

    #region RadioButton Demo

    private static IElement RadioButtonComp(UiScope uiScope)
    {
        var threeState = new MutSignal<bool?>(null);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HRadioButton(new(
                bindIsChecked: threeState,
                onClick: isChecked => Console.WriteLine($"Accept: {isChecked}"))
            {
                HTextBlock("I accept the terms")
            }), // HRadioButton
            HButton("OnCheckedChanged Me!", onClick: _ => threeState.NotifySet(prev => prev is false))
        }); // rootElement
        return rootElement;
    }

    public static IElement RadioButtonDemo() => Element.WithScope(RadioButtonComp);

    #endregion

    #region ToggleSwitch Demo

    private static IElement ToggleSwitchComp(UiScope uiScope)
    {
        var wifiEnabled = new MutSignal<bool?>(null);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HToggleSwitch(new(
                bindIsChecked: wifiEnabled,
                strStyle: "m-2",
                onCheckedChanged: isOn => Console.WriteLine($"WiFi: {isOn}"))
            {
                HTextBlock("Enable WiFi")
            }), // HToggleSwitch
            HTextBlock(new(() => $"WiFi is {(wifiEnabled.RxValue is true ? "ON" : "OFF")}"))
        }); // rootElement
        return rootElement;
    }

    public static IElement ToggleSwitchDemo() => Element.WithScope(ToggleSwitchComp);

    #endregion

    #region FilePicker Demo

    private static IElement FilePickerComp(UiScope uiScope)
    {
        FilePickerFilter[] filterFlies =
        [
            new("Images", "*.png", "*.jpg", "*.jpeg"),
            new("All files", "*.*")
        ];

        var selectedFile = new MutSignal<string?>(null);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HFilePicker(
                bindSelectedPath: selectedFile,
                title: "Choose an image",
                filters: filterFlies
            ), // HFilePicker
            HTextBlock(new(() => $"Selected: {selectedFile.RxValue ?? string.Empty}")),
            HStackPanel(new(strStyle: "w-64 h-64 overflow-hidden")
            {
                HUriImage(selectedFile,
                    stretch: Stretch.UniformToFill,
                    strStyle: "transition-transform ease-in-out duration-500 hover:scale-120"
                ) // HUriImage
            }) // HStackPanel
        }); // rootElement
        return rootElement;
    }

    public static IElement FilePickerDemo() => Element.WithScope(FilePickerComp);

    #endregion

    #region ProgressBar Demo

    private static IElement ProgressBarComp(UiScope uiScope)
    {
        var progress = new MutSignal<double>(0);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HProgressBar(value: progress, strStyle: "w-full h-4"),
            HButton("Increase", onClick: _ => progress.RxValue = Math.Min(100, progress.RxValue + 10))
        }); // rootElement
        return rootElement;
    }

    public static IElement ProgressBarDemo() => Element.WithScope(ProgressBarComp);

    #endregion

    #region SplitView Demo

    private static IElement SplitViewComp(UiScope uiScope)
    {
        var isPaneOpen = new MutSignal<bool>(true);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HSplitView(new(
                isPaneOpen: isPaneOpen,
                strStyle: "w-800 h-500",
                displayMode: SplitViewDisplayMode.CompactInline,
                openPaneLength: 200,
                compactPaneLength: 48)
            {
                Pane = HStackPanel(new HStackPanelProp(strStyle: "gap-2 p-4 bg-gray-200")
                {
                    HButton("Home", onClick: _ => { }),
                    HButton("Settings", onClick: _ => { })
                }),
                Content = HTextBlock("Main content", strStyle: "p-4")
            }),
            HButton("Toggle Pane", onClick: _ => isPaneOpen.RxValue = !isPaneOpen.RxValue)
        }); // rootElement
        return rootElement;
    }

    public static IElement SplitViewDemo() => Element.WithScope(SplitViewComp);

    #endregion

    #region SplitPanel Demo

    private static IElement SplitPanelComp(UiScope uiScope)
    {
        var splitPos = new MutSignal<double>(200);
        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HSplitPanel(new(strStyle: "h-40 w-100")
            {
                HTextBlock("Left Panel", strStyle: "bg-blue-200 mr-4"),
                HTextBlock("Right Panel", strStyle: "bg-green-200 ml-4")
            }), // HSplitPanel
            HStackPanel(new(strStyle: "h-60 gap-x-8 horizontal")
            {
                HSplitPanel(new(
                    splitFraction: 0.3,
                    strStyle: "w-50 h-50 vertical")
                {
                    HTextBlock("Top Panel"),
                    HTextBlock("Bottom Panel")
                }), // HSplitPanel
                HSplitPanel(new(strStyle: "w-100 h-50 horizontal",
                    splitPosition: splitPos)
                {
                    HTextBlock("Left"),
                    HTextBlock("Right")
                }), // HSplitPanel
            })
        }); // rootElement
        return rootElement;
    }

    public static IElement SplitPanelDemo() => Element.WithScope(SplitPanelComp);

    #endregion

    #region TabView Demo

    private static IElement TabViewComp(UiScope uiScope)
    {
        var currentTab = new MutSignal<int>(0);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HTabControl(new(bindSelectedIndex: currentTab)
            {
                ["Home"] = HTextBlock("欢迎页内容"),
                ["Settings"] = HTextBlock("设置页面"),
                ["Profile"] = HButton("编辑资料", onClick: _ => Console.WriteLine("编辑"))
            }), // HTabControl
            HButton("切换到第一页", onClick: _ => currentTab.RxValue = 0),
            HButton("切换到第二页", onClick: _ => currentTab.RxValue = 1)
        }); // rootElement
        return rootElement;
    }

    public static IElement TabViewDemo() => Element.WithScope(TabViewComp);

    #endregion

    #region Slider Demo

    private static IElement SliderComp(UiScope uiScope)
    {
        // 响应式双向绑定
        var volume = new MutSignal<double>(50);

        var rootElement = uiScope.RootElement(new(strStyle: "m-5 gap-4 vertical")
        {
            HSlider(
                bindValue: volume,
                minimum: 0,
                maximum: 100,
                isSnapToTickEnabled: true,
                tickFrequency: 10,
                tickPlacement: TickPlacement.Outside,
                strStyle: "w-32"
            ), // HSlider
            HTextBlock(new(() => $"音量: {volume.RxValue:F0}"))
        }); // rootElement
        return rootElement;
    }

    public static IElement SliderDemo() => Element.WithScope(SliderComp);

    #endregion

    #region TreeView Demo

    public static IElement TreeViewDemo()
    {
        var treeView = HTreeView(
            new HTreeViewProp(strStyle: "m-3 w-75 h-100")
            {
                new HTreeViewItemProp("Root 1")
                {
                    Children =
                    {
                        new HTreeViewItemProp("Child 1.1"),
                        new HTreeViewItemProp("Child 1.2")
                    } // Root 1.Children
                }, // Root 1
                new HTreeViewItemProp("Root 2", isExpanded: true)
                {
                    Children =
                    {
                        new HTreeViewItemProp("Child 2.1")
                    } // Root 2.Children
                } // Root 2
            }); // HTreeView;

        return treeView;
    }

    #endregion

    #region ListBox Demo

    public static IElement ListBoxDemo()
    {
        // 创建响应式数据源
        var users = new MutSignal<IReadOnlyList<User>>([
            new() { Name = "张三", Id = 28 },
            new() { Name = "李四", Id = 32 },
            new() { Name = "王五", Id = 25 }
        ]);

        var selectedUser = new MutSignal<User?>(null);

        var listBox = HStackPanel(new()
        {
            HListBox<User>(new(
                users,
                bindSelectedItem: selectedUser,
                selectionMode: SelectionMode.Single,
                strStyle: "w-full h-60 bg-gray-100"
            )
            {
                ItemTemplate = user => HTextBlock($"User ID: {user.Id} User Name: {user.Name}", strStyle: "p-2")
            }), // HListBox<User>
            HTextBlock(new(() => $"选中：{selectedUser.RxValue?.Name ?? "无"}")),
            HButton("添加用户", onClick: _ =>
            {
                var list = users.RxValue.ToList();
                list.Add(new User { Name = $"用户{list.Count + 1}", Id = list.Count + 1 });
                users.RxValue = list;
            }) // HButton
        }); // listBox

        return listBox;
    }

    #endregion

    #region ComboBox Demo

    public static IElement ComboBoxDemo()
    {
        // 创建响应式数据源
        var countries = new MutSignal<IReadOnlyList<Country>>([
            new Country { Name = "中国", Code = "CN" },
            new Country { Name = "美国", Code = "US" },
            new Country { Name = "日本", Code = "JP" }
        ]);

        var selectedCountry = new MutSignal<Country?>(null);

        var comboBox = HStackPanel(new(strStyle: "w-50")
        {
            HComboBox<Country>(new(
                countries,
                bindSelectedItem: selectedCountry,
                placeholderText: "请选择国家"
            )
            {
                ItemTemplate = c => HTextBlock($"{c.Name} ({c.Code})")
            }), // HListBox<User>
            HTextBlock(new(() => $"选中: {selectedCountry.RxValue?.Name ?? "未选"}"))
        }); // listBox

        return comboBox;
    }

    #endregion

    #region Menu Demo

    public static IElement MenuDemo()
    {
        var menu = HMenu(new()
        {
            new(header: "File")
            {
                new(header: "Open", onClick: () => Console.WriteLine("Open")),
                new(header: "Save", onClick: () => Console.WriteLine("Save")),
                new(header: "-"), // 分隔线（Header 为 "-" 时会自动渲染分隔符）
                new(header: "Exit", onClick: () => Environment.Exit(0))
            },
            new(header: "Edit")
            {
                new(header: "Copy"),
                new(header: "Paste")
            }
        });

        return menu;
    }

    #endregion

    #region GridSplitter Demo

    public static IElement GridSplitterDemo()
    {
        return HGrid(new(
            rowDefinitions: new([HgLen.Auto]),
            columnDefinitions: new([100, HgLen.Auto, HgLen.Star()]),
            strStyle: "m-2 p-4 bg-gray-100 rounded-xl")
        {
            [(0, 0)] = HTextBlock("Left", strStyle: "mr-2 text-xl"),
            [(0, 1)] = HGridSplitter(
                strStyle: "w-1 horizontal bg-gray-500"
            ),
            [(0, 2)] = HTextBlock("Right", strStyle: "ml-2 text-xl")
        }); // HGrid
    }

    #endregion

    #region UniformGrid Demo

    public static IElement UniformGridDemo()
    {
        var columnsSig = new MutSignal<int>(3);
        return HStackPanel(new()
        {
            HUniformGrid(new(
                rows: 2, columns: columnsSig,
                strStyle: "m-2 w-full p-2 gap-3 bg-gray-100 rounded-lg")
            {
                HTextBlock("1"),
                HTextBlock("2"),
                HTextBlock("3"),
                HTextBlock("4"),
                HTextBlock("5"),
                HTextBlock("6")
            }), // HUniformGrid
            HStackPanel(new(strStyle: "horizontal")
            {
                HButton("add columns", onClick: _ => columnsSig.RxValue++),
                HButton("sub columns", onClick: _ => columnsSig.RxValue--)
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region DockPanel Demo

    public static IElement DockPanelDemo()
    {
        return HDockPanel(new(strStyle: "w-170 h-100 bg-gray-200")
        {
            [Dock.Top] = HTextBlock("Top"),
            [Dock.Bottom] = HTextBlock("Bottom"),
            [Dock.Left] = HTextBlock("Left"),
            [Dock.Right] = HTextBlock("Right"),
            [Dock.Top] = HTextBlock("Fill area (last child fills remaining space)")
        }); // HDockPanel
    }

    #endregion

    #region WrapPanel Demo

    public static IElement WrapPanelDemo()
    {
        return HWrapPanel(new(
            itemWidth: 100,
            strStyle: "w-60 gap-2 bg-gray-100")
        {
            HButton("1"),
            HButton("2"),
            HButton("3 with long text"),
            HButton("4")
        }); // HWrapPanel
    }

    #endregion

    #region TextStyle Demo

    public static IElement TextStyleDemo()
    {
        return HScrollViewer(new()
        {
            HStackPanel(new(strStyle: "p-6 gap-6 vertical")
            {
                HTextBlock("Text Style Demo",
                    strStyle: "text-3xl fw-bold font-jetmono fg-sky-700"),

                // 字号
                DemoSection("Font Size",
                [
                    HTextBlock("text-xs", strStyle: "text-xs"),
                    HTextBlock("text-sm", strStyle: "text-sm"),
                    HTextBlock("text-base", strStyle: "text-base"),
                    HTextBlock("text-lg", strStyle: "text-lg"),
                    HTextBlock("text-xl", strStyle: "text-xl"),
                    HTextBlock("text-2xl", strStyle: "text-2xl"),
                    HTextBlock("text-3xl", strStyle: "text-3xl")
                ]), // DemoSection

                // 字重
                DemoSection("Font Weight",
                [
                    HTextBlock("Thin", strStyle: "fw-thin"),
                    HTextBlock("ExtraLight", strStyle: "fw-extralight"),
                    HTextBlock("Light", strStyle: "fw-light"),
                    HTextBlock("Normal", strStyle: "fw-normal"),
                    HTextBlock("Medium", strStyle: "fw-medium"),
                    HTextBlock("SemiBold", strStyle: "fw-semibold"),
                    HTextBlock("Bold", strStyle: "fw-bold"),
                    HTextBlock("ExtraBold", strStyle: "fw-extrabold"),
                    HTextBlock("Black", strStyle: "fw-black")
                ]), // DemoSection

                // 字体
                DemoSection("Font Family",
                [
                    HTextBlock("JetBrains Mono",
                        strStyle: "font-jetmono text-lg"),

                    HTextBlock("落霞与孤鹜齐飞 秋水共长天一色",
                        strStyle: "font-lxgw text-lg")
                ]), // DemoSection

                // 字体风格
                DemoSection("Font Style",
                [
                    HTextBlock("Italic", strStyle: "italic"),
                    HTextBlock("Oblique", strStyle: "oblique"),
                    HTextBlock("Normal", strStyle: "not-italic")
                ]), // DemoSection

                // 对齐
                DemoSection("Alignment",
                [
                    HTextBlock("Left",
                        strStyle: "w-80 p-2 text-left border border-gray-300"),

                    HTextBlock("Center",
                        strStyle: "w-80 p-2 text-center border border-gray-300"),

                    HTextBlock("Right",
                        strStyle: "p-2 w-80 text-right border border-gray-300")
                ]), // DemoSection

                // 装饰线
                DemoSection("Decoration",
                [
                    HTextBlock("Underline", strStyle: "underline"),
                    HTextBlock("Overline", strStyle: "overline"),
                    HTextBlock("Baseline", strStyle: "baseline"),
                    HTextBlock("Line Through", strStyle: "line-through"),
                    HTextBlock("Red Decoration", strStyle: "underline decoration-red-500"),
                    HTextBlock("Dashed Decoration", strStyle: "underline decoration-dashed"),
                    HTextBlock("Dotted Decoration", strStyle: "underline decoration-dotted")
                ]), // DemoSection

                // 字间距
                DemoSection("Letter Spacing",
                [
                    HTextBlock("Tracking Tighter", strStyle: "tracking-tighter"),
                    HTextBlock("Tracking Tight", strStyle: "tracking-tight"),
                    HTextBlock("Tracking Normal", strStyle: "tracking-normal"),
                    HTextBlock("Tracking Wide", strStyle: "tracking-wide"),
                    HTextBlock("Tracking Wider", strStyle: "tracking-wider"),
                    HTextBlock("Tracking Widest", strStyle: "tracking-widest"),
                    
                    HTextBlock("Tracking -1", strStyle: "-tracking-1"),
                    HTextBlock("Tracking -0.5", strStyle: "-tracking-0.5"),
                    HTextBlock("Tracking 0.5", strStyle: "tracking-0.5"),
                    HTextBlock("Tracking 1", strStyle: "tracking-1"),
                    HTextBlock("Tracking 2", strStyle: "tracking-2"),
                    HTextBlock("Tracking 4", strStyle: "tracking-4")
                ]), // DemoSection

                // 换行
                DemoSection("Wrapping",
                [
                    HTextBlock(
                        "Lorem ipsum dolor sit amet consectetur adipiscing elit",
                        strStyle: "w-50 wrap border border-gray-300"),

                    HTextBlock(
                        "Lorem ipsum dolor sit amet consectetur adipiscing elit",
                        strStyle: "w-50 whitespace-nowrap border border-gray-300")
                ]), // DemoSection
                
                // 行高
                DemoSection("Line Height",
                [
                    HTextBlock(
                        """
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        """,
                        strStyle: "w-80 p-2 wrap leading-4 border border-gray-300"
                    ),

                    HTextBlock(
                        """
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        """,
                        strStyle: "w-80 p-2 wrap leading-8 border border-gray-300"
                    ),

                    HTextBlock(
                        """
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        The quick brown fox jumps over the lazy dog.
                        """,
                        strStyle: "w-80 p-2 wrap leading-12 border border-gray-300"
                    )
                ]), // DemoSection

                // 裁剪
                DemoSection("Text Trimming",
                [
                    HTextBlock(
                        "This is a very very very long text",
                        strStyle: "w-40 truncate"),

                    HTextBlock(
                        "This is a very very very long text",
                        strStyle: "w-40 text-clip-char"),

                    HTextBlock(
                        "This is a very very very long text",
                        strStyle: "w-40 text-clip-start")
                ]), // DemoSection

                // 颜色
                DemoSection("Foreground",
                [
                    HTextBlock("Sky 500",
                        strStyle: "fg-sky-500"),

                    HTextBlock("Rose 500",
                        strStyle: "fg-rose-500"),

                    HTextBlock("Emerald 500",
                        strStyle: "fg-emerald-500")
                ]), // DemoSection
                
                // 综合
                DemoSection("Combined Typography",
                [
                    HTextBlock(
                        "NeHive UI Typography",
                        strStyle: "text-3xl tracking-2 fw-black italic fg-sky-700"
                    ), // HTextBlock

                    HTextBlock(
                        "JetBrains Mono Programming Font",
                        strStyle: "text-lg tracking-1 fw-semibold font-jetmono fg-emerald-600"
                    ), // HTextBlock

                    HTextBlock(
                        "落霞与孤鹜齐飞，秋水共长天一色",
                        strStyle: "text-xl tracking-1 leading-10 fw-medium font-lxgw fg-rose-600"
                    ), // HTextBlock

                    HTextBlock(
                        "Decorated Typography",
                        strStyle: "text-2xl fw-bold underline decoration-dashed decoration-w-2 decoration-violet-500"
                    ) // HTextBlock
                ]) // DemoSection
            }) // HStackPanel
        });

        static IElement DemoSection(string title, IEnumerable<IElement> children)
        {
            var stackPanelProp = new HStackPanelProp(strStyle: "gap-2 vertical");
            foreach (var child in children)
            {
                stackPanelProp.Add(child);
            }
            return HStackPanel(new(strStyle: "gap-2 vertical")
            {
                HTextBlock(title,
                    strStyle: "text-xl fw-bold fg-sky-700"),

                HStackPanel(stackPanelProp)
            });
        }
    }

    #endregion

    #region Group Demo

    public static IElement GroupDemo()
    {
        return HGroup(new(strStyle: "mx-auto my-auto")
        {
            Child = state => HStackPanel(new(
                strStyle: "gap-8 horizontal justify-center items-center")
            {
                HSvgImage("~/Assets/play.svg",
                    strStyle: new(() => $"w-16 h-16 fg-blue-{ToValue(state.IsHover.RxValue)}")),
                HSvgImage("~/Assets/skip-back.svg",
                    strStyle: new(() => $"w-16 h-16 fg-orange-{ToValue(state.IsHover.RxValue)}")),
                HSvgImage("~/Assets/skip-forward.svg",
                    strStyle: new(() => $"w-16 h-16 fg-yellow-{ToValue(state.IsHover.RxValue)}")),
                HSvgImage("~/Assets/stretch-vertical.svg",
                    strStyle: new(() => $"w-16 h-16 fg-red-{ToValue(state.IsHover.RxValue)}")),
            }) // HWrapPanel
        });

        string ToValue(bool isHover)
            => isHover ? "300" : "200";
    }

    #endregion

    #region ContextKey Demo

    private static readonly ContextKey<Signal<string>> Theme = new();
    private static readonly ContextKey<Action> ToggleTheme = new();

    public static IElement ContextDemo()
    {
        var theme = new MutSignal<string>("light");
        return HContext(ctx => ctx.SetContext(Theme, theme).SetContext(ToggleTheme, Toggle),
            () => HStackPanel(new(strStyle: "h-full p-8")
            {
                UseContextDemo1(),
                UseContextDemo2()
            }));

        void Toggle()
        {
            theme.RxValue = theme.Value is "dark" ? "light" : "dark";
        }
    }

    private static IElement UseContextDemo1()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null) throw new ArgumentNullException(nameof(theme));
            if (toggleTheme is null) throw new ArgumentNullException(nameof(toggleTheme));
            return uiScope.RootElement(new()
            {
                HContentButton(new(strStyle: new(() =>
                        $"""
                         px-4 py-2 horizontal 
                         {(theme.RxValue is "dark" ? "fg-gray-200 bg-gray-700" : "fg-gray-800 bg-gray-200")} 
                         transition-colors rounded-lg
                         """),
                    onClick: _ => toggleTheme())
                {
                    Switch<string>(new(theme)
                    {
                        Cases = new()
                        {
                            ["dark"] = () => HSvgImage("~/Assets/sun.svg", strStyle: "w-4 h-4 fw-extralight")
                        },
                        Default = () => HSvgImage("~/Assets/moon.svg", strStyle: "w-4 h-4 fw-extralight")
                    }), // Switch
                    HTextBlock(new(() => $"Switch to {(theme.RxValue is "dark" ? "light" : "dark")} mode"),
                        strStyle: "ml-1")
                }) // HContentButton
            }); // RootElement
        });
    }

    private static IElement UseContextDemo2()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null) throw new ArgumentNullException(nameof(theme));
            if (toggleTheme is null) throw new ArgumentNullException(nameof(toggleTheme));

            return uiScope.RootElement(new(strStyle: new(() =>
                $"""
                 mt-6 mx-auto max-w-md overflow-hidden 
                 {(theme.RxValue is "dark" ? "bg-gray-800 border-gray-700" : "bg-white border-gray-200")} 
                 border rounded-xl shadow-md transition-colors
                 """))
            {
                HStackPanel(new(strStyle: "p-6")
                {
                    HTextBlock("Theme Injection Demonstration",
                        strStyle: new(() => $"""
                                             text-2xl fw-bold 
                                             fg-{(theme.RxValue is "dark" ? "white" : "gray-900")}
                                             """)),
                    HTextBlock(new(() => $"Current Topic Mode: {theme.RxValue}"),
                        strStyle: new(() => $"""
                                             mt-2 
                                             fg-{(theme.RxValue is "dark" ? "gray-300" : "gray-600")}
                                             """)),
                    HTextBlock(
                        new(
                            "The background, text and border color of this card component will automatically change according to the theme."),
                        strStyle: new(() => $"""
                                             mt-2 
                                             text-{(theme.RxValue is "dark" ? "gray-400" : "gray-500")}
                                             """)),
                })
            });
        });
    }

    #endregion

    #region Global Main Nav (Switch Demo As Entry)

    private static IElement MainNavDemoComp(UiScope uiScope)
    {
        var currentView = new MutSignal<DemoView>(DemoView.SimpleCounter);

        var rootElement = uiScope.RootElement(new(
            strStyle: "m-5 w-full vertical rounded-xl p-5")
        {
            // Top global navigation bar
            HScrollViewer(new(
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                horizontalScrollBarVisibility: ScrollBarVisibility.Disabled,
                strStyle: "w-full max-h-150 bg-gray-100")
            {
                HUniformGrid(new(
                    columns: 4,
                    strStyle: "gap-3")
                {
                    NavButton("Simple Counter", DemoView.SimpleCounter, "bg-blue-300 border-blue-400"),
                    NavButton("ForEach List", DemoView.ForEachDemo, "bg-green-300 border-green-400"),
                    NavButton("Async Loading", DemoView.LoadingDemo, "bg-amber-300 border-amber-400"),
                    NavButton("Grid Layout", DemoView.GridDemo, "bg-indigo-300 border-indigo-400"),

                    NavButton("Absolute Layout", DemoView.AbsoluteDemo, "bg-slate-300 border-slate-400"),
                    NavButton("Scroll Viewer", DemoView.ScrollDemo, "bg-teal-300 border-teal-400"),
                    NavButton("TextBox Input", DemoView.TextBoxDemo, "bg-rose-300 border-rose-400"),
                    NavButton("CheckBox", DemoView.CheckBoxDemo, "bg-cyan-300 border-cyan-400"),

                    NavButton("RadioButton", DemoView.RadioButtonDemo, "bg-fuchsia-300 border-fuchsia-400"),
                    NavButton("ToggleSwitch", DemoView.ToggleSwitchDemo, "bg-lime-300 border-lime-400"),
                    NavButton("FilePicker", DemoView.FilePickerDemo, "bg-orange-300 border-orange-400"),
                    NavButton("ProgressBar", DemoView.ProgressBarDemo, "bg-emerald-300 border-emerald-400"),

                    NavButton("SplitView", DemoView.SplitViewDemo, "bg-violet-300 border-violet-400"),
                    NavButton("SplitPanel", DemoView.SplitPanelDemo, "bg-sky-300 border-sky-400"),
                    NavButton("TabControl", DemoView.TabViewDemo, "bg-pink-300 border-pink-400"),
                    NavButton("Slider", DemoView.SliderDemo, "bg-yellow-300 border-yellow-400"),

                    NavButton("TreeView", DemoView.TreeViewDemo, "bg-red-300 border-red-400"),
                    NavButton("ListBox", DemoView.ListBoxDemo, "bg-blue-400 border-blue-500"),
                    NavButton("ComboBox", DemoView.ComboBoxDemo, "bg-green-400 border-green-500"),
                    NavButton("Menu", DemoView.MenuDemo, "bg-purple-400 border-purple-500"),

                    NavButton("GridSplitter", DemoView.GridSplitterDemo, "bg-indigo-400 border-indigo-500"),
                    NavButton("UniformGrid", DemoView.UniformGridDemo, "bg-orange-400 border-orange-500"),
                    NavButton("DockPanel", DemoView.DockPanelDemo, "bg-cyan-400 border-cyan-500"),
                    NavButton("WrapPanel", DemoView.WrapPanelDemo, "bg-pink-400 border-pink-500")
                }), // HUniformGrid
                // Page content container
                HStackPanel(new(strStyle: "mt-4 w-full gap-3 bg-white rounded-xl p-4 shadow-sm")
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
                            [DemoView.TextBoxDemo] = TextBoxDemo,
                            [DemoView.CheckBoxDemo] = CheckBoxDemo,
                            [DemoView.RadioButtonDemo] = RadioButtonDemo,
                            [DemoView.ToggleSwitchDemo] = ToggleSwitchDemo,
                            [DemoView.FilePickerDemo] = FilePickerDemo,
                            [DemoView.ProgressBarDemo] = ProgressBarDemo,
                            [DemoView.SplitViewDemo] = SplitViewDemo,
                            [DemoView.SplitPanelDemo] = SplitPanelDemo,
                            [DemoView.TabViewDemo] = TabViewDemo,
                            [DemoView.SliderDemo] = SliderDemo,
                            [DemoView.TreeViewDemo] = TreeViewDemo,
                            [DemoView.ListBoxDemo] = ListBoxDemo,
                            [DemoView.ComboBoxDemo] = ComboBoxDemo,
                            [DemoView.MenuDemo] = MenuDemo,
                            [DemoView.GridSplitterDemo] = GridSplitterDemo,
                            [DemoView.UniformGridDemo] = UniformGridDemo,
                            [DemoView.DockPanelDemo] = DockPanelDemo,
                            [DemoView.WrapPanelDemo] = WrapPanelDemo
                        }, // Switch<DemoView>.Cases
                        Default = () => HTextBlock("Select a component demo",
                            strStyle: "text-base fg-gray-500"
                        ) // Switch<DemoView>.Default
                    }) // Switch<DemoView>
                }) // HStackPanel
            }) // HScrollViewer
        }); // rootElement

        return rootElement;

        IElement NavButton(
            string title,
            DemoView view,
            string colorClass)
        {
            return HButton(title,
                strStyle: $$"""
                            px-3 py-2 text-sm fw-bold fg-white
                            {{colorClass}}
                            hover:opacity-80
                            border-w-1 rounded-6
                            """,
                onClick: _ => currentView.RxValue = view
            );
        }
    }

    public static IElement MainNavDemo()
        => Element.WithScope(MainNavDemoComp);

    #endregion
}

public record User(int? Id = null, string? Name = null);

public record Country(string? Name = null, string? Code = null);

public enum DemoView
{
    SimpleCounter,
    ForEachDemo,
    LoadingDemo,
    GridDemo,
    AbsoluteDemo,
    ScrollDemo,
    TextBoxDemo,

    CheckBoxDemo,
    RadioButtonDemo,
    ToggleSwitchDemo,
    FilePickerDemo,
    ProgressBarDemo,

    SplitViewDemo,
    SplitPanelDemo,
    TabViewDemo,
    SliderDemo,

    TreeViewDemo,
    ListBoxDemo,
    ComboBoxDemo,
    MenuDemo,

    GridSplitterDemo,
    UniformGridDemo,
    DockPanelDemo,
    WrapPanelDemo,

    Unknown
}