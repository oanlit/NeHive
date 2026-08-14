using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;

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

public class HTextBoxProps(Scope scope, Border border, TextBox textBox)
    : BaseComponentProps(scope, border, textBox)
{
    public Signal<bool> CanCut
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBox, TextBox.CanCutProperty, textBox.CanCut);
            return field;
        }
    }

    public Signal<bool> CanPaste
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBox, TextBox.CanPasteProperty, textBox.CanPaste);
            return field;
        }
    }

    public Signal<bool> CanUndo
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBox, TextBox.CanUndoProperty, textBox.CanUndo);
            return field;
        }
    }

    public Signal<bool> CanRedo
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, textBox, TextBox.CanRedoProperty, textBox.CanRedo);
            return field;
        }
    }

    public MutSignal<string?> Text
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(textBox.Text);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.TextProperty, true);
            return field;
        }
    }

    // public MutSignal<string> SelectedText
    // {
    //     get
    //     {
    //         if (field is not null) return field;
    //         field = new MutSignal<string>(textBox.SelectedText);
    //         BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.SelectedTextProperty);
    //         return field;
    //     }
    // }

    public MutSignal<string?> PlaceholderText
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(textBox.PlaceholderText);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.PlaceholderTextProperty);
            return field;
        }
    }

    public MutSignal<char> PasswordChar
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<char>(textBox.PasswordChar);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.PasswordCharProperty);
            return field;
        }
    }

    public MutSignal<int> SelectionStart
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.SelectionStart);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.SelectionStartProperty);
            return field;
        }
    }

    public MutSignal<int> SelectionEnd
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.SelectionEnd);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.SelectionEndProperty);
            return field;
        }
    }

    public MutSignal<bool> IsReadOnly
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.IsReadOnly);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.IsReadOnlyProperty);
            return field;
        }
    }

    public MutSignal<bool> IsUseFloatingPlaceholder
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.UseFloatingPlaceholder);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.UseFloatingPlaceholderProperty);
            return field;
        }
    }

    public MutSignal<bool> IsRevealPassword
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.RevealPassword);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.RevealPasswordProperty);
            return field;
        }
    }

    public MutSignal<bool> IsInactiveSelectionHighlightEnabled
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.IsInactiveSelectionHighlightEnabled);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox,
                TextBox.IsInactiveSelectionHighlightEnabledProperty);
            return field;
        }
    }

    public MutSignal<bool> IsClearSelectionOnLostFocus
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.ClearSelectionOnLostFocus);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.ClearSelectionOnLostFocusProperty);
            return field;
        }
    }

    public MutSignal<bool> IsAcceptsReturn
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.AcceptsReturn);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.AcceptsReturnProperty);
            return field;
        }
    }

    public MutSignal<bool> IsAcceptsTab
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.AcceptsTab);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.AcceptsTabProperty);
            return field;
        }
    }

    public MutSignal<bool> IsUndoEnabled
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(textBox.IsUndoEnabled);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.IsUndoEnabledProperty);
            return field;
        }
    }

    public MutSignal<string> NewLine
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string>(textBox.NewLine);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.NewLineProperty);
            return field;
        }
    }

    public MutSignal<int> MaxLength
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.MaxLength);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.MaxLengthProperty);
            return field;
        }
    }

    public MutSignal<int> UndoLimit
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.UndoLimit);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.UndoLimitProperty);
            return field;
        }
    }

    public MutSignal<int> CaretIndex
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.CaretIndex);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.CaretIndexProperty);
            return field;
        }
    }

    public MutSignal<TimeSpan> CaretBlinkInterval
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<TimeSpan>(textBox.CaretBlinkInterval);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.CaretBlinkIntervalProperty);
            return field;
        }
    }

    public MutSignal<int> MinLines
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<int>(textBox.MinLines);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.MinLinesProperty);
            return field;
        }
    }

    public MutSignal<HorizontalAlignment> HorizontalContentAlignment
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<HorizontalAlignment>(textBox.HorizontalContentAlignment);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.HorizontalContentAlignmentProperty);
            return field;
        }
    }

    public MutSignal<VerticalAlignment> VerticalContentAlignment
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<VerticalAlignment>(textBox.VerticalContentAlignment);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.VerticalContentAlignmentProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBox.SelectionBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.SelectionBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> SelectionForegroundBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBox.SelectionForegroundBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.SelectionForegroundBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> CaretBrush
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBox.CaretBrush);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.CaretBrushProperty);
            return field;
        }
    }

    public MutSignal<IBrush?> PlaceholderForeground
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IBrush?>(textBox.PlaceholderForeground);
            BridgeAvalonia.BindPropertySignal(scope, field, textBox, TextBox.PlaceholderForegroundProperty);
            return field;
        }
    }
}

