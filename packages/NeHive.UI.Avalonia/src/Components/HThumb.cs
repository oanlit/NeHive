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

public class HThumbProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<VectorEventArgs>? onDragStarted = null,
    Action<VectorEventArgs>? onDragDelta = null,
    Action<VectorEventArgs>? onDragCompleted = null) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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
    public static IElement<Thumb> HThumb(HThumbProp prop)
    {
        return Element<Thumb>.WithScope(uiScope =>
        {
            var content = ElementUtil.WrapSingleContainerContent(prop).Content;

            var border = new Border
            {
                Child = content
            };

            var thumb = new Thumb
            {
                Template = new FuncControlTemplate((_, _) => border)
            };

            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants
            };

            state.ApplyAccessorStyle(prop.Style, content, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);

            if (prop.OnDragStarted is not null) thumb.DragStarted += (_, e) => prop.OnDragStarted(e);
            if (prop.OnDragDelta is not null) thumb.DragDelta += (_, e) => prop.OnDragDelta(e);
            if (prop.OnDragCompleted is not null) thumb.DragCompleted += (_, e) => prop.OnDragCompleted(e);

            return (thumb, thumb);
        });
    }
}