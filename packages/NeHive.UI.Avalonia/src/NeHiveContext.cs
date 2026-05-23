namespace NeHive.UI.Avalonia;

public static class NeHiveContext
{
    private static string _projBaseUri = "";
    public static string ProjBaseUri => _projBaseUri;

    public static void SetProjBaseUri(string baseUri)
    {
        if (!baseUri.EndsWith('/')) baseUri += "/";
        _projBaseUri = baseUri;
    }
}