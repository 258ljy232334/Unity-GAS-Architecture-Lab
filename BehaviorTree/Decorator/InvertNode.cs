
using BehaviorTree.Enum;
namespace BehaviorTree
{
    [CreateNodeMenu("Decorator/Invert")]
    public class InvertNode : BehaviorTreeNode
    {
        [Input]
        public BehaviorTreeNode Input;
        [Output]
        public BehaviorTreeNode Child;
        public override NodeState Tick(Blackboard blackboard)
        {
            var port = GetOutputPort("Child");
            if(!port.IsConnected)
            {
                return NodeState.Failure;
            }
            var node = port.Connection.node as BehaviorTreeNode;
            var state = node.Tick(blackboard);
            if (state == NodeState.Success)
            {
                return NodeState.Failure;
            }
            if(state==NodeState.Failure)
            {
                return NodeState.Success;
            }
            return NodeState.Running;
        }
    }
}