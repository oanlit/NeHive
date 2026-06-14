using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HImage(
        Accessor<Bitmap?> source,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

        var uiScope = new UiScope();

        var image = new Image();

        var border = new Border
        {
            Child = image,
            ClipToBounds = true
        };
        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, image, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(image, border, StyleUtil.ApplyStyle);

        // 绑定 Source
        image.Source = source.Value;
        if (source.IsReactive)
            uiScope.CreateEffect(() => image.Source = source.RxValue);

        if (stretch is not null)
        {
            image.Stretch = stretch.Value;
            if (stretch.IsReactive)
                uiScope.CreateEffect(epochScope => image.Stretch = epochScope.Track(stretch));
        }

        return new Element(uiScope, border);
    }

    // 可选：支持从 Uri 或字符串路径加载图像的重载版本
    public static IElement HUriImage(
        Accessor<string?> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        var sourceSignal = new Computed<Bitmap?>(() =>
        {
            var u = uri.RxValue;
            if (string.IsNullOrEmpty(u)) return null;

            if (!u.StartsWith("avares://")) return LoadBitmapFromUri(u);

            var avaresUri = new Uri(u);

            using var stream = AssetLoader.Open(avaresUri);
            return new Bitmap(stream);
        });

        return HImage(sourceSignal, stretch, strStyle, style, variants);
    }

    // 示例：简单的 Uri 加载（实际应缓存和异步处理）
    private static Bitmap? LoadBitmapFromUri(string uri)
    {
        try
        {
            return new Bitmap(uri);
        }
        catch
        {
            return null;
        }
    }
}