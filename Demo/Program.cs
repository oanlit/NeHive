using Lib;
using Demo.Store;

var s1 = new Student(1, "A", 10);
var s2 = new Student(2, "B", 20);

var dispose = Reactive.CreateRoot(dispose =>
{
    Reactive.CreateEffect(() =>
    {
        Console.WriteLine($"年龄: {s1.Age}");
    });
    Reactive.CreateEffect(() =>
    {
        Console.WriteLine($"姓名: {s1.Name}");
    });
    return dispose;
});
Reactive.Batch(() =>
{
    s1.Age = 60;
    s1.Age = 15;
});
dispose();