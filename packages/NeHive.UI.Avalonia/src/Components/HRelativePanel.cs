using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public record RelativePosition(
    Control? AlignLeftWith = null,
    Control? AlignRightWith = null,
    Control? AlignTopWith = null,
    Control? AlignBottomWith = null,
    Control? AlignHorizontalCenterWith = null,
    Control? AlignVerticalCenterWith = null,
    Control? LeftOf = null,
    Control? RightOf = null,
    Control? Above = null,
    Control? Below = null,
    bool? AlignLeftWithPanel = null,
    bool? AlignRightWithPanel = null,
    bool? AlignTopWithPanel = null,
    bool? AlignBottomWithPanel = null,
    bool? AlignHorizontalCenterWithPanel = null,
    bool? AlignVerticalCenterWithPanel = null
);

public class HRelativePanelArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), IEnumerable<(RelativePosition?, IElement)>
{
    private readonly List<(RelativePosition?, IElement)> _children = [];

    public IElement this[RelativePosition key]
    {
        set => _children.Add((key, value));
    }

    public IElement this[Control? alignLeftWith = null,
        Control? alignRightWith = null,
        Control? alignTopWith = null,
        Control? alignBottomWith = null,
        Control? alignHorizontalCenterWith = null,
        Control? alignVerticalCenterWith = null,
        Control? leftOf = null,
        Control? rightOf = null,
        Control? above = null,
        Control? below = null,
        bool? alignLeftWithPanel = null,
        bool? alignRightWithPanel = null,
        bool? alignTopWithPanel = null,
        bool? alignBottomWithPanel = null,
        bool? alignHorizontalCenterWithPanel = null,
        bool? alignVerticalCenterWithPanel = null]
    {
        set
        {
            var relativePosition = new RelativePosition(alignLeftWith, alignRightWith, alignTopWith, alignBottomWith,
                alignHorizontalCenterWith, alignVerticalCenterWith, leftOf, rightOf, above, below, alignLeftWithPanel,
                alignRightWithPanel, alignTopWithPanel, alignBottomWithPanel, alignHorizontalCenterWithPanel,
                alignVerticalCenterWithPanel);
            _children.Add((relativePosition, value));
        }
    }

    public void Add(IElement element)
    {
        _children.Add((null, element));
    }

    public IEnumerator<(RelativePosition?, IElement)> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<RelativePanel> HRelativePanel(
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HRelativePanel(out _, _ => new(strStyle, style, baseInteraction));
    
    public static IElement<RelativePanel> HRelativePanel(
        out RelativePanel expose,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HRelativePanel(out expose, _ => new(strStyle, style, baseInteraction));

    public static IElement<RelativePanel> HRelativePanel(Func<HPanelProps, HRelativePanelArgs> fn
    ) => HRelativePanel(out _, fn);

    public static IElement<RelativePanel> HRelativePanel(out RelativePanel expose,
        Func<HPanelProps, HRelativePanelArgs> fn)
    {
        var panel = new RelativePanel();
        expose = panel;
        return Element<RelativePanel>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = panel
            };

            var props = new HPanelProps(uiScope, border, panel);
            var args = fn(props);

            foreach (var (relative, child) in args)
            {
                if (relative is not null)
                {
                    if (relative.AlignLeftWith is not null)
                        RelativePanel.SetAlignLeftWith(child.Content, relative.AlignLeftWith);
                    if (relative.AlignRightWith is not null)
                        RelativePanel.SetAlignRightWith(child.Content, relative.AlignRightWith);
                    if (relative.AlignTopWith is not null)
                        RelativePanel.SetAlignTopWith(child.Content, relative.AlignTopWith);
                    if (relative.AlignBottomWith is not null)
                        RelativePanel.SetAlignBottomWith(child.Content, relative.AlignBottomWith);
                    if (relative.AlignHorizontalCenterWith is not null)
                        RelativePanel.SetAlignHorizontalCenterWith(child.Content, relative.AlignHorizontalCenterWith);
                    if (relative.AlignVerticalCenterWith is not null)
                        RelativePanel.SetAlignVerticalCenterWith(child.Content, relative.AlignVerticalCenterWith);
                    if (relative.LeftOf is not null)
                        RelativePanel.SetLeftOf(child.Content, relative.LeftOf);
                    if (relative.RightOf is not null)
                        RelativePanel.SetRightOf(child.Content, relative.RightOf);
                    if (relative.Above is not null)
                        RelativePanel.SetAbove(child.Content, relative.Above);
                    if (relative.Below is not null)
                        RelativePanel.SetBelow(child.Content, relative.Below);
                    if (relative.AlignLeftWithPanel is not null)
                        RelativePanel.SetAlignLeftWithPanel(child.Content, relative.AlignLeftWithPanel.Value);
                    if (relative.AlignRightWithPanel is not null)
                        RelativePanel.SetAlignRightWithPanel(child.Content, relative.AlignRightWithPanel.Value);
                    if (relative.AlignTopWithPanel is not null)
                        RelativePanel.SetAlignTopWithPanel(child.Content, relative.AlignTopWithPanel.Value);
                    if (relative.AlignBottomWithPanel is not null)
                        RelativePanel.SetAlignBottomWithPanel(child.Content, relative.AlignBottomWithPanel.Value);
                    if (relative.AlignHorizontalCenterWithPanel is not null)
                        RelativePanel.SetAlignHorizontalCenterWithPanel(child.Content,
                            relative.AlignHorizontalCenterWithPanel.Value);
                    if (relative.AlignVerticalCenterWithPanel is not null)
                        RelativePanel.SetAlignVerticalCenterWithPanel(child.Content,
                            relative.AlignVerticalCenterWithPanel.Value);
                }

                panel.Children.Add(child.Content);
            }

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, panel, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(panel, border, StyleUtil.ApplyStyle);

            if (args.BaseInteraction is not null)
                args.BaseInteraction.ApplyInteractions(uiScope, panel);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);


            return (panel, border);
        });
    }
}