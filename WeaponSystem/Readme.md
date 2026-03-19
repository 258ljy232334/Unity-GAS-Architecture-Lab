# Data-Driven Weapon Framework

基于策略模式和 ScriptableObject 的武器系统，支持动态词条与复杂的枪械行为。

## 设计模式 (Design Patterns)
- **State Pattern**: 用于处理武器的 Reloading, Firing, 和 Recoil 状态切换。
- **Decorator Pattern**: 用于实现武器配件（Attachments）对基础数值的动态修改。
- **Command Pattern**: 武器的发射请求被封装为 Command，便于接入网络同步或重放系统。

## 工作流 (Workflow)
- **SO-Based**: 所有武器参数（射速、弹道、衰减）均在 **ScriptableObject** 中配置，实现 100% 数据驱动。
