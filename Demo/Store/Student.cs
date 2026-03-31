using Lib;

namespace Demo.Store;

[Store]
public class Student(int level = 10)
{
    // 🔥 1. 三元表达式
    public int Score
    {
        get => field > 60 ? field : 0;
        set => field = value > 100 ? 100 : value;
    } = 0;

    [Memo] public int ScoreAndLevel => Score + Level;

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
//
// public class StudentStore(int level = 10)
// {
//     private readonly Signal<int> _scoreSignal = new Signal<int>(0);
//     private readonly Signal<int> _levelSignal = new Signal<int>(level);
//     private readonly Signal<int> _expSignal = new Signal<int>(30);
//     private readonly Signal<int> _goldSignal = new Signal<int>(default);
//     private readonly Signal<int> _hpSignal = new Signal<int>(default);
//     private readonly Signal<int> _powerSignal = new Signal<int>(default);
//     private readonly Signal<int> _mixedSignal = new Signal<int>(default);
//     private readonly Signal<int> _onlysetSignal = new Signal<int>(default);
//     private readonly Signal<int> _onlygetSignal = new Signal<int>(default);
//
//     // 🔥 1. 三元表达式
//     public int Score
//     {
//         get => _scoreSignal.Value > 60 ? _scoreSignal.Value : 0;
//         set => _scoreSignal.Value = value > 100 ? 100 : value;
//     }
//
//     // 🔥 2. 多次使用 field
//     public int Level
//     {
//         get => _levelSignal.Value * _levelSignal.Value + 1;
//         set => _levelSignal.Value = _levelSignal.Value + value;
//     }
//
//     // 🔥 3. block + 条件
//     public int Exp
//     {
//         get
//         {
//             if (_expSignal.Value > 100)
//                 return _expSignal.Value;
//             return _expSignal.Value + 10;
//         }
//
//         set
//         {
//             if (value < 0)
//                 _expSignal.Value = 0;
//             else
//                 _expSignal.Value = value;
//         }
//     }
//
//     // 🔥 4. 嵌套作用域（lambda）
//     public int Gold
//     {
//         get
//         {
//             Func<int> f = () => _goldSignal.Value + 1;
//             return f();
//         }
//
//         set
//         {
//             Action<int> a = v => _goldSignal.Value = v * 2;
//             a(value);
//         }
//     }
//
//     // 🔥 5. 局部函数
//     public int Hp
//     {
//         get
//         {
//             int Calc() => _hpSignal.Value + 5;
//             return Calc();
//         }
//
//         set
//         {
//             void Set(int v) => _hpSignal.Value = v + 10;
//             Set(value);
//         }
//     }
//
//     // 🔥 6. 与其他属性混用
//     public int Power
//     {
//         get => _powerSignal.Value + Score;
//         set => _powerSignal.Value = value + Level;
//     }
//
//     // ❗ 7. 混合非 field（不应该改）
//     public int Mixed
//     {
//         get => _mixedSignal.Value + Exp + 1;
//         set
//         {
//             Console.WriteLine(value);
//             _mixedSignal.Value = value + 2;
//         }
//     }
//
//     // ❗ 8. 只在 setter 用 field
//     public int OnlySet
//     {
//         get => 123;
//         set => _onlysetSignal.Value = value * 3;
//     }
//
//     // ❗ 9. 只在 getter 用 field
//     public int OnlyGet
//     {
//         get => _onlygetSignal.Value * 10;
//         set { Console.WriteLine(value); }
//     }
//
//     public StudentStore(int level, int exp) : this(level)
//     {
//         Exp = exp;
//     }
//
//     private Memo<int>? _scoreandlevelSignal;
//     public int ScoreAndLevel => (_scoreandlevelSignal ??= new Memo<int>(() => Score + Level)).Value;
//
//     public void Update()
//     {
//         Reactive.Batch(() =>
//         {
//             Level += 10;
//             Score += 10;
//         });
//     }
// }