using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;
using Path = Avalonia.Controls.Shapes.Path;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HSvgImageProps(Scope scope, Border border, Path path)
    : BaseComponentProps(scope, border, path)
{
    
}

public static partial class BaseComponent
{
    public static IElement HSvgImage(
        Accessor<string>? uri = null,
        Accessor<string>? path = null,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        HStyle? style = null
    )
    {
        return Element.WithScope(uiScope =>
        {
            var styleAccessor = StyleParser.ParseFull(strStyle);

            var image = new Path
            {
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Stretch = Stretch.Uniform,
                StrokeThickness = 2
            };

            var panel = new Panel
            {
                Children = { image },
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            };

            var border = new Border
            {
                Child = panel
            };

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                PriorityStyle = style is null ? null : StyleUtil.HStyle2Signal(style),
                StrVariants = styleAccessor.Value.Variants
            };

            state.ApplyAccessorStyle(styleAccessor, image, border, ApplyStyle);
            state.ApplyVariantsStyle(image, border, ApplyStyle);

            Accessor<Geometry?>? data = null;
            if (path is null && uri is not null)
            {
                if (uri.IsReactive)
                    data = uiScope.CreateComputed(() => SvgUtil.ParseGeometry(SvgUtil.LoadSvgString(uri.RxValue)));
                else
                    data = SvgUtil.ParseGeometry(SvgUtil.LoadSvgString(uri.Value));
            }
            else if (path is not null)
            {
                if (path.IsReactive)
                {
#pragma warning disable CS8619
                    data = uiScope.CreateComputed(() => Geometry.Parse(path.RxValue));
#pragma warning restore CS8619
                }
                else
                    data = Geometry.Parse(path.Value);
            }


            // image.Data = SvgUtil.ParseGeometry(SvgUtil.LoadSvgString(uri.Value));
            image.Data = data?.Value;
            // if (uri.IsReactive)
            // {
            //     uiScope.CreateEffect(() =>
            //     {
            //         var svg = SvgUtil.LoadSvgString(uri.RxValue);
            //         var data = SvgUtil.ParseGeometry(svg);
            //         image.Data = data;
            //     });
            // }
            if (data?.IsReactive is true)
                uiScope.CreateEffect(epoch => image.Data = epoch.Track(data));

            if (stretch is not null)
            {
                image.Stretch = stretch.Value;
                if (stretch.IsReactive)
                {
                    uiScope.CreateEffect(epochScope =>
                    {
                        var stretchValue = epochScope.Track(stretch);
                        image.Stretch = stretchValue;
                    });
                }
            }

            return border;

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);
                var fg = styleValue.Foreground;
                if (fg is not null) image.Stroke = fg;
                var fontWeight = styleValue.FontWeight;
                if (fontWeight is not null)
                {
                    var weight = (int)fontWeight.Value / 100;
                    image.StrokeThickness = weight;
                }
            }
        });
    }
}