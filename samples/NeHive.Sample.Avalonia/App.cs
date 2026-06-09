using Avalonia;
using NeHive.Model;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace NeHive.Sample.Avalonia;

public class App : Application
{
    public override void Initialize()
    {
        NeHiveContext.SetProjBaseUri("avares://NeHive.Sample.Avalonia/");
        RequestedThemeVariant = ThemeVariant.Default;
        Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}