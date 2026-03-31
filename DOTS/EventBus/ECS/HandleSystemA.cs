using Unity.Entities;
namespace EventBus.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    partial struct HandleSystemA : ISystem
    {

        public void OnUpdate(ref SystemState state)
        {
            
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>
                ().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (eventData, entity) in 
                SystemAPI.Query<EventComponent>()
                .WithAll<NeedSystemAHandleTag>()
                .WithEntityAccess())
            {
                // Logic
                UnityEngine.Debug.Log($"HandleSystemA: {eventData.Value}");
                ecb.RemoveComponent<NeedSystemAHandleTag>(entity);
            }
        }
    }
}
