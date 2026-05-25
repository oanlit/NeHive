using System;
using Avalonia.Controls;
using NeHive.UI.Avalonia;

namespace NeHive.Sample.Xplat.Views;

public partial class MainWindow : Window
{
    private readonly IElement _mainNavDemo;
    
    public MainWindow()
    {
        Width = 1000;
        Height = 700;
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