using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using NeHive.Model;
using NeHive.Reactive;

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
    Accessor<IBrush>? background = null,
    Accessor<IBrush>? opacityMask = null,
    Accessor<IBrush>? borderBrush = null,
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
    Accessor<IBrush>? foreground = null
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

    public Accessor<IBrush>? Background = background;
    public Accessor<IBrush>? OpacityMask = opacityMask;
    public Accessor<IBrush>? BorderBrush = borderBrush;
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
    public Accessor<IBrush>? Foreground = foreground;
}

public class StyleProps(Scope scope, Border border, Control control)
{
    public Signal<Thickness> Margin
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Thickness>(border.Margin);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.MarginProperty)
                    sig.RxValue = (Thickness)args.NewValue!;
            }
        }
    }

    public Signal<int> ZIndex
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int>(border.ZIndex);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.ZIndexProperty)
                    sig.RxValue = (int)args.NewValue!;
            }
        }
    }

    public Signal<double> Width
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.Width);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.WidthProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> Height
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.Height);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.HeightProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> MinWidth
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.MinWidth);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.MinWidthProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> MaxWidth
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.MaxWidth);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.MaxWidthProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> MinHeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.MinHeight);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.MinHeightProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> MaxHeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.MaxHeight);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.MaxHeightProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<Thickness> Padding
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Thickness>(border.Padding);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Decorator.PaddingProperty)
                    sig.RxValue = (Thickness)args.NewValue!;
            }
        }
    }

    public Signal<HorizontalAlignment> HorizontalAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<HorizontalAlignment>(border.HorizontalAlignment);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.HorizontalAlignmentProperty)
                    sig.RxValue = (HorizontalAlignment)args.NewValue!;
            }
        }
    }

    public Signal<VerticalAlignment> VerticalAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<VerticalAlignment>(border.VerticalAlignment);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Layoutable.VerticalAlignmentProperty)
                    sig.RxValue = (VerticalAlignment)args.NewValue!;
            }
        }
    }

    public Signal<IBrush?> Background
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IBrush?>(border.Background);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.BackgroundProperty)
                    sig.RxValue = (IBrush?)args.NewValue;
            }
        }
    }

    public Signal<IBrush?> OpacityMask
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IBrush?>(border.OpacityMask);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.OpacityMaskProperty)
                    sig.RxValue = (IBrush?)args.NewValue;
            }
        }
    }

    public Signal<IBrush?> BorderBrush
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IBrush?>(border.BorderBrush);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.BorderBrushProperty)
                    sig.RxValue = (IBrush?)args.NewValue;
            }
        }
    }

    public Signal<Thickness> BorderThickness
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Thickness>(border.BorderThickness);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.BorderThicknessProperty)
                    sig.RxValue = (Thickness)args.NewValue!;
            }
        }
    }

    public Signal<BackgroundSizing> BackgroundSizing
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<BackgroundSizing>(border.BackgroundSizing);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.BackgroundSizingProperty)
                    sig.RxValue = (BackgroundSizing)args.NewValue!;
            }
        }
    }

    public Signal<CornerRadius> CornerRadius
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<CornerRadius>(border.ClipToBoundsRadius);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.CornerRadiusProperty)
                    sig.RxValue = (CornerRadius)args.NewValue!;
            }
        }
    }

    public Signal<double> Opacity
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(border.Opacity);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.OpacityProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsVisible
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(border.IsVisible);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.IsVisibleProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> ClipToBounds
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(border.ClipToBounds);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.ClipToBoundsProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<Geometry?> Clip
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Geometry?>(border.Clip);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.ClipProperty)
                    sig.RxValue = (Geometry?)args.NewValue;
            }
        }
    }

    public Signal<IEffect?> Effect
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<IEffect?>(border.Effect);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.EffectProperty)
                    sig.RxValue = (IEffect?)args.NewValue;
            }
        }
    }

    public Signal<BoxShadows> BoxShadows
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<BoxShadows>(border.BoxShadow);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Border.BoxShadowProperty)
                    sig.RxValue = (BoxShadows)args.NewValue!;
            }
        }
    }

    public Signal<Cursor?> Cursor
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Cursor?>(border.Cursor);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.CursorProperty)
                    sig.RxValue = (Cursor?)args.NewValue;
            }
        }
    }

    public Signal<FlowDirection> FlowDirection
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FlowDirection>(border.FlowDirection);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.FlowDirectionProperty)
                    sig.RxValue = (FlowDirection)args.NewValue!;
            }
        }
    }

    public Signal<RelativePoint> RenderTransformOrigin
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<RelativePoint>(border.RenderTransformOrigin);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.RenderTransformOriginProperty)
                    sig.RxValue = (RelativePoint)args.NewValue!;
            }
        }
    }

    public Signal<ITransform?> RenderTransform
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<ITransform?>(border.RenderTransform);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Visual.RenderTransformProperty)
                    sig.RxValue = (ITransform?)args.NewValue;
            }
        }
    }

    public Signal<Transitions?> Transitions
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Transitions?>(border.Transitions);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => border.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Animatable.TransitionsProperty)
                    sig.RxValue = (Transitions?)args.NewValue;
            }
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

    public Signal<Orientation?> Orientation
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<Orientation?>(null);
            field = sig;

            if (control is StackPanel stackPanel)
            {
                stackPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => stackPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == StackPanel.OrientationProperty)
                        sig.RxValue = (Orientation?)args.NewValue;
                }
            }
            else if (control is WrapPanel wrapPanel)
            {
                wrapPanel.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => wrapPanel.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == WrapPanel.OrientationProperty)
                        sig.RxValue = (Orientation?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<double?> LetterSpacing
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LetterSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.LetterSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<double?> LineHeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LineHeightProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is TextBox textBox)
            {
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.LineHeightProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<double?> LineSpacing
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.LineSpacingProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<int?> MaxLines
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<int?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.MaxLinesProperty)
                        sig.RxValue = (int?)args.NewValue;
                }
            }
            else if (control is TextBox textBox)
            {
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.MaxLinesProperty)
                        sig.RxValue = (int?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<TextTrimming?> TextTrimming
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextTrimming?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextTrimmingProperty)
                        sig.RxValue = (TextTrimming?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<TextAlignment?> TextAlignment
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextAlignment?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextAlignmentProperty)
                        sig.RxValue = (TextAlignment?)args.NewValue;
                }
            }
            else if (control is TextBox textBox)
            {
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.TextAlignmentProperty)
                        sig.RxValue = (TextAlignment?)args.NewValue;
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

    public Signal<TextWrapping?> TextWrapping
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TextWrapping?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.TextWrappingProperty)
                        sig.RxValue = (TextWrapping?)args.NewValue;
                }
            }
            else if (control is TextBox textBox)
            {
                textBox.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBox.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBox.TextWrappingProperty)
                        sig.RxValue = (TextWrapping?)args.NewValue;
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

    public Signal<double?> FontSize
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontSizeProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontSizeProperty)
                        sig.RxValue = (double?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<FontWeight?> FontWeight
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontWeight?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontWeightProperty)
                        sig.RxValue = (FontWeight?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontWeightProperty)
                        sig.RxValue = (FontWeight?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<FontFamily?> FontFamily
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontFamily?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontFamilyProperty)
                        sig.RxValue = (FontFamily?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontFamilyProperty)
                        sig.RxValue = (FontFamily?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<FontStretch?> FontStretch
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontStretch?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontStretchProperty)
                        sig.RxValue = (FontStretch?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontStretchProperty)
                        sig.RxValue = (FontStretch?)args.NewValue;
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
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontFeaturesProperty)
                        sig.RxValue = (FontFeatureCollection?)args.NewValue;
                }
            }

            return field;
        }
    }

    public Signal<FontStyle?> FontStyle
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<FontStyle?>(null);
            field = sig;

            if (control is TextBlock textBlock)
            {
                textBlock.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => textBlock.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TextBlock.FontStyleProperty)
                        sig.RxValue = (FontStyle?)args.NewValue;
                }
            }
            else if (control is TemplatedControl templatedControl)
            {
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.FontStyleProperty)
                        sig.RxValue = (FontStyle?)args.NewValue;
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
                templatedControl.PropertyChanged += OnPropUpdate;
                scope.OnCleanup += () => templatedControl.PropertyChanged -= OnPropUpdate;

                void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
                {
                    if (args.Property == TemplatedControl.ForegroundProperty)
                        sig.RxValue = (IBrush?)args.NewValue;
                }
            }

            return field;
        }
    }
}