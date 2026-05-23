using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public class HTabViewStyle(
    Thickness? margin = null,
    double? width = null,
    double? height = null,
    double? minWidth = null,
    double? maxWidth = null,
    double? minHeight = null,
    double? maxHeight = null,
    Thickness? padding = null,
    HorizontalAlignment? horizontalAlignment = null,
    VerticalAlignment? verticalAlignment = null,
    double? fontSize = null,
    FontWeight? fontWeight = null,
    FontStyle? fontStyle = null,
    IBrush? foreground = null,
    IBrush? background = null,
    IBrush? borderBrush = null,
    Thickness? borderThickness = null,
    CornerRadius? cornerRadius = null
)
{
    public Thickness? Margin = margin;

    public double? Width = width;
    public double? Height = height;
    public double? MinWidth = minWidth;
    public double? MaxWidth = maxWidth;
    public double? MinHeight = minHeight;
    public double? MaxHeight = maxHeight;

    public Thickness Padding = padding ?? new Thickness(8, 4);

    public HorizontalAlignment HorizontalAlignment =
        horizontalAlignment ?? HorizontalAlignment.Left;

    public VerticalAlignment VerticalAlignment =
        verticalAlignment ?? VerticalAlignment.Top;

    public double FontSize = fontSize ?? 12;
    public FontWeight? FontWeight = fontWeight;
    public FontStyle? FontStyle = fontStyle;
    public IBrush Foreground = foreground ?? Brushes.Black;

    public IBrush Background = background ?? Brushes.LightGray;
    public IBrush BorderBrush = borderBrush ?? Brushes.Gray;
    public Thickness BorderThickness = borderThickness ?? new Thickness(1);
    public CornerRadius CornerRadius = cornerRadius ?? new CornerRadius(4);

    public HTabViewStyle Merge(HButtonStyle style)
    {
        Margin = style.Margin;

        Width = style.Width;
        Height = style.Height;
        MinWidth = style.MinWidth;
        MaxWidth = style.MaxWidth;
        MinHeight = style.MinHeight;
        MaxHeight = style.MaxHeight;

        Padding = style.Padding;

        HorizontalAlignment = style.HorizontalAlignment;
        VerticalAlignment = style.VerticalAlignment;

        FontSize = style.FontSize;
        FontWeight = style.FontWeight;
        FontStyle = style.FontStyle;
        Foreground = style.Foreground;

        Background = style.Background;
        BorderBrush = style.BorderBrush;
        BorderThickness = style.BorderThickness;
        CornerRadius = style.CornerRadius;
        return this;
    }

    public static Accessor<HTabViewStyle> Parse(Accessor<string> text)
    {
        var result = new StyleSet();
        return new Computed<HTabViewStyle>(() =>
        {
            var str = text.RxValue;
            StyleParser.Parse(str, ref result);
            return new HTabViewStyle(
                result.Margin,
                result.Width,
                result.Height,
                result.MinWidth,
                result.MaxWidth,
                result.MinHeight,
                result.MinHeight,
                result.Padding,
                result.HorizontalAlignment,
                result.VerticalAlignment,
                result.FontSize,
                result.FontWeight,
                result.FontStyle,
                result.Foreground,
                result.Background,
                result.BorderBrush,
                result.BorderThickness,
                result.CornerRadius
            );
        });
    }

    public static Accessor<FullStyle> ParseFull(Accessor<string> text)
    {
        var fullStyle = new FullStyle();

        return new Computed<FullStyle>(() =>
        {
            var str = text.RxValue;
            fullStyle.Normal = DefaultStyleSet();
            fullStyle.Variants = [];
            StyleParser.ParseFullStyle(str, ref fullStyle);

            return fullStyle;
        });
    }

    public static StyleSet DefaultStyleSet()
    {
        return new StyleSet
        {
            Padding = new Thickness(8, 4),

            HorizontalAlignment = HorizontalAlignment.Left,

            VerticalAlignment = VerticalAlignment.Top,

            FontSize = 12,

            Foreground = Brushes.Black,

            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
        };
    }
}

/// <summary>
/// TabControl 配置类，支持索引器添加标签页
/// </summary>
public class HTabViewProp : IEnumerable<(Accessor<string> Header, IElement Content)>
{
    private readonly List<(Accessor<string> Header, IElement Content)> _items = [];

    public readonly MutSignal<int>? BindSelectedIndex;
    public readonly Accessor<FullStyle>? Style;

