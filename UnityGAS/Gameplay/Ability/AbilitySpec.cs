
using UnityEngine;
namespace Gameplay.Ability
{
    public class AbilitySpec 
    {
        public GameplayAbility Ability;
        public int Level;
        public float CooldownEndTime;
        public bool IsOnCooldown => Time.time < CooldownEndTime;
        public AbilitySpec(GameplayAbility ability,int level=1)
        {
            Ability = ability;
            Level= level;
        }
    }
}