using UnityEngine;

namespace FSM.Base
{
    public class BaseBlackboard
    {
        public GameObject Self;
        public bool IsHurt;
        public bool IsDead;
        public BaseBlackboard(GameObject gameObject)
        {
            Self = gameObject;
            IsHurt = false;
            IsDead = false;
        }
    }
}
