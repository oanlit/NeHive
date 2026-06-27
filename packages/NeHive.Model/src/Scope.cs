namespace NeHive.Model;

/// <summary>
/// Represents a disposable execution scope.
/// </summary>
public interface IScope : IContextSetter, IDisposable
{
    /// <summary>
    /// Gets whether the scope has been disposed.
    /// </summary>
    public bool IsDisposed { get; }

    /// <summary>
    /// Occurs when the scope is cleaned up.
    /// </summary>
    /// <remarks>
    /// Handlers are executed in reverse registration order.
    /// </remarks>
    public event Action OnCleanup;
}

/// <summary>
/// Represents a runtime scope.
/// </summary>
/// <remarks>
/// A scope provides two primary capabilities:
///
/// <list type="bullet">
/// <item>
/// Lifetime management through cleanup callbacks.
/// </item>
/// <item>
/// Context propagation through ContextKey values.
/// </item>
/// </list>
///
/// Conceptually, a scope acts as a runtime equivalent of a lexical block,
/// carrying both lifetime and contextual information.
/// </remarks>
/// <example>
/// <code>
/// using var scope = new Scope();
///
/// var subscription = observable.Subscribe(...);
///
/// scope.OnCleanup += subscription.Dispose;
///
/// // subscription is automatically disposed
/// // when the scope is cleaned up.
/// </code>
/// </example>
/// <example>
/// <code>
/// var themeKey = new ContextKey&lt;string&gt;();
///
/// using var scope = new Scope();
///
/// scope.SetContext(themeKey, "Dark");
///
/// using (new ScopeFrame(scope))
/// {
///     var theme = themeKey.GetContext();
/// }
/// </code>
/// </example>
public class Scope : IScope
{
    /// <summary>
    /// Gets the root scope of the application.
    /// </summary>
    /// <remarks>
    /// All scopes ultimately descend from this scope.
    /// </remarks>
    public static Scope RootScope = new(true);

    internal Scope(bool isRoot)
    {
    }

    private List<Scope> _children = [];

    /// <summary>
    /// Gets the parent scope.
    /// </summary>
    public readonly Scope? Parent;

    /// <summary>
    /// Gets the direct child scopes.
    /// </summary>
    public IReadOnlyList<Scope> Children => _children;

    internal readonly List<Action> Cleanups = [];
    internal Dictionary<IContextKey, object?>? Context;

    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new scope.
    /// </summary>
    /// <param name="parentScope">
    /// The parent scope. If <c>null</c>,
    /// <see cref="NeHiveContext.CurrentScope"/> is used.
    /// </param>
    public Scope(Scope? parentScope = null)
    {
        Parent = parentScope ?? NeHiveContext.CurrentScope;
        Parent._children.Add(this);
    }

    /// <summary>
    /// Creates a child scope.
    /// </summary>
    /// <returns>
    /// A newly created child scope.
    /// </returns>
    public Scope CreateChild()
        => new(this);

    /// <summary>
    /// Creates a child scope and returns the current scope.
    /// </summary>
    /// <param name="scope">
    /// Receives the created child scope.
    /// </param>
    /// <returns>
    /// The current scope.
    /// </returns>
    /// <remarks>
    /// Useful for fluent APIs and chained initialization.
    /// </remarks>
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
    
    /// <summary>
    /// Registers a cleanup action to run when the current reactive scope is disposed.
    /// </summary>
    /// <param name="fn">The cleanup action to execute on disposal</param>
    /// <example>
    /// <code>
    /// using var effect = new Effect(() =>
    /// {
    ///     Scope.CurrentOnCleanup(() => Console.WriteLine("Cleaning up"));
    /// });
    /// effect.Dispose(); // Prints "Cleaning up"
    /// </code>
    /// </example>
    public static void CurrentOnCleanup(Action fn)
    {
        NeHiveContext.CurrentScope.OnCleanup += fn;
    }

    /// <summary>
    /// Disposes all child scopes.
    /// </summary>
    /// <remarks>
    /// Child scopes are disposed in reverse creation order.
    /// </remarks>
    public void DisposeChildren()
    {
        if (IsDisposed) return;
        List<(Exception, Action)> exceptions = [];
        DisposeChildren(exceptions);
        if (exceptions.Count > 0) throw new Exception();
    }

