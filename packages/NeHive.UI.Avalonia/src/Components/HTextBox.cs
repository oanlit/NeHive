using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static class HTextPresenterStyle
{
    public static StyleSet DefaultStyleSet => new()
    {
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top,
        FontSize = 12,
        Foreground = Brushes.Black,
        Background = Brushes.White,
        FontWeight = FontWeight.Normal,
        BorderThickness = new Thickness(1),
        BorderBrush = Brushes.Gray,
        CornerRadius = new CornerRadius(0),
        Opacity = 1.0,
        IsVisible = true,
        Padding = new Thickness(4, 2, 4, 2)
    };
}

public static partial class BaseComponent
{
    public static IElement<TextBox> HTextBox(
        MutSignal<string?> bindText,
        Accessor<char>? passwordChar = null,
        // Accessor<bool>? selectable = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<string?>? onTextInput = null)
    {
        isReadOnly ??= false;
        var styleAccessor = StyleParser.ParseFull(strStyle, HTextPresenterStyle.DefaultStyleSet, style);
        UiScope uiScope = new();

        var textBox = new TextBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Padding = new Thickness(0), // 将 Padding 交由我们的内部 Border 接管
        };

        var border = new Border
        {
            Child = textBox
        };

        textBox.Template = new FuncControlTemplate<TextBox>((control, scope) =>
        {
            var dockPanel = new DockPanel
            {
                [!!Layoutable.HorizontalAlignmentProperty] =
                    control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
                [!!Layoutable.VerticalAlignmentProperty] =
                    control.GetObservable(Layoutable.VerticalAlignmentProperty).ToBinding(),
            };
            var floatingPlaceholder = new TextBlock
            {
                [!!TextBlock.ForegroundProperty] = control.GetObservable(TextBlock.ForegroundProperty).ToBinding(),
                [!!TextBlock.TextProperty] = control.GetObservable(TextBlock.TextProperty).ToBinding(),
                [!Visual.IsVisibleProperty] = new MultiBinding
                {
                    Converter = BoolConverters.And,
                    Bindings =
                    [
                        new Binding("UseFloatingPlaceholder")
                            { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) },
                        new Binding("Text")
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                            Converter = StringConverters.IsNotNullOrEmpty
                        }
                    ]
                }
            };
            DockPanel.SetDock(floatingPlaceholder, Dock.Top);
            dockPanel.Children.Add(floatingPlaceholder);

            var dataValidationErrors = new DataValidationErrors();
            var grid = new Grid
            {
                ColumnDefinitions =
                [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                ]
            };
            var contentPresenter1 = new ContentPresenter
            {
                [!!ContentPresenter.ContentProperty] =
                    control.GetObservable(TextBox.InnerLeftContentProperty).ToBinding(),
            };
            Grid.SetColumn(contentPresenter1, 0);
            Grid.SetColumnSpan(contentPresenter1, 1);
            grid.Children.Add(contentPresenter1);

            var scrollViewer = new ScrollViewer
            {
                [!!ScrollViewer.AllowAutoHideProperty] =
                    control.GetObservable(ScrollViewer.AllowAutoHideProperty).ToBinding(),
                [!!ScrollViewer.BringIntoViewOnFocusChangeProperty] =
                    control.GetObservable(ScrollViewer.BringIntoViewOnFocusChangeProperty).ToBinding(),
                [!!ScrollViewer.HorizontalScrollBarVisibilityProperty] =
                    control.GetObservable(ScrollViewer.HorizontalScrollBarVisibilityProperty).ToBinding(),
                [!!ScrollViewer.IsScrollChainingEnabledProperty] =
                    control.GetObservable(ScrollViewer.IsScrollChainingEnabledProperty).ToBinding(),
                [!!ScrollViewer.VerticalScrollBarVisibilityProperty] =
                    control.GetObservable(ScrollViewer.VerticalScrollBarVisibilityProperty).ToBinding(),
            };

            var panel = new Panel();

            var presenter = new TextPresenter
            {
                Name = "PART_TextPresenter",
                [!!TextPresenter.CaretBrushProperty] = control.GetObservable(TextBox.CaretBrushProperty).ToBinding(),
                [!!TextPresenter.CaretIndexProperty] = control.GetObservable(TextBox.CaretIndexProperty).ToBinding(),
                [!!TextPresenter.LineHeightProperty] = control.GetObservable(TextBox.LineHeightProperty).ToBinding(),
                [!!TextPresenter.PasswordCharProperty] =
                    control.GetObservable(TextBox.PasswordCharProperty).ToBinding(),
                [!!TextPresenter.RevealPasswordProperty] =
                    control.GetObservable(TextBox.RevealPasswordProperty).ToBinding(),
                [!!TextPresenter.SelectionBrushProperty] =
                    control.GetObservable(TextBox.SelectionBrushProperty).ToBinding(),
                [!!TextPresenter.SelectionEndProperty] =
                    control.GetObservable(TextBox.SelectionEndProperty).ToBinding(),
                [!!TextPresenter.SelectionForegroundBrushProperty] =
                    control.GetObservable(TextBox.SelectionForegroundBrushProperty).ToBinding(),
                [!!TextPresenter.SelectionStartProperty] =
                    control.GetObservable(TextBox.SelectionStartProperty).ToBinding(),
                [!!TextPresenter.TextProperty] = control[!TextBox.TextProperty],
                [!!TextPresenter.TextAlignmentProperty] =
                    control.GetObservable(TextBox.TextAlignmentProperty).ToBinding(),
                [!!TextPresenter.TextWrappingProperty] =
                    control.GetObservable(TextBox.TextWrappingProperty).ToBinding(),
            };
            scope.Register("PART_TextPresenter", presenter);

            // var placeholder = new TextBlock
            // {
            //     [!!TextBlock.ForegroundProperty] = control.GetObservable(TextBox.PlaceholderForegroundProperty).ToBinding(),
            //     [!!Layoutable.HorizontalAlignmentProperty] = control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
            //     [!!Layoutable.VerticalAlignmentProperty] = control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
            //     // [!!Visual.OpacityProperty] = control.GetObservable(TextBox.PlaceholderForegroundProperty).ToBinding(),
            //     [!!TextBlock.TextProperty] = control.GetObservable(TextBox.PlaceholderTextProperty).ToBinding(),
            //     [!!Layoutable.VerticalAlignmentProperty] = control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
            //     [!!TextPresenter.TextAlignmentProperty] =
            //         control.GetObservable(TextBox.TextAlignmentProperty).ToBinding(),
            //     [!!TextPresenter.TextWrappingProperty] =
            //         control.GetObservable(TextBox.TextWrappingProperty).ToBinding(),
            //     [!Visual.IsVisibleProperty] = new MultiBinding
            //     {
            //         Converter = BoolConverters.And,
            //         Bindings =
            //         [
            //             new Binding("PreeditText")
            //             {
            //                 ElementName = "PART_TextPresenter",
            //                 Converter = StringConverters.IsNotNullOrEmpty
            //             },
            //             new Binding("Text")
            //             {
            //                 RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
            //                 Converter = StringConverters.IsNotNullOrEmpty
            //             }
            //         ]
            //     }
            // };
            // panel.Children.Add(placeholder);
            panel.Children.Add(presenter);

            scrollViewer.Content = panel;
            Grid.SetColumn(scrollViewer, 1);
            Grid.SetColumnSpan(scrollViewer, 1);
            grid.Children.Add(scrollViewer);

            var contentPresenter2 = new ContentPresenter
            {
                [!!ContentPresenter.ContentProperty] =
                    control.GetObservable(TextBox.InnerRightContentProperty).ToBinding(),
            };
            Grid.SetColumn(contentPresenter2, 2);
            Grid.SetColumnSpan(contentPresenter2, 1);
            grid.Children.Add(contentPresenter2);

            dataValidationErrors.Content = grid;
            dockPanel.Children.Add(dataValidationErrors);

            scope.Register("PART_ScrollViewer", scrollViewer);

            return dockPanel;
        });

        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, textBox, border, ApplyStyle);
        state.ApplyVariantsStyle(textBox, border, ApplyStyle);
        ApplySelectionStyle(styleAccessor.Value);
        if (styleAccessor.IsReactive)
        {
            var firstApply = true;
            uiScope.CreateEffect(epochScope =>
            {
                var fullStyle = epochScope.Track(styleAccessor);
                if (firstApply)
                {
                    firstApply = false;
                    return;
                }

                ApplySelectionStyle(fullStyle);
            });
        }

        // 4. 数据双向绑定
        textBox.Text = bindText.Value;
        uiScope.CreateEffect(epochScope =>
        {
            var newText = epochScope.Pull(bindText);
            if (textBox.Text != newText) textBox.Text = newText;
        });
        
        textBox.TextChanged += (_, _) =>
        {
            bindText.RxValue = textBox.Text;
            onTextInput?.Invoke(bindText.Value);
        };

        if (passwordChar is not null)
        {
            textBox.PasswordChar = passwordChar.Value;
            if (passwordChar.IsReactive)
            {
                uiScope.CreateEffect(epochScope =>
                {
                    textBox.PasswordChar = epochScope.Track(passwordChar);
                });
            }
        }

        // 响应 editable 访问器
        uiScope.CreateEffect(epochScope =>
        {
            textBox.IsReadOnly = epochScope.Track(isReadOnly);
        });

        return new Element<TextBox>(uiScope, border, textBox);

        // 内部方法：将您的 StyleSet 映射到 TextBox 和 Border 上
        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, layout, bord);
            if (styleValue.Width is not null) layout.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) layout.Height = styleValue.Height.Value;
            if (styleValue.MaxWidth is not null) layout.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinWidth is not null) layout.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MinHeight is not null) layout.MinHeight = styleValue.MinHeight.Value;
            if (styleValue.MaxHeight is not null) layout.MaxHeight = styleValue.MaxHeight.Value;

            ApplyTextStyle(styleValue);
        }

        void ApplySelectionStyle(FullStyle fullStyle)
        {
            StyleSet selectionStyle = new();
            var hasSelectionStyle = false;
            if (fullStyle.Variants.TryGetValue("selection", out var selectionStrStyle))
            {
                StyleParser.Parse(selectionStrStyle, ref selectionStyle);
                hasSelectionStyle = true;
            }

            if (variants?.TryGetValue("selection", out var selectionStyle2) is true)
            {
                selectionStyle.Merge(selectionStyle2);
                hasSelectionStyle = true;
            }

            if (!hasSelectionStyle) return;
            if (selectionStyle.Background is not null) textBox.SelectionBrush = selectionStyle.Background;
            if (selectionStyle.Foreground is not null) textBox.SelectionForegroundBrush = selectionStyle.Foreground;
        }

        void ApplyTextStyle(StyleSet styleValue)
        {
            if (styleValue.LetterSpacing is not null) textBox.LetterSpacing = styleValue.LetterSpacing.Value;
            if (styleValue.LineHeight is not null) textBox.LineHeight = styleValue.LineHeight.Value;
            if (styleValue.TextAlignment is not null) textBox.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.VerticalTextAlignment is not null)
                textBox.VerticalContentAlignment = styleValue.VerticalTextAlignment.Value;
            if (styleValue.TextWrapping is not null) textBox.TextWrapping = styleValue.TextWrapping.Value;
            if (styleValue.FontSize is not null) textBox.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBox.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontFamily is not null) textBox.FontFamily = styleValue.FontFamily;
            if (styleValue.FontStretch is not null) textBox.FontStretch = styleValue.FontStretch.Value;
            if (styleValue.FontFeatures is not null) textBox.FontFeatures = styleValue.FontFeatures;
            if (styleValue.FontStyle is not null) textBox.FontStyle = styleValue.FontStyle.Value;
            if (styleValue.Foreground is not null)
            {
                textBox.Foreground = styleValue.Foreground;
                textBox.CaretBrush = styleValue.Foreground;
            }
        }
    }

    public static IElement<TextBox> HTextBox(
        out TextBox expose,
        MutSignal<string?> bindText,
        // Accessor<bool>? selectable = null,
        Accessor<char>? passwordChar = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        var el = HTextBox(bindText, passwordChar, isReadOnly, strStyle, style, variants);
        expose = el.Expose;
        return el;
    }
}