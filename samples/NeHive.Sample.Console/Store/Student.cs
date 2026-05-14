using NeHive.Reactive;

namespace NeHive.Sample.Console.Store;

using Console = System.Console;

[Store]
public class Student(int level = 10)
{
    // 🔥 1. 三元表达式
    public int Score
    {
        get => field > 60 ? field : 0;
        set => field = value > 100 ? 100 : value;
    } = 0;

    [Computed] public int ScoreAndLevel => Score + Level;

    // 🔥 2. 多次使用 field
    public int Level
    {
        get => field * field + 1;
        set => field = field + value;
    } = level;

    // 🔥 3. block + 条件
    public int Exp
    {
        get
        {
            if (field > 100)
                return field;
            return field + 10;
        }
        set
        {
            if (value < 0)
                field = 0;
            else
                field = value;
        }
    } = 30;

    // 🔥 4. 嵌套作用域（lambda）
    public int Gold
    {
        get
        {
            Func<int> f = () => field + 1;
            return f();
        }
        set
        {
            Action<int> a = v => field = v * 2;
            a(value);
        }
    }

    // 🔥 5. 局部函数
    public int Hp
    {
        get
        {
            int Calc() => field + 5;
            return Calc();
        }
        set
        {
            void Set(int v) => field = v + 10;
            Set(value);
        }
    }

    // 🔥 6. 与其他属性混用
    public int Power
    {
        get => field + Score;
        set => field = value + Level;
    }

    // ❗ 7. 混合非 field（不应该改）
    public int Mixed
    {
        get => field + Exp + 1;
        set
        {
            Console.WriteLine(value);
            field = value + 2;
        }
    }

    // ❗ 8. 只在 setter 用 field
    public int OnlySet
    {
        get => 123;
        set => field = value * 3;
    }

    // ❗ 9. 只在 getter 用 field
    public int OnlyGet
    {
        get => field * 10;
        set { Console.WriteLine(value); }
    }

    public Student(int level, int exp) : this(level)
    {
        Exp = exp;
    }

    public void Update()
    {
        Level += 10;
        Score += 10;
    }
}