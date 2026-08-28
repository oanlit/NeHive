# NeHive UI 组件 API 与书写规范

NeHive 使用原生 C# 表达 UI 树。下列规范用于在保留强类型、响应式能力的同时，让嵌套 UI 的结构边界保持清晰。

## 1. 跨行组件使用尾随注释标记边界

单行组件的边界已经清晰，一般不写尾随注释：

```csharp
HText("Title", strStyle: DemoTitle),
HButton("Save", onClick: Save)
```

组件跨越多行时，在闭合处使用尾随注释，补足 C# 花括号缺少具名闭合标记的问题：

```csharp
HStackPanel(_ => new(strStyle: "gap-3 vertical")
{
    HText("Title"),
    HButton("Save", onClick: Save)
}) // HStackPanel
```

对跨行的具名插槽或模板，尾随注释应尽量同时标出组件与成员：

```csharp
Loading<User>(new(user)
{
    Loading = _ =>
        HStackPanel(_ => new(strStyle: "gap-2 vertical")
        {
            LoadingIndicator(),
            HText("Loading...")
        }), // HStackPanel
    // Loading<User>.Loading
    Success = value =>
        HStackPanel(_ => new(strStyle: "gap-2 vertical")
        {
            Avatar(value),
            HText($"Hello {value.Name}")
        }), // HStackPanel
    // Loading<User>.Success
    Error = error =>
        HStackPanel(_ => new(strStyle: "gap-2 vertical")
        {
            ErrorIcon(),
            HText(error.Message)
        }) // HStackPanel
    // Loading<User>.Error
}) // Loading<User>
```

尾随注释是 UI 树的结构辅助，不是对显而易见代码的重复解释。

## 2. 无子元素时使用普通参数形式

当组件只需要配置自身属性，不包含子 `IElement` 时，直接使用普通参数重载：

```csharp
HText("Title", strStyle: DemoTitle)

ItemsPanel = HStackPanel(
    strStyle: ScrollContainerBase + "w-full p-3 gap-3 vertical"
)
```

这是叶子组件和无子元素容器的默认写法，不需要为了传入 `strStyle` 而使用参数构造函数。

## 3. 子 `IElement` 写在对象初始化器中

当组件包含子元素时，使用参数构造函数创建 Args，并将子元素放在花括号初始化器中：

```csharp
HStackPanel(_ => new(strStyle: "gap-3 vertical")
{
    Header(),
    Content()
}) // HStackPanel
```

位置只由类型决定，不根据成员的名称或语义判断：

- 类型中没有出现 `IElement`，写在参数列表中；
- 类型中出现 `IElement`，写在初始化器中。

“类型中出现 `IElement`”包括 `IElement` 本身，以及在泛型参数、集合元素或委托签名中出现 `IElement` 的类型。

例如，具名 UI 插槽应当清晰地位于初始化器内：

```csharp
HSplitView(_ => new(
    isPaneOpen: isPaneOpen,
    displayMode: SplitViewDisplayMode.CompactInline,
    openPaneLength: 200,
    strStyle: "w-full h-40")
{
    Pane = Sidebar(),
    Content = MainContent()
}) // HSplitView
```

因此，选择书写位置时不需要解释成员的用途，只需要检查它的类型。

## 4. 组件函数的代码顺序

有独立作用域的组件建议按以下顺序编写：

1. 创建资源；
2. 声明基础状态；
3. 声明派生状态；
4. 定义 UI 结构；
5. 添加 `uiScope.OnMount`；
6. 绑定额外事件；
7. 添加 Effect；
8. 通过 `uiScope.OnCleanup` 添加 Dispose/Cleanup 逻辑；
9. 返回根元素；
10. 在 `return` 之后放置局部函数。

如果 UI 结构之后没有生命周期逻辑、额外事件或副作用，建议直接返回根元素：

```csharp
private static IElement Counter()
{
    var count = new MutSignal<int>(0);

    return HStackPanel(_ => new(strStyle: "gap-2 vertical")
    {
        HText(new(() => $"Count: {count.RxValue}")),
        HButton("+1", onClick: _ => count.RxValue++)
    }); // HStackPanel
}
```

只有在 UI 结构之后还需要继续完成组件装配时，才将根元素保存为 `rootElement`：

