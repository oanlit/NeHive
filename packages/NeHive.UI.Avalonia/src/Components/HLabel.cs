using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HLabelProps(Scope scope, Border border, Label label) : BaseComponentProps(scope, border, label);

public class HLabelArgs(
    Accessor<string>? text = null,
    Accessor<Control>? target = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null)
    : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<string>? Text = text;
    public readonly Accessor<Control>? Target = target;

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
    public static IElement<Label> HLabel(
        Accessor<string>? text = null,
        Accessor<Control>? target = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HLabel(out _, _ => new(text, target, strStyle, style, baseInteraction));

    public static IElement<Label> HLabel(
        out Label expose,
        Accessor<string>? text = null,
        Accessor<Control>? target = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HLabel(out expose, _ => new(text, target, strStyle, style, baseInteraction));

    public static IElement<Label> HLabel(Func<HLabelProps, HLabelArgs> fn) => HLabel(out _, fn);

    public static IElement<Label> HLabel(out Label expose, Func<HLabelProps, HLabelArgs> fn)
    {
        var label = new Label();
        expose = label;
        return Element<Label>.WithScope(uiScope =>
        {
            var border = new Border();

            var props = new HLabelProps(uiScope, border, label);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            if (args.Count() is 0)
            {
                var textBlock = new TextBlock();

                border.Child = textBlock;

                state.ApplyAccessorStyle(args.StrStyle, textBlock, border,
                    (styleValue, _, bord) => ApplyTextStyle(styleValue, textBlock, bord));
                state.ApplyVariantsStyle(textBlock, border,
                    (styleValue, _, bord) => ApplyTextStyle(styleValue, textBlock, bord));

                if (args.Text is not null)
                {
                    textBlock.Text = args.Text.Value;
                    label.Content = args.Text.Value;
                    if (args.Text.IsReactive)
                        uiScope.CreateEffect(epoch =>
                        {
                            var t = epoch.Track(args.Text);
                            textBlock.Text = t;
                            label.Content = t;
                        });
                }
            }
            else
            {
                var content = ElementUtil.WrapSingleContainerContent(args).Content;
                border.Child = content;

                state.ApplyAccessorStyle(args.StrStyle, content, border, StyleUtil.ApplyStyle);
                state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);
            }

            label.Template = new FuncControlTemplate((_, _) => border);

            if (args.Target is not null)
            {
                label.Target = args.Target.Value;
                if (args.Target.IsReactive)
                    uiScope.CreateEffect(epoch => label.Target = epoch.Track(args.Target));
            }

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(label, args.Popups);
            args.BaseInteraction?.ApplyInteractions(uiScope, label);

            return (label, label);

            void ApplyTextStyle(StyleSet styleValue, TextBlock tb, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, tb, bord);

                if (styleValue.TextAlignment is not null) tb.TextAlignment = styleValue.TextAlignment.Value;
                if (styleValue.VerticalTextAlignment is not null)
                    border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
                if (styleValue.TextWrapping is not null) tb.TextWrapping = styleValue.TextWrapping.Value;
                if (styleValue.Foreground is not null) tb.Foreground = styleValue.Foreground;

                if (styleValue.FontSize is not null) tb.FontSize = styleValue.FontSize.Value;
                if (styleValue.FontWeight is not null) tb.FontWeight = styleValue.FontWeight.Value;
                if (styleValue.FontStyle is not null) tb.FontStyle = styleValue.FontStyle.Value;
                if (styleValue.Foreground is not null) tb.Foreground = styleValue.Foreground;
            }
        });
    }
}