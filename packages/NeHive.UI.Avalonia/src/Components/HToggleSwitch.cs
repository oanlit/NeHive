using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HToggleSwitchProp(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<bool?>? onCheckedChanged = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked =  isChecked;
    public readonly Accessor<bool>? IsEnabled =  isEnabled;
    public readonly Action<bool?>? OnCheckedChanged =  onCheckedChanged;

    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    // 添加子内容（显示在开关旁边的标签或控件）
    public void Add(IElement element) => _children.Add(element);
    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<ToggleSwitch> HToggleSwitch(HToggleSwitchProp prop)
    {
        var uiScope = new UiScope();
        var toggle = new ToggleSwitch();
        var border = new Border
        {
            Child = toggle
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, toggle, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(toggle, border, StyleUtil.ApplyStyle);

        // 启用状态
        if (prop.IsEnabled is not null)
            uiScope.CreateEffect(() => toggle.IsEnabled = prop.IsEnabled.RxValue);

        // 双向绑定 BindIsChecked
        if (prop.BindIsChecked is not null)
        {
            uiScope.CreateEffect(epochScope => toggle.IsChecked = epochScope.Pull(prop.BindIsChecked));
            toggle.IsCheckedChanged += (_, _) =>
            {
                var newValue = toggle.IsChecked == true;
                if (prop.BindIsChecked.RxValue != newValue)
                    prop.BindIsChecked.RxValue = newValue;
                prop.OnCheckedChanged?.Invoke(newValue);
            };
        }
        else if (prop.IsChecked is not null)
        {
            toggle.IsChecked = prop.IsChecked.Value;
            if(prop.IsChecked.IsReactive)
                uiScope.CreateEffect(epochScope => toggle.IsChecked = epochScope.Track(prop.IsChecked));
            
            toggle.Click += (_, _) => prop.OnCheckedChanged?.Invoke(toggle.IsChecked);
        }
        else if (prop.OnCheckedChanged is not null)
        {
            toggle.IsCheckedChanged += (_, _) => prop.OnCheckedChanged?.Invoke(toggle.IsChecked == true);
        }

        // 设置内容（通常是 TextBlock 或 StackPanel）
        var firstChild = prop.FirstOrDefault();
        if (firstChild is not null)
            toggle.Content = firstChild.Content;

        return new Element<ToggleSwitch>(uiScope, border, toggle);
    }
}