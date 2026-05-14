# NeHive.Reactive
A lightweight high-performance reactive runtime for .NET, built on a single-threaded dependency graph model with signal-driven fine-grained state updates.

[![NuGet](https://img.shields.io/nuget/v/NeHive.Reactive.svg)](https://www.nuget.org/packages/NeHive.Reactive)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Target Framework](https://img.shields.io/badge/.NET-7.0%2F8.0%2F9.0%2F10.0-green)]()

## Overview
**NeHive.Reactive** is a lightweight high-performance single-threaded reactive runtime for .NET.

Built on a **scope dependency graph + signal propagation** architecture, it follows the fine-grained reactivity model popularized by SolidJS and Vue3, while remaining fully native to C#.

No reflection.  
No heavy runtime dependencies.  
Low GC overhead.

Designed for declarative UI systems, reactive state management, and business flow orchestration, it also serves as the core runtime engine of **NeHive.UI**.

### Core Design Principles
- 🧭 **Scope Isolation**  
  `Scope` manages reactive resources and lifecycles uniformly, preventing memory leaks caused by unmanaged subscriptions.

- ⚡ **Fine-Grained Reactivity**  
  Dependencies are tracked precisely. Only affected logic is updated instead of triggering full refreshes.

- 🧩 **Signal-Driven State**  
  `Signal` acts as the reactive state source with explicit and predictable data flow.

- 🌊 **Flow Composition**  
  Built-in reactive flow operators including debounce, throttle, filter, and mapping.

- 📦 **Reactive Collections**  
  Native reactive list support with fine-grained index tracking and batch updates.

- ✅ **Pure Native C#**  
  Modern C# syntax, nullable reference types, implicit usings, and multi-version .NET compatibility.

---

# Installation

## NuGet

```bash
Install-Package NeHive.Reactive
```

```bash
dotnet add package NeHive.Reactive
```

---

# Quick Start

## 1. Basic Signal + Effect Reactivity

```csharp
using NeHive.Reactive;

// Create scope
using var scope = new Scope();

// Define reactive state
var count = new MutSignal<int>(0);

// Automatically tracks dependencies
scope.CreateEffect(() =>
{
    Console.WriteLine($"Current Count: {count.RxValue}");
});

// Update state
count.RxValue = 10;
count.RxValue = 99;
```

---

## 2. Computed Values

Computed values are cached automatically and only recomputed when dependencies change.

```csharp
using var scope = new Scope();

var price = new MutSignal<double>(100);
var quantity = new MutSignal<int>(2);

// Computed value
var totalPrice = new Computed<double>(
    () => price.RxValue * quantity.RxValue);

scope.CreateEffect(() =>
{
    Console.WriteLine($"Total Price: {totalPrice.RxValue}");
});

price.RxValue = 199;
quantity.RxValue = 5;
```

---

## 3. Reactive Flow
Debounce, throttle, filtering, and mapping for high-frequency event streams.

```csharp
using var scope = new Scope();

var inputSignal = new MutSignal<int>(0);

var flow = scope.CreateReactiveFlow(inputSignal)
    .ThrottleLatest(500)
    .Filter(v => v > 5)
    .Map(v => v * 10)
    .PushEffect(val =>
    {
        Console.WriteLine(
            $"{DateTime.Now:HH:mm:ss.fff} Output: {val}");
    });

// Rapid updates
for (int i = 1; i <= 10; i++)
{
    inputSignal.RxValue = i;
    await Task.Delay(100);
}

await Task.Delay(1000);

flow.Dispose();
```

---

## 4. Reactive ListStore

Fine-grained tracking for item updates, insertion, removal, sorting, and batch modifications.

```csharp
using var scope = new Scope();

// Nullable reactive list
var listStore = new ListStore<int?>(
    [1, 2, 3, null, 5]);

// Observe count changes
scope.CreateEffect(() =>
{
    Console.WriteLine(
        $"List Count: {listStore.Count}");
});

// Observe specific index
scope.CreateEffect(() =>
{
    listStore.TryGetValue(0, out var first);

    Console.WriteLine(
        $"First Item: {first ?? -1}");
});

// Batch modification
listStore.BatchModify(list =>
{
    list[0] = 999;
    list.Add(666);
});

listStore.Reverse();
listStore.Sort();
listStore.Clear();
```

---

# Core APIs

## Core Runtime
- `Scope`  
  Reactive scope container and lifecycle manager

- `Signal<T>`  
  Readonly reactive state source

- `MutSignal<T>`  
  Mutable reactive state source

- `Computed<T>`  
  Cached readonly computed state

- `Effect`  
  Automatically tracked reactive side effects

- `AsyncMemo<T>`  
    Cached asynchronous reactive value with loading/error state management

---

## ReactiveFlow

Composable reactive stream operators.

- `Filter()`  
  Conditional filtering

- `Map()`  
  Data transformation

- `Debounce(ms)`  
  Debounce updates

- `ThrottleLatest(ms)`  
  Throttle and emit latest value

- `PushEffect()`  
  Subscribe and consume reactive flow

---

## Reactive Collections

### `ListStore<T>`
Enhanced reactive list container.

Features:
- Fine-grained index tracking
- `BatchModify` merged updates
- Native `Add` / `RemoveAt` / `Sort` / `Reverse` / `Clear`
- Built-in reactive LINQ-style queries

---

# Use Cases

1. Declarative cross-platform UI state management  
   (with NeHive.UI.Avalonia)

2. Lightweight desktop and mobile state-driven applications

3. Debounced forms, search suggestions, and high-frequency UI interactions

4. Business workflow orchestration and reactive dependency updates

5. Fine-grained reactive systems without heavy runtime overhead

---

# Ecosystem

- **NeHive.Reactive**  
  Reactive runtime engine

- **NeHive.UI.Avalonia**  
  Cross-platform declarative UI DSL built on Avalonia
    - Pure C# UI construction
    - No XAML
    - Tailwind-style string-based styling
    - Built-in control flow components:
      `ForEach`, `Show`, `Switch`, `Loading`

---

# Compatibility

- .NET 7.0 / 8.0 / 9.0 / 10.0
- Nullable reference types enabled
- Implicit usings supported
- Modern C# syntax
- Cross-platform compatible:
  Windows / macOS / Linux / WebAssembly / Mobile

---

# License

**MIT License**

Free for commercial and personal use.