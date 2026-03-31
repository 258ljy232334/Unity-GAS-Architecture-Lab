using Unity.Entities;
using Unity.Mathematics;
namespace PlayerInput.ECS
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 Move;
        public bool ShootRequested;
    }
}