public class HTextBoxPart
{
    internal IElement<TextPresenter>? TextPresenterElement { get; private set; }
    internal IElement<ScrollViewer>? ScrollViewerElement { get; private set; }

    public IElement<TextPresenter> TextPresenter(IElement<TextPresenter> element) => TextPresenterElement = element;
    public IElement<ScrollViewer> ScrollViewer(IElement<ScrollViewer> element) => ScrollViewerElement = element;
}

public class HTextBoxArgs(
    Accessor<string?>? text = null,
    MutSignal<string?>? bindText = null,
    Accessor<string?>? placeholderText = null,
    Accessor<char>? passwordChar = null,
    Accessor<int>? selectionStart = null,
    Accessor<int>? selectionEnd = null,
    Accessor<bool>? isReadOnly = null,
    Accessor<bool>? isUseFloatingPlaceholder = null,
    Accessor<bool>? isRevealPassword = null,
    Accessor<bool>? isInactiveSelectionHighlightEnabled = null,
    Accessor<bool>? isClearSelectionOnLostFocus = null,
    Accessor<bool>? isAcceptsReturn = null,
    Accessor<bool>? isAcceptsTab = null,
    Accessor<bool>? isUndoEnabled = null,
    Accessor<string>? newLine = null,
    Accessor<int>? maxLength = null,
    Accessor<int>? undoLimit = null,
    Accessor<int>? caretIndex = null,
    Accessor<TimeSpan>? caretBlinkInterval = null,
    Accessor<int>? minLines = null,
    Accessor<HorizontalAlignment>? horizontalContentAlignment = null,
    Accessor<VerticalAlignment>? verticalContentAlignment = null,
    Accessor<IBrush?>? selectionBrush = null,
    Accessor<IBrush?>? selectionForegroundBrush = null,
    Accessor<IBrush?>? caretBrush = null,
    Accessor<IBrush?>? placeholderForeground = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RoutedEventArgs>? onCopyingToClipboard = null,
    Action<RoutedEventArgs>? onPastingFromClipboard = null,
    Action<TextChangedEventArgs>? onTextChanged = null,
    Action<TextChangingEventArgs>? onTextChanging = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<string?>? Text = bindText ?? text;
    public readonly MutSignal<string?>? BindText = bindText;
    public readonly Accessor<string?>? PlaceholderText = placeholderText;
    public readonly Accessor<char>? PasswordChar = passwordChar;
    public readonly Accessor<int>? SelectionStart = selectionStart;
    public readonly Accessor<int>? SelectionEnd = selectionEnd;
    public readonly Accessor<bool>? IsReadOnly = isReadOnly;
    public readonly Accessor<bool>? IsUseFloatingPlaceholder = isUseFloatingPlaceholder;
    public readonly Accessor<bool>? IsRevealPassword = isRevealPassword;
    public readonly Accessor<bool>? IsInactiveSelectionHighlightEnabled = isInactiveSelectionHighlightEnabled;
    public readonly Accessor<bool>? IsClearSelectionOnLostFocus = isClearSelectionOnLostFocus;
    public readonly Accessor<bool>? IsAcceptsReturn = isAcceptsReturn;
    public readonly Accessor<bool>? IsAcceptsTab = isAcceptsTab;
    public readonly Accessor<bool>? IsUndoEnabled = isUndoEnabled;
    public readonly Accessor<string>? NewLine = newLine;
    public readonly Accessor<int>? MaxLength = maxLength;
    public readonly Accessor<int>? UndoLimit = undoLimit;
    public readonly Accessor<int>? CaretIndex = caretIndex;
    public readonly Accessor<TimeSpan>? CaretBlinkInterval = caretBlinkInterval;
    public readonly Accessor<int>? MinLines = minLines;
    public readonly Accessor<HorizontalAlignment>? HorizontalContentAlignment = horizontalContentAlignment;
    public readonly Accessor<VerticalAlignment>? VerticalContentAlignment = verticalContentAlignment;
    public readonly Accessor<IBrush?>? SelectionBrush = selectionBrush;
    public readonly Accessor<IBrush?>? SelectionForegroundBrush = selectionForegroundBrush;
    public readonly Accessor<IBrush?>? CaretBrush = caretBrush;
    public readonly Accessor<IBrush?>? PlaceholderForeground = placeholderForeground;

    public IElement? InnerLeftContent { get; init; }
    public IElement? InnerRightContent { get; init; }
    public Func<HTextBoxPart, IElement>? Template { get; init; }

    public readonly Action<RoutedEventArgs>? OnCopyingToClipboard = onCopyingToClipboard;
    public readonly Action<RoutedEventArgs>? OnPastingFromClipboard = onPastingFromClipboard;
    public readonly Action<TextChangedEventArgs>? OnTextChanged = onTextChanged;
    public readonly Action<TextChangingEventArgs>? OnTextChanging = onTextChanging;
}

