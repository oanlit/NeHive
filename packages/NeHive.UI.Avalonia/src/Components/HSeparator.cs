using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HSeparatorProps(
    Scope scope,
    Border border,
    Separator separator
) : BaseComponentProps(scope, border, separator);

public class HSeparatorArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public IElement? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<Separator> HSeparator(
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HSeparator(out _, _ => new(strStyle, style, baseInteraction));
    
    public static IElement<Separator> HSeparator(
        out Separator expose,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HSeparator(out expose, _ => new(strStyle, style, baseInteraction));

    public static IElement<Separator> HSeparator(Func<HSeparatorProps, HSeparatorArgs> fn) => HSeparator(out _, fn);

    public static IElement<Separator> HSeparator(
        out Separator expose,
        Func<HSeparatorProps, HSeparatorArgs> fn)
    {
        var sep = new Separator();
        expose = sep;
        return Element<Separator>.WithScope(uiScope =>
        {
            var border = new Border();

            var props = new HSeparatorProps(uiScope, border, sep);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };
            state.ApplyAccessorStyle(args.StrStyle, sep, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(sep, border, StyleUtil.ApplyStyle);
            
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(sep, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, sep);

            var template = args.Template;
            if (template is null)
            {
                sep.Template = new FuncControlTemplate(delegate { return border; });
            }
            else
            {
                sep.Template = new FuncControlTemplate(delegate
                {
                    using (new ScopeFrame(uiScope))
                    {
                        border.Child = template.Content;
                    }

                    return border;
                });
            }

            return (sep, border);
        });
    }
}