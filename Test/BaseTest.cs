namespace Test;

using Lib;

using Xunit;

public class SignalTests
{
    [Fact]
    public void Signal_InitialValue_IsSet()
    {
        var signal = new Signal<int>(42);
        Assert.Equal(42, signal.Value);
        Assert.Equal(42, signal.UntrackValue);
    }

    [Fact]
    public void Signal_SetValue_UpdatesValue()
    {
        var signal = new Signal<string>("hello");
        signal.Value = "world";
        Assert.Equal("world", signal.Value);
    }

    [Fact]
    public void Signal_SetValueMethod_UpdatesValue()
    {
        var signal = new Signal<double>(1.5);
        signal.SetValue(3.14);
        Assert.Equal(3.14, signal.Value);
    }

    [Fact]
    public void Signal_SetValueWithFunc_UpdatesValueBasedOnCurrent()
    {
        var signal = new Signal<int>(5);
        signal.SetValue(x => x * 2);
        Assert.Equal(10, signal.Value);
    }

    [Fact]
    public void Signal_UntrackValue_ReturnsValueWithoutTracking()
    {
        var signal = new Signal<bool>(true);
        // UntrackValue should not establish a dependency (no observable effect in isolation)
        Assert.True(signal.UntrackValue);
        signal.Value = false;
        Assert.False(signal.UntrackValue);
    }

    [Fact]
    public void Signal_HasObserver_IsInternalButBehavioral_WhenEffectSubscribes()
    {
        var signal = new Signal<int>(0);
        var callCount = 0;
        using var effect = new Effect(() => { callCount++; _ = signal.Value; });
        // Effect runs immediately
        Assert.Equal(1, callCount);
        signal.Value = 1;
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
        var signal = new Signal<int>(0);
        var effectCount = 0;
        using var effect = new Effect(() => { effectCount++; _ = signal.Value; });

        Assert.Equal(1, effectCount);
        signal.Value = 1;
        Assert.Equal(2, effectCount);
        signal.Value = 2;
        Assert.Equal(3, effectCount);
    }

    [Fact]
    public void Effect_ReactsToMultipleSignals()
    {
        var a = new Signal<int>(1);
        var b = new Signal<int>(2);
        var sum = 0;
        using var effect = new Effect(() => { sum = a.Value + b.Value; });

        Assert.Equal(3, sum);
        a.Value = 5;
        Assert.Equal(7, sum);
        b.Value = 10;
        Assert.Equal(15, sum);
    }

    [Fact]
    public void Effect_Dispose_StopsReactingAndIsDisposed()
    {
        var signal = new Signal<int>(0);
        var effectCount = 0;
        var effect = new Effect(() => { effectCount++; _ = signal.Value; });
        Assert.Equal(1, effectCount);

        effect.Dispose();
        signal.Value = 1;
        Assert.Equal(1, effectCount); // No further execution
        Assert.True(effect.IsDisposed);
    }

    [Fact]
    public void Effect_IsDisposed_WhenSourcesBecomeEmpty()
    {
        var effect = new Effect(() => { }); // No dependencies
        // After execution, the effect has no sources, so IsDisposed becomes true
        Assert.True(effect.IsDisposed);
    }

    [Fact]
    public void Effect_IsDisposed_AfterManualDisposeEvenWithSources()
    {
        var signal = new Signal<int>(0);
        var effect = new Effect(() => _ = signal.Value);
        Assert.False(effect.IsDisposed); // Has a source (signal)
        effect.Dispose();
        Assert.True(effect.IsDisposed);
    }
}

public class MemoTests
{
    [Fact]
    public void Memo_InitialValue_ReturnsComputedValue()
    {
        var signal = new Signal<int>(5);
        var memo = new Memo<int>(s => s * 2, signal.Value);
        Assert.Equal(10, memo.Value);
        memo.Dispose();
    }

