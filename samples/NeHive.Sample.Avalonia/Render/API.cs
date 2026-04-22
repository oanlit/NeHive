using NeHive.Core;
using System;
using System.Collections.Generic;

namespace NeHive.Sample.Avalonia.Render;

public class UiScope(Scope? parentOwner = null) : Scope(parentOwner)
{
    private readonly List<Action> _onMountQueue = [];
    private bool _mounted;

    public void OnMount(Action fn)
    {
        if (_mounted)
        {
            // 已经 mounted 直接执行（符合语义）
            fn();
            return;
        }

        _onMountQueue.Add(fn);
    }

    internal void RunMount()
    {
        if (_mounted) return;

        _mounted = true;

        foreach (var fn in _onMountQueue)
            fn();

        _onMountQueue.Clear();
    }
}