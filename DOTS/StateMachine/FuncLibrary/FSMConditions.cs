using FSM.ECS;
using Unity.Burst;
using Unity.Entities;

namespace FSM.FuncLibrary
{
    public delegate bool TransitionConditionDelegate(in Entity entity, ref FSMComponent fsm);
    [BurstCompile]
    public static class FSMConditions
    {
        [BurstCompile]
        public static bool CheckInput(in Entity entity, ref FSMComponent fsm)
        {
            if(fsm.CurrentStateIndex==0)
            {
                return false;
            }
            return true;
        }
        [BurstCompile]
        public static bool CheckDistance(in Entity entity, ref FSMComponent fsm)
        {
            return false;
        }
    }
}
