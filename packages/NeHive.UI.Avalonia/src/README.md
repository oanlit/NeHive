# NeHive.UI.Avalonia

A pure C# declarative cross-platform UI framework built on **Avalonia UI**, deeply integrated with the NeHive.Reactive engine. It features an atomic styling system and a functional component model, enabling the development of high-performance cross-platform applications entirely without XAML.

> [!NOTE]
> This project is currently in its early stages of development (0.1.x series). The APIs are subject to change.

## Introduction

**NeHive.UI.Avalonia** is a **pure C# declarative UI framework** designed tailored for Avalonia.

Driven by the **NeHive.Reactive** fine-grained reactive runtime, it utilizes a component-based, functional UI approach. Combined with a built-in Tailwind-like atomic style parser and automated lifecycle management, it delivers a true **UI = f(State)** cross-platform development experience.

It perfectly supports Windows, macOS, Linux, embedded systems, and mobile platforms, offering a XAML-free, low-overhead, high-performance, and strongly type-safe workflow.

### Core Design Philosophy

* ⚡ **Deep Reactivity**: Fine-grained updates driven by Signals ensure components re-render precisely on demand, eliminating redundant view refreshes.
* 🧩 **Pure C# Components**: A functional component model that supports elegant composition, reuse, and scope isolation.
* 🎨 **Atomic Styling**: A Tailwind-inspired styling system supporting colors, spacing, layout, gradients, transforms, and transitions out of the box.
* 🧭 **Automated Lifecycle**: Managed via `UiScope` to automatically recycle resources, subscriptions, and events, completely eliminating memory leaks.
* 🔁 **Built-in Control Flows**: Declarative logical components like `ForEach`, `Show`, `Switch`, `Loading`, and `Match` are built straight into the UI layer.
* 📦 **Full Native Control Coverage**: Wraps all native Avalonia controls, layouts, and interaction capabilities seamlessly.
* 🌊 **Fluid State Orchestration**: Ready-to-use support for debouncing, throttling, asynchronous states, and two-way data binding.

## Installation

### NuGet

```bash
Install-Package NeHive.UI.Avalonia

```

```bash
dotnet add package NeHive.UI.Avalonia

```

# Quick Start

## 1. Basic Reactive Component

```csharp
using NeHive.Reactive;
using NeHive.UI.Avalonia;
using static NeHive.UI.Avalonia.Components.BaseComponent;

public static class Demo
{
    private static IElement CounterComp(UiScope uiScope)
    {
        var count = new MutSignal<int>(0);

        return uiScope.RootElement(new()
        {
            HTextBlock(() => $"Count: {count.RxValue}", strStyle: "text-xl fg-blue-600"),
            
            HStackPanel(new(strStyle: "mt-2 gap-2 horizontal"))
            {
                HButton("+1", onClick: _ => count.RxValue++),
                HButton("-1", onClick: _ => count.RxValue--)
            }
        });
    }

    public static IElement Counter() => Element.WithScope(CounterComp);
}

```

## 2. List Rendering (ForEach)

```csharp
var items = new MutSignal<IReadOnlyList<int>>([1, 2, 3]);

return RootElement(new()
{
    ForEach<int>(new(items))
    {
        ItemTemplate = (item, index) => 
            HTextBlock(() => $"Item {index.RxValue}: {item}")
    }
});

```

## 3. Conditional Rendering (Show)

```csharp
var visible = new MutSignal<bool>(true);

return RootElement(new()
{
    Show(new(visible))
    {
        IfTrue = () => HTextBlock("Visible Content"),
        IfFalse = () => HTextBlock("Hidden Content")
    }
});

```

## 4. Multi-branch Evaluation (Switch)

```csharp
var tab = new MutSignal<int>(0);

return RootElement(new()
{
    Switch<int>(new(tab))
    {
        [0] = () => HTextBlock("Tab 1"),
        [1] = () => HTextBlock("Tab 2"),
        Default = () => HTextBlock("Default")
    }
});

```

