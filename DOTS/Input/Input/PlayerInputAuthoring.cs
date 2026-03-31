using PlayerInput.ECS;
using Unity.Entities;
using UnityEngine;
namespace PlayerInput
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerInputComponent
                {
                    
                });
            }
        }
    }
}
