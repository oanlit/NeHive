[![Language](https://img.shields.io/badge/README-English-green.svg)](README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)

# NeHive.Reactive
A lightweight high-performance reactive runtime for .NET, built on a single-threaded dependency graph model with signal-driven fine-grained state updates.

This project is in early development (0.1.x series). APIs are subject to change.

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

## 1. Basic Reactivity

```csharp
using NeHive.Reactive;

using var scope = new Scope();

var count = new MutSignal<int>(0);

scope.CreateEffect(() =>
{
    Console.WriteLine(
        $"Count: {count.RxValue}");
});

count.RxValue = 1;
count.RxValue = 2;
```

Output:

```text
Count: 0
Count: 1
Count: 2
```

---

## 2. Computed State

Computed values cache automatically and only update when dependencies change.

```csharp
using var scope = new Scope();

var price = new MutSignal<double>(100);
var quantity = new MutSignal<int>(2);

var total = new Computed<double>(
    () => price.RxValue * quantity.RxValue);

scope.CreateEffect(() =>
{
    Console.WriteLine(
        $"Total: {total.RxValue}");
});

price.RxValue = 150;
quantity.RxValue = 5;
```

---

## 3. Async Reactive State

`AsyncMemo<T>` provides reactive async state with loading and error tracking.

```csharp
using var scope = new Scope();

var userId = new MutSignal<int>(1);

var user = scope.CreateAsyncMemo(async epoch =>
{
    var id = epoch.Pull(userId);

    await Task.Delay(500);

    return $"User-{id}";
});

scope.CreateEffect(() =>
{
    Console.WriteLine(
        $"Loading: {user.RxLoading}");

    Console.WriteLine(
        $"Value: {user.RxValue}");
});

userId.RxValue = 2;
```

---

## 4. Reactive Flow

ReactiveFlow provides lightweight stream-style operators.

```csharp
using var scope = new Scope();

var input = new MutSignal<int>(0);

var effect = scope
    .CreateReactiveFlow(input)
    .ThrottleLatest(500)
    .Filter(v => v > 3)
    .Map(v => v * 10)
    .PushEffect(v =>
    {
        Console.WriteLine(
            $"Output: {v}");
    });

for (var i = 0; i < 10; i++)
{
    input.RxValue = i;
    await Task.Delay(100);
}
```

---

## 5. Reactive Collections

```csharp
using var scope = new Scope();

var list = new ListStore<int>(
    [1, 2, 3]);

scope.CreateEffect(() =>
{
    Console.WriteLine(
        $"Count: {list.Count}");
});

scope.CreateEffect(() =>
{
    list.TryGetValue(0, out var value);

    Console.WriteLine(
        $"First: {value}");
});

list.BatchModify(items =>
{
    items[0] = 999;
    items.Add(4);
});

list.Sort();
list.Reverse();
```

---

# Core APIs

## Runtime Primitives

| API            | Description                             |
| -------------- | --------------------------------------- |
| `Scope`        | Reactive lifecycle container            |
| `Signal<T>`    | Readonly reactive state                 |
| `MutSignal<T>` | Mutable reactive state                  |
| `Computed<T>`  | Cached computed signal                  |
| `Effect`       | Reactive side effect                    |
| `AsyncMemo<T>` | Async reactive computed value           |
| `Selector<T>`  | Key-based selective dependency tracking |
| `Context<T>`   | Scoped dependency/context propagation   |

---

## Runtime Utilities

| API              | Description                             |
| ---------------- | --------------------------------------- |
| `Rx.Batch()`     | Batch signal updates                    |
| `Rx.Untrack()`   | Read values without dependency tracking |
| `Rx.OnDispose()` | Register cleanup callbacks              |

---

## ReactiveFlow Operators

| API                | Description                            |
| ------------------ | -------------------------------------- |
| `Filter()`         | Conditional filtering                  |
| `Map()`            | Value transformation                   |
| `Debounce()`       | Debounced updates                      |
| `ThrottleLatest()` | Throttled latest emission              |
| `PushEffect()`     | Terminal effect subscription           |
| `PushComputed()`   | Convert flow into computed state       |
| `PushAsyncMemo()`  | Convert flow into async reactive state |

## Reactive Collections

### `ListStore<T>`
Enhanced reactive list container.

Features:
- Fine-grained index tracking (each index is an independent signal)
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

## Interoperability

### `SignalObservable<T>`
Converts a `MutSignal<T>` into a standard `IObservable<T>` sequence.

```csharp
var signal = new MutSignal<int>(0);
var observable = new SignalObservable<int>(signal);
observable.Subscribe(value => Console.WriteLine(value));
signal.RxValue = 42; // Observer receives 42
```

### `Rx.From(IObservable<T>, T)`
Converts an external `IObservable<T>` into a reactive `ISignal<T>`.

### `StoreAttribute` / `NoSignalAttribute` / `ComputedAttribute`
Attribute-based reactive store generation markers.

---

## Interoperability

### SignalObservable<T>
Converts a MutSignal<T> into a standard IObservable<T> sequence.

### Rx.From(IObservable<T>, T)
Converts an external IObservable<T> into a reactive ISignal<T> with automatic lifecycle management.

### Array Diffing

| API                    | Description                                      |
| ---------------------- | ------------------------------------------------ |
| ArrayDiffUtil        | Compute minimal diffs between two lists           |
| ArrayMapResult       | Incremental array map with move preservation      |
| ArrayMapMemo         | Reactive signal for incremental array mapping     |

---

## Array Diffing

| API                    | Description                                      |
| ---------------------- | ------------------------------------------------ |
| ArrayDiffUtil        | Compute minimal diffs between two lists           |
| ArrayMapResult       | Incremental array map with move preservation      |
| ArrayMapMemo         | Reactive signal for incremental array mapping     |

---

## Inspirations

NeHive.Reactive is heavily inspired by the ideas and research behind:

* [SolidJS](https://github.com/solidjs/solid)
* [ReactiveX](https://github.com/dotnet/reactive)

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
MIT