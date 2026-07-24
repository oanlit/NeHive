using System.Collections;
using Avalonia.Controls;
using Avalonia.Layout;

using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;
using NeHive.UI.Avalonia.State;

namespace NeHive.UI.Avalonia.Components;

public class HTabViewArgs(
    MutSignal<int>? bindSelectedIndex = null,
    Accessor<string>? strStyle = null,
    HStyle? style = null)
    : IEnumerable<(Accessor<string> Header, IElement Content)>
{
    private readonly List<(Accessor<string> Header, IElement Content)> _items = [];

    public readonly MutSignal<int>? BindSelectedIndex = bindSelectedIndex;
    public readonly Accessor<FullStyle> StrStyle = StyleParser.ParseFull(strStyle);
    public readonly Signal<StyleSet>? Style = style is null ? null : StyleUtil.HStyle2Signal(style);
    
    public IElement this[Accessor<string> header]
    {
        set => _items.Add((header, value));
    }

    public void Add(Accessor<string> header, IElement content) => _items.Add((header, content));
    public void Add(string header, IElement content) => _items.Add((header, content));

    public IEnumerator<(Accessor<string> Header, IElement Content)> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    public static IElement HTabControl(HTabViewArgs args)
    {
        return Element.WithScope(uiScope =>
        {
            var tabControl = new TabControl();
            var border = new Border
            {
                Child = tabControl
            };

            var state = new CommonState(uiScope, args.StrStyle.Value.Normal)
            {
                PriorityStyle = args.Style,
                StrVariants = args.StrStyle.Value.Variants
            };

            state.ApplyAccessorStyle(args.StrStyle, tabControl, border, ApplyStyle);
            state.ApplyVariantsStyle(tabControl, border, ApplyStyle);

            // tabControl.ItemTemplate = 

            var tabItems = new List<TabItem>();
            foreach (var (headerAccessor, contentElement) in args)
            {
                var tabItem = new TabItem();

                tabItem.Header = headerAccessor.Value;
                if (headerAccessor.IsReactive)
                    uiScope.CreateEffect(epochScope => tabItem.Header = epochScope.Track(headerAccessor));

                tabItem.Content = contentElement.Content;
                tabItems.Add(tabItem);
            }

            tabControl.ItemsSource = tabItems;

            if (args.BindSelectedIndex is null)
            {
                if (tabItems.Count > 0) tabControl.SelectedIndex = 0;
            }
            else
            {
                tabControl.SelectionChanged += (_, _) =>
                {
                    if (tabControl.SelectedIndex != args.BindSelectedIndex.RxValue)
                        args.BindSelectedIndex.RxValue = tabControl.SelectedIndex;
                };

                uiScope.CreateEffect(epoch =>
                {
                    var idx = epoch.Pull(args.BindSelectedIndex);
                    if (idx >= 0 && idx < tabItems.Count && idx != tabControl.SelectedIndex)
                        tabControl.SelectedIndex = idx;
                });
            }

            return border;

            void ApplyStyle(StyleSet styleValue, Layoutable layout, Border bord)
            {
                StyleUtil.ApplyStyle(styleValue, layout, bord);

                if (styleValue.VerticalTextAlignment is not null)
                    border.VerticalAlignment = styleValue.VerticalTextAlignment.Value;
                if (styleValue.Foreground is not null) tabControl.Foreground = styleValue.Foreground;
                if (styleValue.FontSize is not null) tabControl.FontSize = styleValue.FontSize.Value;
                if (styleValue.FontWeight is not null) tabControl.FontWeight = styleValue.FontWeight.Value;
                if (styleValue.FontStyle is not null) tabControl.FontStyle = styleValue.FontStyle.Value;
                if (styleValue.Foreground is not null) tabControl.Foreground = styleValue.Foreground;
            }
        });
    }
}