![Banner](docs/img/Banner.png)

<p align="center">
  <a href="README.md"><img src="https://img.shields.io/badge/Readme-English-blue" alt="English"></a>
  <a href="docs/README-zh.md"><img src="https://img.shields.io/badge/Readme-简体中文-green" alt="简体中文"></a>
  <a href="https://www.nuget.org/packages/NeHive.Reactive">
    <img src="https://img.shields.io/nuget/v/NeHive.Reactive" alt="NuGet Version">
    <img src="https://img.shields.io/nuget/dt/NeHive.Reactive" alt="NuGet Downloads">
  </a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-orange" alt="MIT License"></a>
  <img src="https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-purple" alt="Target Framework">
  <a href="https://github.com/oanlit/NeHive/stargazers">
    <img src="https://img.shields.io/github/stars/oanlit/NeHive?style=social" alt="Stars">
  </a>
</p>

# ⚡ What is NeHive

NeHive is a:

✔ Framework for writing UI directly using C# functions

✔ Fine-grained reactive system inspired by SolidJS / Vue3

✔ UI that automatically tracks Signal dependencies and updates precisely

✔ Solution that requires NO XAML / INotifyPropertyChanged

Core philosophy:

$$UI = f(State)$$

---

# 🧩 Have You Ever Been Troubled by These Pain Points?

In traditional .NET UI development (WPF / Avalonia / WinUI), we are often forced to accept these "black box" overheads:

* **Bloated MVVM Glue Code**: Tedious layer separation, disconnection between state and view, and a massive amount of repetitive boilerplate code.
* **Cumbersome INotifyPropertyChanged**: Manually triggering notifications, or relying heavily on heavy source generators.
* **Hard-to-Debug XAML Bindings**: Runtime binding failures only output logs, making it impossible to trace issues with breakpoints.
* **Uncontrollable UI Update Granularity**: Relying on implicit framework refreshes, with no way to achieve precise and efficient partial rendering.
* **Severe Concept Fragmentation**: Independent concepts like Templates, Triggers, Converters, State Machines, Attached Properties, Commands, and Value Converters lead to an inconsistent mental model, keeping learning and maintenance costs high.
* **Syntax Disconnection**: Huge gaps between markup languages and backend syntax prevent the flexible use of native C# capabilities to build dynamic UIs.
* **Verbose Style Syntax**: Native style attributes are complex; code quickly becomes bloated and hard to reuse when combining multiple styles.

---

# 💡 NeHive Aims to Change All of This

NeHive is not just another wheel; it is a brand-new philosophy for building UI. We firmly believe that:

> **UI = f(State)**

* **Unified Mental Model**: The entire framework is built around three core primitives: **Scope (lifecycle), Signal (reactive state), and Effect (side effect)**. All capabilities are composed of these three, keeping the concept minimal.
* **State as a Signal**: Declarative reactive states where changes are fully trackable and predictable.
* **UI as a Pure Function**: Views map directly to states with a clear and intuitive structure, written entirely in **native C# syntax**.
* **Precise Targeted Updates**: Automatic dependency collection ensures that state changes only update the corresponding atomic nodes.
* **Explicit Lifecycle**: Managed via a Scope Tree to prevent memory leaks right at the architectural level.
* **Syntax and Paradigm Freedom**: Features like templates, triggers, and data conversion are implemented using C# expressions + reactivity, **eliminating the need to write and maintain dedicated converters**. Declarative and imperative paradigms can be mixed freely without learning extra syntax.
* **Atomic Styling System**: Features a built-in Tailwind-style `strStyle`. It uses concise utility classes to combine styles, saying goodbye to verbose native styling code while making writing highly efficient and reusable.
* **Extremely Low Learning Curve**: Master the three core concepts, and you master all the functionalities of the framework.

---

# 🚀 Hello World

The minimal UI component in NeHive looks like this:

```csharp
public static IElement Counter()
{
    var count = new MutSignal<int>(0);

    return HStackPanel(new()
    {
        HTextBlock(
            () => $"Count: {count.RxValue}",
            strStyle: "text-xl"
        ),

        HButton(
            "Add",
            onClick: _ => count.RxValue++
        )
    });
}

```

## 🧠 Behavior Explanation

This component demonstrates the fundamental reactive mechanism of NeHive:

* `count` is a Signal (reactive state).
* `HTextBlock(() => ...)` automatically subscribes to `count.RxValue`.
* When `count.RxValue` changes:
  → Only the text node that depends on it will update.
  → The UI will NOT rebuild as a whole.
