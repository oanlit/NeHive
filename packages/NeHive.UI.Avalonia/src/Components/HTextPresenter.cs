using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HTextPresenterProps(Scope scope, Border border, TextPresenter presenter)
    : BaseComponentProps(scope, border, presenter)
{
    public MutSignal<string?> Text
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(presenter.Text);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.TextProperty);
            return field;
        }
    }

    public MutSignal<string?> PreeditText
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(presenter.PreeditText);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.PreeditTextProperty);
            return field;
        }
    }

    public MutSignal<char> PasswordChar
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<char>(presenter.PasswordChar);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.PasswordCharProperty);
            return field;
        }
    }

    public MutSignal<int> SelectionStart
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(presenter.SelectionStart);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.SelectionStartProperty);
            return field;
        }
    }

    public MutSignal<int> SelectionEnd
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(presenter.SelectionEnd);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.SelectionEndProperty);
            return field;
        }
    }

    public MutSignal<int?> PreeditTextCursorPosition
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int?>(presenter.PreeditTextCursorPosition);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.PreeditTextCursorPositionProperty);
            return field;
        }
    }

    public MutSignal<int> CaretIndex
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(presenter.CaretIndex);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.CaretIndexProperty);
            return field;
        }
    }

    public MutSignal<TimeSpan> CaretBlinkInterval
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<TimeSpan>(presenter.CaretBlinkInterval);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.CaretBlinkIntervalProperty);
            return field;
        }
    }

    public MutSignal<bool> IsShowSelectionHighlight
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(presenter.ShowSelectionHighlight);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.ShowSelectionHighlightProperty);
            return field;
        }
    }

    public MutSignal<bool> IsRevealPassword
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(presenter.RevealPassword);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.RevealPasswordProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(presenter.SelectionBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.SelectionBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionForegroundBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(presenter.SelectionForegroundBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.SelectionForegroundBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> CaretBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(presenter.CaretBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, presenter, TextPresenter.CaretBrushProperty);
            return field;
        }
    }
}

public class HTextPresenterArgs(
    Accessor<string?>? text = null,
    MutSignal<string?>? bindText = null,
    Accessor<string?>? preeditText = null,
    MutSignal<string?>? bindPreeditText = null,
    Accessor<char>? passwordChar = null,
    Accessor<int>? selectionStart = null,
    Accessor<int>? selectionEnd = null,
    Accessor<int?>? preeditTextCursorPosition = null,
    Accessor<int>? caretIndex = null,
    Accessor<TimeSpan>? caretBlinkInterval = null,
    Accessor<bool>? isShowSelectionHighlight = null,
    Accessor<bool>? isRevealPassword = null,
    Accessor<IBrush?>? selectionBrush = null,
    Accessor<IBrush?>? selectionForegroundBrush = null,
    Accessor<IBrush?>? caretBrush = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<string?>? Text = bindText ?? text;
    public readonly MutSignal<string?>? BindText = bindText;
    public readonly Accessor<string?>? PreeditText = bindPreeditText ?? preeditText;
    public readonly MutSignal<string?>? BindPreeditText = bindPreeditText;
    public readonly Accessor<char>? PasswordChar = passwordChar;
    public readonly Accessor<int>? SelectionStart = selectionStart;
    public readonly Accessor<int>? SelectionEnd = selectionEnd;
    public readonly Accessor<int?>? PreeditTextCursorPosition = preeditTextCursorPosition;
    public readonly Accessor<int>? CaretIndex = caretIndex;
    public readonly Accessor<TimeSpan>? CaretBlinkInterval = caretBlinkInterval;
    public readonly Accessor<bool>? IsShowSelectionHighlight = isShowSelectionHighlight;
    public readonly Accessor<bool>? IsRevealPassword = isRevealPassword;
    public readonly Accessor<IBrush?>? SelectionBrush = selectionBrush;
    public readonly Accessor<IBrush?>? SelectionForegroundBrush = selectionForegroundBrush;
    public readonly Accessor<IBrush?>? CaretBrush = caretBrush;
}

