using Unity.Entities;
using Unity.Mathematics;

namespace Benchmark.Movement
{
    public struct SimpleMoveVelocity : IComponentData
    {
        public float3 Value;
    }
}
