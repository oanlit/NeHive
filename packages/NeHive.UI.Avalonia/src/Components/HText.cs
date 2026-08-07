using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HTextProps(Scope scope, Border border, TextBlock textBlock)
    : BaseComponentProps(scope, border, textBlock);

public class HTextArgs(
    Accessor<string?>? text,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction), ISingleChildrenArgs
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<string?>? Text = text;

    public IEnumerator<IElement> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(IElement element)
        => _children.Add(element);
}

public static partial class BaseComponent
{
    public static IElement<TextBlock> HText(
        Accessor<string?>? text,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HText(out _, _ => new(text, strStyle, style, baseInteraction));
    
    public static IElement<TextBlock> HText(
        out TextBlock expose,
        Accessor<string?>? text,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HText(out expose, _ => new(text, strStyle, style, baseInteraction));

    public static IElement<TextBlock> HText(Func<HTextProps, HTextArgs> fn) => HText(out _, fn);

    public static IElement<TextBlock> HText(
        out TextBlock expose,
        Func<HTextProps, HTextArgs> fn)
    {
        var textBlock = new TextBlock
        {
            TextDecorations = null
        };
        expose = textBlock;
        return Element<TextBlock>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = textBlock
            };

            var props = new HTextProps(uiScope, border, textBlock);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };
            state.ApplyAccessorStyle(args.StrStyle, textBlock, border, ApplyStyle);
            state.ApplyVariantsStyle(textBlock, border, ApplyStyle);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(textBlock, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, textBlock);

            if (args.Text is not null)
            {
                textBlock.Text = args.Text.Value;
                if (args.Text.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.Text = epoch.Track(args.Text));
            }

            return (textBlock, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.LetterSpacing is not null) textBlock.LetterSpacing = styleValue.LetterSpacing.Value;
                if (styleValue.LineHeight is not null) textBlock.LineHeight = styleValue.LineHeight.Value;
                if (styleValue.LineSpacing is not null) textBlock.LineSpacing = styleValue.LineSpacing.Value;

                if (styleValue.MaxLines is not null) textBlock.MaxLines = styleValue.MaxLines.Value;
                if (styleValue.TextTrimming is not null) textBlock.TextTrimming = styleValue.TextTrimming;

                if (styleValue.TextAlignment is not null)
                {
                    switch (styleValue.TextAlignment.Value)
                    {
                        case TextAlignment.Left:
                            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                            break;
                        case TextAlignment.Center:
                            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            break;
                        case TextAlignment.Right:
                            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                            break;
                    }
                }

                if (styleValue.VerticalTextAlignment is not null)
                    textBlock.VerticalAlignment = styleValue.VerticalTextAlignment.Value;

                if (styleValue.TextWrapping is not null) textBlock.TextWrapping = styleValue.TextWrapping.Value;
                if (styleValue.TextDecorations is not null) textBlock.TextDecorations = styleValue.TextDecorations;
                if (styleValue.Inlines is not null) textBlock.Inlines = styleValue.Inlines;

                if (styleValue.FontSize is not null) textBlock.FontSize = styleValue.FontSize.Value;
                if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
                if (styleValue.FontFamily is not null) textBlock.FontFamily = styleValue.FontFamily;
                if (styleValue.FontStretch is not null) textBlock.FontStretch = styleValue.FontStretch.Value;
                if (styleValue.FontFeatures is not null) textBlock.FontFeatures = styleValue.FontFeatures;
                if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
                if (styleValue.Foreground is not null) textBlock.Foreground = styleValue.Foreground;
            }
        });
    }
}