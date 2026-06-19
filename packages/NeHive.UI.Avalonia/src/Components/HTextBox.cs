using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.TextInput;
using Avalonia.Threading;
using NeHive.Reactive;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Styles;
using System.Globalization;
 
namespace NeHive.UI.Avalonia.Components;
 
public static class HTextBoxStyle
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
 
public class HTextBoxExpose
{
    public Action<string>? TextChanged = _ => { };
    public Action<bool>? FocusChanged = _ => { };
}
 
public class HTextBoxTextInputClient : TextInputMethodClient
{
    private readonly TextBlock _textBlock;
    private readonly Border _caret;
 
    public Func<string>? GetText { get; set; }
    public Func<int>? GetCaretIndex { get; set; }
    public Func<(int start, int end)>? GetSelection { get; set; }
 
    public HTextBoxTextInputClient(TextBlock textBlock, Border caret)
    {
        _textBlock = textBlock;
        _caret = caret;
    }
 
    public override Visual TextViewVisual => _textBlock;
    public override bool SupportsPreedit => false;
    public override bool SupportsSurroundingText => true;
    public override string SurroundingText => GetText?.Invoke() ?? string.Empty;
 
    public override Rect CursorRectangle
    {
        get
        {
            var text = GetText?.Invoke() ?? string.Empty;
            var caretIndex = GetCaretIndex?.Invoke() ?? 0;
            var textToCaret = text.Substring(0, Math.Min(caretIndex, text.Length));
 
            var typeface = new Typeface(_textBlock.FontFamily, _textBlock.FontStyle, _textBlock.FontWeight);
            var culture = CultureInfo.CurrentCulture;
 
            var width = MeasureTextWithTrailingSpaces(textToCaret, typeface, culture, _textBlock.FontSize);
 
            var height = _textBlock.Bounds.Height > 0 ? _textBlock.Bounds.Height : _textBlock.FontSize * 1.2;
            return new Rect(width, height / 2, _caret.Width, height);
        }
    }
 
    public override TextSelection Selection
    {
        get
        {
            var (start, end) = GetSelection?.Invoke() ?? (0, 0);
            return new TextSelection(start, end);
        }
        set { }
    }
 
    public static double MeasureTextWithTrailingSpaces(string text, Typeface typeface, CultureInfo culture, double fontSize)
    {
        if (string.IsNullOrEmpty(text)) return 0;
 
        int trailingSpaces = 0;
        for (int i = text.Length - 1; i >= 0; i--)
        {
            if (char.IsWhiteSpace(text[i])) trailingSpaces++;
            else break;
        }
 
        var mainText = trailingSpaces > 0 ? text.Substring(0, text.Length - trailingSpaces) : text;
 
        double width = 0;
        if (mainText.Length > 0)
        {
            var formattedMainText = new FormattedText(
                mainText,
                culture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                null);
            width += formattedMainText.Width;
        }
 
        if (trailingSpaces > 0)
        {
            var formattedSpace1 = new FormattedText(
                "a",
                culture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                null);
            var formattedSpace2 = new FormattedText(
                " a",
                culture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                null);
 
            var formattedSpace = formattedSpace2.Width - formattedSpace1.Width;
            width += formattedSpace * trailingSpaces;
        }
 
        return width;
    }
 
    public void NotifyCursorChanged() => RaiseCursorRectangleChanged();
    public void NotifyTextChanged() => RaiseSurroundingTextChanged();
}
 
public static partial class BaseComponent
{
    public static IElement<HTextBoxExpose> HTextBox(
        MutSignal<string> bindText,
        Accessor<bool>? selectable = null,
        Accessor<bool>? editable = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<string>? onTextChanged = null)
    {
        editable ??= true;
        var styleAccessor = StyleParser.ParseFull(strStyle, HTextBoxStyle.DefaultStyleSet, style);
 
        UiScope uiScope = new();
 
        var textBlock = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center
        };
 
