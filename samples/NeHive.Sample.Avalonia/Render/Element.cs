using Avalonia.Controls;
using Avalonia.Threading;

namespace NeHive.Sample.Avalonia.Render;

public interface IElement
{
    public UiScope Scope { get; }
    public Control Content { get; }
    public void Dispose();
}

public interface IElement<out TExpose> : IElement
{
    public TExpose Expose { get; }
}

public class Element : IElement
{
    public UiScope Scope { get; }
    public Control Content { get; }

    internal Element(UiScope scope, Control content)
    {
        Scope = scope;
        Content = content;
        content.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(scope.RunMount);
        DisposeContent(content);
    }

    public Element(UiScope scope, IElement element) : this(scope, element.Content)
    {
    }

    public static Element Empty => new(new UiScope(), new Control());

    public void Dispose()
    {
        Scope.Dispose();
    }

    public static IElement Create(Func<UiScope, IElement> builder)
    {
        var uiScope = new UiScope();
        var element = uiScope.RunInScope(() => builder(uiScope));
        return element.Scope == uiScope
            ? element
            : new Element(uiScope, element.Content);
    }

    public static IElement Create(Func<IElement> builder)
    {
        return Create(_ => builder());
    }

    public static IElement Create<TProp>(Func<TProp, UiScope, IElement> builder, TProp props)
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

public class Element<TExpose> : Element, IElement<TExpose>
{
    public TExpose Expose { get; }

    internal Element(UiScope scope, Control content, TExpose expose) : base(scope, content)
    {
        Expose = expose;
    }

    public Element(UiScope scope, Element element, TExpose expose) : base(scope, element)
    {
        Expose = expose;
    }

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
            : new Element<TExpose>(uiScope, element.Content, element.Expose);
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

    public static IElement<TExpose> Create(
        Func<UiScope, IElement<TExpose>> builder,
        out IElement<TExpose> expose
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

    public static IElement<TExpose> Create(
        Func<IElement<TExpose>> builder,
        out IElement<TExpose> expose
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

// public class ChildElement : IElement
// {
//     public UiScope Scope { get; }
//     public Control Content { get; }
//     internal StackPanel? Stack { get; private init; }
//
//     public ChildElement(IElement element)
//     {
//         Scope = element.Scope;
//         Content = element.Content;
//     }
//
//     public void Dispose()
//     {
//         Scope.Dispose();
//     }
//
//     public ChildElement(Component comp)
//     {
//         var element = comp.Create();
//         Scope = element.Scope;
//         Content = element.Content;
//     }
//
//     public static implicit operator ChildElement(Component comp)
//     {
//         return new(comp);
//     }
//
//     public ChildElement(Func<IElement> fn)
//     {
//         var element = fn();
//         Scope = element.Scope;
//         Content = element.Content;
//     }
//
//     public static implicit operator ChildElement(Func<IElement> fn)
//     {
//         return new ChildElement(fn());
//     }
//
//     public ChildElement(IEnumerable<IElement> elements)
//     {
//         var uiScope = new UiScope();
//         var stack = new StackPanel();
//         foreach (var el in elements)
//         {
//             stack.Children.Add(el.Content);
//         }
//
//         var element = new Element(uiScope, stack);
//         Scope = element.Scope;
//         Content = element.Content;
//         Stack = stack;
//     }
//
//     public static implicit operator ChildElement(IElement[] elements)
//     {
//         return new ChildElement(elements);
//     }
//
//     public ChildElement(string text)
//     {
//         var uiScope = new UiScope();
//         var textBlock = new TextBlock
//         {
//             Text = text
//         };
//
//         var element = new Element(uiScope, textBlock);
//         Scope = element.Scope;
//         Content = element.Content;
//     }
//
//     public static implicit operator ChildElement(string text)
//     {
//         return new(text);
//     }
//
//     public ChildElement(IReadOnlySignal<string> text)
//     {
//         var uiScope = new UiScope();
//         var textBlock = new TextBlock();
//         uiScope.AddEffect(() => textBlock.Text = text.Value);
//
//         var element = new Element(uiScope, textBlock);
//         Scope = element.Scope;
//         Content = element.Content;
//     }
//
//     public static implicit operator ChildElement(Accessor<string> text)
//     {
//         return new ChildElement(text);
//     }
// }

public class Component
{
    private readonly Func<IElement> _create;

    public Component(Func<UiScope, IElement> builder)
        => _create = () => Element.Create(builder);

    public Component(Func<IElement> builder)
        => _create = () => Element.Create(builder);

    public IElement Create()
        => _create();

    public IElement CreateRef(out IElement expose)
    {
        expose = _create();
        return expose;
    }

    public IElement CreateRef(Action<IElement> fn)
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
        _create = props => Element.Create(builder, props);
    }

    public Component(Func<TProp, IElement> builder) : this((prop, _) => builder(prop))
    {
    }

    public IElement Create(TProp props)
    {
        return _create(props);
    }

    public IElement CreateRef(TProp props, out IElement expose)
    {
        expose = _create(props);
        return expose;
    }

    public IElement CreateRef(TProp props, Action<IElement> fn)
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

    public IElement<TExpose> CreateRef(TProp props, out IElement<TExpose> expose)
    {
        expose = _create(props);
        return expose;
    }

    public IElement<TExpose> CreateRef(TProp props, Action<IElement<TExpose>> fn)
    {
        var element = _create(props);
        fn(element);
        return element;
    }
}

// public class ChildComponent
// {
//     private readonly Component _comp;
//
//     private ChildComponent(Component comp)
//     {
//         _comp = comp;
//     }
//
//     public IElement Create() => _comp.Create();
//
//     public static implicit operator ChildComponent(Component comp)
//     {
//         return new(comp);
//     }
//
//     public static implicit operator ChildComponent(Func<IElement> fn)
//     {
//         return new ChildComponent(new Component(fn));
//     }
//
//     public static implicit operator ChildComponent(Element element)
//     {
//         return new ChildComponent(new Component(() => element));
//     }
//
//     public static implicit operator ChildComponent(IElement[] elements)
//     {
//         return new ChildComponent(new Component(uiScope =>
//         {
//             var stack = new StackPanel();
//             foreach (var el in elements)
//             {
//                 stack.Children.Add(el.Content);
//             }
//
//             return new Element(uiScope, stack);
//         }));
//     }
//
//     public static implicit operator ChildComponent(Accessor<string> text)
//     {
//         return new ChildComponent(new Component(uiScope =>
//         {
//             var textBlock = new TextBlock();
//             uiScope.AddEffect(() => { textBlock.Text = text.Value; });
//             return new Element(uiScope, textBlock);
//         }));
//     }
// }