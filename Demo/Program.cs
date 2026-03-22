using Demo.Store;
using Lib;

var studentStore = new StudentStore(1, 20);

// 测试 StudentStore
var dispose = Reactive.CreateRoot(dispose =>
{
    // 创建 effect 观察Id 和 Age 的变化
    Reactive.CreateEffect(() =>
    {
        Console.WriteLine($"Student Age changed: {studentStore.Age}");
        Console.WriteLine($"Student Id changed: {studentStore.Id}");
    });

    // 输出 DoubleAge 验证派生属性
    Reactive.CreateEffect(() => { Console.WriteLine($"DoubleAge: {studentStore.DoubleAge}"); });
    return dispose;
});

// 修改 Age，effect 应自动执行
studentStore.Age = 21; // 实际变为 22（因为 setter 加了 1）
studentStore.Update(2, 25); // 批量更新，effect 应只触发一次（若批量实现正确）

dispose();
