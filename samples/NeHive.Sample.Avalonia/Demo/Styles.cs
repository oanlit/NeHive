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
    #region Sizing Demo (Width, Height, Min/Max)

    private static IElement SizingDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Sizing Utilities: Width & Height",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Width (w-) — Fixed & Percentage",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4 items-end")
                {
                    HStackPanel(_ => new(strStyle: "vertical gap-2 items-center")
                    {
                        HText("w-16", strStyle: "w-16 h-8 bg-matcha-200 rounded text-center fw-medium text-sm"),
                        HText("w-32", strStyle: "w-32 h-8 bg-matcha-200 rounded text-center fw-medium text-sm"),
                        HText("w-64", strStyle: "w-64 h-8 bg-matcha-200 rounded text-center fw-medium text-sm")
                    }), // HStackPanel
                    HStackPanel(_ => new(strStyle: "vertical gap-2 items-center")
                    {
                        HText("w-full", strStyle: "w-full h-8 bg-sky-200 rounded text-center fw-medium text-sm"),
                        // HText("w-1/2", strStyle: "w-[50%] h-8 bg-sky-200 rounded text-center fw-medium text-sm")
                    }) // HStackPanel
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Height (h-)",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4 items-start")
                {
                    HText("h-12", strStyle: "h-12 w-24 bg-matcha-300 rounded text-center fw-medium"),
                    HText("h-24", strStyle: "h-24 w-24 bg-matcha-300 rounded text-center fw-medium"),
                    HText("h-48", strStyle: "h-48 w-24 bg-matcha-300 rounded text-center fw-medium")
                })
            ]),

            DemoSection("Min/Max Constraints",
            [
                HText("min-w-48 & max-w-64",
                    strStyle: "min-w-48 max-w-64 h-8 bg-amber-100 rounded text-center px-2 fw-medium"),
                HText("min-h-16 & max-h-32",
                    strStyle: "min-h-16 max-h-32 w-32 bg-amber-100 rounded text-center p-2 fw-medium")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Padding Demo

    private static IElement PaddingDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Padding Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Uniform Padding (p-)",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("p-0", strStyle: "p-0 bg-matcha-100 rounded border"),
                    HText("p-2", strStyle: "p-2 bg-matcha-100 rounded border"),
                    HText("p-4", strStyle: "p-4 bg-matcha-100 rounded border"),
                    HText("p-8", strStyle: "p-8 bg-matcha-100 rounded border")
                })
            ]), // DemoSection

            DemoSection("Axis Padding (px-, py-)",
            [
                HText("px-4 py-2", strStyle: "px-4 py-2 bg-sky-100 rounded border"),
                HText("px-8 py-4", strStyle: "px-8 py-4 bg-sky-100 rounded border")
            ]), // DemoSection

            DemoSection("Directional Padding (pt-, pr-, pb-, pl-)",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("pl-4", strStyle: "p-2 pl-4 bg-rose-100 rounded border"),
                    HText("pr-4", strStyle: "p-2 pr-4 bg-rose-100 rounded border"),
                    HText("pt-4", strStyle: "p-2 pt-4 bg-rose-100 rounded border"),
                    HText("pb-4", strStyle: "p-2 pb-4 bg-rose-100 rounded border")
                }) // HStackPanel
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Spacing Demo (Margin & Gap)

    private static IElement SpacingDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Spacing Utilities: Margin & Gap",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            DemoSection("Margin (m-, mx-, my-, mt-, mb-, ml-, mr-)",
            [
                HText("Positive margins (multiples of 4px)",
                    strStyle: "text-sm fw-semibold fg-coffee-600 mb-2"),
                HWrapPanel(_ => new(strStyle: "gap-3")
                {
                    // 4 items with different margin-left
                    HText("ml-0", strStyle: "ml-0 p-2 bg-matcha-100 rounded border fw-medium"),
                    HText("ml-2", strStyle: "ml-2 p-2 bg-matcha-100 rounded border fw-medium"),
                    HText("ml-4", strStyle: "ml-4 p-2 bg-matcha-100 rounded border fw-medium"),
                    HText("ml-8", strStyle: "ml-8 p-2 bg-matcha-100 rounded border fw-medium")
                }), // HWrapPanel
                HStackPanel(_ => new(strStyle: "mt-4 gap-2")
                {
                    HText("mt-2", strStyle: "mt-2 p-2 bg-sky-100 rounded border fw-medium"),
                    HText("mt-4", strStyle: "mt-4 p-2 bg-sky-100 rounded border fw-medium")
                }), // HStackPanel
                HText("Negative margin example (pull element up):",
                    strStyle: "text-sm fg-coffee-600 mt-4 mb-2"),
                HStackPanel(_ => new(strStyle: "horizontal gap-0")
                {
                    HText("mt-4", strStyle: "mt-4 p-2 bg-rose-100 rounded border fw-medium"),
                    HText("-mt-2", strStyle: "mt--2 p-2 bg-rose-200 rounded border fw-medium")
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Gap (row/column spacing)",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("Item 1", strStyle: "p-2 bg-matcha-100 rounded"),
                    HText("Item 2", strStyle: "p-2 bg-matcha-100 rounded"),
                    HText("Item 3", strStyle: "p-2 bg-matcha-100 rounded")
                }), // HStackPanel
                HText("Custom gap-x-8, gap-y-2", strStyle: "text-sm fg-coffee-600 mt-2"),
                HStackPanel(_ => new(strStyle: "horizontal gap-x-8 gap-y-2 wrap")
                {
                    HText("A", strStyle: "p-2 bg-sky-100 rounded"),
                    HText("B", strStyle: "p-2 bg-sky-100 rounded"),
                    HText("C", strStyle: "p-2 bg-sky-100 rounded")
                }) // HStackPanel
            ]) // DemoSection
        });
    }

    #endregion

    #region Layout Demo

    private static IElement LayoutDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Layout Utilities: Flex, Alignment & Direction",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Direction (horizontal / vertical / flex-row / flex-col)",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("horizontal →", strStyle: "p-2 bg-matcha-100 rounded"),
                    HText("items", strStyle: "p-2 bg-matcha-100 rounded"),
                    HText("in a row", strStyle: "p-2 bg-matcha-100 rounded")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "vertical gap-2 mt-4")
                {
                    HText("vertical ↓", strStyle: "p-2 bg-sky-100 rounded"),
                    HText("stacked", strStyle: "p-2 bg-sky-100 rounded"),
                    HText("column", strStyle: "p-2 bg-sky-100 rounded")
                }) // HStackPanel
            ]), // DemoSection

            DemoSection("Shorthand: start / center / end / stretch",
            [
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("start", strStyle: "start w-16 h-16 bg-matcha-200 rounded text-center"),
                    HText("center", strStyle: "center w-16 h-16 bg-matcha-200 rounded text-center"),
                    HText("end", strStyle: "end w-16 h-16 bg-matcha-200 rounded text-center"),
                    HText("stretch", strStyle: "stretch w-16 h-16 bg-matcha-200 rounded text-center")
                }) // HStackPanel
            ]), // DemoSection
            DemoSection("Auto Margins for Centering",
            [
                HText("mx-auto · my-auto (horizontal & vertical centering)",
                    strStyle: "mx-auto my-auto w-48 p-2 bg-matcha-50 rounded border text-center")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region #region Complete Text StrStyle Typography Demo

    private static IElement TextStyleDemo()
    {
        const string longText =
            "This line demonstrates text truncation behaviors when content exceeds the container width, triggering ellipsis or line clamping.";
        const string multiLine =
            "Line one of the clamped block.\nLine two adds more detail.\nLine three is the last visible.\nLine four is completely hidden.";
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Typography Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            // Font Size
            DemoSection("Font Size", new[]
            {
                HText("text-xs (12px)", strStyle: "text-xs"),
                HText("text-sm (14px)", strStyle: "text-sm"),
                HText("text-base (16px)", strStyle: "text-base"),
                HText("text-lg (18px)", strStyle: "text-lg"),
                HText("text-xl (20px)", strStyle: "text-xl"),
                HText("text-2xl (24px)", strStyle: "text-2xl"),
                HText("text-3xl (30px)", strStyle: "text-3xl"),
                HText("text-12 (48px)", strStyle: "text-12 fw-bold fg-matcha-700")
            }), // DemoSection
            // Font Weight
            DemoSection("Font Weight", new[]
            {
                HText("fw-thin", strStyle: "fw-thin text-lg"),
                HText("fw-extralight", strStyle: "fw-extralight text-lg"),
                HText("fw-light", strStyle: "fw-light text-lg"),
                HText("fw-normal", strStyle: "fw-normal text-lg"),
                HText("fw-medium", strStyle: "fw-medium text-lg"),
                HText("fw-semibold", strStyle: "fw-semibold text-lg"),
                HText("fw-bold", strStyle: "fw-bold text-lg"),
                HText("fw-extrabold", strStyle: "fw-extrabold text-lg"),
                HText("fw-black", strStyle: "fw-black text-lg")
            }), // DemoSection
            // Font Family
            DemoSection("Font Family", new[]
            {
                HText("font-jetmono (JetBrains Mono)", strStyle: "font-jetmono text-lg"),
                HText("font-lxgw (LXGW WenKai)", strStyle: "font-lxgw text-lg")
            }), // DemoSection
            // Font StrStyle (Italic/Oblique)
            DemoSection("Font StrStyle", new[]
            {
                HText("italic", strStyle: "italic text-lg"),
                HText("oblique", strStyle: "oblique text-lg"),
                HText("not-italic", strStyle: "not-italic text-lg")
            }), // DemoSection
            // Letter Spacing
            DemoSection("Letter Spacing (tracking-)", new[]
            {
                HText("tracking-tighter", strStyle: "tracking-tighter text-lg"),
                HText("tracking-tight", strStyle: "tracking-tight text-lg"),
                HText("tracking-normal", strStyle: "tracking-normal text-lg"),
                HText("tracking-wide", strStyle: "tracking-wide text-lg"),
                HText("tracking-wider", strStyle: "tracking-wider text-lg"),
                HText("tracking-widest", strStyle: "tracking-widest text-lg"),
                HText("tracking-4 (4px)", strStyle: "tracking-4 text-lg")
            }), // DemoSection
            // Line Height
            DemoSection("Line Height (leading-)", new[]
            {
                HText("leading-none (1.0)", strStyle: "leading-none p-2 bg-white rounded border"),
                HText("leading-tight (1.25)", strStyle: "leading-tight p-2 bg-white rounded border"),
                HText("leading-snug (1.375)", strStyle: "leading-snug p-2 bg-white rounded border"),
                HText("leading-normal (1.5)", strStyle: "leading-normal p-2 bg-white rounded border"),
                HText("leading-relaxed (1.625)",
                    strStyle: "leading-relaxed w-full p-2 bg-white rounded border"),
                HText("leading-loose (2.0)", strStyle: "leading-loose w-full p-2 bg-white rounded border"),
                HText("leading-8 (32px)", strStyle: "leading-8 text-lg w-full p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Text Alignment (Horizontal)
            DemoSection("Text Horizontal Alignment", new[]
            {
                HText("text-left", strStyle: "text-left w-32 h-20 p-2 bg-matcha-100 rounded border"),
                HText("text-x-center", strStyle: "text-x-center h-20 w-32 p-2 bg-matcha-100 rounded border"),
                HText("text-right", strStyle: "text-right h-20 w-32 p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Vertical Text Alignment
            DemoSection("Text Vertical Alignment", new[]
            {
                HStackPanel(_ => new(strStyle: "horizontal gap-4 items-start")
                {
                    HText("text-top", strStyle: "text-top w-32 h-20 p-2 bg-matcha-100 rounded border"),
                    HText("text-y-center", strStyle: "text-y-center w-32 h-20 p-2 bg-matcha-100 rounded border"),
                    HText("text-bottom", strStyle: "text-bottom w-32 h-20 p-2 bg-matcha-100 rounded border")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Text Center Alignment", new[]
            {
                HText("text-center", strStyle: "text-center w-32 h-20 p-2 bg-matcha-100 rounded border")
            }), // DemoSection
            // Wrapping
            DemoSection("Text Wrapping", new[]
            {
                HText("wrap: " + longText, strStyle: "wrap w-80 p-2 bg-white rounded border"),
                HText("wrap-overflow: " + longText,
                    strStyle: "wrap-overflow w-80 p-2 bg-white rounded border"),
                HText("whitespace-nowrap + truncate: " + longText,
                    strStyle: "whitespace-nowrap truncate w-80 p-2 bg-white rounded border")
            }), // DemoSection
            // Truncation & Clipping
            DemoSection("Text Trimming", new[]
            {
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-none — No trimming, overflows horizontally",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-none whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-end / truncate — Ellipsis at the end",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-end whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-char — Character‑by‑character ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-char whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-start — Ellipsis at the start (path‑style)",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-start whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-prefix — Prefix ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-prefix whitespace-nowrap p-2 bg-gray-100 rounded border")
                }), // HStackPanel
                HStackPanel(_ => new(strStyle: "gap-1 vertical")
                {
                    HText("text-clip-path — Path‑segment ellipsis",
                        strStyle: "text-xs fw-semibold fg-coffee-500"),
                    HText(longText,
                        strStyle: "w-80 text-clip-path whitespace-nowrap p-2 bg-gray-100 rounded border")
                }) // HStackPanel
            }), // DemoSection
            // Line Clamp
            DemoSection("Line Clamp (line-clamp-)", new[]
            {
                HText("line-clamp-2: " + multiLine,
                    strStyle: "line-clamp-2 w-full p-2 bg-matcha-50 rounded border"),
                HText("line-clamp-3: " + multiLine,
                    strStyle: "line-clamp-3 w-full p-2 bg-matcha-50 rounded border")
            }), // DemoSection
            // Text Decoration
            DemoSection("Text Decoration", new[]
            {
                HText("underline", strStyle: "underline text-lg"),
                HText("overline", strStyle: "overline text-lg"),
                HText("baseline", strStyle: "baseline text-lg"),
                HText("line-through", strStyle: "line-through text-lg"),
                HText("decoration-none", strStyle: "decoration-none text-lg"),
                HText("dashed underline",
                    strStyle: "underline decoration-dashed decoration-matcha-500 text-lg"),
                HText("dotted underline",
                    strStyle: "underline decoration-dotted decoration-matcha-500 text-lg"),
                HText("decoration-w-4", strStyle: "underline decoration-w-4 decoration-matcha-500 text-lg"),
                HText("decoration-rose-500", strStyle: "underline decoration-rose-500 text-lg")
            }), // DemoSection

            DemoSection("Combined Composite Typography",
            [
                HText("NeHive UI Combined Typography Sample",
                    strStyle: "text-3xl tracking-2 fw-black italic fg-blue-300"),
                HText("JetBrains Mono Programming Code Typography",
                    strStyle: "text-lg tracking-1 fw-semibold font-jetmono fg-emerald-600"),
                HText("LXGW Custom Chinese Font Combined StrStyle Demo Text",
                    strStyle: "text-xl tracking-1 leading-10 fw-medium font-lxgw fg-rose-600")
            ]) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Color & Gradient Demo

    private static IElement ColorDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Color & Gradient Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            // ── Foreground Colors ──
            DemoSection("Foreground Colors (fg-)", new[]
            {
                HText("fg-matcha-600", strStyle: "fg-matcha-600 text-lg fw-medium"),
                HText("fg-sky-500", strStyle: "fg-sky-500 text-lg fw-medium"),
                HText("fg-rose-500", strStyle: "fg-rose-500 text-lg fw-medium"),
                HText("fg-emerald-500", strStyle: "fg-emerald-500 text-lg fw-medium"),
                HText("fg-[#8a2be2] (arbitrary hex)", strStyle: "fg-[#8a2be2] text-lg fw-medium")
            }), // DemoSection
            // ── Foreground Opacity Modifier ──
            DemoSection("Foreground Opacity (fg-<color>/<opacity>)", new[]
            {
                HText("fg-sky-500/100 — fully opaque",
                    strStyle: "fg-sky-500/100 text-lg fw-medium"),
                HText("fg-sky-500/75 — 75% opacity",
                    strStyle: "fg-sky-500/75 text-lg fw-medium"),
                HText("fg-sky-500/50 — 50% opacity",
                    strStyle: "fg-sky-500/50 text-lg fw-medium"),
                HText("fg-sky-500/25 — 25% opacity",
                    strStyle: "fg-sky-500/25 text-lg fw-medium"),
                HText("fg-rose-500/40 — rose at 40%",
                    strStyle: "fg-rose-500/40 text-lg fw-medium")
            }), // DemoSection
            // ── Background Colors ──
            DemoSection("Background Colors (bg-)", new[]
            {
                HStackPanel(_ => new(strStyle: "horizontal gap-3 flex-wrap")
                {
                    HText("bg-matcha-500",
                        strStyle: "bg-matcha-500 fg-white p-2 rounded text-lg"),
                    HText("bg-sky-500",
                        strStyle: "bg-sky-500 fg-white p-2 rounded text-lg"),
                    HText("bg-rose-500",
                        strStyle: "bg-rose-500 fg-white p-2 rounded text-lg")
                }) // HStackPanel
            }), // DemoSection
            // ── Background Opacity Modifier ──
            DemoSection("Background Opacity (bg-<color>/<opacity>)", new IElement[]
            {
                HWrapPanel(_ => new(strStyle: "w-full horizontal gap-3")
                {
                    HText("bg-sky-300/100",
                        strStyle: "bg-sky-300/100 p-2 rounded text-lg fw-medium"),
                    HText("bg-sky-300/75",
                        strStyle: "bg-sky-300/75 p-2 rounded text-lg fw-medium"),
                    HText("bg-sky-300/50",
                        strStyle: "bg-sky-300/50 p-2 rounded text-lg fw-medium"),
                    HText("bg-sky-300/25",
                        strStyle: "bg-sky-300/25 p-2 rounded text-lg fw-medium"),
                    HText("bg-sky-300/10",
                        strStyle: "bg-sky-300/10 p-2 rounded text-lg fw-medium")
                }), // HStackPanel
                HText("Tip: /N sets the alpha channel as N% (0–100). Works with all named colors.",
                    strStyle: "text-xs fg-coffee-500 mt-2 italic")
            }), // DemoSection
            // ── Foreground Gradients ──
            DemoSection("Foreground Gradients (fg-gradient-*)", new[]
            {
                HText("fg-gradient-r from-matcha-400 to-matcha-800",
                    strStyle: "fg-gradient-r fg-from-matcha-400 fg-to-matcha-800 text-sm fw-black"),
                HText("fg-gradient-t from-sky-400 to-sky-800",
                    strStyle: "fg-gradient-t fg-from-sky-400 fg-to-sky-800 text-sm fw-black"),
                HText("fg-gradient-tl from-rose-400 to-rose-800",
                    strStyle: "fg-gradient-tl fg-from-rose-400 fg-to-rose-800 text-sm fw-black"),
                HText("fg-gradient-tr from-amber-400 to-amber-700/60",
                    strStyle: "fg-gradient-tr fg-from-amber-400 fg-to-amber-700/60 text-sm fw-black")
            }), // DemoSection
            // ── Background Gradients ──
            DemoSection("Background Gradients (bg-gradient-*)", new[]
            {
                HText("bg-gradient-r from-amber-300 to-amber-600",
                    strStyle:
                    "bg-gradient-r bg-from-amber-300 bg-to-amber-600 fg-white text-sm fw-black p-4 rounded"),
                HText("bg-gradient-tr from-purple-300 to-purple-600",
                    strStyle:
                    "bg-gradient-tr bg-from-purple-300 bg-to-purple-600 fg-white text-sm fw-black p-4 rounded"),
                HText("bg-gradient-t from-sky-200/80 to-sky-600/40",
                    strStyle: "bg-gradient-t bg-from-sky-200/80 bg-to-sky-600/40 text-sm fw-black p-4 rounded")
            }), // DemoSection
            // ── Border Colors ──
            DemoSection("Border Colors (border-<color>)", new[]
            {
                HText("border-matcha-500",
                    strStyle: "border-w-2 border-matcha-500 p-3 rounded text-xs"),
                HText("border-sky-400/60",
                    strStyle: "border-w-2 border-sky-400/60 p-3 rounded text-xs"),
                HText("border-rose-500/30",
                    strStyle: "border-w-4 border-rose-500/30 p-3 rounded text-xs")
            }), // DemoSection
            // ── Border Gradients ──
            DemoSection("Border Gradients (border-gradient-*)", new[]
            {
                HText("border-gradient-r from-matcha-400 to-matcha-800",
                    strStyle:
                    "border-gradient-r border-from-matcha-400 border-to-matcha-800 border-w-4 p-4 rounded text-xs fw-bold")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Border Demo

    private static IElement BorderDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Border Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Border Width (border, border-w-*)", new[]
            {
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("border", strStyle: "border p-2 rounded"),
                    HText("border-w-2", strStyle: "p-2 border-w-2 border-matcha-500 rounded"),
                    HText("border-w-4", strStyle: "p-2 border-w-4 border-matcha-500 rounded")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Directional Border Widths", new[]
            {
                HText("border-t-2 border-r-4 border-b-2 border-l-4",
                    strStyle: "p-4 border-t-2 border-r-4 border-b-2 border-l-4 border-matcha-500 rounded text-center")
            }), // DemoSection
            DemoSection("Corner Radius (rounded-*)", new[]
            {
                HWrapPanel(_ => new(strStyle: "w-full horizontal gap-4")
                {
                    HText("rounded-none", strStyle: "p-2 rounded-none bg-matcha-100 border"),
                    HText("rounded-sm", strStyle: "p-2 rounded-sm bg-matcha-100 border"),
                    HText("rounded", strStyle: "p-2 rounded bg-matcha-100 border"),
                    HText("rounded-md", strStyle: "p-2 rounded-md bg-matcha-100 border"),
                    HText("rounded-lg", strStyle: "p-2 rounded-lg bg-matcha-100 border"),
                    HText("rounded-xl", strStyle: "p-2 rounded-xl bg-matcha-100 border"),
                    HText("rounded-2xl", strStyle: "p-2 rounded-2xl bg-matcha-100 border"),
                    HText("rounded-full", strStyle: "w-24 h-24 p-2 rounded-full text-center bg-matcha-100 border")
                }) // HStackPanel
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Effects Demo

    private static IElement EffectsDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Visual Effects",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Opacity (opacity-)", new[]
            {
                HStackPanel(_ => new(strStyle: "horizontal gap-4")
                {
                    HText("opacity-100", strStyle: "opacity-100 text-xl fw-bold fg-matcha-800"),
                    HText("opacity-70", strStyle: "opacity-70 text-xl fw-bold fg-matcha-800"),
                    HText("opacity-40", strStyle: "opacity-40 text-xl fw-bold fg-matcha-800"),
                    HText("opacity-10", strStyle: "opacity-10 text-xl fw-bold fg-matcha-800")
                }) // HStackPanel
            }), // DemoSection
            DemoSection("Visibility", new[]
            {
                // Hard to show hidden, but we can show a container with visible/hidden effect via code.
                HText("visible / hidden (controlled by code)", strStyle: "text-sm fg-coffee-500")
            }), // DemoSection
            DemoSection("Blur", new[]
            {
                HText("blur-none", strStyle: "blur-none text-xl fw-bold fg-matcha-800"),
                HText("blur-sm", strStyle: "blur-sm text-xl fw-bold fg-matcha-800"),
                HText("blur", strStyle: "blur text-xl fw-bold fg-matcha-800"),
                HText("blur-lg", strStyle: "blur-lg text-xl fw-bold fg-matcha-800"),
                HText("blur-xl", strStyle: "blur-xl text-xl fw-bold fg-matcha-800")
            }), // DemoSection
            DemoSection("Box Shadow", new[]
            {
                HText("shadow-sm", strStyle: "shadow-sm text-xl fw-bold p-4 bg-white rounded"),
                HText("shadow", strStyle: "shadow text-xl fw-bold p-4 bg-white rounded"),
                HText("shadow-md", strStyle: "shadow-md text-xl fw-bold p-4 bg-white rounded"),
                HText("shadow-lg", strStyle: "shadow-lg text-xl fw-bold p-4 bg-white rounded"),
                HText("shadow-xl", strStyle: "shadow-xl text-xl fw-bold p-4 bg-white rounded")
            }), // DemoSection
            DemoSection("Ring (Focus Ring)", new[]
            {
                HText("ring-2 ring-matcha-500 ring-offset-2",
                    strStyle:
                    "ring-w-2 ring-matcha-500 ring-offset-2 p-4 rounded bg-white border border-matcha-200")
            }), // DemoSection
            DemoSection("Z-Index", new[]
            {
                HText("z-10 (layering)", strStyle: "z-10 p-4 bg-matcha-100 rounded border")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Cursor Demo

    private static IElement CursorDemo()
    {
        var cursors = new (string style, string label)[]
        {
            ("cursor-default", "Default"),
            ("cursor-text", "Text"),
            ("cursor-wait", "Wait"),
            ("cursor-crosshair", "Crosshair"),
            ("cursor-up-arrow", "UpArrow"),
            ("cursor-ew-resize", "EW Resize"),
            ("cursor-ns-resize", "NS Resize"),
            ("cursor-move", "Move"),
            ("cursor-not-allowed", "Not Allowed"),
            ("cursor-pointer", "Pointer"),
            ("cursor-progress", "Progress"),
            ("cursor-help", "Help"),
            ("cursor-n-resize", "N Resize"),
            ("cursor-s-resize", "S Resize"),
            ("cursor-w-resize", "W Resize"),
            ("cursor-e-resize", "E Resize"),
            ("cursor-nw-resize", "NW Resize"),
            ("cursor-ne-resize", "NE Resize"),
            ("cursor-sw-resize", "SW Resize"),
            ("cursor-se-resize", "SE Resize"),
            ("cursor-drag-move", "Drag Move"),
            ("cursor-drag-copy", "Drag Copy"),
            ("cursor-drag-link", "Drag Link"),
            ("cursor-none", "None")
        };
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Cursor Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),

            ForEach<(string style, string label)>(new(cursors)
            {
                ItemsPanel = HWrapPanel(strStyle: "gap-3"),
                ItemTemplate = (t, _) => HText($"  {t.label}  ",
                    strStyle: $"{t.style} p-2 bg-matcha-100 border rounded text-sm fw-medium")
            }) // ForEach<(string style, string label)>
        }); // HStackPanel
    }

    #endregion

    #region Transition Demo

    private static IElement TransitionDemo()
    {
        // Create interactive buttons to show transition effects
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Transition & Animation Utilities",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Transition Property Scope", new[]
            {
                // HButton("transition-none (no animation)",
                //     strStyle: PrimaryBtnBase + "transition-none"),
                // HButton("transition-all (all properties)",
                //     strStyle: PrimaryBtnBase + "transition-all hover:bg-matcha-600 hover:scale-105"),
                HButton("transition-colors",
                    strStyle: PrimaryBtnBase + "transition-colors hover:bg-matcha-600"),
                HButton("transition-transform",
                    strStyle: PrimaryBtnBase + "transition-transform hover:scale-110"),
                HButton("transition-opacity",
                    strStyle: PrimaryBtnBase + "transition-opacity hover:opacity-70"),
                HButton("transition-shadow",
                    strStyle: PrimaryBtnBase + "transition-shadow hover:shadow-lg"),
            }), // DemoSection
            DemoSection("Duration (duration-)", new[]
            {
                HButton("duration-100 (100ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-100 hover:scale-110"),
                HButton("duration-300 (300ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-300 hover:scale-110"),
                HButton("duration-700 (700ms)",
                    strStyle: PrimaryBtnBase + "transition-transform duration-700 hover:scale-110"),
            }), // DemoSection
            DemoSection("Easing Functions", new[]
            {
                HButton("linear",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 linear hover:translate-x-4"),
                HButton("ease",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease hover:translate-x-4"),
                HButton("ease-in",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-in hover:translate-x-4"),
                HButton("ease-out",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-out hover:translate-x-4"),
                HButton("ease-in-out",
                    strStyle: PrimaryBtnBase + "transition-transform duration-500 ease-in-out hover:translate-x-4"),
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

    #region Transform Demo

    private static IElement TransformDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + "w-full h-full p-6 gap-8 vertical")
        {
            HText("Geometric Transforms",
                strStyle: "text-3xl fw-bold font-jetmono fg-matcha-700 tracking-wide mb-2"),
            DemoSection("Translate", new[]
            {
                HText("translate-x-4", strStyle: "translate-x-4 p-4 bg-matcha-100 rounded"),
                HText("translate-y-4", strStyle: "translate-y-4 p-4 bg-matcha-100 rounded"),
                HText("translate-4", strStyle: "translate-4 p-4 bg-matcha-100 rounded")
            }), // DemoSection
            DemoSection("Scale", new[]
            {
                HText("scale-50", strStyle: "scale-50 p-4 bg-rose-100 rounded"),
                HText("scale-100", strStyle: "scale-100 p-4 bg-rose-100 rounded"),
                HText("scale-150", strStyle: "scale-150 p-4 bg-rose-100 rounded"),
                HText("scale-x-75", strStyle: "scale-x-75 p-4 bg-rose-100 rounded")
            }), // DemoSection
            DemoSection("Rotate", new[]
            {
                HText("rotate-45", strStyle: "rotate-45 p-4 bg-sky-100 rounded"),
                HText("-rotate-30", strStyle: "-rotate-30 p-4 bg-sky-100 rounded"),
                HText("rotate-180", strStyle: "rotate-180 p-4 bg-sky-100 rounded")
            }), // DemoSection
            DemoSection("Skew", new[]
            {
                HText("skew-10", strStyle: "skew-10 p-4 bg-amber-100 rounded"),
                HText("skew-x-15", strStyle: "skew-x-15 p-4 bg-amber-100 rounded"),
                HText("-skew-y-10", strStyle: "-skew-y-10 p-4 bg-amber-100 rounded")
            }), // DemoSection
            DemoSection("Combined Transforms", new[]
            {
                HText("Hover: scale-110 rotate-3 ease-out",
                    strStyle:
                    "transition-transform duration-300 ease-out hover:scale-110 hover:rotate-3 p-4 bg-matcha-50 rounded-xl cursor-pointer")
            }) // DemoSection
        }); // HStackPanel
    }

    #endregion

}
