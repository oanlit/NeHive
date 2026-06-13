using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

using NeHive.Model;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.Sample.Avalonia;

public class App : Application
{
    public override void Initialize()
    {
        NeHiveContext.SetProjBaseUri("avares://NeHive.Sample.Avalonia/");
        StyleParser.Init(definitions  =>
        {
            var fonts = definitions.Fonts;
            fonts["jetmono"] = ["~/Assets/Fonts/JetBrainsMono#JetBrains Mono"];
            fonts["lxgw"] = ["~/Assets/Fonts/LXGWWenKai#LXGW WenKai"];
        });
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