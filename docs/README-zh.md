![Banner](./img/Banner.png)

<p align="center">
  <a href="../README.md"><img src="https://img.shields.io/badge/Readme-English-blue" alt="English"></a>
  <a href="README-zh.md"><img src="https://img.shields.io/badge/Readme-简体中文-green" alt="简体中文"></a>
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

# ⚡ 什么是NeHive

NeHive 是一个：

✔ 用 C# 函数直接写 UI 的框架  
✔ 类似 SolidJS / Vue3 的细粒度响应式系统  
✔ UI 自动追踪 Signal 依赖并精准更新  
✔ 不需要 XAML / INotifyPropertyChanged

核心理念：

$$UI = f(State)$$

---

# 🧩 你是否也曾为这些痛点苦恼？

在传统的 .NET UI 开发（WPF / Avalonia / WinUI）中，我们被迫接受了这些暗箱操作：

*  **MVVM 胶水代码臃肿**：层级拆分繁琐，状态与视图割裂，大量重复模板代码
*  **INotifyPropertyChanged 繁琐**：手动触发通知，或依赖重型源码生成器
*  **XAML 绑定难以调试**：运行时绑定失败仅输出日志，无法断点追踪问题
*  **UI 更新粒度不可控**：依赖框架隐式刷新，无法实现精准高效的局部渲染
*  **概念碎片化严重**：模板、触发器、转换器、状态机、附加属性、命令、值转换器等概念各自独立，心智模型不统一，学习与维护成本居高不下
*  **语法割裂**：标记语言与后端语法差异大，无法使用原生 C# 能力灵活构建动态 UI
*  **样式写法冗长**：原生样式属性繁杂，多样式组合时代码臃肿，复用性差

---

# 💡 NeHive 试图改变这一切

NeHive 并不是另一个轮子框架，而是一种全新的 UI 构建思想。我们坚信：

> **UI = f(State)**

* **统一心智模型**：全框架围绕 **Scope（生命周期）、Signal（响应式状态）、Effect（副作用）** 三大核心原语构建，所有能力均由三者组合实现，概念极简
* **状态即 Signal**：声明式响应式状态，变化可追踪、可预测
* **UI 即纯函数**：视图是状态的直接映射，结构清晰直观，全程使用**原生 C# 语法**
* **精准靶向更新**：依赖自动收集，状态变化仅更新对应原子节点
* **显式生命周期**：基于 Scope Tree 托管，从架构层面避免内存泄漏
* **语法与范式自由**：模板、触发器、数据转换等能力均由C#表达式+响应式实现，**无需额外编写和维护专属转换器**；声明式、命令式可自由混用，无需学习额外语法
* **原子化样式体系**：内置类 Tailwind 风格的 `strStyle`，采用简洁工具类写法组合样式，告别冗长原生样式代码，书写高效、便于复用
* **极低学习成本**：掌握三大核心概念，即可驾驭全框架所有功能

---

# 🚀 Hello World

NeHive 的最小 UI 组件如下：

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

## 🧠 行为说明

这个组件展示了 NeHive 的基本响应式机制：

* `count` 是一个 Signal（响应式状态）
* `HTextBlock(() => ...)` 会自动订阅 `count.RxValue`
* 当 `count.RxValue` 发生变化时：
  → 只有依赖它的文本节点会更新
  → UI 不会整体重建
* `HButton` 的点击事件直接修改 Signal 状态

最终效果：点击按钮 → Count 实时更新

---

## 🎨 代码风格说明

NeHive 的 UI 代码具有以下特点：

* **函数即 UI**：组件本质是返回 `IElement` 的纯函数
* **状态局部化**：状态直接在组件内部声明，无需 ViewModel
* **响应式隐式化**：不需要手动订阅或通知更新
* **声明式结构**：UI 结构直接对应代码结构
* **低抽象层级**：没有 XAML / Binding / VM 中间层

可以简单理解为：

> UI = f(State)，状态变化自动驱动视图更新

# 🔭 核心架构

