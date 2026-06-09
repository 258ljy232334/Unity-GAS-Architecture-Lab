
using BehaviorTree.Enum;
using UnityEngine;

namespace BehaviorTree
{
    [CreateNodeMenu("Action/Log")]
    public class LogNode : BehaviorTreeNode
    {
        [Input]
        public BehaviorTreeNode Entry;
        public string Msg = "Run";
        public override NodeState Tick(Blackboard blackboard)
        {
            Debug.Log(Msg);
            return NodeState.Success;
        }
    }
}
