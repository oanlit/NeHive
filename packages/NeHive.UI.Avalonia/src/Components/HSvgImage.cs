using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;
using Path = Avalonia.Controls.Shapes.Path;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HSvgImage(
        Accessor<string> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

        var uiScope = new UiScope();

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
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, image, border, ApplyStyle);
        state.ApplyVariantsStyle(image, border, ApplyStyle);

        // 绑定 Data
        image.Data = SvgUtil.ParseGeometry(SvgUtil.LoadSvgString(uri.Value));
        if (uri.IsReactive)
        {
            uiScope.CreateEffect(() =>
            {
                var svg = SvgUtil.LoadSvgString(uri.RxValue);
                var data = SvgUtil.ParseGeometry(svg);
                image.Data = data;
            });
        }

        if (stretch is not null)
        {
            image.Stretch =  stretch.Value;
            if(stretch.IsReactive)
            {
                uiScope.CreateEffect(epochScope =>
                {
                    var stretchValue = epochScope.Track(stretch);
                    image.Stretch = stretchValue;
                });
            }
        }


        return new Element(uiScope, border);

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
    }
}