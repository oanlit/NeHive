using Avalonia.Controls;
using Avalonia.Media;
using NeHive.UI.Avalonia.Styles;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public class HGroupProp<T> where T : IGroupState
{
    public readonly Accessor<FullStyle>? Style;
    public required Func<T, IElement> Child { get; init; }

    public HGroupProp(Accessor<string>? strStyle = null)
    {
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }
}

public static partial class BaseComponent
{
    public static IElement HGroup(HGroupProp<GroupState> prop)
    {
        var uiScope = new UiScope();
        GroupState state;

        var border = new Border();

        var style = prop.Style;
        if (style is null)
        {
            state = new GroupState(HButtonStyle.DefaultStyleSet);
            state.CurrentStyle.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            StyleUtil.ApplyStyle(state.CurrentStyle, border, border); // 应用默认样式
        }
        else
        {
            state = new GroupState(style.Value.Normal);
            StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                state.BaseStyle = styleValue.Normal;
                state.Variants = styleValue.Variants;
                state.CurrentStyle = StyleUtil.Copy(state.BaseStyle);
                state.CurrentStyle.Background ??= new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
            });
        }

        uiScope.OnMount += () =>
        {
            border.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                    return;

                state.SetClicked(true);
                state.SetClickStyle();
                StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
                e.Handled = true;
            };

            border.PointerReleased += (_, e) =>
            {
                state.SetClicked(false);
                state.ResetSetStyle();
                state.SetCurrentStyle();
                StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
                e.Handled = true;
            };

            border.PointerExited += (_, _) =>
            {
                state.SetHover(false);
                state.SetClicked(false); // 移出区域时取消按下状态
                state.ResetSetStyle();
                StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
            };

            border.PointerEntered += (_, _) =>
            {
                state.SetHover(true);
                state.SetHoverStyle();
                StyleUtil.ApplyStyle(state.CurrentStyle, border, border);
            };
        };

        border.Child = prop.Child(state).Content;
        return new Element(uiScope, border);
    }
}