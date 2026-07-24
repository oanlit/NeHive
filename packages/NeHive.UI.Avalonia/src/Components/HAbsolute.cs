using System.Collections;
using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class AbsPosition(
    Accessor<double>? left = null,
    Accessor<double>? top = null,
    Accessor<double>? right = null,
    Accessor<double>? bottom = null
)
{
    public readonly Accessor<double>? Left = left;
    public readonly Accessor<double>? Top = top;
    public readonly Accessor<double>? Right = right;
    public readonly Accessor<double>? Bottom = bottom;
}

public class HAbsoluteProps(Scope scope, Border border, Canvas canvas)
    : BaseComponentProps(scope, border, canvas);

public class HAbsoluteArgs(
    Accessor<string>? strStyle = null,
    HStyle? style = null
) : IEnumerable<KeyValuePair<AbsPosition, IElement>>
{
    private readonly Dictionary<AbsPosition, IElement> _children = new();

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public IElement this[AbsPosition key]
    {
        set => _children[key] = value;
    }

    public IEnumerator<KeyValuePair<AbsPosition, IElement>> GetEnumerator()
        => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement HAbsolute(Func<HAbsoluteProps,HAbsoluteArgs> fn)
    {
        return Element.WithScope(uiScope =>
        {
            var canvas = new Canvas();

            var border = new Border
            {
                Child = canvas
            };
            
            var props = new HAbsoluteProps(uiScope, border, canvas);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, canvas, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(canvas, border, StyleUtil.ApplyStyle);

            foreach (var (pos, element) in args)
            {
                var control = element.Content;
                SetPos(control, pos.Left?.Value, pos.Top?.Value, pos.Right?.Value, pos.Bottom?.Value);

                if (pos.Left?.IsReactive is true ||
                    pos.Top?.IsReactive is true ||
                    pos.Right?.IsReactive is true ||
                    pos.Bottom?.IsReactive is true
                   )
                    uiScope.CreateEffect(() =>
                    {
                        SetPos(control, pos.Left?.RxValue, pos.Top?.RxValue, pos.Right?.RxValue,
                            pos.Bottom?.RxValue);
                    });

                canvas.Children.Add(control);
            }

            return border;

            void SetPos(Control control, double? left, double? top, double? right, double? bottom)
            {
                if (left is not null) Canvas.SetLeft(control, left.Value);
                if (top is not null) Canvas.SetTop(control, top.Value);
                if (right is not null) Canvas.SetRight(control, right.Value);
                if (bottom is not null) Canvas.SetBottom(control, bottom.Value);
            }
        });
    }
}