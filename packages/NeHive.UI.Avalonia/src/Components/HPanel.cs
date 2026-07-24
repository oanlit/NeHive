using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HPanelProps(Scope scope, Border border, Panel content) : BaseComponentProps(scope, border, content);

public class HPanelArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null) : ISingleChildrenArgs
{
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public List<IElement> Children { private get; init; } = [];
    public List<IElement<Popup>>? Popups { internal get; init; }

    public IEnumerator<IElement> GetEnumerator()
        => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
    {
        Children.Add(element);
    }
}

public static partial class BaseComponent
{
    public static IElement<Panel> HPanel(Func<HPanelProps, HPanelArgs> fn)
    {
        return Element<Panel>.WithScope(uiScope =>
        {
            var panel = new Panel();
            var border = new Border
            {
                Child = panel
            };

            var props = new HPanelProps(uiScope, border, panel);
            var args = fn(props);

            foreach (var child in args)
                panel.Children.Add(child.Content);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, panel, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(panel, border, StyleUtil.ApplyStyle);

            if (args.Popups is not null)
            {
                foreach (var popupEl in args.Popups)
                {
                    _ = popupEl.Content;
                    var popup = popupEl.Expose!;
                    popup.PlacementTarget = border;
                }
            }

            return (panel, border);
        });
    }
}