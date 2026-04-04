namespace Lib;

internal static class Constant
{
    public static readonly OwnerTree UnOwned = new(
        parent: null,
        children: null,
        context: null,
        cleanups: null
    );

    public static bool EqualFn<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    public static readonly object EmptyObj = new { };

    public static readonly Action EmptyAction = () => { };
}
