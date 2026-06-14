using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HProgressBar(
        Accessor<double>? value = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<bool>? isIndeterminate = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        value ??= 0;
        minimum ??= 0;
        maximum ??= 100;
        isIndeterminate ??= false;

        // 样式合并
        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);

        var uiScope = new UiScope();
        var progressBar = new ProgressBar();
        var border = new Border
        {
            Child = progressBar
        };
        
        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, progressBar, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(progressBar, border, StyleUtil.ApplyStyle);

        // 绑定属性
        progressBar.Value = value.Value;
        if(value.IsReactive)
            uiScope.CreateEffect(epochScope => progressBar.Value = epochScope.Track(value));

        progressBar.Minimum = minimum.Value;
        if(minimum.IsReactive)
            uiScope.CreateEffect(epochScope => progressBar.Minimum = epochScope.Track(minimum));
        
        progressBar.Maximum = maximum.Value;
        if(maximum.IsReactive)
            uiScope.CreateEffect(epochScope => progressBar.Maximum = epochScope.Track(maximum));

        progressBar.IsIndeterminate = isIndeterminate.Value;
        if(isIndeterminate.IsReactive)
            uiScope.CreateEffect(epochScope => progressBar.IsIndeterminate = epochScope.Track(isIndeterminate));

        return new Element(uiScope, border);
    }
}