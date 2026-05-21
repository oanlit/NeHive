using Avalonia;
// using Avalonia.Media;
// using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace NeHive.Sample.Avalonia;

public class App : Application
{
    public override void Initialize()
    {
        RequestedThemeVariant = ThemeVariant.Default;
        Styles.Add(new FluentTheme());
        // var style = new Style(x => x.OfType<ScrollViewer>().Child().OfType<ScrollBar>());
        // style.Setters.Add(new Setter(ScrollBar.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
        // style.Setters.Add(new Setter(ScrollBar.CornerRadiusProperty, new CornerRadius(4)));
        //
        // // 针对滑块（Thumb）的样式
        // var thumbStyle = new Style(x => x.OfType<ScrollViewer>().Descendant().OfType<Thumb>());
        // thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Color.Parse("#4A90E2"))));
        // thumbStyle.Setters.Add(new Setter(Thumb.CornerRadiusProperty, new CornerRadius(2)));
        //
        // Styles.Add(style);
        // Styles.Add(thumbStyle);
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