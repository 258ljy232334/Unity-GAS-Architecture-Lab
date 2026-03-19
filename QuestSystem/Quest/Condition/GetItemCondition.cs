using Quest.Blackboard;
using Quest.Interface;
using UnityEngine;

namespace Quest.Condition
{
    [CreateAssetMenu(fileName ="GetItemCondition",
        menuName ="SO/Quest/Condition/GetItem")]
    public class GetItemCondition : TaskCondition
    {

        public override bool IsMet(IQuestBlackboard blackboard, ValueUnion union = default)
        {
            return false;
        }
    }
}
