# UELyraLearn - Lyra 架构研究与插件化实践

这是一个针对虚幻引擎 5 **Lyra Starter Game** 的深度学习与解构项目。本项目重点关注模块化设计、数据驱动架构以及如何通过插件扩展游戏功能。

This repository is a deep-dive into the **Lyra Starter Game** in Unreal Engine 5. It focuses on modular design, data-driven architecture, and extending game features via plugins.

---

## 🏗️ 核心模块解构 | Core Modules

### 1. Experience 架构 (The Entry Point)
Lyra 的灵魂，作为“依赖注入”的入口，决定了游戏的逻辑构成。
The heart of Lyra, acting as a dependency injection entry point that defines the game's logic.

* **Experience + PawnData**: 定义了玩家进入游戏时的外观、属性及初始能力。
    *Defines player appearance, attributes, and initial abilities upon entry.*
* **GameplayFeatureAction**: 实现功能逻辑的“即插即用”，如动态添加组件或注册 Input Mapping。
    *Enables "Plug & Play" logic, such as dynamically adding components or registering Input Mappings.*
* **输入映射 (Input Mapping)**: 结合 Enhanced Input，通过 Experience 动态切换不同角色的操作方案。
    *Utilizes Enhanced Input to dynamically switch control schemes based on the active Experience.*

### 2. 库存与装备 (Inventory & Equipment)
基于“定义与实例分离”的设计，极大减少了内存占用并提高了可扩展性。
A decoupled "Definition vs. Instance" design, reducing memory overhead and improving scalability.

* **Inventory**: 使用 `UInventoryItemDefinition` 定义静态属性，`UInventoryItemInstance` 维护运行时状态。
    *Uses Definitions for static data and Instances for runtime state.*
* **Equipment**: 负责将物品转化为物理表现（Actor）并赋予对应的 Gameplay Abilities。
    *Translates items into physical Actors and grants relevant Gameplay Abilities.*

### 3. 消息总线 (Gameplay Message Subsystem)
基于 **GameplayTags** 的发布-订阅（Pub-Sub）模式，实现模块间完全解耦。
A Pub-Sub pattern based on **GameplayTags**, achieving complete decoupling between modules.
> *类似于 Zenject 的 Signal Bus，但通过 Tag 进行层级化路由。*
> *Similar to Zenject's Signal Bus, but with hierarchical routing via Tags.*

### 4. Lyra 特化 GAS (Specialized GAS)
Lyra 对原生 GAS 进行了工程化封装，更适应现代多端开发。
Lyra wraps native GAS for better engineering and multi-platform support.

* **GameplayCue (GFP)**: 表现层逻辑完全剥离到 GameFeature 插件中，实现逻辑与视觉分离。
    *Visual logic is fully decoupled into GameFeature plugins.*
* **Attribute Set**: 支持根据 PawnData 动态初始化和挂载属性集。
    *Dynamic initialization and mounting of Attribute Sets based on PawnData.*

---

## 🌊 Experience 加载流程 | Loading Sequence

Lyra 的异步初始化是保证流畅体验的关键。以下是其核心加载逻辑：
Lyra's asynchronous initialization is key to a seamless experience.

```mermaid
graph TD
    Start([WorldSettings: ExperienceID]) --> LoadDef[Load Experience Definition Asset]
    LoadDef --> LoadActions[Load GameFeatureActions]
    
    subgraph Initialization [Async Processing]
    LoadActions --> Action_Comp[GameFeatureAction: Add Components]
    LoadActions --> Action_Ab[GameFeatureAction: Grant Abilities]
    LoadActions --> Action_Input[GameFeatureAction: Register Input]
    end
    
    Action_Comp --> Ready[Experience Ready]
    Action_Ab --> Ready
    Action_Input --> Ready
    
    Ready --> Delegate[OnExperienceReady Delegate Fired]
    Delegate --> PawnSpawn[Pawn Extension Component: Finalize Initialization]