public static partial class BaseComponent
{
    public static IElement<TextBox> HTextBox(
        Accessor<string?>? text = null,
        MutSignal<string?>? bindText = null,
        Accessor<string?>? placeholderText = null,
        Accessor<char>? passwordChar = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<bool>? isUseFloatingPlaceholder = null,
        Accessor<bool>? isRevealPassword = null,
        Accessor<bool>? isInactiveSelectionHighlightEnabled = null,
        Accessor<bool>? isClearSelectionOnLostFocus = null,
        Accessor<bool>? isAcceptsReturn = null,
        Accessor<bool>? isAcceptsTab = null,
        Accessor<bool>? isUndoEnabled = null,
        Accessor<string>? newLine = null,
        Accessor<int>? maxLength = null,
        Accessor<int>? undoLimit = null,
        Accessor<int>? caretIndex = null,
        Accessor<TimeSpan>? caretBlinkInterval = null,
        Accessor<int>? minLines = null,
        Accessor<HorizontalAlignment>? horizontalContentAlignment = null,
        Accessor<VerticalAlignment>? verticalContentAlignment = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<IBrush?>? caretBrush = null,
        Accessor<IBrush?>? placeholderForeground = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onCopyingToClipboard = null,
        Action<RoutedEventArgs>? onPastingFromClipboard = null,
        Action<TextChangedEventArgs>? onTextChanged = null,
        Action<TextChangingEventArgs>? onTextChanging = null
    ) => HTextBox(out _, _ => new(text, bindText, placeholderText, passwordChar, selectionStart, selectionEnd,
        isReadOnly, isUseFloatingPlaceholder, isRevealPassword, isInactiveSelectionHighlightEnabled,
        isClearSelectionOnLostFocus, isAcceptsReturn, isAcceptsTab, isUndoEnabled,
        newLine, maxLength, undoLimit, caretIndex, caretBlinkInterval, minLines,
        horizontalContentAlignment, verticalContentAlignment, selectionBrush, selectionForegroundBrush, caretBrush,
        placeholderForeground, strStyle, style, baseInteraction,
        onCopyingToClipboard, onPastingFromClipboard, onTextChanged, onTextChanging));

    public static IElement<TextBox> HTextBox(
        out TextBox expose,
        Accessor<string?>? text = null,
        MutSignal<string?>? bindText = null,
        Accessor<string?>? placeholderText = null,
        Accessor<char>? passwordChar = null,
        Accessor<int>? selectionStart = null,
        Accessor<int>? selectionEnd = null,
        Accessor<bool>? isReadOnly = null,
        Accessor<bool>? isUseFloatingPlaceholder = null,
        Accessor<bool>? isRevealPassword = null,
        Accessor<bool>? isInactiveSelectionHighlightEnabled = null,
        Accessor<bool>? isClearSelectionOnLostFocus = null,
        Accessor<bool>? isAcceptsReturn = null,
        Accessor<bool>? isAcceptsTab = null,
        Accessor<bool>? isUndoEnabled = null,
        Accessor<string>? newLine = null,
        Accessor<int>? maxLength = null,
        Accessor<int>? undoLimit = null,
        Accessor<int>? caretIndex = null,
        Accessor<TimeSpan>? caretBlinkInterval = null,
        Accessor<int>? minLines = null,
        Accessor<HorizontalAlignment>? horizontalContentAlignment = null,
        Accessor<VerticalAlignment>? verticalContentAlignment = null,
        Accessor<IBrush?>? selectionBrush = null,
        Accessor<IBrush?>? selectionForegroundBrush = null,
        Accessor<IBrush?>? caretBrush = null,
        Accessor<IBrush?>? placeholderForeground = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RoutedEventArgs>? onCopyingToClipboard = null,
        Action<RoutedEventArgs>? onPastingFromClipboard = null,
        Action<TextChangedEventArgs>? onTextChanged = null,
        Action<TextChangingEventArgs>? onTextChanging = null
    ) => HTextBox(out expose, _ => new(text, bindText, placeholderText, passwordChar, selectionStart, selectionEnd,
        isReadOnly, isUseFloatingPlaceholder, isRevealPassword, isInactiveSelectionHighlightEnabled,
        isClearSelectionOnLostFocus, isAcceptsReturn, isAcceptsTab, isUndoEnabled,
        newLine, maxLength, undoLimit, caretIndex, caretBlinkInterval, minLines,
        horizontalContentAlignment, verticalContentAlignment, selectionBrush, selectionForegroundBrush, caretBrush,
        placeholderForeground, strStyle, style, baseInteraction,
        onCopyingToClipboard, onPastingFromClipboard, onTextChanged, onTextChanging));

