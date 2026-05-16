namespace NeHive.Generator
{
    public static class Util
    {
        public static string LowerFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}