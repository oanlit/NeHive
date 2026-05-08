using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HTextStyle(
    Thickness? margin = null,
    TextAlignment? textAlignment = null,
    TextWrapping? textWrapping = null,
    double? fontSize = null,
    FontWeight? fontWeight = null,
    IBrush? foreground = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);
    public TextAlignment TextAlignment { get; private set; } = textAlignment ?? TextAlignment.Left;
    public TextWrapping TextWrapping { get; private set; } = textWrapping ?? TextWrapping.NoWrap;

    public double? FontSize { get; private set; } = fontSize;
    public FontWeight? FontWeight { get; private set; } = fontWeight;

    public IBrush? Foreground { get; private set; } = foreground;

    public HTextStyle Merge(
        Thickness? margin = null,
        TextAlignment? textAlignment = null,
        TextWrapping? textWrapping = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        IBrush? foreground = null
    )
    {
        if (margin is not null)
            Margin = margin.Value;
        if (textAlignment is not null)
            TextAlignment = textAlignment.Value;
        if (textWrapping is not null)
            TextWrapping = textWrapping.Value;
        if (fontSize is not null)
            FontSize = fontSize.Value;
        if (fontWeight is not null)
            FontWeight = fontWeight.Value;
        if (foreground is not null)
            Foreground = foreground;
        return this;
    }

    public HTextStyle Merge(HTextStyle style)
    {
        Margin = style.Margin;
        TextAlignment = style.TextAlignment;
        TextWrapping = style.TextWrapping;
        FontSize = style.FontSize;
        FontWeight = style.FontWeight;
        Foreground = style.Foreground;
        return this;
    }

    public static HTextStyle Default => new();

    public static Accessor<HTextStyle> Parse(Accessor<string> text)
    {
        return new Computed<HTextStyle>(() =>
        {
            var str = text.Value;
            var result = StyleParser.Parse(str);
            return new HTextStyle(
                result.Margin,
                result.TextAlignment,
                result.TextWrapping,
                result.FontSize,
                result.FontWeight,
                result.Foreground
            );
        });
    }
}

public class HTextBlockProp
{
    public readonly Accessor<string> Text;
    public readonly Accessor<HTextStyle>? Style;

    public Accessor<HorizontalAlignment> HorizontalAlignment { get; set; } =
        global::Avalonia.Layout.HorizontalAlignment.Left;

    public HTextBlockProp(
        Accessor<string>? text,
        Accessor<string>? strStyle = null,
        Accessor<HTextStyle>? style = null
    )
    {
        Text = text ?? "";
        if (style is not null && strStyle is not null)
        {
            Style = new Computed<HTextStyle>(() =>
                HTextStyle.Parse(strStyle).Value.Merge(style.Value));
        }
        else if (strStyle is not null)
        {
            Style = HTextStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }
}

public static partial class BaseComponent
{
    private static readonly Component<HTextBlockProp> CompTextBlock = new((prop, uiScope) =>
    {
        var tb = new TextBlock();

        uiScope.CreateEffect(() => tb.Text = prop.Text.Value);
        uiScope.CreateEffect(epochScope =>
        {
            if (prop.Style is null) return;
            var style = epochScope.Track(prop.Style);

            tb.TextAlignment = style.TextAlignment;
            tb.TextWrapping = style.TextWrapping;
            tb.Margin = style.Margin;
            tb.HorizontalAlignment = prop.HorizontalAlignment.Value;

            var fontSize = style.FontSize;
            if (fontSize.HasValue)
                tb.FontSize = fontSize.Value;

            var fontWeight = style.FontWeight;
            if (fontWeight.HasValue)
                tb.FontWeight = fontWeight.Value;

            var foreground = style.Foreground;
            if (foreground is not null)
                tb.Foreground = foreground;
        });

        return new Element(uiScope, tb);
    });

    public static IElement HTextBlock(HTextBlockProp prop)
        => CompTextBlock.Create(prop);
}