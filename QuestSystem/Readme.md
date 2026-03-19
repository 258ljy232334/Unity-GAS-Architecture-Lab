# Reactive Quest & Blackboard System
一个基于**黑板模式** (Blackboard Pattern) 与事件驱动架构的高性能任务系统。该系统通过解耦任务逻辑与业务数据，实现了极高的扩展性与运行效率。

## 核心设计 (Core Design)
1. **零装箱数值联合体** (Zero-Boxing ValueUnion)
* 系统核心采用 [StructLayout(LayoutKind.Explicit)] 定义的数值联合体，在 C# 中模拟 C 语言的 union 结构。

* 内存对齐：int, float, bool 共享同一块 4 字节内存空间。

* 性能优势：在全局黑板（Blackboard）频繁读写数据时，彻底消除了原始类型（Value Type）产生装箱（Boxing）导致的 GC 消耗。

2. **事件驱动的黑板模式** (Event-Driven Blackboard)
* 任务系统不再通过 Update() 轮询进度，而是通过 TargetRegistry（反向索引注册表） 实现精准响应。

* 数据触发：只有当黑板中特定的 BlackboardKeyType 发生变化时，系统才会通知关联的任务实例。

* 复杂度控制：状态检查的开销仅与“受影响的任务”成正比，而非“总任务量”。

3. **完全解耦的任务条件** (Decoupled Conditions)
* 利用 ScriptableObject 定义 TaskCondition，实现了逻辑与配置的彻底分离。

* 高度扩展：无需修改核心代码即可通过组合 SO 创建全新的任务目标（如击杀、采集、到达位置）。

* 状态记忆：支持 IsPreviousValue 功能，允许任务追踪自接受任务起的数值增量（如“相比接任务时，再赚取 100 金币”）。

## 技术栈 (Tech Stack)
* DI 框架: Zenject / VContainer (用于解耦 SignalBus 注入)

* 编辑器扩展: Odin Inspector (用于可视化任务流控与 SO 配置)

* 内存优化: System.Runtime.InteropServices (用于 Explicit Layout 内存布局)

## 核心工作流 (Workflow)
* 注册 (Register): QuestManager 在初始化时根据 TaskConfig 建立数据键值到任务实例的反向索引。

* 监听 (Listen): 业务层（如战斗、背包）通过 SetInt/SetFloat 修改黑板数据，触发信号。

* 分发 (Dispatch): QuestBlackboard 派发事件，QuestManager 仅对受影响的 TaskInstance 执行 IsMet 校验。

* 清理 (Cleanup): 任务完成后，系统自动执行深度的注册表清理，确保零内存泄露。
