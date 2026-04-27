using NeHive.Core;

namespace NeHive.Generator.Tests;

#pragma warning disable CS9266
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
#pragma warning restore CS9266

public class GeneratorStoreTest
{
    [Fact]
    public void Store_CanBeCreated()
    {
        var store = new StudentStore();
        Assert.NotNull(store);
    }

    [Fact]
    public void Score_ShouldReactToSignal()
    {
        var store = new StudentStore();

        store.Score = 50;
        Assert.Equal(0, store.Score); // <= 60 → 0

        store.Score = 80;
        Assert.Equal(80, store.Score); // getter 不存储逻辑
        store.Dispose();
    }

    [Fact]
    public void Effect_ShouldReactToStoreSignals()
    {
        var store = new StudentStore();
        var count = 0;

        using var effect = new Effect(() =>
        {
            count++;
            _ = store.Score;
        });

        Assert.Equal(1, count);

        store.Score = 80;
        Assert.Equal(2, count);

        store.Score = 90;
        Assert.Equal(3, count);
        store.Dispose();
    }

    [Fact]
    public void Computed_ShouldCacheAndRecompute()
    {
        var store = new StudentStore();

        var v1 = store.ScoreAndLevel;
        var v2 = store.ScoreAndLevel;

        Assert.Equal(v1, v2); // cache

        store.Score = 100;
        store.Level = 10;

        var v3 = store.ScoreAndLevel;

        Assert.NotEqual(v1, v3); // dependency changed
        store.Dispose();
    }

    [Fact]
    public void Batch_ShouldTriggerSingleRecompute()
    {
        var store = new StudentStore();

        var count = 0;

        using var effect = new Effect(() =>
        {
            count++;
            _ = store.Level;
            _ = store.Score;
        });

        Assert.Equal(1, count);

        store.Update();

        Assert.Equal(2, count); // batch → 只触发一次
    }
}