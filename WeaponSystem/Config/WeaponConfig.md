```
using System.Collections.Generic;
using UnityEngine;
using Weapon.Extra;
using Weapon.Stragety;
namespace Weapon.Config
{
    [CreateAssetMenu(fileName = "WeaponData",
        menuName = "SO/Weapon/Data")]
    public class WeaponData : ScriptableObject
    {
        public int ID;
        public string Name;
        public string Description;
        public float AttackInterval;
        public Sprite Icon;
        public WeaponStragety Stragety;
        public List<WeaponExtra> Extras;
        public List<WeaponAttributeConfig> AttributeConfigs;
    }
}
```