    public static IElement<TextBox> HTextBox(Func<HTextBoxProps, HTextBoxArgs> fn) => HTextBox(out _, fn);

    public static IElement<TextBox> HTextBox(
        out TextBox expose,
        Func<HTextBoxProps, HTextBoxArgs> fn)
    {
        var textBox = new TextBox();
        expose = textBox;
        return Element<TextBox>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HTextBoxProps(uiScope, border, textBox);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, textBox, border, ApplyStyle);
            state.ApplyVariantsStyle(textBox, border, ApplyStyle);
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
                ElementUtil.ApplyPopups(textBox, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, textBox);

            if (args.BindText is not null)
            {
                BridgeAvalonia.BindPropertySignal(uiScope, args.BindText, textBox,
                    TextBox.TextProperty, true);
            }
            else if (args.Text is not null)
            {
                textBox.Text = args.Text.Value;
                if (args.Text.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.Text = epoch.Track(args.Text));
            }

            if (args.PlaceholderText is not null)
            {
                textBox.PlaceholderText = args.PlaceholderText.Value;
                if (args.PlaceholderText.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.PlaceholderText = epoch.Track(args.PlaceholderText));
            }

            if (args.PasswordChar is not null)
            {
                textBox.PasswordChar = args.PasswordChar.Value;
                if (args.PasswordChar.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.PasswordChar = epoch.Track(args.PasswordChar));
            }

            if (args.SelectionStart is not null)
            {
                textBox.SelectionStart = args.SelectionStart.Value;
                if (args.SelectionStart.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.SelectionStart = epoch.Track(args.SelectionStart));
            }

            if (args.SelectionEnd is not null)
            {
                textBox.SelectionEnd = args.SelectionEnd.Value;
                if (args.SelectionEnd.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.SelectionEnd = epoch.Track(args.SelectionEnd));
            }

            if (args.IsReadOnly is not null)
            {
                textBox.IsReadOnly = args.IsReadOnly.Value;
                if (args.IsReadOnly.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.IsReadOnly = epoch.Track(args.IsReadOnly));
            }

            if (args.IsUseFloatingPlaceholder is not null)
            {
                textBox.UseFloatingPlaceholder = args.IsUseFloatingPlaceholder.Value;
                if (args.IsUseFloatingPlaceholder.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.UseFloatingPlaceholder = epoch.Track(args.IsUseFloatingPlaceholder));
            }

            if (args.IsRevealPassword is not null)
            {
                textBox.RevealPassword = args.IsRevealPassword.Value;
                if (args.IsRevealPassword.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.RevealPassword = epoch.Track(args.IsRevealPassword));
            }

            if (args.IsInactiveSelectionHighlightEnabled is not null)
            {
                textBox.IsInactiveSelectionHighlightEnabled = args.IsInactiveSelectionHighlightEnabled.Value;
                if (args.IsInactiveSelectionHighlightEnabled.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.IsInactiveSelectionHighlightEnabled =
                            epoch.Track(args.IsInactiveSelectionHighlightEnabled));
            }

            if (args.IsClearSelectionOnLostFocus is not null)
            {
                textBox.ClearSelectionOnLostFocus = args.IsClearSelectionOnLostFocus.Value;
                if (args.IsClearSelectionOnLostFocus.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.ClearSelectionOnLostFocus = epoch.Track(args.IsClearSelectionOnLostFocus));
            }

