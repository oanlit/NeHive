using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HRadioButtonProp(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<string>? groupName = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<bool?>? onClick = null) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];
    
    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = isChecked;
    public readonly Accessor<string>? GroupName = groupName;
    public readonly Accessor<bool>? IsEnabled = isEnabled;
    public readonly Action<bool?>? OnClick = onClick;
    
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, radio, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(radio, border, StyleUtil.ApplyStyle);

        if (prop.IsEnabled is not null)
        {
            radio.IsEnabled = prop.IsEnabled.Value;
            if(prop.IsEnabled.IsReactive)
                uiScope.CreateEffect(epochScope => radio.IsEnabled = epochScope.Track(prop.IsEnabled));
        }
        
        if (prop.GroupName is not null)
        {
            radio.GroupName = prop.GroupName.Value;
            if(prop.GroupName.IsReactive)
                uiScope.CreateEffect(epochScope => radio.GroupName = epochScope.Track(prop.GroupName));
        }
 
        if (prop.BindIsChecked is not null)
        {
            uiScope.CreateEffect(() => radio.IsChecked = prop.BindIsChecked.RxValue);
            radio.Click += (_, _) =>
            {
                prop.BindIsChecked.NotifySet(prev => prev is not true);
                prop.OnClick?.Invoke(prop.BindIsChecked.Value);
            };
        }
        else if (prop.IsChecked is not null)
        {
            uiScope.CreateEffect(() => radio.IsChecked = prop.IsChecked.RxValue);
            radio.Click += (_, _) => prop.OnClick?.Invoke(radio.IsChecked);
        }
        else if (prop.OnClick is not null)
        {
            radio.Click += (_, _) => prop.OnClick?.Invoke(radio.IsChecked);
        }

        var firstChild = prop.FirstOrDefault();
        if (firstChild is not null)
            radio.Content = firstChild.Content;

        return new Element<RadioButton>(uiScope, border, radio);
    }
}