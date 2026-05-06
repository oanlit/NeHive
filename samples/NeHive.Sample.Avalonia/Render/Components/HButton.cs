using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render.Components;

public class HButtonProp(
    Accessor<string>? text = null,
    Accessor<Thickness>? margin = null,
    Accessor<Thickness>? padding = null,
    Accessor<IBrush>? background = null,
    Accessor<CornerRadius>? cornerRadius = null,
    Accessor<IBrush>? foreground = null,
    Accessor<double>? fontSize = null,
    
    Action<RoutedEventArgs>? click = null
)
{
    public readonly Accessor<string> Text = text ?? "";
    public readonly Accessor<IBrush> Background = background ?? new(Brushes.LightGray);
    public readonly Accessor<IBrush> Foreground = foreground ?? new(Brushes.Black);
    public readonly Accessor<CornerRadius> CornerRadius = cornerRadius ?? new CornerRadius(4);
    public readonly Accessor<Thickness> Padding = padding ?? new Thickness(8, 4);
    public readonly Accessor<Thickness> Margin = margin ?? new Thickness(0);
    public readonly Accessor<double> FontSize = fontSize ?? 12;
    public readonly Action<RoutedEventArgs>? Click = click;
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
            border.Background = prop.Background.Value;
            border.Padding = prop.Padding.Value;
            border.Margin = prop.Margin.Value;
            border.CornerRadius = prop.CornerRadius.Value;
        });

        uiScope.AddEffect(() =>
        {
            textBlock.Text = prop.Text.Value;
            textBlock.Foreground = prop.Foreground.Value;
            textBlock.FontSize = prop.FontSize.Value;
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