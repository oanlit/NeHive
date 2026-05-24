using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HRadioButtonProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle>? Style;

    public readonly MutSignal<bool?>? BindIsChecked;
    public readonly Accessor<bool?>? IsChecked;
    public readonly Accessor<string>? GroupName;
    public readonly Accessor<bool>? IsEnabled;
    public readonly Action<bool?>? OnClick;

    public HRadioButtonProp(
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<string>? groupName = null,
        Accessor<bool>? isEnabled = null,
        Accessor<string>? strStyle = null,
        Action<bool?>? onClick = null)
    {
        BindIsChecked = bindIsChecked;
        IsChecked = isChecked;
        GroupName = groupName;
        IsEnabled = isEnabled;
        OnClick = onClick;
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

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
        var border = new Border
        {
            Child = radio
        };

        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal, radio, border);
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

        return new Element<RadioButton>(uiScope, border, radio);
    }
}