# NeHive UI Component API and Writing Conventions

NeHive expresses UI trees in native C#. These conventions keep nested UI structure readable while preserving strong typing and fine-grained reactivity.

## 1. Mark multiline component boundaries with trailing comments

Single-line components already have clear boundaries and normally need no trailing comment:

```csharp
HText("Title", strStyle: DemoTitle),
HButton("Save", onClick: Save)
```

When a component spans multiple lines, add a trailing comment at its closing boundary:

```csharp
HStackPanel(_ => new(strStyle: "gap-3 vertical")
{
    HText("Title"),
    HButton("Save", onClick: Save)
}) // HStackPanel
```

For a multiline named slot or template, the trailing comment should identify both the component and the member when possible:

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

These comments are structural aids for the UI tree, not restatements of obvious behavior.

## 2. Use ordinary parameters when there are no child elements

When a component only configures its own properties and contains no child `IElement`, use its ordinary parameter overload:

```csharp
HText("Title", strStyle: DemoTitle)

ItemsPanel = HStackPanel(
    strStyle: ScrollContainerBase + "w-full p-3 gap-3 vertical"
)
```

There is no need to use an Args factory merely to pass `strStyle`.

## 3. Put child `IElement` values in the object initializer

When a component contains child elements, create its Args through the factory overload and place the children in the initializer:

```csharp
HStackPanel(_ => new(strStyle: "gap-3 vertical")
{
    Header(),
    Content()
}) // HStackPanel
```

Placement is determined only by type, never by a member's name or semantics:

- if `IElement` does not appear in the type, place it in the parameter list;
- if `IElement` appears in the type, place it in the initializer.

"Appears in the type" includes `IElement` itself and types where `IElement` occurs in a generic argument, collection element, or delegate signature.

For example, named UI slots remain visibly part of the initializer:

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

There is no need to interpret what a member does when choosing its location; inspect its type only.

## 4. Component function code order

Write a scoped component in the following order:

1. create resources;
2. declare source state;
3. declare derived state;
4. define the UI structure;
5. add `uiScope.OnMount` handlers;
6. bind additional events;
7. create Effects;
8. add Dispose/Cleanup logic through `uiScope.OnCleanup`;
9. return the root element;
10. place local functions after `return`.

If no lifecycle logic, additional events, or side effects follow the UI structure, return the root element directly:

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

Only store the root element as `rootElement` when component assembly must continue after the UI structure:

```csharp
private static IElement PlayerComp(UiScope uiScope)
{
    // ---------- Resources ----------
    var player = CreatePlayer();

    // ---------- State ----------
    var isPlaying = new MutSignal<bool>(false);

    // ---------- UI ----------
    var rootElement = HStackPanel(_ => new(strStyle: "gap-3 vertical")
    {
        PlayerControls(isPlaying)
    }); // HStackPanel

    // ---------- OnMount ----------
    uiScope.OnMount += player.Initialize;

    // ---------- Events ----------
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

Reserve `rootElement` for the root `IElement` ultimately returned by the current component. Give internal regions more specific names.

## 5. The `Func<TProps, TArgs>` factory form

Components that need an initializer receive their arguments through a factory such as:

```csharp
Func<HButtonProps, HButtonArgs> fn
```

This form does more than enable object initializers. `props` exposes the component's own reactive properties, allowing its configuration and presentation to respond to its state:

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

Use `_` when the component's Props are not needed:

```csharp
HButton(_ => new(strStyle: PrimaryButtonStyle)
{
    Icon(),
    HText("Save")
}) // HButton
```

## 6. Why there is no additional direct `TArgs` overload

Providing both of these overloads:

```csharp
HButton(HButtonArgs args)
HButton(Func<HButtonProps, HButtonArgs> fn)
```

would have two costs:

1. APIs with variants such as an `out` parameter would require another full family of Args overloads.
2. Multiple candidate target types prevent the preferred target-typed `new` form in common calls, often forcing `new HButtonArgs() { ... }` at every site.

The selected form:

```csharp
HButton(_ => new(...)
{
    ...
}) // HButton
```

keeps Args target-typed, retains access to reactive Props, and avoids multiplying overloads.

## 7. The design boundary of `strStyle`

`strStyle` describes visual properties that do not determine the component's core behavior, including:

- size, spacing, layout, and alignment;
- colors, typography, borders, and shadows;
- visual states, transforms, and transitions.

```csharp
HButton(
    "Save",
    strStyle: "px-3 py-2 fg-white bg-matcha-400 rounded-lg hover:bg-matcha-500"
)
```

These properties form a high-tolerance presentation layer, so NeHive deliberately uses a Tailwind-like string DSL for efficient composition. UI structure, reactive state, events, and lifecycle remain strongly typed C#.

The tolerance boundary must not spread into component behavior. Configuration that changes data flow, event semantics, child structure, or resource lifetime should remain strongly typed.

## Complete example

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

Here, configuration stays in the parameter list, the `IElement` subtree stays in the initializer, component Props drive local reactive presentation, multiline components have closing comments, and the single-line `HText` needs none.
