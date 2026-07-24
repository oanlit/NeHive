using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HThumbArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<VectorEventArgs>? onDragStarted = null,
    Action<VectorEventArgs>? onDragDelta = null,
    Action<VectorEventArgs>? onDragCompleted = null) : ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public readonly Action<VectorEventArgs>? OnDragStarted = onDragStarted;
    public readonly Action<VectorEventArgs>? OnDragDelta = onDragDelta;
    public readonly Action<VectorEventArgs>? OnDragCompleted = onDragCompleted;

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
    public static IElement<Thumb> HThumb(HThumbArgs args)
    {
        return Element<Thumb>.WithScope(uiScope =>
        {
            var content = ElementUtil.WrapSingleContainerContent(args).Content;

            var border = new Border
            {
                Child = content
            };

            var thumb = new Thumb
            {
                Template = new FuncControlTemplate((_, _) => border)
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, content, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);

            if (args.OnDragStarted is not null) thumb.DragStarted += (_, e) => args.OnDragStarted(e);
            if (args.OnDragDelta is not null) thumb.DragDelta += (_, e) => args.OnDragDelta(e);
            if (args.OnDragCompleted is not null) thumb.DragCompleted += (_, e) => args.OnDragCompleted(e);

            return (thumb, thumb);
        });
    }
}