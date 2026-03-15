# 📦 库存与装备系统研究：Lyra vs. 自定义 Unity 架构
# Inventory & Equipment System: Lyra vs. Custom Unity Architecture

本项目深入探讨了工业级游戏中的库存架构。核心理念是 **“数据与实例分离”** 以及 **“组合优于继承”**。

This module explores industrial-grade inventory architectures. The core philosophy is **"Separation of Data and Instance"** and **"Composition over Inheritance"**.

---

## 🏛️ Lyra 库存架构解构 | Lyra Inventory Decoupling

Lyra 的库存系统是典型的虚幻引擎数据驱动设计：

### 1. 组合优于继承 (Composition over Inheritance)
在 Lyra 中，物品定义 (`UInventoryItemDefinition`) 不再通过复杂的类继承来区分功能（如武器类、消耗品类），而是通过 **Fragments (片段)**。
* **Fragments**: 每一个 Fragment 都是一个小的 `UObject`。
    * `ULyraInventoryItemFragment_Equippable`: 赋予装备能力。
    * `ULyraInventoryItemFragment_UI`: 赋予图标和描述。
* **优点**: 策划可以通过自由组合 Fragments 创建全新的物品类型，无需程序员修改代码。

### 2. 物品实例与标签 (Instance & GameplayTags)
* **Item Instance**: `UInventoryItemInstance` 包含指向定义（Definition）的指针，并存储动态数据。
* **GameplayTags**: 实例利用 Tag 存储临时状态（如“已装备”、“冷却中”），这比布尔值更具扩展性。

---

## 🚀 自定义 Unity 多容器架构 | Custom Unity Multi-Container Architecture

基于 **Zenject (DI)** 和 **SignalBus** 构建的高性能、低耦合库存系统。
A decoupled, high-performance inventory system built on **Zenject** and **SignalBus**.

### 1. 核心抽象与 DI 集成 | Core Abstraction & DI
系统采用抽象基类 `InventoryContainerBase`，利用 Zenject 的 `[Inject]` 注入 `SignalBus`，实现容器与外界的完全解耦。

```csharp
public abstract class InventoryContainerBase
{
    public int Capacity => _items.Length;
    public abstract ContainerType ContainerType { get; }
    
    [Inject]
    protected SignalBus _bus; // 基于信号总线的解耦通信
    
    protected ItemInstance[] _items;
    
    // 原子操作：确保数据改变时总是触发信号
    protected virtual void NotifyChange(int idx, ItemInstance newItem) =>
        _bus.Fire(new InventoryItemChangedSignal(ContainerType, idx, newItem));
}
```
### 2. 容器功能特性 | Container Features
* **多容器支持** (Multi-Container): 通过 ContainerType 区分背包、仓库、装备栏等。

* **原子扩容** (Atomic Expansion): 支持运行时动态调整容量。

* **工厂模式** (Factory Pattern): InventoryFactory 负责将 ItemData（静态/存档数据）转化为 ItemInstance（运行时对象）。
