using Unity.Entities;
namespace EventBus.ECS
{
    public struct EventComponent : IComponentData
    {
        public int Value;
        public EventComponent(int value)
        {
            Value=value; 
        }
    }
}