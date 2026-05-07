using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HButtonStyle(
    Thickness? margin = null,
    Thickness? padding = null,
    IBrush? background = null,
    IBrush? foreground = null,
    double? fontSize = null,
    CornerRadius? cornerRadius = null
)
{
    public Thickness Margin = margin ?? new Thickness(0);
    public Thickness Padding = padding ?? new Thickness(8, 4);
    public IBrush Background = background ?? Brushes.LightGray;
    public IBrush Foreground = foreground ?? Brushes.Black;
    public double FontSize = fontSize ?? 12;
    public CornerRadius CornerRadius = cornerRadius ?? new CornerRadius(4);
    
    public HButtonStyle Merge(HButtonStyle style)
    {
        Margin = style.Margin;
        Padding = style.Padding;
        Background = style.Background;
        Foreground = style.Foreground;
        FontSize = style.FontSize;
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
                result.Padding,
                result.Background,
                result.Foreground,
                result.FontSize,
                result.CornerRadius
            );
        });
    }
}

public class HButtonProp
{
    public readonly Accessor<string> Text;
    public readonly Accessor<HButtonStyle>? Style;
    public readonly Action<RoutedEventArgs>? Click;

    public HButtonProp(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<HButtonStyle>? style = null,
        Action<RoutedEventArgs>? click = null
    )
    {
        Text = text ?? "";
        Click = click;
        if (style != null && strStyle != null)
        {
            Style = new Computed<HButtonStyle>(() =>
                HButtonStyle.Parse(strStyle).Value.Merge(style.Value));
        }
        else if (strStyle != null)
        {
            Style = HButtonStyle.Parse(strStyle);
        }
        else
        {
            Style = style;
        }
    }
}

public class HButtonExpose
{
    public Action<RoutedEventArgs> Click = _ => { };
}

public static partial class BaseComponent
{
    private static readonly Component<HButtonProp, HButtonExpose> CompButton = new((prop, uiScope) =>
    {
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

        uiScope.AddEffect(() =>
        {
            textBlock.Text = prop.Text.Value;
        });
        uiScope.AddEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var style = epochScope.Track(prop.Style);
            border.Background = style.Background;
            border.Padding = style.Padding;
            border.Margin = style.Margin;
            border.CornerRadius = style.CornerRadius;
            
            textBlock.FontSize = style.FontSize;
            textBlock.Foreground = style.Foreground;
        });

        var expose = new HButtonExpose();

        // 鼠标交互状态（悬停、按下等）
        var isPressed = false;

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            prop.Click?.Invoke(args);
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
                border.Background = Brushes.LightGray;
            };

            border.PointerEntered += (_, _) =>
            {
                if (!isPressed)
                    border.Background = Brushes.LightGray; // 恢复（可选悬停变色）
            };
        });

        return new Element<HButtonExpose>(uiScope, border, expose);
    });

    public static IElement<HButtonExpose> HButton(HButtonProp prop)
        => CompButton.Create(prop);

    public static IElement<HButtonExpose> HButton(HButtonProp prop, out IElement<HButtonExpose> expose)
        => CompButton.Create(prop, out expose);
}