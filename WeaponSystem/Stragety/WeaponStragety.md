```
using UnityEngine;
using Weapon.Context;
using Weapon.Instance;
namespace Weapon.Stragety
{
    public abstract class WeaponStragety : ScriptableObject
    {
        public abstract void OnEquiped(WeaponInstance weapon);
        public abstract void OnUnequipped(WeaponInstance weapon);
        public abstract bool CanAttack(WeaponInstance weapon);
        public abstract void Attack(WeaponInstance weapon,AttackContext context);
    }
}
```
