using PlayerInput.ECS;
using Unity.Burst;
using Unity.Entities;
namespace EventBus.ECS
{
    partial struct CreateEventSystem : ISystem
    {
        private EntityArchetype _eventArchetype;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputComponent>();
            _eventArchetype=state.EntityManager.CreateArchetype(
                typeof(EventComponent),
                typeof(NeedSystemAHandleTag),
                typeof(NeedSystemBHandleTag));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var input= SystemAPI.GetSingleton<PlayerInputComponent>();
            if(input.ShootRequested)
            {
                Entity e = ecb.CreateEntity(_eventArchetype);
                ecb.SetComponent(e, new EventComponent { Value = 100 });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
