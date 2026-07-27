using System.Collections;
using Avalonia.Input;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HDropArgs(
    Accessor<bool>? isAllowDrop = null,
    Action<DragEventArgs>? onDragEnter = null,
    Action<DragEventArgs>? onDragOver = null,
    Action<DragEventArgs>? onDragLeave = null,
    Action<DragEventArgs>? onDrop = null
):ISingleChildrenArgs
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

public static partial class AttachComponent
{
    public static IElement HDrop(HDropArgs args)
    {
        return Element.WithScope(uiScope =>
        {
            var element = ElementUtil.WrapSingleContainerContent(args);
            var control = element.Content;
            if (args.IsAllowDrop is not null)
            {
                DragDrop.SetAllowDrop(control, args.IsAllowDrop.Value);
                if (args.IsAllowDrop.IsReactive)
                    uiScope.CreateEffect(scope => DragDrop.SetAllowDrop(control, scope.Track(args.IsAllowDrop)));
            }

            if (args.OnDragEnter is not null) DragDrop.AddDragEnterHandler(control, (_, e) => args.OnDragEnter(e));
            if (args.OnDragOver is not null) DragDrop.AddDragOverHandler(control, (_, e) => args.OnDragOver(e));
            if (args.OnDragLeave is not null) DragDrop.AddDragLeaveHandler(control, (_, e) => args.OnDragLeave(e));
            if (args.OnDrop is not null) DragDrop.AddDropHandler(control, (_, e) => args.OnDrop(e));
            return element;
        });
    }
}