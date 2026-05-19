using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// CheckBox 配置类（构造参数仅样式/行为属性）
/// </summary>
public class HCheckBoxProp(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isEnabled = null,
    Accessor<bool>? isThreeState = null,
    Accessor<string>? strStyle = null,
    Accessor<HPanelStyle>? style = null,
    Action<bool?>? onClick = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    // 合并后的样式
    public readonly Accessor<HPanelStyle>? ComputedStyle = (style, strStyle) switch
    {
        (not null, not null) => new Computed<HPanelStyle>(() =>
            HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
        (not null, _) => style,
        (_, not null) => HPanelStyle.Parse(strStyle),
        _ => null
    };

    // 双向绑定的选中状态（支持三态 bool?）
    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = isChecked;

    // 是否启用
    public readonly Accessor<bool>? IsEnabled = isEnabled;

    // 是否支持三态（Indeterminate）
    public readonly Accessor<bool>? IsThreeState = isThreeState;

    // 点击回调（可选，代替命令）
    public readonly Action<bool?>? Click = onClick;

    // 添加子内容（复选框旁边的文本或任意控件）
    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 CheckBox 组件
    /// </summary>
    public static IElement<CheckBox> HCheckBox(HCheckBoxProp prop)
    {
        var uiScope = new UiScope();
        var checkBox = new CheckBox();

        // 应用样式
        if (prop.ComputedStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.ComputedStyle);
                ApplyStyle(style); // 复用 Panel 样式应用逻辑
            });
        }

        // 绑定启用状态
        if (prop.IsEnabled != null)
            uiScope.CreateEffect(() => checkBox.IsEnabled = prop.IsEnabled.RxValue);

        // 绑定三态支持
        if (prop.IsThreeState != null)
            uiScope.CreateEffect(() => checkBox.IsThreeState = prop.IsThreeState.RxValue);

        // 双向绑定 BindIsChecked + 点击回调
        // 信号 -> 控件
        if (prop.BindIsChecked is not null)
        {
            uiScope.CreateEffect(() => checkBox.IsChecked = prop.BindIsChecked.RxValue);
            // 控件 -> 信号
            checkBox.Click += (_, _) =>
            {
                var newValue = checkBox.IsChecked;
                if (prop.BindIsChecked.Value != newValue)
                    prop.BindIsChecked.RxValue = newValue;

                prop.Click?.Invoke(newValue);
            };
        }
        else if (prop.IsChecked is not null)
        {
            uiScope.CreateEffect(() => checkBox.IsChecked = prop.IsChecked.RxValue);
            checkBox.Click += (_, _) => { prop.Click?.Invoke(prop.IsChecked.Value); };
        }
        else if (prop.Click is not null)
        {
            checkBox.Click += (_, _) => { prop.Click(false); };
        }

        // 设置子内容（通常只有一个子元素作为 Content）
        var firstChild = prop.FirstOrDefault();
        if (firstChild != null)
            checkBox.Content = firstChild.Content;

        return new Element<CheckBox>(uiScope, checkBox, checkBox);

        void ApplyStyle(HPanelStyle style)
        {
            checkBox.Margin = style.Margin;
            if (style.ZIndex is not null) checkBox.ZIndex = style.ZIndex.Value;

            if (style.Width is not null) checkBox.Width = style.Width.Value;
            if (style.Height is not null) checkBox.Height = style.Height.Value;
            if (style.MinWidth is not null) checkBox.MinWidth = style.MinWidth.Value;
            if (style.MaxWidth is not null) checkBox.MaxWidth = style.MaxWidth.Value;
            if (style.MinHeight is not null) checkBox.MinHeight = style.MinHeight.Value;
            if (style.MaxHeight is not null) checkBox.MaxHeight = style.MaxHeight.Value;

            checkBox.HorizontalAlignment = style.HorizontalAlignment;
            checkBox.VerticalAlignment = style.VerticalAlignment;

            checkBox.Background = style.Background;
            // 可根据需要扩展更多属性
        }
    }
}