    [Fact]
    public void Memo_CachesValue_UntilDependencyChanges()
    {
        var signal = new Signal<int>(2);
        var computeCount = 0;
        var memo = new Memo<int>(() => { computeCount++; return signal.Value * 3; });

        Assert.Equal(6, memo.Value);
        Assert.Equal(1, computeCount);

        // Access again without changing signal -> no recompute
        Assert.Equal(6, memo.Value);
        Assert.Equal(1, computeCount);

        signal.Value = 5;
        // After signal change, next access recomputes
        Assert.Equal(15, memo.Value);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    [Fact]
    public void Memo_UntrackValue_ReturnsValueWithoutTracking()
    {
        var signal = new Signal<int>(10);
        var memo = new Memo<int>(() => signal.Value + 1);
        var untracked = memo.UntrackValue;
        Assert.Equal(11, untracked);

        signal.Value = 20;
        // Untracked value does not cause recomputation on access, but the property still returns current cached?
        // According to implementation, UntrackValue reads the memo node's UntrackValue which may still recompute if dirty? 
        // But typical behavior: UntrackValue returns value without establishing dependency for the caller.
        // We'll just verify it returns the same as Value after change (since memo will be dirty and recomputed on next Value access).
        Assert.Equal(21, memo.Value); // recompute
        Assert.Equal(21, memo.UntrackValue);
        memo.Dispose();
    }

    [Fact]
    public void Memo_Dispose_StopsTrackingAndIsDisposed()
    {
        var signal = new Signal<int>(1);
        var memo = new Memo<int>(() => signal.Value * 2);
        Assert.Equal(2, memo.Value);

        memo.Dispose();
        signal.Value = 10;
        // After dispose, memo no longer updates and is considered disposed
        Assert.Equal(2, memo.Value); // Stale value
        Assert.True(memo.IsDisposed);
    }

    [Fact]
    public void Memo_IsDisposed_WhenNoSources()
    {
        var memo = new Memo<int>(() => 42); // No dependencies
        // After creation, it has no sources, so IsDisposed becomes true
        Assert.True(memo.IsDisposed);
        // Value still accessible but may return stale (but constant here)
        Assert.Equal(42, memo.Value);
    }

    [Fact]
    public void Memo_WithFuncOfT_T_SupportsPreviousValue()
    {
        var signal = new Signal<int>(0);
        var memo = new Memo<int>(prev => prev + signal.Value);
        Assert.Equal(0, memo.Value); // prev = 0, signal = 0 => 0

        signal.Value = 5;
        Assert.Equal(5, memo.Value); // prev = 0, signal = 5 => 5

        signal.Value = 2;
        Assert.Equal(7, memo.Value); // prev = 5, signal = 2 => 7
        memo.Dispose();
    }

    [Fact]
    public void Memo_ConstructorWithFuncT_WithoutInitialValue()
    {
        var signal = new Signal<int>(3);
        var memo = new Memo<int>(() => signal.Value * 4);
        Assert.Equal(12, memo.Value);
        signal.Value = 4;
        Assert.Equal(16, memo.Value);
        memo.Dispose();
    }
}

public class IntegrationTests
{
    [Fact]
    public void EffectAndMemo_Combine_UpdatesCorrectly()
    {
        var a = new Signal<int>(2);
        var b = new Signal<int>(3);
        var memo = new Memo<int>(() => a.Value * b.Value);
        var effectResult = 0;
        using var effect = new Effect(() => effectResult = memo.Value);

        Assert.Equal(6, effectResult);

        a.Value = 5;
        Assert.Equal(15, effectResult);
        b.Value = 4;
        Assert.Equal(20, effectResult);
        memo.Dispose();
    }

    [Fact]
    public void MultipleEffects_OnSameSignal_AllRun()
    {
        var signal = new Signal<int>(0);
        var count1 = 0;
        var count2 = 0;
        using var effect1 = new Effect(() => { count1++; _ = signal.Value; });
        using var effect2 = new Effect(() => { count2++; _ = signal.Value; });

        Assert.Equal(1, count1);
        Assert.Equal(1, count2);

        signal.Value = 1;
        Assert.Equal(2, count1);
        Assert.Equal(2, count2);
    }

    [Fact]
    public void DisposeEffect_DoesNotAffectOtherObservers()
    {
        var signal = new Signal<int>(0);
        var count1 = 0;
        var count2 = 0;
        var effect1 = new Effect(() => { count1++; _ = signal.Value; });
        using var effect2 = new Effect(() => { count2++; _ = signal.Value; });

        effect1.Dispose();
        signal.Value = 2;
        Assert.Equal(1, count1); // effect1 stopped
        Assert.Equal(2, count2); // effect2 still runs
    }

