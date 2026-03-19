using Gameplay.Effect;
using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Ability
{
    [CreateAssetMenu(fileName ="Ability",
        menuName ="SO/Gameplay/Ability")]
    public class GameplayAbility:ScriptableObject
    {
        public GameplayTag AbilityTag;
        public GameplayTag CooldownTag;
        public float CooldownTime;
        
        [Header("消耗")]
        public List<CostDef> Costs = new(); // 简化：直接扣属性

        [Header("效果")]
        public List<GameplayEffect> ApplyToSelf;
        public List<GameplayEffect> ApplyToTarget;
        [Header("其他")]
        public bool IsInstant;
    }
}