using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace NeHive.UI.Avalonia.Components;

public class HItemsPresenterArgs
{
    public IElement<Panel>? ItemsPanel
    {
        get;
        init
        {
            if (value is null) return;
            field = value;
        }
    } = BaseComponent.HStackPanel(_ => new(
        style: new(
            orientation: Orientation.Vertical,
            horizontalAlignment: HorizontalAlignment.Stretch,
            verticalAlignment: VerticalAlignment.Stretch,
            background: new(Brushes.White)
        )));
}

public static partial class BaseComponent
{
    public static IElement<ItemsPresenter> HItemsPresenter(HItemsPresenterArgs args) =>
        HItemsPresenter(out _, args);

    public static IElement<ItemsPresenter> HItemsPresenter(out ItemsPresenter expose,
        HItemsPresenterArgs args)
    {
        var itemsPresenter = new ItemsPresenter();
        expose = itemsPresenter;
        return Element<ItemsPresenter>.WithScope(_ =>
        {
            Control control = itemsPresenter;
            var content = args.ItemsPanel!.Content;
            var panel = args.ItemsPanel.Expose!;
            if (content is Border panelBorder && panel.Parent == content)
            {
                panelBorder.Child = itemsPresenter;
                control = panelBorder;
            }

            itemsPresenter.ItemsPanel = new FuncTemplate<Panel?>(() => panel);

            return (itemsPresenter, control);
        });
    }
}