NeHive 统一了三类核心能力，形成了从底层运行时到顶层 UI 的完整生态：

```text
 ┌─────────────────────────────┐
 │     NeHive.UI.Avalonia      │  <-- 基于 Avalonia 的声明式函数 UI 框架
 └──────────────┬──────────────┘
                │ (依赖)
 ┌──────────────▼──────────────┐
 │       NeHive.Reactive       │  <-- 细粒度响应式运行时 (Signal/Effect)
 └──────────────┬──────────────┘
                │ (依赖)
 ┌──────────────▼──────────────┐
 │        NeHive.Model         │  <-- 运行时语义模型与生命周期管理 (Scope)
 └─────────────────────────────┘

```

* **NeHive.Model**：运行时语义模型层。提供 `Scope`、`Context` 及生命周期管理。
* **NeHive.Reactive**：细粒度响应式运行时。提供 `Signal`、`Computed`、`Effect`、`AsyncMemo`、`ReactiveFlow`、`ListStore` 等核心响应式原语。
* **NeHive.UI.Avalonia**：基于 Avalonia 的函数式 UI 顶层实现。提供函数组件、控制流组件、原子样式系统及响应式驱动。

# 🚀 快速体验（推荐入口）

👉 **最好的理解方式不是阅读文档，而是直接运行它！**

> [!IMPORTANT]
> 示例项目需要安装 **.NET 8 或更高版本运行时/SDK**。
>
> 当前已支持：
> - .NET 8
> - .NET 9
> - .NET 10
>
> 请确保本地环境已安装对应版本后再运行示例。

## ▶ 运行完整音乐播放器示例

```bash
git clone https://github.com/oanlit/NeHive.git
cd NeHive/samples/NeHive.Sample.Avalonia
dotnet run
```

## 🎯 你将体验到：

* 🧩 **函数组件 UI**：完全脱离 XAML，享受极致的编译期类型安全与热重载潜力。
* ⚡ **控制流组件**：原生支持 `Switch` / `ForEach` / `Match` 等响应式控制流 UI。
* 🌳 **Scope 树生命周期**：体验状态、副作用（Effect）随作用域销毁而自动释放的优雅。

![img](/docs/img/Sample.gif)

---

# 🌱 工程实践鉴赏

在 NeHive 中，组件的设计彻底回归了 **UI = f(State)** 的声明式纯 C# 函数范式。无需编写冗长的 XAML 与繁重的基类继承链，逻辑与视图得以高度内聚。

为了满足不同复杂度的业务场景，NeHive 将组件提炼为两种核心工程范式：

## 🛠️ 范式一：无作用域组件 (如 Audio)
适用于**无需管理自身生命周期、无需托管底层非托管资源**的组件形态。
* 💡 **数据驱动机制**：组件可以像常规 C# 函数一样接收普通静态数据（如下文中的 `SongInfo` 结构体），作为局部视图快照进行单次渲染；若需要实现参数的动态响应式更新，上层调用者既可以显式传入 `Signal` 对象，也可以传入 Lambda 闭包并在内部通过 `.RxValue` 触发拓扑追踪。
```csharp
// 无作用域组件：随调随用，数据完全由外部参数驱动，不介入生命周期管理
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
            // 静态传入，适合单次渲染的快照
            HTextBlock(song?.Title ?? "未知标题", strStyle: "max-w-100 text-base font-semibold fg-gray-800"),
            // 若要支持响应式更新，可传入闭包（假设入参改为了闭包或 Signal）
            // HTextBlock(() => currentSong.RxValue?.Title ?? "未知标题", ...)
            HTextBlock(song?.Artist ?? "未知歌手", strStyle: "max-w-75 text-sm fg-gray-500"),
            HTextBlock(song?.Album ?? "未知专辑", strStyle: "max-w-50 text-xs fg-gray-500")
        })
    });
}

```

## 🌳 范式二：有生命周期的“作用域组件” (如 CorePlayer)

