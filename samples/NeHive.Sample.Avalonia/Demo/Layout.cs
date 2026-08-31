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
    #region Grid Layout Demo

    private static IElement GridDemo()
    {
        var gapX = new MutSignal<int>(3);

        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("Grid Responsive Layout", strStyle: DemoTitle),
            HSelectableText("Define rows/columns with auto and star sizing", strStyle: DemoDesc),
            HGrid(_ => new(
                showGridLines: true,
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]),
                columnDefinitions: new([120, HgLen.Star()]),
                strStyle: new(() => $"""
                                         {DemoContent} w-full min-h-48
                                         gap-x-{gapX.RxValue} gap-y-4
                                     """)
            )
            {
                [row: 0, column: 0] = HText("Top Left Cell",
                    strStyle: "p-3 text-base fw-bold bg-matcha-100 fg-matcha-700 rounded"),
                [row: 0, column: 1] = HText("Top Right Cell",
                    strStyle: "p-3 text-base fw-bold bg-matcha-50 fg-matcha-700 rounded"),
                [row: 1, column: 0, rowSpan: 1, colSpan: 2] =
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
        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("Absolute Positioning Layout", strStyle: DemoTitle),
            HSelectableText("Fixed Left/Top offset positioning inside a relativeTarget container", strStyle: DemoDesc),
            HAbsolute(_ => new(
                strStyle: "w-full h-80 bg-matcha-50 rounded-xl border border-matcha-200 relativeTarget overflow-hidden")
            {
                [left: 10, top: 10] =
                    HText("Top Left Anchor (10,10)",
                        strStyle: "text-sm fg-matcha-700 p-1 bg-white/80 rounded shadow-sm"),

                [left: 420, top: 10] =
                    HButton("Top Right Action",
                        strStyle: PrimaryBtnBase,
                        onClick: _ => Console.WriteLine("Top Right Button Clicked")
                    ), // HButton

                [left: 120, top: 120] =
                    HStackPanel(_ => new(
                        strStyle: "gap-2 vertical bg-white p-4 rounded-xl shadow-md border border-matcha-100 w-48")
                    {
                        HText("Center Floating Panel", strStyle: "text-base fw-bold fg-matcha-800"),
                        HText("Fixed Offset (120,120)", strStyle: "text-xs fg-coffee-500")
                    }), // HStackPanel

                [left: 420, top: 280] =
                    HButton("Bottom Right Action",
                        strStyle: SecondaryBtnBase,
                        onClick: _ => Console.WriteLine("Bottom Right Button Clicked")
                    ), // HButton

                [left: 20, top: 250] =
                    HStackPanel(_ => new(
                        strStyle: "gap-2 horizontal bg-matcha-900/80 p-3 rounded-xl shadow-md")
                    {
                        HText("Overlay Floating Panel", strStyle: "fg-matcha-50 fw-medium"),
                        HText("Coordinate (20,250)", strStyle: "fg-matcha-200 text-sm")
                    }) // HStackPanel
            }),
            HText("Absolute uses fixed Left/Top offsets for child positioning",
                strStyle: "text-xs fg-coffee-400 mt-2 italic")
        }); // HStackPanel
    }

    #endregion

    #region SplitView Sidebar Layout Demo

    private static IElement SplitViewDemo()
    {
        var isPaneOpen = new MutSignal<bool>(true);

        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("SplitView Collapsible Sidebar", strStyle: DemoTitle),
            HSelectableText("Compact inline mode with expand/collapse support", strStyle: DemoDesc),
            HSplitView(_ => new(
                isPaneOpen: isPaneOpen,
                displayMode: SplitViewDisplayMode.CompactInline,
                openPaneLength: 200,
                compactPaneLength: 48,
                strStyle: "w-full h-40 rounded-xl overflow-hidden border border-matcha-200")
            {
                Pane = HStackPanel(_ => new(strStyle: "gap-2 p-4 bg-matcha-50 vertical h-full")
                {
                    HButton("Home Dashboard", strStyle: SecondaryBtnBase + " w-full text-left"),
                    HButton("Application Settings", strStyle: SecondaryBtnBase + " w-full text-left")
                }), // HSplitView.Pane
                Content = HStackPanel(_ => new(strStyle: "h-full gap-2 p-6 vertical bg-matcha-300")
                {
                    HText("Main Content Area", strStyle: "text-lg fw-bold fg-matcha-800"),
                    HText("Sidebar supports expand/collapse in compact inline mode",
                        strStyle: "fg-coffee-500")
                }) // // HSplitView.Content
            }),
            HButton("Toggle Sidebar",
                strStyle: PrimaryBtnBase,
                onClick: _ => isPaneOpen.RxValue = !isPaneOpen.RxValue)
        }); // HStackPanel
    }

    #endregion

    #region SplitPanel Resizable Divider Demo

    private static IElement SplitPanelDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("SplitPanel Draggable Resizable Divider", strStyle: DemoTitle),
            HSelectableText("Drag to resize regions horizontally or vertically", strStyle: DemoDesc),
            HText("Horizontal Split", strStyle: "text-base fw-medium fg-matcha-700"),
            HSplitPanel(new(strStyle: "h-40 w-full rounded-xl overflow-hidden border border-matcha-200")
            {
                HText("Left Region",
                    strStyle: "mr-4 p-4 w-full h-full text-center bg-matcha-100 fg-matcha-800"),
                HText("Right Region",
                    strStyle: "ml-4 p-4 w-full h-full text-center bg-matcha-50 fg-matcha-800")
            }), // HSplitPanel
            HText("Vertical Split + Fixed Offset (200px)", strStyle: "text-base fw-medium fg-matcha-700 mt-2"),
            HStackPanel(_ => new(strStyle: "w-full h-60 gap-x-6 horizontal")
            {
                HSplitPanel(new(
                    splitFraction: 0.3,
                    strStyle: "h-full vertical rounded-xl overflow-hidden border border-matcha-200")
                {
                    HText("Top Region", strStyle: "w-full p-4 text-center fg-matcha-800 bg-matcha-100"),
                    HText("Bottom Region", strStyle: "w-full p-4 text-center fg-matcha-800 bg-matcha-50")
                }), // HSplitPanel
                HSplitPanel(new(
                    strStyle: "h-full horizontal rounded-xl overflow-hidden border border-matcha-200",
                    splitPosition: 200)
                {
                    HText("Left Panel", strStyle: "bg-matcha-100 p-4 h-full text-center fg-matcha-800"),
                    HText("Right Panel", strStyle: "bg-matcha-50 p-4 h-full text-center fg-matcha-800")
                }) // HSplitPanel
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region DockPanel Edge Dock Layout Demo

    private static IElement DockPanelDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("DockPanel Edge Docking Layout", strStyle: DemoTitle),
            HSelectableText("Children dock to Top, Bottom, Left, Right; last fills remaining space",
                strStyle: DemoDesc),
            HDockPanel(_ =>
                new(strStyle: "w-full h-80 bg-matcha-50 rounded-xl overflow-hidden border border-matcha-200")
                {
                    [Dock.Top] = HText("Top Dock Region",
                        strStyle: "p-3 bg-matcha-200 text-center fw-medium fg-matcha-800"),
                    [Dock.Bottom] = HText("Bottom Dock Region",
                        strStyle: "p-3 bg-matcha-100 text-center fw-medium fg-matcha-800"),
                    [Dock.Left] = HText("Left Dock Region",
                        strStyle: "p-3 bg-matcha-150 w-20 text-center fw-medium fg-matcha-800"),
                    [Dock.Right] = HText("Right Dock Region",
                        strStyle: "p-3 bg-matcha-150 w-20 text-center fw-medium fg-matcha-800"),
                    [null] = HText("Fill Remaining Space (last child)",
                        strStyle: "p-4 bg-white text-center fw-medium fg-matcha-800")
                }) // HDockPanel
        }); // HStackPanel
    }

    #endregion

    #region WrapPanel Auto Wrap Flow Layout Demo

    private static IElement WrapPanelDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("WrapPanel Auto‑Wrap Flow Layout", strStyle: DemoTitle),
            HSelectableText("Items automatically wrap to next line when width exceeds container", strStyle: DemoDesc),
            HWrapPanel(_ => new(
                strStyle: "w-100 gap-3 bg-matcha-50 p-4 rounded-xl border border-matcha-200")
            {
                HButton("Item 1", strStyle: PrimaryBtnBase + "text-lg"),
                HButton("Item 2", strStyle: SecondaryBtnBase + "text-lg"),
                HButton("Long Text Content Item 3", strStyle: SecondaryBtnBase + "text-lg"),
                HButton("Item 4", strStyle: PrimaryBtnBase + "text-lg")
            }) // HWrapPanel
        }); // HStackPanel
    }

    #endregion

    #region UniformGrid Equal Cell Grid Demo

    private static IElement UniformGridDemo()
    {
        var columnsSig = new MutSignal<int>(3);

        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("UniformGrid Equal‑Size Grid", strStyle: DemoTitle),
            HSelectableText("All cells have identical size, columns/rows adjustable", strStyle: DemoDesc),
            HUniformGrid(_ => new(
                rows: 2,
                columns: columnsSig,
                strStyle: $"{DemoContent} gap-3 transition-colors")
            {
                HText("Cell 1", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HText("Cell 2", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800"),
                HText("Cell 3", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HText("Cell 4", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800"),
                HText("Cell 5", strStyle: "p-4 text-center fw-bold bg-matcha-100 rounded fg-matcha-800"),
                HText("Cell 6", strStyle: "p-4 text-center fw-bold bg-matcha-50 rounded fg-matcha-800")
            }), // HUniformGrid
            HStackPanel(_ => new(strStyle: HorizontalRowBase + "mt-2")
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
        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("GridSplitter Column Resizing", strStyle: DemoTitle),
            HSelectableText("Drag the divider to resize adjacent grid columns", strStyle: DemoDesc),
            HGrid(_ => new(
                rowDefinitions: new([150]),
                columnDefinitions: new([100, HgLen.Auto, HgLen.Star()]),
                strStyle: DemoContent)
            {
                [row: 0, column: 0] = HText("Left Column",
                    strStyle: "mr-2 h-full p-3 bg-matcha-100 rounded fg-matcha-800 text-center"),
                [row: 0, column: 1] = HGridSplitter(
                    strStyle:
                    "w-1 h-full horizontal bg-matcha-300 hover:bg-matcha-500 transition-colors cursor-ew-resize"
                ),
                [row: 0, column: 2] = HText("Right Column",
                    strStyle: "ml-2 h-full p-3 bg-matcha-50 rounded fg-matcha-800 text-center")
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

        return HStackPanel(_ => new(strStyle: DemoCardBase + " vertical gap-4")
        {
            HSelectableText("ScrollViewer Scrollable Container", strStyle: DemoTitle),
            HSelectableText("Scrollable area with hidden horizontal and auto vertical scrollbars", strStyle: DemoDesc),
            HScrollViewer(out var scroll, _ => new(
                horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
                verticalScrollBarVisibility: ScrollBarVisibility.Hidden,
                strStyle: "w-full h-60 bg-white border border-matcha-200 rounded-xl"
            )
            {
                HStackPanel(_ => new(strStyle:
                    "w-full h-full p-3 vertical")
                {
                    HSelectableText(longText,
                        strStyle: "text-base leading-relaxed fg-matcha-800 selection:bg-coffee-200")
                }), // HStackPanel
            }), // HScrollViewer
            HStackPanel(_ => new(strStyle: HorizontalRowBase + " justify-center")
            {
                HButton(_ => new(
                    strStyle: PrimaryBtnBase,
                    onClick: _ => scroll.ScrollToHome()
                )
                {
                    HStackPanel(_ => new(strStyle: "w-30 h-6 mx-auto my-auto gap-2 horizontal")
                    {
                        HSvgImage("~/Assets/arrow-big-up-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-white"),
                        HText("Scroll To Top", strStyle: "fw-bold text-xs fg-white")
                    }) // HStackPanel
                }), // HButton
                HButton(_ => new(
                    strStyle: SecondaryBtnBase,
                    onClick: _ => scroll.ScrollToEnd()
                )
                {
                    HStackPanel(_ => new(strStyle: "w-30 h-6 mx-auto my-auto gap-2 horizontal")
                    {
                        HSvgImage("~/Assets/arrow-big-down-dash.svg", strStyle: "w-4 h-4 fw-extralight fg-white"),
                        HText("Scroll To Bottom", strStyle: "fw-bold text-xs fg-white")
                    }) // HStackPanel
                }) // HButton
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

}
