using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HToggleSwitchArgs(
    Accessor<bool?>? isChecked = null,
    MutSignal<bool?>? bindIsChecked = null,
    Accessor<bool>? isEnabled = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null,
    Action<bool?>? onCheckedChanged = null
) : IEnumerable<IElement>
{
    private readonly List<IElement> _children = [];

    public readonly MutSignal<bool?>? BindIsChecked = bindIsChecked;
    public readonly Accessor<bool?>? IsChecked =  isChecked;
    public readonly Accessor<bool>? IsEnabled =  isEnabled;
    public readonly Action<bool?>? OnCheckedChanged =  onCheckedChanged;

    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);

    public void Add(IElement element) => _children.Add(element);
    public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<ToggleSwitch> HToggleSwitch(HToggleSwitchArgs args)
    {
        return Element<ToggleSwitch>.WithScope(uiScope =>
        {
            var toggle = new ToggleSwitch();
            var border = new Border
            {
                Child = toggle
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, toggle, border, StyleUtil.ApplyStyle);
            state.ApplyVariantsStyle(toggle, border, StyleUtil.ApplyStyle);

            // 启用状态
            if (args.IsEnabled is not null)
                uiScope.CreateEffect(() => toggle.IsEnabled = args.IsEnabled.RxValue);

            // 双向绑定 BindIsChecked
            if (args.BindIsChecked is not null)
            {
                uiScope.CreateEffect(epochScope => toggle.IsChecked = epochScope.Pull(args.BindIsChecked));
                toggle.IsCheckedChanged += (_, _) =>
                {
                    var newValue = toggle.IsChecked == true;
                    if (args.BindIsChecked.RxValue != newValue)
                        args.BindIsChecked.RxValue = newValue;
                    args.OnCheckedChanged?.Invoke(newValue);
                };
            }
            else if (args.IsChecked is not null)
            {
                toggle.IsChecked = args.IsChecked.Value;
                if (args.IsChecked.IsReactive)
                    uiScope.CreateEffect(epochScope => toggle.IsChecked = epochScope.Track(args.IsChecked));

                toggle.Click += (_, _) => args.OnCheckedChanged?.Invoke(toggle.IsChecked);
            }
            else if (args.OnCheckedChanged is not null)
            {
                toggle.IsCheckedChanged += (_, _) => args.OnCheckedChanged?.Invoke(toggle.IsChecked == true);
            }

            // 设置内容（通常是 TextBlock 或 StackPanel）
            var firstChild = args.FirstOrDefault();
            if (firstChild is not null)
                toggle.Content = firstChild.Content;

            return (toggle, border);
        });
    }
}