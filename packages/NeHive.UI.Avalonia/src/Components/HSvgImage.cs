using Avalonia.Controls;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.Utils;
using Path = Avalonia.Controls.Shapes.Path;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HSvgImage(
        Accessor<string> uri,
        Accessor<Stretch>? stretch = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null
    )
    {
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

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

        // 绑定 Data
        uiScope.CreateEffect(() =>
        {
            var svg = SvgUtil.LoadSvgString(uri.RxValue);
            var data = SvgUtil.ParseGeometry(svg);
            image.Data = data;
        });

        HImageState state;

        if (style is null)
        {
            state = new HImageState(new StyleSet());
            ApplyStyle(state.BaseStyle);
        }
        // 绑定样式
        else
        {
            state = new HImageState(style.Value.Normal);
            ApplyStyle(style.Value.Normal);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                state.BaseStyle = styleValue.Normal;
                state.Variants = styleValue.Variants;
                ApplyStyle(styleValue.Normal);
                state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
            });
        }

        if (stretch is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var stretchValue = epochScope.Track(stretch);
                image.Stretch = stretchValue;
            });
        }

        // 事件挂载
        uiScope.OnMount += () =>
        {
            panel.PointerExited += (_, _) =>
            {
                state.IsHover = false;
                state.ResetSetStyle();
                ApplyStyle(state.CurrentStyle);
            };

            panel.PointerEntered += (_, _) =>
            {
                state.IsHover = true;
                state.SetHoverStyle();
                ApplyStyle(state.CurrentStyle);
            };
        };

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet styleValue)
        {
            StyleUtil.ApplyStyle(styleValue, image, border);
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