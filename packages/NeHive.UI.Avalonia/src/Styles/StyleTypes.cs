using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;

namespace NeHive.UI.Avalonia.Styles;

// 定位 → 尺寸 → 内间距 → 布局排版 → 文字 → 颜色(渐变) → 边框 → 特效
public class BaseStyle
{
    public Thickness? Margin;
    public int? ZIndex;

    public double? Width;
    public double? Height;
    public double? MinWidth;
    public double? MaxWidth;
    public double? MinHeight;
    public double? MaxHeight;

    public Thickness? Padding;

    public HorizontalAlignment? HorizontalAlignment;
    public VerticalAlignment? VerticalAlignment;

    public IBrush? Background;
    public IBrush? OpacityMask;
    public IBrush? BorderBrush;
    public Thickness? BorderThickness;
    public BackgroundSizing? BackgroundSizing;
    public CornerRadius? CornerRadius;

    public double? Opacity;
    public bool? IsVisible;

    public bool? ClipToBounds;
    public Geometry? Clip;
    public IEffect? Effect;
    public List<BoxShadow>? BoxShadows;
    public Cursor? Cursor;
    public FlowDirection? FlowDirection;

    public RelativePoint? RenderTransformOrigin;
    public ITransform? RenderTransform;

    public Transitions? Transitions;
    public IAnimation? Animation;

    internal TempStyle? TempStyle;
}

internal class TempStyle
{
    public double? FgGradientDir;
    public Color? FgFromColor;
    public Color? FgToColor;

    public double? BgGradientDir;
    public Color? BgFromColor;
    public Color? BgToColor;

    public double? BorderGradientDir;
    public Color? BorderFromColor;
    public Color? BorderToColor;

    public double? MaskGradientDir;
    public double? MaskFromColor;
    public double? MaskToColor;

    public bool HasShadow = false;
    public BoxShadow? BoxShadow;
    public Color? RingColor;
    public double? RingWidth;
    public double? RingOffset;

    public TextDecoration? TextDecoration;

    public TransitionScope? TransitionScope;
    public double? Duration;
    public Easing? Easing;

    public TranslateTransform? TranslateTransform;
    public ScaleTransform? ScaleTransform;
    public RotateTransform? RotateTransform;
    public SkewTransform? SkewTransform;
}

public class StyleSet : BaseStyle
{
    public double? GapX;
    public double? GapY;

    public Orientation? Orientation;

    public double? LetterSpacing;
    public double? LineHeight;
    public double? LineSpacing;

    public int? MaxLines;
    public TextTrimming? TextTrimming;

    public TextAlignment? TextAlignment;
    public VerticalAlignment? VerticalTextAlignment;

    public TextWrapping? TextWrapping;
    public TextDecorationCollection? TextDecorations;
    public InlineCollection? Inlines;

    public double? FontSize;
    public FontWeight? FontWeight;
    public FontFamily? FontFamily;
    public FontStretch? FontStretch;
    public FontFeatureCollection? FontFeatures;
    public FontStyle? FontStyle;
    public IBrush? Foreground;
}

public enum TransitionScope
{
    None,
    All,
    Opacity,
    Transform,
    Colors,
    Shadow
}

public struct FullStyle
{
    public StyleSet Normal;

    public Dictionary<string, List<string>> Variants;
}

public readonly struct StyleDefinitions
{
    public Dictionary<string, Action<string[], bool, StyleSet>> Handlers { get; init; }
    public Dictionary<string, Color> Colors { get; init; }
    public Dictionary<string, string[]> Fonts { get; init; }
}

