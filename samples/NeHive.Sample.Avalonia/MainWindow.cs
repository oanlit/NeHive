using Avalonia.Controls;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly IElement _counter;

    public MainWindow()
    {
        // _counter = DemoComponent.ShowDemo();
        // _counter = DemoComponent.ForEachDemo();
        _counter = DemoComponent.SwitchDemo();
        // _counter = DemoComponent.GridDemo();
        // _counter = DemoComponent.AbsoluteDemo();
        Content = _counter.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _counter.Dispose();
        base.OnClosed(e);
    }
}