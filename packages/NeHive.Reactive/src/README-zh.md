[![Language](https://img.shields.io/badge/README-English-green.svg)](README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)

# NeHive.Reactive
轻量高性能的 .NET 响应式库，采用单线程依赖图模型，以信号驱动细粒度状态更新。

该项目处于早期开发阶段（0.1.x 系列）。其 API 可能会有所变动。

[![NuGet](https://img.shields.io/nuget/v/NeHive.Core.svg)](https://www.nuget.org/packages/NeHive.Core)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Target Framework](https://img.shields.io/badge/.NET-7.0%2F8.0%2F9.0%2F10.0-green)]()

## 简介
**NeHive.Core** 是一套专为 .NET 设计的**轻量高性能单线程响应式运行时**，基于**作用域依赖图 + 信号传播**架构实现，对标 SolidJS/Vue3 细粒度响应式思想，无反射、无重型依赖、低 GC 开销。

专为声明式 UI、状态管理、业务流联动场景打造，也是 **NeHive.UI** 跨平台声明式 UI 框架底层核心引擎。

### 核心设计理念
- 🧭 **作用域隔离**：`Scope` 统一管理响应式资源，自动生命周期回收，杜绝内存泄漏
- ⚡ **细粒度响应**：精准追踪读写依赖，仅触发变更关联逻辑，而非全量刷新
- 🧩 **信号驱动**：`Signal` 作为状态源，单向数据流，逻辑清晰易维护
- 🌊 **流式编排**：内置响应式流，开箱即用防抖、节流、过滤、映射等操作符
- 📦 **集合增强**：原生支持响应式列表，细粒度监听索引、数量、批量变更
- ✅ **纯原生 C#**：最高语言版本特性，可空/隐式命名空间友好，跨 .NET 多版本兼容

## 安装
### NuGet
```bash
Install-Package NeHive.Core
```
```bash
dotnet add package NeHive.Core
```

# 快速入门

## 1. 基础响应式

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

输出：

```text
Count: 0
Count: 1
Count: 2
```

---

## 2. 计算状态

计算值会自动缓存，并且仅在其依赖项发生变化时更新。

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

## 3. 异步响应式状态

`AsyncMemo<T>` 提供了带有加载和错误追踪的异步响应式状态。

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

## 4. 响应式流

`ReactiveFlow` 提供了轻量级的流式操作符。

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

## 5. 响应式集合

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

# 核心 API

## 运行时原语

| API                | 描述                     |
| ------------------ | ------------------------ |
| `Scope`            | 响应式生命周期容器         |
| `Signal<T>`        | 只读响应式状态             |
| `MutSignal<T>`     | 可写响应式状态             |
| `Computed<T>`      | 缓存的 computed 信号      |
| `Effect`           | 响应式副作用               |
| `AsyncMemo<T>`     | 异步响应式计算值           |
| `Selector<T>`      | 基于键的选择性依赖追踪      |
| `Context<T>`       | 作用域依赖/上下文传播       |

---

## 运行时工具

| API                  | 描述                          |
| -------------------- | ---------------------------- |
| `Rx.Batch()`         | 批量处理信号更新                |
| `Rx.Untrack()`       | 读取值但不进行依赖追踪           |
| `Rx.OnDispose()`     | 注册清理回调                   |

---

## ReactiveFlow 操作符

| API                    | 描述                        |
| ---------------------- |----------------------------|
| `Filter()`             | 条件过滤                     |
| `Map()`                | 值转换                      |
| `Debounce()`           | 防抖更新                     |
| `ThrottleLatest()`     | 节流最新值                   |
| `PushEffect()`         | 终端副作用订阅                |
| `PushComputed()`       | 将流转换为计算状态            |
| `PushAsyncMemo()`      | 将流转换为异步响应式状态       |

---

## 响应式集合
### `ListStore<T>`
增强响应式列表，支持：
- 索引精准监听
- `BatchModify` 批量修改合并触发
- 原生支持 `Add`/`RemoveAt`/`Sort`/`Reverse`/`Clear`
- 内置 LINQ 响应式查询

## 适用场景
1. **跨平台声明式 UI 状态管理**（搭配 NeHive.UI.Avalonia）
2. 桌面端 / 移动端轻量状态驱动开发
3. 表单输入防抖、搜索联想、高频交互节流优化
4. 业务流程状态联动、多数据依赖自动联动更新
5. 替代传统 MVVM 繁杂绑定，简化状态流转

## 互操作性

### SignalObservable<T>
将 MutSignal<T> 转换为标准 IObservable<T> 序列。

### Rx.From(IObservable<T>, T)
将外部 IObservable<T> 转换为响应式 ISignal<T>。

### 数组 Diff 工具

| API                    | 描述                                         |
| ---------------------- | -------------------------------------------- |
| ArrayDiffUtil        | 计算两个列表之间的最小差异                    |
| ArrayMapResult       | 增量数组映射（保持已移动项引用）              |
| ArrayMapMemo         | 响应式增量数组映射信号                       |

---

## 灵感来源
NeHive.Reactive 深受以下项目背后的思想和研究的启发：

* [SolidJS](https://github.com/solidjs/solid)
* [ReactiveX](https://github.com/dotnet/reactive)

## 兼容性
- 支持 .NET 7.0 / .NET 8.0 / .NET 9.0 / .NET 10.0
- 开启可空引用、隐式命名空间、最新 C# 语法
- 全平台兼容：Windows / macOS / Linux / WebAssembly / 移动端

## 开源协议
MIT
