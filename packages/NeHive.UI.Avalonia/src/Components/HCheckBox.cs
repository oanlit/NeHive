using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;
using NeHive.UI.Avalonia.Utils;

namespace NeHive.UI.Avalonia.Components;

public class HCheckBoxArgs(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isEnabled = null,
    Accessor<bool>? isThreeState = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<bool?>? onClick = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = isChecked;

    public readonly Accessor<bool>? IsEnabled = isEnabled;

    public readonly Accessor<bool>? IsThreeState = isThreeState;

    public readonly Action<bool?>? OnClick = onClick;

    public void Add(IElement element) => _children.Add(element);

    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<CheckBox> HCheckBox(HCheckBoxArgs args)
    {
        return Element<CheckBox>.WithScope(uiScope =>
        {
            var content = ElementUtil.WrapSingleContainerContent(args).Content;
            var checkBox = new CheckBox
            {
                Content = content
            };
            var border = new Border
            {
                Child = checkBox
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, checkBox, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(checkBox, border, StyleUtil.ApplyStyle);

            if (args.IsEnabled != null)
            {
                checkBox.IsEnabled = args.IsEnabled.Value;
                if (args.IsEnabled.IsReactive)
                    uiScope.CreateEffect(() => checkBox.IsEnabled = args.IsEnabled.RxValue);
            }

            if (args.IsThreeState != null)
            {
                checkBox.IsThreeState = args.IsThreeState.Value;
                if (args.IsThreeState.IsReactive)
                    uiScope.CreateEffect(() => checkBox.IsThreeState = args.IsThreeState.RxValue);
            }

            if (args.BindIsChecked is not null)
            {
                uiScope.CreateEffect(() => checkBox.IsChecked = args.BindIsChecked.RxValue);
                checkBox.Click += (_, _) =>
                {
                    var newValue = checkBox.IsChecked;
                    if (args.BindIsChecked.Value != newValue)
                        args.BindIsChecked.RxValue = newValue;

                    args.OnClick?.Invoke(newValue);
                };
            }
            else if (args.IsChecked is not null)
            {
                checkBox.IsChecked = args.IsChecked.Value;
                checkBox.Click += (_, _) => args.OnClick?.Invoke(args.IsChecked.Value);
                if (args.IsChecked.IsReactive)
                    uiScope.CreateEffect(() => checkBox.IsChecked = args.IsChecked.RxValue);
            }
            else if (args.OnClick is not null)
            {
                checkBox.Click += (_, _) => { args.OnClick(false); };
            }

            var firstChild = args.FirstOrDefault();
            if (firstChild != null)
                checkBox.Content = firstChild.Content;

            return (checkBox, border);
        });
    }
}