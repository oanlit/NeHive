[![Language](https://img.shields.io/badge/README-English-green.svg)](README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)
# NeHive.Model

> A general-purpose runtime semantic model layer

---

## 1. Overview

NeHive.Model is the foundational model layer of the NeHive ecosystem.

It defines a set of **runtime semantic abstractions** for representing:

* Execution structure
* Context propagation
* Lifecycle management

It does not aim to provide application-level frameworks or features.

Instead, it focuses on:

> How runtime systems should be modeled, not how specific systems are implemented.

---

It can serve as a base layer for:

* UI frameworks
* Reactive systems
* Dependency injection systems
* DSL runtimes
* Structured execution environments

---

## 2. Design Goals

NeHive.Model is built around the following principles:

### 1. Explicit runtime semantics

Replace implicit runtime mechanisms (e.g. thread-local storage, ambient context) with explicit model structures.

---

### 2. Composable runtime structures

Runtime systems are modeled as composable units that can:

* Be nested
* Be passed
* Be combined
* Form hierarchical structures

---

### 3. First-class runtime objects

Runtime context is represented as first-class objects rather than lexical scope constructs.

These objects can be:

* Created
* Held
* Passed
* Stored
* Composed

---

### 4. Unified semantic boundaries

The model unifies three core runtime concerns:

* Lifecycle
* Context
* Execution boundary

---

## 3. Core Model

### Scope

`Scope` is the primary runtime model currently provided.

It represents a **runtime execution node** that encapsulates:

* Hierarchical structure
* Context propagation boundary
* Lifecycle boundary

A Scope is not a lexical block.

It is a first-class runtime object.

---

### Scope Hierarchy

Scopes form a tree structure:

```text
RootScope
 ├── Scope A
 │    └── Scope A1
 └── Scope B
```

Semantic rules:

* Child scopes inherit context from parent scopes
* Scopes form an ownership-based hierarchy
* Lifecycle flows through the hierarchy

---

### Lifecycle Ownership Rule (Important)

Scopes follow a strict ownership model:

* A parent scope owns its child scopes
* Disposing a parent scope recursively disposes all child scopes
* A child scope cannot outlive its parent

```csharp
var parent = new Scope();
var child = new Scope(parent);

parent.Dispose();
```

Result:

* `child` is automatically disposed
* `parent` is disposed after children

---

### Context Propagation

Context values are stored in a `Scope` and resolved through the scope hierarchy:

```csharp
var key = new ContextKey<string>();

var root = new Scope();
root.SetContext(key, "Dark");

var child = new Scope(root);
```

Lookup:

```csharp
using (new ScopeFrame(child))
{
    var value = key.GetContext();
}
```

Resolution follows:

```text
child → parent → ancestor lookup
```

---

### Execution Context Switching (ScopeFrame)

`ScopeFrame` provides explicit runtime context switching:

```csharp
using (new ScopeFrame(scope))
{
    Handle();
}
```

It temporarily sets:

* `CurrentScope = scope`

and restores the previous scope after execution.

---

## Typical Model Behaviors

### 1. Runtime structure construction

Scopes can form runtime trees:

```csharp
var root = new Scope();
var child = new Scope(root);
var leaf = new Scope(child);
```

```text
root → child → leaf
```

---

### 2. Explicit context propagation

```csharp
var themeKey = new ContextKey<string>();

var scope = new Scope();
scope.SetContext(themeKey, "Dark");

using (new ScopeFrame(scope))
{
    var theme = themeKey.GetContext();
}
```

---

### 3. Lifecycle management

```csharp
var scope = new Scope();

scope.OnCleanup += () =>
{
    ReleaseResources();
};

scope.Dispose();
```

---

### 4. First-class runtime object usage

Scopes can exist independently of lexical blocks:

```csharp
Scope scope = new Scope();

Cache(scope);

Task.Run(() =>
{
    using (new ScopeFrame(scope))
    {
        Execute();
    }
});
```

---

## Design Philosophy

The core idea behind NeHive.Model is:

> Runtime behavior should be expressed as explicit, composable semantic structures rather than implicit execution mechanisms.

---

A Scope is therefore:

> A runtime execution structure, not a lexical construct.

---

## Comparison with Traditional Models

| Traditional Model              | NeHive.Model                |
| ------------------------------ | --------------------------- |
| ThreadLocal / AsyncLocal       | Scope hierarchy             |
| Implicit ambient context       | Explicit ContextKey         |
| Lexical block `{}`             | Runtime Scope object        |
| Scattered lifecycle management | Unified lifecycle ownership |
| Call-stack dependency          | Structural hierarchy        |

---

## Current Implementation

NeHive.Model currently provides:

* `Scope` — runtime execution model
* `ContextKey<T>` — context key abstraction
* `ScopeFrame` — execution context switcher

Together they form the minimal runtime semantic kernel.

---

## Future Directions

Future extensions may evolve toward additional runtime semantic models:

### 1. Controlled state semantics

A model for describing state with runtime-aware behavior and constraints.

---

### 2. Execution semantics

A model for representing function execution, side effects, and invocation structure.

---

### 3. Structured execution model

Extensions for richer runtime composition and structured execution flows.

---

### 4. Concurrency semantics

Models for structured asynchronous and concurrent execution built on top of Scope hierarchy.

## License
MIT