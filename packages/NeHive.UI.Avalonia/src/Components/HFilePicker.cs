using Avalonia.Controls;
using Avalonia.Platform.Storage;
using NeHive.Model;
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

    public static IElement HFilePicker(
        MutSignal<string?> bindSelectedPath,
        string? title = null,
        bool allowMultiple = false,
        FilePickerFilter[]? filters = null,
        Accessor<string>? text = null,
        Action<string[]>? onFileSelected = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null
    )
    {
        text ??= "Select File";

        // 按钮实例（需要获取点击事件并调用对话框）
        var uiScope = new UiScope();

        IElement button;

        using (new ScopeFrame(uiScope))
        {
            button = HButton(text, strStyle, style, variants);
        }

        // 点击时打开文件对话框
        button.Content.PointerPressed += async (_, _) =>
        {
            // 获取顶层窗口
            var topLevel = TopLevel.GetTopLevel(button.Content);
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

        return new Element(uiScope, button);
    }
}