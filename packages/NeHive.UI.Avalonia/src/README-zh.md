# NeHive.UI.Avalonia
[![Language](https://img.shields.io/badge/README-English-green.svg)](README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)

基于 **Avalonia UI** 的纯 C# 声明式跨平台 UI 框架，深度整合 NeHive.Reactive 响应式引擎，提供原子化样式系统与函数式组件模型，无需 XAML 即可构建高性能跨平台应用。

该项目处于早期开发阶段（0.1.x 系列）。其 API 可能会有所变动。

[![NuGet](https://img.shields.io/nuget/v/NeHive.UI.Avalonia.svg)](https://www.nuget.org/packages/NeHive.UI.Avalonia)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Target Framework](https://img.shields.io/badge/.NET-7.0%2F8.0%2F9.0%2F10.0-green)]()

## 简介
**NeHive.UI.Avalonia** 是一套为 Avalonia 设计的**纯 C# 声明式 UI 开发框架**。

基于 **NeHive.Reactive** 细粒度响应式运行时驱动，采用组件化、函数式 UI 写法，内置类 Tailwind 原子化样式解析器，自动生命周期管理，实现真正的**UI = f(State)** 跨平台开发体验。

完美支持 Windows / macOS / Linux / 嵌入式 / 移动端，无 XAML、低开销、高性能、强类型安全。

### 核心设计理念
- ⚡ **深度响应式**：基于 Signal 细粒度更新，组件按需刷新，无冗余渲染
- 🧩 **纯 C# 组件**：函数式组件模型，支持组合、复用、作用域隔离
- 🎨 **原子化样式**：类 Tailwind 样式体系，支持颜色、间距、布局、渐变、变换、过渡
- 🧭 **自动生命周期**：UiScope 自动回收资源、订阅、事件，杜绝内存泄漏
- 🔁 **内置控制流**：提供 ForEach / Show / Switch / Loading / Match 等声明式逻辑组件
- 📦 **原生控件全覆盖**：封装 Avalonia 全部基础控件、布局、交互能力
- 🌊 **流式状态编排**：支持防抖、节流、异步状态、双向绑定等开箱即用

## 安装
### NuGet
```bash
Install-Package NeHive.UI.Avalonia
```
```bash
dotnet add package NeHive.UI.Avalonia
```

# 快速入门

## 1. 基础响应式组件
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

## 2. 列表渲染（ForEach）
```csharp
var items = new MutSignal<IReadOnlyList<int>>([1,2,3]);

return RootElement(new()
{
    ForEach<int>(new(items))
    {
        ItemTemplate = (item, index) => 
            HTextBlock(() => $"Item {index.RxValue}: {item}")
    }
});
```

## 3. 条件渲染（Show）
```csharp
var visible = new MutSignal<bool>(true);

return RootElement(new()
{
    Show(new(visible))
    {
        IfTrue = () => HTextBlock("可见内容"),
        IfFalse = () => HTextBlock("已隐藏")
    }
});
```

## 4. 多分支匹配（Switch）
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

## 5. 模式匹配（Match）
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

## 6. 异步加载状态（Loading）
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

# 控制流组件一览

| 组件 | 用途 |
|------|------|
| `Show` | 简单条件显示/隐藏 |
| `Switch` | 固定值多分支匹配 |
| `Match` | 表达式/谓词模式匹配 |
| `ForEach` | 响应式列表渲染 |
| `Loading` | 异步状态三态管理 |

# 布局与控件

## 布局容器
- `HStackPanel`
- `HGrid`
- `HAbsolute`
- `HDockPanel`
- `HWrapPanel`
- `HUniformGrid`
- `HSplitView` / `HSplitPanel`
- `HScrollViewer`

## 基础控件
- `HTextBlock`
- `HButton` / `HContentButton`
- `HTextBox`（支持双向绑定）
- `HCheckBox` / `HRadioButton` / `HToggleSwitch`
- `HSlider` / `HProgressBar`
- `HListBox` / `HComboBox` / `HTreeView`
- `HTabControl`
- `HUriImage` / `HSvgImage`
- `HFilePicker` / `HFolderPicker`

# 原子样式系统
支持类 Tailwind 原子 CSS，自动解析为 Avalonia 样式：

- 间距：`m-2` `p-3` `mt-4` `gap-x-2`
- 尺寸：`w-20` `h-12` `max-w-60`
- 颜色：`fg-blue-500` `bg-gray-100` `border-red-300`
- 渐变：`bg-gradient-tl from-blue to-purple`
- 边框圆角：`rounded-lg` `border-w-1`
- 文本：`text-lg` `font-bold` `text-center`
- 对齐：`center` `justify-center` `items-center`
- 变换：`scale-110` `translate-y-1` `rotate-3`
- 过渡：`transition-colors duration-300 ease-out`
- 交互：`hover:opacity-80` `click:scale-95`

# 适用场景
- 跨平台桌面应用（Windows / macOS / Linux）
- 嵌入式 / 工控 UI
- 工具软件、配置面板
- 数据可视化、列表、表单系统
- 多媒体播放器、音视频控制界面
- 纯 C# 无 XAML 高效开发项目

# 互操作性
- 完全兼容 Avalonia 原生控件
- 无缝对接 NeHive.Reactive 响应式体系
- 支持 Signal 与 IObservable 互转
- 可与原生 Style / 第三方控件混用

## 灵感来源
- Avalonia UI
- Tailwind CSS
- SolidJS / Vue3（细粒度响应式）
- NeHive.Reactive

## 开源协议
MIT