            if (args.IsAcceptsReturn is not null)
            {
                textBox.AcceptsReturn = args.IsAcceptsReturn.Value;
                if (args.IsAcceptsReturn.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.AcceptsReturn = epoch.Track(args.IsAcceptsReturn));
            }

            if (args.IsAcceptsTab is not null)
            {
                textBox.AcceptsTab = args.IsAcceptsTab.Value;
                if (args.IsAcceptsTab.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.AcceptsTab = epoch.Track(args.IsAcceptsTab));
            }

            if (args.IsUndoEnabled is not null)
            {
                textBox.IsUndoEnabled = args.IsUndoEnabled.Value;
                if (args.IsUndoEnabled.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.IsUndoEnabled = epoch.Track(args.IsUndoEnabled));
            }

            if (args.NewLine is not null)
            {
                textBox.NewLine = args.NewLine.Value;
                if (args.NewLine.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.NewLine = epoch.Track(args.NewLine));
            }


            if (args.MaxLength is not null)
            {
                textBox.MaxLength = args.MaxLength.Value;
                if (args.MaxLength.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.MaxLength = epoch.Track(args.MaxLength));
            }

            if (args.UndoLimit is not null)
            {
                textBox.UndoLimit = args.UndoLimit.Value;
                if (args.UndoLimit.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.UndoLimit = epoch.Track(args.UndoLimit));
            }

            if (args.CaretIndex is not null)
            {
                textBox.CaretIndex = args.CaretIndex.Value;
                if (args.CaretIndex.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.CaretIndex = epoch.Track(args.CaretIndex));
            }

            if (args.CaretBlinkInterval is not null)
            {
                textBox.CaretBlinkInterval = args.CaretBlinkInterval.Value;
                if (args.CaretBlinkInterval.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.CaretBlinkInterval = epoch.Track(args.CaretBlinkInterval));
            }

            if (args.MinLines is not null)
            {
                textBox.MinLines = args.MinLines.Value;
                if (args.MinLines.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.MinLines = epoch.Track(args.MinLines));
            }

            if (args.HorizontalContentAlignment is not null)
            {
                textBox.HorizontalContentAlignment = args.HorizontalContentAlignment.Value;
                if (args.HorizontalContentAlignment.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.HorizontalContentAlignment = epoch.Track(args.HorizontalContentAlignment));
            }

            if (args.VerticalContentAlignment is not null)
            {
                textBox.VerticalContentAlignment = args.VerticalContentAlignment.Value;
                if (args.VerticalContentAlignment.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.VerticalContentAlignment = epoch.Track(args.VerticalContentAlignment));
            }

            if (args.SelectionBrush is not null)
            {
                textBox.SelectionBrush = args.SelectionBrush.Value;
                if (args.SelectionBrush.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.SelectionBrush = epoch.Track(args.SelectionBrush));
            }

            if (args.SelectionForegroundBrush is not null)
            {
                textBox.SelectionForegroundBrush = args.SelectionForegroundBrush.Value;
                if (args.SelectionForegroundBrush.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.SelectionForegroundBrush = epoch.Track(args.SelectionForegroundBrush));
            }

            if (args.CaretBrush is not null)
            {
                textBox.CaretBrush = args.CaretBrush.Value;
                if (args.CaretBrush.IsReactive)
                    uiScope.CreateEffect(epoch => textBox.CaretBrush = epoch.Track(args.CaretBrush));
            }

            if (args.PlaceholderForeground is not null)
            {
                textBox.PlaceholderForeground = args.PlaceholderForeground.Value;
                if (args.PlaceholderForeground.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        textBox.PlaceholderForeground = epoch.Track(args.PlaceholderForeground));
            }

            var template = args.Template;

