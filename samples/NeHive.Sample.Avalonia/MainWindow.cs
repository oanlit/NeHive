using Avalonia;
using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia;

namespace NeHive.Sample.Avalonia;

public class MainWindow : Window
{
    private readonly Scope _scope = new();

    public MainWindow()
    {
        Width = 1000;
        Height = 720;
        Title = "NeHive UI Avalonia Demo";
        var size = new MutSignal<Size>(ClientSize);
        Resized += (_, e) => size.RxValue = e.ClientSize;

        var isLockWindow = new MutSignal<int>(0);
        _scope
            .SetContext(NeHiveUiContext.Window, this)
            .SetContext(NeHiveUiContext.WindowSize, size)
            .SetContext(ContextKey.LockWindowCount, isLockWindow);

        using (new ScopeFrame(_scope))
        {
            Content = DemoComponent.MainNav().Content;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _scope.Dispose();
        base.OnClosed(e);
    }
}