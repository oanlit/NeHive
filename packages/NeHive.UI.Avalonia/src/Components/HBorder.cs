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
    Accessor<bool>? isAllowDrop = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<DragEventArgs>? onDragEnter = null,
    Action<DragEventArgs>? onDragOver = null,
    Action<DragEventArgs>? onDragLeave = null,
    Action<DragEventArgs>? onDrop = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsAllowDrop = isAllowDrop;
    public readonly Action<DragEventArgs>? OnDragEnter = onDragEnter;
    public readonly Action<DragEventArgs>? OnDragOver = onDragOver;
    public readonly Action<DragEventArgs>? OnDragLeave = onDragLeave;
    public readonly Action<DragEventArgs>? OnDrop = onDrop;

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
        Accessor<bool>? isAllowDrop = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseEvents = null,
        Action<DragEventArgs>? onDragEnter = null,
        Action<DragEventArgs>? onDragOver = null,
        Action<DragEventArgs>? onDragLeave = null,
        Action<DragEventArgs>? onDrop = null
    ) => HBorder(out _, _ => new(isAllowDrop, strStyle, style, baseEvents,
        onDragEnter, onDragOver, onDragLeave, onDrop));
    
    public static IElement<Border> HBorder(
        out Border expose,
        Accessor<bool>? isAllowDrop = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseEvents = null,
        Action<DragEventArgs>? onDragEnter = null,
        Action<DragEventArgs>? onDragOver = null,
        Action<DragEventArgs>? onDragLeave = null,
        Action<DragEventArgs>? onDrop = null
    ) => HBorder(out expose, _ => new(isAllowDrop, strStyle, style, baseEvents,
        onDragEnter, onDragOver, onDragLeave, onDrop));
    
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

            if (args.BaseInteraction is not null)
                args.BaseInteraction.ApplyInteractions(uiScope, border);

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            if (args.IsAllowDrop is not null)
            {
                DragDrop.SetAllowDrop(border, args.IsAllowDrop.Value);
                if (args.IsAllowDrop.IsReactive)
                    uiScope.CreateEffect(scope => DragDrop.SetAllowDrop(border, scope.Track(args.IsAllowDrop)));
            }

            if (args.OnDragEnter is not null) DragDrop.AddDragEnterHandler(border, (_, e) => args.OnDragEnter(e));
            if (args.OnDragOver is not null) DragDrop.AddDragOverHandler(border, (_, e) => args.OnDragOver(e));
            if (args.OnDragLeave is not null) DragDrop.AddDragLeaveHandler(border, (_, e) => args.OnDragLeave(e));
            if (args.OnDrop is not null) DragDrop.AddDropHandler(border, (_, e) => args.OnDrop(e));

            return (border, border);
        });
    }
}