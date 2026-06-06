using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using NeHive.Model;
using NeHive.Sample.Xplat.Views;

namespace NeHive.Sample.Xplat;

public class App : Application
{
    public override void Initialize()
    {
        NeHiveContext.SetProjBaseUri("avares://NeHive.Sample.Xplat/");
        RequestedThemeVariant = ThemeVariant.Default;
        Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = MainViewFactory;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = MainViewFactory();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Control MainViewFactory()
        => DemoComponent.MainNavDemo().Content;
}