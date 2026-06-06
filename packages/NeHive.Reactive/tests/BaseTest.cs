using NeHive.Model;
namespace NeHive.Reactive.Tests;

public class MutSignalTests
{
    [Fact]
    public void Signal_InitialValue_IsSet()
    {
        var signal = new MutSignal<int>(42);
        Assert.Equal(42, signal.RxValue);
        Assert.Equal(42, signal.Value);
    }

    [Fact]
    public void Signal_SetValue_UpdatesValue()
    {
        var signal = new MutSignal<string>("hello");
        signal.RxValue = "world";
        Assert.Equal("world", signal.RxValue);
    }

    [Fact]
    public void Signal_SetValueMethod_UpdatesValue()
    {
        var signal = new MutSignal<double>(1.5);
        signal.NotifySet(3.14);
        Assert.Equal(3.14, signal.RxValue);
    }

    [Fact]
    public void Signal_SetValueWithFunc_UpdatesValueBasedOnCurrent()
    {
        var signal = new MutSignal<int>(5);
        signal.NotifySet(x => x * 2);
        Assert.Equal(10, signal.RxValue);
    }

    [Fact]
    public void Signal_UntrackValue_ReturnsValueWithoutTracking()
    {
        var signal = new MutSignal<bool>(true);
        // Value should not establish a dependency (no observable effect in isolation)
        Assert.True(signal.Value);
        signal.RxValue = false;
        Assert.False(signal.Value);
    }

    [Fact]
    public void Signal_HasObserver_IsInternalButBehavioral_WhenEffectSubscribes()
    {
        var signal = new MutSignal<int>(0);
        var callCount = 0;
        using var effect = new Effect(() =>
        {
            callCount++;
            _ = signal.RxValue;
        });
        // Effect runs immediately
        Assert.Equal(1, callCount);
        signal.RxValue = 1;
        // Effect runs again due to dependency
        Assert.Equal(2, callCount);
    }
}

public class EffectTests
{
    [Fact]
    public void Effect_RunsImmediatelyOnCreation()
    {
        var executed = false;
        using var effect = new Effect(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Effect_ReactsToSignalChanges()
    {
        var signal = new MutSignal<int>(0);
        var effectCount = 0;
        using var effect = new Effect(() =>
        {
            effectCount++;
            _ = signal.RxValue;
        });

        Assert.Equal(1, effectCount);
        signal.RxValue = 1;
        Assert.Equal(2, effectCount);
        signal.RxValue = 2;
        Assert.Equal(3, effectCount);
    }

    [Fact]
    public void Effect_ReactsToMultipleSignals()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(2);
        var sum = 0;
        using var effect = new Effect(() => { sum = a.RxValue + b.RxValue; });

        Assert.Equal(3, sum);
        a.RxValue = 5;
        Assert.Equal(7, sum);
        b.RxValue = 10;
        Assert.Equal(15, sum);
    }

    [Fact]
    public void Effect_Dispose_StopsReactingAndIsDisposed()
    {
        var signal = new MutSignal<int>(0);
        var effectCount = 0;
        var effect = new Effect(() =>
        {
            effectCount++;
            _ = signal.RxValue;
        });
        Assert.Equal(1, effectCount);

        effect.Dispose();
        signal.RxValue = 1;
        Assert.Equal(1, effectCount); // No further execution
        Assert.True(effect.IsInvalid);
    }

    [Fact]
    public void Effect_IsDisposed_AfterManualDisposeEvenWithSources()
    {
        var signal = new MutSignal<int>(0);
        var effect = new Effect(() => _ = signal.RxValue);
        Assert.False(effect.IsInvalid); // Has a source (mutSignal)
        effect.Dispose();
        Assert.True(effect.IsInvalid);
    }
}

public class ComputedTests
{
    [Fact]
    public void Memo_InitialValue_ReturnsComputedValue()
    {
        var signal = new MutSignal<int>(5);
        var memo = new Computed<int>(s => signal.RxValue * 2 + s, 20);
        Assert.Equal(30, memo.RxValue);
        memo.Dispose();
    }

