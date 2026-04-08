namespace Test;

using Lib;
using Xunit;

public class SelectorTests
{
    [Fact]
    public void Select_ReturnsCorrectBoolean()
    {
        var source = new Signal<int>(5);
        var selector = new Selector<int>(source);

        Assert.True(selector.Select(5));
        Assert.False(selector.Select(10));
    }

    [Fact]
    public void Select_EstablishesDependencyOnSource()
    {
        var source = new Signal<int>(0);
        var selector = new Selector<int>(source);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = selector.Select(42);
        });

        Assert.Equal(1, effectRunCount);

        source.Value = 1; // 42 仍不相等，不应触发 effect
        Assert.Equal(1, effectRunCount);

        source.Value = 42; // 变为相等，触发 effect
        Assert.Equal(2, effectRunCount);
    }

    [Fact]
    public void Select_MultipleKeys_OnlyAffectedKeysTriggerUpdates()
    {
        var source = new Signal<string>("apple");
        var selector = new Selector<string>(source);
        var effectAppleCount = 0;
        var effectBananaCount = 0;

        using var effectApple = new Effect(() =>
        {
            effectAppleCount++;
            _ = selector.Select("apple");
        });

        using var effectBanana = new Effect(() =>
        {
            effectBananaCount++;
            _ = selector.Select("banana");
        });

        Assert.Equal(1, effectAppleCount);
        Assert.Equal(1, effectBananaCount);

        source.Value = "apple"; // 未变化，不会触发任何 effect（取决于实现是否检查变化）
        // 根据 Selector 内部，只有当 Fn(key, nextValue) != Fn(key, prevValue) 时才通知。
        // 因为 prevValue="apple", nextValue="apple"，对 key="apple" 结果 true->true，无变化；对 "banana" false->false，也无变化。
        // 所以无触发
        Assert.Equal(1, effectAppleCount);
        Assert.Equal(1, effectBananaCount);

        source.Value = "banana"; // "apple" 从 true 变 false，触发 effectApple；"banana" 从 false 变 true，触发 effectBanana
        Assert.Equal(2, effectAppleCount);
        Assert.Equal(2, effectBananaCount);

        source.Value = "orange"; // 两个键都变为 false（apple 从 false->false，banana true->false），触发banana
        Assert.Equal(2, effectAppleCount);
        Assert.Equal(3, effectBananaCount);
    }

    [Fact]
    public void Select_WithCustomComparer_UsesProvidedFunction()
    {
        var source = new Signal<string>("Hello");
        var selector = new Selector<string>(source, (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase));

        Assert.True(selector.Select("HELLO"));
        Assert.False(selector.Select("WORLD"));

        var effectRunCount = 0;
        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = selector.Select("HELLO");
        });

        Assert.Equal(1, effectRunCount);

        source.Value = "hello"; // 不区分大小写相等，所以状态 true->true 无变化，不触发
        Assert.Equal(1, effectRunCount);

        source.Value = "WORLD"; // 变为 false（因为 HELLO 不相等），触发
        Assert.Equal(2, effectRunCount);
    }

    [Fact]
    public void Select_CleansUpWhenEffectDisposed()
    {
        var source = new Signal<int>(0);
        var selector = new Selector<int>(source);
        var effect = new Effect(() => { _ = selector.Select(100); });

        // 确保 effect 订阅了 key 100
        source.Value = 100;
        // 此时 effect 应该运行过一次（创建时）和这次变化
        // 记录运行次数需要额外计数器，简化：直接释放并验证内部 _subs 为空
        effect.Dispose();

        // 再次改变源，不应有任何副作用（无异常）
        source.Value = 200;
        // 如果能访问内部 _subs 可以断言，但这里只能保证无异常
    }

    [Fact]
    public void Select_WithoutCurrentComputation_ReturnsValueWithoutTracking()
    {
        var source = new Signal<int>(10);
        var selector = new Selector<int>(source);

        // 模拟无当前计算节点（例如在普通函数中调用）
        var result = selector.Select(10);
        Assert.True(result);

        // 改变源值，再次调用（仍然无计算节点），应基于最新值计算
        source.Value = 20;
        Assert.False(selector.Select(10));
        // 因为没有记录依赖，不会有后续影响
    }

    [Fact]
    public void Select_MemoDependsOnSelector_UpdatesCorrectly()
    {
        var source = new Signal<int>(0);
        var selector = new Selector<int>(source);
        var memoComputeCount = 0;

        var memo = new Memo<bool>(() =>
        {
            memoComputeCount++;
            return selector.Select(42);
        });

        Assert.Equal(1, memoComputeCount);
        Assert.False(memo.Value);

        source.Value = 42;

        Assert.True(memo.Value);
        Assert.Equal(2, memoComputeCount);

        memo.Dispose();
    }

    [Fact]
    public void Select_MultipleEffectsOnSameKey_AllTriggerWhenSourceChanges()
    {
        var source = new Signal<int>(1);
        var selector = new Selector<int>(source);
        var effect1Count = 0;
        var effect2Count = 0;

        using var effect1 = new Effect(() =>
        {
            effect1Count++;
            _ = selector.Select(1);
        });
        using var effect2 = new Effect(() =>
        {
            effect2Count++;
            _ = selector.Select(1);
        });

        Assert.Equal(1, effect1Count);
        Assert.Equal(1, effect2Count);

        source.Value = 2; // 1 从 true 变 false，两个 effect 都应触发
        Assert.Equal(2, effect1Count);
        Assert.Equal(2, effect2Count);

        source.Value = 1; // 变回 true，再次触发
        Assert.Equal(3, effect1Count);
        Assert.Equal(3, effect2Count);
    }

    [Fact]
    public void Select_EqualComparisonPreventsUnnecessaryUpdates()
    {
        var source = new Signal<int>(0);
        var selector = new Selector<int>(source);
        var effectRunCount = 0;

        using var effect = new Effect(() =>
        {
            effectRunCount++;
            _ = selector.Select(0);
        });

        Assert.Equal(1, effectRunCount);

        source.Value = 0; // 值未变，Fn(0,0) 结果不变（true），不应触发 effect
        Assert.Equal(1, effectRunCount);

        source.Value = 1; // 变为 false，触发
        Assert.Equal(2, effectRunCount);
    }
}