using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Benchmark.Movement
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(LocalToWorldSystem))]
    public partial struct SimpleMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new SimpleMoveJob { DeltaTime = SystemAPI.Time.DeltaTime };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}
