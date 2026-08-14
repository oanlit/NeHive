using System.Windows.Input;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia;

public interface INeHiveCommand : ICommand, IDisposable
{
    bool IsDisposed { get; }
    Signal<bool> ExecuteAble { get; }
    bool IsPrototype { get; }
}

public class NeHiveCommand : INeHiveCommand
{
    private readonly Scope _scope;
    private readonly Action? _execute;
    private readonly MutSignal<bool> _canExecute;

    public bool IsDisposed => _scope.IsDisposed;
    public Signal<bool> ExecuteAble => _canExecute;
    public NeHiveCommand Prototype => this;
    public bool IsPrototype => true;

    public void Execute(object? parameter)
    {
        if (!_canExecute.Value || _execute is null) return;
        _execute();
    }

    public bool CanExecute(object? parameter) => _canExecute.Value;

    public event EventHandler? CanExecuteChanged;
    private void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public void Dispose() => _scope.Dispose();

    public NeHiveCommand(Action? execute = null, Func<bool>? canExecute = null, Scope? parentScope = null)
    {
        _scope = new Scope(parentScope);
        _execute = execute;
        _canExecute = new MutSignal<bool>(true);
        if (canExecute is not null)
        {
            _scope.CreateEffect(epoch =>
            {
                var executeAble = epoch.Track(canExecute);
                if (_canExecute.Value == executeAble) return;
                _canExecute.RxValue = executeAble;
                NotifyCanExecuteChanged();
            });
        }

        _scope.OnCleanup += () =>
        {
            if (!_canExecute.Value) return;
            _canExecute.RxValue = false;
            NotifyCanExecuteChanged();
        };
    }
}

public class NeHiveCommand<T> : INeHiveCommand
{
    private readonly Scope _scope;
    private readonly Action<T>? _execute;

    private readonly MutSignal<bool> _canExecute;
    private readonly Func<T, bool>? _canExecuteFn;

    public bool IsDisposed => _scope.IsDisposed;
    public Signal<bool> ExecuteAble => _canExecute;
    public readonly Accessor<T> Parameter;
    public NeHiveCommand<T> Prototype { get; private init; }
    public bool IsPrototype => Prototype == this;

    public void Execute(object? parameter)
    {
        if (!_canExecute.Value || _execute is null) return;
        _execute(Parameter.Value);
    }

    public bool CanExecute(object? parameter) => _canExecute.Value;

    public event EventHandler? CanExecuteChanged;
    private void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public void Dispose() => _scope.Dispose();

    public NeHiveCommand(Accessor<T> parameter, Action<T>? execute = null, Func<T, bool>? canExecute = null,
        Scope? parentScope = null)
    {
        _scope = new Scope(parentScope);
        Parameter = parameter;
        _execute = execute;
        _canExecute = new MutSignal<bool>(true);
        if (canExecute is not null)
        {
            _canExecuteFn = canExecute;
            _scope.CreateEffect(epoch =>
            {
                var executeAble = epoch.Track(() => canExecute(Parameter.RxValue));
                if (executeAble == _canExecute.Value) return;
                _canExecute.RxValue = executeAble;
                NotifyCanExecuteChanged();
            });
        }

        _scope.OnCleanup += () =>
        {
            if (!_canExecute.Value) return;
            _canExecute.RxValue = false;
            NotifyCanExecuteChanged();
        };

        Prototype = this;
    }

    public NeHiveCommand<T> WithParameter(Accessor<T> parameter) =>
        new(parameter, Prototype._execute, Prototype._canExecuteFn, Prototype._scope)
        {
            Prototype = Prototype
        };
}

public class AsyncNeHiveCommand : INeHiveCommand
{
    private readonly Scope _scope;
    private readonly Func<Task>? _executeAsync;

    private readonly MutSignal<bool> _canExecute;
    private readonly MutSignal<bool> _isRunning;

    public bool IsDisposed => _scope.IsDisposed;
    public Signal<bool> ExecuteAble => _canExecute;
    public Signal<bool> IsRunning => _isRunning;

