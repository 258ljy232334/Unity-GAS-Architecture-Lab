using Quest.Condition;
using Quest.Enum;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace Quest.Config
{
    [CreateAssetMenu(fileName ="NewTask",
        menuName ="SO/Quest/TaskConfig")]
    public class TaskConfig : ScriptableObject
    {
        [Title("基础信息")]
        [HorizontalGroup("Split", 0.3f)]
        [BoxGroup("Split/Left"), LabelText("任务ID")]
        public int Id;

        [BoxGroup("Split/Right"), LabelText("任务标题")]
        public string Title;

        [BoxGroup("Split/Right"), MultiLineProperty(3), LabelText("任务描述")]
        public string Description;

        [Title("流程控制")]
        [TabGroup("Tabs", "前置后续"),LabelText("前置任务ID")]
        public List<int> PreTaskId;
        [TabGroup("Tabs", "前置后续"),LabelText("后续任务ID")]
        public int NextTaskId;

        [TabGroup("Tabs", "事件监听")]
        [InfoBox("黑板中对应的 Key 发生变化时，将触发此任务的状态检查", InfoMessageType.None)]
        public List<BlackboardKeyType> ListenerEventTypes;
        [Title("目标与奖励")]
        
        [LabelText("任务目标")]
        [AssetList(Path = "SO/Quest/Condition")] // 方便快速选择条件资源
        public List<TaskCondition> Conditions;
        [LabelText("奖励物品")]
        [TableList(AlwaysExpanded = true)] 
        public List<RewardItem> Items;
        [LabelText("奖励金币数")]
        public int CoinCount;
        [Title("其他设置")]
        [LabelText("记录过往值")]
        public bool IsPreviousValue;
    }
    [System.Serializable]
    public struct RewardItem
    {
        public int ItemId;
        public int Count;
    }
}
