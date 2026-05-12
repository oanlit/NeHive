using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render.Styles;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HButtonStyle(
    Thickness? margin = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    Thickness? padding = null,
    double? fontSize = null,
    FontWeight? fontWeight = null,
    FontStyle? fontStyle = null,
    IBrush? foreground = null,
    IBrush? background = null,
    IBrush? borderBrush = null,
    Thickness? borderThickness = null,
    CornerRadius? cornerRadius = null
)
{
    public Thickness Margin = margin ?? new Thickness(0);

    public double? Width = width;
    public double? Height = height;
    public double? MinWidth = minWidth;
    public double? MaxWidth = maxWidth;
    public double? MinHeight = minHeight;
    public double? MaxHeight = maxHeight;

    public Thickness Padding = padding ?? new Thickness(8, 4);

    public double FontSize = fontSize ?? 12;
    public FontWeight? FontWeight = fontWeight;
    public FontStyle? FontStyle = fontStyle;
    public IBrush Foreground = foreground ?? Brushes.Black;

    public IBrush Background = background ?? Brushes.LightGray;
    public IBrush? BorderBrush = borderBrush;
    public Thickness? BorderThickness = borderThickness;
    public CornerRadius CornerRadius = cornerRadius ?? new CornerRadius(4);

    public HButtonStyle Merge(HButtonStyle style)
    {
        Margin = style.Margin;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        Padding = style.Padding;

        FontSize = style.FontSize;
        FontWeight = style.FontWeight;
        FontStyle = style.FontStyle;
        Foreground = style.Foreground;

        Background = style.Background;
        BorderBrush = style.BorderBrush;
        BorderThickness = style.BorderThickness;
        CornerRadius = style.CornerRadius;
        return this;
    }

    public static Accessor<HButtonStyle> Parse(Accessor<string> text)
    {
        return new Computed<HButtonStyle>(() =>
        {
            var str = text.Value;
            var result = StyleParser.Parse(str);
            return new HButtonStyle(
                result.Margin,
                result.Width,
                result.Height,
                result.MinWidth,
                result.MaxWidth,
                result.MinHeight,
                result.MinHeight,
                result.Padding,
                result.FontSize,
                result.FontWeight,
                result.FontStyle,
                result.Foreground,
                result.Background,
                result.BorderBrush,
                result.BorderThickness,
                result.CornerRadius
            );
        });
    }
}

public class HButtonExpose
{
    public Action<RoutedEventArgs> Click = _ => { };
}

public static partial class BaseComponent
{
    public static IElement<HButtonExpose> HButton(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<HButtonStyle>? style = null,
        Action<RoutedEventArgs>? click = null)
    {
        text ??= "";
        if (style is not null && strStyle is not null)
        {
            style = new Computed<HButtonStyle>(() =>
                HButtonStyle.Parse(strStyle).Value.Merge(style.Value));
        }
        else if (strStyle != null)
        {
            style = HButtonStyle.Parse(strStyle);
        }

        UiScope uiScope = new();

        // 创建基础视觉元素
        var border = new Border
        {
            // 设置默认样式
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 4)
        };

        var textBlock = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        border.Child = textBlock;

        uiScope.CreateEffect(() => { textBlock.Text = text.Value; });
        uiScope.CreateEffect(epochScope =>
        {
            if (style is null) return;
            var styleValue = epochScope.Pull(style);

            border.Margin = styleValue.Margin;

            if (styleValue.Width is not null) border.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) border.Height = styleValue.Height.Value;
            if (styleValue.MaxWidth is not null) border.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinWidth is not null) border.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxHeight is not null) border.MaxHeight = styleValue.MaxHeight.Value;
            if (styleValue.MinHeight is not null) border.MinHeight = styleValue.MinHeight.Value;

            border.Padding = styleValue.Padding;

            textBlock.FontSize = styleValue.FontSize;
            if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
            textBlock.Foreground = styleValue.Foreground;

            border.Background = styleValue.Background;
            if (styleValue.BorderBrush is not null) border.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness is not null) border.BorderThickness = styleValue.BorderThickness.Value;
            border.CornerRadius = styleValue.CornerRadius;
        });

        var expose = new HButtonExpose();

        // 鼠标交互状态（悬停、按下等）
        var isPressed = false;

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            click?.Invoke(args);
            expose.Click.Invoke(args);
        }

        // 事件挂载
        uiScope.OnMount(() =>
        {
            border.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                    return;

                isPressed = true;
                border.Background = Brushes.DarkGray; // 按下效果
                e.Handled = true;
            };

            border.PointerReleased += (_, e) =>
            {
                if (isPressed && border.IsPointerOver)
                {
                    RaiseClick();
                }

                isPressed = false;
                border.Background = Brushes.LightGray; // 恢复
                e.Handled = true;
            };

            border.PointerExited += (_, _) =>
            {
                isPressed = false; // 移出区域时取消按下状态
                border.Background = style?.UntrackValue.Background ?? Brushes.LightGray;
            };

            border.PointerEntered += (_, _) =>
            {
                if (!isPressed)
                    border.Background = Brushes.LightGray; // 恢复（可选悬停变色）
            };
        });

        return new Element<HButtonExpose>(uiScope, border, expose);
    }

    public static IElement<HButtonExpose> HButton(
        out HButtonExpose expose,
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<HButtonStyle>? style = null,
        Action<RoutedEventArgs>? click = null)
    {
        var el = HButton(text, strStyle, style, click);
        expose = el.Expose;
        return el;
    }
}