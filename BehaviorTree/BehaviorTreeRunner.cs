using System.Collections.Generic;
using UnityEngine;
namespace BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTreeGraph _tree;
        [SerializeField]
        private float _tickInterval = 1f;
        
        private Blackboard _blackboard;
        private float _lastTickTime;
        private List<int> _toRemove=new List<int>();

        private void Start()
        {
            _blackboard = new Blackboard(gameObject);
        }
        private void Update()
        {
            if (_tree == null) return;
            if (Time.time - _lastTickTime < _tickInterval) return;

            // 每帧清空记录
            _toRemove.Clear();
            _blackboard.ActiveNodeThisFrame.Clear();

            _lastTickTime = Time.time;
            _tree.Tick(_blackboard);
            
            CleanUpDic(_blackboard.TimerDic);              // 等待计时器
            CleanUpDic(_blackboard.CurrentChildIndexMap);  // 序列/选择器索引
        }

        // 通用清理方法：所有字典都用这个清理
        private void CleanUpDic(Dictionary<int, float> dic)
        {
            _toRemove.Clear();
            foreach (var key in dic.Keys)
            {
                if (!_blackboard.ActiveNodeThisFrame.Contains(key))
                    _toRemove.Add(key);
            }
            foreach (var key in _toRemove)
                dic.Remove(key);
        }

        private void CleanUpDic(Dictionary<int, int> dic)
        {
            _toRemove.Clear();
            foreach (var key in dic.Keys)
            {
                if (!_blackboard.ActiveNodeThisFrame.Contains(key))
                    _toRemove.Add(key);
            }
            foreach (var key in _toRemove)
                dic.Remove(key);
        }
    }
}
