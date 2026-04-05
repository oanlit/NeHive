namespace Lib;

internal static class Util
{
    public static T RemoveLast<T>(List<T> list)
    {
        var count = list.Count;
        var value = list[count - 1];
        list.RemoveAt(count - 1);
        return value;
    }
    
    public static void RemoveRangeFrom<T>(List<T> list, int indexFrom)
    {
        list.RemoveRange(indexFrom, list.Count - indexFrom);
    }

    public static Func<object> WrapAction(Action fn)
    {
        return () =>
        {
            fn();
            return Constant.EmptyObj;
        };
    }

    public static Func<object, object> WrapActionWithArg(Action fn)
    {
        return _ =>
        {
            fn();
            return Constant.EmptyObj;
        };
    }
}