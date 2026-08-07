using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HDockPanelProps(Scope scope, Border border, DockPanel dockPanel)
    : BaseComponentProps(scope, border, dockPanel)
{
    public MutSignal<bool> IsLastChildFill
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(dockPanel.LastChildFill);
            BridgeAvalonia.BindPropertySignal(scope, field, dockPanel, DockPanel.LastChildFillProperty);
            return field;
        }
    }
}

public class HDockPanelArgs(
    Accessor<bool>? isLastChildFill = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), IEnumerable<(Dock? Dock, IElement Element)>
{
    private readonly List<(Dock? Dock, IElement Element)> _children = [];
    public readonly Accessor<bool> LastChildFill = isLastChildFill ?? true;

    public IElement this[Dock? key]
    {
        set => _children.Add((key, value));
    }

    public void Add(IElement element, Dock dock) => _children.Add((dock, element));

    public IEnumerator<(Dock? Dock, IElement Element)> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<DockPanel> HDockPanel(
        Accessor<bool>? isLastChildFill = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HDockPanel(out _, _ => new(isLastChildFill, strStyle, style, baseInteraction));

    public static IElement<DockPanel> HDockPanel(
        out DockPanel expose,
        Accessor<bool>? isLastChildFill = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HDockPanel(out expose, _ => new(isLastChildFill, strStyle, style, baseInteraction));

    public static IElement<DockPanel> HDockPanel(Func<HDockPanelProps, HDockPanelArgs> fn) => HDockPanel(out _, fn);

    public static IElement<DockPanel> HDockPanel(out DockPanel expose, Func<HDockPanelProps, HDockPanelArgs> fn)
    {
        var dockPanel = new DockPanel();
        expose = dockPanel;
        return Element<DockPanel>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = dockPanel
            };

            var props = new HDockPanelProps(uiScope, border, dockPanel);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, dockPanel, border, ApplyStyle);
            state.ApplyVariantsStyle(dockPanel, border, ApplyStyle);
            args.BaseInteraction?.ApplyInteractions(uiScope, dockPanel);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);

            Control? lastItem = null;
            foreach (var (dock, element) in args)
            {
                var control = element.Content;
                if (dock is null)
                {
                    lastItem = control;
                    continue;
                }

                DockPanel.SetDock(control, dock.Value);
                dockPanel.Children.Add(control);
            }

            dockPanel.LastChildFill = args.LastChildFill.Value;
            if (args.LastChildFill.IsReactive)
            {
                uiScope.CreateEffect(epoch => dockPanel.LastChildFill = epoch.Track(args.LastChildFill));
            }

            if (lastItem is not null) dockPanel.Children.Add(lastItem);

            return (dockPanel, border);

            void ApplyStyle(StyleSet style, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(style, layout, bord);
                if (style.Width is not null)
                    dockPanel.Width = style.Width.Value;

                if (style.Height is not null)
                    dockPanel.Height = style.Height.Value;

                if (style.MinWidth is not null)
                    dockPanel.MinWidth = style.MinWidth.Value;

                if (style.MaxWidth is not null)
                    dockPanel.MaxWidth = style.MaxWidth.Value;

                if (style.MinHeight is not null)
                    dockPanel.MinHeight = style.MinHeight.Value;

                if (style.MaxHeight is not null)
                    dockPanel.MaxHeight = style.MaxHeight.Value;

                if (style.GapY is not null) dockPanel.HorizontalSpacing = style.GapY.Value;
                if (style.GapX is not null) dockPanel.VerticalSpacing = style.GapX.Value;
            }
        });
    }
}