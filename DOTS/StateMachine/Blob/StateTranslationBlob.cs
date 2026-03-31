using FSM.FuncLibrary;
using Unity.Burst;

namespace FSM.Blob
{
    public struct StateTranslationBlob
    {
        public int TargetStateId;
        public FunctionPointer<TransitionConditionDelegate> ConditionFunc;
    }
}