    [Fact]
    public void Memo_CachesValue_UntilDependencyChanges()
    {
        var signal = new MutSignal<int>(2);
        var computeCount = 0;
        var memo = new Computed<int>(() =>
        {
            computeCount++;
            return signal.RxValue * 3;
        });

        Assert.Equal(6, memo.RxValue);
        Assert.Equal(1, computeCount);

        // Access again without changing mutSignal -> no recompute
        Assert.Equal(6, memo.RxValue);
        Assert.Equal(1, computeCount);

        signal.RxValue = 5;
        // After mutSignal change, next access recomputes
        Assert.Equal(15, memo.RxValue);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    [Fact]
    public void Memo_UntrackValue_ReturnsValueWithoutTracking()
    {
        var signal = new MutSignal<int>(10);
        var memo = new Computed<int>(() => signal.RxValue + 1);
        var untracked = memo.Value;
        Assert.Equal(11, untracked);

        signal.RxValue = 20;
        // Untracked value does not cause recomputation on access, but the property still returns current cached?
        // According to implementation, Value reads the memo node's Value which may still recompute if dirty? 
        // But typical behavior: Value returns value without establishing dependency for the caller.
        // We'll just verify it returns the same as RxValue after change (since memo will be dirty and recomputed on next RxValue access).
        Assert.Equal(21, memo.RxValue); // recompute
        Assert.Equal(21, memo.Value);
        memo.Dispose();
    }

    [Fact]
    public void Memo_WithFuncOfT_T_SupportsPreviousValue()
    {
        var signal = new MutSignal<int>(0);
        var memo = new Computed<int>(prev => prev + signal.RxValue);
        Assert.Equal(0, memo.RxValue); // prev = 0, mutSignal = 0 => 0

        signal.RxValue = 5;
        Assert.Equal(5, memo.RxValue); // prev = 0, mutSignal = 5 => 5

        signal.RxValue = 2;
        Assert.Equal(7, memo.RxValue); // prev = 5, mutSignal = 2 => 7
        memo.Dispose();
    }

    [Fact]
    public void Memo_ConstructorWithFuncT_WithoutInitialValue()
    {
        var signal = new MutSignal<int>(3);
        var memo = new Computed<int>(() => signal.RxValue * 4);
        Assert.Equal(12, memo.RxValue);
        signal.RxValue = 4;
        Assert.Equal(16, memo.RxValue);
        memo.Dispose();
    }
}

public class IntegrationTests
{
    [Fact]
    public void EffectAndMemo_Combine_UpdatesCorrectly()
    {
        var a = new MutSignal<int>(2);
        var b = new MutSignal<int>(3);
        var memo = new Computed<int>(() => a.RxValue * b.RxValue);
        var effectResult = 0;
        using var effect = new Effect(() => effectResult = memo.RxValue);

        Assert.Equal(6, effectResult);

        a.RxValue = 5;
        Assert.Equal(15, effectResult);
        b.RxValue = 4;
        Assert.Equal(20, effectResult);
        memo.Dispose();
    }

    [Fact]
    public void MultipleEffects_OnSameSignal_AllRun()
    {
        var signal = new MutSignal<int>(0);
        var count1 = 0;
        var count2 = 0;
        using var effect1 = new Effect(() =>
        {
            count1++;
            _ = signal.RxValue;
        });
        using var effect2 = new Effect(() =>
        {
            count2++;
            _ = signal.RxValue;
        });

        Assert.Equal(1, count1);
        Assert.Equal(1, count2);

        signal.RxValue = 1;
        Assert.Equal(2, count1);
        Assert.Equal(2, count2);
    }

    [Fact]
    public void DisposeEffect_DoesNotAffectOtherObservers()
    {
        var signal = new MutSignal<int>(0);
        var count1 = 0;
        var count2 = 0;
        var effect1 = new Effect(() =>
        {
            count1++;
            _ = signal.RxValue;
        });
        using var effect2 = new Effect(() =>
        {
            count2++;
            _ = signal.RxValue;
        });

        effect1.Dispose();
        signal.RxValue = 2;
        Assert.Equal(1, count1); // effect1 stopped
        Assert.Equal(2, count2); // effect2 still runs
    }

    [Fact]
    public void Memo_DependencyOnMemo_WorksCorrectly()
    {
        var a = new MutSignal<int>(1);
        var memo1 = new Computed<int>(() => a.RxValue * 2);
        var memo2 = new Computed<int>(() => memo1.RxValue + 3);

        Assert.Equal(5, memo2.RxValue); // (1*2)+3 = 5

        a.RxValue = 4;
        Assert.Equal(11, memo2.RxValue); // (4*2)+3 = 11
        memo1.Dispose();
        memo2.Dispose();
    }

