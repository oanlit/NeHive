using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Input;
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

    public bool IsHover;
    public bool IsClicked;
    public bool IsFocused;
    public bool IsDragOver;

    public void ResetSetStyle()
    {
        if (CurrentIsBase) return;
        CurrentStyle.Merge(BaseStyle, true);
        CurrentIsBase = true;
    }

    public void SetCurrentStyle()
    {
        if (StrVariants is null && Variants is null) return;
        SetHoverStyle();
        SetFocusStyle();
        SetClickStyle();
        SetDragOverStyle();
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

    public void SetDragOverStyle()
    {
        if (!IsDragOver) return;
        if (StrVariants is not null && StrVariants.TryGetValue("dragover", out var strs))
        {
            StyleParser.Parse(strs, ref CurrentStyle);
            CurrentIsBase = false;
        }

        if (Variants is not null && Variants.TryGetValue("dragover", out var styleSet))
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

    public void ApplyVariantsStyle(InputElement layout, Border border,
        Action<StyleSet, Layoutable, Border> applyStyle)
    {
        var hover = BindPointerOver(border, uiScope);
        uiScope.CreateEffect(epoch =>
        {
            var newHover = epoch.Pull(hover);
            if (IsHover == newHover) return;

            IsHover = newHover;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        });

        var focus = BindFocused(layout, uiScope);
        uiScope.CreateEffect(epoch =>
        {
            var newFocus = epoch.Pull(focus);
            if (IsFocused == newFocus) return;

            IsFocused = newFocus;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        });

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

        DragDrop.AddDragEnterHandler(border, (_, _) =>
        {
            IsDragOver = true;
            SetDragOverStyle();
            applyStyle(CurrentStyle, layout, border);
        });

        DragDrop.AddDragLeaveHandler(border, (_, _) => LoseDragState());
        DragDrop.AddDropHandler(border, (_, _) => LoseDragState());

        return;

        void LoseDragState()
        {
            IsDragOver = false;
            ResetSetStyle();
            SetCurrentStyle();
            applyStyle(CurrentStyle, layout, border);
        }
    }

    public static Signal<bool> BindPointerOver(InputElement target, UiScope scope)
    {
        var sig = new MutSignal<bool>(target.IsPointerOver);

        target.PropertyChanged += OnPropUpdate;
        scope.OnCleanup += () => target.PropertyChanged -= OnPropUpdate;

        return sig;

        void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == InputElement.IsPointerOverProperty)
                sig.RxValue = (bool)args.NewValue!;
        }
    }

    public static Signal<bool> BindFocused(InputElement target, UiScope scope)
    {
        var sig = new MutSignal<bool>(target.IsFocused);

        target.PropertyChanged += OnPropUpdate;
        scope.OnCleanup += () => target.PropertyChanged -= OnPropUpdate;

        return sig;

        void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == InputElement.IsFocusedProperty)
                sig.RxValue = (bool)args.NewValue!;
        }
    }
}