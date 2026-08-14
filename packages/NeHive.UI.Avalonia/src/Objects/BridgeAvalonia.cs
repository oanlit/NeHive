using Avalonia;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Objects;

public static class BridgeAvalonia
{
    public static Signal<T> CreatePropertySignal<T>(Scope scope, AvaloniaObject obj, AvaloniaProperty<T> property,
        T initialValue)
    {
        var signal = new MutSignal<T>(initialValue);
        obj.PropertyChanged += OnPropUpdate;
        scope.OnCleanup += () => obj.PropertyChanged -= OnPropUpdate;

        return signal;

        void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == property)
                signal.RxValue = (T)args.NewValue!;
        }
    }

    public static void BindPropertySignal<T>(Scope scope, MutSignal<T> signal, AvaloniaObject obj,
        AvaloniaProperty<T> property, bool? isInit = null)
    {
        obj.PropertyChanged += OnPropUpdate;
        scope.OnCleanup += () => obj.PropertyChanged -= OnPropUpdate;
        // var isSetValue = isInit ?? property is not StyledProperty<T>;
        var isSetValue = isInit ?? false;
        scope.CreateEffect(epoch =>
        {
            var value = epoch.Pull(signal);
            if (isSetValue) obj.SetValue(property, value);
            else
                isSetValue = true;
        });

        return;

        void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == property)
                signal.RxValue = (T)args.NewValue!;
        }
    }

    extension(Scope scope)
    {
        public void AddHandler<T>(Action<T> add, Action<T> remove, T handler)
        {
            if (handler is not Delegate)
                throw new ArgumentException("handler must be a delegate.", nameof(handler));
            add(handler);
            scope.OnCleanup += () => remove(handler);
        }
    }
}