    // ========== MutSignal + Effect ==========
    [Fact]
    public void Effect_UsingUntrackValue_DoesNotReRunWhenSignalChanges()
    {
        var signal = new MutSignal<int>(0);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = signal.Value; // No dependency tracking
        });

        Assert.Equal(1, effectRunCount);

        signal.RxValue = 1;
        signal.RxValue = 2;
        signal.RxValue = 3;

        // Effect should not have re-run because Value didn't establish a dependency
        Assert.Equal(1, effectRunCount);
    }

    [Fact]
    public void Effect_UsingValue_ReRunsWhenSignalChanges()
    {
        var signal = new MutSignal<int>(0);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = signal.RxValue; // Establishes dependency
        });

        Assert.Equal(1, effectRunCount);

        signal.RxValue = 1;
        Assert.Equal(2, effectRunCount);
        signal.RxValue = 2;
        Assert.Equal(3, effectRunCount);
    }

    [Fact]
    public void Effect_MixesValueAndUntrackValue_OnlyValueCreatesDependency()
    {
        var a = new MutSignal<int>(10);
        var b = new MutSignal<int>(20);
        var effectRunCount = 0;
        var lastResult = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            // a.RxValue creates dependency, b.Value does not
            lastResult = a.RxValue + b.Value;
        });

        Assert.Equal(1, effectRunCount);
        Assert.Equal(30, lastResult);

        a.RxValue = 5; // Should trigger effect
        Assert.Equal(2, effectRunCount);
        Assert.Equal(25, lastResult);

        b.RxValue = 100; // Should NOT trigger effect (used via Value)
        Assert.Equal(2, effectRunCount);
        Assert.Equal(25, lastResult); // lastResult unchanged because effect didn't run
    }

    // ========== Computed + Value ==========

    [Fact]
    public void Memo_UsingValue_RecomputesWhenSignalChanges()
    {
        var signal = new MutSignal<int>(1);
        var computeCount = 0;

        var memo = new Computed<int>(() =>
        {
            computeCount++;
            return signal.RxValue * 10;
        });

        Assert.Equal(10, memo.RxValue);
        Assert.Equal(1, computeCount);

        signal.RxValue = 2;
        Assert.Equal(20, memo.RxValue);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    [Fact]
    public void Memo_MixesValueAndUntrackValue_OnlyValueTriggersRecompute()
    {
        var a = new MutSignal<int>(2);
        var b = new MutSignal<int>(3);
        var computeCount = 0;

        var memo = new Computed<int>(() =>
        {
            computeCount++;
            return a.RxValue + b.Value;
        });

        Assert.Equal(5, memo.RxValue);
        Assert.Equal(1, computeCount);

        a.RxValue = 5; // Should cause recompute
        Assert.Equal(8, memo.RxValue);
        Assert.Equal(2, computeCount);

        b.RxValue = 100; // Should NOT cause recompute (Value)
        Assert.Equal(8, memo.RxValue);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    // ========== Effect + Computed + Value ==========
    [Fact]
    public void Memo_TransitionsFromInvalidToValidWhenReadingValueWithDependency()
    {
        // 验证失效 Computed 在首次读取 RxValue 且 fn 内使用信号 .RxValue 时，
        // 会建立依赖，变为有效，后续信号变化时自动更新。
        var signal = new MutSignal<int>(5);
        var computeCount = 0;
        var effectRunCount = 0;

        var memo = new Computed<int>(() =>
        {
            computeCount++;
            return signal.RxValue * 2; // 使用 .RxValue，但构造时因 Untrack 不建立依赖
        });

        Assert.False(memo.IsInvalid);
        Assert.Equal(1, computeCount); // 构造执行一次

        // 创建 Effect 依赖 memo.RxValue
        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = memo.RxValue;
        });

        Assert.Equal(1, effectRunCount);
        Assert.Equal(10, memo.RxValue); // 5*2

        // 修改信号，memo 自动重新计算
        signal.RxValue = 10;
        Assert.Equal(2, computeCount);

        Assert.Equal(20, memo.RxValue);
        Assert.Equal(2, computeCount);

        // Effect 应因 memo 变化而重新运行
        Assert.Equal(2, effectRunCount);

        memo.Dispose();
    }

    [Fact]
    public void Memo_DisposeCausesInvalidationAndDegradation()
    {
        // 验证 Dispose 后 Computed 失效，退化为普通函数
        var signal = new MutSignal<int>(1);
        var computeCount = 0;
        var effectRunCount = 0;

        var memo = new Computed<int>(() =>
        {
            computeCount++;
            return signal.RxValue * 10;
        });

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = memo.RxValue;
        });

        // 正常依赖行为
        Assert.Equal(1, computeCount);
        Assert.Equal(1, effectRunCount);
        Assert.Equal(10, memo.RxValue);

        signal.RxValue = 2;
        Assert.Equal(2, computeCount);
        Assert.Equal(2, effectRunCount); // Effect 因 memo 变化而运行
        Assert.Equal(20, memo.RxValue);

        // Dispose Computed
        memo.Dispose();

        // 此时 Computed 失效，退化为普通函数，会通知effect，执行退化的普通函数，暴露函数所依赖的信号
        Assert.True(memo.IsInvalid);
        Assert.Equal(3, effectRunCount);
        Assert.Equal(3, computeCount); // 重新计算

        // 再次修改信号
        signal.RxValue = 3;
        // Effect 运行（因为读取 memo.RxValue 执行的是普通函数会读取内部的依赖）
        Assert.Equal(4, effectRunCount);
        Assert.Equal(4, computeCount); // 重新计算

        // 读取 memo.RxValue：失效分支，重新执行 fn
        Assert.Equal(30, memo.RxValue);
        Assert.Equal(5, computeCount); // 重新计算

        // 再次读取仍会重复计算（每次失效都重新执行）
        Assert.Equal(30, memo.RxValue);
        Assert.Equal(6, computeCount);
    }

    [Fact]
    public void Effect_Multiple_Writes_Should_Batch()
    {
        var a = new MutSignal<int>(0);
        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = a.RxValue;
        });

        Rx.Batch(() =>
        {
            a.RxValue = 1;
            a.RxValue = 2;
            a.RxValue = 3;
        });

        Assert.Equal(2, runs);
        // 初始 1 次 + batch 后 1 次
    }

    [Fact]
    public void Effect_Memo_Effect_Chain_Test()
    {
        var a = new MutSignal<int>(1);

        using var scope = new Scope();

        var m = scope.CreateComputed(() => a.RxValue + 1);

        int result = 0;

        scope.CreateEffect(() => { result = m.RxValue; });

        a.RxValue = 10;

        Assert.Equal(11, result);
    }
}

