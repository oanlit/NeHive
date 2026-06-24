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
    #region Global Shared Style Constants

    /// <summary>Base style for all card containers</summary>
    private const string DemoCardBase = """
                                        m-2 p-4 w-full border-w-2 bg-matcha-50 border-matcha-200 rounded-xl shadow-md
                                        hover:border-matcha-400 hover:shadow-lg 
                                        """;

    private const string DemoTitle = "text-2xl fw-bold fg-matcha-800 mb-4 tracking-tight";
    private const string DemoDesc = "text-sm fg-coffee-700 mb-3";
    private const string DemoContent = "p-4 bg-white rounded-xl border border-matcha-100";

    /// <summary>Vertical stack layout base</summary>
    private const string VerticalStackBase = "gap-4 vertical items-start w-full ";

    /// <summary>Horizontal row layout base with wrap support</summary>
    private const string HorizontalRowBase = "gap-3 horizontal items-center ";

    /// <summary>Primary action button base style</summary>
    private const string PrimaryBtnBase = @"px-3 py-1.5 fg-white bg-matcha-400 border border-matcha-500 rounded-lg 
hover:bg-matcha-500 click:bg-matcha-600 transition-transform duration-100 click:scale-95 ";

    /// <summary>Secondary / neutral button base style</summary>
    private const string SecondaryBtnBase = """
                                            px-2.5 py-1 rounded-md border border-coffee-200 bg-coffee-50 fg-coffee-700
                                            hover:bg-matcha-100 click:bg-matcha-200 transition-colors duration-100
                                            focus:ring-w-1 focus:ring-matcha-300 
                                            """;

    /// <summary>Text input universal base style</summary>
    private const string InputBaseStyle = """
                                          p-2.5 rounded-lg border border-matcha-200 bg-coffee-50 w-full
                                          hover:cursor-text hover:border-matcha-300
                                          focus:border-matcha-500 focus:ring-w-2 focus:ring-matcha-200 selection:bg-matcha-300 
                                          transition-colors duration-200 
                                          """;

    /// <summary>Scrollable container base style</summary>
    private const string ScrollContainerBase = "rounded-xl border border-matcha-200 bg-matcha-50 p-3 overflow-hidden ";

    /// <summary>Section header text unified style</summary>
    private const string SectionTitleStyle = "text-xl fw-bold fg-matcha-700 mb-2 tracking-wide ";

    #endregion

    #region Counter Component

    private static IElement Counter(int id)
    {
        var count = new MutSignal<int>(0);
        var countText = () => $"Count: {count.RxValue}";

        return HStackPanel(new(strStyle: DemoCardBase + " gap-3 vertical w-64")
        {
            HTextBlock($"Counter Instance #{id}",
                strStyle: "text-lg fw-semibold fg-matcha-700"
            ), // HTextBlock
            HTextBlock(countText,
                strStyle: "text-4xl fw-bold fg-matcha-600 leading-10"
            ), // HTextBlock
            HStackPanel(new(strStyle: HorizontalRowBase)
            {
                HButton("+1",
                    strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                    onClick: _ => count.RxValue++
                ), // HButton
                HButton("-1",
                    strStyle: PrimaryBtnBase +
                              " bg-coffee-400 hover:bg-coffee-500 click:bg-coffee-700 border-coffee-500",
                    onClick: _ => count.RxValue--
                ) // HButton
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region Grid Layout Demo

    private static IElement GridDemo()
    {
        var gapX = new MutSignal<int>(3);

        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("Grid Responsive Layout", strStyle: DemoTitle),
            HTextBlock("Define rows/columns with auto and star sizing", strStyle: DemoDesc),
            HGrid(new(
                showGridLines: true,
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]),
                columnDefinitions: new([120, HgLen.Star()]),
                strStyle: new(() => $"""
                                         {DemoContent} w-full min-h-48
                                         gap-x-{gapX.RxValue} gap-y-4
                                     """)
            )
            {
                [(0, 0)] = HTextBlock("Top Left Cell",
                    strStyle: "p-3 text-base fw-bold bg-matcha-100 fg-matcha-700 rounded"),
                [(0, 1)] = HTextBlock("Top Right Cell",
                    strStyle: "p-3 text-base fw-bold bg-matcha-50 fg-matcha-700 rounded"),
                [(1, 0, 1, 2)] =
                    HButton(new(() => $"Expand Horizontal Gap (current: {gapX.RxValue})"),
                        strStyle: PrimaryBtnBase + " w-full",
                        onClick: _ => gapX.RxValue++
                    ) // HButton
                // [(1, 0, 1, 2)]
            }) // HGrid
        }); // HStackPanel
    }

    #endregion

    #region Absolute Position Layout Demo

    private static IElement AbsoluteDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("Absolute Positioning Layout", strStyle: DemoTitle),
            HTextBlock("Fixed Left/Top offset positioning inside a relative container", strStyle: DemoDesc),
            HAbsolute(new(
                strStyle: "w-full h-80 bg-matcha-50 rounded-xl border border-matcha-200 relative overflow-hidden")
            {
                [new(left: 10, top: 10)] =
                    HTextBlock("Top Left Anchor (10,10)",
                        strStyle: "text-sm fg-matcha-700 p-1 bg-white/80 rounded shadow-sm"),

                [new(left: 420, top: 10)] =
                    HButton("Top Right Action",
                        strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                        onClick: _ => Console.WriteLine("Top Right Button Clicked")
                    ), // HButton

                [new(left: 120, top: 120)] =
                    HStackPanel(new(
                        strStyle: "gap-2 vertical bg-white p-4 rounded-xl shadow-md border border-matcha-100 w-48")
                    {
                        HTextBlock("Center Floating Panel", strStyle: "text-base fw-bold fg-matcha-800"),
                        HTextBlock("Fixed Offset (120,120)", strStyle: "text-xs fg-coffee-500")
                    }), // HStackPanel

                [new(left: 420, top: 280)] =
                    HButton("Bottom Right Action",
                        strStyle: PrimaryBtnBase +
                                  " bg-coffee-400 hover:bg-coffee-500 click:bg-coffee-700 border-coffee-500",
                        onClick: _ => Console.WriteLine("Bottom Right Button Clicked")
                    ), // HButton

                [new(left: 20, top: 250)] =
                    HStackPanel(new(
                        strStyle: "gap-2 horizontal bg-matcha-900/80 p-3 rounded-xl shadow-md")
                    {
                        HTextBlock("Overlay Floating Panel", strStyle: "fg-matcha-50 fw-medium"),
                        HTextBlock("Coordinate (20,250)", strStyle: "fg-matcha-200 text-sm")
                    }) // HStackPanel
            }),
            HTextBlock("Absolute uses fixed Left/Top offsets for child positioning",
                strStyle: "text-xs fg-coffee-400 mt-2 italic")
        });
    }

    #endregion

    #region SplitView Sidebar Layout Demo

    private static IElement SplitViewDemo()
    {
        var isPaneOpen = new MutSignal<bool>(true);

        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("SplitView Collapsible Sidebar", strStyle: DemoTitle),
            HTextBlock("Compact inline mode with expand/collapse support", strStyle: DemoDesc),
            HSplitView(new(
                isPaneOpen: isPaneOpen,
                displayMode: SplitViewDisplayMode.CompactInline,
                openPaneLength: 200,
                compactPaneLength: 48,
                strStyle: "w-full h-40 rounded-xl overflow-hidden border border-matcha-200")
            {
                Pane = HStackPanel(new HPanelProp(strStyle: "gap-2 p-4 bg-matcha-50 vertical h-full")
                {
                    HButton("Home Dashboard", strStyle: SecondaryBtnBase + " w-full text-left"),
                    HButton("Application Settings", strStyle: SecondaryBtnBase + " w-full text-left")
                }), // HSplitView.Pane
                Content = HStackPanel(new(strStyle: "h-full gap-2 p-6 vertical bg-matcha-300")
                {
                    HTextBlock("Main Content Area", strStyle: "text-lg fw-bold fg-matcha-800"),
                    HTextBlock("Sidebar supports expand/collapse in compact inline mode",
                        strStyle: "fg-coffee-500")
                }) // // HSplitView.Content
            }),
            HButton("Toggle Sidebar",
                strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                onClick: _ => isPaneOpen.RxValue = !isPaneOpen.RxValue)
        });
    }

    #endregion

    #region SplitPanel Resizable Divider Demo

    private static IElement SplitPanelDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("SplitPanel Draggable Resizable Divider", strStyle: DemoTitle),
            HTextBlock("Drag to resize regions horizontally or vertically", strStyle: DemoDesc),
            HTextBlock("Horizontal Split", strStyle: "text-base fw-medium fg-matcha-700"),
            HSplitPanel(new(strStyle: "h-40 w-full rounded-xl overflow-hidden border border-matcha-200")
            {
                HTextBlock("Left Region",
                    strStyle: "mr-4 p-4 w-full h-full text-center bg-matcha-100 fg-matcha-800"),
                HTextBlock("Right Region",
                    strStyle: "ml-4 p-4 w-full h-full text-center bg-matcha-50 fg-matcha-800")
            }), // HSplitPanel
            HTextBlock("Vertical Split + Fixed Offset (200px)", strStyle: "text-base fw-medium fg-matcha-700 mt-2"),
            HStackPanel(new(strStyle: "w-full h-60 gap-x-6 horizontal")
            {
                HSplitPanel(new(
                    splitFraction: 0.3,
                    strStyle: "h-full vertical rounded-xl overflow-hidden border border-matcha-200")
                {
                    HTextBlock("Top Region", strStyle: "w-full p-4 text-center fg-matcha-800 bg-matcha-100"),
                    HTextBlock("Bottom Region", strStyle: "w-full p-4 text-center fg-matcha-800 bg-matcha-50")
                }), // HSplitPanel
                HSplitPanel(new(
                    strStyle: "h-full horizontal rounded-xl overflow-hidden border border-matcha-200",
                    splitPosition: 200)
                {
                    HTextBlock("Left Panel", strStyle: "bg-matcha-100 p-4 h-full text-center fg-matcha-800"),
                    HTextBlock("Right Panel", strStyle: "bg-matcha-50 p-4 h-full text-center fg-matcha-800")
                }) // HSplitPanel
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region DockPanel Edge Dock Layout Demo

    private static IElement DockPanelDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("DockPanel Edge Docking Layout", strStyle: DemoTitle),
            HTextBlock("Children dock to Top, Bottom, Left, Right; last fills remaining space", strStyle: DemoDesc),
            HDockPanel(new(strStyle: "w-full h-80 bg-matcha-50 rounded-xl overflow-hidden border border-matcha-200")
            {
                [Dock.Top] = HTextBlock("Top Dock Region",
                    strStyle: "p-3 bg-matcha-200 text-center fw-medium fg-matcha-800"),
                [Dock.Bottom] = HTextBlock("Bottom Dock Region",
                    strStyle: "p-3 bg-matcha-100 text-center fw-medium fg-matcha-800"),
                [Dock.Left] = HTextBlock("Left Dock Region",
                    strStyle: "p-3 bg-matcha-150 w-20 text-center fw-medium fg-matcha-800"),
                [Dock.Right] = HTextBlock("Right Dock Region",
                    strStyle: "p-3 bg-matcha-150 w-20 text-center fw-medium fg-matcha-800"),
                [null] = HTextBlock("Fill Remaining Space (last child)",
                    strStyle: "p-4 bg-white text-center fw-medium fg-matcha-800")
            }) // HDockPanel
        }); // HStackPanel
    }

    #endregion

    #region WrapPanel Auto Wrap Flow Layout Demo

    private static IElement WrapPanelDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("WrapPanel Auto‑Wrap Flow Layout", strStyle: DemoTitle),
            HTextBlock("Items automatically wrap to next line when width exceeds container", strStyle: DemoDesc),
            HWrapPanel(new(
                itemWidth: 120,
                strStyle: "w-full gap-3 bg-matcha-50 p-4 rounded-xl border border-matcha-200")
            {
                HButton("Item 1", strStyle: SecondaryBtnBase),
                HButton("Item 2", strStyle: SecondaryBtnBase),
                HButton("Long Text Content Item 3", strStyle: SecondaryBtnBase),
                HButton("Item 4", strStyle: SecondaryBtnBase)
            }) // HWrapPanel
        }); // HStackPanel
    }

    #endregion

    #region UniformGrid Equal Cell Grid Demo

    private static IElement UniformGridDemo()
    {
        var columnsSig = new MutSignal<int>(3);

        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("UniformGrid Equal‑Size Grid", strStyle: DemoTitle),
            HTextBlock("All cells have identical size, columns/rows adjustable", strStyle: DemoDesc),
            HUniformGrid(new(
                rows: 2,
                columns: columnsSig,
                strStyle: $"{DemoContent} gap-3 transition-colors")
            {
                HTextBlock("Cell 1", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HTextBlock("Cell 2", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800"),
                HTextBlock("Cell 3", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HTextBlock("Cell 4", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800"),
                HTextBlock("Cell 5", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HTextBlock("Cell 6", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800")
            }), // HUniformGrid
            HStackPanel(new(strStyle: HorizontalRowBase + "mt-2")
            {
                HButton("Add Column", strStyle: SecondaryBtnBase, onClick: _ => columnsSig.RxValue++),
                HButton("Remove Column", strStyle: SecondaryBtnBase, onClick: _ => columnsSig.RxValue--)
            }) // HStackPanel
        });
    }

    #endregion

    #region GridSplitter Resize Grid Column Demo

    private static IElement GridSplitterDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("GridSplitter Column Resizing", strStyle: DemoTitle),
            HTextBlock("Drag the divider to resize adjacent grid columns", strStyle: DemoDesc),
            HGrid(new(
                rowDefinitions: new([HgLen.Auto]),
                columnDefinitions: new([100, HgLen.Auto, HgLen.Star()]),
                strStyle: $"{DemoContent} min-h-32")
            {
                [(0, 0)] = HTextBlock("Left Column",
                    strStyle: "mr-2 p-3 bg-matcha-100 rounded h-full fg-matcha-800 text-center"),
                [(0, 1)] = HGridSplitter(
                    strStyle:
                    "w-1 h-full horizontal bg-matcha-300 hover:bg-matcha-500 transition-colors cursor-ew-resize"
                ),
                [(0, 2)] = HTextBlock("Right Column",
                    strStyle: "ml-2 p-3 bg-matcha-50 rounded h-full fg-matcha-800 text-center")
            }) // HGrid
        }); // HStackPanel
    }

    #endregion

    #region ScrollViewer Container Demo

    private static IElement ScrollDemo()
    {
        var sb = new StringBuilder();
        for (var i = 1; i <= 40; i++)
        {
            sb.AppendLine(
                $"Line {i}: Long vertical scrollable text content sample for NeHive UI framework demonstration.");
        }

        var longText = sb.ToString();

        return HStackPanel(new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HTextBlock("ScrollViewer Scrollable Container", strStyle: DemoTitle),
            HTextBlock("Scrollable area with hidden horizontal and auto vertical scrollbars", strStyle: DemoDesc),
            HScrollViewer(out var scroll, new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                strStyle: "w-full h-60 vertical bg-white rounded-xl border border-matcha-200 p-3 overflow-hidden"
            )
            {
                HTextBlock(longText, strStyle: "text-base leading-relaxed fg-matcha-800")
            }), // HScrollViewer
            HStackPanel(new(strStyle: HorizontalRowBase + " justify-center")
            {
                HContentButton(new(
                    strStyle: PrimaryBtnBase +
                              " w-36 h-10 bg-matcha-400 hover:bg-matcha-500",
                    onClick: _ => scroll.ScrollToHome()
                )
                {
                    Content = HStackPanel(new(strStyle: "gap-2 horizontal")
                    {
                        HSvgImage("~/Assets/arrow-big-up-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-white"),
                        HTextBlock("Scroll To Top", strStyle: "fw-bold text-xs fg-white")
                    })
                }), // HContentButton
                HContentButton(new(
                    strStyle: PrimaryBtnBase +
                              " w-36 h-10 bg-coffee-400 hover:bg-coffee-500 click:bg-coffee-700 border-coffee-500",
                    onClick: _ => scroll.ScrollToEnd()
                )
                {
                    Content = HStackPanel(new(strStyle: "gap-2 horizontal")
                    {
                        HSvgImage("~/Assets/arrow-big-down-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-white"),
                        HTextBlock("Scroll To Bottom", strStyle: "fw-bold text-xs fg-white")
                    }) // HStackPanel
                }) // HContentButton
            })
        });
    }

    #endregion

    #region TextBox Input Demo

    private static IElement TextBoxDemo()
    {
        var textSignal = new MutSignal<string>("Default input text value");
        var log = new MutSignal<string>("");

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("TextBox Text Input Control Demo", strStyle: SectionTitleStyle),
            HTextBox(
                bindText: textSignal,
                // placeholderText: "Enter custom text content here...",
                strStyle: InputBaseStyle + "text-base focus:ring-offset-2",
                onTextChanged: newText => log.RxValue = $"Input content updated: {newText}"
            ), // HTextBox
            HTextBox(
                bindText: textSignal,
                // placeholderText: "Type something...",
                strStyle: InputBaseStyle + "text-base selection:fg-matcha-800",
                onTextChanged: newText => log.RxValue = $"Input content updated: {newText}"
            ), // HTextBox
            HTextBlock(new(() => $"Realtime Bound Value: {textSignal.RxValue}"),
                strStyle: "mt-2 text-base fw-medium fg-gray-800"),
            HTextBlock(log, strStyle: "text-sm fg-gray-500 mt-1 italic")
        }); // HStackPanel
    }

    #endregion

    #region CheckBox Three-State Demo

    private static IElement CheckBoxDemo()
    {
        var threeState = new MutSignal<bool?>(null);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("CheckBox Tri-State Selection Demo", strStyle: SectionTitleStyle),
            HCheckBox(new(
                bindIsChecked: threeState,
                onClick: isChecked => Console.WriteLine($"Agreement Checkbox State: {isChecked}"),
                strStyle: "p-2 rounded-lg hover:bg-gray-50 transition-colors w-full"
            )
            {
                HTextBlock("I agree to the service terms", strStyle: "text-base fg-gray-800 ml-2")
            }), // HCheckBox
            HButton("Toggle Checkbox State", strStyle: SecondaryBtnBase,
                onClick: _ => threeState.NotifySet(prev => prev is false))
        }); // HStackPanel
    }

    #endregion

    #region RadioButton Single Select Demo

    private static IElement RadioButtonDemo()
    {
        var threeState = new MutSignal<bool?>(null);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("RadioButton Exclusive Single Select Demo", strStyle: SectionTitleStyle),
            HRadioButton(new(
                bindIsChecked: threeState,
                onClick: isChecked => Console.WriteLine($"Radio Selection State: {isChecked}"),
                strStyle: "p-2 rounded-lg hover:bg-gray-50 transition-colors w-full"
            )
            {
                HTextBlock("I accept the privacy policy", strStyle: "text-base fg-gray-800 ml-2")
            }),
            HButton("Switch Radio Selected State", strStyle: SecondaryBtnBase,
                onClick: _ => threeState.NotifySet(prev => prev is false))
        }); // HStackPanel
    }

    #endregion

    #region ToggleSwitch On/Off Switch Demo

    private static IElement ToggleSwitchDemo()
    {
        var wifiEnabled = new MutSignal<bool?>(null);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("ToggleSwitch Boolean Switch Control Demo", strStyle: SectionTitleStyle),
            HToggleSwitch(new(
                bindIsChecked: wifiEnabled,
                strStyle: "m-2 p-2 rounded-lg hover:bg-gray-50 w-full transition-colors",
                onCheckedChanged: isOn => Console.WriteLine($"Wireless Network Toggle State: {isOn}")
            )
            {
                HTextBlock("Enable Wireless Network", strStyle: "text-base fg-gray-800 ml-2")
            }), // HToggleSwitch
            HTextBlock(
                new(() => $"Wireless Network Status: {(wifiEnabled.RxValue is true ? "ENABLED" : "DISABLED")}"),
                strStyle: "mt-1 text-lg fw-medium")
        }); // HStackPanel
    }

    #endregion

    #region File Picker Dialog Demo

    private static IElement FilePickerDemo()
    {
        FilePickerFilter[] filterFlies =
        [
            new("Image Files", "*.png", "*.jpg", "*.jpeg"),
            new("All File Types", "*.*")
        ];

        var selectedFile = new MutSignal<string?>(null);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("FilePicker Local File Selection Dialog Demo", strStyle: SectionTitleStyle),
            HFilePicker(
                bindSelectedPath: selectedFile,
                title: "Select An Image File",
                filters: filterFlies,
                strStyle: InputBaseStyle
            ), // HFilePicker
            HTextBlock(new(() => $"Selected File Path: {selectedFile.RxValue ?? "No file selected"}"),
                strStyle: "mt-2 text-sm fg-gray-700 break-all"),
            HBlock(new(strStyle: "mt-2 w-64 h-64 overflow-hidden bg-gray-50 border border-gray-200 rounded-xl")
            {
                Child = HUriImage(selectedFile,
                    stretch: Stretch.UniformToFill,
                    strStyle: "transition-transform ease-in-out duration-500 hover:scale-110"
                ), // HUriImage
            }) // HBlock
        }); // HStackPanel
    }

    #endregion

    #region ProgressBar Progress Indicator Demo

    private static IElement ProgressBarDemo()
    {
        var progress = new MutSignal<double>(0);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("ProgressBar Task Progress Indicator Demo", strStyle: SectionTitleStyle),
            HProgressBar(value: progress, strStyle: "w-full h-4 rounded-full overflow-hidden bg-gray-200"),
            HStackPanel(new(strStyle: HorizontalRowBase + "mt-2")
            {
                HButton("Add 10% Progress", strStyle: PrimaryBtnBase + "bg-emerald-400 fg-white border-emerald-500",
                    onClick: _ => progress.RxValue = Math.Min(100, progress.RxValue + 10)),
                HButton("Reset Progress To Zero", strStyle: SecondaryBtnBase, onClick: _ => progress.RxValue = 0)
            }), // HStackPanel
            HTextBlock(new(() => $"Current Completion Rate: {progress.RxValue:F0}%"),
                strStyle: "mt-1 fw-medium fg-gray-700")
        }); // HStackPanel
    }

    #endregion

    #region Slider Value Slider Control Demo

    private static IElement SliderDemo()
    {
        var volume = new MutSignal<double>(50);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Slider Continuous Value Adjustment Demo", strStyle: SectionTitleStyle),
            HSlider(
                bindValue: volume,
                minimum: 0,
                maximum: 100,
                isSnapToTickEnabled: true,
                tickFrequency: 10,
                tickPlacement: TickPlacement.Outside,
                strStyle: "w-72 h-6"
            ), // HSlider
            HTextBlock(new(() => $"Audio Volume Level: {volume.RxValue:F0}"),
                strStyle: "mt-2 text-xl fw-semibold fg-matcha-600")
        }); // HStackPanel
    }

    #endregion

    #region Flyout Demo

    private static IElement FlyoutDemo()
    {
        var select = new MutSignal<string>("");
        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Flyout Demo", strStyle: SectionTitleStyle),
            HStackPanel(new(strStyle: HorizontalRowBase)
            {
                HFlyout(new(
                    placement: PlacementMode.Right,
                    showMode: FlyoutShowMode.Transient
                )
                {
                    Host = (host, flyout) =>
                        HButton("File",
                            strStyle: PrimaryBtnBase,
                            onClick: _ => flyout.ShowAt(host, showAtPointer: true)),
                    Content = flyout => HStackPanel(new(strStyle: VerticalStackBase +
                                                                  "gap-y-1 p-2 bg-matcha-50 border border-matcha-300 rounded-lg shadow")
                    {
                        HButton("Open", strStyle: "text-sm fg-matcha-700 bg-matcha-200/0 hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Open")),
                        // HButton
                        HButton("Save", strStyle: "text-sm fg-matcha-700 bg-transparent hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Save")),
                        // HButton
                    }) // HFlyout.Content
                }), // HFlyout

                HFlyout(new(
                    placement: PlacementMode.Right,
                    showMode: FlyoutShowMode.Transient
                )
                {
                    Host = (host, flyout) =>
                        HButton("Edit",
                            strStyle: PrimaryBtnBase,
                            onClick: _ => flyout.ShowAt(host, showAtPointer: true)),
                    Content = flyout => HStackPanel(new(strStyle: VerticalStackBase +
                                                                  "gap-y-1 p-2 bg-matcha-50 border border-matcha-300 rounded-lg shadow")
                    {
                        HButton("Copy", strStyle: "text-sm fg-matcha-700 bg-matcha-200/0 hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Copy")),
                        // HButton
                        HButton("Paste", strStyle: "text-sm fg-matcha-700 bg-transparent hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Paste")),
                        // HButton
                    }) // HFlyout.Content
                }), // HFlyout
            }),
            HTextBlock(new(() => $"You Selected: {select.RxValue}."),
                strStyle: "fg-coffee-500")
        }); // HStackPanel

        void SetSelect(Flyout flyout, string value)
        {
            select.RxValue = value;
            flyout.Hide();
        }
    }

    #endregion

    #region Window Demo

    private static IElement WindowDemoComp(UiScope scope)
    {
        var text = new MutSignal<string>("");

        return scope.RootElement(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Window & Dialog Management", strStyle: SectionTitleStyle),
            HTextBlock("Open a modal dialog to input text; result reflects back here",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HTextBlock(new(() => string.IsNullOrEmpty(text.RxValue) ? "(No input yet)" : text.RxValue),
                strStyle:
                "text-lg fw-medium fg-matcha-800 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full"),

            HStackPanel(new(strStyle: HorizontalRowBase + "mt-2")
            {
                HButton("Open Dialog",
                    strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                    onClick: _ => OpenDialog(scope, text))
            }) // HStackPanel
        });
    }

    private static void OpenDialog(UiScope scope, MutSignal<string> text)
    {
        var parentWindow = scope.GetContext(NeHiveUiContext.Window);
        if (parentWindow is null)
        {
            Console.WriteLine("Parent Window is null");
            return;
        }

        var dialog = scope.CreateWindow((window, _) =>
        {
            return HStackPanel(new(strStyle: VerticalStackBase + "mt-2 w-100 h-60 p-4")
            {
                HTextBlock("Enter your text", strStyle: "text-lg fw-semibold fg-matcha-800"),
                HTextBox(
                    bindText: text,
                    // placeholderText: "Type something...",
                    strStyle: InputBaseStyle + "text-base focus:border-matcha-500"
                ), // HTextBox
                HStackPanel(new(strStyle: HorizontalRowBase + "justify-end gap-2 mt-2")
                {
                    HButton("Cancel",
                        strStyle: PrimaryBtnBase +
                                  " bg-coffee-400 hover:bg-coffee-500 click:bg-coffee-700 border-coffee-500",
                        onClick: _ => CancelInput(window)
                    ), // HButton
                    HButton("OK",
                        strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                        onClick: _ => window.Close())
                }) // HStackPanel
            }); // HStackPanel
        }); // dialog

        // dialog.Width = 400;
        // dialog.Height = 240;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog(parentWindow);
        return;

        void CancelInput(Window window)
        {
            text.RxValue = "";
            window.Close();
        }
    }

    private static IElement WindowDemo()
        => Element.WithScope(WindowDemoComp);

    #endregion

    #region TreeView Hierarchical Tree Demo

    private static IElement TreeViewDemo()
    {
        return HStackPanel(new(strStyle: VerticalStackBase)
        {
            HTextBlock("TreeView Hierarchical Data Tree Demo", strStyle: SectionTitleStyle),
            HTreeView(new(strStyle: DemoCardBase + "w-80 h-96 overflow-hidden")
            {
                new HTreeViewItemProp("Root Directory 1",
                    strStyle: "p-1 rounded hover:bg-gray-100 transition-colors")
                {
                    Children =
                    {
                        new HTreeViewItemProp("Sub Folder 1.1", strStyle: "p-1 hover:bg-gray-50 rounded"),
                        new HTreeViewItemProp("Sub Folder 1.2", strStyle: "p-1 hover:bg-gray-50 rounded")
                    } // HTreeViewItemProp.Children
                }, // HTreeViewProp
                new HTreeViewItemProp("Root Directory 2", isExpanded: true,
                    strStyle: "p-1 rounded hover:bg-gray-100 transition-colors")
                {
                    Children =
                    {
                        new HTreeViewItemProp("Sub Folder 2.1", strStyle: "p-1 hover:bg-gray-50 rounded")
                    } // HTreeViewProp.Children
                } // HTreeViewProp
            }) // HTreeView
        }); // HStackPanel
    }

    #endregion

    #region ComboBox Dropdown Select Demo

    private static IElement ComboBoxDemo()
    {
        var countries = new MutSignal<IReadOnlyList<Country>>([
            new Country { Name = "China", Code = "CN" },
            new Country { Name = "United States", Code = "US" },
            new Country { Name = "Japan", Code = "JP" }
        ]);

        var selectedCountry = new MutSignal<Country?>(null);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("ComboBox Dropdown Selection List Demo", strStyle: SectionTitleStyle),
            HComboBox<Country>(new(
                countries,
                bindSelectedItem: selectedCountry,
                placeholderText: "Please select a country",
                strStyle: InputBaseStyle + "w-64"
            )
            {
                ItemTemplate = c => HTextBlock($"{c.Name} (Region Code: {c.Code})", strStyle: "p-2 hover:bg-matcha-50")
            }),
            HTextBlock(new(() => $"Selected Region: {selectedCountry.RxValue?.Name ?? "Nothing selected"}"),
                strStyle: "mt-2 fw-medium fg-matcha-600")
        });
    }

    #endregion

    #region Top Menu Bar Demo

    private static IElement MenuDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Menu Top Navigation Bar Demo", strStyle: SectionTitleStyle),
            HMenu(new(strStyle: "p-2 bg-gray-50 rounded-lg border border-gray-200 w-fit")
            {
                new(header: "File", strStyle: "px-3 py-1 rounded hover:bg-gray-200 transition-colors")
                {
                    new(header: "Open Document", onClick: () => Console.WriteLine("Open File Command Triggered"),
                        strStyle: "px-3 py-1 hover:bg-matcha-50"),
                    new(header: "Save Changes", onClick: () => Console.WriteLine("Save File Command Triggered"),
                        strStyle: "px-3 py-1 hover:bg-matcha-50"),
                    new(header: "-"),
                    new(header: "Exit Application", onClick: () => Environment.Exit(0),
                        strStyle: "px-3 py-1 hover:bg-rose-50 text-rose-600")
                },
                new(header: "Edit", strStyle: "px-3 py-1 rounded hover:bg-gray-200 transition-colors")
                {
                    new(header: "Copy Selection", strStyle: "px-3 py-1 hover:bg-matcha-50"),
                    new(header: "Paste Content", strStyle: "px-3 py-1 hover:bg-matcha-50")
                }
            }) // HMenu
        }); // HStackPanel
    }

    #endregion

    #region Show – Conditional Rendering Demo

    private static IElement ShowDemo()
    {
        var showFlag = new MutSignal<bool?>(true);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Show – Conditional Rendering", strStyle: SectionTitleStyle),
            HTextBlock("Toggle visibility of content using a boolean signal",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HToggleSwitch(new(bindIsChecked: showFlag) { HTextBlock(" Show extra content") }),
                Show(new(new(() => showFlag.RxValue is true))
                {
                    IfTrue = () => HTextBlock("✨ Extra content is now visible",
                        strStyle: "fg-matcha-600 italic")
                })
            })
        });
    }

    #endregion

    #region Switch – Integer Branch Demo

    private static IElement SwitchDemo()
    {
        var switchValue = new MutSignal<int>(0);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Switch – Branch by Integer", strStyle: SectionTitleStyle),
            HTextBlock("Render different content based on discrete integer values",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Value 0", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 0),
                HButton("Value 1", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 1),
                HButton("Value 2", strStyle: SecondaryBtnBase, onClick: _ => switchValue.RxValue = 2)
            }),
            HStackPanel(new(strStyle: "mt-2 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full")
            {
                Switch<int>(new(switchValue)
                {
                    Cases = new()
                    {
                        [0] = () => HTextBlock("Selected: 0 – default option", strStyle: "fg-matcha-700"),
                        [1] = () => HTextBlock("Selected: 1 – alternative option", strStyle: "fg-matcha-600"),
                        [2] = () => HTextBlock("Selected: 2 – third option", strStyle: "fg-matcha-500")
                    },
                    Default = () => HTextBlock("Unknown value", strStyle: "fg-coffee-400")
                })
            })
        });
    }

    #endregion

    #region Match – Predicate-based Conditional Demo

    private static IElement MatchDemo()
    {
        var matchValue = new MutSignal<double>(2);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Match – Predicate-based Branching", strStyle: SectionTitleStyle),
            HTextBlock("Use lambdas to match complex conditions (range, equality, etc.)",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HSlider(bindValue: matchValue, minimum: 0, maximum: 10, strStyle: "w-64"),
            HStackPanel(new(strStyle: "mt-2 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full")
            {
                Match<double>(new(matchValue)
                {
                    Cases = new()
                    {
                        [v => v == 0] = () => HTextBlock("⭐ Zero", strStyle: "fg-matcha-700"),
                        [v => v > 0 && v <= 5] = () => HTextBlock("🔵 Small (1–5)", strStyle: "fg-matcha-600"),
                        [v => v > 5] = () => HTextBlock("🟢 Large (6–10)", strStyle: "fg-matcha-500")
                    },
                    Default = () => HTextBlock("No match", strStyle: "fg-coffee-400")
                })
            })
        });
    }

    #endregion

    #region ForEach Dynamic Collection Demo

    private static IElement ForEachDemo()
    {
        var items = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("ForEach – Dynamic Collection Rendering", strStyle: SectionTitleStyle),
            HTextBlock("Items reactively added/removed; DOM updates only where changed",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HStackPanel(new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Add Item",
                    strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                    onClick: _ =>
                    {
                        var list = items.Value.ToList();
                        list.Add(list.Count + 1);
                        items.RxValue = list;
                    }
                ), // HButton
                HButton("Remove Last",
                    strStyle: PrimaryBtnBase + " bg-coffee-400 hover:bg-coffee-500 border-coffee-500",
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

            ForEach<int>(new(items)
            {
                ItemsPanel = HScrollViewer(new(
                    horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                    verticalScrollBarVisibility: ScrollBarVisibility.Visible,
                    strStyle: ScrollContainerBase + "min-h-60 max-h-75 w-full gap-3 vertical"
                )), // ForEach<int>.ItemsPanel
                ItemTemplate = (id, index) =>
                {
                    Console.WriteLine($"Rendering item index: {index.Value}");
                    return Counter(id);
                }
            }) // ForEach<int>.ItemTemplate
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

        var rootElement = uiScope.RootElement(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Loading – Asynchronous State Management", strStyle: SectionTitleStyle),
            HTextBlock("Debounced ID changes trigger async fetch; shows loading/error/success",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            // 控制 ID 的按钮
            HStackPanel(new(strStyle: HorizontalRowBase + " flex-wrap gap-2")
            {
                HButton("Increase ID",
                    strStyle: PrimaryBtnBase + " bg-matcha-400 hover:bg-matcha-500",
                    onClick: _ => userId.RxValue++
                ), // HButton
                HButton("Decrease ID",
                    strStyle: PrimaryBtnBase + " bg-coffee-400 hover:bg-coffee-500 border-coffee-500",
                    onClick: _ => userId.RxValue--
                ) // HButton
            }), // HStackPanel

            Loading<User>(new(userMemo)
            {
                Success = user => HChildren(
                    HTextBlock($"User ID: {user.Id}",
                        strStyle: "mt-2 text-lg fw-medium fg-matcha-800"),
                    HTextBlock($"Welcome, {user.Name}",
                        strStyle: "text-xl fw-semibold fg-matcha-600")
                ), // Loading<User>.Success
                Loading = _ =>
                    HStackPanel(new(strStyle: HorizontalRowBase + " p-3 bg-matcha-50 rounded-lg w-full justify-center")
                    {
                        HTextBlock("Fetching user data...",
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

    public static IElement LoadingDemo() => Element.WithScope(LoadingDemoComp);

    #endregion

    #region Component Lifecycle Demo

    private static IElement LifecycleDemoComp(UiScope uiScope)
    {
        var showExtra = new MutSignal<bool>(false);
        var logMessages = new MutSignal<IReadOnlyList<string>>(new List<string>());

        AddLog("Root component initialized");

        var root = uiScope.RootElement(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Component Lifecycle Tracking Demo", strStyle: SectionTitleStyle),

            HButton("Toggle Dynamic Child Visibility",
                strStyle: PrimaryBtnBase + "bg-amber-400 fg-white border-amber-500",
                onClick: _ => showExtra.RxValue = !showExtra.Value),

            Show(new(showExtra)
            {
                IfTrue = () => Element.WithScope(childScope =>
                {
                    AddLog("Dynamic child component initialized");

                    var childRootElement = childScope.RootElement(
                        new(strStyle: "p-3 bg-amber-50 border border-amber-200 rounded-lg")
                        {
                            HTextBlock(
                                "This child component is dynamically created and destroyed. Toggle the button above to trigger its construction and cleanup callback.")
                        }); // childRootElement

                    childScope.OnCleanup += () => AddLog("Dynamic child component disposed");

                    return childRootElement;
                }),
                IfFalse = () => HTextBlock("Dynamic child hidden (no instance alive)", strStyle: "fg-gray-400 italic")
            }),

            HTextBlock("Lifecycle Event Log (Latest 10 Entries)", strStyle: "text-md fw-semibold mt-2"),
            HScrollViewer(new(strStyle: ScrollContainerBase + "max-h-48")
            {
                ForEach<string>(new(logMessages)
                {
                    ItemsPanel = HStackPanel(new(strStyle: "gap-1 vertical")),
                    ItemTemplate = (msg, _) => HTextBlock(msg, strStyle: "text-xs font-mono fg-gray-700")
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

    private static IElement DemoSection(string title, IEnumerable<IElement> children)
    {
        var stackPanelProp = new HPanelProp(strStyle: "gap-2 vertical pl-2");
        foreach (var child in children) stackPanelProp.Add(child);

        return HStackPanel(new(strStyle: "gap-3 vertical")
        {
            HTextBlock(title, strStyle: SectionTitleStyle + "pl-4 py-1 fg-lime-700 border-l-4 border-lime-700"),
            HStackPanel(stackPanelProp)
        }); // HStackPanel
    }

    #region Sizing Demo (Width, Height, Min/Max)

    private static IElement SizingDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Sizing Utilities: Width & Height",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Width (w-) — Fixed & Percentage",
            [
                HStackPanel(new(strStyle: "horizontal gap-4 items-end")
                {
                    HStackPanel(new(strStyle: "vertical gap-2 items-center")
                    {
                        HTextBlock("w-16", strStyle: "w-16 h-8 bg-matcha-200 rounded text-center fw-medium text-sm"),
                        HTextBlock("w-32", strStyle: "w-32 h-8 bg-matcha-200 rounded text-center fw-medium text-sm"),
                        HTextBlock("w-64", strStyle: "w-64 h-8 bg-matcha-200 rounded text-center fw-medium text-sm")
                    }), // HStackPanel
                    HStackPanel(new(strStyle: "vertical gap-2 items-center")
                    {
                        HTextBlock("w-full", strStyle: "w-full h-8 bg-sky-200 rounded text-center fw-medium text-sm"),
                        // HTextBlock("w-1/2", strStyle: "w-[50%] h-8 bg-sky-200 rounded text-center fw-medium text-sm")
                    }) // HStackPanel
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Height (h-)",
            [
                HStackPanel(new(strStyle: "horizontal gap-4 items-start")
                {
                    HTextBlock("h-12", strStyle: "h-12 w-24 bg-matcha-300 rounded text-center fw-medium"),
                    HTextBlock("h-24", strStyle: "h-24 w-24 bg-matcha-300 rounded text-center fw-medium"),
                    HTextBlock("h-48", strStyle: "h-48 w-24 bg-matcha-300 rounded text-center fw-medium")
                })
            ]),

            DemoSection("Min/Max Constraints",
            [
                HTextBlock("min-w-48 & max-w-64",
                    strStyle: "min-w-48 max-w-64 h-8 bg-amber-100 rounded text-center px-2 fw-medium"),
                HTextBlock("min-h-16 & max-h-32",
                    strStyle: "min-h-16 max-h-32 w-32 bg-amber-100 rounded text-center p-2 fw-medium")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Padding Demo

    private static IElement PaddingDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Padding Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Uniform Padding (p-)",
            [
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("p-0", strStyle: "p-0 bg-matcha-100 rounded border"),
                    HTextBlock("p-2", strStyle: "p-2 bg-matcha-100 rounded border"),
                    HTextBlock("p-4", strStyle: "p-4 bg-matcha-100 rounded border"),
                    HTextBlock("p-8", strStyle: "p-8 bg-matcha-100 rounded border")
                })
            ]), // DemoSection

            DemoSection("Axis Padding (px-, py-)",
            [
                HTextBlock("px-4 py-2", strStyle: "px-4 py-2 bg-sky-100 rounded border"),
                HTextBlock("px-8 py-4", strStyle: "px-8 py-4 bg-sky-100 rounded border")
            ]), // DemoSection

            DemoSection("Directional Padding (pt-, pr-, pb-, pl-)",
            [
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("pl-4", strStyle: "p-2 pl-4 bg-rose-100 rounded border"),
                    HTextBlock("pr-4", strStyle: "p-2 pr-4 bg-rose-100 rounded border"),
                    HTextBlock("pt-4", strStyle: "p-2 pt-4 bg-rose-100 rounded border"),
                    HTextBlock("pb-4", strStyle: "p-2 pb-4 bg-rose-100 rounded border")
                }) // HStackPanel
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion


    #region Spacing Demo (Margin & Gap)

    private static IElement SpacingDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Spacing Utilities: Margin & Gap",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Margin (m-, mx-, my-, mt-, mb-, ml-, mr-)",
            [
                HTextBlock("Positive margins (multiples of 4px)",
                    strStyle: "text-sm fw-semibold fg-coffee-600 mb-2"),
                HWrapPanel(new(strStyle: "gap-3")
                {
                    // 4 items with different margin-left
                    HTextBlock("ml-0", strStyle: "ml-0 p-2 bg-matcha-100 rounded border fw-medium"),
                    HTextBlock("ml-2", strStyle: "ml-2 p-2 bg-matcha-100 rounded border fw-medium"),
                    HTextBlock("ml-4", strStyle: "ml-4 p-2 bg-matcha-100 rounded border fw-medium"),
                    HTextBlock("ml-8", strStyle: "ml-8 p-2 bg-matcha-100 rounded border fw-medium")
                }), // HWrapPanel
                HStackPanel(new(strStyle: "mt-4 gap-2")
                {
                    HTextBlock("mt-2", strStyle: "mt-2 p-2 bg-sky-100 rounded border fw-medium"),
                    HTextBlock("mt-4", strStyle: "mt-4 p-2 bg-sky-100 rounded border fw-medium")
                }), // HStackPanel
                HTextBlock("Negative margin example (pull element up):",
                    strStyle: "text-sm fg-coffee-600 mt-4 mb-2"),
                HStackPanel(new(strStyle: "horizontal gap-0")
                {
                    HTextBlock("mt-4", strStyle: "mt-4 p-2 bg-rose-100 rounded border fw-medium"),
                    HTextBlock("-mt-2", strStyle: "mt--2 p-2 bg-rose-200 rounded border fw-medium")
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Gap (row/column spacing)",
            [
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("Item 1", strStyle: "p-2 bg-matcha-100 rounded"),
                    HTextBlock("Item 2", strStyle: "p-2 bg-matcha-100 rounded"),
                    HTextBlock("Item 3", strStyle: "p-2 bg-matcha-100 rounded")
                }), // HStackPanel
                HTextBlock("Custom gap-x-8, gap-y-2", strStyle: "text-sm fg-coffee-600 mt-2"),
                HStackPanel(new(strStyle: "horizontal gap-x-8 gap-y-2 wrap")
                {
                    HTextBlock("A", strStyle: "p-2 bg-sky-100 rounded"),
                    HTextBlock("B", strStyle: "p-2 bg-sky-100 rounded"),
                    HTextBlock("C", strStyle: "p-2 bg-sky-100 rounded")
                }) // HStackPanel
            ]) // DemoSection
        });
    }

    #endregion

    #region Layout Demo

    private static IElement LayoutDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Layout Utilities: Flex, Alignment & Direction",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Direction (horizontal / vertical / flex-row / flex-col)",
            [
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("horizontal →", strStyle: "p-2 bg-matcha-100 rounded"),
                    HTextBlock("items", strStyle: "p-2 bg-matcha-100 rounded"),
                    HTextBlock("in a row", strStyle: "p-2 bg-matcha-100 rounded")
                }), // HStackPanel
                HStackPanel(new(strStyle: "vertical gap-2 mt-4")
                {
                    HTextBlock("vertical ↓", strStyle: "p-2 bg-sky-100 rounded"),
                    HTextBlock("stacked", strStyle: "p-2 bg-sky-100 rounded"),
                    HTextBlock("column", strStyle: "p-2 bg-sky-100 rounded")
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Shorthand: start / center / end / stretch",
            [
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("start", strStyle: "start w-16 h-16 bg-matcha-200 rounded text-center"),
                    HTextBlock("center", strStyle: "center w-16 h-16 bg-matcha-200 rounded text-center"),
                    HTextBlock("end", strStyle: "end w-16 h-16 bg-matcha-200 rounded text-center"),
                    HTextBlock("stretch", strStyle: "stretch w-16 h-16 bg-matcha-200 rounded text-center")
                }) // HStackPanel
            ]), // DemoSection
            DemoSection("Auto Margins for Centering",
            [
                HTextBlock("mx-auto · my-auto (horizontal & vertical centering)",
                    strStyle: "mx-auto my-auto w-48 p-2 bg-matcha-50 rounded border text-center")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region #region Complete Text Style Typography Demo

    private static IElement TextStyleDemo()
    {
        const string longText =
            "This line demonstrates text truncation behaviors when content exceeds the container width, triggering ellipsis or line clamping.";
        const string multiLine =
            "Line one of the clamped block.\nLine two adds more detail.\nLine three is the last visible.\nLine four is completely hidden.";
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Typography Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            // Font Size
            DemoSection("Font Size", new[]
            {
                HTextBlock("text-xs (12px)", strStyle: "text-xs"),
                HTextBlock("text-sm (14px)", strStyle: "text-sm"),
                HTextBlock("text-base (16px)", strStyle: "text-base"),
                HTextBlock("text-lg (18px)", strStyle: "text-lg"),
                HTextBlock("text-xl (20px)", strStyle: "text-xl"),
                HTextBlock("text-2xl (24px)", strStyle: "text-2xl"),
                HTextBlock("text-3xl (30px)", strStyle: "text-3xl"),
                HTextBlock("text-12 (48px)", strStyle: "text-12 fw-bold fg-matcha-700")
            }), // DemoSection
            // Font Weight
            DemoSection("Font Weight", new[]
            {
                HTextBlock("fw-thin", strStyle: "fw-thin text-lg"),
                HTextBlock("fw-extralight", strStyle: "fw-extralight text-lg"),
                HTextBlock("fw-light", strStyle: "fw-light text-lg"),
                HTextBlock("fw-normal", strStyle: "fw-normal text-lg"),
                HTextBlock("fw-medium", strStyle: "fw-medium text-lg"),
                HTextBlock("fw-semibold", strStyle: "fw-semibold text-lg"),
                HTextBlock("fw-bold", strStyle: "fw-bold text-lg"),
                HTextBlock("fw-extrabold", strStyle: "fw-extrabold text-lg"),
                HTextBlock("fw-black", strStyle: "fw-black text-lg")
            }), // DemoSection
            // Font Family
            DemoSection("Font Family", new[]
            {
                HTextBlock("font-jetmono (JetBrains Mono)", strStyle: "font-jetmono text-lg"),
                HTextBlock("font-lxgw (LXGW WenKai)", strStyle: "font-lxgw text-lg")
            }), // DemoSection
            // Font Style (Italic/Oblique)
            DemoSection("Font Style", new[]
            {
                HTextBlock("italic", strStyle: "italic text-lg"),
                HTextBlock("oblique", strStyle: "oblique text-lg"),
                HTextBlock("not-italic", strStyle: "not-italic text-lg")
            }), // DemoSection
            // Letter Spacing
            DemoSection("Letter Spacing (tracking-)", new[]
            {
                HTextBlock("tracking-tighter", strStyle: "tracking-tighter text-lg"),
                HTextBlock("tracking-tight", strStyle: "tracking-tight text-lg"),
                HTextBlock("tracking-normal", strStyle: "tracking-normal text-lg"),
                HTextBlock("tracking-wide", strStyle: "tracking-wide text-lg"),
                HTextBlock("tracking-wider", strStyle: "tracking-wider text-lg"),
                HTextBlock("tracking-widest", strStyle: "tracking-widest text-lg"),
                HTextBlock("tracking-4 (4px)", strStyle: "tracking-4 text-lg")
            }), // DemoSection
            // Line Height
            DemoSection("Line Height (leading-)", new[]
            {
                HTextBlock("leading-none (1.0)", strStyle: "leading-none p-2 bg-white rounded border"),
                HTextBlock("leading-tight (1.25)", strStyle: "leading-tight p-2 bg-white rounded border"),
                HTextBlock("leading-snug (1.375)", strStyle: "leading-snug p-2 bg-white rounded border"),
                HTextBlock("leading-normal (1.5)", strStyle: "leading-normal p-2 bg-white rounded border"),
                HTextBlock("leading-relaxed (1.625)",
                    strStyle: "leading-relaxed w-full p-2 bg-white rounded border"),
                HTextBlock("leading-loose (2.0)", strStyle: "leading-loose w-full p-2 bg-white rounded border"),
                HTextBlock("leading-8 (32px)", strStyle: "leading-8 text-lg w-full p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Text Alignment (Horizontal)
            DemoSection("Text Horizontal Alignment", new[]
            {
                HTextBlock("text-left", strStyle: "text-left w-32 h-20 p-2 bg-matcha-100 rounded border"),
                HTextBlock("text-x-center", strStyle: "text-x-center h-20 w-32 p-2 bg-matcha-100 rounded border"),
                HTextBlock("text-right", strStyle: "text-right h-20 w-32 p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Vertical Text Alignment
            DemoSection("Text Vertical Alignment", new[]
            {
                HStackPanel(new(strStyle: "horizontal gap-4 items-start")
                {
                    HTextBlock("text-top", strStyle: "text-top w-32 h-20 p-2 bg-matcha-100 rounded border"),
                    HTextBlock("text-y-center", strStyle: "text-y-center w-32 h-20 p-2 bg-matcha-100 rounded border"),
                    HTextBlock("text-bottom", strStyle: "text-bottom w-32 h-20 p-2 bg-matcha-100 rounded border")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Text Center Alignment", new[]
            {
                HTextBlock("text-center", strStyle: "text-center w-32 h-20 p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Wrapping
            DemoSection("Text Wrapping", new[]
            {
                HTextBlock("wrap: " + longText, strStyle: "wrap w-80 p-2 bg-white rounded border"),
                HTextBlock("wrap-overflow: " + longText,
                    strStyle: "wrap-overflow w-80 p-2 bg-white rounded border"),
                HTextBlock("whitespace-nowrap + truncate: " + longText,
                    strStyle: "whitespace-nowrap truncate w-80 p-2 bg-white rounded border")
            }), // DemoSection
            // Truncation & Clipping
            DemoSection("Text Trimming", new[]
            {
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-none — No trimming, overflows horizontally",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-none whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-end / truncate — Ellipsis at the end",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-end whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-char — Character‑by‑character ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-char whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-start — Ellipsis at the start (path‑style)",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-start whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-prefix — Prefix ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-prefix whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(new(strStyle: "gap-1 vertical")
                {
                    HTextBlock("text-clip-path — Path‑segment ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HTextBlock(longText,
                        strStyle: "w-80 text-clip-path whitespace-nowrap p-2 bg-gray-100 rounded border")
                }) // HStackPanel
            }), // DemoSection
            // Line Clamp
            DemoSection("Line Clamp (line-clamp-)", new[]
            {
                HTextBlock("line-clamp-2: " + multiLine,
                    strStyle: "line-clamp-2 w-full p-2 bg-matcha-50 rounded border"),
                HTextBlock("line-clamp-3: " + multiLine,
                    strStyle: "line-clamp-3 w-full p-2 bg-matcha-50 rounded border")
            }), // DemoSection
            // Text Decoration
            DemoSection("Text Decoration", new[]
            {
                HTextBlock("underline", strStyle: "underline text-lg"),
                HTextBlock("overline", strStyle: "overline text-lg"),
                HTextBlock("baseline", strStyle: "baseline text-lg"),
                HTextBlock("line-through", strStyle: "line-through text-lg"),
                HTextBlock("decoration-none", strStyle: "decoration-none text-lg"),
                HTextBlock("dashed underline",
                    strStyle: "underline decoration-dashed decoration-matcha-500 text-lg"),
                HTextBlock("dotted underline",
                    strStyle: "underline decoration-dotted decoration-matcha-500 text-lg"),
                HTextBlock("decoration-w-4", strStyle: "underline decoration-w-4 decoration-matcha-500 text-lg"),
                HTextBlock("decoration-rose-500", strStyle: "underline decoration-rose-500 text-lg")
            }), // DemoSection

            DemoSection("Combined Composite Typography",
            [
                HTextBlock("NeHive UI Combined Typography Sample",
                    strStyle: "text-3xl tracking-2 fw-black italic fg-blue-300"),
                HTextBlock("JetBrains Mono Programming Code Typography",
                    strStyle: "text-lg tracking-1 fw-semibold font-jetmono fg-emerald-600"),
                HTextBlock("LXGW Custom Chinese Font Combined Style Demo Text",
                    strStyle: "text-xl tracking-1 leading-10 fw-medium font-lxgw fg-rose-600")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Color & Gradient Demo

    private static IElement ColorDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Color & Gradient Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            // ── Foreground Colors ──
            DemoSection("Foreground Colors (fg-)", new[]
            {
                HTextBlock("fg-matcha-600", strStyle: "fg-matcha-600 text-lg fw-medium"),
                HTextBlock("fg-sky-500", strStyle: "fg-sky-500 text-lg fw-medium"),
                HTextBlock("fg-rose-500", strStyle: "fg-rose-500 text-lg fw-medium"),
                HTextBlock("fg-emerald-500", strStyle: "fg-emerald-500 text-lg fw-medium"),
                HTextBlock("fg-[#8a2be2] (arbitrary hex)", strStyle: "fg-[#8a2be2] text-lg fw-medium")
            }), // DemoSection
            // ── Foreground Opacity Modifier ──
            DemoSection("Foreground Opacity (fg-<color>/<opacity>)", new[]
            {
                HTextBlock("fg-sky-500/100 — fully opaque",
                    strStyle: "fg-sky-500/100 text-lg fw-medium"),
                HTextBlock("fg-sky-500/75 — 75% opacity",
                    strStyle: "fg-sky-500/75 text-lg fw-medium"),
                HTextBlock("fg-sky-500/50 — 50% opacity",
                    strStyle: "fg-sky-500/50 text-lg fw-medium"),
                HTextBlock("fg-sky-500/25 — 25% opacity",
                    strStyle: "fg-sky-500/25 text-lg fw-medium"),
                HTextBlock("fg-rose-500/40 — rose at 40%",
                    strStyle: "fg-rose-500/40 text-lg fw-medium")
            }), // DemoSection
            // ── Background Colors ──
            DemoSection("Background Colors (bg-)", new[]
            {
                HStackPanel(new(strStyle: "horizontal gap-3 flex-wrap")
                {
                    HTextBlock("bg-matcha-500",
                        strStyle: "bg-matcha-500 fg-white p-2 rounded text-lg"),
                    HTextBlock("bg-sky-500",
                        strStyle: "bg-sky-500 fg-white p-2 rounded text-lg"),
                    HTextBlock("bg-rose-500",
                        strStyle: "bg-rose-500 fg-white p-2 rounded text-lg")
                }) // HStackPanel
            }), // DemoSection
            // ── Background Opacity Modifier ──
            DemoSection("Background Opacity (bg-<color>/<opacity>)", new[]
            {
                HWrapPanel(new(strStyle: "w-full horizontal gap-3")
                {
                    HTextBlock("bg-sky-300/100",
                        strStyle: "bg-sky-300/100 p-2 rounded text-lg fw-medium"),
                    HTextBlock("bg-sky-300/75",
                        strStyle: "bg-sky-300/75 p-2 rounded text-lg fw-medium"),
                    HTextBlock("bg-sky-300/50",
                        strStyle: "bg-sky-300/50 p-2 rounded text-lg fw-medium"),
                    HTextBlock("bg-sky-300/25",
                        strStyle: "bg-sky-300/25 p-2 rounded text-lg fw-medium"),
                    HTextBlock("bg-sky-300/10",
                        strStyle: "bg-sky-300/10 p-2 rounded text-lg fw-medium")
                }), // HStackPanel
                HTextBlock("Tip: /N sets the alpha channel as N% (0–100). Works with all named colors.",
                    strStyle: "text-xs fg-coffee-500 mt-2 italic")
            }), // DemoSection
            // ── Foreground Gradients ──
            DemoSection("Foreground Gradients (fg-gradient-*)", new[]
            {
                HTextBlock("fg-gradient-r from-matcha-400 to-matcha-800",
                    strStyle: "fg-gradient-r fg-from-matcha-400 fg-to-matcha-800 text-sm fw-black"),
                HTextBlock("fg-gradient-t from-sky-400 to-sky-800",
                    strStyle: "fg-gradient-t fg-from-sky-400 fg-to-sky-800 text-sm fw-black"),
                HTextBlock("fg-gradient-tl from-rose-400 to-rose-800",
                    strStyle: "fg-gradient-tl fg-from-rose-400 fg-to-rose-800 text-sm fw-black"),
                HTextBlock("fg-gradient-tr from-amber-400 to-amber-700/60",
                    strStyle: "fg-gradient-tr fg-from-amber-400 fg-to-amber-700/60 text-sm fw-black")
            }), // DemoSection
            // ── Background Gradients ──
            DemoSection("Background Gradients (bg-gradient-*)", new[]
            {
                HTextBlock("bg-gradient-r from-amber-300 to-amber-600",
                    strStyle:
                    "bg-gradient-r bg-from-amber-300 bg-to-amber-600 fg-white text-sm fw-black p-4 rounded"),
                HTextBlock("bg-gradient-tr from-purple-300 to-purple-600",
                    strStyle:
                    "bg-gradient-tr bg-from-purple-300 bg-to-purple-600 fg-white text-sm fw-black p-4 rounded"),
                HTextBlock("bg-gradient-t from-sky-200/80 to-sky-600/40",
                    strStyle: "bg-gradient-t bg-from-sky-200/80 bg-to-sky-600/40 text-sm fw-black p-4 rounded")
            }), // DemoSection
            // ── Border Colors ──
            DemoSection("Border Colors (border-<color>)", new[]
            {
                HTextBlock("border-matcha-500",
                    strStyle: "border-w-2 border-matcha-500 p-3 rounded text-xs"),
                HTextBlock("border-sky-400/60",
                    strStyle: "border-w-2 border-sky-400/60 p-3 rounded text-xs"),
                HTextBlock("border-rose-500/30",
                    strStyle: "border-w-4 border-rose-500/30 p-3 rounded text-xs")
            }), // DemoSection
            // ── Border Gradients ──
            DemoSection("Border Gradients (border-gradient-*)", new[]
            {
                HTextBlock("border-gradient-r from-matcha-400 to-matcha-800",
                    strStyle:
                    "border-gradient-r border-from-matcha-400 border-to-matcha-800 border-w-4 p-4 rounded text-xs fw-bold")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Border Demo

    private static IElement BorderDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Border Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Border Width (border, border-w-*)", new[]
            {
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("border", strStyle: "border p-2 rounded"),
                    HTextBlock("border-w-2", strStyle: "p-2 border-w-2 border-matcha-500 rounded"),
                    HTextBlock("border-w-4", strStyle: "p-2 border-w-4 border-matcha-500 rounded")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Directional Border Widths", new[]
            {
                HTextBlock("border-t-2 border-r-4 border-b-2 border-l-4",
                    strStyle: "p-4 border-t-2 border-r-4 border-b-2 border-l-4 border-matcha-500 rounded text-center")
            }), // DemoSection
            DemoSection("Corner Radius (rounded-*)", new[]
            {
                HWrapPanel(new(strStyle: "w-full horizontal gap-4")
                {
                    HTextBlock("rounded-none", strStyle: "p-2 rounded-none bg-matcha-100 border"),
                    HTextBlock("rounded-sm", strStyle: "p-2 rounded-sm bg-matcha-100 border"),
                    HTextBlock("rounded", strStyle: "p-2 rounded bg-matcha-100 border"),
                    HTextBlock("rounded-md", strStyle: "p-2 rounded-md bg-matcha-100 border"),
                    HTextBlock("rounded-lg", strStyle: "p-2 rounded-lg bg-matcha-100 border"),
                    HTextBlock("rounded-xl", strStyle: "p-2 rounded-xl bg-matcha-100 border"),
                    HTextBlock("rounded-2xl", strStyle: "p-2 rounded-2xl bg-matcha-100 border"),
                    HTextBlock("rounded-full", strStyle: "w-24 h-24 p-2 rounded-full text-center bg-matcha-100 border")
                }) // HStackPanel
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Effects Demo

    private static IElement EffectsDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Visual Effects",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Opacity (opacity-)", new[]
            {
                HStackPanel(new(strStyle: "horizontal gap-4")
                {
                    HTextBlock("opacity-100", strStyle: "opacity-100 text-xl fw-bold fg-matcha-800"),
                    HTextBlock("opacity-70", strStyle: "opacity-70 text-xl fw-bold fg-matcha-800"),
                    HTextBlock("opacity-40", strStyle: "opacity-40 text-xl fw-bold fg-matcha-800"),
                    HTextBlock("opacity-10", strStyle: "opacity-10 text-xl fw-bold fg-matcha-800")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Visibility", new[]
            {
                // Hard to show hidden, but we can show a container with visible/hidden effect via code.
                HTextBlock("visible / hidden (controlled by code)", strStyle: "text-sm fg-coffee-500")
            }), // DemoSection
            DemoSection("Blur", new[]
            {
                HTextBlock("blur-none", strStyle: "blur-none text-xl fw-bold fg-matcha-800"),
                HTextBlock("blur-sm", strStyle: "blur-sm text-xl fw-bold fg-matcha-800"),
                HTextBlock("blur", strStyle: "blur text-xl fw-bold fg-matcha-800"),
                HTextBlock("blur-lg", strStyle: "blur-lg text-xl fw-bold fg-matcha-800"),
                HTextBlock("blur-xl", strStyle: "blur-xl text-xl fw-bold fg-matcha-800")
            }), // DemoSection
            DemoSection("Box Shadow", new[]
            {
                HTextBlock("shadow-sm", strStyle: "shadow-sm text-xl fw-bold p-4 bg-white rounded"),
                HTextBlock("shadow", strStyle: "shadow text-xl fw-bold p-4 bg-white rounded"),
                HTextBlock("shadow-md", strStyle: "shadow-md text-xl fw-bold p-4 bg-white rounded"),
                HTextBlock("shadow-lg", strStyle: "shadow-lg text-xl fw-bold p-4 bg-white rounded"),
                HTextBlock("shadow-xl", strStyle: "shadow-xl text-xl fw-bold p-4 bg-white rounded")
            }), // DemoSection
            DemoSection("Ring (Focus Ring)", new[]
            {
                HTextBlock("ring-2 ring-matcha-500 ring-offset-2",
                    strStyle:
                    "ring-w-2 ring-matcha-500 ring-offset-2 p-4 rounded bg-white border border-matcha-200")
            }), // DemoSection
            DemoSection("Z-Index", new[]
            {
                HTextBlock("z-10 (layering)", strStyle: "z-10 p-4 bg-matcha-100 rounded border")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Cursor Demo

    private static IElement CursorDemo()
    {
        var cursors = new (string style, string label)[]
        {
            ("cursor-default", "Default"),
            ("cursor-text", "Text"),
            ("cursor-wait", "Wait"),
            ("cursor-crosshair", "Crosshair"),
            ("cursor-up-arrow", "UpArrow"),
            ("cursor-ew-resize", "EW Resize"),
            ("cursor-ns-resize", "NS Resize"),
            ("cursor-move", "Move"),
            ("cursor-not-allowed", "Not Allowed"),
            ("cursor-pointer", "Pointer"),
            ("cursor-progress", "Progress"),
            ("cursor-help", "Help"),
            ("cursor-n-resize", "N Resize"),
            ("cursor-s-resize", "S Resize"),
            ("cursor-w-resize", "W Resize"),
            ("cursor-e-resize", "E Resize"),
            ("cursor-nw-resize", "NW Resize"),
            ("cursor-ne-resize", "NE Resize"),
            ("cursor-sw-resize", "SW Resize"),
            ("cursor-se-resize", "SE Resize"),
            ("cursor-drag-move", "Drag Move"),
            ("cursor-drag-copy", "Drag Copy"),
            ("cursor-drag-link", "Drag Link"),
            ("cursor-none", "None")
        };
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Cursor Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            ForEach<(string style, string label)>(new(cursors)
            {
                ItemsPanel = HWrapPanel(new(strStyle: "gap-3")),
                ItemTemplate = (t, _) => HTextBlock($"  {t.label}  ",
                    strStyle: $"{t.style} p-2 bg-gray-50 border rounded text-sm fw-medium")
            }) // ForEach<(string style, string label)>
        }); // HStackPanel
    }

    #endregion

    #region Transition Demo

    private static IElement TransitionDemo()
    {
        // Create interactive buttons to show transition effects
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Transition & Animation Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Transition Property Scope", new[]
            {
                // HButton("transition-none (no animation)",
                //     strStyle: PrimaryBtnBase + "transition-none"),
                // HButton("transition-all (all properties)",
                //     strStyle: PrimaryBtnBase + "transition-all hover:bg-matcha-600 hover:scale-105"),
                HButton("transition-colors",
                    strStyle: PrimaryBtnBase + "transition-colors hover:bg-matcha-600"),
                HButton("transition-transform",
                    strStyle: PrimaryBtnBase + "transition-transform hover:scale-110"),
                HButton("transition-opacity",
                    strStyle: PrimaryBtnBase + "transition-opacity hover:opacity-70"),
                HButton("transition-shadow",
                    strStyle: PrimaryBtnBase + "transition-shadow hover:shadow-lg"),
            }), // DemoSection
            DemoSection("Duration (duration-)", new[]
            {
                HButton("duration-100 (100ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-100 hover:scale-110"),
                HButton("duration-300 (300ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-300 hover:scale-110"),
                HButton("duration-700 (700ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-700 hover:scale-110"),
            }), // DemoSection
            DemoSection("Easing Functions", new[]
            {
                HButton("linear",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 linear hover:translate-x-4"),
                HButton("ease",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease hover:translate-x-4"),
                HButton("ease-in",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-in hover:translate-x-4"),
                HButton("ease-out",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-out hover:translate-x-4"),
                HButton("ease-in-out",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-in-out hover:translate-x-4"),
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Transform Demo

    private static IElement TransformDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HTextBlock("Geometric Transforms",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Translate", new[]
            {
                HTextBlock("translate-x-4", strStyle: "translate-x-4 p-4 bg-matcha-100 rounded"),
                HTextBlock("translate-y-4", strStyle: "translate-y-4 p-4 bg-matcha-100 rounded"),
                HTextBlock("translate-4", strStyle: "translate-4 p-4 bg-matcha-100 rounded")
            }), // DemoSection
            DemoSection("Scale", new[]
            {
                HTextBlock("scale-50", strStyle: "scale-50 p-4 bg-rose-100 rounded"),
                HTextBlock("scale-100", strStyle: "scale-100 p-4 bg-rose-100 rounded"),
                HTextBlock("scale-150", strStyle: "scale-150 p-4 bg-rose-100 rounded"),
                HTextBlock("scale-x-75", strStyle: "scale-x-75 p-4 bg-rose-100 rounded")
            }),
            DemoSection("Rotate", new[]
            {
                HTextBlock("rotate-45", strStyle: "rotate-45 p-4 bg-sky-100 rounded"),
                HTextBlock("-rotate-30", strStyle: "-rotate-30 p-4 bg-sky-100 rounded"),
                HTextBlock("rotate-180", strStyle: "rotate-180 p-4 bg-sky-100 rounded")
            }), // DemoSection
            DemoSection("Skew", new[]
            {
                HTextBlock("skew-10", strStyle: "skew-10 p-4 bg-amber-100 rounded"),
                HTextBlock("skew-x-15", strStyle: "skew-x-15 p-4 bg-amber-100 rounded"),
                HTextBlock("-skew-y-10", strStyle: "-skew-y-10 p-4 bg-amber-100 rounded")
            }), // DemoSection
            DemoSection("Combined Transforms", new[]
            {
                HTextBlock("Hover: scale-110 rotate-3 ease-out",
                    strStyle:
                    "transition-transform duration-300 ease-out hover:scale-110 hover:rotate-3 p-4 bg-matcha-50 rounded-xl cursor-pointer")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Group Unified Hover State Container Demo

    private static IElement GroupDemo()
    {
        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Group Unified Hover State Parent Container Demo", strStyle: SectionTitleStyle),
            HGroup(new(strStyle: "mx-auto my-auto p-6 bg-gray-50 border border-gray-200 rounded-xl")
            {
                Child = state => HStackPanel(new(
                    strStyle: "gap-8 horizontal justify-center items-center")
                {
                    HSvgImage("~/Assets/play.svg",
                        strStyle: new(() =>
                            $"w-16 h-16 fg-blue-{ToValue(state.IsHover.RxValue)} transition-colors duration-300")),
                    HSvgImage("~/Assets/skip-back.svg",
                        strStyle: new(() =>
                            $"w-16 h-16 fg-orange-{ToValue(state.IsHover.RxValue)} transition-colors duration-300")),
                    HSvgImage("~/Assets/skip-forward.svg",
                        strStyle: new(() =>
                            $"w-16 h-16 fg-yellow-{ToValue(state.IsHover.RxValue)} transition-colors duration-300")),
                    HSvgImage("~/Assets/stretch-vertical.svg",
                        strStyle: new(() =>
                            $"w-16 h-16 fg-red-{ToValue(state.IsHover.RxValue)} transition-colors duration-300"))
                })
            }) // HGroup
        }); // HStackPanel

        string ToValue(bool isHover) => isHover ? "400" : "200";
    }

    #endregion

    #region Context Theme Injection Demo

    private static readonly ContextKey<Signal<string>> Theme = new();
    private static readonly ContextKey<Action> ToggleTheme = new();

    private static IElement ContextDemo()
    {
        var theme = new MutSignal<string>("light");
        return HStackPanel(new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HTextBlock("Context Scope Theme Dependency Injection Demo", strStyle: SectionTitleStyle),
            HContext(ctx => ctx
                    .SetContext(Theme, theme)
                    .SetContext(ToggleTheme, Toggle),
                () => HStackPanel(new(strStyle: "gap-6 vertical w-full")
                {
                    UseContextDemo1(),
                    UseContextDemo2()
                }) // HStackPanel
            ) // HContext
        });

        void Toggle() => theme.RxValue = theme.Value is "dark" ? "light" : "dark";
    }

    private static IElement UseContextDemo1()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null || toggleTheme is null) throw new ArgumentNullException();
            return uiScope.RootElement(new()
            {
                HContentButton(new(strStyle: new(() =>
                        $"""
                         px-4 py-2 horizontal rounded-lg
                         {(theme.RxValue is "dark" ? "fg-matcha-100 bg-matcha-900 border-matcha-700" : "fg-coffee-700 bg-coffee-50 border-matcha-200")} 
                         transition-colors duration-300 border-w-1 focus:ring-w-2 focus:ring-matcha-300
                         """),
                    onClick: _ => toggleTheme())
                {
                    Content = HStackPanel(new()
                    {
                        Switch<string>(new(theme)
                        {
                            Cases = new()
                            {
                                ["dark"] = () => HSvgImage("~/Assets/sun.svg",
                                    strStyle: new(() =>
                                        $"w-4 h-4 fw-extralight fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}"))
                            }, // Switch<string>.Cases
                            Default = () => HSvgImage("~/Assets/moon.svg",
                                strStyle:
                                new(() =>
                                    $"w-4 h-4 fw-extralight fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}"))
                        }), // Switch<string>
                        HTextBlock(new(() => $"Switch To {(theme.RxValue is "dark" ? "Light" : "Dark")} Theme Mode"),
                            strStyle: new(() => $"ml-2 fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}"))
                    }) // HContentButton.Content
                }) // HContentButton
            }); // rootElement
        });
    }

    private static IElement UseContextDemo2()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null || toggleTheme is null) throw new ArgumentNullException();

            return uiScope.RootElement(new(strStyle: new(() =>
                $"""
                 mt-2 mx-auto max-w-md overflow-hidden rounded-xl shadow-md border-w-1
                 {(theme.RxValue is "dark" ? "bg-matcha-900 border-matcha-700" : "bg-coffee-50 border-matcha-200")} 
                 transition-colors duration-300
                 """))
            {
                HStackPanel(new(strStyle: "p-6 vertical gap-3")
                {
                    HTextBlock("Theme Injection Demonstration",
                        strStyle: new(() =>
                            $"""text-2xl fw-bold fg-{(theme.RxValue is "dark" ? "matcha-100" : "matcha-800")}""")),
                    HTextBlock(new(() => $"Current Topic Mode: {theme.RxValue}"),
                        strStyle: new(() => $"fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}")),
                    HTextBlock(
                        "The background, text and border color of this card component will automatically change according to the theme.",
                        strStyle: new(() => $"fg-{(theme.RxValue is "dark" ? "matcha-300" : "coffee-700")}")),
                }) // HStackPanel
            }); // rootElement
        });
    }

    #endregion

    #region Main Category Navigation Layout

    public static IElement MainNavDemo()
    {
        var categories = new List<DemoCategory>
        {
            new("📐 Layout Panels", DemoView.GridDemo, DemoView.AbsoluteDemo, DemoView.SplitViewDemo,
                DemoView.SplitPanelDemo, DemoView.DockPanelDemo, DemoView.WrapPanelDemo,
                DemoView.UniformGridDemo, DemoView.GridSplitterDemo, DemoView.ScrollDemo),

            new("🔘 Basic Input Controls", DemoView.TextBoxDemo, DemoView.CheckBoxDemo, DemoView.RadioButtonDemo,
                DemoView.ToggleSwitchDemo, DemoView.FilePickerDemo, DemoView.ProgressBarDemo,
                DemoView.SliderDemo, DemoView.FlyoutDemo, DemoView.WindowDemo),

            new("📋 Data Selection & Lists", DemoView.TreeViewDemo, DemoView.ComboBoxDemo,
                DemoView.MenuDemo),

            new("⚙️ Reactive Control Flow", DemoView.ShowDemo, DemoView.SwitchDemo,
                DemoView.MatchDemo, DemoView.ForEachDemo, DemoView.LoadingDemo),

            new("🔄 Scope & Component Lifecycle", DemoView.LifecycleDemo),

            new("🎨 Styling & Visuals",
                DemoView.SpacingDemo,
                DemoView.SizingDemo,
                DemoView.PaddingDemo,
                DemoView.LayoutDemo,
                DemoView.TextStyleDemo,
                DemoView.ColorDemo,
                DemoView.BorderDemo,
                DemoView.EffectsDemo,
                DemoView.CursorDemo,
                DemoView.TransitionDemo,
                DemoView.TransformDemo,
                DemoView.GroupDemo,
                DemoView.ContextDemo
            ),

            new("🚀 Advanced Integrated Sample", DemoView.MusicPlayerDemo)
        };

        var categoriesSignal = new MutSignal<IReadOnlyList<DemoCategory>>(categories);
        var selectedCategory = new MutSignal<DemoCategory>(categories[0]);
        var currentView = new MutSignal<DemoView>(categories[0].Demos.First());

        void SelectDemo(DemoView view) => currentView.RxValue = view;

        return HGrid(new(
            columnDefinitions: new([224, HgLen.Star()]),
            strStyle: "w-full h-full gap-4")
        {
            // Left Category Sidebar – Manual implementation without HListBox
            [(0, 0)] =
                ForEach<DemoCategory>(new(categoriesSignal)
                {
                    ItemsPanel =
                        HScrollViewer(new(
                            strStyle:
                            "h-full vertical gap-1 px-3 py-2 bg-white border border-matcha-200 rounded-2xl shadow-sm")),
                    ItemTemplate = (cat, _) =>
                        HButton(cat.Name,
                            strStyle: new(() => $"""
                                                 w-full px-3 py-2 text-left fw-medium {(selectedCategory.RxValue == cat
                                                     ? "fg-white bg-matcha-400"
                                                     : "fg-matcha-800 bg-matcha-50 hover:bg-matcha-100")}
                                                     rounded-lg transition-colors cursor-pointer
                                                 """),
                            onClick: _ => selectedCategory.RxValue = cat
                        ) // HButton
                    // ForEach<DemoCategory>.ItemTemplate
                }), // ForEach<DemoCategory>
            // [(0, 0)]

            // Right Main Content Area (unchanged)
            [(0, 1)] = HGrid(new(
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]),
                strStyle: "gap-4 h-full")
            {
                // Top Demo Button Grid
                [(0, 0)] =
                    ForEach<DemoView>(new(new(() => selectedCategory.RxValue.Demos))
                    {
                        ItemsPanel = HUniformGrid(new(
                            columns: 4,
                            strStyle: "gap-2 p-3 bg-white rounded-xl border border-matcha-200 shadow-sm")),
                        ItemTemplate = (view, _) =>
                            HButton(view.ToString(),
                                strStyle: new(() => $"""
                                                     px-3 py-2 text-sm fw-medium rounded-lg transition-all duration-200
                                                     {(currentView.RxValue == view
                                                         ? "bg-matcha-500 fg-white shadow-md"
                                                         : "bg-matcha-50 fg-matcha-700 hover:bg-matcha-100 shadow-none")}
                                                     """),
                                onClick: _ => SelectDemo(view)
                            ) // HButton
                        // ForEach<DemoView>.ItemTemplate
                    }), // ForEach<DemoView>
                // [(0, 0)]

                // Bottom Demo Render Viewport (unchanged)
                [(1, 0)] =
                    HScrollViewer(new(strStyle: "w-full max-h-125")
                    {
                        Switch<DemoView>(new(currentView)
                        {
                            Cases = new()
                            {
                                [DemoView.SimpleCounter] = () => Counter(888),

                                [DemoView.GridDemo] = GridDemo,
                                [DemoView.AbsoluteDemo] = AbsoluteDemo,
                                [DemoView.SplitViewDemo] = SplitViewDemo,
                                [DemoView.SplitPanelDemo] = SplitPanelDemo,
                                [DemoView.UniformGridDemo] = UniformGridDemo,
                                [DemoView.DockPanelDemo] = DockPanelDemo,
                                [DemoView.WrapPanelDemo] = WrapPanelDemo,
                                [DemoView.GridSplitterDemo] = GridSplitterDemo,
                                [DemoView.ScrollDemo] = ScrollDemo,

                                [DemoView.TextBoxDemo] = TextBoxDemo,
                                [DemoView.CheckBoxDemo] = CheckBoxDemo,
                                [DemoView.RadioButtonDemo] = RadioButtonDemo,
                                [DemoView.ToggleSwitchDemo] = ToggleSwitchDemo,
                                [DemoView.FilePickerDemo] = FilePickerDemo,
                                [DemoView.ProgressBarDemo] = ProgressBarDemo,
                                [DemoView.SliderDemo] = SliderDemo,
                                [DemoView.FlyoutDemo] = FlyoutDemo,
                                [DemoView.WindowDemo] = WindowDemo,

                                [DemoView.TreeViewDemo] = TreeViewDemo,
                                [DemoView.ComboBoxDemo] = ComboBoxDemo,
                                [DemoView.MenuDemo] = MenuDemo,

                                [DemoView.ShowDemo] = ShowDemo,
                                [DemoView.SwitchDemo] = SwitchDemo,
                                [DemoView.MatchDemo] = MatchDemo,
                                [DemoView.ForEachDemo] = ForEachDemo,
                                [DemoView.LoadingDemo] = LoadingDemo,

                                [DemoView.LifecycleDemo] = LifecycleDemo,

                                [DemoView.SpacingDemo] = SpacingDemo,
                                [DemoView.SizingDemo] = SizingDemo,
                                [DemoView.PaddingDemo] = PaddingDemo,
                                [DemoView.LayoutDemo] = LayoutDemo,
                                [DemoView.TextStyleDemo] = TextStyleDemo,
                                [DemoView.ColorDemo] = ColorDemo,
                                [DemoView.BorderDemo] = BorderDemo,
                                [DemoView.EffectsDemo] = EffectsDemo,
                                [DemoView.CursorDemo] = CursorDemo,
                                [DemoView.TransitionDemo] = TransitionDemo,
                                [DemoView.TransformDemo] = TransformDemo,
                                [DemoView.GroupDemo] = GroupDemo,
                                [DemoView.ContextDemo] = ContextDemo,

                                [DemoView.MusicPlayerDemo] = MusicPlayerDemo.MusicPlayer
                            }, // Switch<DemoView>.Cases
                            Default = () => HTextBlock("Select a demo item from left sidebar to preview",
                                strStyle: "fg-gray-400 text-center p-16 text-lg")
                        }) // Switch<DemoView>
                    }) // HScrollViewer
                // [(1, 0)]
            }) // // HGird
        }); // HGird
    }

    #endregion

    // Helper Record Type
    private record DemoCategory(string Name, params DemoView[] Demos);
}

// Data Model Records
public record User(int? Id = null, string? Name = null);

public record Country(string? Name = null, string? Code = null);

// Demo Page Enumeration
public enum DemoView
{
    SimpleCounter,

    GridDemo,
    AbsoluteDemo,
    SplitViewDemo,
    SplitPanelDemo,
    UniformGridDemo,
    DockPanelDemo,
    WrapPanelDemo,
    GridSplitterDemo,
    ScrollDemo,

    TextBoxDemo,
    CheckBoxDemo,
    RadioButtonDemo,
    ToggleSwitchDemo,
    FilePickerDemo,
    ProgressBarDemo,
    SliderDemo,
    FlyoutDemo,
    WindowDemo,

    TreeViewDemo,
    ComboBoxDemo,
    MenuDemo,

    ShowDemo,
    SwitchDemo,
    MatchDemo,
    ForEachDemo,
    LoadingDemo,

    LifecycleDemo,

    SpacingDemo,
    SizingDemo,
    PaddingDemo,
    LayoutDemo,
    TextStyleDemo,
    ColorDemo,
    BorderDemo,
    EffectsDemo,
    CursorDemo,
    TransitionDemo,
    TransformDemo,

    GroupDemo,
    ContextDemo,

    MusicPlayerDemo,
    Unknown
}