using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HStackPanelProps(
    Scope scope,
    Border border,
    Panel content) : HPanelProps(scope, border, content);

public class HStackPanelArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null) : HPanelArgs(strStyle, style, baseInteraction);

public static partial class BaseComponent
{
    public static IElement<StackPanel> HStackPanel(
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HStackPanel(out _, _ => new(strStyle, style, baseInteraction));

    public static IElement<StackPanel> HStackPanel(
        out StackPanel expose,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null) =>
        HStackPanel(out expose, _ => new(strStyle, style, baseInteraction));

    public static IElement<StackPanel> HStackPanel(Func<HStackPanelProps, HStackPanelArgs> fn) =>
        HStackPanel(out _, fn);

    public static IElement<StackPanel> HStackPanel(out StackPanel expose, Func<HStackPanelProps, HStackPanelArgs> fn)
    {
        var panel = new StackPanel();
        expose = panel;
        return Element<StackPanel>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = panel
            };

            var props = new HStackPanelProps(uiScope, border, panel);
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

            args.BaseInteraction?.ApplyInteractions(uiScope, panel);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            return (panel, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                var orientation = styleValue.Orientation ?? Orientation.Vertical;
                panel.Orientation = orientation;

                switch (orientation)
                {
                    case Orientation.Vertical:
                        if (styleValue.GapY is not null) panel.Spacing = styleValue.GapY.Value;
                        break;
                    case Orientation.Horizontal:
                        if (styleValue.GapX is not null) panel.Spacing = styleValue.GapX.Value;
                        break;
                }
            }
        });
    }
}