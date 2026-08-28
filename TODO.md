# NeHive TODO

> This roadmap records known engineering gaps and verifiable development directions for NeHive. Priorities and proposed solutions may evolve with practical experience; they are not immutable commitments.

## Core direction

NeHive will continue to develop around these core primitives:

- `Scope`: unifies structure, context, and lifecycle;
- `Signal`: unifies reactive state;
- `Effect`: unifies synchronization between state and external systems;
- `IElement`: unifies UI structure;
- `strStyle`: handles high-tolerance visual presentation without expanding into core component behavior.

## P0: Unify UI component implementation

### Component API consistency

- [ ] Define when Props expose a read-only Signal or writable `MutSignal`.
- [ ] Standardize Avalonia Property initialization, update targets, and cleanup behavior.
- [ ] Audit ordinary parameter, `out`, and `Func<TProps, TArgs>` component overloads.
- [ ] Establish a component capability matrix covering static properties, reactive properties, two-way binding, Props, style variants, and cleanup tests.
- [ ] Audit focus, keyboard, IME, validation, and accessibility behavior.

### Reduce mechanical repetition

- [ ] Introduce a consistent Avalonia Property Bridge abstraction.
- [ ] Reduce repeated PropertyChanged subscription and cleanup logic in `StyleTypes.cs`.
- [ ] Evaluate generating Props properties, Args parameters, and component overloads.
- [ ] Add snapshot tests for generated output.

### Define the Generator's role

- [ ] Decide whether the Generator only serves Stores or becomes infrastructure for UI APIs.
- [ ] Decide whether to publish the Generator as a separate Analyzer NuGet package.
- [ ] Add an explicit `#nullable` context to generated sources.
- [ ] Extend tests for generics, nested types, file-scoped namespaces, and type-name conflicts.

### Refactor the Avalonia sample

- [ ] Split the oversized `MainNavDemo.cs` so demos, navigation mappings, and shared styles are not concentrated in one file.
- [ ] Organize demo files by responsibility, such as layout, input, control flow, styling, and advanced scenarios.
- [ ] Consolidate the demo catalog, categories, and view factories into one maintainable navigation data source.
- [ ] Make every refactored demo follow the [UI Component API and Writing Conventions](docs/UI-CONVENTIONS.md).
- [ ] Normally omit trailing comments for single-line components; mark structural boundaries for multiline components and multiline named slots.
- [ ] Determine member placement by type: types containing `IElement` belong in initializers; all others belong in parameter lists.
- [ ] Return UI directly when no lifecycle work, additional events, or effects follow; use `rootElement` when component assembly continues.
- [ ] Preserve demo coverage and navigation behavior, and add basic regression verification for the extracted components.

### Responsive `strStyle` breakpoints

- [ ] Design Tailwind-like `sm:`, `md:`, and `lg:` responsive variant syntax.
- [ ] Decide whether breakpoints observe the window, root container, or component container size.
- [ ] Define default breakpoint values, custom registration, and override rules.
- [ ] Define precedence among base styles, breakpoint styles, and interaction variants.
- [ ] Allow breakpoint prefixes to compose with existing high-tolerance visual properties without introducing fractional-width layout semantics.
- [ ] Keep layout responsibility explicit: proportional widths, column sizing, and remaining-space allocation belong to containers such as `HGrid`; do not add `w-1/2`-style tokens.
- [ ] Update only affected style state when a size crosses a breakpoint; do not rebuild the UI subtree.
- [ ] Test boundary values, continuous resizing, dynamic `strStyle`, and combinations with variants such as `hover:`.
- [ ] Add an Avalonia demo that makes `sm:` / `md:` / `lg:` behavior easy to verify visually.

## P1: Stabilize foundational semantics

### UI lifecycle tests

- [ ] Create a `NeHive.UI.Avalonia.Tests` project.
- [ ] Verify Element mounting, disposal, and repeated disposal.
- [ ] Verify that `UiScope.OnMount` runs exactly once.
- [ ] Verify immediate invocation when `OnMount` is added to an already-mounted Scope.
- [ ] Verify that `Show`, `Switch`, `Match`, and `Loading` dispose old branch scopes.
- [ ] Verify node identity and Scope ownership during `ForEach` insertion, removal, movement, and sorting.
- [ ] Verify that disposed Elements no longer receive Signal updates.

### UI property and event tests

- [ ] Verify that Signals update only dependent Avalonia properties.
- [ ] Verify lazy creation, updates, and cleanup of Props properties.
- [ ] Verify that two-way bindings do not create feedback loops.
- [ ] Verify that additional event handlers are detached during `OnCleanup`.
- [ ] Verify static, reactive, and interaction-variant `strStyle` behavior.

### Scope and error handling

