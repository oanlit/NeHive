using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public class HRadioButtonProp(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<string>? groupName = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    Accessor<HPanelStyle>? style = null,
    Action<bool?>? onClick = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<HPanelStyle>? ComputedStyle = (style, strStyle) switch
    {
        (not null, not null) => new Computed<HPanelStyle>(() =>
            HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
        (not null, _) => style,
        (_, not null) => HPanelStyle.Parse(strStyle),
        _ => null
    };

    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = isChecked;
    public readonly Accessor<string>? GroupName = groupName;
    public readonly Accessor<bool>? IsEnabled = isEnabled;
    public readonly Action<bool?>? OnClick = onClick;

    public void Add(IElement element) => _children.Add(element);
    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<RadioButton> HRadioButton(HRadioButtonProp prop)
    {
        var uiScope = new UiScope();
        var radio = new RadioButton();

        if (prop.ComputedStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.ComputedStyle);
                ApplyPanelStyle(radio, style);
            });
        }

        if (prop.IsEnabled != null)
            uiScope.CreateEffect(() => radio.IsEnabled = prop.IsEnabled.RxValue);
        if (prop.GroupName != null)
            uiScope.CreateEffect(() => radio.GroupName = prop.GroupName.RxValue);

        if (prop.BindIsChecked != null)
        {
            uiScope.CreateEffect(() => radio.IsChecked = prop.BindIsChecked.RxValue);
            radio.Click += (_, _) =>
            {
                prop.BindIsChecked.NotifySet(prev => prev is not true);
                prop.OnClick?.Invoke(prop.BindIsChecked.Value);
            };
        }
        else if (prop.IsChecked != null)
        {
            uiScope.CreateEffect(() => radio.IsChecked = prop.IsChecked.RxValue);
            radio.Click += (_, _) => prop.OnClick?.Invoke(radio.IsChecked);
        }
        else if (prop.OnClick != null)
        {
            radio.Click += (_, _) => prop.OnClick?.Invoke(radio.IsChecked);
        }

        var firstChild = prop.FirstOrDefault();
        if (firstChild != null)
            radio.Content = firstChild.Content;

        return new Element<RadioButton>(uiScope, radio, radio);

        void ApplyPanelStyle(Control control, HPanelStyle style)
        {
            control.Margin = style.Margin;
            control.HorizontalAlignment = style.HorizontalAlignment;
            control.VerticalAlignment = style.VerticalAlignment;
            // control.Background = style.Background;
        }
    }
}