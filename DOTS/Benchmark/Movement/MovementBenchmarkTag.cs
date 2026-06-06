using Unity.Entities;

namespace Benchmark.Movement
{
    /// <summary>仅用于批量生成/销毁基准实体，避免误删场景里其它 Entity。</summary>
    public struct MovementBenchmarkTag : IComponentData { }
}
