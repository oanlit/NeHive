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
        Accessor<FullStyle>? style = null,
        Action<string>? onTextChanged = null,
        Action? onLostFocus = null,
        Action? onGotFocus = null,
        Action<KeyEventArgs>? keyDown = null)
    {
        var uiScope = new UiScope();
        var textBox = new TextBox();
        var border = new Border
        {
            Child = textBox
        };

        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        // 应用样式
        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                ApplyStyle(styleValue.Normal);
            });
        }

        // 处理 Watermark (Avalonia TextBox 有 Watermark 属性)
        uiScope.CreateEffect(scope =>
        {
            if (watermark == null)
                return;

            var wm = scope.Track(watermark);
            textBox.PlaceholderText = wm;
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
                    onTextChanged?.Invoke(newText);
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
                onTextChanged?.Invoke(val);
            });
        }

        // 注册事件
        uiScope.OnMount += () =>
        {
            if (onLostFocus != null)
                textBox.LostFocus += (_, _) => onLostFocus();
            if (onGotFocus != null)
                textBox.GotFocus += (_, _) => onGotFocus();
            if (keyDown != null)
                textBox.KeyDown += (_, e) => keyDown(e);
        };

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, textBox, border);
            
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
        }
    }
}