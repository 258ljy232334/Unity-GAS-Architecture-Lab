# 📡 Unity DOTS: 基于标签的事件流转架构 (EventBus)
## 📖 简介
本项目展示了一种在非托管 ECS 环境下实现 解耦事件处理 的高效方案。不同于传统的 C# Delegate 或广播，该方案利用 Entity Archetype 和 Component Tags 来精确控制事件的产生、分发、多系统并行消费以及自动回收。

## 🌟 技术亮点
* **原子性分发** (Multi-Consumer Support)：通过 NeedSystemAHandleTag 和 NeedSystemBHandleTag，一个事件可以同时被多个独立的系统消费，且互不干扰。

* **零残余回收** (Auto-Cleanup)：CleanEventEntitySystem 利用 QueryBuilder 过滤出所有标签已被移除的事件实体并统一销毁，确保内存零泄漏。

* **帧内处理** (Same-Frame Processing)：通过 UpdateInGroup 特性精确控制 System 排序，实现从事件产生到消费在同一帧内闭环，消除逻辑延迟。

* **并行友好** (Jobified Handling)：支持在 IJobEntity 中通过 ParallelWriter 异步移除标签，充分发挥多核性能。

## 🏗️ 运行流程 (The Pipeline)
1. **事件产生** (Production)
CreateEventSystem 根据输入或其他逻辑，创建一个带有多个“任务标签”的事件实体。

```
// 创建包含所有处理目标标签的实体
_eventArchetype = state.EntityManager.CreateArchetype(
    typeof(EventComponent),
    typeof(NeedSystemAHandleTag),
    typeof(NeedSystemBHandleTag));
```
2. **并行消费** (Consumption)
多个 HandleSystem 独立运行，只关心带有自己特定标签的事件。处理完成后，仅移除自己的标签。

```
// HandleSystemB 示例：处理逻辑并移除 B 标签
public void Execute(Entity entity, [ChunkIndexInQuery] int sortKey, in NeedSystemBHandleTag tag)
{
    // ... 执行业务逻辑 ...
    Ecb.RemoveComponent<NeedSystemBHandleTag>(sortKey, entity);
}
```
3. **自动清理** (Cleanup)
当所有消费者系统都处理完毕（即所有 Tag 都被移除），清理系统将其销毁。

```
// 核心逻辑：只剩下 EventComponent 但没有任务标签时，销毁实体
var query = SystemAPI.QueryBuilder()
    .WithAll<EventComponent>()
    .WithNone<NeedSystemAHandleTag>()
    .WithNone<NeedSystemBHandleTag>()
    .Build();
ecb.DestroyEntity(query, EntityQueryCaptureMode.AtPlayback);
```
## 🚀 性能优势
* **Structural Integrity**: 避免了在主逻辑中直接销毁实体的结构突变（Structural Changes），通过 ECB 延迟执行，保证了 Job 的并行效率。

* **Filter Efficiency**: 系统仅通过 WithAll<Tag> 过滤，完全利用了 ECS 的 Chunk 迭代优势，即便每帧产生数千个事件，开销也极低。

* **Order Decoupling**: 生产者不需要知道谁在监听事件，消费者也不需要知道谁产生了事件，通过组件标签实现完美的逻辑解耦。

## 📂 使用说明
1. **定义事件**：实现 IComponentData 作为事件载体。

2. **定义标签**：为每个需要处理该事件的系统定义一个空的 TagComponent。

3. **配置排序**：确保 CleanEventEntitySystem 在 LateSimulationSystemGroup 中运行，以保证它在所有处理系统之后执行。
