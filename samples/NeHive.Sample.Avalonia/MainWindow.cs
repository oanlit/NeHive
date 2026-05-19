// using Avalonia;
using Avalonia.Controls;
// using Avalonia.Input;
// using Avalonia.Controls.Primitives;
using NeHive.UI.Avalonia;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly IElement _mainNavDemo;

    public MainWindow()
    {
        Width = 1000;
        Height = 600;
        Title = "NeHive UI Avalonia Demo";
        _mainNavDemo = DemoComponent.MainNavDemo();
        // _mainNavDemo = DemoComponent.SplitViewDemo();
        // _mainNavDemo = DemoComponent.SplitPanelDemo();
        // _mainNavDemo = DemoComponent.CheckBoxDemo();
        // _mainNavDemo = DemoComponent.RadioButtonDemo();
        // _mainNavDemo = DemoComponent.ToggleSwitchDemo();
        // _mainNavDemo = DemoComponent.FilePickerDemo();
        // _mainNavDemo = DemoComponent.ProgressBarDemo();
        // _mainNavDemo = DemoComponent.TabViewDemo();
        // _mainNavDemo = DemoComponent.SliderDemo();
        // _mainNavDemo = DemoComponent.TreeViewDemo();
        // _mainNavDemo = DemoComponent.ListBoxDemo();
        // _mainNavDemo = DemoComponent.ComboBoxDemo();
        // _mainNavDemo = DemoComponent.MenuDemo();
        // _mainNavDemo = DemoComponent.GridSplitterDemo();
        // _mainNavDemo = DemoComponent.UniformGridDemo();
        // _mainNavDemo = DemoComponent.DockPanelDemo();
        // _mainNavDemo = DemoComponent.WrapPanelDemo();
        Content = _mainNavDemo.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _mainNavDemo.Dispose();
        base.OnClosed(e);
    }
}