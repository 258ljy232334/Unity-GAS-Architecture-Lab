
using BehaviorTree.Enum;

namespace BehaviorTree
{
    public abstract class ConditionalNode : BehaviorTreeNode
    {
        public override NodeState Tick(Blackboard blackboard)
        {
            if (blackboard == null)
            {
                return NodeState.Failure;
            }
            if(Check(blackboard))
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Failure;
            }
        }
        public abstract bool Check(Blackboard blackboard);
    }
}
