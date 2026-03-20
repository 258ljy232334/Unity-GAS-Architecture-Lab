# Deep Dive: Gameplay Message Router — 语义化解耦的终极形态

> **Architecture Core**: `GameplayMessageRouter` 是 Lyra 架构中的“中枢神经系统”。它抛弃了 Unity 常用的“类寻址”模式，引入了基于 **GameplayTag** 的“标签寻址”协议，实现了真正意义上的协议级脱钩（Protocol-Level Decoupling）。

---

## 1. 寻址范式转移：类型 vs. 标签

在传统的事件总线（如 Unity 中常见的实现）中，通信通常是基于类型的：
* **类寻址 (Unity Style)**: `Bus.Publish<ItemAddedEvent>(data)`。
    * **隐患**: 订阅者必须引用 `ItemAddedEvent` 所在的程序集。在 **HybridCLR** 架构下，这会导致主工程与热更工程产生循环引用或严重的类型污染。
* **标签寻址 (Lyra Style)**: `Bus.Broadcast(Tag, data)`。
    * **优势**: 订阅者只认 `GameplayTag`（本质是字符串哈希）。发送方和接收方互不相识，只要双方达成“语义共识”，即可完成跨模块、跨 C++/C# 域的通信。



---

## 2. 内部执行逻辑解析

消息路由的工作流程分为 **注册 (Register)** 与 **广播 (Broadcast)** 两个阶段：

### A. 绑定 (Listener Registration)
当一个逻辑模块（如 UI 表现层）想要监听数据时：
1. 向 `UGameplayMessageRouter` 注册。
2. 提供目标 `GameplayTag`（寻址键）。
3. 指定回调函数及预期的 `UScriptStruct` 数据类型。
4. **底层实现**: Manager 内部维护一个 `TMap<FGameplayTag, FMessageListenerList>`，将监听者按标签归类存储。

### B. 分发 (Message Broadcasting)
当数据源（如库存系统）发生变动时：
1. 调用 `BroadcastMessage`。
2. 传入 `GameplayTag` 和数据负载。
3. **底层实现**: 
    * 根据 Tag 瞬间定位监听者列表。
    * **动态验证**: 利用反射检查发送的数据结构是否符合订阅者的预期（这是保证系统健壮性的关键）。
    * 执行回调。

---

## 3. 架构审计：工业级风险与权衡

虽然标签寻址带来了极致的解耦，但也引入了不可忽视的“架构税”：

| 维度 | 缺陷 / 挑战 | 客观评价 |
| :--- | :--- | :--- |
| **性能开销** | **反射开销 (Reflection)** | 相比于原生 C# 委托或 C++ 函数指针，Message Router 涉及动态类型检查。严禁在 `Tick` 或高频物理计算中使用。 |
| **可追溯性** | **逻辑隐匿性 (Opaque)** | 无法通过 IDE 的 `Find References` 找到消息的流转链路。这种“心电感应式”通信极大增加了 Debug 的难度。 |
| **开发规范** | **Tag 爆炸** | 如果缺乏命名空间管理，项目后期会充斥大量意义不明的 Tag。必须强制实行 `Message.Module.Action` 的分层命名规范。 |

---
## 4. 使用示例
1. **消息结构体定义** (Data Load)

消息总线不传“类”，只传“数据碎片”。这个结构体必须是 USTRUCT，以便 UE 的反射系统识别。
```
// 定义在 C++ 核心层，作为通信协议
USTRUCT(BlueprintType)
struct FInventoryChangeMessage
{
    GENERATED_BODY()

    UPROPERTY(BlueprintReadOnly)
    FGameplayTag ItemTag; // 哪个物品变了

    UPROPERTY(BlueprintReadOnly)
    int32 NewCount;       // 现在的数量
};
```
2. **绑定监听** (Listen/Subscribe)

在 Lyra 中，监听通常发生在 BeginPlay 或初始化阶段。

```
// 某 UI 组件或逻辑类
void UMyWidget::NativeConstruct()
{
    Super::NativeConstruct();

    // 1. 获取全局单例管理器
    UGameplayMessageRouter& Router = UGameplayMessageRouter::Get(this);

    // 2. 绑定：按 Tag 寻址。注意 AddListener 的泛型约束。
    // 它会返回一个 Handle，用于取消订阅，防止内存泄露。
    ListenerHandle = Router.AddListener<FInventoryChangeMessage>(
        TAG_Message_Inventory_CountChanged, 
        this, 
        &this::OnInventoryChanged
    );
}

// 回调函数
void UMyWidget::OnInventoryChanged(FGameplayTag Channel, const FInventoryChangeMessage& Message)
{
    // 更新 UI 逻辑
    UpdateUI(Message.ItemTag, Message.NewCount);
}
```
3. **发送消息** (Broadcast/Publish)

发送方完全不需要知道谁在听。

```
// 在 InventoryComponent 中
void UInventoryComponent::AddItem(FGameplayTag ItemTag, int32 Amount)
{
    // ... 处理库存逻辑 ...

    // 构造数据负载
    FInventoryChangeMessage Msg;
    Msg.ItemTag = ItemTag;
    Msg.NewCount = GetItemCount(ItemTag);

    // 扔进总线，按 Tag 发送
    UGameplayMessageRouter::Get(this).BroadcastMessage(TAG_Message_Inventory_CountChanged, Msg);
}
```
