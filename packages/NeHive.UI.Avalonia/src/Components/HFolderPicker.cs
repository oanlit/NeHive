using Avalonia.Controls;
using Avalonia.Platform.Storage;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HFolderPicker(
        MutSignal<string?> bindSelectedPath,
        string? title = null,
        Accessor<string>? text = null,
        string? startDirectory = null,
        Accessor<string>? strStyle = null,
        Accessor<StyleSet>? style = null,
        Dictionary<string, StyleSet>? variants = null)
    {
        text ??= "Select File";
        
        var uiScope = new UiScope();
        IElement button;

        using (new ScopeFrame(uiScope))
        {
            button = HButton(text, strStyle, style, variants);
        }
        button.Content.PointerPressed += async (_, _) =>
        {
            var topLevel = TopLevel.GetTopLevel(button.Content);
            if (topLevel == null) return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title ?? "请选择文件夹",
                SuggestedStartLocation = startDirectory != null 
                    ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(startDirectory) 
                    : null,
                AllowMultiple = false
            });

            var folder = folders.FirstOrDefault();
            if (folder != null)
            {
                bindSelectedPath.RxValue = folder.Path.AbsolutePath;
            }
        };

        return new Element(uiScope, button);
    }
}