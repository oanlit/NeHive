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
    public bool IsFocused; // 新增：焦点状态

    public void ResetSetStyle()
    {
        if (CurrentIsBase) return;
        CurrentStyle.Merge(BaseStyle);
        CurrentIsBase = true;
    }

    public void SetCurrentStyle()
    {
        if (StrVariants is null && Variants is null) return;
        SetHoverStyle();
        SetFocusStyle();
        SetClickStyle();
    }

    public void SetHoverStyle()
    {
        if (!IsHover) return;
        if (IsFocused)
        {
            if (StrVariants is not null && StrVariants.TryGetValue("focus:hover", out var strs))
            {
                StyleParser.Parse(strs, ref CurrentStyle);
                CurrentIsBase = false;
            }

            if (Variants is not null && Variants.TryGetValue("focus:hover", out var styleSet))
            {
                CurrentStyle.Merge(styleSet);
                CurrentIsBase = false;
            }
        }
        else
        {
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

    // 新增：焦点样式设置
    public void SetFocusStyle()
    {
        if (!IsFocused) return;
        if (StrVariants is not null && StrVariants.TryGetValue("focus", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
            CurrentIsBase = false;
        }

        if (Variants is not null && Variants.TryGetValue("focus", out var styleSet))
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
        applyStyle(CurrentStyle, layout, border);
        if (!accessorStyle.IsReactive) return;
        var firstApply = true;
        uiScope.CreateEffect(epochScope =>
        {
            var styleValue = epochScope.Track(accessorStyle);
            BaseStyle = styleValue.Normal;
            StrVariants = styleValue.Variants;
            CurrentStyle = BaseStyle.Copy();
            if (firstApply)
            {
                firstApply = false;
                return;
            }

            applyStyle(CurrentStyle, layout, border);
        });
    }

    public void ApplyVariantsStyle(Layoutable layout, Border border,
        Action<StyleSet, Layoutable, Border> applyStyle)
    {
        // 焦点事件绑定
        border.GotFocus += (_, _) =>
        {
            IsFocused = true;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        };
        border.LostFocus += (_, _) =>
        {
            IsFocused = false;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        };
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
            IsClicked = false;
            ResetSetStyle();
            SetCurrentStyle();
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