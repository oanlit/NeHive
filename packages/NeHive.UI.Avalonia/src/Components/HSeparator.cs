using Avalonia.Controls;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HSeparator(Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        var styleAccessor = StyleParser.ParseFull(strStyle, null, style);
        var uiScope = new UiScope();
        var sep = new Separator();
        var border = new Border
        {
            Child = sep
        };
        var state = new CommonState(uiScope, styleAccessor.Value.Normal)
        {
            StrVariants = styleAccessor.Value.Variants,
            Variants = variants
        };

        state.ApplyAccessorStyle(styleAccessor, sep, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(sep, border, StyleUtil.ApplyStyle);
        return new Element(uiScope, border);
    }
}