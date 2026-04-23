using Avalonia.Controls;
using NeHive.Sample.Avalonia.Render;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly Component _counter;

    public MainWindow()
    {
        _counter = CounterComponent.Demo();
        Content = _counter.Content;
    }

    protected override void OnClosed(EventArgs e)
    {
        _counter.Dispose();
        base.OnClosed(e);
    }
}