```csharp
private static IElement PlayerComp(UiScope uiScope)
{
    // ---------- 资源 ----------
    var player = CreatePlayer();

    // ---------- 状态 ----------
    var isPlaying = new MutSignal<bool>(false);

    // ---------- UI ----------
    var rootElement = HStackPanel(_ => new(strStyle: "gap-3 vertical")
    {
        PlayerControls(isPlaying)
    }); // HStackPanel

    // ---------- OnMount ----------
    uiScope.OnMount += player.Initialize;

    // ---------- 事件 ----------
    player.Playing += OnPlaying;

    // ---------- Effect ----------
    uiScope.CreateEffect(() => player.SetPlaying(isPlaying.RxValue));

    // ---------- Dispose / Cleanup ----------
    uiScope.OnCleanup += () =>
    {
        player.Playing -= OnPlaying;
        player.Dispose();
    };

    return rootElement;

    void OnPlaying(object? sender, EventArgs args)
    {
        isPlaying.RxValue = true;
    }
}
```

`rootElement` 仅用于当前组件最终返回的根 `IElement`；内部区域应使用更具体的名称。

## 5. `Func<TProps, TArgs>` 参数构造形式

需要初始化器的组件通过以下形式接收参数：

```csharp
Func<HButtonProps, HButtonArgs> fn
```

这不只是为了获得对象初始化器语法。`props` 暴露了组件自身的响应式属性，因此参数和视觉表现可以响应组件自身状态：

```csharp
HButton(props => new(
    strStyle: new(() =>
        props.IsPressed.RxValue
            ? "bg-matcha-600 scale-95"
            : "bg-matcha-400"))
{
    ButtonContent()
}) // HButton
```

不需要读取自身 Props 时，使用 `_` 表达该意图：

```csharp
HButton(_ => new(strStyle: PrimaryButtonStyle)
{
    Icon(),
    HText("Save")
}) // HButton
```

## 6. 为什么不再提供直接的 `TArgs` 重载

如果同时提供：

```csharp
HButton(HButtonArgs args)
HButton(Func<HButtonProps, HButtonArgs> fn)
```

会带来两类成本：

1. 每一组包含 `out` 参数等变体的 API 都要复制一套 Args 重载，使重载数量持续增长。
2. 由于存在多个候选目标类型，调用端无法保持理想的目标类型 `new` 写法，往往需要显式写出 `new HButtonArgs() { ... }`。

相比之下：

```csharp
HButton(_ => new(...)
{
    ...
}) // HButton
```

使 Args 类型可以由编译器推断，同时保留读取自身 Props 的能力，并避免扩大重载集合。

## 7. `strStyle` 的设计边界

`strStyle` 用于表达与视觉相关、但不影响组件核心工作行为的属性，例如：

- 尺寸、间距和布局对齐；
- 颜色、字体、边框和阴影；
- 视觉状态、变换和过渡。

```csharp
HButton(
    "Save",
    strStyle: "px-3 py-2 fg-white bg-matcha-400 rounded-lg hover:bg-matcha-500"
)
```

这些属性属于高容错的视觉区域，因此 NeHive 有意使用类 Tailwind 的字符串 DSL，以获得更高的组合效率。UI 结构、响应式状态、事件和生命周期仍然保持在强类型 C# 中。

`strStyle` 的容错边界不应扩展到组件工作行为：会改变数据流、事件语义、子元素结构或资源生命周期的配置，应继续使用强类型 API。

## 完整示例

```csharp
private static IElement SaveButton()
{
    return HButton(props => new(
        onClick: _ => Save(),
        strStyle: new(() => $"""
            px-3 py-2 fg-white rounded-lg
            {(props.IsPressed.RxValue ? "bg-matcha-600 scale-95" : "bg-matcha-400")}
            """))
    {
        HStackPanel(_ => new(strStyle: "gap-2 horizontal items-center")
        {
            SaveIcon(),
            HText("Save")
        }) // HStackPanel
    }); // HButton
}
```

在这个示例中：

- `onClick` 和 `strStyle` 是组件配置，写在参数列表中；
- 按钮内容是 `IElement` 子树，写在初始化器中；
- `props.IsPressed` 让样式响应按钮自身状态；
- `HStackPanel` 和 `HButton` 均跨越多行，因此使用尾随注释标记闭合边界；
- `HText("Save")` 保持在单行，因此不需要尾随注释。
