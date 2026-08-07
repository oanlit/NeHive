using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HThumbProps(Scope scope, Border border, Thumb thumb) : BaseComponentProps(scope, border, thumb);

public class HThumbArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<VectorEventArgs>? onDragStarted = null,
    Action<VectorEventArgs>? onDragDelta = null,
    Action<VectorEventArgs>? onDragCompleted = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

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
    public static IElement<Thumb> HThumb(
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<VectorEventArgs>? onDragStarted = null,
        Action<VectorEventArgs>? onDragDelta = null,
        Action<VectorEventArgs>? onDragCompleted = null
    ) => HThumb(out _, _ => new(strStyle, style, baseInteraction, onDragStarted, onDragDelta, onDragCompleted));

    public static IElement<Thumb> HThumb(
        out Thumb expose,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<VectorEventArgs>? onDragStarted = null,
        Action<VectorEventArgs>? onDragDelta = null,
        Action<VectorEventArgs>? onDragCompleted = null
    ) => HThumb(out expose, _ => new(strStyle, style, baseInteraction, onDragStarted, onDragDelta, onDragCompleted));

    public static IElement<Thumb> HThumb(Func<HThumbProps, HThumbArgs> fn) => HThumb(out _, fn);

    public static IElement<Thumb> HThumb(out Thumb expose, Func<HThumbProps, HThumbArgs> fn)
    {
        var thumb = new Thumb();
        expose = thumb;
        return Element<Thumb>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HThumbProps(uiScope, border, thumb);
            var args = fn(props);
            InternalThumbUtil.SetThumbEffectCore(uiScope, thumb, border, args);
            return (thumb, thumb);
        });
    }
}

internal static class InternalThumbUtil
{
    internal static void SetThumbEffectCore(UiScope uiScope, Thumb thumb, Border border, HThumbArgs args,Action<StyleSet, Layoutable, Border>? applyStyle = null)
    {
        var content = ElementUtil.WrapSingleContainerContent(args).Content;
        border.Child = content;
        thumb.Template = new FuncControlTemplate((_, _) => border);

        var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
        {
            PriorityStyle = args.Style,
            StrVariants = args.StrStyle.Value.Variants
        };

        applyStyle ??= StyleUtil.ApplyStyle;
        state.ApplyAccessorStyle(args.StrStyle, content, border, applyStyle);
        state.ApplyVariantsStyle(content, border, applyStyle);

        if (args.Popups is not null)
            ElementUtil.ApplyPopups(thumb, args.Popups);
        args.BaseInteraction?.ApplyInteractions(uiScope, thumb);

        if (args.OnDragStarted is not null)
        {
            uiScope.AddHandler<EventHandler<VectorEventArgs>>(
                add: h => thumb.DragStarted += h,
                remove: h => thumb.DragStarted -= h,
                handler: (_, e) => args.OnDragStarted(e)
            );
        }

        if (args.OnDragDelta is not null)
        {
            uiScope.AddHandler<EventHandler<VectorEventArgs>>(
                add: h => thumb.DragDelta += h,
                remove: h => thumb.DragDelta -= h,
                handler: (_, e) => args.OnDragDelta(e)
            );
        }

        if (args.OnDragCompleted is not null)
        {
            uiScope.AddHandler<EventHandler<VectorEventArgs>>(
                add: h => thumb.DragCompleted += h,
                remove: h => thumb.DragCompleted -= h,
                handler: (_, e) => args.OnDragCompleted(e)
            );
        }
    }
}