* The click event of `HButton` directly mutates the Signal state.

Final Effect: Click the button → Count updates in real-time.

---

## 🎨 Code Style Explanation

NeHive's UI code features the following characteristics:

* **Functions as UI**: Components are essentially pure functions that return an `IElement`.
* **Localized State**: State is declared directly inside the component, removing the need for a ViewModel.
* **Implicit Reactivity**: No manual subscriptions or update notifications required.
* **Declarative Structure**: The UI structure maps directly to the code layout.
* **Low Abstraction Layer**: No intermediate layers like XAML / Binding / VM.

Simply put:

> UI = f(State), where state changes automatically drive view updates.

# 🔭 Core Architecture

NeHive unifies three core capabilities, forming a complete ecosystem from the low-level runtime to the top-level UI:

```text
 ┌─────────────────────────────┐
 │     NeHive.UI.Avalonia      │  <-- Avalonia-based declarative functional UI framework
 └──────────────┬──────────────┘
                │ (Depends on)
 ┌──────────────▼──────────────┐
 │       NeHive.Reactive       │  <-- Fine-grained reactive runtime (Signal/Effect)
 └──────────────┬──────────────┘
                │ (Depends on)
 ┌──────────────▼──────────────┐
 │        NeHive.Model         │  <-- Runtime semantic model & lifecycle management (Scope)
 └─────────────────────────────┘

```

* **NeHive.Model**: The runtime semantic model layer. It provides `Scope`, `Context`, and lifecycle management.
* **NeHive.Reactive**: The fine-grained reactive runtime core. It provides core reactive primitives like `Signal`, `Computed`, `Effect`, `AsyncMemo`, `ReactiveFlow`, and `ListStore`.
* **NeHive.UI.Avalonia**: The top-level functional UI implementation based on Avalonia. It provides functional components, control flow components, an atomic styling system, and reactive drivers.

# 🚀 Quick Start (Recommended Entry)

👉 **The best way to understand it is not by reading the docs, but by running it!**

## ▶ Run the Full Music Player Example

```bash
git clone https://github.com/oanlit/NeHive.git
cd samples/NeHive.Sample.Avalonia
dotnet run
```

## 🎯 What You Will Experience:

* 🧩 **Functional Component UI**: Completely detached from XAML, enjoying ultimate compile-time type safety and hot-reload potential.
* ⚡ **Control Flow Components**: Native support for reactive control flow UIs such as `Switch` / `ForEach` / `Match`.
* 🌳 **Scope Tree Lifecycle**: Experience the elegance of states and side effects (Effects) being automatically and safely released when their scope is destroyed.

![img](/docs/img/Sample.gif)

---

# 🌱 Engineering Practice Appreciation

In NeHive, component design completely returns to the declarative pure C# function paradigm of **UI = f(State)**. Without writing verbose XAML and heavy base class inheritance chains, logic and views become highly cohesive.

To meet business scenarios of varying complexities, NeHive refines components into two core engineering paradigms:

## 🛠️ Paradigm 1: Scopeless Components (e.g., Audio)

Suitable for components that **do not need to manage their own lifecycle or host underlying unmanaged resources**.

* 💡 **Data-Driven Mechanism**: The component can receive ordinary static data (like the `SongInfo` struct below) just like a regular C# function, rendering it as a local view snapshot for a single run. If dynamic reactive updates for parameters are needed, upstream callers can either explicitly pass a `Signal` object or pass a Lambda closure to trigger topological tracking internally via `.RxValue`.

```csharp
// Scopeless Component: Ready to use on the fly; data is driven entirely by external parameters, without intervening in lifecycle management.
private static IElement Audio(SongInfo? song)
{
    return HStackPanel(new(strStyle: "w-110 gap-4 horizontal items-center")
    {
        Show(new(song?.CoverPath is not null)
        {
            IfFalse = () => HButton(strStyle: "w-32 h-32 bg-gray-200 rounded-xl"),
            IfTrue = () => HUriImage(song?.CoverPath, stretch: Stretch.UniformToFill, strStyle: "...")
        }),

        HStackPanel(new(strStyle: "gap-1 vertical")
        {
            // Static passing, ideal for a single-rendering snapshot
            HTextBlock(song?.Title ?? "Unknown Title", strStyle: "max-w-100 text-base font-semibold fg-gray-800"),
            // To support reactive updates, you can pass a closure (assuming the parameter is changed to a closure or Signal)
            // HTextBlock(() => currentSong.RxValue?.Title ?? "Unknown Title", ...)
            HTextBlock(song?.Artist ?? "Unknown Artist", strStyle: "max-w-75 text-sm fg-gray-500"),
            HTextBlock(song?.Album ?? "Unknown Album", strStyle: "max-w-50 text-xs fg-gray-500")
        })
    });
}

```

