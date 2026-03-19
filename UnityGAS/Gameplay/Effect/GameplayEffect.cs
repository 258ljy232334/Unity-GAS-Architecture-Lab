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
        public DurationType DurationType = DurationType.Instant;
        public float Duration;
        public float Period;
        [Header("修改器")]
        public List<Modifier> Modifiers = new List<Modifier>();
        [Header("标签")]
        public GameplayTag AssetTag;
        public List<GameplayTag> GrantedTags = new List<GameplayTag>();
        [Header("叠层")]
        public bool CanStack=false;     //能否叠层
        public int StackLimit = 1;      //叠层上限
    }
}
