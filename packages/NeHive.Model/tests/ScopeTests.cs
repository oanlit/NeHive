namespace NeHive.Model.Tests;

public class ScopeTests
{
    [Fact]
    public void CreateChild_Should_SetParent()
    {
        var root = new Scope(Scope.RootScope);

        var child = root.CreateChild();

        Assert.Equal(root, child.Parent);
        Assert.Contains(child, root.Children);
    }

    [Fact]
    public void CreateChildren_Should_ReturnParent_And_OutputChild()
    {
        var root = new Scope(Scope.RootScope);

        var result = root.CreateChildren(out var child);

        Assert.Equal(root, result);
        Assert.Equal(root, child.Parent);
    }

    [Fact]
    public void Dispose_Should_SetDisposed()
    {
        var scope = new Scope();

        scope.Dispose();

        Assert.True(scope.IsDisposed);
    }

    [Fact]
    public void Cleanup_Should_InvokeCallbacks_InReverseOrder()
    {
        var scope = new Scope();

        List<int> result = [];

        scope.OnCleanup += () => result.Add(1);
        scope.OnCleanup += () => result.Add(2);
        scope.OnCleanup += () => result.Add(3);

        scope.Cleanup();

        Assert.Equal([3, 2, 1], result);
    }

    [Fact]
    public void DisposeChildren_Should_Dispose_AllChildren()
    {
        var root = new Scope();

        var child1 = root.CreateChild();
        var child2 = root.CreateChild();

        root.DisposeChildren();

        Assert.True(child1.IsDisposed);
        Assert.True(child2.IsDisposed);
        Assert.Empty(root.Children);
    }

    [Fact]
    public void Cleanup_Should_DisposeChildren_First()
    {
        var root = new Scope();

        List<string> result = [];

        var child = root.CreateChild();

        child.OnCleanup += () => result.Add("child");
        root.OnCleanup += () => result.Add("parent");

        root.Cleanup();

        Assert.Equal(["child", "parent"], result);
    }

    [Fact]
    public void Context_Should_Be_Inherited()
    {
        var key = new ContextKey<string>();

        var parent = new Scope();
        parent.SetContext(key, "hello");

        var child = parent.CreateChild();

        Assert.Equal("hello", child.GetContext(key));
    }

    [Fact]
    public void Context_Should_Be_Overridden()
    {
        var key = new ContextKey<string>();

        var parent = new Scope();
        parent.SetContext(key, "parent");

        var child = parent.CreateChild();
        child.SetContext(key, "child");

        Assert.Equal("child", child.GetContext(key));
    }

    [Fact]
    public void Context_Should_ReturnDefaultValue()
    {
        var key = new ContextKey<string>("default");

        var scope = new Scope();

        Assert.Equal("default", scope.GetContext(key));
    }

    [Fact]
    public void RunInScope_Should_SwitchCurrentScope()
    {
        var scope = new Scope();

        Scope? current = null;

        scope.RunInScope(() =>
        {
            current = NeHiveContext.CurrentScope;
        });

        Assert.Equal(scope, current);
    }

    [Fact]
    public void RunInScope_Should_RestorePreviousScope()
    {
        var original = NeHiveContext.CurrentScope;

        var scope = new Scope();

        scope.RunInScope(() =>
        {
            Assert.Equal(scope, NeHiveContext.CurrentScope);
        });

        Assert.Equal(original, NeHiveContext.CurrentScope);
    }

    [Fact]
    public void ScopeFrame_Should_Throw_WhenScopeDisposed()
    {
        var scope = new Scope();
        scope.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
        {
            using var _ = new ScopeFrame(scope);
        });
    }

    [Fact]
    public void Cleanup_Should_CollectExceptions()
    {
        var scope = new Scope();

        scope.OnCleanup += () => throw new InvalidOperationException();
        scope.OnCleanup += () => { };

        scope.Cleanup(out var exceptions);

        Assert.Single(exceptions);
        Assert.IsType<InvalidOperationException>(exceptions[0].Item1);
    }

    [Fact]
    public void Dispose_Should_CollectExceptions()
    {
        var scope = new Scope();

        scope.OnCleanup += () => throw new InvalidOperationException();

        scope.Dispose(out var exceptions);

        Assert.True(scope.IsDisposed);
        Assert.Single(exceptions);
    }

    [Fact]
    public void RemovingCleanupHandler_Should_PreventExecution()
    {
        var scope = new Scope();

        var executed = false;

        Action action = () => executed = true;

        scope.OnCleanup += action;
        scope.OnCleanup -= action;

        scope.Cleanup();

        Assert.False(executed);
    }

    [Fact]
    public void ContextKey_GetContext_Should_ReadCurrentScope()
    {
        var key = new ContextKey<string>();

        var scope = new Scope();
        scope.SetContext(key, "test");

        string? value = null;

        scope.RunInScope(() =>
        {
            value = key.GetContext();
        });

        Assert.Equal("test", value);
    }
}