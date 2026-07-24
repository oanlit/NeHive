using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HDockPanelProps(Scope scope, Border border, DockPanel dockPanel)
    : BaseComponentProps(scope, border, dockPanel)
{
    public Signal<bool> LastChildFill
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(dockPanel.LastChildFill);
            field = sig;

            dockPanel.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => dockPanel.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == DockPanel.LastChildFillProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
}

public class HDockPanelArgs(
    Accessor<bool>? lastChildFill = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null
) : IEnumerable<(Dock? Dock, IElement Element)>
{
    private readonly List<(Dock? Dock, IElement Element)> _children = [];
    public readonly Accessor<bool> LastChildFill = lastChildFill ?? true;
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

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
    public static IElement<DockPanel> HDockPanel(Func<HDockPanelProps,HDockPanelArgs> fn)
    {
        return Element<DockPanel>.WithScope(uiScope =>
        {
            var dockPanel = new DockPanel();
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