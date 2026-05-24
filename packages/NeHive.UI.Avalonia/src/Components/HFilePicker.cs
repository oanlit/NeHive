using Avalonia.Controls;
using Avalonia.Platform.Storage;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    /// <summary>
    /// 文件过滤器定义
    /// </summary>
    public class FilePickerFilter
    {
        public string Name { get; set; }
        public string[] Patterns { get; set; }

        public FilePickerFilter(string name, params string[] patterns)
        {
            Name = name;
            Patterns = patterns;
        }
    }

    /// <summary>
    /// 文件选择器按钮组件，点击打开系统文件对话框，选择后更新输出信号。
    /// </summary>
    /// <param name="bindSelectedPath">输出信号，选择后会被设置为所选文件的绝对路径（多选模式时为第一个文件路径）</param>
    /// <param name="title">对话框标题</param>
    /// <param name="allowMultiple">是否允许多选（此时 bindSelectedPath 输出第一个文件路径，额外路径可通过事件获取）</param>
    /// <param name="filters">文件过滤器列表，例如 new FilePickerFilter("Text files", "*.txt")</param>
    /// <param name="buttonText">按钮上显示的文本</param>
    /// <param name="strStyle">按钮样式字符串</param>
    /// <param name="onFileSelected">多选时的额外事件，参数为选中的所有文件路径数组</param>
    /// <returns>IElement 组件</returns>
    public static IElement HFilePicker(
        MutSignal<string?> bindSelectedPath,
        string? title = null,
        bool allowMultiple = false,
        FilePickerFilter[]? filters = null,
        Accessor<string>? buttonText = null,
        Accessor<string>? strStyle = null,
        Action<string[]>? onFileSelected = null
    )
    {
        buttonText ??= "Select File";

        // 按钮实例（需要获取点击事件并调用对话框）
        var uiScope = new UiScope();

        var button = new Button();

        var border = new Border
        {
            Child = button
        };

        Accessor<FullStyle>? style = null;
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }

        // 响应式更新按钮文本
        uiScope.CreateEffect(() => button.Content = buttonText?.RxValue ?? "Select File");

        if (style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var styleValue = scope.Track(style);
                StyleUtil.ApplyStyle(styleValue.Normal, button, border);
            });
        }

        // 点击时打开文件对话框
        button.Click += async (_, _) =>
        {
            // 获取顶层窗口
            var topLevel = TopLevel.GetTopLevel(button);
            if (topLevel == null) return;

            // 构建文件选择选项
            var options = new FilePickerOpenOptions
            {
                Title = title ?? "Select a file",
                AllowMultiple = allowMultiple,
                FileTypeFilter = filters?.Select(f => new FilePickerFileType(f.Name)
                {
                    Patterns = f.Patterns
                }).ToList()
            };

            var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            if (result.Count > 0)
            {
                var file = result[0];
                bindSelectedPath.RxValue = file.Path.LocalPath;
                // 如果允许多选，可以通过额外的多选事件输出，但为简化，只输出第一个路径
                onFileSelected?.Invoke(result.Select(f => f.Path.LocalPath).ToArray());
            }
            else
            {
                bindSelectedPath.RxValue = null;
                onFileSelected?.Invoke([]);
            }
        };

        return new Element(uiScope, border);
    }
}