using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

// public class HGridSplitterStyle(
//     Thickness? margin = null,
//     int? zIndex = null,
//     double? width = null,
//     double? height = null,
//     double? minWidth = null,
//     double? maxWidth = null,
//     double? minHeight = null,
//     double? maxHeight = null,
//     double? columnSpacing = null,
//     double? rowSpacing = null,
//     HorizontalAlignment? horizontalAlignment = null,
//     VerticalAlignment? verticalAlignment = null,
//     IBrush? background = null
// )
// {
//     // 边距
//     public Thickness Margin { get; private set; } = margin ?? new(0);
//     public int? ZIndex { get; private set; } = zIndex;
//
//     public double Width { get; private set; } = width ?? 4.0;
//     public double? Height { get; private set; } = height ?? 4.0;
//     public double? MinWidth { get; private set; } = minWidth;
//     public double? MaxWidth { get; private set; } = maxWidth;
//     public double? MinHeight { get; private set; } = minHeight;
//     public double? MaxHeight { get; private set; } = maxHeight;
//
//     public double ColumnSpacing { get; private set; } = columnSpacing ?? 0;
//     public double RowSpacing { get; private set; } = rowSpacing ?? 0;
//
//     // 对齐
//     public HorizontalAlignment HorizontalAlignment { get; private set; } =
//         horizontalAlignment ?? HorizontalAlignment.Stretch;
//
//     public VerticalAlignment VerticalAlignment { get; private set; } =
//         verticalAlignment ?? VerticalAlignment.Stretch;
//
//     // 背景
//     public IBrush? Background { get; private set; } = background;
//
//     // 样式默认值
//     public static HGridStyle Default => new();
//
//     public HGridSplitterStyle Merge(HGridSplitterStyle style)
//     {
//         Margin = style.Margin;
//
//         Width = style.Width;
//         Height = style.Height;
//         MinWidth = style.MinWidth;
//         MaxWidth = style.MaxWidth;
//         MinHeight = style.MinHeight;
//         MaxHeight = style.MaxHeight;
//
//         ColumnSpacing = style.ColumnSpacing;
//         RowSpacing = style.RowSpacing;
//         HorizontalAlignment = style.HorizontalAlignment;
//         VerticalAlignment = style.VerticalAlignment;
//         Background = style.Background ?? Background;
//         return this;
//     }
//
//     public static Accessor<HGridSplitterStyle> Parse(Accessor<string> text)
//     {
//         return new Computed<HGridSplitterStyle>(() =>
//         {
//             var str = text.RxValue;
//             var result = StyleParser.Parse(str);
//             return new HGridSplitterStyle(
//                 result.Margin,
//                 result.ZIndex,
//                 result.Width,
//                 result.Height,
//                 result.MinWidth,
//                 result.MaxWidth,
//                 result.MinHeight,
//                 result.MaxHeight,
//                 result.ColumnSpacing,
//                 result.RowSpacing,
//                 result.HorizontalAlignment,
//                 result.VerticalAlignment,
//                 result.Background
//             );
//         });
//     }
// }

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 GridSplitter 控件
    /// </summary>
    /// <param name="strStyle">样式字符串</param>
    public static IElement<GridSplitter> HGridSplitter(Accessor<string>? strStyle = null)
    {
        var uiScope = new UiScope();
        var splitter = new GridSplitter();

        // 默认值
        splitter.ResizeDirection = GridResizeDirection.Columns;
        splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
        splitter.VerticalAlignment = VerticalAlignment.Stretch;

        // 应用样式字符串（可复用 StyleParser）
        if (strStyle is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(strStyle);
                ApplySplitterStyle(splitter, style);
            });
        }

        return new Element<GridSplitter>(uiScope, splitter, splitter);
    }

    private static void ApplySplitterStyle(GridSplitter splitter, string styleStr)
    {
        var result = new StyleSet();
        StyleParser.Parse(styleStr, ref result);

        if (result.Margin is not null) splitter.Margin = splitter.Margin;
        if (result.ZIndex is not null) splitter.ZIndex = splitter.ZIndex;

        if (result.Width is not null) splitter.Width = result.Width.Value;
        if (result.Height is not null) splitter.Height = result.Height.Value;
        if (result.MinWidth is not null) splitter.MinWidth = result.MinWidth.Value;
        if (result.MaxWidth is not null) splitter.MaxWidth = result.MaxWidth.Value;
        if (result.MinHeight is not null) splitter.MinHeight = result.MinHeight.Value;
        if (result.MaxHeight is not null) splitter.MaxHeight = result.MaxHeight.Value;

        var orientation = result.Orientation;
        if (orientation is not null)
        {
            if (orientation == Orientation.Vertical)
            {
                splitter.ResizeDirection = GridResizeDirection.Rows;
            }
            else if (orientation == Orientation.Horizontal)
            {
                splitter.ResizeDirection = GridResizeDirection.Columns;
            }
        }

        if (result.HorizontalAlignment is not null)
            splitter.HorizontalAlignment = result.HorizontalAlignment.Value;
        if (result.VerticalAlignment is not null)
            splitter.VerticalAlignment = result.VerticalAlignment.Value;

        if (result.Background is not null)
            splitter.Background = result.Background;
        // 其他样式属性可扩展
    }
}