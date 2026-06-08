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
        public GameplayTag AssetTag;    //资源标签
        public float CooldownTime;
        public Sprite Icon;
        
        [Header("消耗")]
        public List<CostDef> Costs = new(); // 需要扣除的属性

        [Header("效果")]
        public List<GameplayEffect> ApplyToSelf;    //给自己应用的属性
        public List<GameplayEffect> ApplyToTarget;  //给敌人应用的效果
        [Header("其他")]
        public bool IsInstant;
        public List<AbilityExtraFuncSO> ExtraFunc;      //要执行的额外方法
        public GameObject Prefab;                       //要生成的额外预制体
        public Vector3 PrefabOffset;
    }
}