using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;
using NeHive.Core;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HButtonStyle(
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
    public Thickness? Margin = margin;

    public double? Width = width;
    public double? Height = height;
    public double? MinWidth = minWidth;
    public double? MaxWidth = maxWidth;
    public double? MinHeight = minHeight;
    public double? MaxHeight = maxHeight;

    public Thickness Padding = padding ?? new Thickness(8, 4);

    public HorizontalAlignment HorizontalAlignment =
        horizontalAlignment ?? HorizontalAlignment.Left;

    public VerticalAlignment VerticalAlignment =
        verticalAlignment ?? VerticalAlignment.Top;

    public double FontSize = fontSize ?? 12;
    public FontWeight? FontWeight = fontWeight;
    public FontStyle? FontStyle = fontStyle;
    public IBrush Foreground = foreground ?? Brushes.Black;

    public IBrush Background = background ?? Brushes.LightGray;
    public IBrush BorderBrush = borderBrush ?? Brushes.Gray;
    public Thickness BorderThickness = borderThickness ?? new Thickness(1);
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

        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

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
        var result = new StyleSet();
        return new Computed<HButtonStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HButtonStyle(
                result.Margin,
                result.Width,
                result.Height,
                result.MinWidth,
                result.MaxWidth,
                result.MinHeight,
                result.MinHeight,
                result.Padding,
                result.HorizontalAlignment,
                result.VerticalAlignment,
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

    public static Accessor<FullStyle> ParseFull(Accessor<string> text)
    {
        var fullStyle = new FullStyle();

        return new Computed<FullStyle>(() =>
        {
            var str = text.RxValue;
            fullStyle.Base = DefaultStyleSet();
            fullStyle.Variants = [];
            StyleParser.ParseFullStyle(str, ref fullStyle);

            return fullStyle;
        });
    }

    public static StyleSet DefaultStyleSet()
    {
        return new StyleSet
        {
            Padding = new Thickness(8, 4),

            HorizontalAlignment = HorizontalAlignment.Left,

            VerticalAlignment = VerticalAlignment.Top,

            FontSize = 12,

            Foreground = Brushes.Black,

            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
        };
    }
}

public class HButtonExpose
{
    public Action<RoutedEventArgs> Click = _ => { };
}

// public static partial class BaseComponent
// {
//     public static IElement<HButtonExpose> HButton(
//         Accessor<string>? text = null,
//         Accessor<string>? strStyle = null,
//         Accessor<HButtonStyle>? style = null,
//         Action<RoutedEventArgs>? click = null)
//     {
//         text ??= "";
//         if (style is not null && strStyle is not null)
//         {
//             style = new Computed<HButtonStyle>(() =>
//                 HButtonStyle.Parse(strStyle).RxValue.Merge(style.RxValue));
//         }
//         else if (strStyle != null)
//         {
//             style = HButtonStyle.Parse(strStyle);
//         }
//
//         UiScope uiScope = new();
//
//         // 创建基础视觉元素
//         var border = new Border
//         {
//             HorizontalAlignment = HorizontalAlignment.Left,
//         };
//
//         var textBlock = new TextBlock();
//         border.Child = textBlock;
//
//         uiScope.CreateEffect(() => { textBlock.Text = text.RxValue; });
//         if (style is null)
//         {
//             ApplyStyle(new HButtonStyle()); // 应用默认样式
//         }
//         else
//         {
//             uiScope.CreateEffect(epochScope =>
//             {
//                 var styleValue = epochScope.Pull(style);
//                 ApplyStyle(styleValue);
//             });
//         }
//         
//         var expose = new HButtonExpose();
//
//         // 鼠标交互状态（悬停、按下等）
//         var isPressed = false;
//
//         // 统一触发点击的方法
//         void RaiseClick()
//         {
//             var args = new RoutedEventArgs(Button.ClickEvent);
//             click?.Invoke(args);
//             expose.Click.Invoke(args);
//         }
//
//         // 事件挂载
//         uiScope.OnMount(() =>
//         {
//             border.PointerPressed += (_, e) =>
//             {
//                 if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
//                     return;
//
//                 isPressed = true;
//                 border.Background = Brushes.DarkGray; // 按下效果
//                 e.Handled = true;
//             };
//
//             border.PointerReleased += (_, e) =>
//             {
//                 if (isPressed && border.IsPointerOver)
//                 {
//                     RaiseClick();
//                 }
//
//                 isPressed = false;
//                 border.Background = Brushes.LightGray; // 恢复
//                 e.Handled = true;
//             };
//
//             border.PointerExited += (_, _) =>
//             {
//                 isPressed = false; // 移出区域时取消按下状态
//                 border.Background = style?.Value.Background ?? Brushes.LightGray;
//             };
//
//             border.PointerEntered += (_, _) =>
//             {
//                 if (!isPressed)
//                     border.Background = Brushes.LightGray; // 恢复（可选悬停变色）
//             };
//         });
//
//         return new Element<HButtonExpose>(uiScope, border, expose);
//
//         void ApplyStyle(HButtonStyle styleValue)
//         {
//             if (styleValue.Margin is not null) border.Margin = styleValue.Margin.RxValue;
//
//             if (styleValue.Width is not null) border.Width = styleValue.Width.RxValue;
//             if (styleValue.Height is not null) border.Height = styleValue.Height.RxValue;
//             if (styleValue.MaxWidth is not null) border.MaxWidth = styleValue.MaxWidth.RxValue;
//             if (styleValue.MinWidth is not null) border.MinWidth = styleValue.MinWidth.RxValue;
//             if (styleValue.MaxHeight is not null) border.MaxHeight = styleValue.MaxHeight.RxValue;
//             if (styleValue.MinHeight is not null) border.MinHeight = styleValue.MinHeight.RxValue;
//
//             border.Padding = styleValue.Padding;
//
//             textBlock.HorizontalAlignment = styleValue.HorizontalAlignment;
//             textBlock.VerticalAlignment = styleValue.VerticalAlignment;
//
//             textBlock.FontSize = styleValue.FontSize;
//             if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.RxValue;
//             if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.RxValue;
//             textBlock.Foreground = styleValue.Foreground;
//
//             border.Background = styleValue.Background;
//             border.BorderBrush = styleValue.BorderBrush;
//             border.BorderThickness = styleValue.BorderThickness;
//             border.CornerRadius = styleValue.CornerRadius;
//         }
//     }
//
//     public static IElement<HButtonExpose> HButton(
//         out HButtonExpose expose,
//         Accessor<string>? text = null,
//         Accessor<string>? strStyle = null,
//         Accessor<HButtonStyle>? style = null,
//         Action<RoutedEventArgs>? click = null)
//     {
//         var el = HButton(text, strStyle, style, click);
//         expose = el.Expose;
//         return el;
//     }
// }

public static partial class BaseComponent
{
    private class HButtonState(StyleSet baseStyle)
    {
        public StyleSet BaseStyle = baseStyle;
        public StyleSet CurrentStyle = StyleSet.Copy(baseStyle);
        public Dictionary<string, List<string>>? Variants;

        // 鼠标交互状态（悬停、按下等）
        public bool IsHover;
        public bool IsClicked;

        public void ResetSetStyle()
        {
            CurrentStyle.Merge(BaseStyle);
        }

        public void SetCurrentStyle()
        {
            if (Variants == null) return;
            List<string>? strs;
            if (IsHover && Variants.TryGetValue("hover", out strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
            }

            if (IsClicked && Variants.TryGetValue("click", out strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
            }
        }

        public void SetHoverStyle()
        {
            if (Variants == null) return;
            if (IsHover && Variants.TryGetValue("hover", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
            }
        }

        public void SetClickStyle()
        {
            if (Variants == null) return;
            if (IsClicked && Variants.TryGetValue("click", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
            }
        }
    }

    public static IElement<HButtonExpose> HButton(
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null,
        Action<RoutedEventArgs>? click = null)
    {
        text ??= "";
        if (strStyle != null)
        {
            style = HButtonStyle.ParseFull(strStyle);
        }

        UiScope uiScope = new();

        // 创建基础视觉元素
        var textBlock = new TextBlock();
        var border = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = textBlock
        };
        
        uiScope.CreateEffect(() => textBlock.Text = text.RxValue);
        
        HButtonState state;

        if (style is null)
        {
            state = new HButtonState(HButtonStyle.DefaultStyleSet());
            ApplyStyle(state.CurrentStyle); // 应用默认样式
        }
        else
        {
            state = new HButtonState(style.Value.Base);
            ApplyStyle(state.CurrentStyle);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                state.BaseStyle = styleValue.Base;
                state.Variants = styleValue.Variants;
                state.CurrentStyle = StyleSet.Copy(state.BaseStyle);
                ApplyStyle(state.CurrentStyle);
            });
        }

        var expose = new HButtonExpose();
        // 事件挂载
        uiScope.OnMount(() =>
        {
            border.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                    return;

                state.IsClicked = true;
                state.SetClickStyle();
                ApplyStyle(state.CurrentStyle);
                // border.Background = Brushes.DarkGray; // 按下效果
                e.Handled = true;
            };

            border.PointerReleased += (_, e) =>
            {
                if (state.IsClicked && border.IsPointerOver)
                {
                    RaiseClick();
                }

                state.IsClicked = false;
                state.ResetSetStyle();
                state.SetCurrentStyle();
                ApplyStyle(state.CurrentStyle);
                e.Handled = true;
            };

            border.PointerExited += (_, _) =>
            {
                state.IsHover = false;
                state.IsClicked = false; // 移出区域时取消按下状态
                // border.Background = style?.Value.Background ?? Brushes.LightGray;
                state.ResetSetStyle();
                ApplyStyle(state.CurrentStyle);
            };

            border.PointerEntered += (_, _) =>
            {
                state.IsHover = true;
                state.SetHoverStyle();
                ApplyStyle(state.CurrentStyle);
            };
        });

        return new Element<HButtonExpose>(uiScope, border, expose);

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            click?.Invoke(args);
            expose.Click.Invoke(args);
        }

        void ApplyStyle(StyleSet styleValue)
        {
            if (styleValue.Margin is not null) border.Margin = styleValue.Margin.Value;

            if (styleValue.Width is not null) border.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) border.Height = styleValue.Height.Value;
            if (styleValue.MaxWidth is not null) border.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinWidth is not null) border.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxHeight is not null) border.MaxHeight = styleValue.MaxHeight.Value;
            if (styleValue.MinHeight is not null) border.MinHeight = styleValue.MinHeight.Value;

            if (styleValue.Padding is not null) border.Padding = styleValue.Padding.Value;

            if (styleValue.HorizontalAlignment is not null)
                textBlock.HorizontalAlignment = styleValue.HorizontalAlignment.Value;
            if (styleValue.VerticalAlignment is not null)
                textBlock.VerticalAlignment = styleValue.VerticalAlignment.Value;

            if (styleValue.FontSize is not null) textBlock.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
            textBlock.Foreground = styleValue.Foreground;

            border.Background = styleValue.Background;
            border.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness is not null) border.BorderThickness = styleValue.BorderThickness.Value;
            if (styleValue.CornerRadius is not null) border.CornerRadius = styleValue.CornerRadius.Value;
        }
    }

    public static IElement<HButtonExpose> HButton(
        out HButtonExpose expose,
        Accessor<string>? text = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null,
        Action<RoutedEventArgs>? click = null)
    {
        var el = HButton(text, strStyle, style, click);
        expose = el.Expose;
        return el;
    }
}