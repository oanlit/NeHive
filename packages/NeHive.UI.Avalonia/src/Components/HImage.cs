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
        HStyle? style = null
    )
    {
        return Element.WithScope(uiScope =>
        {
            var styleAccessor = StyleParser.ParseFull(strStyle);
            var priorityStyle = style is null ? null : StyleUtil.HStyle2Signal(style);

            var image = new Image();

            var border = new Border
            {
                Child = image,
                ClipToBounds = true
            };
            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                PriorityStyle = priorityStyle,
                StrVariants = styleAccessor.Value.Variants
            };

            state.ApplyAccessorStyle(styleAccessor, image, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(image, border, StyleUtil.ApplyStyle);

            image.Source = source.Value;
            if (source.IsReactive)
                uiScope.CreateEffect(() => image.Source = source.RxValue);

            if (stretch is not null)
            {
                image.Stretch = stretch.Value;
                if (stretch.IsReactive)
                    uiScope.CreateEffect(epochScope => image.Stretch = epochScope.Track(stretch));
            }

            return border;
        });
    }

    public static IElement HUriImage(
        Accessor<string?> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null
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

        return HImage(sourceSignal, stretch, strStyle, style);
    }

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