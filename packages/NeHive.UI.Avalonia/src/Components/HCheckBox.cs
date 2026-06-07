using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// CheckBox 配置类（构造参数仅样式/行为属性）
/// </summary>
public class HCheckBoxProp : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    // 合并后的样式
    public readonly Accessor<FullStyle>? Style;

    public HCheckBoxProp(
        Accessor<bool?>? isChecked = null,
        MutSignal<bool?>? bindIsChecked = null,
        Accessor<bool>? isEnabled = null,
        Accessor<bool>? isThreeState = null,
        Accessor<string>? strStyle = null,
        Action<bool?>? onClick = null
    )
    {
        BindIsChecked = bindIsChecked;
        IsChecked = isChecked;
        IsEnabled = isEnabled;
        IsThreeState = isThreeState;
        Click = onClick;

        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

    // 双向绑定的选中状态（支持三态 bool?）
    public readonly MutSignal<bool?>? BindIsChecked;
    public readonly Accessor<bool?>? IsChecked;

    // 是否启用
    public readonly Accessor<bool>? IsEnabled;

    // 是否支持三态（Indeterminate）
    public readonly Accessor<bool>? IsThreeState;

    // 点击回调（可选，代替命令）
    public readonly Action<bool?>? Click;

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
        if (prop.Style is not null)
        {
            var style = prop.Style.Value;
            StyleUtil.ApplyStyle(style.Normal, checkBox, border);
            if (prop.Style.IsReactive)
            {
                uiScope.CreateEffect(scope =>
                {
                    var style2 = scope.Track(prop.Style);
                    StyleUtil.ApplyStyle(style2.Normal, checkBox, border);
                });
            }
        }

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