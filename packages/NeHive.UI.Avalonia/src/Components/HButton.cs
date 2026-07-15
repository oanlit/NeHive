using System.Collections;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public static class HButtonStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontSize = 12,
        Foreground = Brushes.Black,
        Background = Brushes.LightGray,
        FontWeight = FontWeight.Normal,
        BorderThickness = new Thickness(0),
        CornerRadius = new CornerRadius(0),
        Opacity = 1.0,
        IsVisible = true
    };
}

public class HButtonProp(
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<RoutedEventArgs>? onClick = null) : ISingleChildrenProp
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;
    public readonly Action<RoutedEventArgs>? OnClick = onClick;

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
    public static IElement<Button> HButton(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        var el = HButton(out _, text, strStyle, style, variants, onClick);
        return el;
    }

    public static IElement<Button> HButton(
        out Button exp,
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RoutedEventArgs>? onClick = null)
    {
        var button = new Button();
        exp = button;
        return Element<Button>.WithScope(uiScope =>
        {
            text ??= "";
            var styleAccessor = StyleParser.ParseFull(strStyle, HButtonStyle.DefaultStyleSet, style);

            var textBlock = new TextBlock();
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = textBlock
            };

            button.Template = new FuncControlTemplate((_, _) => border);

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                StrVariants = styleAccessor.Value.Variants,
                Variants = variants
            };

            state.ApplyAccessorStyle(styleAccessor, textBlock, border, ApplyStyle);
            state.ApplyVariantsStyle(textBlock, border, ApplyStyle);

            textBlock.Text = text.Value;
            if (text.IsReactive)
                uiScope.CreateEffect(() => textBlock.Text = text.RxValue);

            if (onClick is not null) button.Click += (_, e) => onClick(e);

            return (button, button);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, textBlock, bord);

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
        });
    }

    public static IElement<Button> HButton(
        out Button exp,
        HButtonProp prop)
    {
        var button = new Button();
        exp = button;
        return Element<Button>.WithScope(uiScope =>
        {
            var content = ElementUtil.WrapSingleContainerContent(prop).Content;
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = content
            };

            button.Template = new FuncControlTemplate((_, _) => border);

            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants,
                Variants = prop.Variants
            };

            state.ApplyAccessorStyle(prop.Style, content, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(content, border, StyleUtil.ApplyStyle);

            if (prop.OnClick is not null) button.Click += (_, e) => prop.OnClick(e);

            return (button, button);
        });
    }

    public static IElement<Button> HButton(HButtonProp prop)
        => HButton(out _, prop);
}