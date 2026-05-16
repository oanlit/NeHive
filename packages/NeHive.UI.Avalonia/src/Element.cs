using Avalonia.Controls;
using Avalonia.Threading;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia;

public interface IElement
{
    public IScope Scope { get; }
    public Control Content { get; }
    public void Dispose();
}

public interface IElement<out TExpose> : IElement
{
    public TExpose Expose { get; }
}

public class Element : IElement
{
    public IScope Scope { get; }
    public Control Content { get; }

    public Element(UiScope scope, Control content)
    {
        Scope = scope;
        Content = content;
        content.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(scope.RunMount);
        OnDisposeContent(content);
    }

    public Element(UiScope scope, IElement element) : this(scope, element.Content)
    {
    }

    internal Element(Control content)
    {
        Scope = NeHive.Reactive.Scope.CurrentScope;
        Content = content;
        OnDisposeContent(content);
    }

    public static Element Empty => new(new UiScope(), new Control());

    public void Dispose()
    {
        Scope.Dispose();
    }

    private void OnDisposeContent(Control control)
    {
        Scope.OnDispose += () =>
        {
            var parent = control.Parent;
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(control);
                    break;

                case ContentControl contentControl:
                    if (control.Equals(contentControl.Content))
                        contentControl.Content = null;
                    break;

                case Decorator decorator: // 比如 Border
                    if (decorator.Child == control)
                        decorator.Child = null;
                    break;
            }
        };
    }

    public static IElement WithScope(Func<UiScope, IElement> builder)
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() => builder(uiScope));

        return element.Scope == uiScope
            ? element
            : throw new InvalidOperationException("Cross-scope element");
    }

    public static IElement WithScope(Func<IElement> builder)
    {
        return WithScope(_ => builder());
    }

    public static IElement WithScope<TProp>(Func<TProp, UiScope, IElement> builder, TProp props)
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() => builder(props, uiScope));
        return element.Scope == uiScope
            ? element
            : throw new InvalidOperationException("Cross-scope element");
    }
}

public class Element<TExpose> : Element, IElement<TExpose>
{
    public TExpose Expose { get; }

    public Element(UiScope scope, Control content, TExpose expose) : base(scope, content)
        => Expose = expose;

    public Element(UiScope scope, Element element, TExpose expose) : base(scope, element)
        => Expose = expose;

    public static IElement<TExpose> Create<TProp>(
        Func<TProp, UiScope, IElement<TExpose>> builder,
        TProp props
    )
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() =>
            builder(props, uiScope));

        return element.Scope == uiScope
            ? element
            : throw new InvalidOperationException("Cross-scope element");
    }

    public static IElement<TExpose> Create<TProp>(
        Func<TProp, IElement<TExpose>> builder,
        TProp props
    )
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() =>
        {
            var el = builder(props);
            return el;
        });

        return element.Scope == uiScope
            ? element
            : new Element<TExpose>(uiScope, element.Content, element.Expose);
    }
}

public class Component
{
    private readonly Func<IElement> _create;

    public Component(Func<UiScope, IElement> builder)
        => _create = () => Element.WithScope(builder);

    public Component(Func<IElement> builder)
        => _create = () => Element.WithScope(builder);

    public IElement Create()
        => _create();

    public IElement Create(out IElement expose)
    {
        expose = _create();
        return expose;
    }

    public IElement Create(Action<IElement> fn)
    {
        var element = _create();
        fn(element);
        return element;
    }

    public static implicit operator Component(Func<UiScope, IElement> builder)
    {
        return new Component(builder);
    }

    public static implicit operator Component(Func<IElement> builder)
    {
        return new Component(builder);
    }
}

public class Component<TProp>
{
    private readonly Func<TProp, IElement> _create;

    public Component(Func<TProp, UiScope, IElement> builder)
    {
        _create = props => Element.WithScope(builder, props);
    }

    public Component(Func<TProp, IElement> builder) : this((prop, _) => builder(prop))
    {
    }

    public IElement Create(TProp props)
    {
        return _create(props);
    }

    public IElement Create(TProp props, out IElement expose)
    {
        expose = _create(props);
        return expose;
    }

    public IElement Create(TProp props, Action<IElement> fn)
    {
        var element = _create(props);
        fn(element);
        return element;
    }

    public static implicit operator Component<TProp>(Func<TProp, UiScope, IElement> builder)
    {
        return new Component<TProp>(builder);
    }

    public static implicit operator Component<TProp>(Func<TProp, IElement> builder)
    {
        return new Component<TProp>(builder);
    }
}

public class Component<TProp, TExpose>
{
    private readonly Func<TProp, IElement<TExpose>> _create;

    public Component(Func<TProp, IElement<TExpose>> builder)
    {
        _create = props =>
            Element<TExpose>.Create(builder, props);
    }

    public Component(Func<TProp, UiScope, IElement<TExpose>> builder)
    {
        _create = props =>
            Element<TExpose>.Create(builder, props);
    }

    public IElement<TExpose> Create(TProp props)
    {
        return _create(props);
    }

    public IElement<TExpose> Create(TProp props, out IElement<TExpose> expose)
    {
        expose = _create(props);
        return expose;
    }

    public IElement<TExpose> Create(TProp props, Action<IElement<TExpose>> fn)
    {
        var element = _create(props);
        fn(element);
        return element;
    }
}