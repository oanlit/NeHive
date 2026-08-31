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
    #region Main Category Navigation Layout

    public static IElement MainNavDemo()
    {
        var categories = new List<DemoCategory>
        {
            new("📐 Layout Panels", DemoView.GridDemo, DemoView.AbsoluteDemo, DemoView.SplitViewDemo,
                DemoView.SplitPanelDemo, DemoView.DockPanelDemo, DemoView.WrapPanelDemo,
                DemoView.UniformGridDemo, DemoView.GridSplitterDemo, DemoView.ScrollDemo),

            new("🔘 Basic Input Controls", DemoView.TextBoxDemo, DemoView.CommandDemo,
                DemoView.AsyncCommandProgressDemo,
                DemoView.CheckBoxDemo, DemoView.RadioButtonDemo,
                DemoView.ToggleSwitchDemo, DemoView.FilePickerDemo, DemoView.DragFileDemo,
                DemoView.SliderDemo, DemoView.PopupDemo, DemoView.FlyoutDemo, DemoView.WindowDemo),

            new("📋 Data Selection & Lists", DemoView.TreeViewDemo, DemoView.ComboBoxDemo),

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
                DemoView.CustomSliderDemo,
                DemoView.CustomScrollDemo,
                DemoView.GroupDemo,
                DemoView.ContextDemo
            ),

            new("🚀 Advanced Integrated Sample", DemoView.ClockDemo, DemoView.MusicPlayerDemo)
        };

        var categoriesSignal = new MutSignal<IReadOnlyList<DemoCategory>>(categories);
        var selectedCategory = new MutSignal<DemoCategory>(categories[0]);
        var currentView = new MutSignal<DemoView>(categories[0].Demos.First());

        void SelectDemo(DemoView view) => currentView.RxValue = view;

        return HGrid(_ => new(
            columnDefinitions: new([224, HgLen.Star()]),
            strStyle: "w-full h-full gap-4")
        {
            // Left Category Sidebar – Manual implementation without HListBox
            [row: 0, column: 0] = HScrollViewer(_ => new()
            {
                ForEach<DemoCategory>(new(categoriesSignal)
                {
                    ItemsPanel = HStackPanel(strStyle:
                        "h-full vertical gap-1 px-3 py-2 bg-white border border-matcha-200 rounded-2xl shadow-sm"),
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
            }), // [row: 0, column: 0]
            // Right Main Content Area (unchanged)
            [row: 0, column: 1] = HGrid(_ => new(
                rowDefinitions: new([HgLen.Auto, HgLen.Star()]),
                strStyle: "gap-4 h-full")
            {
                // Top Demo Button Grid
                [row: 0, column: 2] =
                    ForEach<DemoView>(new(new(() => selectedCategory.RxValue.Demos))
                    {
                        ItemsPanel = HUniformGrid(columns: 4,
                            strStyle: "gap-2 p-3 bg-white rounded-xl border border-matcha-200 shadow-sm"),
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
                // [row: 0, column: 2]

                // Bottom Demo Render Viewport (unchanged)
                [row: 1, column: 0] =
                    HScrollViewer(_ => new(strStyle: "w-full max-h-125")
                    {
                        Switch<DemoView>(new(currentView)
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
                            [DemoView.CommandDemo] = CommandDemo,
                            [DemoView.AsyncCommandProgressDemo] = AsyncCommandProgressDemo,
                            [DemoView.CheckBoxDemo] = CheckBoxDemo,
                            [DemoView.RadioButtonDemo] = RadioButtonDemo,
                            [DemoView.ToggleSwitchDemo] = ToggleSwitchDemo,
                            [DemoView.FilePickerDemo] = FilePickerDemo,
                            [DemoView.DragFileDemo] = DragFileDemo,
                            [DemoView.SliderDemo] = SliderDemo,
                            [DemoView.PopupDemo] = PopupDemo,
                            [DemoView.FlyoutDemo] = FlyoutDemo,
                            [DemoView.WindowDemo] = WindowDemo,

                            [DemoView.TreeViewDemo] = TreeViewDemo,
                            [DemoView.ComboBoxDemo] = ComboBoxDemo,

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

                            [DemoView.CustomSliderDemo] = CustomSliderDemo,
                            [DemoView.CustomScrollDemo] = CustomScrollDemo,

                            [DemoView.GroupDemo] = GroupDemo,
                            [DemoView.ContextDemo] = ContextDemo,

                            [DemoView.ClockDemo] = ClockDemo,
                            [DemoView.MusicPlayerDemo] = MusicPlayerDemo.MusicPlayer,
                            Default = () => HText("Select a demo item from left sidebar to preview",
                                strStyle: "fg-gray-400 text-center p-16 text-lg")
                        }) // Switch<DemoView>
                    }) // HScrollViewer
                // [row: 1, column: 0]
            }) // // HGird
        }); // HGird
    }

    #endregion

    public static IElement MainNav()
    {
        return Element.WithScope(scope =>
        {
            var isLockWindow = scope.GetContext(ContextKey.LockWindowCount);
            var lockStyle = scope.CreateComputed(() =>
                isLockWindow?.RxValue is 0
                    ? "hidden"
                    : "w-9999 h-9999 bg-black/40 visible"
            );

            return HStackPanel(_ => new(strStyle: "w-full h-full")
            {
                HPanel(_ => new(strStyle: "w-full h-full")
                {
                    MainNavDemo(),
                    HBorder(strStyle: lockStyle)
                }) // HPanel
            }); // RootElement
        });
    }

    // Helper Record Type
    private record DemoCategory(string Name, params DemoView[] Demos);
}
