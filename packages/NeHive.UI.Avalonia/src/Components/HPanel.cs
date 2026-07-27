using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HPanelProps(Scope scope, Border border, Panel content) : BaseComponentProps(scope, border, content);

public class HPanelArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? events = null
) : BaseComponentArgs(strStyle, style, events), ISingleChildrenArgs
{
    public List<IElement> Children { private get; init; } = [];

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

            state.ApplyAccessorStyle(args.StrStyle, panel, border, ApplyStyle);
            state.ApplyVariantsStyle(panel, border, ApplyStyle);

            if (args.BaseInteraction is not null)
                args.BaseInteraction.ApplyInteractions(uiScope, panel);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            return (panel, border);

            void ApplyStyle(StyleSet style, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(style, layout, bord);
                if (style.Width is not null)
                    panel.Width = style.Width.Value;

                if (style.Height is not null)
                    panel.Height = style.Height.Value;

                if (style.MinWidth is not null)
                    panel.MinWidth = style.MinWidth.Value;

                if (style.MaxWidth is not null)
                    panel.MaxWidth = style.MaxWidth.Value;

                if (style.MinHeight is not null)
                    panel.MinHeight = style.MinHeight.Value;

                if (style.MaxHeight is not null)
                    panel.MaxHeight = style.MaxHeight.Value;
            }
        });
    }
}