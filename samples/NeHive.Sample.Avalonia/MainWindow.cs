using Avalonia.Controls;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly Element _counter;

    public MainWindow()
    {
        // _counter = CounterComponent.ShowDemo.Create();
        _counter = CounterComponent.ForEachDemo.Create();
        Content = _counter.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _counter.Dispose();
        base.OnClosed(e);
    }
}