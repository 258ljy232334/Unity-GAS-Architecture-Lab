using Unity.Burst;
using Unity.Entities;
namespace EventBus.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct CleanEventEntitySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>
                ().CreateCommandBuffer(state.WorldUnmanaged);
            var query = SystemAPI.QueryBuilder()
                .WithAll<EventComponent>()
                .WithNone<NeedSystemAHandleTag>()
                .WithNone<NeedSystemBHandleTag>()
                .Build();
            //Use EntityQuery to delete
            ecb.DestroyEntity(query, EntityQueryCaptureMode.AtPlayback);
        }
    }
}
