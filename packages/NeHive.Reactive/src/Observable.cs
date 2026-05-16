//-----------------------------------------------------------------------------
// NeHive Reactive Flow & Observable Extensions
//
// This module implements reactive stream operators (Filter/Map/Debounce/Throttle)
// inspired by ReactiveX (Rx.NET / RxJS) design patterns.
// Implemented independently for NeHive reactive runtime.
//-----------------------------------------------------------------------------

namespace NeHive.Reactive;

using System;

public class SignalObservable<T>(MutSignal<T> mutSignal) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var effect = new Effect(epochScope =>
        {
            var value = epochScope.Pull(mutSignal);
            observer.OnNext(value);
        });

        return new Unsubscriber(effect.Dispose);
    }

    private class Unsubscriber(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}

public static partial class Rx
{
    public static ISignal<T> From<T>(IObservable<T> producer, T initialValue)
    {
        var scope = new Scope();
        var signal = new MutSignal<T>(initialValue);
        var subscription = producer.Subscribe(new ObserverSignal<T>(signal));
        scope.OnDispose(subscription.Dispose);
        return signal;
    }

    private class ObserverSignal<T>(MutSignal<T> mutSignal) : IObserver<T>
    {
        public void OnNext(T value)
        {
            mutSignal.RxValue = value;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw error;
        }
    }
}

internal delegate bool ProducerFn<T>(out T value);

public readonly struct ReactiveFlow<T>
{
    internal readonly Scope Scope;
    internal readonly ProducerFn<T> Producer;

    internal ReactiveFlow(Scope scope, ProducerFn<T> producer)
    {
        Scope = scope;
        Producer = producer;
    }
}

public static partial class Rx
{
    extension(Scope scope)
    {
        // 从 MutSignal 创建流
        public ReactiveFlow<T> CreateReactiveFlow<T>(Signal<T> signal)
        {
            return new ReactiveFlow<T>(scope, (out value) =>
            {
                value = signal.RxValue;
                return true;
            });
        }

        // 从 Signal 创建流
        // public ReactiveFlow<T> CreateReactiveFlow<T>(Signal<T> accessor)
        // {
        //     return new ReactiveFlow<T>(scope, (out value) =>
        //     {
        //         value = accessor.RxValue;
        //         return true;
        //     });
        // }
    }
}

public static class ReactiveFlowExtensions
{
    // 过滤
    extension<T>(ReactiveFlow<T> flow)
    {
        public ReactiveFlow<T> Filter(Func<T, bool> predicate)
        {
            return new ReactiveFlow<T>(flow.Scope, (out value) =>
            {
                if (!flow.Producer(out value))
                    return false;

                if (predicate(value)) return true;

                value = default!;
                return false;
            });
        }

        public ReactiveFlow<T> Where(Func<T, bool> predicate)
            => Filter(flow, predicate);

        // 映射
        public ReactiveFlow<TU> Map<TU>(Func<T, TU> mapper)
        {
            return new ReactiveFlow<TU>(flow.Scope, (out value) =>
            {
                if (!flow.Producer(out var current))
                {
                    value = default!;
                    return false;
                }

                value = mapper(current);
                return true;
            });
        }

        public ReactiveFlow<TU> Select<TU>(Func<T, TU> mapper)
            => Map(flow, mapper);

        // 去重
        // public ReactiveFlow<T> Distinct(
        //     IEqualityComparer<T>? comparer = null)
        // {
        //     comparer ??= EqualityComparer<T>.Default;
        //     T last = default!;
        //     var firstRun = true;
        //
        //     return new ReactiveFlow<T>(flow.Scope, () =>
        //     {
        //         var current = flow.Producer();
        //         if (firstRun || !comparer.Equals(last, current))
        //         {
        //             firstRun = false;
        //             last = current;
        //             return current;
        //         }
        //
        //         return default!;
        //     });
        // }

        public ReactiveFlow<T> Debounce(TimeSpan delay)
        {
            var output = new MutSignal<T>(default!);

            CancellationTokenSource? cts = null;

            flow.Scope.CreateEffect(() =>
            {
                if (!flow.Producer(out var value))
                    return;

                cts?.Cancel();

                var newCts = new CancellationTokenSource();
                cts = newCts;

                _ = Cooldown();
                return;

                async Task Cooldown()
                {
                    try
                    {
                        await Task.Delay(delay, newCts.Token);

                        if (!newCts.IsCancellationRequested)
                        {
                            output.RxValue = value;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            });

            return flow.Scope.CreateReactiveFlow(output);
        }

        public ReactiveFlow<T> Debounce(long delay)
            => Debounce(flow, TimeSpan.FromMilliseconds(delay));

        public ReactiveFlow<T> ThrottleLatest(
            TimeSpan interval)
        {
            var output = new MutSignal<T>(default!);

            var throttled = false;

            T latest = default!;
            var hasLatest = false;

            flow.Scope.CreateEffect(() =>
            {
                if (!flow.Producer(out var value))
                    return;

                // 当前不在节流
                if (!throttled)
                {
                    throttled = true;

                    output.RxValue = value;

                    _ = Cooldown();

                    return;
                }

                // 节流期间，只记录最后值
                latest = value;
                hasLatest = true;
            });

            return flow.Scope.CreateReactiveFlow(output);

            async Task Cooldown()
            {
                while (true)
                {
                    await Task.Delay(interval);

                    if (!hasLatest)
                    {
                        throttled = false;
                        return;
                    }

                    var value = latest;
                    hasLatest = false;

                    output.RxValue = value;
                }
            }
        }

        public ReactiveFlow<T> ThrottleLatest(long interval)
            => ThrottleLatest(flow, TimeSpan.FromMilliseconds(interval));
    }
}

/// <summary>
/// Only by calling here will the flow start to run.
/// </summary>
public static class ReactiveFlowTerminals
{
    extension<TSource>(ReactiveFlow<TSource> flow)
    {
        public Effect PushEffect(
            Action<TSource> effect)
        {
            return flow.Scope.CreateEffect(() =>
            {
                if (flow.Producer(out var value))
                    effect(value);
            });
        }

        public Computed<TResult> PushComputed<TResult>(
            Func<TSource, TResult> executeFn)
        {
            TResult res = default!;
            return flow.Scope.CreateComputed(() =>
            {
                if (flow.Producer(out var value))
                {
                    res = executeFn(value);
                }

                return res;
            });
        }

        public AsyncMemo<TResult> PushAsyncMemo<TResult>(
            Func<TSource, Task<TResult>> executeFn, TResult? initValue = default)
        {
            var res = initValue!;
            return flow.Scope.CreateAsyncMemo(async () =>
            {
                if (flow.Producer(out var value))
                {
                    res = await executeFn(value);
                }

                return res;
            });
        }
    }
}