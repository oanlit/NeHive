using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.Model;
using NeHive.UI.Avalonia.Styles;
using Avalonia.Layout;

namespace NeHive.UI.Avalonia.Components;

public class RootProp : HStackPanelProp
{
    public Action<IContextSetter>? ContextSetter;

    public RootProp(Action<IContextSetter>? contextSetter = null, Accessor<string>? strStyle = null) : base(strStyle)
    {
        ContextSetter = contextSetter;
    }
}

public static partial class BaseComponent
{
    public static IElement RootElement(SingleChildrenProp prop)
    {
        var scope = Model.NeHiveContext.CurrentScope;
        if (scope is not UiScope uiScope)
            throw new InvalidOperationException("CurrentScope is not UiScope");

        var stack = new StackPanel();
        foreach (var el in prop)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    }

    public static IElement RootElement(RootProp prop, UiScope uiScope)
    {
        var stack = new StackPanel();
        var border = new Border
        {
            Child = stack
        };

        uiScope.CreateEffect(epochScope =>
        {
            if (prop.Style == null) return;
            var track = epochScope.Track(prop.Style);
            ApplyStyle(track.Normal);
        });

        foreach (var el in prop)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, border);

        void ApplyStyle(StyleSet style)
        {
            StyleUtil.ApplyStyle(style, stack, border);

            var orientation = style.Orientation ?? Orientation.Vertical;
            stack.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Vertical:
                    if (style.RowSpacing is not null) stack.Spacing = style.RowSpacing.Value;
                    break;
                case Orientation.Horizontal:
                    if (style.ColumnSpacing is not null) stack.Spacing = style.ColumnSpacing.Value;
                    break;
            }

            var overflowHandle = style.OverflowHandle;
            if (overflowHandle is not null)
            {
                if (overflowHandle is OverflowHandle.Visible)
                    stack.ClipToBounds = false;
                else if (overflowHandle is OverflowHandle.Hidden)
                    stack.ClipToBounds = true;
            }

            if (style.Orientation is not null) stack.Orientation = style.Orientation.Value;
        }
    }
}