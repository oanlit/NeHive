using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.State;

public interface IGroupState;

public class GroupState(StyleSet baseStyle) : IGroupState
{
    internal MutSignal<bool>? InnerIsHover;
    internal MutSignal<bool>? InnerIsFocus;
    internal MutSignal<bool>? InnerIsClicked;

    public StyleSet BaseStyle = baseStyle;
    public StyleSet CurrentStyle = baseStyle.Copy();
    public Dictionary<string, List<string>>? Variants;

    public Signal<bool> IsHover
    {
        get
        {
            InnerIsHover ??= new MutSignal<bool>(false);
            return InnerIsHover;
        }
    }

    public Signal<bool> IsFocus
    {
        get
        {
            InnerIsFocus ??= new MutSignal<bool>(false);
            return InnerIsFocus;
        }
    }
    
    public Signal<bool> IsClicked
    {
        get
        {
            InnerIsClicked ??= new MutSignal<bool>(false);
            return InnerIsClicked;
        }
    }

    public void ResetSetStyle()
    {
        CurrentStyle.Merge(BaseStyle);
    }

    public void SetCurrentStyle()
    {
        if (Variants == null) return;
        if (IsHover.Value && Variants.TryGetValue("hover", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
        }

        if (IsClicked.Value && Variants.TryGetValue("click", out strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
        }
    }

    public void SetHoverStyle()
    {
        if (Variants == null) return;
        if (IsHover.Value && Variants.TryGetValue("hover", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
        }
    }

    public void SetClickStyle()
    {
        if (Variants == null) return;
        if (IsClicked.Value && Variants.TryGetValue("click", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
        }
    }
}