public class HStyle(
    Accessor<Thickness>? margin = null,
    Accessor<int>? zIndex = null,
    Accessor<double>? width = null,
    Accessor<double>? height = null,
    Accessor<double>? minWidth = null,
    Accessor<double>? maxWidth = null,
    Accessor<double>? minHeight = null,
    Accessor<double>? maxHeight = null,
    Accessor<Thickness>? padding = null,
    Accessor<HorizontalAlignment>? horizontalAlignment = null,
    Accessor<VerticalAlignment>? verticalAlignment = null,
    Accessor<IBrush?>? background = null,
    Accessor<IBrush?>? opacityMask = null,
    Accessor<IBrush?>? borderBrush = null,
    Accessor<Thickness>? borderThickness = null,
    Accessor<BackgroundSizing>? backgroundSizing = null,
    Accessor<CornerRadius>? cornerRadius = null,
    Accessor<double>? opacity = null,
    Accessor<bool>? isVisible = null,
    Accessor<bool>? clipToBounds = null,
    Accessor<Geometry>? clip = null,
    Accessor<IEffect>? effect = null,
    Accessor<List<BoxShadow>>? boxShadows = null,
    Accessor<Cursor>? cursor = null,
    Accessor<FlowDirection>? flowDirection = null,
    Accessor<RelativePoint>? renderTransformOrigin = null,
    Accessor<ITransform>? renderTransform = null,
    Accessor<Transitions>? transitions = null,
    Accessor<IAnimation>? animation = null,
    Accessor<double>? gapY = null,
    Accessor<double>? gapX = null,
    Accessor<Orientation>? orientation = null,
    Accessor<double>? letterSpacing = null,
    Accessor<double>? lineHeight = null,
    Accessor<double>? lineSpacing = null,
    Accessor<int>? maxLines = null,
    Accessor<TextTrimming>? textTrimming = null,
    Accessor<TextAlignment>? textAlignment = null,
    Accessor<VerticalAlignment>? verticalTextAlignment = null,
    Accessor<TextWrapping>? textWrapping = null,
    Accessor<TextDecorationCollection>? textDecorations = null,
    Accessor<InlineCollection>? inlines = null,
    Accessor<double>? fontSize = null,
    Accessor<FontWeight>? fontWeight = null,
    Accessor<FontFamily>? fontFamily = null,
    Accessor<FontStretch>? fontStretch = null,
    Accessor<FontFeatureCollection>? fontFeatures = null,
    Accessor<FontStyle>? fontStyle = null,
    Accessor<IBrush?>? foreground = null
)
{
    public Accessor<Thickness>? Margin = margin;
    public Accessor<int>? ZIndex = zIndex;

    public Accessor<double>? Width = width;
    public Accessor<double>? Height = height;
    public Accessor<double>? MinWidth = minWidth;
    public Accessor<double>? MaxWidth = maxWidth;
    public Accessor<double>? MinHeight = minHeight;
    public Accessor<double>? MaxHeight = maxHeight;

    public Accessor<Thickness>? Padding = padding;

    public Accessor<HorizontalAlignment>? HorizontalAlignment = horizontalAlignment;
    public Accessor<VerticalAlignment>? VerticalAlignment = verticalAlignment;

    public Accessor<IBrush?>? Background = background;
    public Accessor<IBrush?>? OpacityMask = opacityMask;
    public Accessor<IBrush?>? BorderBrush = borderBrush;
    public Accessor<Thickness>? BorderThickness = borderThickness;
    public Accessor<BackgroundSizing>? BackgroundSizing = backgroundSizing;
    public Accessor<CornerRadius>? CornerRadius = cornerRadius;

    public Accessor<double>? Opacity = opacity;
    public Accessor<bool>? IsVisible = isVisible;

    public Accessor<bool>? ClipToBounds = clipToBounds;
    public Accessor<Geometry>? Clip = clip;
    public Accessor<IEffect>? Effect = effect;
    public Accessor<List<BoxShadow>>? BoxShadows = boxShadows;
    public Accessor<Cursor>? Cursor = cursor;
    public Accessor<FlowDirection>? FlowDirection = flowDirection;

    public Accessor<RelativePoint>? RenderTransformOrigin = renderTransformOrigin;
    public Accessor<ITransform>? RenderTransform = renderTransform;

    public Accessor<Transitions>? Transitions = transitions;
    public Accessor<IAnimation>? Animation = animation;

    public Accessor<double>? GapX = gapX;
    public Accessor<double>? GapY = gapY;

    public Accessor<Orientation>? Orientation = orientation;

    public Accessor<double>? LetterSpacing = letterSpacing;
    public Accessor<double>? LineHeight = lineHeight;
    public Accessor<double>? LineSpacing = lineSpacing;

    public Accessor<int>? MaxLines = maxLines;
    public Accessor<TextTrimming>? TextTrimming = textTrimming;

    public Accessor<TextAlignment>? TextAlignment = textAlignment;
    public Accessor<VerticalAlignment>? VerticalTextAlignment = verticalTextAlignment;

    public Accessor<TextWrapping>? TextWrapping = textWrapping;
    public Accessor<TextDecorationCollection>? TextDecorations = textDecorations;
    public Accessor<InlineCollection>? Inlines = inlines;

    public Accessor<double>? FontSize = fontSize;
    public Accessor<FontWeight>? FontWeight = fontWeight;
    public Accessor<FontFamily>? FontFamily = fontFamily;
    public Accessor<FontStretch>? FontStretch = fontStretch;
    public Accessor<FontFeatureCollection>? FontFeatures = fontFeatures;
    public Accessor<FontStyle>? FontStyle = fontStyle;
    public Accessor<IBrush?>? Foreground = foreground;
}

