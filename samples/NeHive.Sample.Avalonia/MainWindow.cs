using Avalonia.Controls;
using NeHive.UI.Avalonia;
// using Avalonia.Media;
// using Avalonia;
// using Avalonia.Controls.Chrome;
// using Avalonia.Input;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly IElement _mainNavDemo;

    public MainWindow()
    {
        Width = 1000;
        Height = 700;
        Title = "NeHive UI Avalonia Demo";
        // TransparencyLevelHint =
        // [
        //     WindowTransparencyLevel.Mica,
        //     WindowTransparencyLevel.Transparent
        // ];
        // Background = Brushes.Transparent;
        // ExtendClientAreaToDecorationsHint = true;
        // WindowDecorations = WindowDecorations.None;
        
        // var titleBar = new ExperimentalAcrylicBorder
        // {
        //     Height = 32,
        //     Material = new ExperimentalAcrylicMaterial
        //     {
        //         // BackgroundSource = AcrylicBackgroundSource.Digger,
        //         MaterialOpacity = 0.8,
        //         // 降级颜色：当系统不支持模糊效果时，会显示为这个颜色[reference:3]
        //         FallbackColor = Colors.DarkSlateGray
        //     },
        //     Child = new TextBlock
        //     {
        //         Text = "我的毛玻璃应用",
        //         Foreground = Brushes.Black,
        //         Margin = new Thickness(12, 0),
        //         VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center
        //     }
        // };
        //
        // WindowDecorationProperties.SetElementRole(titleBar, WindowDecorationsElementRole.TitleBar);
        //
        // _mainNavDemo = DemoComponent.MainNavDemo();
        // _mainNavDemo = MusicPlayerDemo.CorePlayer();
        // _mainNavDemo = DemoComponent.GroupDemo();
        _mainNavDemo = DemoComponent.ContextDemo();
        Content = _mainNavDemo.Content;
        
        // var mainPanel = new DockPanel();
        // DockPanel.SetDock(titleBar, Dock.Top);
        // mainPanel.Children.Add(titleBar);
        // mainPanel.Children.Add(_mainNavDemo.Content);
        // Content = mainPanel;
    }

    protected override void OnClosed(EventArgs e)
    {
        _mainNavDemo.Dispose();
        base.OnClosed(e);
    }
}