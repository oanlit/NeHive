using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ComboBox 配置类（泛型，T 为项数据类型）
/// </summary>
public class HComboBoxProp<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    Accessor<T?>? selectedItem = null,
    MutSignal<T?>? bindSelectedItem = null,
    Accessor<bool>? isEditable = null,
    Accessor<string>? placeholderText = null,
    Accessor<double>? maxDropDownHeight = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
)
{
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    // 数据源
    public readonly Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;

    // 项模板
    public required Func<T, IElement> ItemTemplate { get; init; }

    // 选中项
    public readonly Accessor<T?>? SelectedItem = selectedItem;

    // 选中项（双向绑定）
    public readonly MutSignal<T?>? BindSelectedItem = bindSelectedItem;

    // 是否可编辑
    public readonly Accessor<bool>? IsEditable = isEditable;

    // 占位文本（当没有选中时显示）
    public readonly Accessor<string>? PlaceholderText = placeholderText;

    // 最大下拉高度
    public readonly Accessor<double>? MaxDropDownHeight = maxDropDownHeight;
}

public static partial class BaseComponent
{
    public static IElement<ComboBox> HComboBox<T>(HComboBoxProp<T> prop)
    {
        var uiScope = new UiScope();
        var comboBox = new ComboBox();
        var border = new Border
        {
            Child = comboBox
        };

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, comboBox, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(comboBox, border, StyleUtil.ApplyStyle);

        // 绑定数据源
        comboBox.ItemsSource = prop.ItemsSource.Value;
        if (prop.ItemsSource.IsReactive)
            uiScope.CreateEffect(epochScope => comboBox.ItemsSource = epochScope.Track(prop.ItemsSource));

        // 绑定项模板
        comboBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            if (item is null) return null;
            var element = prop.ItemTemplate(item);
            return element.Content;
        }, supportsRecycling: true);

        // 绑定是否可编辑
        if (prop.IsEditable is not null)
        {
            comboBox.IsEditable = prop.IsEditable.Value;
            if (prop.IsEditable.IsReactive)
                uiScope.CreateEffect(epochScope => comboBox.IsEditable = epochScope.Track(prop.IsEditable));
        }

        // 绑定占位文本
        if (prop.PlaceholderText is not null)
        {
            comboBox.PlaceholderText = prop.PlaceholderText.Value;
            if (prop.PlaceholderText.IsReactive)
                uiScope.CreateEffect(epochScope => comboBox.PlaceholderText = epochScope.Track(prop.PlaceholderText));
        }

        // 绑定最大下拉高度
        if (prop.MaxDropDownHeight is not null)
        {
            comboBox.MaxDropDownHeight = prop.MaxDropDownHeight.Value;
            if (prop.MaxDropDownHeight.IsReactive)
                uiScope.CreateEffect(epochScope =>
                    comboBox.MaxDropDownHeight = epochScope.Track(prop.MaxDropDownHeight));
        }

        // 双向绑定 BindSelectedItem
        if (prop.BindSelectedItem is not null)
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
        else if (prop.SelectedItem is not null)
        {
            comboBox.SelectedItem = prop.SelectedItem.Value;
            if (prop.SelectedItem.IsReactive)
                uiScope.CreateEffect(epochScope => comboBox.SelectedItem = epochScope.Track(prop.SelectedItem));
        }

        return new Element<ComboBox>(uiScope, border, comboBox);
    }
}