[![Language](https://img.shields.io/badge/README-English-green.svg)](../README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)

# NeHive
## .NET 细粒度响应式 runtime + 纯函数式跨平台 UI 生态

[![NuGet](https://img.shields.io/nuget/v/NeHive.Reactive.svg)](https://www.nuget.org/packages/NeHive.Reactive)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Target](https://img.shields.io/badge/.NET-7%2F8%2F9%2F10-green)]()

> 告别 INotifyPropertyChanged、告别依赖属性、告别 XAML、告别控件继承  
> 借鉴 SolidJS/Vue3 细粒度响应式思想，在 .NET 原生实现 **Signal 驱动 + 函数组件** 开发范式

---

## 🔭 整体架构
NeHive 由三大核心模块构成，分层解耦、各司其职：

- **NeHive.Reactive**  
  轻量高性能单线程响应式运行时  
  Signal / MutSignal / Computed / Effect / AsyncMemo / ReactiveFlow / ListStore  
  无反射、低GC、Scope 自动生命周期管理，杜绝内存泄漏。

- **NeHive.UI.Avalonia**  
  基于 Avalonia 封装的**函数式声明式 UI 框架**  
  无XAML、无自定义控件继承、纯C#构建界面  
  内置布局、基础控件、Tailwind 风格原子样式、  
  控制流组件：`ForEach` / `Show` / `Switch` / `Loading`。

- **NeHive.Generator**  
  源码生成器，简化响应式样板代码与组件模板编写。

---

## ✨ 核心理念 & 差异化
- ⚡ **细粒度响应式更新**  
  精准依赖追踪，仅更新变化节点，无全量重绘。
- 🧩 **纯函数组件**  
  不用继承 Control、不用写后台绑定，组件即是函数。
- 📝 **零 XAML**  
  全程强类型 C# 声明式 UI，编译安全、智能提示。
- 🎨 **原子化字符串样式**  
  类 Tailwind 写法，告别繁琐 Style/Setter。
- 🧭 **Scope 作用域托管**  
  状态、订阅、组件生命周期统一管理，天然防泄漏。
- 🌍 **全平台原生**  
  基于 Avalonia，一次编写跑 Windows / macOS / Linux / WASM / 移动端。

---

## 📦 子项目说明
- [`src/NeHive.Reactive`](src/NeHive.Reactive)：响应式核心运行时
- [`src/NeHive.UI.Avalonia`](src/NeHive.UI.Avalonia)：函数式 Avalonia UI 组件库
- [`src/NeHive.Generator`](src/NeHive.Generator)：源码生成器
- [`samples`](samples)：控制台 & Avalonia 完整示例工程

---

## 🚀 适用场景
- 不想写 XAML、不想维护传统绑定的 Avalonia 桌面开发
- 追求高性能细粒度状态更新的跨平台应用
- 业务复杂、需要组件化、状态集中管理的系统
- 想用 Solid/Vue 响应式思维写原生 .NET 应用

---

## 👀 示例预览

### 示例代码
```csharp
private static IElement ScrollDemoComp(UiScope uiScope)
{
    var sb = new StringBuilder();
    for (var i = 1; i <= 40; i++)
    {
        sb.AppendLine($"Line {i}: Long scrollable content sample for NeHive UI framework.");
    }

    var longText = sb.ToString();

    var rootElement = uiScope.RootElement(new(strStyle: "m-4 flex-col")
    {
        HTextBlock("Scroll Viewer Demo", strStyle: "text-lg font-bold"),

        HScrollViewer(out var scroll, new(
            horizontalScrollBarVisibility: ScrollBarVisibility.Hidden,
            verticalScrollBarVisibility: ScrollBarVisibility.Auto,
            strStyle: "m-2 h-60 p-3 vertical bg-gray-100 rounded-xl border border-gray-300")
        {
            HTextBlock(longText, strStyle: "text-base")
        }), // HScrollViewer

        HStackPanel(new(strStyle: "my-4 gap-x-16 flex-row justify-center")
        {
            HButton("⬆️ Top",
                strStyle: "px-3 py-1 font-bold fg-sky-600 bg-sky-200 border-sky-400 rounded",
                click: _ => ScrollToHome()
            ), // HButton
            HButton("⬇️ Bottom",
                strStyle: "px-3 py-1 font-bold fg-green-600 bg-green-200 border-green-400 rounded",
                click: _ => ScrollToEnd()
            ) // HButton
        }) // HStackPanel
    }); // rootElement
    return rootElement;

    void ScrollToHome()
    {
        scroll.ScrollToHome();
    }

    void ScrollToEnd()
    {
        scroll.ScrollToEnd();
    }
}

public static IElement ScrollDemo()
    => Element.WithScope(ScrollDemoComp);
```

### 结果预览
![Scroll_Viewer_Demo.png](docs/img/Scroll_Viewer_Demo.png)

---

## 📄 License
MIT
