using Avalonia.Controls;
using NeHive.Core;
using NeHive.Sample.Avalonia.Render.Styles;

namespace NeHive.Sample.Avalonia.Render.Components;

public static partial class BaseComponent
{
    public static IElement HCheckBox(
        Signal<bool>? bindIsChecked = null,
        Accessor<bool>? isChecked = null,
        Accessor<string>? label = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Action<bool>? checkedChanged = null)
    {
        var uiScope = new UiScope();
        var checkBox = new CheckBox();

        // 样式合并
        Accessor<StyleSet>? finalStyle = null;
        if (strStyle is not null || style is not null)
        {
            if (style is not null && strStyle is not null)
                finalStyle = new Computed<StyleSet>(() =>
                {
                    var ss = StyleParser.Parse(strStyle.Value);
                    ss.Merge(style.Value);
                    return ss;
                });
            else if (strStyle != null)
            {
                var result = new StyleSet();
                finalStyle = new Computed<StyleSet>(() =>
                {
                    StyleParser.Parse(strStyle.Value, ref result);
                    return result;
                });
            }

            else
                finalStyle = style;
        }

        uiScope.CreateEffect(scope =>
        {
            if (finalStyle == null) return;
            var styleValue = scope.Pull(finalStyle);
            ApplyStyle(styleValue);
        });

        // 标签
        if (label != null)
            uiScope.CreateEffect(scope => checkBox.Content = scope.Pull(label));

        // 双向绑定
        if (bindIsChecked != null)
        {
            var signal = bindIsChecked;
            var updating = false;
            checkBox.IsChecked = signal.Value;
            checkBox.Checked += (_, _) =>
            {
                if (updating) return;
                signal.Value = true;
                checkedChanged?.Invoke(true);
            };
            checkBox.Unchecked += (_, _) =>
            {
                if (updating) return;
                signal.Value = false;
                checkedChanged?.Invoke(false);
            };
            uiScope.CreateEffect(scope =>
            {
                var val = scope.Pull(signal);
                if (checkBox.IsChecked == val) return;
                updating = true;
                checkBox.IsChecked = val;
                updating = false;
            });
        }
        else if (isChecked != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var val = scope.Pull(isChecked);
                if (checkBox.IsChecked != val)
                    checkBox.IsChecked = val;
                checkedChanged?.Invoke(val);
            });
        }

        return new Element(uiScope, checkBox);

        void ApplyStyle(StyleSet styleValue)
        {
            if (styleValue.Margin.HasValue) checkBox.Margin = styleValue.Margin.Value;

            if (styleValue.Width.HasValue) checkBox.Width = styleValue.Width.Value;
            if (styleValue.Height.HasValue) checkBox.Height = styleValue.Height.Value;
            if (styleValue.MinWidth.HasValue) checkBox.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxWidth.HasValue) checkBox.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinHeight.HasValue) checkBox.MinHeight = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight.HasValue) checkBox.MaxHeight = styleValue.MaxHeight.Value;

            if (styleValue.Padding.HasValue) checkBox.Padding = styleValue.Padding.Value;

            if (styleValue.HorizontalAlignment.HasValue)
                checkBox.HorizontalAlignment = styleValue.HorizontalAlignment.Value;
            if (styleValue.VerticalAlignment.HasValue)
                checkBox.VerticalAlignment = styleValue.VerticalAlignment.Value;

            if (styleValue.FontSize.HasValue) checkBox.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight.HasValue) checkBox.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle.HasValue) checkBox.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null) checkBox.Foreground = styleValue.Foreground;

            if (styleValue.Background is not null) checkBox.Background = styleValue.Background;
            if (styleValue.BorderBrush is not null) checkBox.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness.HasValue) checkBox.BorderThickness = styleValue.BorderThickness.Value;
            if (styleValue.CornerRadius.HasValue) checkBox.CornerRadius = styleValue.CornerRadius.Value;

            if (styleValue.Opacity.HasValue) checkBox.Opacity = styleValue.Opacity.Value;
            if (styleValue.IsVisible.HasValue) checkBox.IsVisible = styleValue.IsVisible.Value;
        }
    }
}