using NeHive.Core;
using NeHive.UI.Avalonia.Components;

namespace NeHive.UI.Avalonia;

public class UiScope(Scope? parentOwner = null) : Scope(parentOwner)
{
    private readonly List<Action> _onMountQueue = [];
    private bool _mounted;

    public void OnMount(Action fn)
    {
        if (_mounted)
        {
            fn();
            return;
        }

        _onMountQueue.Add(fn);
    }

    public IElement RootElement(HStackPanelProp prop)
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