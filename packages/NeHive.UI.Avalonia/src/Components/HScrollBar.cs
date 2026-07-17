using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HScrollBarPart
{
    internal IElement<Button>? LineDownButtonElement { get; private set; }
    internal IElement<Button>? LineUpButtonElement { get; private set; }
    internal IElement<Button>? PageDownButtonElement { get; private set; }
    internal IElement<Button>? PageUpButtonElement { get; private set; }

    public IElement<Button> LineDownButton(IElement<Button> element) => LineDownButtonElement = element;
    public IElement<Button> LineUpButton(IElement<Button> element) => LineUpButtonElement = element;
    public IElement<Button> PageDownButton(IElement<Button> element) => PageDownButtonElement = element;
    public IElement<Button> PageUpButton(IElement<Button> element) => PageUpButtonElement = element;
}

public class HScrollBarProps(Scope scope, ScrollBar scrollBar)
{
    public MutSignal<double> Value
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.Value);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;
            scope.CreateEffect(epoch => scrollBar.Value = epoch.Pull(sig));

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.ValueProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> Minimum
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.Minimum);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.MinimumProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> Maximum
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.Maximum);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.MaximumProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> SmallChange
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.SmallChange);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.SmallChangeProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> LargeChange
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.LargeChange);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == RangeBase.LargeChangeProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<double> ViewportSize
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<double>(scrollBar.ViewportSize);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollBar.ViewportSizeProperty)
                    sig.RxValue = (double)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsAllowAutoHide
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(scrollBar.AllowAutoHide);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollBar.AllowAutoHideProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsDeferredScrollingEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(ScrollViewer.GetIsDeferredScrollingEnabled(scrollBar));
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollViewer.IsDeferredScrollingEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<ScrollBarVisibility> Visibility
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<ScrollBarVisibility>(scrollBar.Visibility);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollBar.VisibilityProperty)
                    sig.RxValue = (ScrollBarVisibility)args.NewValue!;
            }
        }
    }

    public Signal<TimeSpan> HideDelay
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TimeSpan>(scrollBar.HideDelay);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollBar.HideDelayProperty)
                    sig.RxValue = (TimeSpan)args.NewValue!;
            }
        }
    }

    public Signal<TimeSpan> ShowDelay
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<TimeSpan>(scrollBar.ShowDelay);
            field = sig;

            scrollBar.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => scrollBar.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ScrollBar.ShowDelayProperty)
                    sig.RxValue = (TimeSpan)args.NewValue!;
            }
        }
    }
}

