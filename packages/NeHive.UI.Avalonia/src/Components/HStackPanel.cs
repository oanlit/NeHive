using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement<StackPanel> HStackPanel(HPanelProp prop)
    {
        return Element<StackPanel>.WithScope(uiScope =>
        {
            var stack = new StackPanel();
            var border = new Border
            {
                Child = stack
            };

            foreach (var child in prop)
                stack.Children.Add(child.Content);

            var state = new CommonState(uiScope, prop.Style.Value.Normal)
            {
                StrVariants = prop.Style.Value.Variants
            };

            state.ApplyAccessorStyle(prop.Style, stack, border, ApplyStyle);
            state.ApplyVariantsStyle(stack, border, ApplyStyle);

            return (stack, border);

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                var orientation = styleValue.Orientation ?? Orientation.Vertical;
                stack.Orientation = orientation;

                switch (orientation)
                {
                    case Orientation.Vertical:
                        if (styleValue.RowSpacing is not null) stack.Spacing = styleValue.RowSpacing.Value;
                        break;
                    case Orientation.Horizontal:
                        if (styleValue.ColumnSpacing is not null) stack.Spacing = styleValue.ColumnSpacing.Value;
                        break;
                }
            }
        });
    }
}