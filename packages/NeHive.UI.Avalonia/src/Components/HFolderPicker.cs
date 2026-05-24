using Avalonia.Controls;
using Avalonia.Platform.Storage;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    public static IElement HFolderPicker(
        MutSignal<string?> bindSelectedPath,
        string? title = null,
        Accessor<string>? buttonText = null,
        string? startDirectory = null,
        string? strStyle = null)
    {
        buttonText ??= "Select File";
        
        var uiScope = new UiScope();
        var button = new Button();
        var border = new Border
        {
            Child = button
        };
        
        uiScope.CreateEffect(() => button.Content = buttonText?.RxValue ?? "Select File");
        
        Accessor<FullStyle>? style = null;
        if (strStyle != null)
        {
            style = StyleParser.ParseFull(strStyle);
        }
        
        if (style != null)
        {
            uiScope.CreateEffect(scope =>
            {
                var styleValue = scope.Track(style);
                StyleUtil.ApplyStyle(styleValue.Normal, button, border);
            });
        }
        
        button.Click += async (_, _) =>
        {
            var topLevel = TopLevel.GetTopLevel(button);
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

        return new Element(uiScope, border);
    }
}