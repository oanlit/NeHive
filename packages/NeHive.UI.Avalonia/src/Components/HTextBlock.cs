using Avalonia.Controls;
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

            if (styleValue.TextAlignment is not null) textBlock.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.VerticalTextAlignment is not null)
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.TextWrapping is not null) textBlock.TextWrapping = styleValue.TextWrapping.Value;
            if (styleValue.Foreground is not null) textBlock.Foreground = styleValue.Foreground;

            if (styleValue.FontSize is not null) textBlock.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) textBlock.Foreground = styleValue.Foreground;
        }
    }
}