    /// <summary>
    /// Disposes all child scopes and collects any exceptions.
    /// </summary>
    /// <param name="exceptions">
    /// A collection used to store cleanup exceptions.
    /// </param>
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

    /// <summary>
    /// Disposes all child scopes and collects any exceptions.
    /// </summary>
    /// <param name="exceptions">
    /// A collection used to store cleanup exceptions.
    /// </param>
    public void DisposeChildren(out List<(Exception, Action)>? exceptions)
    {
        exceptions = [];
        DisposeChildren(exceptions);
    }

    /// <summary>
    /// Cleans up the scope.
    /// </summary>
    /// <remarks>
    /// Cleanup performs the following operations:
    /// <list type="number">
    /// <item>Disposes all child scopes.</item>
    /// <item>Executes registered cleanup callbacks.</item>
    /// </list>
    /// </remarks>
    public void Cleanup()
    {
        if (IsDisposed) return;
        List<(Exception, Action)> exceptions = [];
        Cleanup(exceptions);
        if (exceptions.Count > 0) throw new Exception();
    }

    /// <summary>
    /// Cleans up the scope and collects any exceptions.
    /// </summary>
    /// <param name="exceptions">
    /// A collection used to store cleanup exceptions.
    /// </param>
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

    /// <summary>
    /// Cleans up the scope and collects any exceptions.
    /// </summary>
    /// <param name="exceptions">
    /// A collection used to store cleanup exceptions.
    /// </param>
    public void Cleanup(out List<(Exception, Action)> exceptions)
    {
        exceptions = [];
        Cleanup(exceptions);
    }

    /// <summary>
    /// Disposes the scope.
    /// </summary>
    /// <remarks>
    /// Calling this method automatically invokes <see cref="Cleanup()"/>.
    /// </remarks>
    public void Dispose()
    {
        try
        {
            Cleanup();
            _children = null!;
            ClearContext();
        }
        finally
        {
            IsDisposed = true;
        }
    }

    public void Dispose(List<(Exception, Action)> exceptions)
    {
        try
        {
            Cleanup(exceptions);
            _children = null!;
            ClearContext();
        }
        finally
        {
            IsDisposed = true;
        }
    }

