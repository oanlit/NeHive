namespace Lib;

using System.Diagnostics;
using System.Text;

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

    /// <summary>
    /// 获取当前堆栈字符串（倒序、截断前 50 行）
    /// </summary>
    /// <returns>堆栈信息字符串</returns>
    public static string GetStackTraceString(int maxLines = 50)
    {
        var trace = new StackTrace(true); // true 获取文件名和行号
        var frames = trace.GetFrames();

        var limitedFrames = frames.Reverse().Take(maxLines);

        var sb = new StringBuilder();

        foreach (var frame in limitedFrames)
        {
            var method = frame.GetMethod();
            var file = frame.GetFileName() ?? "UnknownFile";
            var line = frame.GetFileLineNumber();
            sb.AppendLine($"{method} at {file}:{line}");
        }
        
        sb.AppendLine("...");

        return sb.ToString();
    }
}