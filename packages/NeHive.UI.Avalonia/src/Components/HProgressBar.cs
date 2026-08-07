using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;
using Colors = NeHive.UI.Avalonia.Styles.Colors;

namespace NeHive.UI.Avalonia.Components;

public class HProgressBarProps(Scope scope, Border border, ProgressBar progressBar)
    : HRangeBaseProps(scope, border, progressBar)
{
    public Signal<double> Percentage
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, progressBar,
                ProgressBar.PercentageProperty, progressBar.Percentage);
            return field;
        }
    }

    public MutSignal<bool> IsIndeterminate
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(progressBar.IsIndeterminate);
            BridgeAvalonia.BindPropertySignal(scope, field, progressBar, ProgressBar.IsIndeterminateProperty);
            return field;
        }
    }

    public MutSignal<bool> IsShowProgressText
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(progressBar.ShowProgressText);
            BridgeAvalonia.BindPropertySignal(scope, field, progressBar, ProgressBar.ShowProgressTextProperty);
            return field;
        }
    }

    public MutSignal<string> ProgressTextFormat
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string>(progressBar.ProgressTextFormat);
            BridgeAvalonia.BindPropertySignal(scope, field, progressBar, ProgressBar.ProgressTextFormatProperty);
            return field;
        }
    }

    public HProgressBarTemplateSettingsProps TemplateSettings
    {
        get
        {
            if (field is not null) return field;
            field = new HProgressBarTemplateSettingsProps(scope, progressBar.TemplateSettings);
            return field;
        }
    }

    public class HProgressBarTemplateSettingsProps(
        Scope scope,
        ProgressBar.ProgressBarTemplateSettings templateSettings)
    {
        public MutSignal<double> ContainerWidth
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.ContainerWidth);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.ContainerWidthProperty);
                return field;
            }
        }

        public MutSignal<double> Container2Width
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.Container2Width);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.Container2WidthProperty);
                return field;
            }
        }

        public MutSignal<double> ContainerAnimationStartPosition
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.ContainerAnimationStartPosition);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.ContainerAnimationStartPositionProperty);
                return field;
            }
        }

        public MutSignal<double> ContainerAnimationEndPosition
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.ContainerAnimationEndPosition);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.ContainerAnimationEndPositionProperty);
                return field;
            }
        }

        public MutSignal<double> Container2AnimationStartPosition
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.Container2AnimationStartPosition);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.Container2AnimationStartPositionProperty);
                return field;
            }
        }

        public MutSignal<double> Container2AnimationEndPosition
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.Container2AnimationEndPosition);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.Container2AnimationEndPositionProperty);
                return field;
            }
        }

        public MutSignal<double> IndeterminateStartingOffset
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.IndeterminateStartingOffset);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.IndeterminateStartingOffsetProperty);
                return field;
            }
        }

        public MutSignal<double> IndeterminateEndingOffset
        {
            get
            {
                if (field is not null) return field;
                field = new MutSignal<double>(templateSettings.IndeterminateEndingOffset);
                BridgeAvalonia.BindPropertySignal(scope, field, templateSettings,
                    ProgressBar.ProgressBarTemplateSettings.IndeterminateEndingOffsetProperty);
                return field;
            }
        }
    }
}

public class HProgressBarPart
{
    internal IElement<Border>? IndicatorElement { get; private set; }
    public IElement<Border> Indicator(IElement<Border> element) => IndicatorElement = element;
}

