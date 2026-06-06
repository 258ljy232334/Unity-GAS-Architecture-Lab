namespace Benchmark.Movement
{
    /// <summary>
    /// 对比用模式说明：
    /// Dots — ECS + Burst 并行，更新 LocalTransform（与项目其余 ECS 一致）。
    /// MonoBatched — 单 MonoBehaviour 内 for 循环更新大量 Transform（常见“手写批处理”优化写法）。
    /// MonoPerObject — 每个物体一个 MonoBehaviour.Update（经典 GameObject 架构，调度开销更明显）。
    /// </summary>
    public enum MovementBenchmarkMode
    {
        None = 0,
        Dots = 1,
        MonoBatched = 2,
        MonoPerObject = 3
    }
}
