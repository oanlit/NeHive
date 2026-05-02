using Avalonia.Controls;
using Avalonia.Interactivity;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render;

public static class Components
{
    public class HStackPanelProp(Element[]? children = null)
    {
        public Element[] Children = children ?? [];
    }

    public static readonly Component<HStackPanelProp> HStackPanel = new(prop =>
    {
        var stack = new StackPanel();
        foreach (var el in prop.Children)
        {
            stack.Children.Add(el.Content);
        }

        return stack;
    });

    public class HTextBlockProp(Accessor<string>? text = null)
    {
        public Accessor<string> Text = text ?? "";
    }

    public static readonly Component<HTextBlockProp> HTextBlock = new((prop, uiScope) =>
    {
        var text = new TextBlock();
        uiScope.AddEffect(() => { text.Text = prop.Text.Value; });
        return text;
    });

    public struct HButtonProp(Accessor<string>? content = null)
    {
        public Accessor<string> Content =  content ?? "";
    }

    public class HButtonExpose
    {
        public EventHandler<RoutedEventArgs> Click = (_, _) => { };
    }

    public static readonly Component<HButtonProp, HButtonExpose> HButton = new((prop, out expose, uiScope) =>
    {
        var button = new Button();
        uiScope.AddEffect(() => { button.Content = prop.Content.Value; });

        var expose1 = new HButtonExpose();
        uiScope.OnMount(() => { button.Click += expose1.Click; });
        expose = expose1;

        return new Element<HButtonExpose>(uiScope, button, expose);
    });
}