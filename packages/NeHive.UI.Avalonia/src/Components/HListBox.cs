using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// ListBox 配置类（泛型，T 为项数据类型）
/// </summary>
/// <typeparam name="T">列表项数据类型</typeparam>
public class HListBoxProp<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    Accessor<T?>? selectedItem = null,
    MutSignal<T?>? bindSelectedItem = null,
    Accessor<SelectionMode>? selectionMode = null,
    Accessor<string>? strStyle = null,
    Accessor<StyleSet>? style = null,
    Dictionary<string, StyleSet>? variants = null
)
{
    // 样式（合并 strStyle 和 style）
    public readonly Accessor<FullStyle> Style = StyleParser.ParseFull(strStyle, null, style);
    public readonly Dictionary<string, StyleSet>? Variants = variants;

    public readonly Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;

    public readonly Accessor<T?>? SelectedItem = selectedItem;
    
    public readonly MutSignal<T?>? BindSelectedItem = bindSelectedItem;

    public readonly Accessor<SelectionMode>? SelectionMode = selectionMode;

    public required Func<T, IElement> ItemTemplate { get; init; }
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

        var state = new CommonState(uiScope, prop.Style.Value.Normal)
        {
            StrVariants = prop.Style.Value.Variants,
            Variants = prop.Variants
        };

        state.ApplyAccessorStyle(prop.Style, listBox, border, StyleUtil.ApplyStyle);
        state.ApplyVariantsStyle(listBox, border, StyleUtil.ApplyStyle);

        // ===== 设置选择模式 =====
        if (prop.SelectionMode is not null)
        {
            listBox.SelectionMode = prop.SelectionMode.Value;
            if (prop.SelectionMode.IsReactive)
                uiScope.CreateEffect(epochScope => listBox.SelectionMode = epochScope.Track(prop.SelectionMode));
        }

        listBox.ItemsSource = prop.ItemsSource.Value;
        if (prop.ItemsSource.IsReactive)
            uiScope.CreateEffect(epochScope => listBox.ItemsSource = epochScope.Track(prop.ItemsSource));

        listBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
        {
            var element = prop.ItemTemplate(item);
            var control = element.Content;
            return control;
        }, supportsRecycling: true);

        if (prop.BindSelectedItem is not null)
        {
            // 信号 -> 控件
            uiScope.CreateEffect(epochScope =>
            {
                var selected = epochScope.Pull(prop.BindSelectedItem);
                if (selected is not null && !Equals(selected, listBox.SelectedItem))
                    listBox.SelectedItem = selected;
            });

            // 控件 -> 信号
            listBox.SelectionChanged += (_, _) =>
            {
                var newSelected = listBox.SelectedItem is T val ? val : default;
                if (!Equals(newSelected, prop.BindSelectedItem.RxValue))
                    prop.BindSelectedItem.RxValue = newSelected;
            };
        }
        else if(prop.SelectedItem is not null)
        {
            var selected = prop.SelectedItem.Value;
            if (selected is not null && !Equals(selected, listBox.SelectedItem))
                listBox.SelectedItem = selected;
            if (prop.SelectedItem.IsReactive)
            {
                uiScope.CreateEffect(epochScope =>
                {
                    var selected2 = epochScope.Track(prop.SelectedItem);
                    if (selected2 is not null && !Equals(selected2, listBox.SelectedItem))
                        listBox.SelectedItem = selected2;
                });
            }
        }
        return new Element<ListBox>(uiScope, border, listBox);
    }
}