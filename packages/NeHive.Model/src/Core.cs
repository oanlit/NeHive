namespace NeHive.Model;

public interface IScope : IContextSetter, IDisposable
{
    public bool IsDisposed { get; }

    public event Action OnCleanup;
}

public class Scope : IScope
{
    public static Scope RootScope = new(true);

    internal Scope(bool isRoot)
    {
    }

    private readonly List<Scope> _children = [];

    public readonly Scope? Parent;
    public IReadOnlyList<Scope> Children => _children;
    internal readonly List<Action> Cleanups = [];
    internal Dictionary<IContextKey, object?>? Context;

    public bool IsDisposed { get; private set; }

    public Scope(Scope? parentScope = null)
    {
        Parent = parentScope ?? NeHiveContext.CurrentScope;
        Parent._children.Add(this);
    }

    public Scope CreateChild()
        => new(this);

    public Scope CreateChildren(out Scope scope)
    {
        scope = new Scope(this);
        return this;
    }

    public event Action OnCleanup
    {
        add
        {
            if (IsDisposed) return;
            Cleanups.Add(value);
        }
        remove => Cleanups.Remove(value);
    }

    public void DisposeChildren()
    {
        if (IsDisposed) return;
        List<(Exception, Action)> exceptions = [];
        DisposeChildren(exceptions);
        if (exceptions.Count > 0) throw new Exception();
    }

    public void DisposeChildren(List<(Exception, Action)>? exceptions)
    {
        if (IsDisposed) return;
        exceptions ??= [];
        int i;
        for (i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            child.Dispose(exceptions);
        }

        _children.Clear();
    }

    public void DisposeChildren(out List<(Exception, Action)>? exceptions)
    {
        exceptions = [];
        DisposeChildren(exceptions);
    }

    public void Cleanup()
    {
        if (IsDisposed) return;
        List<(Exception, Action)> exceptions = [];
        Cleanup(exceptions);
        if (exceptions.Count > 0) throw new Exception();
    }

    public void Cleanup(List<(Exception, Action)> exceptions)
    {
        if (IsDisposed) return;
        DisposeChildren(exceptions);

        if (Cleanups.Count == 0) return;

        for (var i = Cleanups.Count - 1; i >= 0; i--)
        {
            var cleanup = Cleanups[i];
            try
            {
                cleanup();
            }
            catch (Exception e)
            {
                exceptions.Add((e, cleanup));
            }
        }

        Cleanups.Clear();
    }

    public void Cleanup(out List<(Exception, Action)> exceptions)
    {
        exceptions = [];
        Cleanup(exceptions);
    }

    public void Dispose()
    {
        Cleanup();
        IsDisposed = true;
    }

    public void Dispose(List<(Exception, Action)> exceptions)
    {
        Cleanup(exceptions);
        IsDisposed = true;
    }

    public void Dispose(out List<(Exception, Action)> exceptions)
    {
        Cleanup(out exceptions);
        IsDisposed = true;
    }

    public T? GetContext<T>(ContextKey<T> contextKey) where T : notnull
    {
        var scope = this;
        do
        {
            if (scope.Context is not null)
            {
                if (scope.Context.TryGetValue(contextKey, out var value))
                {
                    if (value is T t) return t;
                }
            }

            scope = scope.Parent;
        } while (scope is not null);

        return contextKey.DefaultValue;
    }

    public IContextSetter SetContext<T>(ContextKey<T> contextKey, T value) where T : notnull
    {
        Context ??= [];
        Context[contextKey] = value;
        return this;
    }

    public T RunInScope<T>(Func<T> fn)
    {
        T result;
        using (new ScopeFrame(this))
        {
            result = fn();
        }

        return result;
    }

    public void RunInScope(Action fn)
    {
        using (new ScopeFrame(this))
        {
            fn();
        }
    }
}

public static class NeHiveContext
{
    public static Scope CurrentScope { get; internal set; } = Scope.RootScope;
    
    private static string _projBaseUri = "";
    public static string ProjBaseUri => _projBaseUri;

    public static void SetProjBaseUri(string baseUri)
    {
        if (!baseUri.EndsWith('/')) baseUri += "/";
        _projBaseUri = baseUri;
    }
}

public interface IContextSetter
{
    public IContextSetter SetContext<T>(ContextKey<T> contextKey, T value) where T : notnull;
}

public class ScopeFrame : IDisposable
{
    private readonly Scope _lastScope;

    public ScopeFrame(Scope scope)
    {
        ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
        _lastScope = NeHiveContext.CurrentScope;
        NeHiveContext.CurrentScope = scope;
    }

    public virtual void Dispose()
    {
        NeHiveContext.CurrentScope = _lastScope;
    }
}

public interface IContextKey;

public class ContextKey<T>(T? defaultValue = default) : IContextKey where T : notnull
{
    public readonly T? DefaultValue = defaultValue;

    public T? GetContext()
    {
        var scope = NeHiveContext.CurrentScope;
        return scope.GetContext(this);
    }
}