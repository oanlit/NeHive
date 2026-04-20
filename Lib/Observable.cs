namespace Lib;

using System;

public class SignalObservable<T>(Signal<T> signal) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var effect = new Effect(() =>
        {
            var value = signal.Value;
            Reactive.Untrack(() => observer.OnNext(value));
        });

        return new Unsubscriber(effect.Dispose);
    }

    private class Unsubscriber(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}

public static partial class Reactive
{
    public static IReadOnlySignal<T> From<T>(IObservable<T> producer, T initialValue)
    {
        var signal = new Signal<T>(initialValue);
        var subscription = producer.Subscribe(new ObserverSignal<T>(signal));
        OnDispose(() => subscription.Dispose());
        return signal;
    }

    private class ObserverSignal<T>(Signal<T> signal) : IObserver<T>
    {
        public void OnNext(T value)
        {
            signal.Value = value;
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