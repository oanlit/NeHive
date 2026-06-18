using Avalonia;
using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia;

public static class NeHiveUiContext
{
    public static readonly ContextKey<Window> Window = new();
    public static readonly ContextKey<Signal<Size>> WindowSize = new();
}