# Implementation: Gameplay Feature Architecture & Initialization

> **Design Philosophy**: Lyra 采用插件化架构实现逻辑隔离。`ShooterCore` 是官方提供的射击玩法底层。开发新英雄或新模式时，应通过 **Dependency Injection (DI)** 的思路，基于底层核心构建特定的 Feature 插件。

---

## 1. 插件拓扑与核心角色 (Architecture Roles)

* **玩法核心 (Core Layer)**: 如 `ShooterCore` 或自定义的 `RPGCore`。负责定义该类型玩法的通用基础逻辑（如伤害计算、基础动画状态机、通用组件）。
* **功能特性 (Feature Layer)**: 显式依赖于核心层。包含具体的地图、`ExperienceDefinition` 以及用于 FrontEnd UI 导航的 `UserFacingExperienceDefinition`。



---

## 2. 插件初始化标准流程 (Standard Operating Procedure)

### Step 1: 容器初始化 (Plugin Creation)
1.  **路径**: `Edit -> Plugins -> Add`。
2.  **模板**: 选择 `Game Feature (Content Only)` 或 `Game Feature (with C++)`。
3.  **状态锚定**: 在插件根目录自动生成的 `GameFeatureData` (Data Asset) 中，将 **Initial State** 设置为 `Registered`。
    > **Architecture Note**: 设置为 `Registered` 是为了将生命周期管理移交给 Experience 系统，实现按需异步加载，而非启动时全量加载。

### Step 2: 资产管理器寻址配置 (Asset Manager Registry)
为了使全局 `UAssetManager` 能够识别并异步加载插件内的资源，必须在项目设置中配置扫描路径。

**路径**: `Project Settings -> Asset Manager -> Primary Asset Types to Scan`

| Index | Primary Asset Type | Asset Base Class | Directories |
| :--- | :--- | :--- | :--- |
| **01** | `Map` | `World` | `/YourPlugin/Maps` |
| **02** | `LyraExperienceDefinition` | `LyraExperienceDefinition` | `/YourPlugin/Experiences` |
| **03** | `LyraUserFacingExperienceDefinition` | `LyraUserFacingExperienceDefinition` | `/YourPlugin/System/UI` |



### Step 3: 建立依赖拓扑 (Dependency Declaration)
如果当前 Feature 属于扩展层（如基于射击底层的英雄插件），必须在 `.uplugin` 文件中声明显式依赖，否则在加载时会因缺少底层类信息而崩溃。

* **文件位置**: `Plugins/GameFeatures/[YourPlugin]/[YourPlugin].uplugin`
* **配置字段**: 
```json
"Plugins": [
    {
        "Name": "ShooterCore",
        "Enabled": true
    }
]
```
## 3. 入口配置：UserFacingExperience (Navigation Setup)
LyraUserFacingExperienceDefinition 是连接 UI 导航层 与 底层 Experience 逻辑 的桥梁。在 Lyra 默认的 FrontEnd 地图中，交互终端会搜索所有该类型的资产。

* **Map ID**: 指定插件内的地图资源路径（Soft Object Reference）。

* **Experience ID**: 关联对应的 LyraExperienceDefinition 资产。

* **Essential Config**: 决定了在 UI 界面中，玩家点击特定入口后触发哪套异步加载流水线。

## 4. 避坑与调试 (Pro-Tips)
**Asset Registry** 刷新: 若创建资产后在下拉列表中找不到，需手动点击 Asset Manager 的 Scan 按钮或重启编辑器。

**Implicit Dependency**: 若创建 RPG 类型玩法，切记不要盲目依赖 ShooterCore，应仿照其结构独立构建 RPGCore，以保持插件域的纯净。

**DOD** 视角: 每一个插件本质上是一个数据包，Experience 是这个包的索引。
