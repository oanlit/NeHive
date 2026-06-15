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
        _mainNavDemo = DemoComponent.MainNavDemo();
        // _mainNavDemo = MusicPlayerDemo.MusicPlayer();
        // _mainNavDemo = DemoComponent.TextStyleDemo();
        // _mainNavDemo = DemoComponent.GroupDemo();
        // _mainNavDemo = DemoComponent.ContextDemo();
        Content = _mainNavDemo.Content;
        // var slider = new SimpleSlider
        // {
        //     Width = 300,
        //     Height = 20,
        //     Minimum = 0,
        //     Maximum = 100,
        //     Value = 50
        // };
        //
        // Content = slider;
    }

    protected override void OnClosed(EventArgs e)
    {
        _mainNavDemo.Dispose();
        base.OnClosed(e);
    }
}