            if (template is null)
            {
                var innerLeftContent = args.InnerLeftContent ?? Element.Empty;
                var innerRightContent = args.InnerRightContent ?? Element.Empty;
                var preeditText = new MutSignal<string?>(null);
                template = part => HDockPanel(_ => new(style: new(
                    horizontalAlignment: props.HorizontalContentAlignment,
                    verticalAlignment: props.VerticalContentAlignment
                ))
                {
                    [Dock.Top] = HText(text: props.PlaceholderText, style: new(
                        foreground: props.PlaceholderForeground,
                        fontSize: 12,
                        isVisible: new(() =>
                            props.IsUseFloatingPlaceholder.RxValue && string.IsNullOrEmpty(props.Text.RxValue))
                    )),
                    [null] = HGrid(_ => new(
                        columnDefinitions: new([HgLen.Auto, HgLen.Star(), HgLen.Auto]),
                        style: new(
                            horizontalAlignment: HorizontalAlignment.Stretch,
                            verticalAlignment: VerticalAlignment.Stretch
                        )
                    )
                    {
                        [column: 0, colSpan: 1] = innerLeftContent,
                        [column: 1, colSpan: 1] = part.ScrollViewer(HScrollViewer(_ => new(
                            isAllowAutoHide: props.AttachProperty(ScrollViewer.AllowAutoHideProperty),
                            isScrollChainingEnabled: props.AttachProperty(ScrollViewer.IsScrollChainingEnabledProperty),
                            isBringIntoViewOnFocusChange: props.AttachProperty(ScrollViewer
                                .BringIntoViewOnFocusChangeProperty),
                            horizontalScrollBarVisibility: props.AttachProperty(ScrollViewer
                                .HorizontalScrollBarVisibilityProperty),
                            verticalScrollBarVisibility: props.AttachProperty(ScrollViewer
                                .VerticalScrollBarVisibilityProperty),
                            style: new(
                                horizontalAlignment: HorizontalAlignment.Stretch,
                                verticalAlignment: VerticalAlignment.Stretch
                            )
                        )
                        {
                            HPanel(_ => new(style: new(
                                horizontalAlignment: HorizontalAlignment.Stretch,
                                verticalAlignment: VerticalAlignment.Stretch
                            ))
                            {
                                HText(text: props.PlaceholderText, style: new(
                                    foreground: props.PlaceholderForeground,
                                    horizontalAlignment: props.HorizontalContentAlignment,
                                    verticalAlignment: props.VerticalContentAlignment,
                                    textAlignment: props.Style.TextAlignment,
                                    textWrapping: props.Style.TextWrapping,
                                    isVisible: new(() =>
                                        string.IsNullOrEmpty(preeditText.RxValue) &&
                                        string.IsNullOrEmpty(props.Text.RxValue))
                                )), // HText
                                part.TextPresenter(HTextPresenter(
                                    bindText: props.Text,
                                    bindPreeditText: preeditText,
                                    passwordChar: props.PasswordChar,
                                    selectionStart: props.SelectionStart,
                                    selectionEnd: props.SelectionEnd,
                                    caretIndex: props.CaretIndex,
                                    caretBlinkInterval: props.CaretBlinkInterval,
                                    isRevealPassword: props.IsRevealPassword,
                                    selectionBrush: props.SelectionBrush,
                                    selectionForegroundBrush: props.SelectionForegroundBrush,
                                    caretBrush: props.CaretBrush,
                                    style: new(
                                        horizontalAlignment: HorizontalAlignment.Stretch,
                                        verticalAlignment: VerticalAlignment.Stretch,
                                        lineHeight: props.Style.LineHeight,
                                        letterSpacing: props.Style.LetterSpacing,
                                        textAlignment: props.Style.TextAlignment,
                                        textWrapping: props.Style.TextWrapping
                                    )
                                )) // part.TextPresenter
                            }) // HPanel
                        })), // [column: 1, colSpan: 1]
                        [column: 0, colSpan: 1] = innerRightContent,
                    }) // HDockPanel.[null]
                });
            }

            var part = new HTextBoxPart();
            // border.Child = template(part).Content;
            var content = template(part).Content;
            border.Child = textBox;
            textBox.Template = new FuncControlTemplate((_, s) =>
            {
                using (new ScopeFrame(uiScope))
                {
                    if (part.TextPresenterElement is not null)
                    {
                        var __ = part.TextPresenterElement.Content;
                        var textPresenter = part.TextPresenterElement.Expose!;
                        textPresenter.Name = "PART_TextPresenter";
                        s.Register("PART_TextPresenter", textPresenter);
                    }

                    if (part.ScrollViewerElement is not null)
                    {
                        var __ = part.ScrollViewerElement.Content;
                        var scrollViewer = part.ScrollViewerElement.Expose!;
                        scrollViewer.Name = "PART_ScrollViewer";
                        s.Register("PART_ScrollViewer", scrollViewer);
                    }
                }

                // return border;
                return content;
            });