public class StyleProps(Scope scope, Border border, Control control)
{
    public Signal<Thickness> Margin
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.MarginProperty, border.Margin);
            return field;
        }
    }

    public Signal<int> ZIndex
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.ZIndexProperty, border.ZIndex);
            return field;
        }
    }

    public Signal<double> Width
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.WidthProperty, border.Width);
            return field;
        }
    }

    public Signal<double> Height
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.HeightProperty, border.Height);
            return field;
        }
    }

    public Signal<double> MinWidth
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.MinWidthProperty, border.MinWidth);
            return field;
        }
    }

    public Signal<double> MaxWidth
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.MaxWidthProperty, border.MaxWidth);
            return field;
        }
    }

    public Signal<double> MinHeight
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.MinHeightProperty, border.MinHeight);
            return field;
        }
    }

    public Signal<double> MaxHeight
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.MaxHeightProperty, border.MaxHeight);
            return field;
        }
    }

    public Signal<Thickness> Padding
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Decorator.PaddingProperty, border.Padding);
            return field;
        }
    }

    public Signal<HorizontalAlignment> HorizontalAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.HorizontalAlignmentProperty, border.HorizontalAlignment);
            return field;
        }
    }

    public Signal<VerticalAlignment> VerticalAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Layoutable.VerticalAlignmentProperty, border.VerticalAlignment);
            return field;
        }
    }

    public Signal<IBrush?> Background
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.BackgroundProperty, border.Background);
            return field;
        }
    }

    public Signal<IBrush?> OpacityMask
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.OpacityMaskProperty, border.OpacityMask);
            return field;
        }
    }

    public Signal<IBrush?> BorderBrush
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.BorderBrushProperty, border.BorderBrush);
            return field;
        }
    }

    public Signal<Thickness> BorderThickness
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.BorderThicknessProperty, border.BorderThickness);
            return field;
        }
    }

    public Signal<BackgroundSizing> BackgroundSizing
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.BackgroundSizingProperty, border.BackgroundSizing);
            return field;
        }
    }

    public Signal<CornerRadius> CornerRadius
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.CornerRadiusProperty, border.CornerRadius);
            return field;
        }
    }

    public Signal<double> Opacity
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.OpacityProperty, border.Opacity);
            return field;
        }
    }

    public Signal<bool> IsVisible
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.IsVisibleProperty, border.IsVisible);
            return field;
        }
    }

    public Signal<bool> ClipToBounds
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.ClipToBoundsProperty, border.ClipToBounds);
            return field;
        }
    }

    public Signal<Geometry?> Clip
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.ClipProperty, border.Clip);
            return field;
        }
    }

    public Signal<IEffect?> Effect
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.EffectProperty, border.Effect);
            return field;
        }
    }

    public Signal<BoxShadows> BoxShadow
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Border.BoxShadowProperty, border.BoxShadow);
            return field;
        }
    }

    public Signal<Cursor?> Cursor
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                InputElement.CursorProperty, border.Cursor);
            return field;
        }
    }

    public Signal<FlowDirection> FlowDirection
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.FlowDirectionProperty, border.FlowDirection);
            return field;
        }
    }

    public Signal<RelativePoint> RenderTransformOrigin
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.RenderTransformOriginProperty, border.RenderTransformOrigin);
            return field;
        }
    }

    public Signal<ITransform?> RenderTransform
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Visual.RenderTransformProperty, border.RenderTransform);
            return field;
        }
    }

    public Signal<Transitions?> Transitions
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, border,
                Animatable.TransitionsProperty, border.Transitions);
            return field;
        }
    }

    public Signal<double?> GapX
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is StackPanel stackPanel)
            {
                stackPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => stackPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == StackPanel.SpacingProperty &&
                        stackPanel.Orientation is global::Avalonia.Layout.Orientation.Horizontal)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is WrapPanel wrapPanel)
            {
                wrapPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => wrapPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if ((args.Property == WrapPanel.ItemSpacingProperty &&
                         wrapPanel.Orientation is global::Avalonia.Layout.Orientation.Horizontal)
                        || (args.Property == WrapPanel.LineSpacingProperty &&
                            wrapPanel.Orientation is global::Avalonia.Layout.Orientation.Vertical))
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is DockPanel dockPanel)
            {
                dockPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => dockPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == DockPanel.HorizontalSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is Grid grid)
            {
                grid.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => grid.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == Grid.ColumnSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is UniformGrid uniformGrid)
            {
                uniformGrid.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => uniformGrid.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == UniformGrid.ColumnSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<double?> GapY
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is StackPanel stackPanel)
            {
                stackPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => stackPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == StackPanel.SpacingProperty &&
                        stackPanel.Orientation is global::Avalonia.Layout.Orientation.Vertical)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is WrapPanel wrapPanel)
            {
                wrapPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => wrapPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if ((args.Property == WrapPanel.ItemSpacingProperty &&
                         wrapPanel.Orientation is global::Avalonia.Layout.Orientation.Vertical)
                        || (args.Property == WrapPanel.LineSpacingProperty &&
                            wrapPanel.Orientation is global::Avalonia.Layout.Orientation.Horizontal))
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is DockPanel dockPanel)
            {
                dockPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => dockPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == DockPanel.VerticalSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is Grid grid)
            {
                grid.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => grid.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == Grid.RowSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is UniformGrid uniformGrid)
            {
                uniformGrid.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => uniformGrid.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == UniformGrid.RowSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<Orientation> Orientation
    {
        get
        {
            if (field is not null) return field;
            field = control switch
            {
                StackPanel stackPanel => BridgeAvalonia.CreatePropertySignal(scope, stackPanel,
                    StackPanel.OrientationProperty, stackPanel.Orientation),
                WrapPanel wrapPanel => BridgeAvalonia.CreatePropertySignal(scope, wrapPanel,
                    WrapPanel.OrientationProperty, wrapPanel.Orientation),
                ProgressBar progressBar => BridgeAvalonia.CreatePropertySignal(scope, progressBar,
                    ProgressBar.OrientationProperty, progressBar.Orientation),
                ScrollBar scrollBar => BridgeAvalonia.CreatePropertySignal(scope, scrollBar,
                    ScrollBar.OrientationProperty, scrollBar.Orientation),
                Slider slider => BridgeAvalonia.CreatePropertySignal(scope, slider,
                    Slider.OrientationProperty, slider.Orientation),
                TickBar tickBar => BridgeAvalonia.CreatePropertySignal(scope, tickBar,
                    TickBar.OrientationProperty, tickBar.Orientation),
                _ => new MutSignal<Orientation>(global::Avalonia.Layout.Orientation.Vertical)
            };
            return field;
        }
    }

    public Signal<double> LetterSpacing
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(0d);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.LetterSpacing;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LetterSpacingProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.LetterSpacing;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.LetterSpacingProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.LetterSpacing;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextPresenter.LetterSpacingProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<double> LineHeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(0d);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.LineHeight;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LineHeightProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TextBox textBox)
            {
                sig.RxValue = textBox.LineHeight;
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.LineHeightProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.LineHeight;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextPresenter.LineHeightProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<double> LineSpacing
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(0d);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.LineSpacing;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LineSpacingProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<int> MaxLines
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(0);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.MaxLines;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.MaxLinesProperty)
                        sig.RxValue = (int)args.NewValue!;
                }
            }
            else if (control is TextBox textBox)
            {
                sig.RxValue = textBox.MaxLines;
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.MaxLinesProperty)
                        sig.RxValue = (int)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<TextTrimming> TextTrimming
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextTrimming>(global::Avalonia.Media.TextTrimming.None);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.TextTrimming;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextTrimmingProperty)
                        sig.RxValue = (TextTrimming)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<TextAlignment> TextAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextAlignment>(global::Avalonia.Media.TextAlignment.Start);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.TextAlignment;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextAlignmentProperty)
                        sig.RxValue = (TextAlignment)args.NewValue!;
                }
            }
            else if (control is TextBox textBox)
            {
                sig.RxValue = textBox.TextAlignment;
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.TextAlignmentProperty)
                        sig.RxValue = (TextAlignment)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.TextAlignment;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextPresenter.TextAlignmentProperty)
                        sig.RxValue = (TextAlignment)args.NewValue!;
                }
            }

            return field;
        }
    }
    //
    // public Signal<VerticalAlignment?> VerticalTextAlignment
    // {
    //     get
    //     {
    //         if (field is not null) return field;
    //         var sig = new MutSignal<VerticalAlignment?>(null);
    //         field = sig;
    //
    //         if (control is TextBlock textBlock)
    //         {
    //             textBlock.PropertyChanged += OnPropUpdate;
    //             scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;
    //
    //             void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
    //             {
    //                 if (args.Property == TextBlock.VerticalAlignmentProperty)
    //                     sig.RxValue = (VerticalAlignment?)args.NewValue;
    //             }
    //         }
    //         else if (control is TextBox textBox)
    //         {
    //             textBox.PropertyChanged += OnPropUpdate;
    //             scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;
    //
    //             void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
    //             {
    //                 if (args.Property == TextBox.TextAlignmentProperty)
    //                     sig.RxValue = (TextAlignment?)args.NewValue;
    //             }
    //         }
    //
    //         return field;
    //     }
    // }

    public Signal<TextWrapping> TextWrapping
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextWrapping>(global::Avalonia.Media.TextWrapping.NoWrap);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.TextWrapping;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextWrappingProperty)
                        sig.RxValue = (TextWrapping)args.NewValue!;
                }
            }
            else if (control is TextBox textBox)
            {
                sig.RxValue = textBox.TextWrapping;
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.TextWrappingProperty)
                        sig.RxValue = (TextWrapping)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.TextWrapping;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextPresenter.TextWrappingProperty)
                        sig.RxValue = (TextWrapping)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<TextDecorationCollection?> TextDecorations
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextDecorationCollection?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.TextDecorations;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextDecorationsProperty)
                        sig.RxValue = (TextDecorationCollection?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<InlineCollection?> Inlines
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<InlineCollection?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.Inlines;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.InlinesProperty)
                        sig.RxValue = (InlineCollection?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<double> FontSize
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(14d);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontSize;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontSizeProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontSize;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontSizeProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontSize;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontSizeProperty)
                        sig.RxValue = (double)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<FontWeight> FontWeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontWeight>(global::Avalonia.Media.FontWeight.Normal);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontWeight;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontWeightProperty)
                        sig.RxValue = (FontWeight)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontWeight;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontWeightProperty)
                        sig.RxValue = (FontWeight)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontWeight;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontWeightProperty)
                        sig.RxValue = (FontWeight)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<FontFamily> FontFamily
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontFamily>(default!);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontFamily;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontFamilyProperty)
                        sig.RxValue = (FontFamily)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontFamily;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontFamilyProperty)
                        sig.RxValue = (FontFamily)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontFamily;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontFamilyProperty)
                        sig.RxValue = (FontFamily)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<FontStretch> FontStretch
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontStretch>(global::Avalonia.Media.FontStretch.Normal);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontStretch;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontStretchProperty)
                        sig.RxValue = (FontStretch)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontStretch;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontStretchProperty)
                        sig.RxValue = (FontStretch)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontStretch;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontStretchProperty)
                        sig.RxValue = (FontStretch)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<FontFeatureCollection?> FontFeatures
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontFeatureCollection?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontFeatures;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontFeaturesProperty)
                        sig.RxValue = (FontFeatureCollection?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontFeatures;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontFeaturesProperty)
                        sig.RxValue = (FontFeatureCollection?)args.NewValue;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontFeatures;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontFeaturesProperty)
                        sig.RxValue = (FontFeatureCollection?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<FontStyle> FontStyle
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontStyle>(global::Avalonia.Media.FontStyle.Normal);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.FontStyle;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontStyleProperty)
                        sig.RxValue = (FontStyle)args.NewValue!;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.FontStyle;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontStyleProperty)
                        sig.RxValue = (FontStyle)args.NewValue!;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.FontStyle;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontFeaturesProperty)
                        sig.RxValue = (FontStyle)args.NewValue!;
                }
            }

            return field;
        }
    }

    public Signal<IBrush?> Foreground
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IBrush?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                sig.RxValue = textBlock.Foreground;
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.ForegroundProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                sig.RxValue = templatedControl.Foreground;
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.ForegroundProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }
            else if (control is TextPresenter presenter)
            {
                sig.RxValue = presenter.Foreground;
                presenter.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => presenter.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextElement.FontFeaturesProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }
            else if (control is Shape shape)
            {
                sig.RxValue = shape.Fill;
                shape.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => shape.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == Shape.FillProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }
            else if (control is TickBar tickBar)
            {
                sig.RxValue = tickBar.Fill;
                tickBar.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => tickBar.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TickBar.FillProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }

            return field;
        }
    }
}