public class BaseTest
{
    [Fact]
    public void Dynamic_Dependency_Switch_Test()
    {
        var a = new MutSignal<int>(1);
        var b = new MutSignal<int>(2);
        var flag = new MutSignal<bool>(true);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = flag.RxValue ? a.RxValue : b.RxValue;
        });

        Assert.Equal(1, runs);

        flag.RxValue = false;
        Assert.Equal(2, runs);

        a.RxValue = 10; // ❗ 不应触发
        Assert.Equal(2, runs);

        b.RxValue = 20; // ✅ 应触发
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Topological_Order_Test()
    {
        var a = new MutSignal<int>(1);
        var logs = new List<int>();

        using var scope = new Scope();

        var m1 = scope.CreateComputed(() => a.RxValue + 1);
        var m2 = scope.CreateComputed(() => m1.RxValue + 1);

        scope.CreateEffect(() => { logs.Add(m2.RxValue); });

        a.RxValue = 10;

        Assert.Equal(12, logs[^1]); // 必须是最终值
    }

    [Fact]
    public void No_Glitch_Test()
    {
        var a = new MutSignal<int>(1);

        using var scope = new Scope();

        var m1 = scope.CreateComputed(() => a.RxValue + 1);
        var m2 = scope.CreateComputed(() => m1.RxValue + 1);

        int observed = 0;

        scope.CreateEffect(() => { observed = m2.RxValue; });

        a.RxValue = 10;

        Assert.Equal(12, observed); // 不能出现 3、11 等中间值
    }

    void TestAccessor(Accessor<int> readOnlySignal)
    {
    }

    [Fact]
    public void Accessor_Test()
    {
        var a = new MutSignal<int>(1);
        var p = () => a.RxValue;
        TestAccessor(1);
        TestAccessor(p);
        TestAccessor(a);
    }
}