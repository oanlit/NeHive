using Lib;

namespace Demo.Store;

[Store]
public class Student
{
    public int Id { get; private set; }
    public int Age
    {
        get => field;
        set => field = value + 1;
    }
    public int DoubleAge => Age * 2;

    [NoSignal]
    public string Tag { get; set; }

    public void Update(int id, int age)
    {
        Id = id;
        Age = age;
    }

    public Student(int id, int age)
    {
        Id = id;
        Age = age;
        Tag = "default";
    }
}