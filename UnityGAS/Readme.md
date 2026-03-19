# High-Performance Gameplay Ability System (GAS) for Unity

这是一个受 Unreal GAS 启发，专为 Unity 打造的高性能、高度解耦的能力系统。

## 核心架构 (Core Architecture)
- **AttributeSet**: 采用分层设计，支持 BaseValue 与 CurrentValue 的分离，完美处理 Buff 的加成算法。
- **AbilitySpec**: 将能力逻辑（Class）与能力实例（Instance）分离，支持轻量化扩展。
- **EffectContainer**: 预置了多种 GameplayEffect 类型，支持 Instant, Duration 和 Infinite 周期管理。

## 技术亮点 (Technical Highlights)
- **Decoupled via DI**: 使用 **Zenject/VContainer** 实现依赖注入，能力逻辑不直接依赖具体 Character 类。
- **Zero-GC SignalBus**: 核心事件交互（如能力激活、冷却开始）全部采用 **Readonly Struct Signals**，在高频战斗中实现零内存分配。
- **Odin Integration**: 提供全可视化的 Odin Inspector 编辑界面，策划可直接在编辑器内配置技能曲线。
