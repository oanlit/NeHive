using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia;

public interface IGroupState;

public class GroupState(StyleSet baseStyle) : IGroupState
{
    private MutSignal<bool>? _isHover;
    private MutSignal<bool>? _isFocus;
    private MutSignal<bool>? _isClicked;

    public StyleSet BaseStyle = baseStyle;
    public StyleSet CurrentStyle = StyleUtil.Copy(baseStyle);
    public Dictionary<string, List<string>>? Variants;


    public Signal<bool> IsHover
    {
        get
        {
            _isHover ??= new MutSignal<bool>(false);
            return _isHover;
        }
    }

    internal void SetHover(bool value) => _isHover?.RxValue = value;

    public Signal<bool> IsFocus
    {
        get
        {
            _isFocus ??= new MutSignal<bool>(false);
            return _isFocus;
        }
    }

    internal void SetFocus(bool value) => _isFocus?.RxValue = value;

    public Signal<bool> IsClicked
    {
        get
        {
            _isClicked ??= new MutSignal<bool>(false);
            return _isClicked;
        }
    }

    internal void SetClicked(bool value) => _isClicked?.RxValue = value;

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