public class HProgressBarArgs(
    Accessor<double>? value = null,
    MutSignal<double>? bindValue = null,
    Accessor<double>? minimum = null,
    Accessor<double>? maximum = null,
    Accessor<double>? smallChange = null,
    Accessor<double>? largeChange = null,
    Accessor<bool>? isIndeterminate = null,
    Accessor<bool>? isShowProgressText = null,
    Accessor<string>? progressTextFormat = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null,
    Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
) : HRangeBaseArgs(value, bindValue, minimum, maximum, smallChange, largeChange, strStyle, style, baseInteraction,
    onValueChanged)
{
    public readonly Accessor<bool>? IsIndeterminate = isIndeterminate;
    public readonly Accessor<bool>? IsShowProgressText = isShowProgressText;
    public readonly Accessor<string>? ProgressTextFormat = progressTextFormat;
    public Func<HProgressBarPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<ProgressBar> HProgressBar(
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Accessor<bool>? isIndeterminate = null,
        Accessor<bool>? isShowProgressText = null,
        Accessor<string>? progressTextFormat = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HProgressBar(out _, _ => new(value, bindValue, minimum, maximum, smallChange, largeChange,
        isIndeterminate, isShowProgressText, progressTextFormat, strStyle, style, baseInteraction, onValueChanged));

    public static IElement<ProgressBar> HProgressBar(
        out ProgressBar expose,
        Accessor<double>? value = null,
        MutSignal<double>? bindValue = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<double>? smallChange = null,
        Accessor<double>? largeChange = null,
        Accessor<bool>? isIndeterminate = null,
        Accessor<bool>? isShowProgressText = null,
        Accessor<string>? progressTextFormat = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null
    ) => HProgressBar(out expose, _ => new(value, bindValue, minimum, maximum, smallChange, largeChange,
        isIndeterminate, isShowProgressText, progressTextFormat, strStyle, style, baseInteraction, onValueChanged));

    public static IElement<ProgressBar> HProgressBar(Func<HProgressBarProps, HProgressBarArgs> fn) =>
        HProgressBar(out _, fn);

    public static IElement<ProgressBar> HProgressBar(out ProgressBar expose,
        Func<HProgressBarProps, HProgressBarArgs> fn)
    {
        var progressBar = new ProgressBar();
        expose = progressBar;
        return Element<ProgressBar>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HProgressBarProps(uiScope, border, progressBar);
            var args = fn(props);
            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, progressBar, border, ApplyStyle);
            state.ApplyVariantsStyle(progressBar, border, ApplyStyle);

            if (args.Popups is not null)
                ElementUtil.ApplyPopups(progressBar, args.Popups);

            args.BaseInteraction?.ApplyInteractions(uiScope, progressBar);

            RangeBaseUtil.BindAccessor(uiScope, progressBar, args);

            uiScope.CreateEffect(epoch =>
            {
                var isVertical = epoch.Pull(props.Style.Orientation) is Orientation.Vertical;
                border.MinWidth = isVertical ? 16 : 200;
                border.MinHeight = isVertical ? 200 : 16;
            });

            if (args.IsIndeterminate is not null)
            {
                progressBar.IsIndeterminate = args.IsIndeterminate.Value;
                if (args.IsIndeterminate.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        progressBar.IsIndeterminate = epoch.Track(args.IsIndeterminate));
            }

            if (args.IsShowProgressText is not null)
            {
                progressBar.ShowProgressText = args.IsShowProgressText.Value;
                if (args.IsShowProgressText.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        progressBar.IsIndeterminate = epochScope.Track(args.IsShowProgressText));
            }

            if (args.ProgressTextFormat is not null)
            {
                progressBar.ProgressTextFormat = args.ProgressTextFormat.Value;
                if (args.ProgressTextFormat.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        progressBar.ProgressTextFormat = epochScope.Track(args.ProgressTextFormat));
            }

            var template = args.Template;
            if (template is null)
            {
                var layoutTransform = uiScope.CreateComputed<ITransform?>(() =>
                {
                    var isVertical = props.Style.Orientation.RxValue is Orientation.Vertical;
                    var builder = TransformOperations.CreateBuilder(1);
                    if (isVertical)
                        builder.AppendRotate(0.5 * Math.PI);
                    return builder.Build();
                });

                var loadingAnimation = uiScope.CreateComputed<IAnimation>(() =>
                {
                    var linearEasing = new LinearEasing();
                    var indeterminateAnim = new Animation
                    {
                        Easing = linearEasing,
                        IterationCount = IterationCount.Infinite,
                        // PlaybackDirection = PlaybackDirection.Alternate,
                        Duration = TimeSpan.FromSeconds(3), // 0:0:3
                    };

                    var keyFrame0 = new KeyFrame
                    {
                        Cue = new Cue(0.0)
                    };

                    var binding0 = new Binding
                    {
                        Source = progressBar.TemplateSettings,
                        Path = nameof(ProgressBar.ProgressBarTemplateSettings.IndeterminateStartingOffset)
                    };
                    Setter setter0;
                    if (props.Style.Orientation.RxValue is Orientation.Vertical)
                        setter0 = new Setter(TranslateTransform.YProperty, binding0);
                    else
                        setter0 = new Setter(TranslateTransform.XProperty, binding0);

                    keyFrame0.Setters.Add(setter0);
                    indeterminateAnim.Children.Add(keyFrame0);

                    var keyFrame1 = new KeyFrame
                    {
                        Cue = new Cue(1.0)
                    };

                    var binding1 = new Binding
                    {
                        Source = progressBar.TemplateSettings,
                        Path = nameof(ProgressBar.ProgressBarTemplateSettings.IndeterminateEndingOffset)
                    };
                    Setter setter1;
                    if (props.Style.Orientation.RxValue is Orientation.Vertical)
                        setter1 = new Setter(TranslateTransform.YProperty, binding1);
                    else
                        setter1 = new Setter(TranslateTransform.XProperty, binding1);

                    keyFrame1.Setters.Add(setter1);
                    indeterminateAnim.Children.Add(keyFrame1);

                    return indeterminateAnim;
                });

                template = part => HPanel(panelProps => new(style: new(
                    horizontalAlignment: HorizontalAlignment.Stretch,
                    verticalAlignment: VerticalAlignment.Stretch
                ))
                {
                    HPanel(_ => new(style: new(
                        width: new(() => panelProps.Bounds.RxValue.Width),
                        height: new(() => panelProps.Bounds.RxValue.Height)
                    ))
                    {
                        part.Indicator(HBorder(style: new(
                            width: new(() =>
                                (props.Style.Orientation.RxValue is Orientation.Vertical
                                    ? 1
                                    : props.Percentage.RxValue / 100)
                                * panelProps.Bounds.RxValue.Width),
                            height: new(() =>
                                (props.Style.Orientation.RxValue is Orientation.Vertical
                                    ? props.Percentage.RxValue / 100
                                    : 1)
                                * panelProps.Bounds.RxValue.Height),
                            horizontalAlignment: new(() =>
                                props.Style.Orientation.RxValue is Orientation.Vertical
                                    ? HorizontalAlignment.Stretch
                                    : HorizontalAlignment.Left),
                            verticalAlignment: new(() =>
                                props.Style.Orientation.RxValue is Orientation.Vertical
                                    ? VerticalAlignment.Bottom
                                    : VerticalAlignment.Stretch),
                            background: props.Style.Foreground,
                            cornerRadius: props.Style.CornerRadius,
                            isVisible: new(() => !props.IsIndeterminate.RxValue)
                        ))), // part.Indicator
                        HBorder(style: new(
                            width: new(() => props.Style.Orientation.RxValue is Orientation.Vertical
                                ? panelProps.Bounds.RxValue.Width
                                : props.TemplateSettings.ContainerWidth.RxValue),
                            height: new(() => props.Style.Orientation.RxValue is Orientation.Vertical
                                ? props.TemplateSettings.ContainerWidth.RxValue
                                : panelProps.Bounds.RxValue.Height),
                            horizontalAlignment:HorizontalAlignment.Center,
                            verticalAlignment:VerticalAlignment.Center,
                            background: props.Style.Foreground,
                            cornerRadius: props.Style.CornerRadius,
                            isVisible: props.IsIndeterminate,
                            animation: loadingAnimation
                        )) // HBorder
                    }), // HPanel
                    HLayoutTransform(_ => new(
                        layoutTransform: layoutTransform,
                        style: new(
                            horizontalAlignment: HorizontalAlignment.Center,
                            verticalAlignment: VerticalAlignment.Center,
                            isVisible: props.IsShowProgressText
                        )
                    )
                    {
                        HText(text: new(() => string.Format(
                                props.ProgressTextFormat.RxValue,
                                props.Value.RxValue,
                                props.Percentage.RxValue,
                                props.Minimum.RxValue,
                                props.Maximum.RxValue)),
                            style: new(foreground: new(new SolidColorBrush(Colors.Gray100)))
                        ) // HText
                    }) // HLayoutTransform
                }); // HPanel
            }

            var part = new HProgressBarPart();
            var content = template(part).Content;
            progressBar.Template = new FuncControlTemplate((_, s) =>
            {
                using (new ScopeFrame(uiScope))
                {
                    var indicator = part.IndicatorElement;
                    if (indicator is not null)
                    {
                        var __ = indicator.Content;
                        indicator.Expose!.Name = "PART_Indicator";
                        s.Register("PART_Indicator", indicator.Expose);
                    }

                    border.Child = content;
                }

                return border;
            });

            return (progressBar, progressBar);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) progressBar.Orientation = styleValue.Orientation.Value;
                if (styleValue.Foreground is not null) progressBar.Foreground = styleValue.Foreground;
            }
        });
    }
}