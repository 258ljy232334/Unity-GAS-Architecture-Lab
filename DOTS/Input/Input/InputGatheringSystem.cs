using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace PlayerInput.ECS
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputGatheringSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<PlayerInputComponent>()));
        }
        protected override void OnUpdate()
        {
            ref var input=ref SystemAPI.GetSingletonRW<PlayerInputComponent>().ValueRW;
            input.Move =new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            input.ShootRequested = Input.GetMouseButtonDown(0);
        }
    }
}
