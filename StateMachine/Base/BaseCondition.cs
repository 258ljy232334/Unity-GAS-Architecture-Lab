using UnityEngine;

namespace FSM.Base {
    public abstract class BaseCondition :ScriptableObject 
    {
        public abstract bool CanTranslate(BaseBlackboard value);
    }
}
