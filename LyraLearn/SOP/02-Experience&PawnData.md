# Implementation 02: Experience & PawnData Architecture

> **Architecture Core**: Lyra 通过 `Experience` 实现了对传统 `GameMode` 的解耦。玩法不再是硬编码的逻辑类，而被抽象为一套**可动态加载的静态配置**。其核心机制是通过 `FeatureAction` 实现对 Actor 的**逻辑注入 (Dependency Injection)**。

---

## 1. Experience Definition: 玩法骨架配置

`LyraExperienceDefinition` 是定义游戏规则的总纲，其核心任务是协调资源加载与功能注入。

### 核心配置项清单 (Core Configuration)

| 配置项 | 工业意图 |
| :--- | :--- |
| **Game Features to Enable** | **环境隔离**: 声明该玩法强制依赖的插件域，确保相关资产在初始化前完成加载。 |
| **Default Pawn Data** | **身份定义**: 定义玩家在当前 Experience 下的角色原型（Pawn Prototype）。 |
| **Actions (Feature Actions)** | **行为注入**: Lyra 的“手术刀”，用于向现有系统动态切入逻辑（见下文）。 |
| **Action Sets** | **模块复用**: Feature Action 的预制包，用于在不同玩法间快速同步标准配置（如通用 UI 层）。 |

### 常用注入器 (Essential Feature Actions)
* **Add Components**: 动态为 `AActor`（通常是 `LyraCharacter`）挂载组件。
    > *Ref: 解决了组件在 C++ 构造函数中硬编码的问题，支持按需挂载如 `InventoryComponent`。*
* **Add Abilities**: 为持有 `ASC` 的对象授予 `GameplayAbilities` (GA)、`Attributes` (AS) 及 `Effects` (GE)。
* **Add Widgets**: 将玩法相关的 UI 布局（如 HUD）推送到 `CommonUI` 框架中。

---

## 2. Pawn Data: 数据驱动的实体定义

`PawnData` 是角色的“基因库”。它不包含逻辑流，仅存储逻辑运行所需的**元数据映射**。

### 关键映射逻辑 (Mapping Logic)

#### A. Input Config (输入协议)
Lyra 舍弃了旧版的按键字符串绑定，采用 **Enhanced Input + Gameplay Tags**。
* **映射逻辑**: `InputAction (硬件输入)` -> `Gameplay Tag (语义标签)`。
* **优势**: 逻辑层只监听 `Native.Ability.Jump` 这种标签，而不关心玩家按的是空格还是手柄 A 键。

#### B. Ability Sets (能力清单)
* **功能**: 定义角色初始持有的技能包。
* **DOD 视角**: 这是一组能力的“索引列表”，在 Pawn Spawn 时由 Experience 系统读取并触发授权。

#### C. Tag Relationship Mapping (标签关系表)
* **核心**: 定义标签之间的交互逻辑（如：当拥有 `Status.Stun` 标签时，屏蔽 `Input.Ability` 分支下的所有输入）。

---

## 3. 架构审计：为什么这样设计？ (Architecture Insights)

1.  **非侵入式开发**: 通过 `AddComponents` Action，你可以在不修改 `ALyraCharacter` C++ 源码的情况下，为英雄添加全新的系统（如：RPG 的魔力值系统）。
2.  **资产粒度控制**: `ActionSet` 允许我们将“基础 UI”、“基础移动技能”打包。新玩法的创建只需堆砌这些 `ActionSet`，极大地降低了 SOP 的重复劳动。
3.  **适配热更新**: 在 **HybridCLR** 环境下，我们可以通过修改 `PawnData` 的资产引用，在不重编代码的情况下，动态更换热更层驱动的脚本逻辑。

---

## 4. 实操 CheckList (SOP)

- [ ] 创建 `PawnData`，指定 `PawnClass` 并关联 `InputConfig`。
- [ ] 创建 `ExperienceDefinition`，在 `Actions` 数组中添加 `AddComponents` 以注入业务组件。
- [ ] 若有复用逻辑（如通用 Crosshair UI），创建 `ActionSet` 并在 `Experience` 中引用。
- [ ] 将 `Experience` 填入 `UserFacingExperienceDefinition` 进行入口测试。

---

**Next Step**: 
- [ ] 深入 `Enhanced Input`：解析从 `InputConfig` 到 `HeroComponent` 的绑定流水线。

