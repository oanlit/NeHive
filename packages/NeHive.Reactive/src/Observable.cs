//-----------------------------------------------------------------------------
// NeHive Reactive Flow & Observable Extensions
//
// This module implements reactive stream operators (Filter/Map/Debounce/Throttle)
// inspired by ReactiveX (Rx.NET / RxJS) design patterns.
// Implemented independently for NeHive reactive runtime.
//-----------------------------------------------------------------------------

using NeHive.Model;
namespace NeHive.Reactive;

/// <summary>
/// Converts a <see cref="MutSignal{T}"/> into a standard <see cref="IObservable{T}"/> sequence.
/// Subscriptions are backed by reactive Effects that automatically track dependencies.
/// </summary>
/// <typeparam name="T">The type of value being observed</typeparam>
public class SignalObservable<T>(MutSignal<T> mutSignal) : IObservable<T>
{
    /// <summary>
    /// Subscribes an observer to receive reactive signal updates.
    /// The subscription uses an Effect that automatically tracks <paramref name="mutSignal"/>.
    /// </summary>
    /// <param name="observer">The observer to receive values</param>
    /// <returns>An IDisposable that unsubscribes and disposes the underlying Effect</returns>
    /// <example>
    /// <code>
    /// var signal = new MutSignal&lt;int&gt;(0);
    /// var observable = new SignalObservable&lt;int&gt;(signal);
    /// using var disposable = observable.Subscribe(Console.WriteLine);
    /// signal.RxValue = 1; // Observer receives 1
    /// </code>
    /// </example>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var effect = new Effect(epochScope =>
        {
            var value = epochScope.Pull(mutSignal);
            observer.OnNext(value);
        });

        return new Unsubscriber(effect.Dispose);
    }

    /// <summary>
    /// Internal unsubscribe helper that disposes the underlying Effect.
    /// </summary>
    private class Unsubscriber(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}

/// <summary>
/// Converts external IObservable sequences into NeHive reactive signals.
/// </summary>
public static partial class Rx
{
    /// <summary>
    /// Converts an <see cref="IObservable{T}"/> into a reactive <see cref="ISignal{T}"/>.
    /// The signal is automatically updated when the observable produces values.
    /// Resources are cleaned up when the created scope is disposed.
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <param name="producer">The source observable sequence</param>
    /// <param name="initialValue">Initial value of the signal</param>
    /// <returns>A read‑only reactive signal backed by the observable</returns>
    /// <example>
    /// <code>
    /// IObservable&lt;int&gt; observable = ...;
    /// ISignal&lt;int&gt; signal = Rx.From(observable, 0);
    /// </code>
    /// </example>
    public static ISignal<T> From<T>(IObservable<T> producer, T initialValue)
    {
        var scope = new Scope();
        var signal = new MutSignal<T>(initialValue);
        var subscription = producer.Subscribe(new ObserverSignal<T>(signal));
        scope.OnCleanup += subscription.Dispose;
        return signal;
    }

    /// <summary>
    /// Observer that forwards IObservable values to a MutSignal.
    /// </summary>
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

/// <summary>
/// Represents a composable reactive data flow built from signal transformations.
/// Supports operators like Filter, Map, Debounce, ThrottleLatest.
/// </summary>
/// <typeparam name="T">Value type of the flow</typeparam>
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

/// <summary>
/// Extension methods to create and compose ReactiveFlow from signals.
/// </summary>
public static partial class Rx
{
    extension(Scope scope)
    {
        /// <summary>
        /// Creates a <see cref="ReactiveFlow{T}"/> from a reactive <see cref="Signal{T}"/>.
        /// The flow reads <see cref="ISignal{T}.RxValue"/> (with automatic dependency tracking).
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="signal">Source signal</param>
        /// <returns>A new reactive flow</returns>
        public ReactiveFlow<T> CreateReactiveFlow<T>(Signal<T> signal)
        {
            return new ReactiveFlow<T>(scope, (out value) =>
            {
                value = signal.RxValue;
                return true;
            });
        }
    }
}

/// <summary>
/// ReactiveX‑style stream operators for ReactiveFlow: Filter, Map, Debounce, ThrottleLatest.
/// </summary>
public static class ReactiveFlowExtensions
{
    extension<T>(ReactiveFlow<T> flow)
    {
        /// <summary>
        /// Filters values in the flow using a predicate.
        /// Only values that pass the predicate are propagated.
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <returns>Filtered reactive flow</returns>
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

        /// <summary>
        /// Transforms each value in the flow using a mapper function.
        /// </summary>
        /// <typeparam name="TU">Result type</typeparam>
        /// <param name="mapper">Transformation function</param>
        /// <returns>Mapped reactive flow</returns>
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

        /// <summary>
        /// Debounce values by the specified time span.
        /// Only the last value after a quiet period is emitted.
        /// </summary>
        /// <param name="delay">Quiet period</param>
        /// <returns>Debounced reactive flow</returns>
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

        /// <summary>
        /// Throttles the flow to emit at most once per interval.
        /// During throttling, only the latest value is preserved and emitted.
        /// </summary>
        /// <param name="interval">Throttle window</param>
        /// <returns>Throttled reactive flow</returns>
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
/// Terminal operators that start the reactive flow and bind it to Effects, Computed, or AsyncMemo.
/// The flow ONLY becomes active after calling one of these methods.
/// </summary>
public static class ReactiveFlowTerminals
{
    extension<TSource>(ReactiveFlow<TSource> flow)
    {
        /// <summary>
        /// Starts the flow and pushes values to an Effect.
        /// Runs every time the flow produces a new value.
        /// </summary>
        /// <param name="effect">Value handler</param>
        /// <returns>The running Effect</returns>
        public Effect PushEffect(
            Action<TSource> effect)
        {
            return flow.Scope.CreateEffect(() =>
            {
                if (flow.Producer(out var value))
                    effect(value);
            });
        }

        /// <summary>
        /// Starts the flow and creates a cached Computed value from the latest result.
        /// </summary>
        /// <typeparam name="TResult">Computed type</typeparam>
        /// <param name="executeFn">Transform function</param>
        /// <returns>Reactive computed value</returns>
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

        /// <summary>
        /// Starts the flow and creates an AsyncMemo from asynchronous processing of values.
        /// </summary>
        /// <typeparam name="TResult">Async result type</typeparam>
        /// <param name="executeFn">Async transform function</param>
        /// <param name="initValue">Initial value</param>
        /// <returns>Reactive async memo</returns>
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