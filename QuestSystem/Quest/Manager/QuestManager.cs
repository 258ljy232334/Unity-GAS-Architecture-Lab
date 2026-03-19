using Quest.Condition;
using Quest.Config;
using Quest.Enum;
using Quest.Instance;
using Quest.Interface;
using Quest.Signal;
using System;
using System.Collections.Generic;
using Zenject;
namespace Quest.Manager
{
    public class QuestManager : IInitializable, IDisposable,IQuestManager
    {
        [Inject]
        private SignalBus _bus;
        [Inject]
        private IQuestBlackboard _blackboard;
        private readonly Dictionary<int, TaskInstance> _taskDic = new Dictionary<int, TaskInstance>();
        private readonly List<TaskInstance> _tasks = new List<TaskInstance>();
        private readonly Dictionary<BlackboardKeyType, List<ConditionLocation>> _targetRegistry = new Dictionary<BlackboardKeyType, List<ConditionLocation>>();

        public void Initialize()
        {
            _bus.Subscribe<OnBlackboardValueChangedSignal>(OnBlackboardValueChanged);
        }
        public void Dispose()
        {
            _bus.Unsubscribe<OnBlackboardValueChangedSignal>(OnBlackboardValueChanged);
        }
        public void RegisterTask(TaskConfig config)
        {
            var instance = new TaskInstance(config, _blackboard);
            for (int i = 0; i < config.Conditions.Count; i++)
            {
                var targetType = config.Conditions[i].KeyType;

                if (!_targetRegistry.TryGetValue(targetType, out var list))
                {
                    list = new List<ConditionLocation>();
                    _targetRegistry[targetType] = list;
                }
                list.Add(new ConditionLocation(instance, i));
            }
            _taskDic.Add(config.Id, instance);
            _tasks.Add(instance);
        }
        private void CompleteTask(TaskInstance instance)
        {
            var config = instance.Config;

            // 1. 处理奖励 (建议通过 DI 注入 InventoryManager 进行解耦)
            HandleRewards(config);

            // 2. 发送信号
            _bus.Fire(new OnTaskFinishedSignal(config));

            // 3. 核心清理逻辑：三管齐下

            // A. 移除 UI 表现层和逻辑索引层
            _tasks.Remove(instance);
            _taskDic.Remove(config.Id);

            // B. 清理 Blackboard 注册表 (防止内存泄露)
            // 遍历该任务所有的条件类型，从全局注册表中把自己踢出去
            for (int i = 0; i < config.Conditions.Count; i++)
            {
                var keyType = config.Conditions[i].KeyType;
                if (_targetRegistry.TryGetValue(keyType, out var list))
                {
                    // 移除所有属于这个任务实例的条件定位
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        if (list[j].Task == instance)
                        {
                            list.RemoveAt(j);
                        }
                    }
                    // 如果该 Key 下没有其他任务在监听了，可以清理掉 List 减小字典压力
                    if (list.Count == 0)
                    {
                        _targetRegistry.Remove(keyType);
                    }
                }
            }
        }

        private void HandleRewards(TaskConfig config)
        {
            // 建议注入 IInventoryService 或使用 Signal 发送奖励请求
            // 这样 QuestManager 不需要知道背包系统的具体实现
            if (config.Items != null)
            {
                foreach (var item in config.Items)
                {
                    // _inventory.AddItem(item.Id, item.Count);
                }
            }
            if (config.CoinCount > 0)
            {
                //添加金币数量
            }
        }
        private void OnBlackboardValueChanged(OnBlackboardValueChangedSignal signal)
        {
            if (_targetRegistry.TryGetValue(signal.KeyType, out var list))
            {
                // 倒序遍历依然安全
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var loc = list[i];
                    if (loc.Task.Config.Conditions[loc.ConditionIndex].IsMet(_blackboard, loc.Task.GetPreValue(loc.ConditionIndex)))
                    {
                        if (loc.Task.MarkConditionComplete(loc.ConditionIndex))
                        {
                            // 整个任务完成后，这里会清理掉该 Task 在所有 Key 下的监听
                            CompleteTask(loc.Task);
                        }
                        else
                        {
                            // 任务没完，只删这一个条件的监听
                            list.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
