using System.Collections.Frozen;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Animation;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Styles;

public static class StyleParser
{
    // 原子类字典：前缀 → 处理逻辑
    private static FrozenDictionary<string, Action<string[], bool, StyleSet>>? _finalAtomHandlers;

    // 样式缓存
    private static readonly Dictionary<string, StyleSet> StyleCache = new(StringComparer.Ordinal);

    public static void Init(Action<StyleDefinitions> modifier)
    {
        if (_finalAtomHandlers is not null)
            throw new InvalidOperationException(
                "Style engine has already been initialized.");
        var styleDefinitions = new StyleDefinitions
        {
            Handlers = AtomHandler.DefaultHandlers,
            Colors = Colors.ColorDict,
            Fonts = Fonts.FontDict
        };
        modifier(styleDefinitions);
        _finalAtomHandlers = AtomHandler.DefaultHandlers.ToFrozenDictionary();
        Fonts.FinalFonts = Fonts.ToFrozen();
    }

    public static void ParsePart(string part, ref StyleSet set)
    {
        _finalAtomHandlers ??= AtomHandler.DefaultHandlers.ToFrozenDictionary();
        try
        {
            var p = part.Trim();
            if (_finalAtomHandlers.TryGetValue(p, out var handler))
            {
                handler([p], false, set);
                return;
            }

            var isNegative = p.StartsWith('-');
            var items = p.Split('-');
            if (isNegative) items = items[1..];
            if (items.Length == 1) return;

            for (var i = 1; i < items.Length; i++)
            {
                var key = string.Join('-', items[..^i]) + '-';
                if (!_finalAtomHandlers.TryGetValue(key, out handler)) continue;

                handler(items[^i..], isNegative, set);
                break;
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
        }
    }

    public static void Parse(IEnumerable<string>? parts, ref StyleSet set)
    {
        if (parts is null) return;
        foreach (var part in parts)
        {
            ParsePart(part, ref set);
        }

        MergeTemp(ref set);
        set.TempStyle = null;
    }

    public static void Parse(string strStyle, ref StyleSet set)
    {
        var parts = strStyle.Split(
            [' ', '\n', '\r', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Parse(parts, ref set);
    }

    public static StyleSet Parse(string strStyle)
    {
        if (string.IsNullOrWhiteSpace(strStyle))
            return new StyleSet();

        lock (StyleCache)
        {
            if (StyleCache.TryGetValue(strStyle, out var cached))
                return cached;
        }

        var set = new StyleSet();

        Parse(strStyle, ref set);

        MergeTemp(ref set);
        set.TempStyle = null;

        lock (StyleCache)
        {
            StyleCache[strStyle] = set;
        }

        return set;
    }

    // 实现伪类
    public static void ParseFullStyle(string strStyle, ref FullStyle fullStyle)
    {
        var baseStyle = fullStyle.Normal;
        var variants = fullStyle.Variants;

        var parts = strStyle.Split(
            [' ', '\n', '\r', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            var variantPrefixes = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (variantPrefixes.Length == 1)
            {
                ParsePart(part, ref baseStyle);
                continue;
            }

            var value = variantPrefixes[^1];
            variantPrefixes = variantPrefixes[..^1];

            // TODO 按平台:响应式:状态:主题 排序
            Array.Sort(variantPrefixes);

            var key = string.Join(':', variantPrefixes);

            if (variants.TryGetValue(key, out var variant))
            {
                variant.Add(value);
                continue;
            }

            variants[key] = [value];
        }

        MergeTemp(ref baseStyle);
        baseStyle.TempStyle = null;
    }

    public static Accessor<FullStyle> ParseFull(
        Accessor<string>? strStyle = null,
        StyleSet? defaultStyle = null,
        Accessor<StyleSet>? style = null)
    {
        var fullStyle = new FullStyle();
        if (strStyle?.IsReactive is not true)
        {
            fullStyle.Normal = defaultStyle?.Copy() ?? StyleUtil.FromDefault();
            fullStyle.Variants = [];
            if (strStyle is not null) ParseFullStyle(strStyle.Value, ref fullStyle);
            if (style is null) return fullStyle;
            if (style.IsReactive)
            {
                return new Computed<FullStyle>(() =>
                {
                    var rxStyle = style.RxValue;
                    fullStyle.Normal.Merge(rxStyle);
                    return fullStyle;
                });
            }

            fullStyle.Normal.Merge(style.Value);
            return fullStyle;
        }

        if (style?.IsReactive is false)
        {
            return new Computed<FullStyle>(() =>
            {
                var str = strStyle.RxValue;
                fullStyle.Normal = defaultStyle?.Copy() ?? StyleUtil.FromDefault();
                fullStyle.Variants = [];
                ParseFullStyle(str, ref fullStyle);
                fullStyle.Normal.Merge(style.Value);
                return fullStyle;
            });
        }

        var computedStrStyle = new Computed<FullStyle>(() =>
        {
            var str = strStyle.RxValue;
            fullStyle.Normal = defaultStyle?.Copy() ?? StyleUtil.FromDefault();
            fullStyle.Variants = [];
            ParseFullStyle(str, ref fullStyle);
            return fullStyle;
        });
        if (style is null) return computedStrStyle;
        return new Computed<FullStyle>(() =>
        {
            var rxStrStyle = computedStrStyle.RxValue;
            var rxStyle = style.RxValue;
            rxStrStyle.Normal.Merge(rxStyle);
            return rxStrStyle;
        });
    }

    internal static void MergeTemp(ref StyleSet styles)
    {
        var temp = styles.TempStyle;
        if (temp is null) return;

        var gradientDir = temp.FgGradientDir;
        var fromColor = temp.FgFromColor;
        var toColor = temp.FgToColor;
        var foreground = GetGradientBrush(gradientDir, fromColor, toColor);
        if (foreground is not null) styles.Foreground = foreground;

        gradientDir = temp.BgGradientDir;
        fromColor = temp.BgFromColor;
        toColor = temp.BgToColor;
        var background = GetGradientBrush(gradientDir, fromColor, toColor);
        if (background is not null) styles.Background = background;

        gradientDir = temp.BorderGradientDir;
        fromColor = temp.BorderFromColor;
        toColor = temp.BorderToColor;
        var borderBrush = GetGradientBrush(gradientDir, fromColor, toColor);
        if (borderBrush is not null) styles.BorderBrush = borderBrush;

        var textDecoration = temp.TextDecoration;
        if (textDecoration is not null)
            styles.TextDecorations = [textDecoration];

        var builder = TransformOperations.CreateBuilder(4);

        if (temp.TranslateTransform is not null)
            builder.AppendTranslate(temp.TranslateTransform.X, temp.TranslateTransform.Y);
        if (temp.ScaleTransform is not null)
            builder.AppendScale(temp.ScaleTransform.ScaleX, temp.ScaleTransform.ScaleY);
        if (temp.RotateTransform is not null)
            builder.AppendRotate(temp.RotateTransform.Angle);
        if (temp.SkewTransform is not null)
            builder.AppendSkew(temp.SkewTransform.AngleX, temp.SkewTransform.AngleY);

        // if (advanced.MatrixTransform is not null) 
        //     builder.AppendMatrix(advanced.MatrixTransform);

        var ops = builder.Build();
        if (ops.Operations.Count > 0)
        {
            styles.RenderTransform = ops;
        }

        var scope = temp.TransitionScope;
        if (scope is null) return;

        if (styles.RenderTransform is null)
        {
            builder.AppendScale(1, 1);
            styles.RenderTransform = builder.Build();
        }

        var duration = temp.Duration ?? 300;

        TransitionBase? transition = scope switch
        {
            TransitionScope.Transform => new TransformOperationsTransition
                { Property = Visual.RenderTransformProperty },
            TransitionScope.Opacity => new DoubleTransition { Property = Visual.OpacityProperty },
            TransitionScope.Colors => new BrushTransition { Property = Border.BackgroundProperty },
            _ => null
        };
        if (transition is null) return;
        
        transition.Duration = TimeSpan.FromMilliseconds(duration);
        if (temp.Easing is not null) transition.Easing = temp.Easing;
        styles.Transitions =
        [
            transition
        ];
    }

    private static readonly Dictionary<double, (RelativePoint Start, RelativePoint End)> GradientPointsResultCache =
        new();

    private static IBrush? GetGradientBrush(double? gradientDir, Color? fromColor, Color? toColor)
    {
        if (gradientDir is null || fromColor is null || toColor is null) return null;

        var (start, end) = GetGradientPoints(gradientDir.Value);

        return new LinearGradientBrush
        {
            StartPoint = start,
            EndPoint = end,
            GradientStops =
            {
                new GradientStop(fromColor.Value, 0.0f),
                new GradientStop(toColor.Value, 1.0f)
            }
        };
    }

    private static (RelativePoint Start, RelativePoint End) GetGradientPoints(double angle)
    {
        if (GradientPointsResultCache.TryGetValue(angle, out var cacheValue)) return cacheValue;
        // 标准渐变数学计算：角度 → 方向向量
        var radians = angle * Math.PI / 180.0;
        var cos = Math.Cos(radians);
        var sin = Math.Sin(radians);

        // 中心点 (0.5, 0.5) 发射渐变（标准模式）
        var startX = 0.5 + cos * 0.5;
        var startY = 0.5 + sin * 0.5;
        var endX = 0.5 - cos * 0.5;
        var endY = 0.5 - sin * 0.5;

        var result = (
            new RelativePoint(startX, startY, RelativeUnit.Relative),
            new RelativePoint(endX, endY, RelativeUnit.Relative)
        );

        GradientPointsResultCache[angle] = result;

        return result;
    }
}