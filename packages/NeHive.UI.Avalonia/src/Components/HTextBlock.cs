using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HTextBlock(
        Accessor<string>? text,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null)
    {
        text ??= "";
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        var uiScope = new UiScope();
        var textBlock = new TextBlock();
        textBlock.TextDecorations = null;
        var border = new Border
        {
            Child = textBlock
        };

        uiScope.CreateEffect(() => textBlock.Text = text.RxValue);

        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                ApplyStyle(styleValue.Normal);
            });
        }


        return new Element(uiScope, border);

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, textBlock, border);

            if (styleValue.LetterSpacing is not null) textBlock.LetterSpacing = styleValue.LetterSpacing.Value;
            if (styleValue.LineHeight is not null) textBlock.LineHeight = styleValue.LineHeight.Value;
            if (styleValue.LineSpacing is not null) textBlock.LineSpacing = styleValue.LineSpacing.Value;

            if (styleValue.MaxLines is not null) textBlock.MaxLines = styleValue.MaxLines.Value;
            if (styleValue.TextTrimming is not null) textBlock.TextTrimming = styleValue.TextTrimming;

            if (styleValue.TextAlignment is not null)
            {
                // textBlock.TextAlignment = styleValue.TextAlignment.Value;
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
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;

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
    }
}