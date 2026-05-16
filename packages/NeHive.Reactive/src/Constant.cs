namespace NeHive.Reactive;

internal static class Constant
{
    public static readonly ScopeNode RootScopeTree = new(true);

    public static bool EqualFn<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    public static readonly object EmptyObj = new { };

    public static readonly Action EmptyAction = () => { };
}
