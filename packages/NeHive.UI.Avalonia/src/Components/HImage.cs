using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HImageProps(Scope scope, Border border, Image image)
    : BaseComponentProps(scope, border, image)
{
    // public MutSignal<string> Uri
    // {
    //     get
    //     {
    //         if (field is not null) return field;
    //         field = new MutSignal<string>("");
    //         return field;
    //     }
    // }

    public MutSignal<IImage?> Source
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<IImage?>(image.Source);
            BridgeAvalonia.BindPropertySignal(scope, field, image, Image.SourceProperty);
            return field;
        }
    }

    public MutSignal<BitmapBlendingMode> BlendMode
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<BitmapBlendingMode>(image.BlendMode);
            BridgeAvalonia.BindPropertySignal(scope, field, image, Image.BlendModeProperty);
            return field;
        }
    }

    public MutSignal<Stretch> Stretch
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<Stretch>(image.Stretch);
            BridgeAvalonia.BindPropertySignal(scope, field, image, Image.StretchProperty);
            return field;
        }
    }

    public MutSignal<StretchDirection> StretchDirection
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<StretchDirection>(image.StretchDirection);
            BridgeAvalonia.BindPropertySignal(scope, field, image, Image.StretchDirectionProperty);
            return field;
        }
    }
}

public class HImageArgs(
    Accessor<string?>? uri = null,
    Accessor<IImage?>? source = null,
    Accessor<BitmapBlendingMode>? blendMode = null,
    Accessor<Stretch>? stretch = null,
    Accessor<StretchDirection>? stretchDirection = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<string?>? Uri = uri;
    public readonly Accessor<IImage?>? Source = source;
    public readonly Accessor<BitmapBlendingMode>? BlendMode = blendMode;
    public readonly Accessor<Stretch>? Stretch = stretch;
    public readonly Accessor<StretchDirection>? StretchDirection = stretchDirection;
}

public static partial class BaseComponent
{
    public static IElement<Image> HImage(
        Accessor<string?>? uri = null,
        Accessor<IImage?>? source = null,
        Accessor<BitmapBlendingMode>? blendMode = null,
        Accessor<Stretch>? stretch = null,
        Accessor<StretchDirection>? stretchDirection = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HImage(out _, _ => new(uri, source, blendMode, stretch, stretchDirection, strStyle, style, baseInteraction));

    public static IElement<Image> HImage(
        out Image expose,
        Accessor<string?>? uri = null,
        Accessor<IImage?>? source = null,
        Accessor<BitmapBlendingMode>? blendMode = null,
        Accessor<Stretch>? stretch = null,
        Accessor<StretchDirection>? stretchDirection = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null,
        BaseComponentInteraction? baseInteraction = null
    ) => HImage(out expose,
        _ => new(uri, source, blendMode, stretch, stretchDirection, strStyle, style, baseInteraction));

    public static IElement<Image> HImage(Func<HImageProps, HImageArgs> fn) => HImage(out _, fn);

    public static IElement<Image> HImage(out Image expose, Func<HImageProps, HImageArgs> fn)
    {
        var image = new Image();
        expose = image;
        return Element<Image>.WithScope(uiScope =>
        {
            var border = new Border();
            var props = new HImageProps(uiScope, border, image);
            var args = fn(props);

            border.Child = image;

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, image, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(image, border, StyleUtil.ApplyStyle);
            if (args.Popups is not null)
                ElementUtil.ApplyPopups(border, args.Popups);
            args.BaseInteraction?.ApplyInteractions(uiScope, image);

            if (args.Source is not null)
            {
                image.Source = args.Source.Value;
                if (args.Source.IsReactive)
                    uiScope.CreateEffect(epoch => image.Source = epoch.Track(args.Source));
            }
            else if (args.Uri is not null)
            {
                if (args.Uri.Value is not null) image.Source = ImageUtil.LoadImage(args.Uri.Value);
                if (args.Uri.IsReactive)
                    uiScope.CreateEffect(epoch =>
                    {
                        var uri = epoch.Track(args.Uri);
                        image.Source = uri is null ? null : ImageUtil.LoadImage(uri);
                    });
            }

            if (args.BlendMode is not null)
            {
                image.BlendMode = args.BlendMode.Value;
                if (args.BlendMode.IsReactive)
                    uiScope.CreateEffect(epoch => image.BlendMode = epoch.Track(args.BlendMode));
            }

            if (args.Stretch is not null)
            {
                image.Stretch = args.Stretch.Value;
                if (args.Stretch.IsReactive)
                    uiScope.CreateEffect(epoch => image.Stretch = epoch.Track(args.Stretch));
            }

            if (args.StretchDirection is not null)
            {
                image.StretchDirection = args.StretchDirection.Value;
                if (args.StretchDirection.IsReactive)
                    uiScope.CreateEffect(epoch => image.StretchDirection = epoch.Track(args.StretchDirection));
            }

            return (image, border);
        });
    }
}