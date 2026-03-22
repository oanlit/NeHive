using Lib;

namespace Demo.Store;

[Store]
public partial class Student
{
    public partial int Id { get; private set; }
    public partial string Name { get; set; }
    public partial int Age { get; set; }

    public void SetId(int id)
    {
        Reactive.Batch(() =>
        {
            Id = id;
        });
    }
}

[Store]
public partial class ClassRoom
{
    public partial string Name { get; set; }
    public partial Student Leader { get; set; }
}