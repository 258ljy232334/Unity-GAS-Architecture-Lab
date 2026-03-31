
using FSM.Enum;
using System.Collections.Generic;

namespace FSM.SO
{
    [System.Serializable]
    public class StateNode
    {
        public int StateID;
        public FSMBehaviorType Behavior;
        public List<TransitionConfig> Translations;
    }
}