        var caret = new Border
        {
            Width = 1,
            Background = Brushes.Black,
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch
        };
 
        // 选区背景可视化控件
        var selectionBg = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 120, 215)), // 系统选区蓝
            IsVisible = false,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Left
        };
 
        var contentPanel = new Panel();
        contentPanel.Children.Add(selectionBg); // 最底层
        contentPanel.Children.Add(textBlock);   // 中间层
        contentPanel.Children.Add(caret);       // 最顶层
 
        var border = new Border
        {
            Child = contentPanel,
            Focusable = true
        };
 
        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };
 
        state.ApplyAccessorStyle(styleAccessor, textBlock, border, ApplyStyle);
        state.ApplyVariantsStyle(textBlock, border, ApplyStyle);
 
        textBlock.Text = bindText.Value;
        uiScope.CreateEffect(() => textBlock.Text = bindText.RxValue);
 
        var expose = new HTextBoxExpose();
        var caretIndex = 0;
        var selAnchor = 0;
        var isFocused = false;
        var isSelecting = false;
 
        var imeClient = new HTextBoxTextInputClient(textBlock, caret)
        {
            GetText = () => bindText.Value,
            GetCaretIndex = () => caretIndex,
            GetSelection = () => (Math.Min(selAnchor, caretIndex), Math.Max(selAnchor, caretIndex))
        };
 
        border.TextInputMethodClientRequested += (_, e) => e.Client = imeClient;
 
        var caretTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(530) };
        caretTimer.Tick += (_, _) => caret.IsVisible = !caret.IsVisible;
 
        uiScope.OnCleanup += () => caretTimer.Stop();
 
        uiScope.OnMount += () =>
        {
            uiScope.CreateEffect(epochScope =>
            {
                var isEditable = epochScope.Track(editable);
                if (!isEditable && isFocused)
                {
                    caret.IsVisible = false;
                    caretTimer.Stop();
                    selectionBg.IsVisible = false;
                }
                else if (isEditable && isFocused)
                {
                    caret.IsVisible = true;
                    caretTimer.Start();
                    UpdateSelectionVisual();
                }
            });
 
            border.GotFocus += (_, _) =>
            {
                isFocused = true;
                if (editable.Value)
                {
                    caret.IsVisible = true;
                    caretTimer.Start();
                }
 
                UpdateCaretPosition();
                UpdateSelectionVisual();
                expose.FocusChanged?.Invoke(true);
            };
 
            border.LostFocus += (_, _) =>
            {
                isFocused = false;
                caret.IsVisible = false;
                caretTimer.Stop();
                selAnchor = caretIndex; // 失去焦点时清除选区
                selectionBg.IsVisible = false;
                expose.FocusChanged?.Invoke(false);
            };
 
            border.PointerPressed += (_, e) =>
            {
                border.Focus();
                e.Handled = true;
 
                if (!editable.Value) return;
 
                var point = e.GetPosition(textBlock);
                caretIndex = GetCaretIndexFromPoint(point);
 
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && isFocused)
                {
                    // 按住Shift扩展选区，保持锚点不动
                }
                else
                {
                    selAnchor = caretIndex;
                }
 
                isSelecting = true;
                BlinkCaret();
            };
 
            border.PointerMoved += (_, e) =>
            {
                if (!isSelecting || !editable.Value) return;
 
                var point = e.GetPosition(textBlock);
                caretIndex = GetCaretIndexFromPoint(point);
                
                BlinkCaret();
            };
 
            border.PointerReleased += (_, _) =>
            {
                isSelecting = false;
            };
 
            border.TextInput += (_, e) =>
            {
                if (!editable.Value || !isFocused) return;
 
                // 如果有选区，先删除选中的文本
                if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex))
                {
                    DeleteSelection();
                }
 
                var currentText = bindText.Value;
                var t = e.Text ?? "";
                bindText.RxValue = currentText.Insert(caretIndex, t);
                caretIndex += t.Length;
                selAnchor = caretIndex;
 
                BlinkCaret();
                imeClient.NotifyTextChanged();
                onTextChanged?.Invoke(bindText.Value);
                expose.TextChanged?.Invoke(bindText.Value);
                e.Handled = true;
            };
 
            border.KeyDown += async (_, e) =>
            {
                if (!editable.Value || !isFocused) return;
 
                var currentText = bindText.Value;
                var handled = true;
                var clipboard = TopLevel.GetTopLevel(border)?.Clipboard;
 
                switch (e.Key)
                {
                    case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                        if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex) && clipboard != null)
                        {
                            var selectedText = currentText.Substring(Math.Min(selAnchor, caretIndex), Math.Max(selAnchor, caretIndex) - Math.Min(selAnchor, caretIndex));
                            await clipboard.SetTextAsync(selectedText);
                        }
                        break;
                    case Key.X when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                        if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex) && clipboard != null)
                        {
                            var selectedText = currentText.Substring(Math.Min(selAnchor, caretIndex), Math.Max(selAnchor, caretIndex) - Math.Min(selAnchor, caretIndex));
                            await clipboard.SetTextAsync(selectedText);
                            DeleteSelection();
                        }
                        break;
                    case Key.V when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                        if (clipboard != null)
                        {
                            var pasteText = await clipboard.TryGetTextAsync();
                            if (!string.IsNullOrEmpty(pasteText))
                            {
                                if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex))
                                {
                                    DeleteSelection();
                                    currentText = bindText.Value; // 删除后更新当前文本
                                }
                                bindText.RxValue = currentText.Insert(caretIndex, pasteText);
                                caretIndex += pasteText.Length;
                                selAnchor = caretIndex;
                                imeClient.NotifyTextChanged();
                                onTextChanged?.Invoke(bindText.Value);
                                expose.TextChanged?.Invoke(bindText.Value);
                            }
                        }
                        break;
                    case Key.A when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                        selAnchor = 0;
                        caretIndex = currentText.Length;
                        break;
                    case Key.Back:
                        if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex))
                        {
                            DeleteSelection();
                        }
                        else if (caretIndex > 0)
                        {
                            var t = currentText.Remove(caretIndex - 1, 1);
                            caretIndex--;
                            selAnchor = caretIndex;
                            imeClient.NotifyTextChanged();
                            bindText.RxValue = t;
                            onTextChanged?.Invoke(t);
                            expose.TextChanged?.Invoke(t);
                        }
                        break;
                    case Key.Delete:
                        if (Math.Max(selAnchor, caretIndex) > Math.Min(selAnchor, caretIndex))
                        {
                            DeleteSelection();
                        }
                        else if (caretIndex < currentText.Length)
                        {
                            var t = currentText.Remove(caretIndex, 1);
                            imeClient.NotifyTextChanged();
                            bindText.RxValue = t;
                            onTextChanged?.Invoke(t);
                            expose.TextChanged?.Invoke(t);
                        }
                        break;
                    case Key.Left:
                        if (caretIndex > 0) caretIndex--;
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) selAnchor = caretIndex;
                        break;
                    case Key.Right:
                        if (caretIndex < currentText.Length) caretIndex++;
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) selAnchor = caretIndex;
                        break;
                    case Key.Home:
                        caretIndex = 0;
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) selAnchor = caretIndex;
                        break;
                    case Key.End:
                        caretIndex = currentText.Length;
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) selAnchor = caretIndex;
                        break;
                    default:
                        handled = false;
                        break;
                }
 
                if (handled)
                {
                    BlinkCaret();
                    e.Handled = true;
                }
            };
        };
 
        return new Element<HTextBoxExpose>(uiScope, border, expose);
 
        int GetCaretIndexFromPoint(Point point)
        {
            var text = textBlock.Text ?? "";
            if (string.IsNullOrEmpty(text)) return 0;
 
            int newIndex = 0;
            double currentWidth = 0;
            var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight);
            var culture = CultureInfo.CurrentCulture;
 
            for (int i = 0; i < text.Length; i++)
            {
                var subText = text.Substring(0, i + 1);
                double nextWidth =
                    HTextBoxTextInputClient.MeasureTextWithTrailingSpaces(subText, typeface, culture,
                        textBlock.FontSize);
 
                if (point.X <= nextWidth)
                {
                    double charWidth = nextWidth - currentWidth;
                    if (point.X < currentWidth + charWidth / 2)
                    {
                        newIndex = i;
                    }
                    else
                    {
                        newIndex = i + 1;
                    }
                    break;
                }
 
                currentWidth = nextWidth;
                newIndex = i + 1;
            }
            return newIndex;
        }
 
        void DeleteSelection()
        {
            int start = Math.Min(selAnchor, caretIndex);
            int end = Math.Max(selAnchor, caretIndex);
            if (end <= start) return;
 
            var currentText = bindText.Value;
            var t = currentText.Remove(start, end - start);
            caretIndex = start;
            selAnchor = caretIndex;
            imeClient.NotifyTextChanged();
            bindText.RxValue = t;
            onTextChanged?.Invoke(t);
            expose.TextChanged?.Invoke(t);
            UpdateSelectionVisual();
        }
 
        void BlinkCaret()
        {
            if (!isFocused || !editable.Value) return;
            caret.IsVisible = true;
            caretTimer.Stop();
            caretTimer.Start();
            UpdateCaretPosition();
            UpdateSelectionVisual();
        }
 
        void UpdateCaretPosition()
        {
            var text = textBlock.Text ?? "";
            var idx = Math.Min(caretIndex, text.Length);
            var textToCaret = text.Substring(0, idx);
 
            var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight);
            var culture = CultureInfo.CurrentCulture;
 
            double width =
                HTextBoxTextInputClient.MeasureTextWithTrailingSpaces(textToCaret, typeface, culture,
                    textBlock.FontSize);
 
            caret.Margin = new Thickness(width, 0, 0, 0);
            imeClient.NotifyCursorChanged();
        }
 
        void UpdateSelectionVisual()
        {
            int start = Math.Min(selAnchor, caretIndex);
            int end = Math.Max(selAnchor, caretIndex);
            if (end > start)
            {
                selectionBg.IsVisible = true;
                var text = textBlock.Text ?? "";
                var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight);
                var culture = CultureInfo.CurrentCulture;
 
                double x1 = HTextBoxTextInputClient.MeasureTextWithTrailingSpaces(text.Substring(0, Math.Min(start, text.Length)), typeface, culture, textBlock.FontSize);
                double x2 = HTextBoxTextInputClient.MeasureTextWithTrailingSpaces(text.Substring(0, Math.Min(end, text.Length)), typeface, culture, textBlock.FontSize);
 
                selectionBg.Margin = new Thickness(x1, 0, 0, 0);
                selectionBg.Width = x2 - x1;
            }
            else
            {
                selectionBg.IsVisible = false;
            }
        }
 
        void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
        {
            StyleUtil.ApplyStyle(styleValue, textBlock, bord);
 
            if (styleValue.TextAlignment is not null) textBlock.TextAlignment = styleValue.TextAlignment.Value;
            if (styleValue.TextWrapping is not null) textBlock.TextWrapping = styleValue.TextWrapping.Value;
            if (styleValue.Foreground is not null)
            {
                textBlock.Foreground = styleValue.Foreground;
                caret.Background = styleValue.Foreground;
            }
 
            if (styleValue.FontSize is not null) textBlock.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) textBlock.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) textBlock.FontStyle = styleValue.FontStyle.Value;
        }
    }
 
    public static IElement<HTextBoxExpose> HTextBox(
        out HTextBoxExpose expose,
        MutSignal<string> bindText,
        Accessor<bool>? selectable = null,
        Accessor<bool>? editable = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        var el = HTextBox(bindText, selectable, editable, strStyle, style, variants);
        expose = el.Expose;
        return el;
    }
}