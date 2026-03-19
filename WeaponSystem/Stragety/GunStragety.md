```
using UnityEngine;
using Weapon.Attribute;
using Weapon.Context;
using Weapon.Extra;
using Weapon.Instance;

namespace Weapon.Stragety
{
    [CreateAssetMenu(fileName ="GunStragety",
        menuName ="SO/Weapon/Stragety/Gun")]
    public class GunStragety : WeaponStragety
    {
        public override void OnEquiped(WeaponInstance weapon)
        {
            Debug.Log("已装载枪支");
        }

        public override void OnUnequipped(WeaponInstance weapon)
        {
            Debug.Log("已卸载枪支");
        }
        public override bool CanAttack(WeaponInstance weapon)
        {
            if (weapon == null)
            {
                return false;
            }
            if(weapon.GetExtra<ProjectileExtra>()==null)
            {
                return false;
            }
            if (weapon.GetAttribute(WeaponAttribute.BulletCount) <= 0
                ||weapon.GetAttribute(WeaponAttribute.Durability) <= 0)
            {
                return false;
            }
            if(weapon.LastAttackTime+weapon.Data.AttackInterval>Time.time)
            {
                return false;
            }
            return true;
        }
        public override void Attack(WeaponInstance weapon, AttackContext context)
        {
            ProjectileExtra extra=weapon.GetExtra<ProjectileExtra>();
            GameObject projectile = Instantiate(extra.Projectile);
            weapon.CommitAttack();
            weapon.RemoveStack(WeaponAttribute.BulletCount,1);
            weapon.RemoveStack(WeaponAttribute.Durability,1);
        }
    }

}
```
