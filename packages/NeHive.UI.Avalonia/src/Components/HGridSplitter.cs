using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HGridSplitterProps(Scope scope, Border border, GridSplitter gridSplitter)
    : HThumbProps(scope, border, gridSplitter)
{
    public MutSignal<bool> IsShowsPreview
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(gridSplitter.ShowsPreview);
            BridgeAvalonia.BindPropertySignal(scope, field, gridSplitter, GridSplitter.ShowsPreviewProperty);
            return field;
        }
    }

    public MutSignal<GridResizeDirection> ResizeDirection
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<GridResizeDirection>(gridSplitter.ResizeDirection);
            BridgeAvalonia.BindPropertySignal(scope, field, gridSplitter, GridSplitter.ResizeDirectionProperty);
            return field;
        }
    }

    public MutSignal<GridResizeBehavior> ResizeBehavior
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<GridResizeBehavior>(gridSplitter.ResizeBehavior);
            BridgeAvalonia.BindPropertySignal(scope, field, gridSplitter, GridSplitter.ResizeBehaviorProperty);
            return field;
        }
    }

    public MutSignal<double> KeyboardIncrement
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(gridSplitter.KeyboardIncrement);
            BridgeAvalonia.BindPropertySignal(scope, field, gridSplitter, GridSplitter.KeyboardIncrementProperty);
            return field;
        }
    }

    public MutSignal<double> DragIncrement
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<double>(gridSplitter.DragIncrement);
            BridgeAvalonia.BindPropertySignal(scope, field, gridSplitter, GridSplitter.DragIncrementProperty);
            return field;
        }
    }
}

public class HGridSplitterArgs(
    Accessor<bool>? isShowsPreview = null,
    Accessor<GridResizeDirection>? resizeDirection = null,
    Accessor<GridResizeBehavior>? resizeBehavior = null,
    Accessor<double>? keyboardIncrement = null,
    Accessor<double>? dragIncrement = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<VectorEventArgs>? onDragStarted = null,
    Action<VectorEventArgs>? onDragDelta = null,
    Action<VectorEventArgs>? onDragCompleted = null
) : HThumbArgs(strStyle, style, baseInteraction, onDragStarted, onDragDelta, onDragCompleted)
{
    public Accessor<bool>? IsShowsPreview = isShowsPreview;
    public Accessor<GridResizeDirection>? ResizeDirection = resizeDirection;
    public Accessor<GridResizeBehavior>? ResizeBehavior = resizeBehavior;
    public Accessor<double>? KeyboardIncrement = keyboardIncrement;
    public Accessor<double>? DragIncrement = dragIncrement;

    public IElement? PreviewContent { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<GridSplitter> HGridSplitter(
        Accessor<bool>? isShowsPreview = null,
        Accessor<GridResizeDirection>? resizeDirection = null,
        Accessor<GridResizeBehavior>? resizeBehavior = null,
        Accessor<double>? keyboardIncrement = null,
        Accessor<double>? dragIncrement = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<VectorEventArgs>? onDragStarted = null,
        Action<VectorEventArgs>? onDragDelta = null,
        Action<VectorEventArgs>? onDragCompleted = null
    ) => HGridSplitter(out _, _ => new(isShowsPreview, resizeDirection, resizeBehavior,
        keyboardIncrement, dragIncrement,
        strStyle, style, baseInteraction,
        onDragStarted, onDragDelta, onDragCompleted)
    );
    
    public static IElement<GridSplitter> HGridSplitter(
        out GridSplitter expose,
        Accessor<bool>? isShowsPreview = null,
        Accessor<GridResizeDirection>? resizeDirection = null,
        Accessor<GridResizeBehavior>? resizeBehavior = null,
        Accessor<double>? keyboardIncrement = null,
        Accessor<double>? dragIncrement = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<VectorEventArgs>? onDragStarted = null,
        Action<VectorEventArgs>? onDragDelta = null,
        Action<VectorEventArgs>? onDragCompleted = null
    ) => HGridSplitter(out expose, _ => new(isShowsPreview, resizeDirection, resizeBehavior,
        keyboardIncrement, dragIncrement,
        strStyle, style, baseInteraction,
        onDragStarted, onDragDelta, onDragCompleted)
    );

    public static IElement<GridSplitter> HGridSplitter(Func<HGridSplitterProps, HGridSplitterArgs> fn
    ) => HGridSplitter(out _, fn);

    public static IElement<GridSplitter> HGridSplitter(out GridSplitter expose,
        Func<HGridSplitterProps, HGridSplitterArgs> fn)
    {
        var splitter = new GridSplitter();
        expose = splitter;

        return Element<GridSplitter>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HGridSplitterProps(uiScope, border, splitter);
            var args = fn(props);
            InternalThumbUtil.SetThumbEffectCore(uiScope, splitter, border, args, ApplyStyle);

            if (args.IsShowsPreview is not null)
            {
                splitter.ShowsPreview = args.IsShowsPreview.Value;
                if (args.IsShowsPreview.IsReactive)
                    uiScope.CreateEffect(epoch => splitter.ShowsPreview = epoch.Track(args.IsShowsPreview));
            }

            if (args.ResizeDirection is not null)
            {
                splitter.ResizeDirection = args.ResizeDirection.Value;
                if (args.ResizeDirection.IsReactive)
                    uiScope.CreateEffect(epoch => splitter.ResizeDirection = epoch.Track(args.ResizeDirection));
            }

            if (args.ResizeBehavior is not null)
            {
                splitter.ResizeBehavior = args.ResizeBehavior.Value;
                if (args.ResizeBehavior.IsReactive)
                    uiScope.CreateEffect(epoch => splitter.ResizeBehavior = epoch.Track(args.ResizeBehavior));
            }

            if (args.KeyboardIncrement is not null)
            {
                splitter.KeyboardIncrement = args.KeyboardIncrement.Value;
                if (args.KeyboardIncrement.IsReactive)
                    uiScope.CreateEffect(epoch => splitter.KeyboardIncrement = epoch.Track(args.KeyboardIncrement));
            }

            if (args.DragIncrement is not null)
            {
                splitter.DragIncrement = args.DragIncrement.Value;
                if (args.DragIncrement.IsReactive)
                    uiScope.CreateEffect(epoch => splitter.DragIncrement = epoch.Track(args.DragIncrement));
            }

            if (args.PreviewContent is not null)
                splitter.PreviewContent = new FuncTemplate<Control>(() =>
                {
                    Control result;
                    using (new ScopeFrame(uiScope))
                    {
                        result = args.PreviewContent.Content;
                    }

                    return result;
                });

            return (splitter, splitter);

            void ApplyStyle(StyleSet styleValue, Layoutable layoutable, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layoutable, bord);
                var orientation = styleValue.Orientation;
                if (orientation is not null)
                {
                    if (orientation == Orientation.Horizontal)
                    {
                        splitter.ResizeDirection = GridResizeDirection.Columns;
                    }
                    else if (orientation == Orientation.Vertical)
                    {
                        splitter.ResizeDirection = GridResizeDirection.Rows;
                    }
                }
            }
        });
    }
}