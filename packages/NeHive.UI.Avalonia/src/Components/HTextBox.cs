using Avalonia.Controls;
using Avalonia.Input;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HTextBox(MutSignal<string>? bindText = null,
        Accessor<string>? text = null,
        Accessor<string>? watermark = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<int>? maxLength = null,
        Accessor<bool>? acceptsReturn = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Action<string>? textChanged = null,
        Action? lostFocus = null,
        Action? gotFocus = null,
        Action<KeyEventArgs>? keyDown = null)
    {
        var uiScope = new UiScope();
        var textBox = new TextBox();

        // 样式合并
        Accessor<StyleSet>? finalStyle = null;
        if (strStyle != null || style != null)
        {
            if (style != null && strStyle != null)
            {
                finalStyle = new Computed<StyleSet>(() =>
                {
                    var ss = StyleParser.Parse(strStyle.RxValue);
                    ss.Merge(style.RxValue);
                    return ss;
                });
            }
            else if (strStyle != null)
            {
                var result = new StyleSet();
                finalStyle = new Computed<StyleSet>(() =>
                {
                    StyleParser.Parse(strStyle.RxValue, ref result);
                    return result;
                });
            }
            else
            {
                finalStyle = style;
            }
        }

        // 应用样式
        uiScope.CreateEffect(scope =>
        {
            if (finalStyle == null) return;
            var styleValue = scope.Track(finalStyle);
            ApplyStyle(styleValue);
        });

        // 处理 Watermark (Avalonia TextBox 有 Watermark 属性)
        uiScope.CreateEffect(scope =>
        {
            if (watermark == null)
                return;

            var wm = scope.Track(watermark);
            textBox.Watermark = wm;
        });

        // 处理只读
        uiScope.CreateEffect(scope =>
        {
            if (isReadOnly != null)
                textBox.IsReadOnly = scope.Track(isReadOnly);
        });

        // 处理最大长度
        uiScope.CreateEffect(scope =>
        {
            if (maxLength != null)
                textBox.MaxLength = scope.Track(maxLength);
        });

        // 处理多行
        uiScope.CreateEffect(scope =>
        {
            if (acceptsReturn != null)
                textBox.AcceptsReturn = scope.Track(acceptsReturn);
        });

        // 处理双向/单向绑定
        if (bindText != null)
        {
            // 双向绑定：UI 编辑时更新 MutSignal，且 MutSignal 变化时更新 UI
            var signal = bindText;
            // 避免循环更新标志
            var updatingFromSignal = false;

            textBox.Text = signal.RxValue;

            uiScope.OnMount += () =>
            {
                textBox.TextChanged += (_, _) =>
                {
                    if (updatingFromSignal) return;
                    var newText = textBox.Text ?? string.Empty;
                    signal.RxValue = newText;
                    textChanged?.Invoke(newText);
                };
            };

            uiScope.CreateEffect(scope =>
            {
                var val = scope.Pull(signal);
                if (textBox.Text == val)
                    return;

                updatingFromSignal = true;
                textBox.Text = val;
                updatingFromSignal = false;
            });
        }
        else if (text != null)
        {
            // 单向更新
            uiScope.CreateEffect(scope =>
            {
                var val = scope.Track(text);
                if (textBox.Text != val)
                    textBox.Text = val;
                textChanged?.Invoke(val);
            });
        }

        // 注册事件
        uiScope.OnMount+=() =>
        {
            if (lostFocus != null)
                textBox.LostFocus += (_, _) => lostFocus();
            if (gotFocus != null)
                textBox.GotFocus += (_, _) => gotFocus();
            if (keyDown != null)
                textBox.KeyDown += (_, e) => keyDown(e);
        };

        // 这里没有子元素，所以不需要 ISingleChildrenProp 或集合初始化器
        return new Element(uiScope, textBox);

        void ApplyStyle(StyleSet styleValue)
        {
            if (styleValue.Margin.HasValue) textBox.Margin = styleValue.Margin.Value;

            if (styleValue.Width.HasValue) textBox.Width = styleValue.Width.Value;
            if (styleValue.Height.HasValue) textBox.Height = styleValue.Height.Value;
            if (styleValue.MinWidth.HasValue) textBox.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxWidth.HasValue) textBox.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinHeight.HasValue) textBox.MinHeight = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight.HasValue) textBox.MaxHeight = styleValue.MaxHeight.Value;

            if (styleValue.Padding.HasValue) textBox.Padding = styleValue.Padding.Value;

            if (styleValue.HorizontalAlignment.HasValue)
                textBox.HorizontalAlignment = styleValue.HorizontalAlignment.Value;
            if (styleValue.VerticalAlignment.HasValue)
                textBox.VerticalAlignment = styleValue.VerticalAlignment.Value;

            if (styleValue.TextAlignment.HasValue)
                textBox.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.VerticalTextAlignment.HasValue)
                textBox.VerticalContentAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.TextWrapping.HasValue)
                textBox.TextWrapping = styleValue.TextWrapping.Value;

            if (styleValue.FontSize.HasValue) textBox.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight.HasValue) textBox.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle.HasValue) textBox.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) textBox.Foreground = styleValue.Foreground;

            if (styleValue.Background is not null) textBox.Background = styleValue.Background;
            if (styleValue.BorderBrush is not null) textBox.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness.HasValue) textBox.BorderThickness = styleValue.BorderThickness.Value;
            if (styleValue.CornerRadius.HasValue) textBox.CornerRadius = styleValue.CornerRadius.Value;

            if (styleValue.Opacity.HasValue) textBox.Opacity = styleValue.Opacity.Value;
            if (styleValue.IsVisible.HasValue) textBox.IsVisible = styleValue.IsVisible.Value;
        }
    }
}