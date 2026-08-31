using System.Text;
using NeHive.Model;
using NeHive.UI.Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Components;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;
using static NeHive.UI.Avalonia.Components.AttachComponent;

namespace NeHive.Sample.Avalonia;

public static partial class DemoComponent
{
    private static IElement CustomSliderDemo()
    {
        var volume = new MutSignal<double>(50);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Slider Continuous Value Adjustment Demo", strStyle: SectionTitleStyle),
            HSlider(_ => new(
                bindValue: volume,
                minimum: 0,
                maximum: 100,
                strStyle: "w-72 h-12 horizontal")
            {
                Template = part =>
                    part.Track(HTrack(new(
                        value: volume,
                        minimum: 0,
                        maximum: 100,
                        strStyle: "w-full h-full horizontal rounded-lg")
                    {
                        DecreaseButton =
                            part.DecreaseButton(
                                HButton(strStyle: "my-auto w-full h-1 bg-matcha-700 hover:bg-matcha-500 rounded")),
                        IncreaseButton =
                            part.IncreaseButton(
                                HButton(strStyle: "my-auto w-full h-1 bg-coffee-200 hover:bg-coffee-400 rounded")),
                        Thumb = HThumb(_ => new(strStyle: "my-auto")
                        {
                            HSvgImage("~/Assets/circle-star.svg",
                                strStyle: "w-4 h-4 fw-extralight fg-yellow-500 bg-yellow-200 rounded-full"
                            ) // HSvgImage
                        }) // HTrack.Thumb
                    })) // part.Track
                // HSlider.Template
            }), // HSlider
            HText(new(() => $"Audio Volume Level: {volume.RxValue:F0}"),
                strStyle: "mt-2 text-xl fw-semibold fg-matcha-600")
        }); // HStackPanel
    }

    private static IElement CustomScrollDemo()
    {
        var sb = new StringBuilder();
        for (var i = 1; i <= 40; i++)
        {
            sb.AppendLine(
                $"Line {i}: Long vertical scrollable text content sample for NeHive UI framework demonstration.");
        }

        var longText = sb.ToString();

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Custom Scroll Demo", strStyle: SectionTitleStyle),
            HScrollViewer(props => new(
                verticalScrollBarVisibility: ScrollBarVisibility.Auto,
                strStyle: "w-full h-60 p-3 vertical bg-white rounded-xl border border-matcha-200 overflow-hidden")
            {
                Template = part => HGrid(_ => new(columnDefinitions: new([HgLen.Star(), HgLen.Auto]))
                {
                    [row: 0, column: 0] = part.ContentPresenter(HScrollContentPresenter(presenterProps =>
                        new(isScrollInertiaEnabled: props.IsScrollInertiaEnabled,
                            horizontalSnapPointsType: props.HorizontalSnapPointsType,
                            verticalSnapPointsType: props.VerticalSnapPointsType,
                            horizontalSnapPointsAlignment: props.HorizontalSnapPointsAlignment,
                            verticalSnapPointsAlignment: props.VerticalSnapPointsAlignment,
                            strStyle: "w-full h-60 p-3 vertical bg-matcha-200/50",
                            gestureRecognizer: new(
                                canHorizontallyScroll: presenterProps.CanHorizontallyScroll,
                                canVerticallyScroll: presenterProps.CanVerticallyScroll,
                                isScrollInertiaEnabled: presenterProps.IsScrollInertiaEnabled,
                                offset: presenterProps.Offset,
                                viewport: presenterProps.Viewport,
                                extent: presenterProps.Extent
                            )))),
                    [row: 0, column: 1] = part.VerticalScrollBar(HScrollBar(scrollBarProps =>
                        new(strStyle: "w-2 h-full vertical rounded")
                        {
                            Template = scrollBarPart =>
                                HTrack(new(
                                    bindValue: scrollBarProps.Value,
                                    viewportSize: scrollBarProps.ViewportSize,
                                    minimum: scrollBarProps.Minimum,
                                    maximum: scrollBarProps.Maximum,
                                    isDeferThumbDrag: scrollBarProps.IsDeferredScrollingEnabled,
                                    isDirectionReversed: true,
                                    strStyle: "w-2 h-full vertical bg-coffee-200 rounded")
                                {
                                    DecreaseButton = scrollBarPart.PageUpButton(
                                        HButton(strStyle: "w-2 h-full bg-transparent hover:cursor-pointer")
                                    ), // HTrack.DecreaseButton
                                    Thumb = HThumb(strStyle: "mx-auto w-2 h-full bg-white/60 rounded"),
                                    IncreaseButton = scrollBarPart.PageDownButton(
                                        HButton(strStyle: "w-2 h-full bg-transparent hover:cursor-pointer")
                                    ) // HTrack.IncreaseButton
                                }) // HTrack
                            // HScrollBar.Template
                        })) // part.VerticalScrollBar
                }), // HScrollViewer.Template
                Content = HText(longText, strStyle: "text-base leading-relaxed fg-matcha-800")
            }), // HScrollViewer
        }); // HStackPanel
    }

    #region Group Unified Hover State Container Demo

    private static IElement GroupDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Group Unified Hover State Parent Container Demo", strStyle: SectionTitleStyle),
            HStackPanel(props => new(
                strStyle: "mx-auto my-auto gap-8 p-6 horizontal bg-gray-50 border border-gray-200 rounded-xl")
            {
                HSvgImage(
                    uri: "~/Assets/play.svg",
                    // path: "F1 M 301.14,-189.041L 311.57,-189.041L 306.355,-182.942L 301.14,-189.041 Z",
                    strStyle: "w-16 h-16",
                    style: new(
                        foreground: BlueColor(props.IsPointerOver)
                    )
                ), // HSvgImage
                HSvgImage("~/Assets/skip-back.svg",
                    strStyle: new(() =>
                        $"w-16 h-16 fg-orange-{ToValue(props.IsPointerOver.RxValue)}")),
                HSvgImage("~/Assets/skip-forward.svg",
                    strStyle: new(() =>
                        $"w-16 h-16 fg-yellow-{ToValue(props.IsPointerOver.RxValue)}")),
                HSvgImage("~/Assets/stretch-vertical.svg",
                    strStyle: new(() =>
                        $"w-16 h-16 fg-red-{ToValue(props.IsPointerOver.RxValue)}"))
            })
        }); // HStackPanel

        string ToValue(bool isHover) => isHover ? "400" : "200";

        Computed<IBrush?> BlueColor(Signal<bool> isHover) => new(() => isHover.RxValue
            ? new SolidColorBrush(UI.Avalonia.Styles.Colors.Blue400)
            : new SolidColorBrush(UI.Avalonia.Styles.Colors.Blue200));
        // ? new SolidColorBrush(Color.FromRgb(96, 165, 250))
        // : new SolidColorBrush(Color.FromRgb(191, 219, 254)));
    }

    #endregion

    #region Context Theme Injection Demo

    private static readonly ContextKey<Signal<string>> Theme = new();
    private static readonly ContextKey<Action> ToggleTheme = new();

    private static IElement ContextDemo()
    {
        var theme = new MutSignal<string>("light");
        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Context Scope Theme Dependency Injection Demo", strStyle: SectionTitleStyle),
            HContext(new(ctx => ctx
                .SetContext(Theme, theme)
                .SetContext(ToggleTheme, Toggle))
            {
                UseContextDemo1(),
                UseContextDemo2()
            }) // HContext
        }); // HStackPanel

        void Toggle() => theme.RxValue = theme.Value is "dark" ? "light" : "dark";
    }

    private static IElement UseContextDemo1()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null || toggleTheme is null) throw new ArgumentNullException();
            return HStackPanel(_ => new()
            {
                HButton(_ => new(strStyle: new(() =>
                        $"""
                         {(theme.RxValue is "dark" ? "fg-matcha-100 bg-matcha-900 border-matcha-700" : "fg-coffee-700 bg-coffee-50 border-matcha-200")}
                         rounded-lg
                         transition-colors duration-300 border-w-1 focus:ring-w-2 focus:ring-matcha-300
                         """),
                    onClick: _ => toggleTheme())
                {
                    HStackPanel(_ => new(strStyle: "px-4 py-2 horizontal")
                    {
                        Switch<string>(new(theme)
                        {
                            ["dark"] = () => HSvgImage("~/Assets/sun.svg",
                                strStyle: "w-4 h-4 fw-extralight fg-matcha-200"),
                            // Switch<string>.["dark"]
                            Default = () => HSvgImage("~/Assets/moon.svg",
                                strStyle: "w-4 h-4 fw-extralight fg-coffee-700")
                        }), // Switch<string>
                        HText(new(() => $"Switch To {(theme.RxValue is "dark" ? "Light" : "Dark")} Theme Mode"),
                            strStyle: new(() => $"ml-2 fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}"))
                    }) // HStackPanel
                }) // HButton
            }); // HStackPanel
        });
    }

    private static IElement UseContextDemo2()
    {
        return Element.WithScope(uiScope =>
        {
            var theme = uiScope.GetContext(Theme);
            var toggleTheme = uiScope.GetContext(ToggleTheme);
            if (theme is null || toggleTheme is null) throw new ArgumentNullException();

            return HStackPanel(_ => new(strStyle: new(() =>
                $"""
                 mt-2 mx-auto max-w-md overflow-hidden rounded-xl shadow-md border-w-1
                 {(theme.RxValue is "dark" ? "bg-matcha-900 border-matcha-700" : "bg-coffee-50 border-matcha-200")} 
                 transition-colors duration-300
                 """))
            {
                HStackPanel(_ => new(strStyle: "p-6 vertical gap-3")
                {
                    HText("Theme Injection Demonstration",
                        strStyle: new(() =>
                            $"""text-2xl fw-bold fg-{(theme.RxValue is "dark" ? "matcha-100" : "matcha-800")}""")),
                    HText(new(() => $"Current Topic Mode: {theme.RxValue}"),
                        strStyle: new(() => $"fg-{(theme.RxValue is "dark" ? "matcha-200" : "coffee-700")}")),
                    HText(
                        "The background, text and border color of this card component will automatically change according to the theme.",
                        strStyle: new(() => $"fg-{(theme.RxValue is "dark" ? "matcha-300" : "coffee-700")}")),
                }) // HStackPanel
            }); // HStackPanel
        });
    }

    #endregion

    extension(Scope scope)
    {
        DispatcherTimer CreateInterval(int interval, Action fn)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(interval)
            };

            timer.Tick += Handler;
            scope.OnCleanup += () =>
            {
                timer.Stop();
                timer.Tick -= Handler;
            };

            timer.Start();
            return timer;

            void Handler(object? sender, EventArgs e)
            {
                fn();
            }
        }
    }

    private static IElement ClockDemo()
    {
        return Element.WithScope(uiScope =>
        {
            var size = 400;
            var borderWidth = 8;
            var hourTransforms = Enumerable.Range(1, 12).Select(n =>
            {
                var a = Math.PI * n / 6;
                var x = Math.Sin(a) * size / 2 * 0.85;
                var y = -Math.Cos(a) * size / 2 * 0.85;

                var builder = TransformOperations.CreateBuilder(1);
                builder.AppendTranslate(x, y);
                return builder.Build();
            }).ToArray();

            var tickStyles = Enumerable.Range(0, 60).Select(n =>
            {
                var isFiveTime = n % 5 == 0;
                var width = isFiveTime ? 4 : 2;
                var height = isFiveTime ? 8 : 6;
                var angle = Math.PI * n / 30;

                var builder = TransformOperations.CreateBuilder(2);
                builder.AppendTranslate(0, size * 0.5 - height * 0.5);
                builder.AppendRotate(angle);
                ITransform transform = builder.Build();
                return (width, height, transform);
            }).ToArray();

            var currentTime = new MutSignal<DateTime>(DateTime.Now);
            uiScope.CreateInterval(500, () => currentTime.RxValue = DateTime.Now);

            return HPanel(_ => new(
                strStyle: $"bg-matcha-50 border-w-{borderWidth} border-matcha-200 rounded-full")
            {
                ForEach<(int, int, ITransform)>(new(tickStyles)
                {
                    ItemsPanel = HPanel(style: new(width: size, height: size)),
                    ItemTemplate = (tickStyle, _) =>
                        HBorder(strStyle: "mx-auto my-auto bg-coffee-700 origin-center",
                            style: new(
                                width: tickStyle.Item1,
                                height: tickStyle.Item2,
                                renderTransform: new(tickStyle.Item3)
                            )
                        ) // HBorder
                }), // ForEach<ITransform>
                ForEach<ITransform>(new(hourTransforms)
                {
                    ItemsPanel = HPanel(style: new(width: size, height: size)),
                    ItemTemplate = (transform, i) =>
                        HText($"{i.Value + 1}", strStyle: "mx-auto my-auto text-2xl fw-bold fg-coffee-700",
                            style: new(renderTransform: new(transform)))
                }), // ForEach<ITransform>
                HText(new(() => currentTime.RxValue.ToString("yyyy-MM-dd HH:mm:ss")),
                    strStyle: "mx-auto my-auto mt-60 text-base fg-coffee-500"),
                HBorder(strStyle: "mx-auto my-auto bg-coffee-700 origin-center",
                    style: new(width: 10, height: size / 3.5,
                        renderTransform: HandTransform(-size * 0.075,
                            () => 2 * Math.PI * (currentTime.RxValue.Hour * 3600 + currentTime.RxValue.Minute * 60 +
                                                 currentTime.RxValue.Second) / 43200))
                ), // HBorder
                HBorder(strStyle: "mx-auto my-auto bg-coffee-700 origin-center",
                    style: new(width: 5, height: size / 2.5,
                        renderTransform: HandTransform(-size * 0.15,
                            () => 2 * Math.PI * (currentTime.RxValue.Minute * 60 +
                                                 currentTime.RxValue.Second) / 3600))
                ), // HBorder
                HBorder(strStyle: "mx-auto my-auto bg-matcha-700 origin-center",
                    style: new(width: 2, height: size / 2,
                        renderTransform: HandTransform(-size * 0.2,
                            () => 2 * Math.PI * currentTime.RxValue.Second / 60))
                ), // HBorder
                HBorder(strStyle: "mx-auto my-auto w-2 h-2 bg-coffee-50 rounded-full")
            });

            Func<ITransform> HandTransform(double move, Func<double> getAngle)
            {
                return () =>
                {
                    var angle = getAngle();
                    var builder = TransformOperations.CreateBuilder(2);
                    builder.AppendTranslate(0, move);
                    builder.AppendRotate(angle);
                    return builder.Build();
                };
            }
        });
    }

}