    [Fact]
    public void Memo_DependencyOnMemo_WorksCorrectly()
    {
        var a = new Signal<int>(1);
        var memo1 = new Memo<int>(() => a.Value * 2);
        var memo2 = new Memo<int>(() => memo1.Value + 3);

        Assert.Equal(5, memo2.Value); // (1*2)+3 = 5

        a.Value = 4;
        Assert.Equal(11, memo2.Value); // (4*2)+3 = 11
        memo1.Dispose();
        memo2.Dispose();
    }
    
    // ========== Signal + Effect ==========
    [Fact]
    public void Effect_UsingUntrackValue_DoesNotReRunWhenSignalChanges()
    {
        var signal = new Signal<int>(0);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = signal.UntrackValue; // No dependency tracking
        });

        Assert.Equal(1, effectRunCount);

        signal.Value = 1;
        signal.Value = 2;
        signal.Value = 3;

        // Effect should not have re-run because UntrackValue didn't establish a dependency
        Assert.Equal(1, effectRunCount);
    }

    [Fact]
    public void Effect_UsingValue_ReRunsWhenSignalChanges()
    {
        var signal = new Signal<int>(0);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = signal.Value; // Establishes dependency
        });

        Assert.Equal(1, effectRunCount);

        signal.Value = 1;
        Assert.Equal(2, effectRunCount);
        signal.Value = 2;
        Assert.Equal(3, effectRunCount);
    }

    [Fact]
    public void Effect_MixesValueAndUntrackValue_OnlyValueCreatesDependency()
    {
        var a = new Signal<int>(10);
        var b = new Signal<int>(20);
        var effectRunCount = 0;
        var lastResult = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            // a.Value creates dependency, b.UntrackValue does not
            lastResult = a.Value + b.UntrackValue;
        });

        Assert.Equal(1, effectRunCount);
        Assert.Equal(30, lastResult);

        a.Value = 5; // Should trigger effect
        Assert.Equal(2, effectRunCount);
        Assert.Equal(25, lastResult);

        b.Value = 100; // Should NOT trigger effect (used via UntrackValue)
        Assert.Equal(2, effectRunCount);
        Assert.Equal(25, lastResult); // lastResult unchanged because effect didn't run
    }

    // ========== Memo + UntrackValue ==========

    [Fact]
    public void Memo_UsingUntrackValue_DoesNotCauseRecomputationOnSignalChange()
    {
        var signal = new Signal<int>(1);
        var computeCount = 0;

        var memo = new Memo<int>(() =>
        {
            computeCount++;
            return signal.UntrackValue * 10; // Untracked access
        });

        Assert.Equal(10, memo.Value);
        Assert.Equal(1, computeCount);

        signal.Value = 2;
        // Memo should NOT recompute because it used UntrackValue (no dependency recorded)
        Assert.Equal(10, memo.Value);
        Assert.Equal(1, computeCount);

        // Force access again – still no recomputation
        Assert.Equal(10, memo.Value);
        Assert.Equal(1, computeCount);
        memo.Dispose();
    }

    [Fact]
    public void Memo_UsingValue_RecomputesWhenSignalChanges()
    {
        var signal = new Signal<int>(1);
        var computeCount = 0;

        var memo = new Memo<int>(() =>
        {
            computeCount++;
            return signal.Value * 10;
        });

        Assert.Equal(10, memo.Value);
        Assert.Equal(1, computeCount);

        signal.Value = 2;
        Assert.Equal(20, memo.Value);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    [Fact]
    public void Memo_MixesValueAndUntrackValue_OnlyValueTriggersRecompute()
    {
        var a = new Signal<int>(2);
        var b = new Signal<int>(3);
        var computeCount = 0;

        var memo = new Memo<int>(() =>
        {
            computeCount++;
            return a.Value + b.UntrackValue;
        });

        Assert.Equal(5, memo.Value);
        Assert.Equal(1, computeCount);

        a.Value = 5; // Should cause recompute
        Assert.Equal(8, memo.Value);
        Assert.Equal(2, computeCount);

        b.Value = 100; // Should NOT cause recompute (UntrackValue)
        Assert.Equal(8, memo.Value);
        Assert.Equal(2, computeCount);
        memo.Dispose();
    }

    // ========== Effect + Memo + UntrackValue ==========

    [Fact]
    public void Effect_ObservesMemoThatUsesUntrackValue_EffectRunsOnlyWhenMemoRecomputes()
    {
        var signal = new Signal<int>(1);
        var memoComputeCount = 0;
        var effectRunCount = 0;

        var memo = new Memo<int>(() =>
        {
            memoComputeCount++;
            return signal.UntrackValue * 5; // No dependency on signal
        });

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            var _ = memo.Value; // Effect depends on memo
        });

        Assert.Equal(1, memoComputeCount);
        Assert.Equal(1, effectRunCount);
        Assert.Equal(5, memo.Value);

        signal.Value = 10;
        // Memo does NOT recompute (UntrackValue), so memo.Value unchanged
        Assert.Equal(5, memo.Value);
        Assert.Equal(1, memoComputeCount);
        // Effect should NOT run because memo.Value didn't change
        Assert.Equal(1, effectRunCount);
        memo.Dispose();
    }

    [Fact]
    public void Effect_ObservesMemoThatUsesValue_EffectRunsWhenSignalChanges()
    {
        var signal = new Signal<int>(1);
        var memoComputeCount = 0;
        var effectRunCount = 0;

        var memo = new Memo<int>(() =>
        {
            memoComputeCount++;
            return signal.Value * 5; // Establishes dependency
        });

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = memo.Value;
        });

        Assert.Equal(1, memoComputeCount);
        Assert.Equal(1, effectRunCount);
        Assert.Equal(5, memo.Value);

        signal.Value = 2;
        // Memo recomputes
        Assert.Equal(10, memo.Value);
        Assert.Equal(2, memoComputeCount);
        // Effect runs due to memo.Value change
        Assert.Equal(2, effectRunCount);
        memo.Dispose();
    }

    // ========== UntrackValue + Manual Disposal / Cleanup ==========

    [Fact]
    public void UntrackValue_DoesNotPreventDisposalWhenNoDependenciesExist()
    {
        var signal = new Signal<int>(42);
        var memo = new Memo<int>(() => signal.UntrackValue + 1);
        Assert.Equal(43, memo.Value);

        // Even though memo reads UntrackValue, it has no dependencies, so IsDisposed should become true
        Assert.True(memo.IsDisposed);

        // Access after "disposal" still returns the stale value (implementation behavior)
        signal.Value = 100;
        Assert.Equal(43, memo.Value);
        memo.Dispose();
    }
}