    public bool IsPrototype => true;

    public event EventHandler? CanExecuteChanged;

    public void Execute(object? parameter)
    {
        _ = RunAsync();
    }

    public bool CanExecute(object? parameter) => _canExecute.Value;

    public void Dispose() => _scope.Dispose();

    private void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private async Task RunAsync()
    {
        if (!_canExecute.Value || _executeAsync is null)
            return;

        _isRunning.RxValue = true;
        try
        {
            await _executeAsync();
        }
        catch
        {
            // 
        }
        finally
        {
            if (!IsDisposed)
            {
                _isRunning.RxValue = false;
            }
        }
    }

    public AsyncNeHiveCommand(
        Func<Task>? executeAsync = null,
        Func<bool>? canExecute = null,
        Scope? parentScope = null)
    {
        _scope = new Scope(parentScope);
        _executeAsync = executeAsync;

        _canExecute = new MutSignal<bool>(true);
        _isRunning = new MutSignal<bool>(false);

        if (canExecute is not null)
        {
            _scope.CreateEffect(() =>
            {
                var executable = !_isRunning.RxValue && canExecute();
                if (executable == _canExecute.Value)
                    return;
                _canExecute.RxValue = executable;
                NotifyCanExecuteChanged();
            });
        }

        _scope.OnCleanup += () =>
        {
            if (!_canExecute.Value) return;
            _canExecute.RxValue = false;
            NotifyCanExecuteChanged();
        };
    }
}

public class AsyncNeHiveCommand<T> : INeHiveCommand
{
    private readonly Scope _scope;
    private readonly Func<T, Task>? _executeAsync;
    private readonly Func<T, bool>? _canExecuteFn;

    private readonly MutSignal<bool> _canExecute;
    private readonly MutSignal<bool> _isRunning;

    public bool IsDisposed => _scope.IsDisposed;
    public Signal<bool> ExecuteAble => _canExecute;
    public Signal<bool> IsRunning => _isRunning;

    public readonly Accessor<T> Parameter;
    public AsyncNeHiveCommand<T> Prototype { get; private init; }
    public bool IsPrototype => Prototype == this;

    public event EventHandler? CanExecuteChanged;

    public void Execute(object? parameter)
    {
        _ = RunAsync();
    }

    public bool CanExecute(object? parameter) => _canExecute.Value;

    public void Dispose() => _scope.Dispose();

    private void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private async Task RunAsync()
    {
        if (!_canExecute.Value || _executeAsync is null)
            return;

        _isRunning.RxValue = true;
        try
        {
            await _executeAsync.Invoke(Parameter.Value);
        }
        catch
        {
            // 
        }
        finally
        {
            if (!IsDisposed)
            {
                _isRunning.RxValue = false;
            }
        }
    }

    public AsyncNeHiveCommand(Accessor<T> parameter,
        Func<T, Task>? executeAsync = null,
        Func<T, bool>? canExecute = null,
        Scope? parentScope = null)
    {
        _scope = new Scope(parentScope);
        Parameter = parameter;
        _executeAsync = executeAsync;
        _canExecuteFn = canExecute;

        _canExecute = new MutSignal<bool>(true);
        _isRunning = new MutSignal<bool>(false);

        if (_canExecuteFn is not null)
        {
            _scope.CreateEffect(() =>
            {
                var executable = !_isRunning.RxValue && _canExecuteFn(parameter.RxValue);
                if (executable == _canExecute.Value)
                    return;
                _canExecute.RxValue = executable;
                NotifyCanExecuteChanged();
            });
        }

        _scope.OnCleanup += () =>
        {
            if (!_canExecute.Value) return;
            _canExecute.RxValue = false;
            NotifyCanExecuteChanged();
        };

        Prototype = this;
    }

    public AsyncNeHiveCommand<T> WithParameter(Accessor<T> parameter) =>
        new(parameter, Prototype._executeAsync, Prototype._canExecuteFn, Prototype._scope)
        {
            Prototype = Prototype
        };
}