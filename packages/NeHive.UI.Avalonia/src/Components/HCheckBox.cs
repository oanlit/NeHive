using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

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
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null,
    Action<bool?>? onClick = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    // 合并后的样式
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

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
    public static IElement<CheckBox> HCheckBox(HCheckBoxProp prop)
    {
        var uiScope = new UiScope();
        var checkBox = new CheckBox();
        var border = new Border
        {
            Child = checkBox
        };

        // 应用样式
        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };
        StyleUtil.ApplyStyle(state.CurrentStyle, checkBox, border);
        if (prop.Style.IsReactive)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(prop.Style);
                state.BaseStyle = styleValue.Normal;
                state.StrVariants = styleValue.Variants;
                state.CurrentStyle = state.BaseStyle.Copy();
                StyleUtil.ApplyStyle(state.CurrentStyle, checkBox, border);
            });
        }
        
        state.ApplyAccessorStyle(prop.Style, checkBox, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(checkBox, border, StyleUtil.ApplyStyle);

        // 绑定启用状态
        if (prop.IsEnabled != null)
        {
            checkBox.IsEnabled = prop.IsEnabled.Value;
            if (prop.IsEnabled.IsReactive)
                uiScope.CreateEffect(() => checkBox.IsEnabled = prop.IsEnabled.RxValue);
        }

        // 绑定三态支持
        if (prop.IsThreeState != null)
        {
            checkBox.IsThreeState = prop.IsThreeState.Value;
            if (prop.IsThreeState.IsReactive)
                uiScope.CreateEffect(() => checkBox.IsThreeState = prop.IsThreeState.RxValue);
        }

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
            checkBox.IsChecked = prop.IsChecked.Value;
            checkBox.Click += (_, _) => prop.Click?.Invoke(prop.IsChecked.Value);
            if (prop.IsChecked.IsReactive)
                uiScope.CreateEffect(() => checkBox.IsChecked = prop.IsChecked.RxValue);
        }
        else if (prop.Click is not null)
        {
            checkBox.Click += (_, _) => { prop.Click(false); };
        }

        // 设置子内容（通常只有一个子元素作为 Content）
        var firstChild = prop.FirstOrDefault();
        if (firstChild != null)
            checkBox.Content = firstChild.Content;

        return new Element<CheckBox>(uiScope, border, checkBox);
    }
}