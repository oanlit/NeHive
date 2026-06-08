[![Language](https://img.shields.io/badge/README-English-green.svg)](README.md)
[![Language](https://img.shields.io/badge/README-中文-green.svg)](README-zh.md)

# NeHive.Model

> 通用运行时语义模型层

---

## 项目定位

NeHive.Model 是 NeHive 体系中的基础模型层，用于描述运行时系统中的核心语义结构，而不是提供具体功能实现。

它关注的是：

> 如何建模运行时系统中的结构、上下文与生命周期，而不是实现某个具体框架功能。

---

它可作为以下系统的基础模型层：

* UI 框架
* 响应式系统（Reactive）
* 依赖注入容器
* DSL 运行时系统
* 结构化执行环境

---

## 设计目标

NeHive.Model 的设计围绕以下核心目标：

### 1. 显式运行时语义

将传统隐式机制（如 ThreadLocal、AsyncLocal、隐式作用域传递）提升为显式模型结构。

---

### 2. 可组合运行时结构

运行时系统应由可组合的结构单元构成，而不是分散的隐式机制。

这些结构单元可以：

* 嵌套
* 传递
* 组合
* 构建层级关系

---

### 3. 一等运行时对象

运行时上下文不依赖编译期作用域，而是以可持有、可传递的对象形式存在。

---

### 4. 统一语义边界

将以下概念统一建模：

* 生命周期
* 上下文
* 执行边界

---

## 当前模型体系

### Scope（运行时作用域模型）

Scope 是当前提供的核心运行时模型，用于表达：

* 运行时结构节点
* 上下文传播边界
* 生命周期管理边界

Scope 是一个**一等运行时对象**，而不是代码块语义的延伸。

---

### Scope 结构语义

Scope 构成运行时树结构：

```text
RootScope
 ├── Scope A
 │    └── Scope A1
 └── Scope B
```

语义特性：

* 子 Scope 从父 Scope 继承 Context
* Scope 构成运行时结构树
* Scope 表达所有权关系，而不是引用关系

---

### 生命周期规则（重要）

Scope 之间存在严格的所有权关系：

* 子 Scope 由父 Scope 创建并持有
* 父 Scope Dispose 时递归释放所有子 Scope
* 子 Scope 生命周期不会超过其父 Scope

```csharp
var parent = new Scope();
var child = new Scope(parent);

parent.Dispose();
```

语义结果：

```text
child → 自动 Dispose
parent → Dispose
```

---

### Context 传播语义

Context 通过 Scope 树进行传播，而不是通过隐式环境：

```csharp
var key = new ContextKey<string>();

var root = new Scope();
root.SetContext(key, "Dark");

var child = new Scope(root);

using (new ScopeFrame(child))
{
    var value = key.GetContext();
}
```

语义：

```text
child → root → context lookup
```

---

### 执行上下文切换（ScopeFrame）

通过 ScopeFrame 显式切换当前运行时 Scope：

```csharp
using (new ScopeFrame(scope))
{
    Handle();
}
```

语义：

* 当前执行上下文绑定到 Scope
* 执行结束后自动恢复原上下文

---

## 典型模型行为

### 1. 运行时结构构建

Scope 可构建运行时结构树：

```csharp
var root = new Scope();
var child = new Scope(root);
var leaf = new Scope(child);
```

```text
root → child → leaf
```

---

### 2. 上下文显式传播

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

### 3. 生命周期绑定

```csharp
var scope = new Scope();

scope.OnCleanup += () =>
{
    ReleaseResources();
};

scope.Dispose();
```

---

### 4. 可持有运行时对象

Scope 可以脱离代码块存在：

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

## 设计本质

NeHive.Model 的本质是：

> 一个运行时语义建模层，用于统一表达结构、上下文与生命周期。

---

核心原则：

> 将运行时行为从隐式机制提升为显式模型结构。

---

## 与传统模型的区别

| 传统模型                     | NeHive.Model  |
| ------------------------ | ------------- |
| ThreadLocal / AsyncLocal | Scope 树结构     |
| 隐式上下文                    | 显式 ContextKey |
| `{}` 代码块                 | 运行时对象 Scope   |
| 分散生命周期管理                 | 统一生命周期模型      |
| 调用栈依赖                    | 结构树依赖         |

---

## 当前实现状态

当前 NeHive.Model 包含：

* Scope（运行时作用域）
* ContextKey（上下文键模型）
* ScopeFrame（执行上下文切换）

构成最小运行时语义闭包。

---

## 未来扩展方向

未来可能扩展以下运行时语义方向：

### 1. 受控状态语义

用于表达具有运行时语义约束的数据模型。

---

### 2. 运行时行为语义

用于表达函数执行与副作用边界的结构化模型。

---

### 3. 结构化执行语义

用于扩展运行时结构的组合能力与表达能力。

---

### 4. 并发与异步语义

在 Scope 结构基础上扩展结构化并发与异步执行语义。

## 开源协议
MIT