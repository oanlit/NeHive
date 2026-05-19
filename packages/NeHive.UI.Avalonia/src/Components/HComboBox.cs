using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ComboBox 配置类（泛型，T 为项数据类型）
/// </summary>
public class HComboBoxProp<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    MutSignal<T?>? bindSelectedItem = null,
    Accessor<bool>? isEditable = null,
    Accessor<string>? placeholderText = null,
    Accessor<double>? maxDropDownHeight = null,
    Accessor<string>? strStyle = null,
    Accessor<HPanelStyle>? style = null
)
{
    // 布局样式
    public readonly Accessor<string>? StrStyle = strStyle;
    public readonly Accessor<HPanelStyle>? Style = style;

    // 数据源
    public Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;

    // 项模板
    public required Func<T, IElement> ItemTemplate { get; init; }

    // 选中项（双向绑定）
    public readonly MutSignal<T?>? BindSelectedItem = bindSelectedItem;

    // 是否可编辑
    public readonly Accessor<bool>? IsEditable = isEditable;

    // 占位文本（当没有选中时显示）
    public readonly Accessor<string>? PlaceholderText = placeholderText;

    // 最大下拉高度
    public readonly Accessor<double>? MaxDropDownHeight = maxDropDownHeight;

    // 内部计算合并后的样式
    public Accessor<HPanelStyle>? ComputedStyle
    {
        get
        {
            if (Style != null && StrStyle != null)
                return new Computed<HPanelStyle>(() =>
                    HPanelStyle.Parse(StrStyle).RxValue.Merge(Style.RxValue));
            if (StrStyle != null)
                return HPanelStyle.Parse(StrStyle);
            return Style;
        }
    }
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 ComboBox 组件
    /// </summary>
    public static IElement<ComboBox> HComboBox<T>(HComboBoxProp<T> prop)
    {
        var uiScope = new UiScope();
        var comboBox = new ComboBox();

        // 应用样式
        if (prop.ComputedStyle != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.ComputedStyle);
                ApplyComboBoxStyle(comboBox, style);
            });
        }

        // 绑定数据源
        uiScope.CreateEffect(() => comboBox.ItemsSource = prop.ItemsSource.RxValue);

        // 绑定项模板
        comboBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            if(item is null) return null;
            var element = prop.ItemTemplate(item);
            return element.Content;
        }, supportsRecycling: true);
        
        // 绑定是否可编辑
        if (prop.IsEditable != null)
            uiScope.CreateEffect(() => comboBox.IsEditable = prop.IsEditable.RxValue);

        // 绑定占位文本
        if (prop.PlaceholderText != null)
            uiScope.CreateEffect(() => comboBox.PlaceholderText = prop.PlaceholderText.RxValue);

        // 绑定最大下拉高度
        if (prop.MaxDropDownHeight != null)
            uiScope.CreateEffect(() => comboBox.MaxDropDownHeight = prop.MaxDropDownHeight.RxValue);

        // 双向绑定 BindSelectedItem
        if (prop.BindSelectedItem != null)
        {
            // 信号 -> 控件
            uiScope.CreateEffect(() =>
            {
                var selected = prop.BindSelectedItem.RxValue;
                // if (selected != null)
                //     comboBox.SelectedItem = selected;
                // else if (selected == null)
                //     comboBox.SelectedItem = null;
                comboBox.SelectedItem = selected;
            });

            // 控件 -> 信号
            comboBox.SelectionChanged += (_, _) =>
            {
                var newSelected = comboBox.SelectedItem is T val ? val : default;
                // if (!Equals(newSelected, prop.BindSelectedItem.RxValue))
                //     prop.BindSelectedItem.RxValue = newSelected;
                prop.BindSelectedItem.RxValue = newSelected;
            };
        }

        return new Element<ComboBox>(uiScope, comboBox, comboBox);
    }

    private static void ApplyComboBoxStyle(ComboBox comboBox, HPanelStyle style)
    {
        comboBox.Margin = style.Margin;
        if (style.ZIndex is not null) comboBox.ZIndex = style.ZIndex.Value;

        if (style.Width is not null) comboBox.Width = style.Width.Value;
        if (style.Height is not null) comboBox.Height = style.Height.Value;
        if (style.MinWidth is not null) comboBox.MinWidth = style.MinWidth.Value;
        if (style.MaxWidth is not null) comboBox.MaxWidth = style.MaxWidth.Value;
        if (style.MinHeight is not null) comboBox.MinHeight = style.MinHeight.Value;
        if (style.MaxHeight is not null) comboBox.MaxHeight = style.MaxHeight.Value;
        
        comboBox.HorizontalAlignment = style.HorizontalAlignment;
        comboBox.VerticalAlignment = style.VerticalAlignment;
        
        comboBox.Background = style.Background;
        // 更多样式可根据需要扩展
    }
}