## 🌳 Paradigm 2: Scoped Components with Lifecycles (e.g., CorePlayer)

When a component **needs to manage its own lifecycle or take over the lifecycle of underlying native resources** (such as initializing/disposing of an unmanaged engine or mounting asynchronous network requests), a `UiScope` is injected via `Element.WithScope` for unified hosting. The lifecycle topology within the component will be fully mounted to the scope tree, ensuring predictable behavior and resource safety.

```csharp
// Scoped Component: Features an independent runtime context and explicit lifecycle control
public static IElement CorePlayer() => Element.WithScope(uiScope => 
{
    // 1. Initialize local states and engines belonging to this component's lifecycle (using scope to manage resources)
    var (libVlc, mediaPlayer) = uiScope.CreateMediaPlayer();
    var playlist = new MutSignal<IReadOnlyList<TrackInfo>>([]);
    var currentIndex = new MutSignal<int>(-1);

    // 2. Declare local asynchronous derived streams and side effects (lifecycle automatically and safely releases upon uiScope destruction)
    var songInfo = uiScope.CreateAsyncMemo(async epochScope => { /* ... */ });
    uiScope.CreateEffect(epochScope => { /* ... */ });

    // 3. Return the declarative UI topology structure
    return rootElement;
});

```

---

The following production-ready code snippets are taken from the **Music Player** sample project, demonstrating how NeHive provides rigorous and highly cohesive engineering solutions when facing complex, highly interactive desktop industrial scenarios:

## 1️⃣ Co-indexing the Lifecycles of Resources and UI (Safe Leak Prevention)

By leveraging `Scope` runtime semantics, the disposal logic of underlying unmanaged resources (such as `MediaPlayer`) can perfectly co-exist within the UI component's lifecycle. When a UI component is unmounted due to routing switches or conditional rendering, its associated underlying native resources automatically trigger `Dispose()`, preventing common memory leaks from an architectural standpoint.

```csharp
public static (LibVLC, MediaPlayer) CreateMediaPlayer(this Scope scope)
{
    var libVlc = new LibVLC();
    var mediaPlayer = new MediaPlayer(libVlc);

    // When the UI scope is destroyed, underlying unmanaged resources are automatically disposed of synchronously
    scope.OnCleanup += () => {
        mediaPlayer.Dispose();
        libVlc.Dispose();
    };
    return (libVlc, mediaPlayer);
}

```

## 2️⃣ Safe Synchronization of Cross-Thread State

Low-level events from multimedia engines (like time updates `TimeChanged`) usually run on background worker threads. NeHive allows configuring an `onSet` interceptor during `Signal` initialization, which automatically redirects data back to the UI dispatcher thread at the source. This ensures thread safety while keeping upstream business logic clean.

```csharp
var position = new MutSignal<TimeSpan>(TimeSpan.Zero,
    onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));

```

## 3️⃣ Declarative Asynchronous Business Flows (`AsyncMemo`)

Extracting multimedia metadata (such as parsing large file covers or reading ID3 tags) requires asynchronous IO. NeHive provides the `AsyncMemo` infrastructure, allowing developers to write asynchronous derived states using a synchronous, declarative topological network, and effortlessly hook them up to the UI's loading states.

```csharp
var songInfo = uiScope.CreateAsyncMemo<SongInfo?>(async epochScope => {
    var index = epochScope.Pull(currentIndex); // Automatically subscribes to and tracks changes in the current playback index
    if (index < 0) return null;
    
    var media = new Media(libVlc, playlist.Value[index].FilePath);
    await media.Parse(timeout: 5000); // Non-blocking asynchronous parsing
    return new SongInfo(media.Meta(MetadataType.Title), ...);
});

// Explicitly consume each state branch of the asynchronous stream in the UI topology (Success, Loading, Error)
Loading<SongInfo?>(new(songInfo)
{
    Success = song => Audio(song),
    Loading = () => Audio(null),
    Error = ex => HTextBlock($"Failed to load: {ex.Message}")
});

```

