using System.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HLayoutTransformProps(Scope scope, Border border, LayoutTransformControl layoutTransformControl)
    : BaseComponentProps(scope, border, layoutTransformControl)
{
    public MutSignal<bool> IsUseRenderTransform
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(layoutTransformControl.UseRenderTransform);
            BridgeAvalonia.BindPropertySignal(scope, field, layoutTransformControl,
                LayoutTransformControl.UseRenderTransformProperty);
            return field;
        }
    }

    public MutSignal<ITransform?> LayoutTransform
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<ITransform?>(layoutTransformControl.LayoutTransform);
            BridgeAvalonia.BindPropertySignal(scope, field, layoutTransformControl,
                LayoutTransformControl.LayoutTransformProperty);
            return field;
        }
    }
}

public class HLayoutTransformArgs(
    Accessor<bool>? isUseRenderTransform = null,
    Accessor<ITransform?>? layoutTransform = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<bool>? IsUseRenderTransform = isUseRenderTransform;
    public readonly Accessor<ITransform?>? LayoutTransform = layoutTransform;

    public IElement Content
    {
        init => _children.Add(value);
    }

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
        => _children.Add(element);
}

public static partial class BaseComponent
{
    public static IElement<LayoutTransformControl> HLayoutTransform(
        Accessor<bool>? isUseRenderTransform = null,
        Accessor<ITransform?>? layoutTransform = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HLayoutTransform(out _, _ => new(isUseRenderTransform, layoutTransform, strStyle, style, baseInteraction));
    
    public static IElement<LayoutTransformControl> HLayoutTransform(
        out LayoutTransformControl expose,
        Accessor<bool>? isUseRenderTransform = null,
        Accessor<ITransform?>? layoutTransform = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HLayoutTransform(out expose, _ => new(isUseRenderTransform, layoutTransform, strStyle, style, baseInteraction));

    public static IElement<LayoutTransformControl> HLayoutTransform(Func<HLayoutTransformProps, HLayoutTransformArgs> fn
    ) => HLayoutTransform(out _, fn);

    public static IElement<LayoutTransformControl> HLayoutTransform(out LayoutTransformControl expose,
        Func<HLayoutTransformProps, HLayoutTransformArgs> fn)
    {
        var layoutTransform = new LayoutTransformControl();
        expose = layoutTransform;
        return Element<LayoutTransformControl>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = layoutTransform
            };
            var props = new HLayoutTransformProps(uiScope, border, layoutTransform);
            var args = fn(props);

            layoutTransform.Child = ElementUtil.WrapSingleContainerContent(args).Content;

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, layoutTransform, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(layoutTransform, border, StyleUtil.ApplyStyle);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(layoutTransform, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, layoutTransform);

            if (args.IsUseRenderTransform is not null)
            {
                layoutTransform.UseRenderTransform = args.IsUseRenderTransform.Value;
                if (args.IsUseRenderTransform.IsReactive)
                    uiScope.CreateEffect(scope =>
                        layoutTransform.UseRenderTransform = scope.Track(args.IsUseRenderTransform));
            }

            if (args.LayoutTransform is not null)
            {
                layoutTransform.LayoutTransform = args.LayoutTransform.Value;
                if (args.LayoutTransform.IsReactive)
                    uiScope.CreateEffect(scope =>
                        layoutTransform.LayoutTransform = scope.Track(args.LayoutTransform));
            }

            return (layoutTransform, border);
        });
    }
}