            if (args.OnCopyingToClipboard is not null)
            {
                uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                    add: h => textBox.CopyingToClipboard += h,
                    remove: h => textBox.CopyingToClipboard -= h,
                    handler: (_, e) => args.OnCopyingToClipboard(e)
                );
            }

            if (args.OnCopyingToClipboard is not null)
            {
                uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                    add: h => textBox.CopyingToClipboard += h,
                    remove: h => textBox.CopyingToClipboard -= h,
                    handler: (_, e) => args.OnCopyingToClipboard(e)
                );
            }

            if (args.OnPastingFromClipboard is not null)
            {
                uiScope.AddHandler<EventHandler<RoutedEventArgs>>(
                    add: h => textBox.PastingFromClipboard += h,
                    remove: h => textBox.PastingFromClipboard -= h,
                    handler: (_, e) => args.OnPastingFromClipboard(e)
                );
            }

            if (args.OnTextChanging is not null)
            {
                uiScope.AddHandler<EventHandler<TextChangingEventArgs>>(
                    add: h => textBox.TextChanging += h,
                    remove: h => textBox.TextChanging -= h,
                    handler: (_, e) => args.OnTextChanging(e)
                );
            }

            if (args.OnTextChanged is not null)
            {
                uiScope.AddHandler<EventHandler<TextChangedEventArgs>>(
                    add: h => textBox.TextChanged += h,
                    remove: h => textBox.TextChanged -= h,
                    handler: (_, e) => args.OnTextChanged(e)
                );
            }

            return (textBox, border);

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
                    textBox.SelectionBrush = selectionStyle.Background;
                if (selectionStyle.Background is not null && args.SelectionForegroundBrush is null)
                    textBox.SelectionForegroundBrush = selectionStyle.Foreground;
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
        });
    }
}

