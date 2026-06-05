using NeHive.Reactive;
using NeHive.UI.Avalonia.Components;

namespace NeHive.UI.Avalonia;

public class UiScope(Scope? parentOwner = null) : Scope(parentOwner)
{
    private readonly List<Action> _onMountQueue = [];
    private bool _mounted;

    public event Action OnMount
    {
        add
        {
            if (_mounted)
            {
                value();
                return;
            }

            _onMountQueue.Add(value);
        }
        remove => _onMountQueue.Remove(value);
    }

    public IElement RootElement(RootProp prop)
        => BaseComponent.RootElement(prop, this);

    internal void RunMount()
    {
        if (_mounted) return;

        _mounted = true;

        foreach (var fn in _onMountQueue)
            fn();

        _onMountQueue.Clear();
    }
}
