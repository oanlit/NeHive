using System.Collections;
using Avalonia.Controls;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

/// <summary>
/// 菜单项配置
/// </summary>
public class HMenuItemProp(
    Accessor<string>? header = null,
    Accessor<bool>? isEnabled = null,
    Accessor<bool>? isChecked = null,
    Action? onClick = null
) : IEnumerable<HMenuItemProp>
{
    public readonly Accessor<string> Header = header ?? "";

    // public ICommand? Command;
    // public object? CommandParameter;
    public readonly Accessor<bool> IsEnabled = isEnabled ?? true;
    public Accessor<bool> IsChecked = isChecked ?? false; // 用于可选中菜单
    public Action? OnClick = onClick;

    private readonly List<HMenuItemProp> _items = new(); // 子菜单

    public void Add(HMenuItemProp child) => _items.Add(child);

    public IEnumerator<HMenuItemProp> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// 菜单栏配置
/// </summary>
public class HMenuProp : IEnumerable<HMenuItemProp>
{
    private readonly List<HMenuItemProp> _items = [];

    public readonly Accessor<FullStyle>? Style;

    public HMenuProp(
        Accessor<string>? strStyle = null)
    {
        if (strStyle != null)
        {
            Style = StyleParser.ParseFull(strStyle);
        }
    }

    public void Add(HMenuItemProp item) => _items.Add(item);
    public IEnumerator<HMenuItemProp> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement<Menu> HMenu(HMenuProp prop)
    {
        var menu = new Menu();
        var uiScope = new UiScope();
        var border = new Border
        {
            Child = menu
        };

        // 应用样式
        if (prop.Style is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(prop.Style);
                StyleUtil.ApplyStyle(style.Normal, menu, border);
            });
        }

        // 递归构建 MenuItem
        MenuItem BuildMenuItem(HMenuItemProp itemProp)
        {
            var mi = new MenuItem();
            uiScope.CreateEffect(() => mi.Header = itemProp.Header.RxValue);
            uiScope.CreateEffect(() => mi.IsEnabled = itemProp.IsEnabled.RxValue);

            if (itemProp.OnClick is not null)
            {
                mi.Click += (_, _) => itemProp.OnClick();
            }

            // 可选：BindIsChecked 支持（需要设置 StaysOpenOnClick 等，简单起见先不处理）
            foreach (var childProp in itemProp)
            {
                mi.Items.Add(BuildMenuItem(childProp));
            }

            return mi;
        }

        foreach (var itemProp in prop)
        {
            menu.Items.Add(BuildMenuItem(itemProp));
        }

        return new Element<Menu>(uiScope, border, menu);
    }
}