using Avalonia.Controls;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly IElement _counter;

    public MainWindow()
    {
        _counter = CounterComponent.ShowDemo();
        // _counter = CounterComponent.ForEachDemo();
        // _counter = CounterComponent.LoadDemo();
        Content = _counter.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _counter.Dispose();
        base.OnClosed(e);
    }
}