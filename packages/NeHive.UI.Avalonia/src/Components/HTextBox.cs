using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HTextBox(MutSignal<string>? bindText = null,
        Accessor<string>? text = null,
        Accessor<string>? placeholderText = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<int>? maxLength = null,
        Accessor<bool>? acceptsReturn = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<string>? onTextChanged = null,
        Action? onLostFocus = null,
        Action? onGotFocus = null,
        Action<KeyEventArgs>? keyDown = null)
    {
        var uiScope = new UiScope();
        var textBox = new TextBox
        {
            BorderBrush = null
        };
        var border = new Border
        {
            Child = textBox
        };

        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, textBox, border, ApplyStyle);
        state.ApplyVariantsStyle(textBox, border, ApplyStyle);

        // 占位文本
        if (placeholderText is not null)
        {
            textBox.PlaceholderText = placeholderText.Value;
            if (placeholderText.IsReactive)
                uiScope.CreateEffect(epochScope => textBox.PlaceholderText = epochScope.Track(placeholderText));
        }

        // 处理只读
        if (isReadOnly is not null)
        {
            textBox.IsReadOnly = isReadOnly.Value;
            if (isReadOnly.IsReactive)
                uiScope.CreateEffect(epochScope => textBox.IsReadOnly = epochScope.Track(isReadOnly));
        }

        // 处理最大长度
        if (maxLength is not null)
        {
            textBox.MaxLength = maxLength.Value;
            if (maxLength.IsReactive)
                uiScope.CreateEffect(epochScope => textBox.MaxLength = epochScope.Track(maxLength));
        }

        // 处理多行
        if (acceptsReturn is not null)
        {
            textBox.AcceptsReturn = acceptsReturn.Value;
            if (acceptsReturn.IsReactive)
                uiScope.CreateEffect(epochScope => textBox.AcceptsReturn = epochScope.Track(acceptsReturn));
        }

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

        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, layout, bord);

            if (styleValue.Width is not null)
            {
                textBox.Width = styleValue.Width.Value;
                if (styleValue.Padding is not null)
                {
                    textBox.Width = textBox.Width - styleValue.Padding.Value.Left- styleValue.Padding.Value.Right;
                }
            }

            if (styleValue.Height is not null)
            {
                textBox.Height = styleValue.Height.Value;
                if (styleValue.Padding is not null)
                {
                    textBox.Height = textBox.Height - styleValue.Padding.Value.Top- styleValue.Padding.Value.Bottom;
                }
            }

            if (styleValue.MinWidth is not null)
                textBox.MinWidth = styleValue.MinWidth.Value;

            if (styleValue.MaxWidth is not null)
                textBox.MaxWidth = styleValue.MaxWidth.Value;

            if (styleValue.MinHeight is not null)
                textBox.MinHeight = styleValue.MinHeight.Value;

            if (styleValue.MaxHeight is not null)
                textBox.MaxHeight = styleValue.MaxHeight.Value;

            if (styleValue.LetterSpacing is not null) textBox.LetterSpacing = styleValue.LetterSpacing.Value;
            if (styleValue.LineHeight is not null) textBox.LineHeight = styleValue.LineHeight.Value;
            if (styleValue.MaxLines is not null) textBox.MaxLines = styleValue.MaxLines.Value;

            if (styleValue.TextAlignment is not null)
            {
                switch (styleValue.TextAlignment.Value)
                {
                    case TextAlignment.Left:
                        textBox.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case TextAlignment.Center:
                        textBox.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case TextAlignment.Right:
                        textBox.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }
            }

            if (styleValue.VerticalTextAlignment is not null)
                border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;

            if (styleValue.TextWrapping is not null) textBox.TextWrapping = styleValue.TextWrapping.Value;

            if (styleValue.FontSize is not null) textBox.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBox.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontFamily is not null) textBox.FontFamily = styleValue.FontFamily;
            if (styleValue.FontStretch is not null) textBox.FontStretch = styleValue.FontStretch.Value;
            if (styleValue.FontFeatures is not null) textBox.FontFeatures = styleValue.FontFeatures;
            if (styleValue.FontStyle is not null) textBox.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) textBox.Foreground = styleValue.Foreground;
        }
    }
}