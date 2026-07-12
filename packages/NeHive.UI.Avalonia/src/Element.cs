using Avalonia.Controls;
using Avalonia.Threading;
using NeHive.Model;

namespace NeHive.UI.Avalonia;

public interface IElement
{
    public UiScope? Scope { get; }
    public Control Content { get; }
    public void Dispose();
}

public interface IElement<out TExpose> : IElement
{
    public TExpose? Expose { get; }
}

public class Element : IElement
{
    private readonly Func<UiScope, Control> _builder;
    public UiScope? Scope { get; private set; }

    protected virtual Control Build(UiScope scope) => _builder(scope);

    public Control Content
    {
        get
        {
            if (field is not null) return field;
            var scope = new UiScope();
            Scope = scope;
            using (new ScopeFrame(scope))
            {
                field = Build(scope);
            }

            field.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(scope.RunMount);
            OnDisposeContent();
            return field;
        }
    }

    protected static Control ToControl(UiScope scope, IElement element)
    {
        var content = element.Content;
        if (element.Scope == scope) return content;
        return new Border { Child = content };
    }

    public void OnDisposeContent()
    {
        var control = Content;
        Scope?.OnCleanup += () =>
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

    protected Element(Func<UiScope, Control> builder)
    {
        _builder = builder;
    }

    public static Element Empty => new(_ => new Control());

    public void Dispose() => Scope?.Dispose();

    public static IElement WithScope(Func<UiScope, Control> builder)
        => new Element(builder);

    public static IElement WithScope(Func<UiScope, IElement> builder)
    {
        return new Element(scope =>
        {
            var el = builder(scope);
            return ToControl(scope, el);
        });
    }
}

public class Element<TExpose> : Element, IElement<TExpose>
{
    public TExpose? Expose { get; private set; }

    private readonly Func<UiScope, (TExpose, Control)> _builder;

    protected override Control Build(UiScope scope)
    {
        var (expose, control) = _builder(scope);
        Expose = expose;
        return control;
    }

    internal Element(Func<UiScope, (TExpose, Control)> builder) : base(scope => builder(scope).Item2)
        => _builder = builder;

    public static IElement<TExpose> WithScope(Func<UiScope, (TExpose, Control)> builder)
        => new Element<TExpose>(builder);

    public static IElement<TExpose> WithScope(Func<UiScope, (TExpose, IElement)> builder)
    {
        return new Element<TExpose>(scope =>
        {
            var (expose, element) = builder(scope);
            return (expose, ToControl(scope, element));
        });
    }
}