## 4️⃣ High-Performance, Atomic Local Updates

NeHive discards heavy global redrawing mechanisms. Through built-in control flow primitives, style toggles and conditional branches map directly to fine-grained topologies. **When `currentIndex` changes, only the attributes of the corresponding list item will trigger a targeted redraw**, ensuring exceptionally high UI rendering performance.

```csharp
ForEach<TrackInfo>(new(playlist)
{
    ItemTemplate = (track, index) =>
        HButton(track.Title,
            strStyle: new(() => {
                var isActive = index.RxValue == currentIndex.RxValue; // Automatically tracks the currently active item
                return $"px-3 py-2 {(isActive ? "bg-sky-100 fg-sky-800" : "bg-transparent")}";
            }),
            onClick: _ => currentIndex.RxValue = index.Value
        )
});

```

---

# 📋 Paradigm Comparison

| Feature | Traditional MVVM (WPF / Traditional Avalonia) | The NeHive Paradigm |
| --- | --- | --- |
| **UI Expression** | XAML (Markup language, separated logic) | Pure C# Functional DSL (Declarative code as UI) |
| **State Responsiveness** | `Binding` + `INotifyPropertyChanged` | `Signal` automatic dependency tracking & topological updates |
| **Update Granularity** | Component-level / DataContext-level redraws | **Fine-grained, precise updates at the atomic node level** |
| **Debugging Experience** | Runtime black-box; XAML binding errors are hard to debug | Pure C# code; clear call stack; supports direct breakpoints |
| **Core Mental Model** | Fragmented concepts: Templates, Triggers, Attached Properties, Commands, Converters, etc. | **Unified model: Scope + Signal + Effect**; everything is built on three primitives |
| **Syntax System** | Markup language + C# double syntax; split codebase | Pure native C#, where templates and triggers equal anonymous functions + reactivity |
| **Data Conversion** | Relies on separate Value Converters that must be written, registered, and maintained | Native anonymous functions complete conversions directly with zero extra converter overhead |
| **Styling System** | Native attribute writing is verbose; combinations are complex and hard to reuse | Tailwind-like atomic `strStyle`; concise, flexible, and easy to reuse |
| **Learning Curve** | Numerous concepts and complex rules; high entry barrier | Minimal concepts; mastering the three core primitives covers everything easily |
| **Building Paradigm** | Forced separation between declarative and imperative styles | Free mixing of declarative and imperative styles to adapt to different business scenarios |

---

# 📦 Project Module Status

| Module | Description | Status |
| --- | --- | --- |
| `package/NeHive.Model` | Runtime semantic model layer (Scope / Context) | Preview |
| `package/NeHive.Reactive` | Fine-grained reactive core (Signal / Effect / Flow) | Preview |
| `package/NeHive.UI.Avalonia` | Functional UI declarative top-level implementation | Experimental |
| `package/NeHive.Generator` | Source generators to improve development experience | In Development |
| `samples` | Collection of sample projects (includes full MusicPlayer implementation) | Actively Maintained |

## 🔧 Install NuGet Packages

You can directly install the latest preview versions via NuGet:

```bash
# Reactive Core (Required)
dotnet add package NeHive.Reactive --prerelease

# Avalonia UI Integration (For building interfaces)
dotnet add package NeHive.UI.Avalonia --prerelease

```

Or search and install via the **NuGet Package Manager** in Visual Studio:

* `NeHive.Reactive`
* `NeHive.UI.Avalonia`

> Note: All packages are currently in **Preview**. Please check **"Include prerelease"** when installing.

---

# 🎯 Target Scenarios & Design Boundaries

## If you are:

* A .NET developer longing for the ultimate development experience similar to frontend SolidJS / React.
* Building tools, editors, or dashboard applications driven by complex interactions and high-frequency states.
* Trying to break free from complex MVVM architectures to pursue highly cohesive, easy-to-debug codebases.

## Please note, NeHive is NOT:

* ❌ A drop-in tweak replacement for traditional MVVM frameworks.
* ❌ Pure syntactic sugar made just to replace XAML (it includes a comprehensive reactive runtime architecture).

---

> [!WARNING]
> NeHive is currently in an early evolutionary stage, and the APIs are frequently iterating and optimizing. Direct use in production environments is not recommended yet. Any constructive Issues and PRs are highly welcome!

---

# 📄 License

This project is open-sourced under the [MIT](LICENSE) License.
