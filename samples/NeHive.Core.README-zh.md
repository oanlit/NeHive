# NeHive.Core
轻量高性能的 .NET 响应式库，采用单线程依赖图模型，以信号驱动细粒度状态更新。

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

## 快速上手
### 1. 基础 Signal + Effect 响应式
```csharp
using NeHive.Core;

// 创建作用域
using var scope = new Scope();

// 定义响应式状态
var count = new MutSignal<int>(0);

// 自动追踪依赖，状态变化自动执行
scope.CreateEffect(() =>
{
    Console.WriteLine($"当前计数：{count.RxValue}");
});

// 修改状态，自动触发副作用
count.RxValue = 10;
count.RxValue = 99;
```

### 2. 计算属性 Computed
依赖自动缓存，仅依赖变更才重新计算
```csharp
using var scope = new Scope();

var price = new MutSignal<double>(100);
var num = new MutSignal<int>(2);

// 计算总价
var totalPrice = new Computed<double>(() => price.RxValue * num.RxValue);

scope.CreateEffect(() =>
{
    Console.WriteLine($"总价：{totalPrice.RxValue}");
});

price.RxValue = 199;
num.RxValue = 5;
```

### 3. 响应式流（防抖 / 节流 / 过滤 / 映射）
高频事件降频、数据预处理一站式实现
```csharp
using var scope = new Scope();

var inputSignal = new MutSignal<int>(0);

// 500ms 节流，取最新值
var flow = scope.CreateReactiveFlow(inputSignal)
    .ThrottleLatest(500)
    .Filter(v => v > 5)
    .Map(v => v * 10)
    .PushEffect(val =>
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} 输出值：{val}");
    });

// 快速连续赋值
for (int i = 1; i <= 10; i++)
{
    inputSignal.RxValue = i;
    await Task.Delay(100);
}

await Task.Delay(1000);
flow.Dispose();
```

### 4. 响应式列表 ListStore
细粒度监听元素修改、新增、删除、排序、批量修改
```csharp
using var scope = new Scope();

// 可空响应式列表
var listStore = new ListStore<int?>([1, 2, 3, null, 5]);

// 监听列表整体变化
scope.CreateEffect(() =>
{
    Console.WriteLine($"列表总数：{listStore.Count}");
});

// 监听指定索引元素
scope.CreateEffect(() =>
{
    listStore.TryGetValue(0, out var first);
    Console.WriteLine($"首位元素：{first ?? -1}");
});

// 批量修改，仅触发一次更新
listStore.BatchModify(list =>
{
    list[0] = 999;
    list.Add(666);
});

listStore.Reverse();
listStore.Sort();
listStore.Clear();
```

## 核心API一览
### 基础核心
- `Scope`：作用域容器，管理所有响应式对象生命周期
- `MutSignal<T>`：只读响应式状态源
- `MutSignal<T>`：可变响应式状态源
- `Computed<T>`：只读缓存计算状态
- `Effect`：依赖自动追踪副作用
- `AsyncMemo<T>` 缓存异步响应值，并具备加载/错误状态管理功能

### 响应式流 ReactiveFlow
链式流式操作符
- `Filter()` 条件过滤
- `Map()` 数据转换映射
- `Debounce(ms)` 防抖
- `ThrottleLatest(ms)` 节流取最新
- `PushEffect()` 订阅消费数据流

### 响应式集合
- `ListStore<T>`：增强响应式列表
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

## 框架生态
- **NeHive.Core**：底层响应式运行时（本库）
- **NeHive.UI.Avalonia**：基于 Avalonia 跨平台纯 C# 声明式 DSL UI 库
    - 抛弃 XAML，全代码构建界面
    - Tailwind 风格字符串样式
    - 内置 `ForEach` / `Show` / `Switch` / `Loading` 常用控制流组件

## 兼容性
- 支持 .NET 7.0 / .NET 8.0 / .NET 9.0 / .NET 10.0
- 开启可空引用、隐式命名空间、最新 C# 语法
- 全平台兼容：Windows / macOS / Linux / WebAssembly / 移动端

## 开源协议
**MIT License**
开源免费，可自由商用、二次开发、二次分发
