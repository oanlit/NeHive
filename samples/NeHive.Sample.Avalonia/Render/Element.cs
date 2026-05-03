using Avalonia.Controls;
using Avalonia.Threading;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render;

public class Element
{
    internal readonly UiScope Scope;
    public readonly Control Content;

    internal Element(UiScope scope, Control content)
    {
        Scope = scope;
        Content = content;
        content.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(scope.RunMount);
        DisposeContent(content);
    }

    public Element(UiScope scope, Element element) : this(scope, element.Content)
    {
    }

    public static Element Empty => new(new UiScope(), new Control());

    public void Dispose()
    {
        Scope.Dispose();
    }

    public static Element Create(Func<UiScope, Element> builder)
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() => builder(uiScope));
        return element.Scope == uiScope
            ? element
            : new Element(uiScope, element.Content);
    }

    public static Element Create(Func<Element> builder)
    {
        return Create(_ => builder());
    }

    public static Element Create<TProp>(Func<TProp, UiScope, Element> builder, TProp props)
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() => builder(props, uiScope));
        return element.Scope == uiScope
            ? element
            : new Element(uiScope, element.Content);
    }

    private void DisposeContent(Control control)
    {
        Scope.OnDispose(() =>
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

    internal Element(UiScope scope, Control content, TExpose expose) : base(scope, content)
    {
        Expose = expose;
    }

    public Element(UiScope scope, Element element, TExpose expose) : base(scope, element)
    {
        Expose = expose;
    }

    public static Element<TExpose> Create<TProp>(
        Func<TProp, UiScope, Element<TExpose>> builder,
        TProp props
    )
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() =>
            builder(props, uiScope));

        return element.Scope == uiScope
            ? element
            : new Element<TExpose>(uiScope, element.Content, element.Expose);
    }

    public static Element<TExpose> Create<TProp>(
        Func<TProp, Element<TExpose>> builder,
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

    public static Element<TExpose> Create(
        Func<UiScope, Element<TExpose>> builder,
        out Element<TExpose> expose
    )
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() =>
            builder(uiScope));

        expose = element;
        return element.Scope == uiScope
            ? element
            : new Element<TExpose>(uiScope, element.Content, element.Expose);
    }

    public static Element<TExpose> Create(
        Func<Element<TExpose>> builder,
        out Element<TExpose> expose
    )
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(builder);

        expose = element;
        return element.Scope == uiScope
            ? element
            : new Element<TExpose>(uiScope, element.Content, element.Expose);
    }
}

public class ChildElement
{
    internal readonly Element Content;
    internal StackPanel? Stack { get; private init; }

    public ChildElement(Element element)
    {
        Content = new Element(element.Scope, element.Content);
    }

    public static implicit operator ChildElement(Element element)
    {
        return new(element);
    }

    public ChildElement(Component comp)
    {
        Content = comp.Create();
    }

    public static implicit operator ChildElement(Component comp)
    {
        return new(comp);
    }

    public ChildElement(Func<Element> fn)
    {
        Content = fn();
    }

    public static implicit operator ChildElement(Func<Element> fn)
    {
        return new ChildElement(fn());
    }

    public ChildElement(Element[] elements)
    {
        var uiScope = new UiScope();
        var stack = new StackPanel();
        foreach (var el in elements)
        {
            stack.Children.Add(el.Content);
        }

        Content = new Element(uiScope, stack);
        Stack = stack;
    }

    public static implicit operator ChildElement(Element[] elements)
    {
        return new ChildElement(elements);
    }

    public ChildElement(string text)
    {
        var uiScope = new UiScope();
        var textBlock = new TextBlock()
        {
            Text = text
        };
        Content = new Element(uiScope, textBlock);
    }

    public static implicit operator ChildElement(string text)
    {
        return new(text);
    }
    
    public ChildElement(IReadOnlySignal<string> text)
    {
        var uiScope = new UiScope();
        var textBlock = new TextBlock();
        uiScope.AddEffect(() => textBlock.Text = text.Value);
        Content = new Element(uiScope, textBlock);
    }

    public static implicit operator ChildElement(Accessor<string> text)
    {
        return new ChildElement(text);
    }
}

public class Component
{
    private readonly Func<Element> _create;

    public Component(Func<UiScope, Element> builder)
    {
        _create = () => Element.Create(builder);
    }

    public Component(Func<Element> builder)
    {
        _create = () => Element.Create(builder);
    }

    public Element Create()
    {
        return _create();
    }

    public Element Create(out Element expose)
    {
        expose = _create();
        return expose;
    }

    public Element Create(Action<Element> fn)
    {
        var element = _create();
        fn(element);
        return element;
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
    private readonly Func<TProp, Element> _create;

    public Component(Func<TProp, UiScope, Element> builder)
    {
        _create = props => Element.Create(builder, props);
    }

    public Component(Func<TProp, Element> builder) : this((prop, _) => builder(prop))
    {
    }

    public Element Create(TProp props)
    {
        return _create(props);
    }

    public Element Create(TProp props, out Element expose)
    {
        expose = _create(props);
        return expose;
    }

    public Element Create(TProp props, Action<Element> fn)
    {
        var element = _create(props);
        fn(element);
        return element;
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

public class Component<TProp, TExpose>
{
    private readonly Func<TProp, Element<TExpose>> _create;

    public Component(Func<TProp, Element<TExpose>> builder)
    {
        _create = props =>
            Element<TExpose>.Create(builder, props);
    }

    public Component(Func<TProp, UiScope, Element<TExpose>> builder)
    {
        _create = props =>
            Element<TExpose>.Create(builder, props);
    }

    public Element<TExpose> Create(TProp props)
    {
        return _create(props);
    }

    public Element<TExpose> Create(TProp props, out Element<TExpose> expose)
    {
        expose = _create(props);
        return expose;
    }

    public Element<TExpose> Create(TProp props, Action<Element<TExpose>> fn)
    {
        var element = _create(props);
        fn(element);
        return element;
    }
}

public class ChildComponent
{
    private readonly Component _comp;

    private ChildComponent(Component comp)
    {
        _comp = comp;
    }

    public Element Create() => _comp.Create();

    public static implicit operator ChildComponent(Component comp)
    {
        return new(comp);
    }

    public static implicit operator ChildComponent(Func<Element> fn)
    {
        return new ChildComponent(new Component(fn));
    }

    public static implicit operator ChildComponent(Element element)
    {
        return new ChildComponent(new Component(() => element));
    }

    public static implicit operator ChildComponent(Element[] elements)
    {
        return new ChildComponent(new Component(uiScope =>
        {
            var stack = new StackPanel();
            foreach (var el in elements)
            {
                stack.Children.Add(el.Content);
            }

            return new Element(uiScope, stack);
        }));
    }

    public static implicit operator ChildComponent(Accessor<string> text)
    {
        return new ChildComponent(new Component(uiScope =>
        {
            var textBlock = new TextBlock();
            uiScope.AddEffect(() => { textBlock.Text = text.Value; });
            return new Element(uiScope, textBlock);
        }));
    }
}