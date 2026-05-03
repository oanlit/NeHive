using Avalonia.Controls;
using Avalonia.Threading;

namespace NeHive.Sample.Avalonia.Render;

public class Element
{
    private readonly UiScope _scope;
    public readonly Control Content;

    internal Element(UiScope scope, Control content)
    {
        _scope = scope;
        Content = content;
        content.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(scope.RunMount);
        DisposeContent(content);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public static Element Create(Func<UiScope, Control> builder)
    {
        var scope = new UiScope();
        var content = scope.RunInScope(() => builder(scope));
        return new Element(scope, content);
    }

    public static Element Create(Func<Control> builder)
    {
        return Create(_ => builder());
    }

    public static Element Create<TProp>(Func<TProp, UiScope, Control> builder, TProp props)
    {
        var uiScope = new UiScope();
        var control = uiScope.RunInScope(() => builder(props, uiScope));
        return new Element(uiScope, control);
    }

    public static Element Create(Func<UiScope, Element> builder)
    {
        var scope = new UiScope();
        var content = scope.RunInScope(() => builder(scope)).Content;
        return new Element(scope, content);
    }

    public static Element Create(Func<Element> builder)
    {
        return Create(_ => builder());
    }

    public static Element Create<TProp>(Func<TProp, UiScope, Element> builder, TProp props)
    {
        var uiScope = new UiScope();
        var control = uiScope.RunInScope(() => builder(props, uiScope)).Content;
        return new Element(uiScope, control);
    }

    private void DisposeContent(Control control)
    {
        _scope.OnDispose(() =>
        {
            var parent = control.Parent;
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(control);
                    break;

                case ContentControl contentControl:
                    if (contentControl.Content == control)
                        contentControl.Content = null;
                    break;

                case Decorator decorator: // 比如 Border
                    if (decorator.Child == control)
                        decorator.Child = null;
                    break;
            }
        });
    }
}

public class Element<TExpose> : Element
{
    public readonly TExpose Expose;

    public delegate Element<TExpose> ExposeBuildWithScope<in TProp>(TProp props, out TExpose expose, UiScope scope);

    public delegate Element<TExpose> ExposeBuild<in TProp>(TProp props, out TExpose expose);

    public delegate Element<TExpose> ExposeBuildWithScope(out TExpose expose, UiScope scope);

    public delegate Element<TExpose> ExposeBuild(out TExpose expose);

    public delegate Element<TExpose> ExposeCreate<in TProp>(TProp props, out TExpose expose);

    public delegate Element<TExpose> ExposeCreate(out TExpose expose);

    internal Element(UiScope scope, Control content, TExpose expose) : base(scope, content)
    {
        Expose = expose;
    }

    public static Element<TExpose> Create<TProp>(
        ExposeBuildWithScope<TProp> builder,
        TProp props,
        out TExpose expose
    )
    {
        var uiScope = new UiScope();
        var res = uiScope.RunInScope(() =>
        {
            var content = builder(props, out var expose1, uiScope).Content;
            return new
            {
                Content = content,
                Exposer = expose1
            };
        });
        var control = res.Content;
        expose = res.Exposer;
        return new Element<TExpose>(uiScope, control, expose);
    }

    public static Element<TExpose> Create<TProp>(
        ExposeBuild<TProp> builder,
        TProp props,
        out TExpose expose
    )
    {
        var uiScope = new UiScope();
        var res = uiScope.RunInScope(() =>
        {
            var content = builder(props, out var expose1).Content;
            return new
            {
                Content = content,
                Exposer = expose1
            };
        });
        var content = res.Content;
        expose = res.Exposer;
        return new Element<TExpose>(uiScope, content, expose);
    }

    public static Element<TExpose> Create(
        ExposeBuildWithScope builder,
        out TExpose expose
    )
    {
        var uiScope = new UiScope();
        var res = uiScope.RunInScope(() =>
        {
            var content = builder(out var expose1, uiScope).Content;
            return new
            {
                Content = content,
                Exposer = expose1
            };
        });
        var control = res.Content;
        expose = res.Exposer;
        return new Element<TExpose>(uiScope, control, expose);
    }

