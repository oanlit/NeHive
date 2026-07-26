using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HSelectableText(
        Accessor<string>? text = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<IBrush>? selectionBrush = null,
        Accessor<IBrush>? selectionForegroundBrush = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        Action<RoutedEventArgs>? copyingToClipboard = null)
    {
        return Element.WithScope(uiScope =>
        {
            text ??= "";
            var styleAccessor = StyleParser.ParseFull(strStyle);
            var priorityStyle = style is null ? null : StyleUtil.HStyle2Signal(style);

            var textBlock = new SelectableTextBlock
            {
                TextDecorations = null
            };

            var border = new Border
            {
                Child = textBlock
            };

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                PriorityStyle = priorityStyle,
                StrVariants = styleAccessor.Value.Variants
            };

            state.ApplyAccessorStyle(styleAccessor, textBlock, border, ApplyStyle);
            state.ApplyVariantsStyle(textBlock, border, ApplyStyle);
            ApplySelectionStyle(styleAccessor.Value);
            if (styleAccessor.IsReactive)
            {
                var firstApply = true;
                uiScope.CreateEffect(epoch =>
                {
                    var fullStyle = epoch.Track(styleAccessor);
                    if (firstApply)
                    {
                        firstApply = false;
                        return;
                    }

                    ApplySelectionStyle(fullStyle);
                });
            }

            if (selectionBrush is not null)
            {
                textBlock.SelectionBrush = selectionBrush.Value;
                if (selectionBrush.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionBrush = epoch.Track(selectionBrush));
            }

            if (selectionForegroundBrush is not null)
            {
                textBlock.SelectionForegroundBrush = selectionForegroundBrush.Value;
                if (selectionForegroundBrush.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBlock.SelectionForegroundBrush = epoch.Track(selectionForegroundBrush));
            }

            textBlock.Text = text.Value;
            if (text.IsReactive)
                uiScope.CreateEffect(epoch => textBlock.Text = epoch.Track(text));

            if (selectionStart is not null)
            {
                textBlock.SelectionStart = selectionStart.Value;
                if (selectionStart.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionStart = epoch.Track(selectionStart));
            }

            if (selectionEnd is not null)
            {
                textBlock.SelectionEnd = selectionEnd.Value;
                if (selectionEnd.IsReactive)
                    uiScope.CreateEffect(epoch => textBlock.SelectionEnd = epoch.Track(selectionEnd));
            }

            if (copyingToClipboard is not null)
                textBlock.CopyingToClipboard += (_, e) => copyingToClipboard(e);

            return border;

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
                if (selectionStyle.Background is not null && selectionBrush is null)
                    textBlock.SelectionBrush = selectionStyle.Background;
                if (selectionStyle.Foreground is not null && selectionForegroundBrush is null)
                    textBlock.SelectionForegroundBrush = selectionStyle.Foreground;
            }
        });
    }
}