- [ ] Preserve original exceptions raised during Scope cleanup.
- [ ] Throw `AggregateException` or a dedicated `ScopeCleanupException` when multiple cleanup callbacks fail.
- [ ] Include Scope and cleanup-source information in cleanup diagnostics.
- [ ] Add regression tests for idempotent disposal, recursive child disposal, and continued cleanup after an exception.

### Threading and scheduling semantics

- [ ] Formally document the single-threaded NeHive.Reactive runtime contract.
- [ ] Define the boundary between Avalonia's UI thread and the Reactive runtime.
- [ ] Establish a consistent scheduling approach for cross-thread Signal writes.
- [ ] Verify that queued Dispatcher callbacks cannot mutate invalid state after Scope disposal.
- [ ] Define support boundaries for `Scope.CurrentScope` and `ScopeFrame` across async flows, threads, and multiple windows.

### Sample resource ownership

- [ ] Remove or explicitly manage the long-lived static Scope in `MusicPlayerDemo`.
- [ ] Define ownership for creating, replacing, and disposing `Media` instances.
- [ ] Validate playlist indexes before and after asynchronous parsing.
- [ ] Explicitly pair LibVLC event subscriptions and unsubscriptions.
- [ ] Make the music-player sample follow the component code order in the UI writing conventions.

## P2: Diagnostics, documentation, and performance

### `strStyle` diagnostics

- [ ] Provide non-blocking diagnostics for unknown tokens and parse failures.
- [ ] Define token, message, severity, and component source for `StyleDiagnostic`.
- [ ] Provide a configurable diagnostic sink instead of writing directly to `Console.WriteLine` from library code.
- [ ] Preserve the high-tolerance nature of `strStyle`; purely visual errors should not stop component behavior by default.

### Public API documentation

- [ ] Resolve `CS1591` warnings for public members.
- [ ] Fix invalid `typeparam`, `paramref`, and `cref` documentation references.
- [ ] Align README files and demos with current API names, including `HText` / `HTextBlock`.
- [ ] Document and compare the intended use of `Value`, `RxValue`, `Pull`, and `Track`.
- [ ] Add documentation for Effects, async epochs, cross-thread state, and resource cleanup.
- [ ] Keep the [UI Component API and Writing Conventions](docs/UI-CONVENTIONS.md) synchronized with the demos.

### Performance baselines

- [ ] Create a BenchmarkDotNet project.
- [ ] Establish baselines for Signal reads/writes, Effect propagation, Computed chains, and Batch.
- [ ] Establish baselines for `ForEach` insertion, removal, movement, and large-list updates.
- [ ] Measure allocations for bulk Element creation and disposal.
- [ ] Establish a baseline for reactive `strStyle` updates.
- [ ] Support performance claims with reproducible measurements.

## P3: Developer experience and production validation

### Developer tooling

- [ ] Explore `strStyle` completion, unknown-token hints, and safe rename support.
- [ ] Explore a Scope Tree inspector.
- [ ] Explore Signal dependency visualization and Effect execution tracing.
- [ ] Verify Hot Reload support boundaries.
- [ ] Evaluate possible UI Preview implementations.
- [ ] Provide project templates and common component snippets.

### Production scenarios

- [ ] Validate multiple windows and multiple root Scopes.
- [ ] Run long-duration and resource-leak tests.
- [ ] Validate large lists, virtualization, and high-frequency updates.
- [ ] Validate file, network, media, and other asynchronous resources.
- [ ] Validate high DPI, IME, keyboard navigation, and accessibility.
- [ ] Establish continuous integration on Windows, Linux, and macOS.
- [ ] Validate NativeAOT, trimming, and interoperability with third-party Avalonia controls.
- [ ] Evolve the music player into a complete architecture-validation application.

## Pre-release checklist

- [ ] All tests pass.
- [ ] No new compiler warnings are introduced.
- [ ] Public API changes are documented.
- [ ] README files, package documentation, demos, and actual APIs agree.
- [ ] Scope cleanup and event-resource ownership have been audited.
- [ ] NuGet package versions, dependencies, README, license, and symbol packages are correct.

## Current non-goals

- Do not pursue control count at the expense of behavioral consistency in existing components.
- Do not turn Reactive into a fully multithreaded system before its threading boundaries are defined.
- Do not extend the `strStyle` string DSL into state, events, child-element structure, or resource lifetime.
- Do not reintroduce many unrelated concepts merely for convenience.

## Acceptance principles

A task should not be considered complete merely because code has been written. In principle, completion should also include:

- automated tests or a reproducible verification method;
- documentation and demo updates when public APIs are affected;
- explicit creation, replacement, and disposal responsibilities when lifecycle is involved;
- baseline comparisons when performance is involved;
- documented scheduling and invalidation behavior when cross-thread execution is involved.
