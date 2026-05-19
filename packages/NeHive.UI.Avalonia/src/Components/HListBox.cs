using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ListBox 配置类（泛型，T 为项数据类型）
/// </summary>
/// <typeparam name="T">列表项数据类型</typeparam>
public class HListBoxProp<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    MutSignal<T?>? bindBindSelectedItem = null,
    Accessor<SelectionMode>? selectionMode = null,
    Accessor<string>? strStyle = null,
    Accessor<HPanelStyle>? style = null
)
{
    // 样式（合并 strStyle 和 style）
    public readonly Accessor<HPanelStyle>? Style = style switch
    {
        not null when strStyle is not null => new Computed<HPanelStyle>(() =>
            HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
        not null => style,
        _ when strStyle is not null => HPanelStyle.Parse(strStyle),
        _ => null
    };

    // 数据源（支持响应式）
    public readonly Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;

    // 选中项（可选双向绑定）
    public readonly MutSignal<T?>? BindSelectedItem = bindBindSelectedItem;

    // 选择模式
    public readonly Accessor<SelectionMode>? SelectionMode = selectionMode;

    // 项模板：给定数据项，返回 IElement
    public required Func<T, IElement> ItemTemplate { get; init; }
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建基于数据绑定的 ListBox
    /// </summary>
    public static IElement<ListBox> HListBox<T>(HListBoxProp<T> prop)
    {
        var uiScope = new UiScope();
        var listBox = new ListBox();

        // ===== 应用样式 =====
        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                ApplyListBoxStyle(listBox, style);
            });
        }

        // ===== 设置选择模式 =====
        if (prop.SelectionMode != null)
            uiScope.CreateEffect(() => listBox.SelectionMode = prop.SelectionMode.RxValue);

        // ===== 数据绑定与项模板 =====

        // 1. 绑定数据源
        uiScope.CreateEffect(() => listBox.ItemsSource = prop.ItemsSource.RxValue);

        // 2. 设置项模板（使用 FuncDataTemplate）
        listBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            // item 是数据项，调用用户提供的 ItemTemplate 生成 UI 元素
            var element = prop.ItemTemplate(item);
            // element 包装了 Control，需要提取其 Root
            var control = element.Content;
            return control;
        }, supportsRecycling: true);

        // ===== 双向绑定 BindSelectedItem =====
        if (prop.BindSelectedItem == null) return new Element<ListBox>(uiScope, listBox, listBox);

        // 信号 -> 控件
        uiScope.CreateEffect(() =>
        {
            var selected = prop.BindSelectedItem.RxValue;
            // if (selected != null && listBox.BindSelectedItem != selected)
            //     listBox.BindSelectedItem = selected;
            if (selected != null && !Equals(selected, listBox.SelectedItem))
                listBox.SelectedItem = selected;
        });

        // 控件 -> 信号
        listBox.SelectionChanged += (_, _) =>
        {
            var newSelected = listBox.SelectedItem is T val ? val : default;
            if (!Equals(newSelected, prop.BindSelectedItem.RxValue))
                prop.BindSelectedItem.RxValue = newSelected;
        };

        return new Element<ListBox>(uiScope, listBox, listBox);
    }

    private static void ApplyListBoxStyle(ListBox listBox, HPanelStyle style)
    {
        listBox.Margin = style.Margin;
        listBox.HorizontalAlignment = style.HorizontalAlignment;
        listBox.VerticalAlignment = style.VerticalAlignment;
        listBox.Background = style.Background;

        // ListBox 继承自 ItemsControl，可以使用 ScrollViewer 样式等，这里只设置基础
        // 若需额外属性（如 ItemSpacing）可扩展 HPanelStyle
    }
}