    public HTabViewProp(
        MutSignal<int>? bindSelectedIndex = null,
        Accessor<string>? strStyle = null)
    {
        BindSelectedIndex = bindSelectedIndex;
        if (strStyle != null)
        {
            Style = HButtonStyle.ParseFull(strStyle);
        }
    }

    // 索引器：支持 Accessor<string> 标题（响应式）
    public IElement this[Accessor<string> header]
    {
        set => _items.Add((header, value));
    }

    // 可选：传统 Add 方法（配合集合初始化器）
    public void Add(Accessor<string> header, IElement content) => _items.Add((header, content));
    public void Add(string header, IElement content) => _items.Add((header, content));

    public IEnumerator<(Accessor<string> Header, IElement Content)> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 TabControl
    /// </summary>
    /// <param name="prop">标签页配置</param>
    // public static IElement HTabControl(HTabViewProp prop)
    //     => CompTabControl.Create(prop);
    public static IElement HTabControl(HTabViewProp prop)
    {
        var uiScope = new UiScope();

        var tabControl = new TabControl();

        // 应用布局样式
        if (prop.Style is not null)
        {
            uiScope.CreateEffect(epochScope =>
            {
                var style = epochScope.Track(prop.Style);
                ApplyStyle(style.Normal);
            });
        }
        
        // tabControl.ItemTemplate = 

        // 构建 TabItems
        var tabItems = new List<TabItem>();
        foreach (var (headerAccessor, contentElement) in prop)
        {
            var tabItem = new TabItem();
            uiScope.CreateEffect(() => tabItem.Header = headerAccessor.RxValue);
            tabItem.Content = contentElement.Content;
            tabItems.Add(tabItem);
        }

        tabControl.ItemsSource = tabItems;

        if (prop.BindSelectedIndex is null)
        {
            if (tabItems.Count > 0) tabControl.SelectedIndex = 0;
        }
        else
        {
            // 双向绑定 selectedIndex
            // View -> ViewModel
            tabControl.SelectionChanged += (_, _) =>
            {
                if (tabControl.SelectedIndex != prop.BindSelectedIndex.RxValue)
                    prop.BindSelectedIndex.RxValue = tabControl.SelectedIndex;
            };
            // ViewModel -> View
            uiScope.CreateEffect(() =>
            {
                var idx = prop.BindSelectedIndex.RxValue;
                if (idx >= 0 && idx < tabItems.Count && idx != tabControl.SelectedIndex)
                    tabControl.SelectedIndex = idx;
            });
        }

        return new Element(uiScope, tabControl);

        void ApplyStyle(StyleSet styleValue)
        {
            if (styleValue.Margin is not null) tabControl.Margin = styleValue.Margin.Value;
            if (styleValue.ZIndex is not null) tabControl.ZIndex = styleValue.ZIndex.Value;

            if (styleValue.Width is not null) tabControl.Width = styleValue.Width.Value;
            if (styleValue.Height is not null) tabControl.Height = styleValue.Height.Value;
            if (styleValue.MaxWidth is not null) tabControl.MaxWidth = styleValue.MaxWidth.Value;
            if (styleValue.MinWidth is not null) tabControl.MinWidth = styleValue.MinWidth.Value;
            if (styleValue.MaxHeight is not null) tabControl.MaxHeight = styleValue.MaxHeight.Value;
            if (styleValue.MinHeight is not null) tabControl.MinHeight = styleValue.MinHeight.Value;

            if (styleValue.Padding is not null) tabControl.Padding = styleValue.Padding.Value;

            if (styleValue.HorizontalAlignment is not null)
                tabControl.HorizontalAlignment = styleValue.HorizontalAlignment.Value;
            if (styleValue.VerticalAlignment is not null)
                tabControl.VerticalAlignment = styleValue.VerticalAlignment.Value;

            if (styleValue.FontSize is not null) tabControl.FontSize = styleValue.FontSize.Value;
            if (styleValue.FontWeight is not null) tabControl.FontWeight = styleValue.FontWeight.Value;
            if (styleValue.FontStyle is not null) tabControl.FontStyle = styleValue.FontStyle.Value;
            tabControl.Foreground = styleValue.Foreground;

            tabControl.Background = styleValue.Background;
            tabControl.BorderBrush = styleValue.BorderBrush;
            if (styleValue.BorderThickness is not null) tabControl.BorderThickness = styleValue.BorderThickness.Value;
            if (styleValue.CornerRadius is not null) tabControl.CornerRadius = styleValue.CornerRadius.Value;
        }
    }
}