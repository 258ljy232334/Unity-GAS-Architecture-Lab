using XNode;
using UnityEngine;
using BehaviorTree.Enum;
namespace BehaviorTree
{
	[CreateAssetMenu(fileName ="BehaviorTree",
		menuName ="SO/BehaviorTree")]
	public class BehaviorTreeGraph  : NodeGraph
	{
		public RootNode Root;
		public NodeState Tick(Blackboard blackboard)
		{
			if(Root!=null)
			{
				return Root.Tick(blackboard);
			}
			return NodeState.Failure;
		}


	}
}