public class HScrollBarArgs(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<bool>? isAllowAutoHide = null,
    Accessor<bool>? isDeferredScrollingEnabled = null,
    Accessor<double>? viewportSize = null,
    Accessor<ScrollBarVisibility>? visibility = null,
    Accessor<TimeSpan>? showDelay = null,
    Accessor<TimeSpan>? hideDelay = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null,
    Action<ScrollEventArgs>? onScroll = null)
{
    public readonly Accessor<double>? Value = bindValue ?? value;
    public readonly MutSignal<double>? BindValue = bindValue;

    public readonly Accessor<double> Minimum = minimum ?? 0.0;
    public readonly Accessor<double> Maximum = maximum ?? 100.0;
    public readonly Accessor<double>? SmallChange = smallChange;
    public readonly Accessor<double>? LargeChange = largeChange;
    public readonly Accessor<double>? ViewportSize = viewportSize;

    public readonly Accessor<bool>? IsAllowAutoHide = isAllowAutoHide;
    public readonly Accessor<bool>? IsDeferredScrollingEnabled = isDeferredScrollingEnabled;
    public readonly Accessor<ScrollBarVisibility>? Visibility = visibility;
    public readonly Accessor<TimeSpan>? HideDelay = hideDelay;
    public readonly Accessor<TimeSpan>? ShowDelay = showDelay;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public readonly Action<RangeBaseValueChangedEventArgs>? OnValueChanged = onValueChanged;
    public readonly Action<ScrollEventArgs>? OnScroll = onScroll;

    public Func<HScrollBarProps, HScrollBarPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<ScrollBar> HScrollBar(HScrollBarArgs args)
    {
        return Element<ScrollBar>.WithScope(uiScope =>
        {
            var scrollBar = new ScrollBar();

            var border = new Border();

            var state = new CommonState(uiScope, args.Style.Value.Normal)
            {
                StrVariants = args.Style.Value.Variants,
                Variants = args.Variants
            };

            state.ApplyAccessorStyle(args.Style, scrollBar, border, ApplyStyle);
            state.ApplyVariantsStyle(scrollBar, border, ApplyStyle);

            RangeBaseUtil.BindAccessor(uiScope, scrollBar, args.Value, args.BindValue, args.Minimum, args.Maximum,
                args.SmallChange, args.LargeChange, args.OnValueChanged);

            if (args.IsAllowAutoHide is not null)
            {
                scrollBar.AllowAutoHide = args.IsAllowAutoHide.Value;
                if (args.IsAllowAutoHide.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.AllowAutoHide = scope.Track(args.IsAllowAutoHide));
            }

            if (args.IsDeferredScrollingEnabled is not null)
            {
                ScrollViewer.SetIsDeferredScrollingEnabled(scrollBar, args.IsDeferredScrollingEnabled.Value);
                if (args.IsDeferredScrollingEnabled.IsReactive)
                    uiScope.CreateEffect(scope =>
                        ScrollViewer.SetIsDeferredScrollingEnabled(scrollBar,
                            scope.Track(args.IsDeferredScrollingEnabled)));
            }

            if (args.ViewportSize is not null)
            {
                scrollBar.ViewportSize = args.ViewportSize.Value;
                if (args.ViewportSize.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.ViewportSize = scope.Track(args.ViewportSize));
            }

            if (args.Visibility is not null)
            {
                scrollBar.Visibility = args.Visibility.Value;
                if (args.Visibility.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.Visibility = scope.Track(args.Visibility));
            }

            if (args.ShowDelay is not null)
            {
                scrollBar.ShowDelay = args.ShowDelay.Value;
                if (args.ShowDelay.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.ShowDelay = scope.Track(args.ShowDelay));
            }

            if (args.HideDelay is not null)
            {
                scrollBar.HideDelay = args.HideDelay.Value;
                if (args.HideDelay.IsReactive)
                    uiScope.CreateEffect(scope =>
                        scrollBar.HideDelay = scope.Track(args.HideDelay));
            }

            if (args.OnScroll is not null)
                scrollBar.Scroll += (_, e) => args.OnScroll(e);

            if (args.Template is null)
            {
                scrollBar.Template = new FuncControlTemplate((_, _) => border);
            }
            else
            {
                var props = new HScrollBarProps(uiScope, scrollBar);
                var part = new HScrollBarPart();
                var content = args.Template(props, part).Content;
                border.Child = content;
                scrollBar.Template = new FuncControlTemplate((_, s) =>
                {
                    if (part.LineDownButtonElement is not null)
                    {
                        var __ = part.LineDownButtonElement.Content;
                        var lineDownButtonElement = part.LineDownButtonElement.Expose!;
                        lineDownButtonElement.Name = "PART_LineDownButton";
                        s.Register("PART_LineDownButton", lineDownButtonElement);
                    }

                    if (part.LineUpButtonElement is not null)
                    {
                        var __ = part.LineUpButtonElement.Content;
                        var lineUpButtonElement = part.LineUpButtonElement.Expose!;
                        lineUpButtonElement.Name = "PART_LineUpButton";
                        s.Register("PART_LineUpButton", lineUpButtonElement);
                    }

                    if (part.PageDownButtonElement is not null)
                    {
                        var __ = part.PageDownButtonElement.Content;
                        var pageDownButtonElement = part.PageDownButtonElement.Expose!;
                        pageDownButtonElement.Name = "PART_PageDownButton";
                        s.Register("PART_PageDownButton", pageDownButtonElement);
                    }

                    if (part.PageUpButtonElement is not null)
                    {
                        var __ = part.PageUpButtonElement.Content;
                        var pageUpButtonElement = part.PageUpButtonElement.Expose!;
                        pageUpButtonElement.Name = "PART_PageUpButton";
                        s.Register("PART_PageUpButton", pageUpButtonElement);
                    }

                    return border;
                });
            }

            return (scrollBar, scrollBar);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) scrollBar.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}