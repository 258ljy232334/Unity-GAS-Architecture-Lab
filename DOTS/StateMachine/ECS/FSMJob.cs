using Unity.Entities;
namespace FSM.ECS
{
    public partial struct FSMJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public float DeltaTime;
        public void Execute(Entity entity, [ChunkIndexInQuery]int sortKey,ref FSMComponent fsm)
        {
            fsm.Timer+=DeltaTime;
            ref var blob= ref fsm.BlobRef.Value;
            ref var currentState= ref blob.AllStates[fsm.CurrentStateIndex];
            if(currentState.ExecuteFunc.IsCreated)
            {
                currentState.ExecuteFunc.Invoke(entity,sortKey,Ecb);
            }
            for(int i = 0;i<currentState.Translations.Length;i++)
            {
               ref var translation =ref currentState.Translations[i];
                if(translation.ConditionFunc.IsCreated&&
                    translation.ConditionFunc.Invoke(entity,ref fsm))
                {
                    fsm.CurrentStateIndex=translation.TargetStateId;
                    fsm.Timer=0;
                    break;
                }
            }
        }
    }
}