// textBox.Template = new FuncControlTemplate<TextBox>((control, scope) =>
// {
//     var dockPanel = new DockPanel
//     {
//         [!!Layoutable.HorizontalAlignmentProperty] =
//             control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
//         [!!Layoutable.VerticalAlignmentProperty] =
//             control.GetObservable(Layoutable.VerticalAlignmentProperty).ToBinding(),
//     };
//     var floatingPlaceholder = new TextBlock
//     {
//         [!!TextBlock.ForegroundProperty] = control.GetObservable(TextBlock.ForegroundProperty).ToBinding(),
//         [!!TextBlock.TextProperty] = control.GetObservable(TextBlock.TextProperty).ToBinding(),
//         [!Visual.IsVisibleProperty] = new MultiBinding
//         {
//             Converter = BoolConverters.And,
//             Bindings =
//             [
//                 new Binding("UseFloatingPlaceholder")
//                     { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) },
//                 new Binding("Text")
//                 {
//                     RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
//                     Converter = StringConverters.IsNotNullOrEmpty
//                 }
//             ]
//         }
//     };
//     DockPanel.SetDock(floatingPlaceholder, Dock.Top);
//     dockPanel.Children.Add(floatingPlaceholder);
//
//     var dataValidationErrors = new DataValidationErrors();
//     var grid = new Grid
//     {
//         ColumnDefinitions =
//         [
//             new ColumnDefinition(GridLength.Auto),
//             new ColumnDefinition(GridLength.Star),
//             new ColumnDefinition(GridLength.Auto)
//         ]
//     };
//     var contentPresenter1 = new ContentPresenter
//     {
//         [!!ContentPresenter.ContentProperty] =
//             control.GetObservable(TextBox.InnerLeftContentProperty).ToBinding(),
//     };
//     Grid.SetColumn(contentPresenter1, 0);
//     Grid.SetColumnSpan(contentPresenter1, 1);
//     grid.Children.Add(contentPresenter1);
//
//     var scrollViewer = new ScrollViewer
//     {
//         [!!ScrollViewer.AllowAutoHideProperty] =
//             control.GetObservable(ScrollViewer.AllowAutoHideProperty).ToBinding(),
//         [!!ScrollViewer.BringIntoViewOnFocusChangeProperty] =
//             control.GetObservable(ScrollViewer.BringIntoViewOnFocusChangeProperty).ToBinding(),
//         [!!ScrollViewer.HorizontalScrollBarVisibilityProperty] =
//             control.GetObservable(ScrollViewer.HorizontalScrollBarVisibilityProperty).ToBinding(),
//         [!!ScrollViewer.IsScrollChainingEnabledProperty] =
//             control.GetObservable(ScrollViewer.IsScrollChainingEnabledProperty).ToBinding(),
//         [!!ScrollViewer.VerticalScrollBarVisibilityProperty] =
//             control.GetObservable(ScrollViewer.VerticalScrollBarVisibilityProperty).ToBinding(),
//     };
//
//     var panel = new Panel();
//
//     var presenter = new TextPresenter
//     {
//         Name = "PART_TextPresenter",
//         [!!TextPresenter.CaretBrushProperty] =
//             control.GetObservable(TextBox.CaretBrushProperty).ToBinding(),
//         [!!TextPresenter.CaretIndexProperty] =
//             control.GetObservable(TextBox.CaretIndexProperty).ToBinding(),
//         [!!TextPresenter.LineHeightProperty] =
//             control.GetObservable(TextBox.LineHeightProperty).ToBinding(),
//         [!!TextPresenter.PasswordCharProperty] =
//             control.GetObservable(TextBox.PasswordCharProperty).ToBinding(),
//         [!!TextPresenter.RevealPasswordProperty] =
//             control.GetObservable(TextBox.RevealPasswordProperty).ToBinding(),
//         [!!TextPresenter.SelectionBrushProperty] =
//             control.GetObservable(TextBox.SelectionBrushProperty).ToBinding(),
//         [!!TextPresenter.SelectionEndProperty] =
//             control.GetObservable(TextBox.SelectionEndProperty).ToBinding(),
//         [!!TextPresenter.SelectionForegroundBrushProperty] =
//             control.GetObservable(TextBox.SelectionForegroundBrushProperty).ToBinding(),
//         [!!TextPresenter.SelectionStartProperty] =
//             control.GetObservable(TextBox.SelectionStartProperty).ToBinding(),
//         [!!TextPresenter.TextProperty] = control[!TextBox.TextProperty],
//         [!!TextPresenter.TextAlignmentProperty] =
//             control.GetObservable(TextBox.TextAlignmentProperty).ToBinding(),
//         [!!TextPresenter.TextWrappingProperty] =
//             control.GetObservable(TextBox.TextWrappingProperty).ToBinding(),
//     };
//     scope.Register("PART_TextPresenter", presenter);
//
//     var placeholder = new TextBlock
//     {
//         [!!TextBlock.ForegroundProperty] =
//             control.GetObservable(TextBox.PlaceholderForegroundProperty).ToBinding(),
//         [!!Layoutable.HorizontalAlignmentProperty] =
//             control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
//         [!!Layoutable.VerticalAlignmentProperty] =
//             control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
//         // [!!Visual.OpacityProperty] = control.GetObservable(TextBox.PlaceholderForegroundProperty).ToBinding(),
//         [!!TextBlock.TextProperty] = control.GetObservable(TextBox.PlaceholderTextProperty).ToBinding(),
//         [!!Layoutable.VerticalAlignmentProperty] =
//             control.GetObservable(Layoutable.HorizontalAlignmentProperty).ToBinding(),
//         [!!TextPresenter.TextAlignmentProperty] =
//             control.GetObservable(TextBox.TextAlignmentProperty).ToBinding(),
//         [!!TextPresenter.TextWrappingProperty] =
//             control.GetObservable(TextBox.TextWrappingProperty).ToBinding(),
//         [!Visual.IsVisibleProperty] = new MultiBinding
//         {
//             Converter = BoolConverters.And,
//             Bindings =
//             [
//                 new Binding
//                 {
//                     Source = presenter,
//                     Path = nameof(TextPresenter.PreeditText),
//                     Converter = StringConverters.IsNullOrEmpty
//                 },
//
//                 new TemplateBinding(TextBox.TextProperty)
//                 {
//                     Converter = StringConverters.IsNullOrEmpty
//                 }
//             ]
//         }
//     };
//     panel.Children.Add(placeholder);
//     panel.Children.Add(presenter);
//
//     scrollViewer.Content = panel;
//     Grid.SetColumn(scrollViewer, 1);
//     Grid.SetColumnSpan(scrollViewer, 1);
//     grid.Children.Add(scrollViewer);
//
//     var contentPresenter2 = new ContentPresenter
//     {
//         [!!ContentPresenter.ContentProperty] =
//             control.GetObservable(TextBox.InnerRightContentProperty).ToBinding(),
//     };
//     Grid.SetColumn(contentPresenter2, 2);
//     Grid.SetColumnSpan(contentPresenter2, 1);
//     grid.Children.Add(contentPresenter2);
//
//     dataValidationErrors.Content = grid;
//     dockPanel.Children.Add(dataValidationErrors);
//
//     scope.Register("PART_ScrollViewer", scrollViewer);
//
//     border.Child = dockPanel;
//     return border;
// });