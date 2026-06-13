using System.Collections.Frozen;
using Avalonia.Media;

using NeHive.Model;

namespace NeHive.UI.Avalonia.Styles;

internal class Fonts
{
    internal static readonly Dictionary<string, string[]> FontDict = new();
    internal static FrozenDictionary<string, FontFamily>? FinalFonts;

    internal static FrozenDictionary<string, FontFamily> ToFrozen()
    {
        var dict = new Dictionary<string, FontFamily>();
        foreach (var fontsItem in FontDict)
        {
            var key = fontsItem.Key;
            var fonts = fontsItem.Value;
            var len = fonts.Length;
            for (var i = 0; i < len; i++)
            {
                var font = fonts[i];
                if (!font.StartsWith("~/")) continue;
                fonts[i] = $"{NeHiveContext.ProjBaseUri}{font[2..]}";
            }
            var fontsString = string.Join(", ", fonts);
            dict[key] = new FontFamily(fontsString);
        }

        return dict.ToFrozenDictionary();
    }
}