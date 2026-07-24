using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HComboBoxArgs<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    Accessor<T?>? selectedItem = null,
    MutSignal<T?>? bindSelectedItem = null,
    Accessor<bool>? isEditable = null,
    Accessor<string>? placeholderText = null,
    Accessor<double>? maxDropDownHeight = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null
)
{
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);
    public readonly Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;
    public required Func<T, IElement> ItemTemplate { get; init; }
    public readonly Accessor<T?>? SelectedItem = selectedItem;
    public readonly MutSignal<T?>? BindSelectedItem = bindSelectedItem;
    public readonly Accessor<bool>? IsEditable = isEditable;
    public readonly Accessor<string>? PlaceholderText = placeholderText;
    public readonly Accessor<double>? MaxDropDownHeight = maxDropDownHeight;
}

public static partial class BaseComponent
{
    public static IElement<ComboBox> HComboBox<T>(HComboBoxArgs<T> args)
    {
        return Element<ComboBox>.WithScope(uiScope =>
        {
            var comboBox = new ComboBox();
            var border = new Border
            {
                Child = comboBox
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, comboBox, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(comboBox, border, StyleUtil.ApplyStyle);

            comboBox.ItemsSource = args.ItemsSource.Value;
            if (args.ItemsSource.IsReactive)
                uiScope.CreateEffect(epochScope => comboBox.ItemsSource = epochScope.Track(args.ItemsSource));

            // 绑定项模板
            comboBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
            {
                if (item is null) return null;
                var element = args.ItemTemplate(item);
                return element.Content;
            }, supportsRecycling: true);

            // 绑定是否可编辑
            if (args.IsEditable is not null)
            {
                comboBox.IsEditable = args.IsEditable.Value;
                if (args.IsEditable.IsReactive)
                    uiScope.CreateEffect(epochScope => comboBox.IsEditable = epochScope.Track(args.IsEditable));
            }

            // 绑定占位文本
            if (args.PlaceholderText is not null)
            {
                comboBox.PlaceholderText = args.PlaceholderText.Value;
                if (args.PlaceholderText.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        comboBox.PlaceholderText = epochScope.Track(args.PlaceholderText));
            }

            // 绑定最大下拉高度
            if (args.MaxDropDownHeight is not null)
            {
                comboBox.MaxDropDownHeight = args.MaxDropDownHeight.Value;
                if (args.MaxDropDownHeight.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        comboBox.MaxDropDownHeight = epochScope.Track(args.MaxDropDownHeight));
            }

            // 双向绑定 BindSelectedItem
            if (args.BindSelectedItem is not null)
            {
                // 信号 -> 控件
                uiScope.CreateEffect(() =>
                {
                    var selected = args.BindSelectedItem.RxValue;
                    comboBox.SelectedItem = selected;
                });

                // 控件 -> 信号
                comboBox.SelectionChanged += (_, _) =>
                {
                    var newSelected = comboBox.SelectedItem is T val ? val : default;
                    args.BindSelectedItem.RxValue = newSelected;
                };
            }
            else if (args.SelectedItem is not null)
            {
                comboBox.SelectedItem = args.SelectedItem.Value;
                if (args.SelectedItem.IsReactive)
                    uiScope.CreateEffect(epochScope => comboBox.SelectedItem = epochScope.Track(args.SelectedItem));
            }

            return (comboBox, border);
        });
    }
}