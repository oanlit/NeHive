// using Avalonia;
using Avalonia.Controls;
// using Avalonia.Input;
// using Avalonia.Controls.Primitives;
using NeHive.UI.Avalonia;

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
        // _counter = DemoComponent.ScrollDemo();
        // _counter = DemoComponent.TextBoxDemo();
        Content = _counter.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _counter.Dispose();
        base.OnClosed(e);
    }
}