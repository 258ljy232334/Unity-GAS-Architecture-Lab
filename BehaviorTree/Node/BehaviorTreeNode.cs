using BehaviorTree.Enum;
using XNode;
namespace BehaviorTree
{
	public abstract class BehaviorTreeNode : Node
	{
        protected override void Init()
        {
            base.Init();
        }
        public override object GetValue(NodePort port)
        {
            return null;
        }
		public abstract NodeState Tick(Blackboard blackboard);
	}
}