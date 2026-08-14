using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace NeHive.UI.Avalonia.Utils;

public static class ImageUtil
{
    public static Bitmap? LoadImage(string uri)
    {
        if (string.IsNullOrEmpty(uri)) return null;

        if (!uri.StartsWith("avares://")) return LoadBitmapFromUri(uri);

        var avaresUri = new Uri(uri);

        using var stream = AssetLoader.Open(avaresUri);
        return new Bitmap(stream);
    }
    
    private static Bitmap? LoadBitmapFromUri(string uri)
    {
        try
        {
            return new Bitmap(uri);
        }
        catch
        {
            return null;
        }
    }
}