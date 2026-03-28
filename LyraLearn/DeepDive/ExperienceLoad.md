# 🛠️ Lyra 架构笔记：Experience 异步加载与注入管线

> **核心定位**：`ULyraExperienceManagerComponent` 是整个玩法逻辑的“装配中心”。它挂载于 `LyraGameState` 之上，利用 **Game Feature** 插件系统实现玩法逻辑与基础框架的彻底解耦。

---

## 1. 核心组件关系
* **宿主 (Host)**：`ALyraGameState`
* **管理器 (Manager)**：`ULyraExperienceManagerComponent`
* **驱动源 (Source)**：`ULyraExperienceDefinition` (Data Asset)
* **设计模式**：**异步依赖注入 (Async DI)**。Pawn 本身是“空壳”，所有功能（组件、技能、输入）均由该组件在运行时动态“缝合”。

---

## 2. 核心加载流程

### 阶段一：资源预检与普查 (Pre-Load)
1.  **入口**：调用 `SetCurrentExperience()` 确定目标玩法。
2.  **执行**：进入 `StartExperienceLoad()`。
3.  **分端过滤**：
    * 通过 `OwnerNetMode` 判断当前环境（Client / Server）。
    * **工业优化**：利用 `Bundles` 机制，服务端仅加载 `Server` Bundle（逻辑/属性/碰撞），跳过材质、贴图等渲染资源。这极大地提升了 Dedicated Server 的启动速度。

### 阶段二：异步资源加载 (Async Loading)
1.  **工具**：利用 `UPrimaryAssetManager` 发起异步请求。
2.  **任务聚合**：使用 `CreateCombinedHandle` 将所有 `ActionSet` 和插件资源的 `StreamableHandle` 合并。
3.  **阻塞点**：该阶段是异步的，只有当合并后的 `Handle` 完成（所有资源进入内存）时，才会流转到回调函数。



### 阶段三：插件激活与递归加载 (Plug-in Activation)
1. **函数入口**：`OnExperienceLoadComplete()`
2. **插件普查**：
   - 遍历 `CurrentExperience` 及所有 `ActionSets`。
   - 收集所有 `GameFeaturesToEnable` 插件的 URL。
3. **异步加载插件**：
   - 设置计数器 `NumGameFeaturePluginsLoading`。
   - 循环执行 `LoadAndActivateGameFeaturePlugin`。
4. **状态流转**：
   - 如果有插件：状态转为 `LoadingGameFeatures`，等待计数器归零。
   - 如果无插件：**直接触发** `OnExperienceFullLoadCompleted()`。

### 阶段四：全载入完成 (Full Load Completed)
- **触发点**：`OnExperienceFullLoadCompleted()`。
- **职责**：
  - 此时 **资源** 和 **插件** 已全部 Ready。
  - 执行所有 `Action->OnGameFeatureActivating()`（真正的组件/技能注入发生在这里）。
  - 最后发送 `OnExperienceLoaded` 广播。



---

## 3. 架构深度洞察 (Architecture Insights)

### 为什么挂载在 GameState 上？
* **生命周期一致**：玩法（Experience）的生命周期与对局（Match/GameState）完全同步。
* **同步权威性**：GameState 在服务器上具有权威性，由它管理 Experience 的状态可以确保所有连接的客户端都能同步加载正确的玩法插件。

### 关于“解耦”的 DI 思考
* **逻辑切片化**：武器、任务、库存系统被拆分为独立的 `GameFeatureAction`。
* **延迟注入**：利用 `GFCM`（组件管理器）解决了异步加载导致的“先后顺序”问题。即便 Pawn 先生成，Action 后加载，系统也能在 Action 激活的一瞬间将组件“补丁”上去。

---
