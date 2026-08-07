using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Objects;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HComboBoxPart
{
    internal IElement<Popup>? PopupElement { get; private set; }
    internal IElement<TextBox>? EditableTextBoxElement { get; private set; }
    internal IElement<ItemsPresenter>? ItemsPresenterElement { get; private set; }

    public IElement<Popup> Popup(IElement<Popup> element) => PopupElement = element;
    public IElement<TextBox> EditableTextBox(IElement<TextBox> element) => EditableTextBoxElement = element;
    public IElement<ItemsPresenter> ItemsPresenter(IElement<ItemsPresenter> element) => ItemsPresenterElement = element;
}

public class HComboBoxProps<T>(Scope scope, Border border, ComboBox comboBox)
    : BaseComponentProps(scope, border, comboBox)
{
    public MutSignal<bool> IsDropDownOpen
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<bool>(comboBox.IsDropDownOpen);
            BridgeAvalonia.BindPropertySignal(scope, field, comboBox, ComboBox.IsDropDownOpenProperty);
            return field;
        }
    }

    public Signal<bool> IsEditable
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.IsEditableProperty, comboBox.IsEditable);
            return field;
        }
    }

    public Signal<double> MaxDropDownHeight
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.MaxDropDownHeightProperty, comboBox.MaxDropDownHeight);
            return field;
        }
    }

    public Signal<T?> SelectionBoxItem
    {
        get
        {
            if (field is not null) return field;
            var signal = new MutSignal<T?>((T?)comboBox.SelectionBoxItem);
            comboBox.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => comboBox.PropertyChanged -= OnPropUpdate;
            field = signal;
            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == ComboBox.SelectionBoxItemProperty)
                    signal.RxValue = (T?)args.NewValue;
            }
        }
    }

    public Signal<string?> PlaceholderText
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.PlaceholderTextProperty, comboBox.PlaceholderText);
            return field;
        }
    }

    public Signal<IBrush?> PlaceholderForeground
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.PlaceholderForegroundProperty, comboBox.PlaceholderForeground);
            return field;
        }
    }

    public MutSignal<string?> Text
    {
        get
        {
            if (field is not null) return field;
            field = new MutSignal<string?>(comboBox.Text);
            BridgeAvalonia.BindPropertySignal(scope, field, comboBox, ComboBox.TextProperty);
            return field;
        }
    }

    public Signal<HorizontalAlignment> HorizontalContentAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.HorizontalContentAlignmentProperty, comboBox.HorizontalContentAlignment);
            return field;
        }
    }

    public Signal<VerticalAlignment> VerticalContentAlignment
    {
        get
        {
            if (field is not null) return field;
            field = BridgeAvalonia.CreatePropertySignal(scope, comboBox,
                ComboBox.VerticalContentAlignmentProperty, comboBox.VerticalContentAlignment);
            return field;
        }
    }
}

public class HComboBoxArgs<T>(
    Accessor<IReadOnlyList<T>> itemsSource,
    Accessor<T?>? selectedItem = null,
    MutSignal<T?>? bindSelectedItem = null,
    Accessor<bool>? isEditable = null,
    Accessor<string>? placeholderText = null,
    Accessor<double>? maxDropDownHeight = null,
    Accessor<HorizontalAlignment>? horizontalContentAlignment = null,
    Accessor<VerticalAlignment>? verticalContentAlignment = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    BaseComponentInteraction? baseInteraction = null
) : BaseComponentArgs(strStyle, style, baseInteraction)
{
    public readonly Accessor<T?>? SelectedItem = selectedItem;
    public readonly MutSignal<T?>? BindSelectedItem = bindSelectedItem;
    public readonly Accessor<bool>? IsEditable = isEditable;
    public readonly Accessor<string>? PlaceholderText = placeholderText;
    public readonly Accessor<double>? MaxDropDownHeight = maxDropDownHeight;
    public readonly Accessor<HorizontalAlignment>? HorizontalContentAlignment = horizontalContentAlignment;
    public readonly Accessor<VerticalAlignment>? VerticalContentAlignment = verticalContentAlignment;

    public readonly Accessor<IReadOnlyList<T>> ItemsSource = itemsSource;
    public required Func<T, IElement> ItemTemplate { get; init; }

    public Func<Signal<T?>, IElement> SelectionBoxItemTemplate { get; init; } =
        data => BaseComponent.HText(data.RxValue?.ToString() ?? "");

    public IElement<Panel>? ItemsPanel { get; init; }
    public Func<HComboBoxPart, IElement>? Template { get; init; }
}

