using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Blackboard
    {
        public GameObject Owner;
        public Dictionary<int, float> TimerDic;
        public Dictionary<int, int> CurrentChildIndexMap;
        //本次执行时执行过的活跃节点,当前只添加了WaitNode
        public HashSet<int> ActiveNodeThisFrame;    
        public Blackboard(GameObject owner)
        {
            Owner = owner;
            TimerDic = new Dictionary<int, float>();
            ActiveNodeThisFrame = new HashSet<int>();
            CurrentChildIndexMap = new Dictionary<int, int>();
        }
    }
}