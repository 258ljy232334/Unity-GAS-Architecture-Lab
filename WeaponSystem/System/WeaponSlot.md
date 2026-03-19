```
using System.Collections.Generic;
using UnityEngine;
using Weapon.Config;
using Weapon.Instance;
using Weapon.Interface;
namespace Weapon.Manager
{
    public class WeaponSlot : MonoBehaviour,IWeaponSlot
    {
        [SerializeField]
        private List<WeaponData> _datas;
        private const int SLOT_LENGTH = 3;
        private WeaponInstance[] _instance=new WeaponInstance[SLOT_LENGTH];
        private void Awake()
        {
            int count = Mathf.Min(_datas.Count, SLOT_LENGTH);
            for (int i = 0; i < count; i++)
            {
                _instance[i] = new WeaponInstance(_datas[i]);
            }
        }
        public WeaponInstance GetInstance(int index)
        {
            if(index<0||index>=_instance.Length)
            {
                return null;
            }
            return _instance[index];
        }
        public int GetSlotCount()
        {
            return _datas.Count;
        }
    }
}

```