public static partial class BaseComponent
{
    public static IElement<ComboBox> HComboBox<T>(Func<HComboBoxProps<T>, HComboBoxArgs<T>> fn)
    {
        return Element<ComboBox>.WithScope(uiScope =>
        {
            var comboBox = new ComboBox()
            {
                Styles =
                {
                    new Style(s => s.OfType<ComboBoxItem>())
                    {
                        Setters =
                        {
                            new Setter(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                            new Setter(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch),
                            new Setter(
                                TemplatedControl.TemplateProperty,
                                new FuncControlTemplate<ComboBoxItem>((c, s) =>
                                    {
                                        var presenter = new ContentPresenter
                                        {
                                            Name = "PART_ContentPresenter",
                                            [!!ContentPresenter.PaddingProperty] =
                                                c.GetObservable(ContentPresenter.PaddingProperty).ToBinding(),
                                            [!!ContentPresenter.HorizontalContentAlignmentProperty] =
                                                c.GetObservable(ContentPresenter.HorizontalContentAlignmentProperty)
                                                    .ToBinding(),
                                            // VerticalContentAlignment = VerticalAlignment.Stretch,
                                            [!!ContentPresenter.VerticalContentAlignmentProperty] =
                                                c.GetObservable(ContentPresenter.VerticalContentAlignmentProperty)
                                                    .ToBinding(),
                                            [!!ContentPresenter.BackgroundProperty] =
                                                c.GetObservable(ContentPresenter.BackgroundProperty).ToBinding(),
                                            [!!ContentPresenter.BorderBrushProperty] =
                                                c.GetObservable(ContentPresenter.BorderBrushProperty).ToBinding(),
                                            [!!ContentPresenter.BorderThicknessProperty] =
                                                c.GetObservable(ContentPresenter.BorderThicknessProperty).ToBinding(),
                                            [!!ContentPresenter.ContentProperty] =
                                                c.GetObservable(ContentPresenter.ContentProperty).ToBinding(),
                                            [!!ContentPresenter.ContentTemplateProperty] =
                                                c.GetObservable(ContentPresenter.ContentTemplateProperty).ToBinding(),
                                            [!!ContentPresenter.CornerRadiusProperty] =
                                                c.GetObservable(ContentPresenter.CornerRadiusProperty).ToBinding(),
                                        };
                                        s.Register("PART_ContentPresenter", presenter);
                                        return presenter;
                                    }
                                )
                            )
                        }
                    },
                }
            };
            var border = new Border();

            var props = new HComboBoxProps<T>(uiScope, border, comboBox);
            var args = fn(props);

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, comboBox, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(comboBox, border, StyleUtil.ApplyStyle);

            comboBox.ItemsSource = args.ItemsSource.Value;
            if (args.ItemsSource.IsReactive)
                uiScope.CreateEffect(epoch => comboBox.ItemsSource = epoch.Track(args.ItemsSource));

            comboBox.ItemTemplate = new FuncDataTemplate<T>((item, _) =>
            {
                if (item is null) return null;
                var element = args.ItemTemplate(item);
                return element.Content;
            }, supportsRecycling: true);

            var template = args.Template;

            if (template is null)
            {
                template = part =>
                    HGrid(_ => new(
                        columnDefinitions: new([HgLen.Star(), HgLen.Auto]),
                        style: new(
                            horizontalAlignment: HorizontalAlignment.Stretch,
                            verticalAlignment: VerticalAlignment.Stretch
                        )
                    )
                    {
                        [column: 0] = ControlFlow.Show(new(props.IsEditable)
                        {
                            IfTrue = () => part.EditableTextBox(HTextBox(bindText: props.Text,
                                placeholderText: props.PlaceholderText,
                                style: new(
                                    horizontalAlignment: props.HorizontalContentAlignment,
                                    verticalAlignment: props.VerticalContentAlignment,
                                    foreground: props.Style.Foreground,
                                    background: new(Brushes.Transparent),
                                    borderThickness: new Thickness(0)
                                )
                            )),
                            IfFalse = () => HBorder(_ => new(style: new(
                                horizontalAlignment: props.HorizontalContentAlignment,
                                verticalAlignment: props.VerticalContentAlignment
                            ))
                            {
                                args.SelectionBoxItemTemplate(props.SelectionBoxItem)
                            }),
                        }), // HGrid.[column: 0]
                        [column: 1] = HToggleButton(_ => new(
                            bindIsChecked: props.IsDropDownOpen.AsNullable(),
                            clickMode: ClickMode.Press,
                            style: new(
                                background: new(Brushes.Transparent),
                                borderThickness: new Thickness(0)),
                            baseInteraction: new(isFocusable: false))
                        {
                            HSvgImage(
                                path: "F1 M 301.14,-189.041L 311.57,-189.041L 306.355,-182.942L 301.14,-189.041 Z",
                                stretch: Stretch.Uniform,
                                style: new(
                                    width: 8,
                                    height: 4,
                                    horizontalAlignment: HorizontalAlignment.Center,
                                    verticalAlignment: VerticalAlignment.Center
                                )
                            ) // HSvgImage
                        }), // HGrid.[column: 1]
                        [null] = part.Popup(HPopup(_ => new(
                            bindIsOpen: props.IsDropDownOpen,
                            isLightDismissEnabled: true,
                            isInheritsTransform: true,
                            style: new(
                                minWidth: new(() => props.Bounds.RxValue.Width),
                                maxHeight: props.MaxDropDownHeight
                            )
                        )
                        {
                            HScrollViewer(_ => new(
                                horizontalScrollBarVisibility: props.AttachProperty(
                                    ScrollViewer.HorizontalScrollBarVisibilityProperty),
                                verticalScrollBarVisibility: props.AttachProperty(
                                    ScrollViewer.VerticalScrollBarVisibilityProperty),
                                isDeferredScrollingEnabled: props.AttachProperty(
                                    ScrollViewer.IsDeferredScrollingEnabledProperty),
                                isScrollChainingEnabled: false,
                                style: new(
                                    horizontalAlignment: HorizontalAlignment.Stretch,
                                    verticalAlignment: VerticalAlignment.Stretch
                                    // background: new SolidColorBrush(Colors.Gray200)
                                )
                            )
                            {
                                part.ItemsPresenter(HItemsPresenter(new()
                                {
                                    ItemsPanel = args.ItemsPanel
                                })) // part.ItemsPresenter
                            }) // HScrollViewer
                        })) // part.Popup
                    });
            }

            var part = new HComboBoxPart();
            border.Child = template(part).Content;
            comboBox.Template = new FuncControlTemplate((_, s) =>
            {
                if (part.EditableTextBoxElement is not null)
                {
                    var __ = part.EditableTextBoxElement.Content;
                    var editableTextBox = part.EditableTextBoxElement.Expose!;
                    editableTextBox.Name = "PART_EditableTextBox";
                    s.Register("PART_EditableTextBox", editableTextBox);
                }

                if (part.PopupElement is not null)
                {
                    var __ = part.PopupElement.Content;
                    var popup = part.PopupElement.Expose!;
                    popup.Name = "PART_Popup";
                    s.Register("PART_Popup", popup);
                }

                if (part.ItemsPresenterElement is not null)
                {
                    var __ = part.ItemsPresenterElement.Content;
                    var itemsPresenterElement = part.ItemsPresenterElement.Expose!;
                    itemsPresenterElement.Name = "PART_ItemsPresenter";
                    s.Register("PART_ItemsPresenter", itemsPresenterElement);
                }

                return border;
            });

            if (args.IsEditable is not null)
            {
                comboBox.IsEditable = args.IsEditable.Value;
                if (args.IsEditable.IsReactive)
                    uiScope.CreateEffect(epoch => comboBox.IsEditable = epoch.Track(args.IsEditable));
            }

            if (args.PlaceholderText is not null)
            {
                comboBox.PlaceholderText = args.PlaceholderText.Value;
                if (args.PlaceholderText.IsReactive)
                    uiScope.CreateEffect(epochScope =>
                        comboBox.PlaceholderText = epochScope.Track(args.PlaceholderText));
            }

            if (args.MaxDropDownHeight is not null)
            {
                comboBox.MaxDropDownHeight = args.MaxDropDownHeight.Value;
                if (args.MaxDropDownHeight.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        comboBox.MaxDropDownHeight = epoch.Track(args.MaxDropDownHeight));
            }

            if (args.BindSelectedItem is not null)
            {
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
                    uiScope.CreateEffect(epoch => comboBox.SelectedItem = epoch.Track(args.SelectedItem));
            }

            if (args.HorizontalContentAlignment is not null)
            {
                comboBox.HorizontalContentAlignment = args.HorizontalContentAlignment.Value;
                if (args.HorizontalContentAlignment.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        comboBox.HorizontalContentAlignment = epoch.Track(args.HorizontalContentAlignment));
            }

            if (args.VerticalContentAlignment is not null)
            {
                comboBox.VerticalContentAlignment = args.VerticalContentAlignment.Value;
                if (args.VerticalContentAlignment.IsReactive)
                    uiScope.CreateEffect(epoch =>
                        comboBox.VerticalContentAlignment = epoch.Track(args.VerticalContentAlignment));
            }

            return (comboBox, comboBox);
        });
    }
}