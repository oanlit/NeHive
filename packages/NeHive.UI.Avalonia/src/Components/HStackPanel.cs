using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement<StackPanel> HStackPanel(Func<HPanelProps,HPanelArgs> fn)
    {
        return Element<StackPanel>.WithScope(uiScope =>
        {
            var stack = new StackPanel();
            var border = new Border
            {
                Child = stack
            };

            var props = new HPanelProps(uiScope, border, stack);
            var args = fn(props);

            foreach (var child in args)
                stack.Children.Add(child.Content);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, stack, border, ApplyStyle);
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
                        if (styleValue.GapY is not null) stack.Spacing = styleValue.GapY.Value;
                        break;
                    case Orientation.Horizontal:
                        if (styleValue.GapX is not null) stack.Spacing = styleValue.GapX.Value;
                        break;
                }
            }
        });
    }
}