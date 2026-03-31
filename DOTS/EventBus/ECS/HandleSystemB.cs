using Unity.Burst;
using Unity.Entities;
namespace EventBus.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    partial struct HandleSystemB : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //AsParallelWriter is used in job
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>
                    ().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            new HandleJobB { Ecb = ecb }.ScheduleParallel();
        }

    }
}
