using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Styles;

namespace NeHive.UI.Avalonia.Components;

public interface ISingleChildrenArgs : IEnumerable<IElement>
{
    public void Add(IElement element);
}

public class BaseComponentProps(Scope scope, Border border, Control control)
{
    public readonly StyleProps Style = new (scope, border, control);
    public readonly Control Content = control;
    
    public Signal<bool> IsEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsEnabled);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<bool> IsEffectivelyEnabled
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsEffectivelyEnabled);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsEffectivelyEnabledProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<bool> IsHitTestVisible
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsHitTestVisible);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsHitTestVisibleProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsPointerOver
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(border.IsPointerOver);
            field = sig;

            border.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsPointerOverProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<bool> IsFocusable
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.Focusable);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.FocusableProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }

    public Signal<bool> IsFocused
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsFocused);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsFocusedProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<bool> IsKeyboardFocusWithin
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsKeyboardFocusWithin);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsKeyboardFocusWithinProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<bool> IsTabStop
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<bool>(Content.IsTabStop);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == InputElement.IsTabStopProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
    
    public Signal<object?> Tag
    {
        get
        {
            if (field is not null) return field;
            var sig = new MutSignal<object?>(Content.Tag);
            field = sig;

            Content.PropertyChanged += OnPropUpdate;
            scope.OnCleanup += () => Content.PropertyChanged -= OnPropUpdate;

            return field;

            void OnPropUpdate(object? _, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property == Control.TagProperty)
                    sig.RxValue = (bool)args.NewValue!;
            }
        }
    }
}
