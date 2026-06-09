using BehaviorTree.Enum;
using UnityEngine;
namespace BehaviorTree
{
    [CreateNodeMenu("Decorator/Wait")]
    public class WaitNode : BehaviorTreeNode
    {
        public float WaitTimer;

        [Input]
        public BehaviorTreeNode Input;
        [Output]
        public BehaviorTreeNode Child;
        public override NodeState Tick(Blackboard blackboard)
        {
            int key = GetHashCode();
            blackboard.ActiveNodeThisFrame.Add(key);
            
            if (!blackboard.TimerDic.ContainsKey(key))
            {
                blackboard.TimerDic[key]=Time.time;
            }

            if (Time.time - blackboard.TimerDic[key] < WaitTimer)
            {
                return NodeState.Running;
            }

          
            blackboard.TimerDic.Remove(key);

            // 获取子节点
            var child = GetOutputPort("Child").Connection.node as BehaviorTreeNode;

            // 子节点存在 → 执行它
            if (child != null)
            {
                return child.Tick(blackboard);
            }

            // 没有子节点 → 返回 Success（兜底）
            return NodeState.Success;
        }
    }
}