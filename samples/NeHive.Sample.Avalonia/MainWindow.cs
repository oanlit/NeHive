using Avalonia.Controls;
using NeHive.UI.Avalonia;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly IElement _mainNavDemo;

    public MainWindow()
    {
        Width = 1000;
        Height = 700;
        Title = "NeHive UI Avalonia Demo";
        // _mainNavDemo = DemoComponent.MainNavDemo();
        _mainNavDemo = MusicPlayerDemo.CorePlayer();
        // _mainNavDemo = DemoComponent.SvgDemo();
        Content = _mainNavDemo.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _mainNavDemo.Dispose();
        base.OnClosed(e);
    }
}