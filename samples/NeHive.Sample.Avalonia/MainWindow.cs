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
        Content = _mainNavDemo.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _mainNavDemo.Dispose();
        base.OnClosed(e);
    }
}