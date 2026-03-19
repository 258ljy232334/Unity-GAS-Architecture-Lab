# Implementation 04: Inventory System - Fragmented Data & Tag-Based State

> **Architecture Core**: Lyra 的库存系统是“组合优于继承”的极端实践。它通过 `Fragment` 模式实现静态属性组合，通过 `GameplayTagStack` 模拟动态实例数据，彻底消灭了冗余的 `Item` 继承树。

---

## 1. Item Definition: 资产的实体组件化 (ECS-like)

`LyraInventoryItemDefinition` 本质上是一个空壳容器，仅持有一个 `ULyraInventoryItemFragment` 数组。

### 核心设计评价 (Critical Audit)
* **组合优于继承**: 
    * **实现**: 没有 `WeaponDefinition` 或 `ConsumableDefinition`。如果你想让一个物品既能当武器又能当药水，只需同时挂载 `UInventoryFragment_EquippableItem` 和 `UInventoryFragment_Consumable`。
        * **优点**: 极大地降低了资产类型的膨胀。
        * **缺陷 - 数据散乱**: 逻辑层（如 UI 或 战斗系统）必须频繁使用 `FindFragmentByClass<T>`。这种操作虽然有缓存优化，但在大规模遍历时仍比直接访问成员变量慢。
        * **缺陷 - 编辑器负担**: 由于一切皆 Fragment，美术和策划在创建资产时需要手动添加大量小组件，缺乏类型强约束，极易漏填关键片段。

---

## 2. Item Instance: 从“字段”向“标签映射”的演进

这是 Lyra 最硬核的改进：`LyraInventoryItemInstance` 几乎没有任何具名属性（如 `Durability`, `Level`）。

### 核心设计评价 (Critical Audit)
* **Tag-Based State (基于标签的状态机)**:
    * **实现**: 物品状态通过一个 `FGameplayTagStackContainer` 维护。它本质上是一个 `TMap<GameplayTag, int32>`。
    * **案例**: 
        * 耐久度 50 $\rightarrow$ `Status.Durability` : 50
        * 强化等级 +3 $\rightarrow$ `Status.Level` : 3
    * **优点 - 极致的解耦**: 增加一种新属性不需要改 C++ 结构体，只需定义一个新 Tag。这对 **HybridCLR** 热更环境极其友好，因为热更层可以自由定义新的状态维度。
    * **优点 - 序列化优势**: 存档和网络同步时，只需要传输这个 Tag 映射表，不需要处理复杂的继承对象序列化。
    * **缺陷 - 类型丢失**: `int32` 限制了数值类型。如果你需要存储浮点数（如 `0.75` 的热度值），你必须进行定点数转换（乘以 100 存入 TagStack），增加了逻辑复杂度。



---

## 3. SOP: 工业级物品定义流程

1.  **定义 Fragment (静态)**:
    * `UUIFragment_ItemStats`: 存储图标、名称等静态展示数据。
    * `UInventoryFragment_EquippableItem`: 引用 `EquipmentDefinition` 以开启装备功能。
2.  **创建 ID 资产**: 新建 `ItemDefinition`，将上述 Fragment 塞入数组。
3.  **动态状态初始化 (动态)**:
    * 在物品生成时，通过 `InventoryManager` 劫持 `OnInstanceCreated` 回调。
    * 利用 `AddStack` 方法为 Instance 注入初始 Tag 数值（如初始弹药量）。

---

## 4. 架构缺陷分析与思考

### 缺陷 01: 逻辑查询的“黑盒化”
由于数据全在 Tag 容器里，代码中充满了 `GetStatTagStackCount(SomeTag)`。这使得代码的自解释性变差，如果不看 Tag 定义，很难一眼看出这个物品支持哪些业务逻辑。

### 缺陷 02: 内存与查找开销
虽然 `GameplayTag` 经过了优化（本质是 `FName` 比较），但相比于直接访问 C++ `float Durability` 成员，从 TArray/TMap 中按 Tag 查找数值依然存在 $O(logN)$ 或 $O(1)$ 散列开销。
