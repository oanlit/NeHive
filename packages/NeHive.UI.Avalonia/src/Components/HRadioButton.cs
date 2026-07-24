using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HRadioButtonArgs(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<string>? groupName = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<bool?>? onClick = null) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked = isChecked;
    public readonly Accessor<string>? GroupName = groupName;
    public readonly Accessor<bool>? IsEnabled = isEnabled;
    public readonly Action<bool?>? OnClick = onClick;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public void Add(IElement element) => _children.Add(element);
    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<RadioButton> HRadioButton(HRadioButtonArgs args)
    {
        return Element<RadioButton>.WithScope(uiScope =>
        {
            var radio = new RadioButton();
            var border = new Border
            {
                Child = radio
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, radio, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(radio, border, StyleUtil.ApplyStyle);

            if (args.IsEnabled is not null)
            {
                radio.IsEnabled = args.IsEnabled.Value;
                if (args.IsEnabled.IsReactive)
                    uiScope.CreateEffect(epochScope => radio.IsEnabled = epochScope.Track(args.IsEnabled));
            }

            if (args.GroupName is not null)
            {
                radio.GroupName = args.GroupName.Value;
                if (args.GroupName.IsReactive)
                    uiScope.CreateEffect(epochScope => radio.GroupName = epochScope.Track(args.GroupName));
            }

            if (args.BindIsChecked is not null)
            {
                uiScope.CreateEffect(() => radio.IsChecked = args.BindIsChecked.RxValue);
                radio.Click += (_, _) =>
                {
                    args.BindIsChecked.NotifySet(prev => prev is not true);
                    args.OnClick?.Invoke(args.BindIsChecked.Value);
                };
            }
            else if (args.IsChecked is not null)
            {
                uiScope.CreateEffect(() => radio.IsChecked = args.IsChecked.RxValue);
                radio.Click += (_, _) => args.OnClick?.Invoke(radio.IsChecked);
            }
            else if (args.OnClick is not null)
            {
                radio.Click += (_, _) => args.OnClick?.Invoke(radio.IsChecked);
            }

            var firstChild = args.FirstOrDefault();
            if (firstChild is not null)
                radio.Content = firstChild.Content;

            return (radio, border);
        });
    }
}