using FSM.Enum;

namespace FSM.SO
{
    [System.Serializable]
    public class TransitionConfig
    {
        public int TargetStateID;
        public ConditionType Condition;
    }
}
