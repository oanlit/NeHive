using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ListBox 配置类（泛型，T 为项数据类型）
/// </summary>
/// <typeparam name="T">列表项数据类型</typeparam>
public class HListBoxProp<T>
{
    // 样式（合并 strStyle 和 style）
    public readonly Accessor<FullStyle>? Style;

    public readonly Accessor<IReadOnlyList<T>> ItemsSource;

    public readonly MutSignal<T?>? BindSelectedItem;

    public readonly Accessor<SelectionMode>? SelectionMode;

    public required Func<T, IElement> ItemTemplate { get; init; }

    public HListBoxProp(
        Accessor<IReadOnlyList<T>> itemsSource,
        MutSignal<T?>? bindBindSelectedItem = null,
        Accessor<SelectionMode>? selectionMode = null,
        Accessor<string>? strStyle = null,
        Accessor<HPanelStyle>? style = null
    )
    {
        ItemsSource = itemsSource;
        BindSelectedItem = bindBindSelectedItem;
        SelectionMode = selectionMode;

        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }
}

public static partial class BaseComponent
{
    public static IElement<ListBox> HListBox<T>(HListBoxProp<T> prop)
    {
        var uiScope = new UiScope();
        var listBox = new ListBox();
        var border = new Border
        {
            Child = listBox
        };

        if (prop.Style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                // ApplyStyle(listBox, style);
                StyleUtil.ApplyStyle(style.Normal, listBox, border);
            });
        }

        // ===== 设置选择模式 =====
        if (prop.SelectionMode != null)
            uiScope.CreateEffect(() => listBox.SelectionMode = prop.SelectionMode.RxValue);

        uiScope.CreateEffect(() => listBox.ItemsSource = prop.ItemsSource.RxValue);

        listBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            var element = prop.ItemTemplate(item);
            var control = element.Content;
            return control;
        }, supportsRecycling: true);

        if (prop.BindSelectedItem == null) return new Element<ListBox>(uiScope, listBox, listBox);

        // 信号 -> 控件
        uiScope.CreateEffect(() =>
        {
            var selected = prop.BindSelectedItem.RxValue;
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

        return new Element<ListBox>(uiScope, border, listBox);
    }
}