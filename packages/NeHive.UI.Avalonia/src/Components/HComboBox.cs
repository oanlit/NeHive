using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ComboBox 配置类（泛型，T 为项数据类型）
/// </summary>
public class HComboBoxProp<T>
{
    public readonly Accessor<FullStyle>? Style;

    // 数据源
    public readonly Accessor<IReadOnlyList<T>> ItemsSource;

    // 项模板
    public required Func<T, IElement> ItemTemplate { get; init; }

    // 选中项（双向绑定）
    public readonly MutSignal<T?>? BindSelectedItem;

    // 是否可编辑
    public readonly Accessor<bool>? IsEditable;

    // 占位文本（当没有选中时显示）
    public readonly Accessor<string>? PlaceholderText;

    // 最大下拉高度
    public readonly Accessor<double>? MaxDropDownHeight;

    public HComboBoxProp(
        Accessor<IReadOnlyList<T>> itemsSource,
        MutSignal<T?>? bindSelectedItem = null,
        Accessor<bool>? isEditable = null,
        Accessor<string>? placeholderText = null,
        Accessor<double>? maxDropDownHeight = null,
        Accessor<string>? strStyle = null
    )
    {
        // 数据源
        ItemsSource = itemsSource;

        // 选中项（双向绑定）
        BindSelectedItem = bindSelectedItem;

        // 是否可编辑
        IsEditable = isEditable;

        // 占位文本（当没有选中时显示）
        PlaceholderText = placeholderText;

        // 最大下拉高度
        MaxDropDownHeight = maxDropDownHeight;
        
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }
}

public static partial class BaseComponent
{
    public static IElement<ComboBox> HComboBox<T>(HComboBoxProp<T> prop)
    {
        var uiScope = new UiScope();
        var comboBox = new ComboBox();
        var border =  new Border
        {
            Child = comboBox
        };

        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal, comboBox,border);
            });
        }

        // 绑定数据源
        uiScope.CreateEffect(() => comboBox.ItemsSource = prop.ItemsSource.RxValue);

        // 绑定项模板
        comboBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            if (item is null) return null;
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
                comboBox.SelectedItem = selected;
            });

            // 控件 -> 信号
            comboBox.SelectionChanged += (_, _) =>
            {
                var newSelected = comboBox.SelectedItem is T val ? val : default;
                prop.BindSelectedItem.RxValue = newSelected;
            };
        }

        return new Element<ComboBox>(uiScope, border, comboBox);
    }
}