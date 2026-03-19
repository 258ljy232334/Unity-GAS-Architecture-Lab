using Quest.Blackboard;
using Quest.Enum;
using Quest.Interface;
using UnityEngine;
namespace Quest.Condition
{
    public abstract class TaskCondition : ScriptableObject
    {
        public int InstanceId;
        public BlackboardKeyType KeyType;
        public BlackboardValueType ValueType;
        public abstract bool IsMet(IQuestBlackboard blackboard,ValueUnion union=default);
    }
}