
using BehaviorTree.Enum;
namespace BehaviorTree
{
    [CreateNodeMenu("Node/Root")]
    public class RootNode : BehaviorTreeNode
    {
        [Output]
        public BehaviorTreeNode Child;
        public override NodeState Tick(Blackboard blackboard)
        {
            BehaviorTreeNode node = GetOutputPort("Child").Connection.node as BehaviorTreeNode;
            return node?.Tick(blackboard) ?? NodeState.Failure;
        }
    }
}