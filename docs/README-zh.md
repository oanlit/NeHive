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

## 📦 项目模块

| 模块                                                         | 描述                        | 状态     |
|------------------------------------------------------------|---------------------------|--------|
| [`package/NeHive.Reactive`](package/NeHive.Reactive)       | 面向 .NET 的细粒度响应式运行时        | 公开预览版  |
| [`package/NeHive.UI.Avalonia`](package/NeHive.UI.Avalonia) | 基于 Avalonia 的实验性函数式 UI 框架 | 实验性    |
| [`package/NeHive.Generator`](package/NeHive.Generator)     | 简化响应式样板代码的源码生成器           | 早期开发阶段 |
| [`samples`](samples)                                       | 控制台与 Avalonia 示例项目        | 活跃维护中  |

## 🚀 适用场景
- 不想写 XAML、不想维护传统绑定的 Avalonia 桌面开发
- 追求高性能细粒度状态更新的跨平台应用
- 业务复杂、需要组件化、状态集中管理的系统
- 想用 Solid/Vue 响应式思维写原生 .NET 应用

---

## 👀 示例预览

### 示例代码

#### 简单计数器
```csharp
private static IElement CounterComp(int id, UiScope uiScope)
{
    Console.WriteLine($"Counter {id} is creating");
    var count = new MutSignal<int>(0);
    var countText = () => $"Count: {count.RxValue}";

    var rootElement = uiScope.RootElement(new()
    {
        HTextBlock($"Id: {id}",
            strStyle: "text-lg font-bold fg-sky-200"
        ), // HTextBlock

        HTextBlock(countText,
            strStyle: "text-2xl fg-lime-300"
        ), // HTextBlock

        HButton("Add",
            strStyle: """
                      mt-1 ml-2 px-2 py-1 fg-white
                      bg-green-300 hover:bg-green-400 click:bg-green-500
                      border-w-1 border-green-400 rounded-lg
                      """,
            onClick: _ => count.RxValue++
        ), // HButton

        HButton("Sub",
            strStyle: """
                      mt-1 ml-2 px-2 py-1 fg-white 
                      bg-pink-300 hover:bg-pink-400 click:bg-pink-500
                      border-w-1 border-pink-400 rounded-lg
                      """,
            onClick: _ => count.RxValue--
        ) // HButton
    }); // rootElement

    uiScope.OnDispose += () => Console.WriteLine($"Counter {id} disposed");
    return rootElement;
}

public static IElement Counter(int prop)
    => Element.WithScope(CounterComp, prop);
```

#### 异步组件
```csharp
private static IElement LoadingDemoComp(UiScope uiScope)
{
    var userId = new MutSignal<int>(1);

    var userMemo = uiScope.CreateReactiveFlow(userId)
        .Debounce(500)
        .Filter(id => id > 0)
        .Map(id => new User(id, $"User {id}"))
        .PushAsyncMemo(async user =>
        {
            await Task.Delay(500);
            return user;
        }, initValue: new User(0, "Unknown"));

    var rootElement = uiScope.RootElement(new()
    {
        HStackPanel(new(strStyle: "mb-4 horizontal gap-3")
        {
            HButton("Increase User Id",
                strStyle: "px-3 py-1 bg-blue-400 fg-white rounded-lg",
                onClick: _ => userId.RxValue++
            ), // HButton
            HButton("Decrease User Id",
                strStyle: "px-3 py-1 bg-slate-400 fg-white rounded-lg",
                onClick: _ => userId.RxValue--
            ) // HButton
        }), // HStackPanel

        Loading<User>(new(userMemo)
        {
            Success = user => HChildren(
                HTextBlock($"User Id: {user.Id}", strStyle: "mt-1.5 text-lg"),
                HTextBlock($"Hello, {user.Name}", strStyle: "mt-1.5 fg-sky-600")
            ), // Loading<User>.Success
            Loading = _ => HTextBlock("Fetching user data...", strStyle: "fg-gray-500"),
            Error = ex =>
                HButton($"Retry: {ex.Message}",
                    strStyle: "mt-2 px-3 py-1 bg-rose-400 fg-white rounded-lg",
                    onClick: _ => userMemo.Refetch()
                ) // HButton
            // Loading<User>.Error
        }) // Loading<User>
    }); // rootElement

    return rootElement;
}

public static IElement LoadingDemo()
    => Element.WithScope(LoadingDemoComp);
```

#### 引用组件
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