public static partial class BaseComponent
{
    public static IElement<TextPresenter> HTextPresenter(
        Accessor<string?>? text = null,
        MutSignal<string?>? bindText = null,
        Accessor<string?>? preeditText = null,
        MutSignal<string?>? bindPreeditText = null,
        Accessor<char>? passwordChar = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<int?>? preeditTextCursorPosition = null,
        Accessor<int>? caretIndex = null,
        Accessor<TimeSpan>? caretBlinkInterval = null,
        Accessor<bool>? isShowSelectionHighlight = null,
        Accessor<bool>? isRevealPassword = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<IBrush?>? caretBrush = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HTextPresenter(out _, _ => new(text, bindText, preeditText, bindPreeditText, passwordChar,
        selectionStart, selectionEnd, preeditTextCursorPosition, caretIndex, caretBlinkInterval,
        isShowSelectionHighlight, isRevealPassword, selectionBrush, selectionForegroundBrush, caretBrush,
        strStyle, style, baseInteraction));

    public static IElement<TextPresenter> HTextPresenter(
        out TextPresenter expose,
        Accessor<string?>? text = null,
        MutSignal<string?>? bindText = null,
        Accessor<string?>? preeditText = null,
        MutSignal<string?>? bindPreeditText = null,
        Accessor<char>? passwordChar = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<int?>? preeditTextCursorPosition = null,
        Accessor<int>? caretIndex = null,
        Accessor<TimeSpan>? caretBlinkInterval = null,
        Accessor<bool>? isShowSelectionHighlight = null,
        Accessor<bool>? isRevealPassword = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<IBrush?>? caretBrush = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HTextPresenter(out expose, _ => new(text, bindText, preeditText, bindPreeditText, passwordChar,
        selectionStart, selectionEnd, preeditTextCursorPosition, caretIndex, caretBlinkInterval,
        isShowSelectionHighlight, isRevealPassword, selectionBrush, selectionForegroundBrush, caretBrush,
        strStyle, style, baseInteraction));

    public static IElement<TextPresenter> HTextPresenter(Func<HTextPresenterProps, HTextPresenterArgs> fn) =>
        HTextPresenter(out _, fn);

    public static IElement<TextPresenter> HTextPresenter(
        out TextPresenter expose,
        Func<HTextPresenterProps, HTextPresenterArgs> fn)
    {
        var presenter = new TextPresenter
        {
            Background = Brushes.Transparent,
            // HorizontalAlignment = HorizontalAlignment.Stretch,
            // VerticalAlignment = VerticalAlignment.Stretch
        };
        expose = presenter;
        return Element<TextPresenter>.WithScope(uiScope =>
        {
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = presenter
            };
            var props = new HTextPresenterProps(uiScope, border, presenter);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, presenter, border, ApplyStyle);
            state.ApplyVariantsStyle(presenter, border, ApplyStyle);
            ApplySelectionStyle(args.StrStyle.Value);
            if (args.StrStyle.IsReactive)
            {
                var firstApply = true;
                uiScope.CreateEffect(epoch =>
                {
                    var fullStyle = epoch.Track(args.StrStyle);
                    if (firstApply)
                    {
                        firstApply = false;
                        return;
                    }

                    ApplySelectionStyle(fullStyle);
                });
            }

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(presenter, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, presenter);

            if (args.BindText is not null)
            {
                BridgeAvalonia.BindPropertySignal(uiScope, args.BindText, presenter,
                    TextPresenter.TextProperty, true);
            }
            else if (args.Text is not null)
            {
                presenter.Text = args.Text.Value;
                if (args.Text.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.Text = epoch.Track(args.Text));
            }

            if (args.BindPreeditText is not null)
            {
                BridgeAvalonia.BindPropertySignal(uiScope, args.BindPreeditText, presenter,
                    TextPresenter.PreeditTextProperty, true);
            }
            else if (args.PreeditText is not null)
            {
                presenter.PreeditText = args.PreeditText.Value;
                if (args.PreeditText.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.PreeditText = epoch.Track(args.PreeditText));
            }

            if (args.PasswordChar is not null)
            {
                presenter.PasswordChar = args.PasswordChar.Value;
                if (args.PasswordChar.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.PasswordChar = epoch.Track(args.PasswordChar));
            }

            if (args.SelectionStart is not null)
            {
                presenter.SelectionStart = args.SelectionStart.Value;
                if (args.SelectionStart.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.SelectionStart = epoch.Track(args.SelectionStart));
            }

            if (args.SelectionEnd is not null)
            {
                presenter.SelectionEnd = args.SelectionEnd.Value;
                if (args.SelectionEnd.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.SelectionEnd = epoch.Track(args.SelectionEnd));
            }

            if (args.PreeditTextCursorPosition is not null)
            {
                presenter.PreeditTextCursorPosition = args.PreeditTextCursorPosition.Value;
                if (args.PreeditTextCursorPosition.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        presenter.PreeditTextCursorPosition = epoch.Track(args.PreeditTextCursorPosition));
            }

            if (args.CaretIndex is not null)
            {
                presenter.CaretIndex = args.CaretIndex.Value;
                if (args.CaretIndex.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.CaretIndex = epoch.Track(args.CaretIndex));
            }

            if (args.CaretBlinkInterval is not null)
            {
                presenter.CaretBlinkInterval = args.CaretBlinkInterval.Value;
                if (args.CaretBlinkInterval.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.CaretBlinkInterval = epoch.Track(args.CaretBlinkInterval));
            }

            if (args.IsShowSelectionHighlight is not null)
            {
                presenter.ShowSelectionHighlight = args.IsShowSelectionHighlight.Value;
                if (args.IsShowSelectionHighlight.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        presenter.ShowSelectionHighlight = epoch.Track(args.IsShowSelectionHighlight));
            }

            if (args.IsRevealPassword is not null)
            {
                presenter.RevealPassword = args.IsRevealPassword.Value;
                if (args.IsRevealPassword.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.RevealPassword = epoch.Track(args.IsRevealPassword));
            }

            if (args.SelectionBrush is not null)
            {
                presenter.SelectionBrush = args.SelectionBrush.Value;
                if (args.SelectionBrush.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.SelectionBrush = epoch.Track(args.SelectionBrush));
            }

            if (args.SelectionForegroundBrush is not null)
            {
                presenter.SelectionForegroundBrush = args.SelectionForegroundBrush.Value;
                if (args.SelectionForegroundBrush.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        presenter.SelectionForegroundBrush = epoch.Track(args.SelectionForegroundBrush));
            }

            if (args.CaretBrush is not null)
            {
                presenter.CaretBrush = args.CaretBrush.Value;
                if (args.CaretBrush.IsReactive)
                    uiScope.CreateEffect(epoch => presenter.CaretBrush = epoch.Track(args.CaretBrush));
            }

            return (presenter, border);

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

                if (!hasSelectionStyle) return;
                if (selectionStyle.Background is not null && args.SelectionBrush is null)
                    presenter.SelectionBrush = selectionStyle.Background;
                if (selectionStyle.Background is not null && args.SelectionForegroundBrush is null)
                    presenter.SelectionForegroundBrush = selectionStyle.Foreground;
            }

            void ApplyTextStyle(StyleSet styleValue)
            {
                if (styleValue.LetterSpacing is not null) presenter.LetterSpacing = styleValue.LetterSpacing.Value;
                if (styleValue.LineHeight is not null) presenter.LineHeight = styleValue.LineHeight.Value;
                if (styleValue.TextAlignment is not null) presenter.TextAlignment = styleValue.TextAlignment.Value;
                if (styleValue.TextWrapping is not null) presenter.TextWrapping = styleValue.TextWrapping.Value;
                if (styleValue.FontSize is not null) presenter.FontSize = styleValue.FontSize.Value;
                if (styleValue.FontWeight is not null) presenter.FontWeight = styleValue.FontWeight.Value;
                if (styleValue.FontFamily is not null) presenter.FontFamily = styleValue.FontFamily;
                if (styleValue.FontStretch is not null) presenter.FontStretch = styleValue.FontStretch.Value;
                if (styleValue.FontFeatures is not null) presenter.FontFeatures = styleValue.FontFeatures;
                if (styleValue.FontStyle is not null) presenter.FontStyle = styleValue.FontStyle.Value;
                if (styleValue.Foreground is not null)
                {
                    presenter.Foreground = styleValue.Foreground;
                    presenter.CaretBrush = styleValue.Foreground;
                }
            }
        });
    }
}