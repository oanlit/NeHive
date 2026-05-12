using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render.Styles;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HTextStyle(
    Thickness? margin = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    Thickness? padding = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    TextAlignment? textAlignment = null,
    TextWrapping? textWrapping = null,
    double? fontSize = null,
    FontWeight? fontWeight = null,
    FontStyle? fontStyle = null,
    IBrush? foreground = null
)
{
    public Thickness Margin { get; private set; } = margin ?? new Thickness(0);

    public double? Width { get; private set; } = width;
    public double? Height { get; private set; } = height;
    public double? MinWidth { get; private set; } = minWidth;
    public double? MaxWidth { get; private set; } = maxWidth;
    public double? MinHeight { get; private set; } = minHeight;
    public double? MaxHeight { get; private set; } = maxHeight;

    public Thickness? Padding { get; private set; } = padding;

    public HorizontalAlignment HorizontalAlignment { get; private set; } =
        horizontalAlignment ?? HorizontalAlignment.Left;

    public VerticalAlignment VerticalAlignment { get; private set; } =
        verticalAlignment ?? VerticalAlignment.Top;

    public TextAlignment TextAlignment { get; private set; } =
        textAlignment ?? TextAlignment.Left;

    public TextWrapping TextWrapping { get; private set; } =
        textWrapping ?? TextWrapping.NoWrap;

    public double? FontSize { get; private set; } = fontSize;
    public FontWeight? FontWeight { get; private set; } = fontWeight;
    public FontStyle? FontStyle { get; private set; } = fontStyle;
    public IBrush? Foreground { get; private set; } = foreground;

    public HTextStyle Merge(
        Thickness? margin = null,
        double? width = null,
        double? height = null,
        double? minWidth = null,
        double? maxWidth = null,
        double? minHeight = null,
        double? maxHeight = null,
        Thickness? padding = null,
        HorizontalAlignment? horizontalAlignment = null,
        VerticalAlignment? verticalAlignment = null,
        TextAlignment? textAlignment = null,
        TextWrapping? textWrapping = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        IBrush? foreground = null
    )
    {
        if (margin is not null) Margin = margin.Value;

        if (width is not null) Width = width;
        if (height is not null) Height = height;
        if (minWidth is not null) MinWidth = minWidth;
        if (maxWidth is not null) MaxWidth = maxWidth;
        if (minHeight is not null) MinHeight = minHeight;
        if (maxHeight is not null) MaxHeight = maxHeight;

        if (padding is not null) Padding = padding;

        if (horizontalAlignment is not null) HorizontalAlignment = horizontalAlignment.Value;
        if (verticalAlignment is not null) VerticalAlignment = verticalAlignment.Value;

        if (textAlignment is not null) TextAlignment = textAlignment.Value;
        if (textWrapping is not null) TextWrapping = textWrapping.Value;

        if (fontSize is not null) FontSize = fontSize.Value;
        if (fontWeight is not null) FontWeight = fontWeight.Value;
        if (fontStyle is not null) FontStyle = fontStyle;
        if (foreground is not null) Foreground = foreground;
        return this;
    }

    public HTextStyle Merge(HTextStyle style)
    {
        Margin = style.Margin;

        HorizontalAlignment = style.HorizontalAlignment;

        TextAlignment = style.TextAlignment;
        TextWrapping = style.TextWrapping;

        FontSize = style.FontSize;
        FontWeight = style.FontWeight;
        FontStyle = style.FontStyle;
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
                result.Width,
                result.Height,
                result.MinWidth,
                result.MaxWidth,
                result.MinHeight,
                result.MaxHeight,
                result.Padding,
                result.HorizontalAlignment,
                result.VerticalAlignment,
                result.TextAlignment,
                result.TextWrapping,
                result.FontSize,
                result.FontWeight,
                result.FontStyle,
                result.Foreground
            );
        });
    }
}

public static partial class BaseComponent
{
    public static IElement HTextBlock(
        Accessor<string>? text,
        Accessor<string>? strStyle = null,
        Accessor<HTextStyle>? style = null)
    {
        text ??= "";
        if (style is not null && strStyle is not null)
        {
            style = new Computed<HTextStyle>(() =>
                HTextStyle.Parse(strStyle).Value.Merge(style.Value));
        }
        else if (strStyle is not null)
        {
            style = HTextStyle.Parse(strStyle);
        }

        var uiScope = new UiScope();
        var tb = new TextBlock();

        uiScope.CreateEffect(() => tb.Text = text.Value);
        uiScope.CreateEffect(epochScope =>
        {
            if (style is null) return;
            var styleValue = epochScope.Pull(style);
            ApplyStyle(styleValue);
        });

        return new Element(uiScope, tb);

        void ApplyStyle(HTextStyle styleValue)
        {
            tb.Margin = styleValue.Margin;

            if (styleValue.Width.HasValue) tb.Width = styleValue.Width.Value;
            if (styleValue.Height.HasValue) tb.Width = styleValue.Height.Value;
            if (styleValue.MinWidth.HasValue) tb.Width = styleValue.MinWidth.Value;
            if (styleValue.MaxWidth.HasValue) tb.Width = styleValue.MaxWidth.Value;
            if (styleValue.MinHeight.HasValue) tb.Width = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight.HasValue) tb.Width = styleValue.MaxHeight.Value;

            if (styleValue.Padding is not null) tb.Padding = styleValue.Padding.Value;
            // tb.LetterSpacing

            tb.HorizontalAlignment = styleValue.HorizontalAlignment;
            tb.VerticalAlignment = styleValue.VerticalAlignment;

            tb.TextAlignment = styleValue.TextAlignment;
            tb.TextWrapping = styleValue.TextWrapping;

            if (styleValue.FontSize.HasValue) tb.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight.HasValue) tb.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle.HasValue) tb.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) tb.Foreground = styleValue.Foreground;
        }
    }
}