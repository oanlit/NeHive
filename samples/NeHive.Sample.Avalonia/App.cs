using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
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
        StyleParser.Init(definitions =>
        {
            var fonts = definitions.Fonts;
            fonts["jetmono"] = ["~/Assets/Fonts/JetBrainsMono#JetBrains Mono"];
            fonts["lxgw"] = ["~/Assets/Fonts/LXGWWenKai#LXGW WenKai"];
            var colors = definitions.Colors;
            
            colors["matcha-50"] = Color.FromRgb(245, 249, 238);
            colors["matcha-100"] = Color.FromRgb(233, 242, 221);
            colors["matcha-200"] = Color.FromRgb(214, 230, 198);
            colors["matcha-300"] = Color.FromRgb(189, 212, 168);
            colors["matcha-400"] = Color.FromRgb(168, 192, 142);
            colors["matcha-500"] = Color.FromRgb(140, 164, 112);
            colors["matcha-600"] = Color.FromRgb(112, 140, 84);
            colors["matcha-700"] = Color.FromRgb(94, 114, 75);
            colors["matcha-800"] = Color.FromRgb(70, 86, 54);
            colors["matcha-900"] = Color.FromRgb(48, 61, 36);
            colors["matcha-950"] = Color.FromRgb(32, 40, 24);
            
            colors["coffee-50"] = Color.FromRgb(248, 246, 238);
            colors["coffee-200"] = Color.FromRgb(217, 207, 188);
            colors["coffee-400"] = Color.FromRgb(176, 155, 132);
            colors["coffee-500"] = Color.FromRgb(150, 130, 108);
            colors["coffee-700"] = Color.FromRgb(124, 107, 86);
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