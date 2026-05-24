using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HToggleSwitchProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle>? Style;

    public readonly MutSignal<bool?>? BindIsChecked;
    public readonly Accessor<bool?>? IsChecked;
    public readonly Accessor<bool>? IsEnabled;
    public readonly Action<bool?>? OnCheckedChanged;

    public HToggleSwitchProp(
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isEnabled = null,
        Accessor<string>? strStyle = null,
        Action<bool?>? onCheckedChanged = null
    )
    {
        BindIsChecked = bindIsChecked;
        IsChecked = isChecked;
        IsEnabled = isEnabled;
        OnCheckedChanged = onCheckedChanged;
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

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

        // 应用样式
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal, toggle, border);
            });
        }

        // 启用状态
        if (prop.IsEnabled != null)
            uiScope.CreateEffect(() => toggle.IsEnabled = prop.IsEnabled.RxValue);

        // 双向绑定 BindIsChecked
        if (prop.BindIsChecked != null)
        {
            uiScope.CreateEffect(() => toggle.IsChecked = prop.BindIsChecked.RxValue);
            toggle.IsCheckedChanged += (_, _) =>
            {
                var newValue = toggle.IsChecked == true;
                if (prop.BindIsChecked.RxValue != newValue)
                    prop.BindIsChecked.RxValue = newValue;
                prop.OnCheckedChanged?.Invoke(newValue);
            };
        }
        else if (prop.IsChecked != null)
        {
            uiScope.CreateEffect(() => toggle.IsChecked = prop.IsChecked.RxValue);
            toggle.Click += (_, _) => prop.OnCheckedChanged?.Invoke(toggle.IsChecked);
        }
        else if (prop.OnCheckedChanged != null)
        {
            toggle.IsCheckedChanged += (_, _) => prop.OnCheckedChanged?.Invoke(toggle.IsChecked == true);
        }

        // 设置内容（通常是 TextBlock 或 StackPanel）
        var firstChild = prop.FirstOrDefault();
        if (firstChild != null)
            toggle.Content = firstChild.Content;

        return new Element<ToggleSwitch>(uiScope, border, toggle);
    }
}