    public static Element<TExpose> Create(
        ExposeBuild builder,
        out TExpose expose
    )
    {
        var uiScope = new UiScope();
        var res = uiScope.RunInScope(() =>
        {
            var content = builder(out var expose1).Content;
            return new
            {
                Content = content,
                Exposer = expose1
            };
        });
        var control = res.Content;
        expose = res.Exposer;
        return new Element<TExpose>(uiScope, control, expose);
    }
}

public class Component
{
    public readonly Func<Element> Create;

    public Component(Func<UiScope, Control> builder)
    {
        Create = () => Element.Create(builder);
    }

    public Component(Func<Control> builder) : this(_ => builder())
    {
    }

    public Component(Func<UiScope, Element> builder)
    {
        Create = () => Element.Create(builder);
    }

    public Component(Func<Element> builder)
    {
        Create = () => Element.Create(builder);
    }

    public static implicit operator Component(Func<UiScope, Control> builder)
    {
        return new Component(builder);
    }

    public static implicit operator Component(Func<Control> builder)
    {
        return new Component(builder);
    }

    public static implicit operator Component(Func<UiScope, Element> builder)
    {
        return new Component(builder);
    }

    public static implicit operator Component(Func<Element> builder)
    {
        return new Component(builder);
    }
}

public class Component<TProp>
{
    public readonly Func<TProp, Element> Create;

    public Component(Func<TProp, UiScope, Control> builder)
    {
        Create = props => Element.Create(builder, props);
    }

    public Component(Func<TProp, Control> builder) : this((prop, _) => builder(prop))
    {
    }

    public Component(Func<TProp, UiScope, Element> builder)
    {
        Create = props => Element.Create(builder, props);
    }

    public Component(Func<TProp, Element> builder) : this((prop, _) => builder(prop))
    {
    }

    public static implicit operator Component<TProp>(Func<TProp, UiScope, Control> builder)
    {
        return new Component<TProp>(builder);
    }

    public static implicit operator Component<TProp>(Func<TProp, Control> builder)
    {
        return new Component<TProp>(builder);
    }

    public static implicit operator Component<TProp>(Func<TProp, UiScope, Element> builder)
    {
        return new Component<TProp>(builder);
    }

    public static implicit operator Component<TProp>(Func<TProp, Element> builder)
    {
        return new Component<TProp>(builder);
    }
}

public class ExposeComponent<TExpose>
{
    private readonly Element<TExpose>.ExposeCreate _create;

    public ExposeComponent(Element<TExpose>.ExposeBuildWithScope builder)
    {
        _create = (out expose) =>
            Element<TExpose>.Create(builder, out expose);
    }

    public ExposeComponent(Element<TExpose>.ExposeBuild builder)
    {
        _create = (out expose) =>
            Element<TExpose>.Create(builder, out expose);
    }

    public Element<TExpose> Create()
    {
        return _create(out _);
    }

    public Element<TExpose> Create(out TExpose expose)
    {
        return _create(out expose);
    }
    
    public Element<TExpose> Create(Action<TExpose> fn)
    {
        var el =  _create(out var expose);
        fn(expose);
        return el;
    }
}

public class Component<TProp, TExpose>
{
    private readonly Element<TExpose>.ExposeCreate<TProp> _create;

    public Component(Element<TExpose>.ExposeBuildWithScope<TProp> builder)
    {
        _create = (props, out expose) =>
            Element<TExpose>.Create(builder, props, out expose);
    }

    public Component(Element<TExpose>.ExposeBuild<TProp> builder)
    {
        _create = (props, out expose) =>
            Element<TExpose>.Create(builder, props, out expose);
    }

    public Element<TExpose> Create(TProp props)
    {
        return _create(props, out _);
    }

    public Element<TExpose> Create(TProp props, out TExpose expose)
    {
        return _create(props, out expose);
    }
    
    public Element<TExpose> Create(TProp props, Action<TExpose> fn)
    {
        var el =  _create(props, out var expose);
        fn(expose);
        return el;
    }
}