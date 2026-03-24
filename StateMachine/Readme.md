# Data-Oriented & Decoupled State Machine (DDSM)

一套基于 **Unity ScriptableObject** 实现的高性能、高度解耦的数据驱动状态机系统。

## 🎯 设计哲学 (Philosophy)

本项目核心在于实现**逻辑与数据的彻底分离**。通过将状态切换逻辑从 `MonoBehaviour` 中抽离并资源化，开发者可以像配置表格一样构建复杂的 AI 或角色行为逻辑，而无需修改核心代码。

* **数据驱动**：状态（State）、条件（Condition）、转换（Translation）均为资源文件。
* **黑板模式**：通过 `Blackboard` 统一管理运行时数据，实现逻辑对具体实体的“无感知”。
* **性能导向**：在编辑期预构建映射表（Map）并进行权重排序，确保运行时 $O(1)$ 的检索效率。

---

## 🏗️ 核心模块说明

### 1. Blackboard (数据黑板)
作为状态机的“短期记忆”，`BaseBlackboard` 存储了当前实体的所有状态位（如 `IsHurt`, `IsDead`）。
> **解耦逻辑**：状态和条件只与黑板交互，不直接引用 `GameObject` 或具体组件。

### 2. State & Condition (逻辑定义)
* **BaseState**: 包含 `Enter`, `Exit`, `OnUpdate` 生命周期。支持通过 `IPhysicsState` 接口扩展物理层的 `FixedUpdate` 逻辑。
* **BaseCondition**: 纯粹的判定逻辑，作为状态切换的“门禁”。

### 3. Translation & Asset (架构拓扑)
* **BaseTranslation**: 连线资源，定义了 `From -> To` 的指向、触发条件以及**权重 (Weight)**。
* **BaseStateMachineAsset**: 状态机的静态拓扑图。
    * **预处理优化**：利用 `OnValidate` 在编辑器阶段完成字典构建和权重排序，避免运行时的性能损耗。

---

## 🚀 关键特性 (Features)

### 📈 权重优先级系统 (Priority System)
当一个状态存在多个出口且同时满足触发条件时，系统会根据 `Weight` 自动选择优先级最高的转换分支，这使得 AI 的行为表现更具确定性和可控性。

### ⚡ 极速状态检索
系统不再通过遍历 `List<Translation>` 来查找下一个状态，而是通过 `StateType` 在预构建的 `Dictionary` 中直接定位。

### 🔗 物理逻辑支持
通过接口检测，系统能平滑地在 `Update` 和 `FixedUpdate` 之间分发逻辑，完美适配需要刚体操作的物理状态。

---

## 🛠️ 扩展方向 (Roadmap)

1.  **Visual Editor**: 计划支持基于节点的图形化编辑界面。
2.  **ECS Integration**: 当前架构天然适配 **Data-Oriented Design (DOD)**。未来计划迁移至 Unity ECS，利用 `BlobAsset` 存储状态拓扑，并结合 `FunctionPointer` 实现超大规模实体的并行状态切换。
3.  **Generic Data Storage**: 增强黑板系统，支持自定义 key-value 或泛型数据存储。

---

## 📝 License
MIT License
