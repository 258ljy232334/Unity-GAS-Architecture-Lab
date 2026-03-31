using Unity.Entities;
namespace EventBus.ECS
{
    public partial struct HandleJobB : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public void Execute(Entity entity, [ChunkIndexInQuery] int sortKey,in NeedSystemBHandleTag tag)
        {

            Ecb.RemoveComponent<NeedSystemBHandleTag>(sortKey, entity);
        }
    }
}