当组件**需要管理自身生命周期、或者接管底层原生资源生命周期**（如初始化与销毁非托管引擎、挂载异步网络请求）时，通过 `Element.WithScope` 注入 `UiScope` 统一托管。组件内的生命周期拓扑将完全挂载到作用域树中，确保行为的可预测性与资源安全性。

```csharp
// 作用域组件：具备独立的运行时上下文与显式生命周期控制
public static IElement CorePlayer() => Element.WithScope(uiScope => 
{
    // 1. 初始化属于该组件生命周期的局部状态与引擎（利用 scope 托管资源）
    var (libVlc, mediaPlayer) = uiScope.CreateMediaPlayer();
    var playlist = new MutSignal<IReadOnlyList<TrackInfo>>([]);
    var currentIndex = new MutSignal<int>(-1);

    // 2. 声明局部异步派生流与副作用（生命周期随 uiScope 销毁而自动安全释放）
    var songInfo = uiScope.CreateAsyncMemo(async epochScope => { /* ... */ });
    uiScope.CreateEffect(epochScope => { /* ... */ });

    // 3. 返回声明式 UI 拓扑结构
    return rootElement;
});

```

---

以下生产力代码片段截取自示例项目的**音乐播放器**，展示了 NeHive 在面对复杂、高交互的桌面端工业场景时，如何提供严谨且高内聚的工程解法：

## 1️⃣ 资源与 UI 生命周期同生共死 (资源安全防泄漏)

利用 `Scope` 运行时语义，底层非托管资源（如 `MediaPlayer`）的释放逻辑可以与 UI 组件的生命周期完美内聚。当 UI 组件由于路由切换或条件卸载时，关联的底层原生资源会自动触发 `Dispose()`，从架构层面避免了常见的内存泄漏。

```csharp
public static (LibVLC, MediaPlayer) CreateMediaPlayer(this Scope scope)
{
    var libVlc = new LibVLC();
    var mediaPlayer = new MediaPlayer(libVlc);

    // 当 UI 作用域被销毁时，底层非托管资源同步自动释放
    scope.OnCleanup += () => {
        mediaPlayer.Dispose();
        libVlc.Dispose();
    };
    return (libVlc, mediaPlayer);
}

```

## 2️⃣ 跨线程状态的安全同步

多媒体引擎的底层事件（如时间更新 `TimeChanged`）通常运行在背景工作线程中。NeHive 允许在 `Signal` 初始化时配置 `onSet` 拦截器，使数据在源头自动回归 UI 调度线程，确保了线程安全，同时让上层业务逻辑保持纯净。

```csharp
var position = new MutSignal<TimeSpan>(TimeSpan.Zero,
    onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));

```

## 3️⃣ 声明式异步业务流 (`AsyncMemo`)

音视频元数据提取（如解析大文件封面、读取 ID3 信息）需要进行异步 IO。NeHive 提供了 `AsyncMemo` 基础设施，让开发者能够以同步的、声明式的拓扑网络来编写异步派生状态，并一键对接 UI 的加载状态。

```csharp
var songInfo = uiScope.CreateAsyncMemo<SongInfo?>(async epochScope => {
    var index = epochScope.Pull(currentIndex); // 自动订阅并追踪当前播放索引的变更
    if (index < 0) return null;
    
    var media = new Media(libVlc, playlist.Value[index].FilePath);
    await media.Parse(timeout: 5000); // 异步非阻塞解析
    return new SongInfo(media.Meta(MetadataType.Title), ...);
});

// 在 UI 拓扑中显式消费异步流的各个状态分支（加载中、成功、异常）
Loading<SongInfo?>(new(songInfo)
{
    Success = song => Audio(song),
    Loading = () => Audio(null),
    Error = ex => HTextBlock($"加载失败: {ex.Message}")
});
```

## 4️⃣ 高性能的原子级局部更新

NeHive 抛弃了重型的全局重绘机制。通过内置的控制流原语，样式切换与条件分支直接映射为细粒度拓扑。**当 `currentIndex` 改变时，有且仅有对应列表项的属性会触发靶向重绘**，从而保证了极其优异的 UI 渲染性能。

