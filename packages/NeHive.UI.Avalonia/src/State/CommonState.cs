using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.State;

public class CommonState(UiScope uiScope, StyleSet baseStyle)
{
    public StyleSet BaseStyle = baseStyle;
    public StyleSet CurrentStyle = baseStyle.Copy();
    public bool CurrentIsBase { get; private set; } = true;
    public Dictionary<string, List<string>>? StrVariants;
    public Dictionary<string, StyleSet>? Variants;

    // 鼠标交互状态（悬停、按下等）
    public bool IsHover;
    public bool IsClicked;

    public void ResetSetStyle()
    {
        if (CurrentIsBase) return;
        CurrentStyle.Merge(BaseStyle);
        CurrentIsBase = true;
    }

    public void SetCurrentStyle()
    {
        if (StrVariants == null) return;
        SetHoverStyle();
        SetClickStyle();
    }

    public void SetHoverStyle()
    {
        if (!IsHover) return;
        if (StrVariants is not null && StrVariants.TryGetValue("hover", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
            CurrentIsBase = false;
        }

        if (Variants is not null && Variants.TryGetValue("hover", out var styleSet))
        {
            CurrentStyle.Merge(styleSet);
            CurrentIsBase = false;
        }
    }

    public void SetClickStyle()
    {
        if (!IsClicked) return;
        if (StrVariants is not null && StrVariants.TryGetValue("click", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
            CurrentIsBase = false;
        }

        if (Variants is not null && Variants.TryGetValue("click", out var styleSet))
        {
            CurrentStyle.Merge(styleSet);
            CurrentIsBase = false;
        }
    }

    public void ApplyAccessorStyle(
        Accessor<FullStyle> accessorStyle,
        Layoutable layout, Border border,
        Action<StyleSet, Layoutable, Border> applyStyle)
    {
        if (accessorStyle.IsReactive)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(accessorStyle);
                BaseStyle = styleValue.Normal;
                StrVariants = styleValue.Variants;
                CurrentStyle = BaseStyle.Copy();
                applyStyle(CurrentStyle, layout, border);
            });
        }
        else
        {
            applyStyle(CurrentStyle, layout, border);
        }
    }

    public void ApplyVariantsStyle(Layoutable layout, Border border, 
        Action<StyleSet, Layoutable, Border> applyStyle)
    {
        border.PointerPressed += (_, e) =>
        {
            if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                return;

            IsClicked = true;
            SetClickStyle();
            applyStyle(CurrentStyle, layout, border);
        };

        border.PointerReleased += (_, _) =>
        {
            IsClicked = false;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        };

        border.PointerExited += (_, _) =>
        {
            IsHover = false;
            IsClicked = false; // 移出区域时取消按下状态
            ResetSetStyle();
            applyStyle(CurrentStyle, layout, border);
        };

        border.PointerEntered += (_, _) =>
        {
            IsHover = true;
            SetHoverStyle();
            applyStyle(CurrentStyle, layout, border);
        };
    }
}