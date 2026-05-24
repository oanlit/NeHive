using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 ProgressBar 控件
    /// </summary>
    /// <param name="value">当前值（支持响应式）</param>
    /// <param name="minimum">最小值（默认0）</param>
    /// <param name="maximum">最大值（默认100）</param>
    /// <param name="isIndeterminate">是否为不确定进度（响应式）</param>
    /// <param name="strStyle">样式字符串</param>
    /// <param name="style">直接样式对象</param>
    public static IElement HProgressBar(
        Accessor<double>? value = null,
        Accessor<double>? minimum = null,
        Accessor<double>? maximum = null,
        Accessor<bool>? isIndeterminate = null,
        Accessor<string>? strStyle = null,
        Accessor<FullStyle>? style = null)
    {
        value ??= 0;
        minimum ??= 0;
        maximum ??= 100;
        isIndeterminate ??= false;

        // 样式合并
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        var uiScope = new UiScope();
        var progressBar = new ProgressBar();
        var border = new Border
        {
            Child = progressBar
        };

        // 绑定属性
        uiScope.CreateEffect(() => progressBar.Value = value.RxValue);
        uiScope.CreateEffect(() => progressBar.Minimum = minimum.RxValue);
        uiScope.CreateEffect(() => progressBar.Maximum = maximum.RxValue);
        uiScope.CreateEffect(() => progressBar.IsIndeterminate = isIndeterminate.RxValue);

        // 应用样式
        if (style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var styleValue = epochScope.Track(style);
                StyleUtil.ApplyStyle(styleValue.Normal, progressBar, border);
            });
        }

        return new Element(uiScope, border);
    }
}