```csharp
ForEach<TrackInfo>(new(playlist)
{
    ItemTemplate = (track, index) =>
        HButton(track.Title,
            strStyle: new(() => {
                var isActive = index.RxValue == currentIndex.RxValue; // 自动追踪当前激活项
                return $"px-3 py-2 {(isActive ? "bg-sky-100 fg-sky-800" : "bg-transparent")}";
            }),
            onClick: _ => currentIndex.RxValue = index.Value
        )
});
```

---

# 📋 范式对比

| 特性 | 传统 MVVM (WPF / 传统 Avalonia) | NeHive 范式 |
| --- | --- | --- |
| **UI 表达** | XAML (标记语言，逻辑分离) | 纯 C# 函数式 DSL (声明式代码即 UI) |
| **状态响应** | `Binding` + `INotifyPropertyChanged` | `Signal` 自动依赖追踪与拓扑更新 |
| **更新粒度** | 组件级 / 数据上下文级重绘 | **原子节点级细粒度精准更新** |
| **调试体验** | 运行时黑盒，XAML 绑定错误难以调试 | 纯 C# 代码，调用栈清晰，可直接断点 |
| **核心心智模型** | 概念碎片化：模板、触发器、附加属性、命令、转换器等多套独立体系 | **统一模型：Scope + Signal + Effect**，万物基于三大原语构建 |
| **语法体系** | 标记语言 + C# 双重语法，语法割裂 | 纯原生 C#，模板、触发器等价于匿名函数+响应式 |
| **数据转换** | 依赖独立值转换器，需单独编写、注册、维护 | 原生匿名函数直接完成转换，无额外转换器负担 |
| **样式体系** | 原生属性写法冗长，组合复杂、复用困难 | 类 Tailwind 原子化 `strStyle`，写法简洁、灵活易复用 |
| **学习成本** | 概念繁多、规则复杂，上手难度高 | 概念极简，掌握三大核心即可全覆盖，上手轻松 |
| **构建范式** | 声明式与命令式强制割裂 | 声明式、命令式自由混合，适配不同业务场景 |

---

# 📦 项目模块状态

| 模块 | 描述 | 状态 |
| --- | --- | --- |
| `package/NeHive.Model` | 运行时语义模型层（Scope / Context） | 预览版 |
| `package/NeHive.Reactive` | 细粒度响应式内核（Signal / Effect / Flow） | 预览版 |
| `package/NeHive.UI.Avalonia` | 函数式 UI 声明式顶层实现 | 实验性 |
| `package/NeHive.Generator` | 提升开发体验的源码生成器 | 开发中 |
| `samples` | 示例项目集合（包含 MusicPlayer 完整实现） | 持续维护 |

## 🔧 安装 NuGet 包
你可以直接通过 NuGet 安装最新预览版：

```bash
# 响应式核心（必装）
dotnet add package NeHive.Reactive --version 0.1.1

# Avalonia UI 集成（用于构建界面）
dotnet add package NeHive.UI.Avalonia --version 0.0.1-alpha
```

或在 Visual Studio 的**NuGet 包管理器**中搜索安装：
- `NeHive.Reactive`
- `NeHive.UI.Avalonia`

---

# 🎯 适用场景与设计边界

## 如果你：

* 渴望追求类似前端 SolidJS / React 极致开发体验的 .NET 开发者。
* 复杂交互、高频状态驱动的工具软件、编辑器、看板应用。
* 想摆脱复杂的 MVVM 架构，追求代码高内聚、易调试的项目。

## 请注意，NeHive **不是**：

* ❌ 传统 MVVM 框架的微调替代品。
* ❌ 纯粹为了替代 XAML 而做的语法糖（它包含一套完整的响应式运行时架构）。

---

> [!WARNING]
> NeHive 目前处于早期演进阶段，API 仍在频繁迭代优化中，暂不建议直接用于生产环境。欢迎任何建设性的 Issue 和 PR！

---

# 📄 License

本项目基于 [MIT](LICENSE) 许可证开源。
