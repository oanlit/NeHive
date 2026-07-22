using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

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
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null,
        Action<RangeBaseValueChangedEventArgs>? onValueChanged = null)
    {
        return Element<ProgressBar>.WithScope(uiScope =>
        {
            value = bindValue ?? value ?? 0;
            minimum ??= 0;
            maximum ??= 100;
            isIndeterminate ??= false;
            isShowProgressText ??= false;

            var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

            var progressBar = new ProgressBar();
            var border = new Border
            {
                Child = progressBar
            };

            var state = new CommonState(uiScope, styleAccessor.Value.Normal)
            {
                StrVariants = styleAccessor.Value.Variants
            };

            state.ApplyAccessorStyle(styleAccessor, progressBar, border, ApplyStyle);
            state.ApplyVariantsStyle(progressBar, border, StyleUtil.ApplyStyle);
            
            RangeBaseUtil.BindAccessor(uiScope, progressBar, value, bindValue, minimum, maximum, smallChange, largeChange, onValueChanged);

            progressBar.Value = value.Value;
            if (value.IsReactive)
                uiScope.CreateEffect(epochScope => progressBar.Value = epochScope.Track(value));

            progressBar.IsIndeterminate = isIndeterminate.Value;
            if (isIndeterminate.IsReactive)
                uiScope.CreateEffect(epochScope => progressBar.IsIndeterminate = epochScope.Track(isIndeterminate));
            
            progressBar.ShowProgressText = isShowProgressText.Value;
            if (isIndeterminate.IsReactive)
                uiScope.CreateEffect(epochScope => progressBar.IsIndeterminate = epochScope.Track(isIndeterminate));

            if (progressTextFormat is not null)
            {
                progressBar.ProgressTextFormat = progressTextFormat.Value;
                if (progressTextFormat.IsReactive)
                    uiScope.CreateEffect(epochScope => progressBar.ProgressTextFormat = epochScope.Track(progressTextFormat));
            }

            return (progressBar, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.Orientation is not null) progressBar.Orientation = styleValue.Orientation.Value;
            }
        });
    }
}