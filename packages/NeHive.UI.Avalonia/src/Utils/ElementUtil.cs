using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using NeHive.UI.Avalonia.Components;
using static NeHive.UI.Avalonia.Components.BaseComponent;

namespace NeHive.UI.Avalonia.Utils;

public static class ElementUtil
{
    public static IElement WrapSingleContainerContent(IEnumerable<IElement> elements)
    {
        var count = elements.Count();
        IElement child;
        switch (count)
        {
            case 0:
                child = Element.Empty;
                break;
            case 1:
                child = elements.First();
                break;
            default:
                var panelProp = new HStackPanelArgs();
                foreach (var el in elements)
                {
                    panelProp.Add(el);
                }

                child = HStackPanel(_ => panelProp);
                break;
        }

        return child;
    }

    public static void ApplyPopups(Control targetControl, IEnumerable<IElement<Popup>> popups)
    {
        foreach (var popupEl in popups)
        {
            _ = popupEl.Content;
            var popup = popupEl.Expose!;
            popup.PlacementTarget = targetControl;
        }
    }
}