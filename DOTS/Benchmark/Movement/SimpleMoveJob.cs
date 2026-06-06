using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Benchmark.Movement
{
    [BurstCompile]
    [WithAll(typeof(MovementBenchmarkTag))]
    public partial struct SimpleMoveJob : IJobEntity
    {
        public float DeltaTime;

        void Execute(ref LocalTransform transform, in SimpleMoveVelocity velocity)
        {
            transform.Position += velocity.Value * DeltaTime;
        }
    }
}
