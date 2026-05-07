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
        => new Computed<HTextStyle>(() => Parse(text.Value));

    public static Accessor<HTextStyle>? ParseNullable(Accessor<string>? text)
        => text is null
            ? null
            : new Computed<HTextStyle>(() => Parse(text.Value));

    private static HTextStyle Parse(string text)
    {
        // Thickness? margin = null;
        // TextAlignment? textAlignment = null;
        // TextWrapping? textWrapping = null;
        // double? fontSize = null;
        // FontWeight? fontWeight = null;
        // IBrush? foreground = null;
        //
        // var tokens = text.Split(
        //     [' ', '\n', '\r', '\t'],
        //     StringSplitOptions.RemoveEmptyEntries |
        //     StringSplitOptions.TrimEntries);
        //
        // foreach (var token in tokens)
        // {
        //     // text-18
        //     if (token.StartsWith("text-"))
        //     {
        //         var value = token["text-".Length..];
        //
        //         // text-center
        //         textAlignment = value switch
        //         {
        //             "left" => TextAlignment.Left,
        //             "center" => TextAlignment.Center,
        //             "right" => TextAlignment.Right,
        //             "justify" => TextAlignment.Justify,
        //             _ => textAlignment
        //         };
        //
        //         // text-18
        //         if (double.TryParse(value, out var fs))
        //         {
        //             fontSize = fs;
        //         }
        //
        //         continue;
        //     }
        //
        //     // font-bold
        //     if (token.StartsWith("font-"))
        //     {
        //         var value = token["font-".Length..];
        //
        //         fontWeight = value switch
        //         {
        //             "bold" => global::Avalonia.Media.FontWeight.Bold,
        //             "light" => global::Avalonia.Media.FontWeight.Light,
        //             "normal" => global::Avalonia.Media.FontWeight.Normal,
        //             _ => fontWeight
        //         };
        //
        //         continue;
        //     }
        //
        //     // fg-red
        //     if (token.StartsWith("fg-"))
        //     {
        //         var value = token["fg-".Length..];
        //
        //         foreground = value switch
        //         {
        //             "red" => Brushes.Red,
        //             "green" => Brushes.Green,
        //             "blue" => Brushes.Blue,
        //             "white" => Brushes.White,
        //             "black" => Brushes.Black,
        //             "gray" => Brushes.Gray,
        //             "darkslateblue" => Brushes.DarkSlateBlue,
        //             "darkgreen" => Brushes.DarkGreen,
        //             _ => foreground
        //         };
        //
        //         continue;
        //     }
        //
        //     // wrap
        //     if (token == "wrap")
        //     {
        //         textWrapping = TextWrapping.Wrap;
        //         continue;
        //     }
        //
        //     // nowrap
        //     if (token == "nowrap")
        //     {
        //         textWrapping = TextWrapping.NoWrap;
        //         continue;
        //     }
        //
        //     // m-12
        //     if (token.StartsWith("m-"))
        //     {
        //         var value = token["m-".Length..];
        //
        //         if (double.TryParse(value, out var m))
        //         {
        //             margin = new Thickness(m);
        //         }
        //     }
        // }
        
        var result = StyleParser.Parse(text);

        return new HTextStyle(
            result.Margin,
            result.TextAlignment,
            result.TextWrapping,
            result.FontSize,
            result.FontWeight,
            result.Foreground
        );
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

        uiScope.AddEffect(() => tb.Text = prop.Text.Value);
        uiScope.AddEffect(epochScope =>
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