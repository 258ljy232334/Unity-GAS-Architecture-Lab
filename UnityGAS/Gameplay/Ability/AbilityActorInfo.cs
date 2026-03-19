using Gameplay.AbilitySystem;
using Gameplay.Effect;
using UnityEngine;

namespace Gameplay.Ability
{
    public readonly struct AbilityActorInfo 
    {
        public readonly GameObject Owner;
        public readonly GameObject Avatar;
        public readonly GameplayEffectManager EffectManager;
        public readonly AbilitySystemComponent ASC;
        public AbilityActorInfo(GameObject owner,GameObject avatar,
            GameplayEffectManager effectManager,AbilitySystemComponent asc)
        {
            Owner=owner; 
            Avatar=avatar;
            EffectManager=effectManager;
            ASC=asc;
        }
    }
}