public class BaseTest
{
    [Fact]
    public void Dynamic_Dependency_Switch_Test()
    {
        var a = new Signal<int>(1);
        var b = new Signal<int>(2);
        var flag = new Signal<bool>(true);

        int runs = 0;

        using var effect = new Effect(() =>
        {
            runs++;
            _ = flag.Value ? a.Value : b.Value;
        });

        Assert.Equal(1, runs);

        flag.Value = false;
        Assert.Equal(2, runs);

        a.Value = 10; // ❗ 不应触发
        Assert.Equal(2, runs);

        b.Value = 20; // ✅ 应触发
        Assert.Equal(3, runs);
    }

    [Fact]
    public void Topological_Order_Test()
    {
        var a = new Signal<int>(1);
        var logs = new List<int>();

        using var owner = new Owner();

        var m1 = owner.AddMemo(() => a.Value + 1);
        var m2 = owner.AddMemo(() => m1.Value + 1);

        owner.AddEffect(() => { logs.Add(m2.Value); });

        a.Value = 10;

        Assert.Equal(12, logs[^1]); // 必须是最终值
    }

    [Fact]
    public void No_Glitch_Test()
    {
        var a = new Signal<int>(1);

        using var owner = new Owner();

        var m1 = owner.AddMemo(() => a.Value + 1);
        var m2 = owner.AddMemo(() => m1.Value + 1);

        int observed = 0;

        owner.AddEffect(() => { observed = m2.Value; });

        a.Value = 10;

        Assert.Equal(12, observed); // 不能出现 3、11 等中间值
    }
}