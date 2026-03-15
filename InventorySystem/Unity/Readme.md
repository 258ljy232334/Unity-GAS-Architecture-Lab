# Unity Inventory System Architecture (Custom Implementation)

本项目展示了一套基于 **Zenject (Dependency Injection)** 和 **SignalBus** 构建的高度解耦库存系统。该系统设计初衷是为了在复杂项目中提供极高的可扩展性与逻辑清晰度。

This module demonstrates a highly decoupled inventory architecture built with **Zenject** and **SignalBus**, designed for scalability and maintainability in large-scale Unity projects.

---

## 💎 架构核心亮点 | Architectural Highlights

### 1. 策略模式：逻辑与数据的彻底分离 (Strategy Pattern)
系统通过 `ConsumableStrategySO` 将物品的“使用效果”从“数据定义”中完全剥离。
* **组合优于继承**：物品的功能不再由类继承决定，而是通过在 `ConsumableInformation` 中注入不同的 `StrategySO`（如回血、加 Buff）来定义。
* **设计模式应用**：典型的策略模式实践，支持在不触动核心代码的前提下，通过编辑器配置快速扩展物品行为。



### 2. 反应式容器管理 (Reactive Container Management)
`InventoryContainerBase` 作为底层基石，严格遵循 **“数据驱动 UI”** 的原则。
* **零耦合通信**：容器不持有任何 UI 引用。所有的状态变更（添加、移除、交换、扩容）均通过 `NotifyChange` 抛出信号。
* **SignalBus 驱动**：利用 Zenject 的信号系统实现反应式刷新，使得 UI、音效、成就系统可以独立订阅变更，极大地降低了模块间的耦合度。

### 3. 指令与查询职责分离 (CQS - Command Query Separation)
我将库存操作抽象为独立的 Service 接口：
* **`IInventoryCommandService`**: 负责“写”操作。作为系统逻辑的唯一入口，确保所有的修改请求（如 `TryMoveItem`）都经过统一的边界检查。
* **`IInventoryQueryService`**: 负责“读”操作。提供高性能的只读访问。

### 4. 插件式分发工厂 (Pluggable Factory Design)
`InventoryFactory` 采用了分发器模式（Dispatcher Pattern）：
* **HybridCLR 友好**：系统通过注入 `List<IItemFactory>` 运行。在热更新层，可以动态注入新的子工厂来处理特殊物品实例（如带随机词条、耐久度的物品），无需修改主工程逻辑。

---

## 🌊 核心交互流 | Core Interaction Flow

```mermaid
graph TD
    UI[UI Event/Input] -->|Call| Command[IInventoryCommandService]
    Command -->|Logic Check| Container[InventoryContainer]
    Container -->|Modify Array| Data[ItemInstance Array]
    Container -->|Fire Signal| Bus((SignalBus))
    Bus -->|Notify| UI_Update[UI Inventory View]
    Bus -->|Notify| VFX[Visual/Audio Effects]
