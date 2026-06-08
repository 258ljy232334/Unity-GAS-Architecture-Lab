
using UnityEngine;
namespace Gameplay.Ability
{
    public abstract class AbilityExtraFuncSO : ScriptableObject
    {
        public abstract void ExecuteFunc(AbilityInstance instance);
    }
}
