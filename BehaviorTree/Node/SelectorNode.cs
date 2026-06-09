
using BehaviorTree.Enum;

namespace BehaviorTree
{
    [CreateNodeMenu("Node/Select")]
    public class SelectorNode : BehaviorTreeNode
    {
        [Input]
        public BehaviorTreeNode Entry;

        [Output(connectionType = ConnectionType.Multiple)]
        public BehaviorTreeNode Children;

        public override NodeState Tick(Blackboard blackboard)
        {
            int key = GetHashCode();
            blackboard.ActiveNodeThisFrame.Add(key);

            var connections = GetOutputPort(nameof(Children)).GetConnections();
            if (connections.Count == 0)
                return NodeState.Failure;

            // 记录当前执行到第几个子节点
            if (!blackboard.CurrentChildIndexMap.ContainsKey(key))
                blackboard.CurrentChildIndexMap[key] = 0;

            int currentIndex = blackboard.CurrentChildIndexMap[key];
            var node = connections[currentIndex].node as BehaviorTreeNode;

            if (node == null)
                return NodeState.Failure;

            var state = node.Tick(blackboard);

            // 成功 → 整个选择器成功，清空索引
            if (state == NodeState.Success)
            {
                blackboard.CurrentChildIndexMap.Remove(key);
                return NodeState.Success;
            }

            // 失败 → 下一个
            if (state == NodeState.Failure)
            {
                currentIndex++;
                blackboard.CurrentChildIndexMap[key] = currentIndex;

                // 所有都失败
                if (currentIndex >= connections.Count)
                {
                    blackboard.CurrentChildIndexMap.Remove(key);
                    return NodeState.Failure;
                }

                return NodeState.Running;
            }

            // Running → 保持不动
            return NodeState.Running;
        }
    }
}
