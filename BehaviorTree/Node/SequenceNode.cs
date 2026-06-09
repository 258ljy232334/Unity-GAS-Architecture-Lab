
using BehaviorTree.Enum;

namespace BehaviorTree
{
    [CreateNodeMenu("Node/Sequence")]
    public class SequenceNode : BehaviorTreeNode
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
            if (connections.Count == 0) return NodeState.Success;

            // 第一次进来，从第0个子节点开始
            if (!blackboard.CurrentChildIndexMap.ContainsKey(key))
                blackboard.CurrentChildIndexMap[key] = 0;

            int currentIndex = blackboard.CurrentChildIndexMap[key];

            var node = connections[currentIndex].node as BehaviorTreeNode;
            if (node == null)
            {
                blackboard.CurrentChildIndexMap.Remove(key);
                return NodeState.Failure;
            }

            NodeState state = node.Tick(blackboard);

            // 子节点成功执行下一个
            if (state == NodeState.Success)
            {
                currentIndex++;
                blackboard.CurrentChildIndexMap[key] = currentIndex;

                // 所有节点都执行完了
                if (currentIndex >= connections.Count)
                {
                    blackboard.CurrentChildIndexMap.Remove(key);
                    return NodeState.Success;
                }
                return NodeState.Running;
            }

            // 子节点失败 → 整个序列失败
            if (state == NodeState.Failure)
            {
                blackboard.CurrentChildIndexMap.Remove(key);
                return NodeState.Failure;
            }

            // 子节点 Running → 保持当前索引
            return NodeState.Running;
        }
    }
}