    public void Dispose(out List<(Exception, Action)> exceptions)
    {
        try
        {
            Cleanup(out exceptions);
            
            _children = null!;
            ClearContext();
        }
        finally
        {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Gets a context value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">
    /// The context value type.
    /// </typeparam>
    /// <param name="contextKey">
    /// The context key.
    /// </param>
    /// <returns>
    /// The value found in the current scope or one of its ancestors;
    /// otherwise the key's default value.
    /// </returns>
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

    /// <summary>
    /// Sets a context value in the current scope.
    /// </summary>
    /// <typeparam name="T">
    /// The context value type.
    /// </typeparam>
    /// <param name="contextKey">
    /// The context key.
    /// </param>
    /// <param name="value">
    /// The value to associate with the key.
    /// </param>
    /// <returns>
    /// The current context setter.
    /// </returns>
    public IContextSetter SetContext<T>(ContextKey<T> contextKey, T value) where T : notnull
    {
        Context ??= [];
        Context[contextKey] = value;
        return this;
    }
    
    public IContextSetter RemoveContext<T>(ContextKey<T> contextKey) where T : notnull
    {
        Context?.Remove(contextKey);
        return this;
    }

    public void ClearContext()
    {
        Context = null;
    }

    /// <summary>
    /// Executes a function within this scope.
    /// </summary>
    /// <typeparam name="T">
    /// The return type.
    /// </typeparam>
    /// <param name="fn">
    /// The function to execute.
    /// </param>
    /// <returns>
    /// The function result.
    /// </returns>
    /// <remarks>
    /// This method is a convenience wrapper around <see cref="ScopeFrame"/>.
    ///
    /// For performance-critical code paths, using <see cref="ScopeFrame"/>
    /// directly avoids delegate allocations.
    /// </remarks>
    public T RunInScope<T>(Func<T> fn)
    {
        T result;
        using (new ScopeFrame(this))
        {
            result = fn();
        }

        return result;
    }

    /// <summary>
    /// Executes an action within this scope.
    /// </summary>
    /// <param name="fn">
    /// The action to execute.
    /// </param>
    /// <remarks>
    /// This method is a convenience wrapper around <see cref="ScopeFrame"/>.
    ///
    /// For performance-critical code paths, using <see cref="ScopeFrame"/>
    /// directly avoids delegate allocations.
    /// </remarks>
    public void RunInScope(Action fn)
    {
        using (new ScopeFrame(this))
        {
            fn();
        }
    }
}

/// <summary>
/// Provides access to the current NeHive execution context.
/// </summary>
public static class NeHiveContext
{
    /// <summary>
    /// Gets the current scope.
    /// </summary>
    /// <remarks>
    /// The current scope is used by ContextKey lookups
    /// and serves as the default parent when creating new scopes.
    /// </remarks>
    public static Scope CurrentScope { get; internal set; } = Scope.RootScope;

    private static string _projBaseUri = "";

    /// <summary>
    /// Gets the project base URI.
    /// </summary>
    public static string ProjBaseUri => _projBaseUri;

    /// <summary>
    /// Sets the project base URI.
    /// </summary>
    /// <param name="baseUri">
    /// The project base URI.
    /// </param>
    public static void SetProjBaseUri(string baseUri)
    {
        if (!baseUri.EndsWith('/')) baseUri += "/";
        _projBaseUri = baseUri;
    }
}

/// <summary>
/// Provides methods for storing contextual values.
/// </summary>
public interface IContextSetter
{
    /// <summary>
    /// Associates a value with the specified context key.
    /// </summary>
    /// <typeparam name="T">
    /// The context value type.
    /// </typeparam>
    /// <param name="contextKey">
    /// The context key.
    /// </param>
    /// <param name="value">
    /// The value to store.
    /// </param>
    /// <returns>
    /// The current context setter.
    /// </returns>
    public IContextSetter SetContext<T>(ContextKey<T> contextKey, T value) where T : notnull;
}

/// <summary>
/// Temporarily switches the current scope.
/// </summary>
/// <remarks>
/// Restores the previous scope when disposed.
/// ScopeFrame is typically used with a using statement:
///
/// <code>
/// using (new ScopeFrame(scope))
/// {
///     BuildUI();
/// }
/// </code>
///
/// This makes the scope boundary explicit and improves readability
/// when nested scopes are involved.
/// </remarks>
public class ScopeFrame : IDisposable
{
    private readonly Scope _lastScope;

    /// <summary>
    /// Initializes a new scope frame.
    /// </summary>
    /// <param name="scope">
    /// The target scope.
    /// </param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the specified scope has already been disposed.
    /// </exception>
    public ScopeFrame(Scope scope)
    {
        ObjectDisposedException.ThrowIf(scope.IsDisposed, nameof(Scope));
        _lastScope = NeHiveContext.CurrentScope;
        NeHiveContext.CurrentScope = scope;
    }

    /// <summary>
    /// Restores the previous scope.
    /// </summary>
    public virtual void Dispose()
    {
        NeHiveContext.CurrentScope = _lastScope;
    }
}

public interface IContextKey;

/// <summary>
/// Represents a key used to access scoped contextual data.
/// </summary>
/// <typeparam name="T">
/// The context value type.
/// </typeparam>
/// <example>
/// <code>
/// var themeKey = new ContextKey&lt;string&gt;();
///
/// using var scope = new Scope();
///
/// scope.SetContext(themeKey, "Dark");
///
/// using (new ScopeFrame(scope))
/// {
///     var theme = themeKey.GetContext();
/// };
/// </code>
/// </example>
public class ContextKey<T>(T? defaultValue = default) : IContextKey where T : notnull
{
    /// <summary>
    /// Gets the default value returned when no value is found.
    /// </summary>
    public readonly T? DefaultValue = defaultValue;

    /// <summary>
    /// Gets the value associated with this key from the current scope.
    /// </summary>
    /// <returns>
    /// The value found in the current scope hierarchy,
    /// or <see cref="DefaultValue"/> if no value exists.
    /// </returns>
    public T? GetContext()
    {
        var scope = NeHiveContext.CurrentScope;
        return scope.GetContext(this);
    }
}