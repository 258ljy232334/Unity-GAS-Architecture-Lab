using FSM.Enums;
using UnityEngine;

namespace FSM.Base
{
    public abstract class BaseState : ScriptableObject 
    {
        public StateType Type;
        public virtual void Enter(BaseBlackboard value){}
        public virtual void Exit(BaseBlackboard value){}
        public virtual void OnUpdate(BaseBlackboard value) {}
    }
}
