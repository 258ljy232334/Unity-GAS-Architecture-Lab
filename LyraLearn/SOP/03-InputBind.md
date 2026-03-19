# Implementation 03: Input Mapping & AbilitySet Injection

> **Architecture Core**: Lyra 的输入系统是一套“语义化分发协议”。它通过 `IMC` 捕获硬件信号，通过 `InputConfig` 转换为 `GameplayTag`，最终在 `AbilitySet` 中完成从 Tag 到 `GameplayAbility (GA)` 的逻辑映射。

---

## 1. Input Mapping Chain: 硬件到语义的流转

Lyra 强制将输入拆分为两条平行链路，这种设计在解耦的同时引入了极高的**配置碎片化**风险。

### A. Native Actions (硬编码链路)
* **流程**: `IA` -> `UHeroComponent` -> `C++ Member Function`。
* **主要用途**: 基础移动（Move）、视角（Look）。
    * **逻辑断层**: Native Action 绕过了 GAS 系统。如果你想在某个状态下（如“定身”）屏蔽移动，你必须在 C++ 函数里手动判断 Tag，或者修改 IMC 的优先级。
    * **热更死角**: 对于使用 **HybridCLR** 的项目，Native Actions 极其难以被热更层拦截或重写，属于“架构硬块”。

### B. Ability Actions (Tag 驱动链路)
* **流程**: `IA` -> `InputTag` -> `ASC` -> `ActivateAbilityByTag`。
* **主要用途**: 所有的战斗技能、交互逻辑。
    * **性能抖动**: 每次按键都会触发一次映射表查找。虽然单次开销极小，但在高频战斗中（如 ACT 或帧同步逻辑），这种基于字符串/Tag 的动态匹配比直接函数回调慢了一个量级。
    * **调试地狱**: 当按键没反应时，你需要检查：`IMC` 是否激活 -> `IA` 是否填入 `InputConfig` -> `InputTag` 是否匹配 -> `AbilitySet` 是否授予 -> `GA` 的 `ActivationTag` 是否正确。链路过长导致排查成本极高。

---

## 2. DataAssets: 核心配置协议

### LyraInputConfig (映射表)
该文件将 `InputAction` 与 `GameplayTag` 强绑定。
* **关键点**: 它是 `HeroComponent` 绑定输入时的唯一索引。
* **缺陷**: 缺乏**唯一性约束**。如果两个不同的 IA 绑定了同一个 Tag，或者一个 IA 绑定了多个 Tag，系统不会报错，但会导致技能触发紊乱。

### LyraAbilitySet (能力包)
负责将 `GA`、`AttributeSet` 和 `GameplayEffect` 批量授予 Pawn。
* **关键点**: 通过 `AbilitySet` 定义按键（Tag）与技能类（Class）的对应关系。
* **缺陷**: **内存冗余**。Lyra 倾向于一次性给玩家授予所有可能的 GA。在一个拥有几十个技能的长线 RPG 中，这会导致 `ASC` 挂载过多的 `AbilitySpec`，增加每帧遍历的开销。

---

## 3. SOP: 工业级注入流程

1.  **资产创建**: 在 `Input` 目录下创建 `IA_Ability_Interact` 及对应的 `IMC`。
2.  **映射定义**: 创建/修改 `LyraInputConfig` (DA)，在 `AbilityInputActions` 数组中添加：
    * `InputAction`: `IA_Ability_Interact`
    * `InputTag`: `InputTag.Ability.Interact`
3.  **能力绑定**: 在 `LyraAbilitySet` (DA) 的 `GrantedGameplayAbilities` 中：
    * `Ability`: `GA_Interact`
    * `InputTag`: `InputTag.Ability.Interact`
4.  **动态注入 (Critical)**: 
    * 模仿 `LAS_ShooterGame_SharedInput` 创建一个 `LyraInputSet` (ActionSet)。
    * 在对应的 `ExperienceDefinition` 的 `Actions` 列表中添加 `AddInputConfig`（注入映射关系）和 `AddAbilities`（注入能力集）。

---

## 4. 架构缺陷分析与热更建议 (HybridCLR)

### 缺陷 01: 静态资产与动态逻辑的冲突
`AbilitySet` 引用的是 C++ 类或蓝图类。
* **风险**: 在 **HybridCLR** 环境下，你无法直接在 DA 文件里填入热更层的 C# 类。
* **对策**: 采用**“代理 GA”**模式。C++ 层只配置一个通用的 `GA_Bridge`，该 GA 激活后通过 `GameplayMessageRouter` 发送 Tag 消息，热更层 C# 脚本订阅该消息并执行具体逻辑。

### 缺陷 02: IMC 的优先级战争
* **现象**: Lyra 通过 `GameFeatureAction` 注入 `IMC`。
* **风险**: 如果多个插件同时注入 IMC 且优先级相同，按键响应顺序将变得不可控。
* **结论**: 必须在 SOP 中强制规定 `Priority` 步长（如：基础插件 10，玩法插件 20，临时 UI 100），禁止使用默认值。


