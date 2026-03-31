using Unity.Burst;
using Unity.Entities;
namespace FSM.FuncLibrary
{
    public delegate void FSMActionDelegate(in Entity entity,in int sortKey,in EntityCommandBuffer.ParallelWriter ecb);
    [BurstCompile]
    public static class FSMBehaviors
    {
        [BurstCompile]
        public static void DoIdle(in Entity entity,in int sortKey,in EntityCommandBuffer.ParallelWriter ecb)
        {
            // Idle behavior implementation
        }
        [BurstCompile]
        public static void DoMove(in Entity entity,in int sortKey,in EntityCommandBuffer.ParallelWriter ecb)
        {
            // Move behavior implementation
        }
         
    }
}
