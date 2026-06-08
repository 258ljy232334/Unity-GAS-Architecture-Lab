using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Effect
{
    [CreateAssetMenu(fileName ="NewEffect",
        menuName ="SO/Gameplay/Effect")]
    public class GameplayEffect : ScriptableObject
    {
        [Header("基础设置")]
        public DurationType EffectDurationType = DurationType.Instant;
        public float Duration;
        public float Period;
        [Header("修改器")]
        public List<ModifierConfig> PeriodModifiers=new List<ModifierConfig>();     //周期修改器
        public List<ModifierConfig> InstantModifiers = new List<ModifierConfig>();  //瞬发修改器
        public List<ModifierConfig> PermanentModifiers = new List<ModifierConfig>();//永久修改器
        [Header("标签")]
        public GameplayTag AssetTag;
        public List<GameplayTag> GrantedTags = new List<GameplayTag>();
        [Header("叠层")]
        public StackType StackType = StackType.Unique;  //叠层类型
        public int MaxStackCount = 1;      //叠层上限
    }
}
