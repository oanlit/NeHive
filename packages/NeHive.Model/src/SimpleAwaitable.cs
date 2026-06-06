using System.Runtime.CompilerServices;

namespace NeHive.Model;

public class SimpleAwaitable
{
    public SimpleAwaiter GetAwaiter() => new SimpleAwaiter();
}

public class SimpleAwaiter : INotifyCompletion
{
    public bool IsCompleted => true; // 直接完成
    public void GetResult() { }      // 无结果
    public void OnCompleted(Action continuation) => continuation();
}

