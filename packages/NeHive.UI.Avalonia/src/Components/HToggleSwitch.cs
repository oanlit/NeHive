using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public class HToggleSwitchProp(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    Accessor<HPanelStyle>? style = null,
    Action<bool?>? onCheckedChanged = null
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
    public readonly Accessor<bool>? IsEnabled = isEnabled;
    public readonly Action<bool?>? OnCheckedChanged = onCheckedChanged;

    // 添加子内容（显示在开关旁边的标签或控件）
    public void Add(IElement element) => _children.Add(element);
    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 ToggleSwitch 组件
    /// </summary>
    public static IElement<ToggleSwitch> HToggleSwitch(HToggleSwitchProp prop)
    {
        var uiScope = new UiScope();
        var toggle = new ToggleSwitch();

        // 应用样式
        if (prop.ComputedStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.ComputedStyle);
                ApplyPanelStyle(style);
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

        return new Element<ToggleSwitch>(uiScope, toggle, toggle);

        void ApplyPanelStyle(HPanelStyle style)
        {
            toggle.Margin = style.Margin;
            if (style.ZIndex is not null) toggle.ZIndex = style.ZIndex.Value;
            
            if (style.Width is not null) toggle.Width = style.Width.Value;
            if (style.Height is not null) toggle.Height = style.Height.Value;
            if (style.MinWidth is not null) toggle.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) toggle.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) toggle.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) toggle.MaxHeight = style.MaxHeight.Value;
            
            toggle.HorizontalAlignment = style.HorizontalAlignment;
            toggle.VerticalAlignment = style.VerticalAlignment;
            
            toggle.Background = style.Background;
        }
    }
}