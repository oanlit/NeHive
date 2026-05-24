using Avalonia.Controls;
using Avalonia.Layout;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    /// <summary>
    /// 创建 GridSplitter 控件
    /// </summary>
    /// <param name="strStyle">样式字符串</param>
    public static IElement HGridSplitter(Accessor<string>? strStyle = null)
    {
        var uiScope = new UiScope();
        var splitter = new GridSplitter();

        // 应用样式字符串（可复用 StyleParser）
        if (strStyle is not null)
        {
            uiScope.CreateEffect(scope =>
            {
                var style = scope.Track(strStyle);
                ApplyStyle(style);
            });
        }

        return new Element(uiScope, splitter);
        
        void ApplyStyle(string styleStr)
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
                if (orientation == Orientation.Horizontal)
                {
                    splitter.ResizeDirection = GridResizeDirection.Columns;
                }
                else if (orientation == Orientation.Vertical)
                {
                    splitter.ResizeDirection = GridResizeDirection.Rows;
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
}