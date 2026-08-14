using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HSelectableTextProps(
    Scope scope,
    Border border,
    SelectableTextBlock textBlock
) : HTextProps(scope, border, textBlock)
{
    public Signal<string> SelectedText
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBlock,
                SelectableTextBlock.SelectedTextProperty, textBlock.SelectedText);
            return field;
        }
    }

    public Signal<bool> CanCopy
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBlock,
                SelectableTextBlock.CanCopyProperty, textBlock.CanCopy);
            return field;
        }
    }

    public MutSignal<int> SelectionStart
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBlock.SelectionStart);
            BridgeAvalonia.BindPropertySignal(scope, field, textBlock, SelectableTextBlock.SelectionStartProperty);
            return field;
        }
    }

    public MutSignal<int> SelectionEnd
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBlock.SelectionEnd);
            BridgeAvalonia.BindPropertySignal(scope, field, textBlock, SelectableTextBlock.SelectionEndProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBlock.SelectionBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, textBlock, SelectableTextBlock.SelectionBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionForegroundBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBlock.SelectionForegroundBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, textBlock,
                SelectableTextBlock.SelectionForegroundBrushProperty);
            return field;
        }
    }
}

public class HSelectableTextArgs(
    Accessor<string>? text,
    MutSignal<string>? fromSelectedText = null,
    Accessor<int>? selectionStart = null,
    Accessor<int>? selectionEnd = null,
    Accessor<IBrush?>? selectionBrush = null,
    Accessor<IBrush?>? selectionForegroundBrush = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onCopyingToClipboard = null
) : HTextArgs(text, strStyle, style, baseInteraction)
{
    public readonly MutSignal<string>? FromSelectedText = fromSelectedText;
    public readonly Accessor<int>? SelectionStart = selectionStart;
    public readonly Accessor<int>? SelectionEnd = selectionEnd;
    public readonly Accessor<IBrush?>? SelectionBrush = selectionBrush;
    public readonly Accessor<IBrush?>? SelectionForegroundBrush = selectionForegroundBrush;
    public readonly Action<RoutedEventArgs>? OnCopyingToClipboard = onCopyingToClipboard;
}

public static partial class BaseComponent
{
    public static IElement<SelectableTextBlock> HSelectableText(
        Accessor<string>? text,
        MutSignal<string>? fromSelectedText = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onCopyingToClipboard = null
    ) => HSelectableText(out _, _ => new(text, fromSelectedText,
        selectionStart, selectionEnd, selectionBrush, selectionForegroundBrush,
        strStyle, style, baseInteraction, onCopyingToClipboard));

    public static IElement<SelectableTextBlock> HSelectableText(
        out SelectableTextBlock expose,
        Accessor<string>? text,
        MutSignal<string>? fromSelectedText = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onCopyingToClipboard = null
    ) => HSelectableText(out expose, _ => new(text, fromSelectedText,
        selectionStart, selectionEnd, selectionBrush, selectionForegroundBrush,
        strStyle, style, baseInteraction, onCopyingToClipboard));

    public static IElement<SelectableTextBlock> HSelectableText(Func<HSelectableTextProps, HSelectableTextArgs> fn
    ) => HSelectableText(out _, fn);

    public static IElement<SelectableTextBlock> HSelectableText(
        out SelectableTextBlock expose,
        Func<HSelectableTextProps, HSelectableTextArgs> fn)
    {
        var textBlock = new SelectableTextBlock
        {
            TextDecorations = null
        };
        expose = textBlock;
        return Element<SelectableTextBlock>.WithScope(uiScope =>
        {
            var border = new Border
            {
                Child = textBlock
            };

            var props = new HSelectableTextProps(uiScope, border, textBlock);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };
            state.ApplyAccessorStyle(args.StrStyle, textBlock, border, ApplyStyle);
            state.ApplyVariantsStyle(textBlock, border, ApplyStyle);

            ApplySelectionStyle(args.StrStyle.Value);
            if (args.StrStyle.IsReactive)
            {
                var firstApply = true;
                uiScope.CreateEffect(epoch =>
                {
                    var fullStyle = epoch.Track(args.StrStyle);
                    if (firstApply)
                    {
                        firstApply = false;
                        return;
                    }

                    ApplySelectionStyle(fullStyle);
                });
            }

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(textBlock, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, textBlock);

            if (args.Text is not null)
            {
                textBlock.Text = args.Text.Value;
                if (args.Text.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.Text = epoch.Track(args.Text));
            }

            if (args.SelectionBrush is not null)
            {
                textBlock.SelectionBrush = args.SelectionBrush.Value;
                if (args.SelectionBrush.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionBrush = epoch.Track(args.SelectionBrush));
            }

            if (args.SelectionForegroundBrush is not null)
            {
                textBlock.SelectionForegroundBrush = args.SelectionForegroundBrush.Value;
                if (args.SelectionForegroundBrush.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBlock.SelectionForegroundBrush = epoch.Track(args.SelectionForegroundBrush));
            }

            if (args.SelectionStart is not null)
            {
                textBlock.SelectionStart = args.SelectionStart.Value;
                if (args.SelectionStart.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionStart = epoch.Track(args.SelectionStart));
            }

            if (args.SelectionEnd is not null)
            {
                textBlock.SelectionEnd = args.SelectionEnd.Value;
                if (args.SelectionEnd.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionEnd = epoch.Track(args.SelectionEnd));
            }

            if (args.FromSelectedText is not null)
                uiScope.CreateEffect(epoch => args.FromSelectedText.RxValue = epoch.Pull(props.SelectedText));

            if (args.OnCopyingToClipboard is not null)
                textBlock.CopyingToClipboard += (_, e) => args.OnCopyingToClipboard(e);

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

            void ApplySelectionStyle(FullStyle fullStyle)
            {
                StyleSet selectionStyle = new();
                var hasSelectionStyle = false;
                if (fullStyle.Variants.TryGetValue("selection", out var selectionStrStyle))
                {
                    StyleParser.Parse(selectionStrStyle, ref selectionStyle);
                    hasSelectionStyle = true;
                }

                if (!hasSelectionStyle) return;
                if (selectionStyle.Background is not null && args.SelectionBrush is null)
                    textBlock.SelectionBrush = selectionStyle.Background;
                if (selectionStyle.Foreground is not null && args.SelectionForegroundBrush is null)
                    textBlock.SelectionForegroundBrush = selectionStyle.Foreground;
            }
        });
    }
}