using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HTextBlockProp(
    Accessor<string>? text,
    Accessor<Thickness>? margin = null,
    Accessor<TextAlignment>? textAlignment = null,
    Accessor<TextWrapping>? textWrapping = null,
    Accessor<double>? fontSize = null,
    Accessor<FontWeight>? fontWeight = null,
    Accessor<IBrush>? foreground = null
)
{
    public readonly Accessor<string> Text = text ?? "";

    public readonly Accessor<Thickness> Margin
        = margin ?? new Thickness(0);

    public readonly Accessor<TextAlignment> TextAlignment
        = textAlignment ?? global::Avalonia.Media.TextAlignment.Left;

    public readonly Accessor<TextWrapping> TextWrapping
        = textWrapping ?? global::Avalonia.Media.TextWrapping.NoWrap;

    public Accessor<HorizontalAlignment> HorizontalAlignment { get; set; } =
        global::Avalonia.Layout.HorizontalAlignment.Left;

    public readonly Accessor<double>? FontSize = fontSize;
    public readonly Accessor<FontWeight>? FontWeight = fontWeight;
    public readonly Accessor<IBrush>? Foreground = foreground;
}

public static partial class BaseComponent
{
    private static readonly Component<HTextBlockProp> CompTextBlock = new((prop, uiScope) =>
    {
        var tb = new TextBlock();

        uiScope.AddEffect(() =>
        {
            tb.TextAlignment = prop.TextAlignment.Value;
            tb.TextWrapping = prop.TextWrapping.Value;
            tb.Margin = prop.Margin.Value;
            tb.HorizontalAlignment = prop.HorizontalAlignment.Value;
        });

        uiScope.AddEffect(() => tb.Text = prop.Text.Value);
        uiScope.AddEffect(() =>
        {
            if (prop.FontSize?.Value is not null)
                tb.FontSize = prop.FontSize.Value;
            if (prop.FontWeight?.Value is not null)
                tb.FontWeight = prop.FontWeight.Value;
            if (prop.FontWeight?.Value is not null)
                tb.FontWeight = prop.FontWeight.Value;
            if (prop.Foreground?.Value is not null)
                tb.Foreground = prop.Foreground.Value;
        });


        return new Element(uiScope, tb);
    });

    public static IElement HTextBlock(HTextBlockProp prop)
        => CompTextBlock.Create(prop);
}