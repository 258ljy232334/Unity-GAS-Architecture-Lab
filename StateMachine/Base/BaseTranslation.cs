using UnityEngine;

namespace FSM.Base
{
    
    public abstract  class BaseTranslation : ScriptableObject 
    {
        public BaseState From;
        public BaseState To;
        public BaseCondition Condition;
        public int Weight;      //ШЈжи
    }
}
