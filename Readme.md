# Cross-Engine Architecture & System Labs | 跨引擎架构与系统实验库

[English](#english) | [中文](#中文)

---

<a name="english"></a>
## 🇺🇸 English Version

### 1. Project Mission
A comprehensive technical archive dedicated to the deep deconstruction and cross-engine implementation of AAA-grade game systems. This repository focuses on mapping **UE5's industrial standards (Lyra/GAS/CommonUI)** to **Unity's modern high-performance C# ecosystem (HybridCLR + DI + Data-Oriented)**.

### 2. Technical Stack Matrix
* **Engines:** Unreal Engine 5 (C++/Blueprint) & Unity (C# / HybridCLR)
* **Architecture:** Dependency Injection (Zenject), Gameplay Ability System (GAS), Message Router
* **Backend & Networking:** Prediction & Rollback logic, Network Data Serialization
* **Tooling:** Odin Inspector (Unity), Data Assets (UE5)

### 3. Systematic Breakdown
* **Combat & Ability:** HybridCLR-based GAS reproduction; Ability granting & Tag-driven logic.
* **Inventory & Equipment:** Fragment-based item definitions (Lyra-inspired) vs. SO-based modular items.
* **Quest & State:** Hierarchical state machines and decoupled quest triggering systems.
* **Network Module:** Research on Prediction/Rollback and Authority-side validation (NEF/GAS patterns).

---

<a name="中文"></a>
## 🇨🇳 中文说明

### 1. 项目愿景
本项目是一个深度的跨引擎技术存档，专注于游戏系统的底层拆解与跨平台复刻。核心目标是建立一套从 **UE5 工业标准 (Lyra/GAS/CommonUI)** 到 **Unity 现代高性能开发范式 (HybridCLR + DI + 面向数据设计)** 的技术映射矩阵。

### 2. 技术栈矩阵
* **引擎端:** Unreal Engine 5 (C++ / 蓝图) & Unity (C# / HybridCLR)
* **架构模式:** 依赖注入 (Zenject), 技能系统 (GAS), 全局消息路由 (Message Router)
* **网络与底层:** 预测回滚机制, 网络序列化优化, 权威服务器校验
* **工具链:** Odin Inspector 扩展, 数据驱动 (Data Assets / ScriptableObject)

### 3. 系统模块划分
* **战斗与技能 (Combat):** 基于 HybridCLR 的轻量化 GAS 实现；能力授权机制与标签驱动逻辑。
* **库存与装备 (Inventory):** 受 Lyra 启发的 Fragment 式物品定义 vs Unity 模块化 SO 设计。
* **任务与状态 (Quest):** 层级状态机架构与完全解耦的任务触发/追踪系统。
* **网络模块 (Networking):** 深入研究预测回滚 (Prediction/Rollback) 与权威帧同步阈值判定。

---

### 🧠 Design Philosophy & Trade-offs | 设计哲学与权衡

#### Q: Why maintain a cross-engine lab instead of a single framework?
**A:** Understanding how different engines solve the same problem (e.g., Inventory in Lyra vs. Unity) reveals the underlying **Engine-Agnostic Patterns**. My goal is to master the "Logic" that survives the engine transition.

#### 问：为什么要建立跨引擎实验室，而不是做一个单一框架？
**答：** 研究不同引擎如何解决同一个问题（例如 Lyra 的库存 vs Unity 的库存）可以揭示背后的**“引擎无关模式”**。我的目标是掌握那些能够跨越引擎鸿沟的底层逻辑与架构思维。

---

### 📊 System Mapping Layer (Architecture)

```mermaid
graph TD
    subgraph "Unreal Side (Reference)"
    U1[GAS/GameplayTags] --> U2[Message Router]
    U2 --> U3[Lyra Inventory/Quest]
    end

    subgraph "Unity Side (Implementation)"
    N1[C# Tag System] --> N2[DI-Injected Bus]
    N2 --> N3[HybridCLR Logic Modules]
    end

    U1 -.->|Logic Mapping| N1
    U3 -.->|Pattern Porting| N3