## 5. Pattern Matching (Match)

```csharp
var volume = new MutSignal<int>(30);

return RootElement(new()
{
    Match<int>(new(volume))
    {
        [v => v == 0] = () => HSvgImage("volume-mute.svg"),
        [v => v < 50] = () => HSvgImage("volume-low.svg"),
        Default = () => HSvgImage("volume-high.svg")
    }
});

```

## 6. Asynchronous Loading State (Loading)

```csharp
var userInfo = uiScope.CreateAsyncMemo(async _ =>
{
    await Task.Delay(500);
    return new User(1, "Test");
});

return Loading<User>(new(userInfo))
{
    Loading = _ => HTextBlock("Loading..."),
    Success = user => HTextBlock($"Hello {user.Name}"),
    Error = ex => HTextBlock($"Error: {ex.Message}")
};

```

# Control Flow Components Overview

| Component | Purpose |
| --- | --- |
| `Show` | Simple conditional visibility toggles |
| `Switch` | Multi-branch evaluation for concrete values |
| `Match` | Expression/predicate-based pattern matching |
| `ForEach` | Fine-grained reactive list rendering |
| `Loading` | Three-state asynchronous lifecycle management |

# Layouts & Controls

## Layout Containers

* `HStackPanel`
* `HGrid`
* `HAbsolute`
* `HDockPanel`
* `HWrapPanel`
* `HUniformGrid`
* `HSplitView` / `HSplitPanel`
* `HScrollViewer`

## Base Controls

* `HTextBlock`
* `HButton` / `HContentButton`
* `HTextBox` (Supports built-in two-way binding)
* `HCheckBox` / `HRadioButton` / `HToggleSwitch`
* `HSlider` / `HProgressBar`
* `HListBox` / `HComboBox` / `HTreeView`
* `HTabControl`
* `HUriImage` / `HSvgImage`
* `HFilePicker` / `HFolderPicker`

# Atomic Styling System

Supports Tailwind-like atomic utility string formatting, which is automatically parsed into Avalonia Styles:

* **Spacing**: `m-2` `p-3` `mt-4` `gap-x-2`
* **Sizing**: `w-20` `h-12` `max-w-60`
* **Colors**: `fg-blue-500` `bg-gray-100` `border-red-300`
* **Gradients**: `bg-gradient-tl from-blue to-purple`
* **Borders & Radii**: `rounded-lg` `border-w-1`
* **Typography**: `text-lg` `font-bold` `text-center`
* **Alignment**: `center` `justify-center` `items-center`
* **Transforms**: `scale-110` `translate-y-1` `rotate-3`
* **Transitions**: `transition-colors duration-300 ease-out`
* **Interactions**: `hover:opacity-80` `click:scale-95`

# Target Scenarios

* Cross-platform desktop applications (Windows / macOS / Linux)
* Embedded systems / Industrial control HMI & UIs
* Tooling software and sophisticated configuration dashboards
* Data visualizations, continuous scrolling lists, and heavy dynamic form systems
* Multimedia players and audio/video control panels
* High-efficiency .NET projects aiming for complete freedom from XAML

# Interoperability

* Fully compatible with native Avalonia controls.
* Seamlessly bound to the NeHive.Reactive ecosystem.
* Interoperable conversion between `Signal` and `IObservable`.
* Mix-and-match capabilities with native Styles and third-party UI libraries.

## Inspiration & Acknowledgments

* [Avalonia UI](https://www.google.com/search?q=https://github.com/AvaloniaUI/Avalonia)
* [Tailwind CSS](https://www.google.com/search?q=https://github.com/tailwindlabs/tailwindcss)
* [SolidJS](https://github.com/solidjs/solid) / [Vue3](https://github.com/vuejs/core) (Fine-grained reactivity paradigms)
* `NeHive.Reactive`

## License

MIT
