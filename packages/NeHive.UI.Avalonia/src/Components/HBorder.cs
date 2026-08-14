using System.Collections;
using Avalonia.Input;
using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HBorderProps(Scope scope, Border border) : BaseComponentProps(scope, border, border);

public class HBorderArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
    {
        _children.Add(element);
    }
}

public static partial class BaseComponent
{
    public static IElement<Border> HBorder(
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HBorder(out _, _ => new(strStyle, style, baseInteraction));
    
    public static IElement<Border> HBorder(
        out Border expose,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HBorder(out expose, _ => new(strStyle, style, baseInteraction));
    
    public static IElement<Border> HBorder(Func<HBorderProps, HBorderArgs> fn) => HBorder(out _, fn);

    public static IElement<Border> HBorder(out Border expose, Func<HBorderProps, HBorderArgs> fn)
    {
        var border = new Border();
        expose = border;
        return Element<Border>.WithScope(uiScope =>
        {
            var props = new HBorderProps(uiScope, border);
            var args = fn(props);

            border.Child = ElementUtil.WrapSingleContainerContent(args).Content;

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, border, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(border, border, StyleUtil.ApplyStyle);

            args.BaseInteraction?.ApplyInteractions(uiScope, border);

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            return (border, border);
        });
    }
}