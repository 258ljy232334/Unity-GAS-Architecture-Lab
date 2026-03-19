```
using System.Collections.Generic;
using UnityEngine;
using Weapon.Config;

namespace Weapon.Attribute
{
    public class WeaponAttributeContainer
    {
        private Dictionary<WeaponAttribute, int> _container;
        public WeaponAttributeContainer(List<WeaponAttributeConfig> config)
        {
            _container = new Dictionary<WeaponAttribute, int>();
            if (config != null)
            {
                foreach (var item in config)
                {
                    _container[item.Attribute] = item.InitializeCount;
                }
            }
        }
        public bool HasAttribute(WeaponAttribute attribute)
        {
            return _container.ContainsKey(attribute);
        }
        public int GetAttribute(WeaponAttribute attribute)
        {
            if(HasAttribute(attribute))
            {
                return _container[attribute];
            }
            return -1;
        }
        public bool HasAny(List<WeaponAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (HasAttribute(attribute))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasAll(List<WeaponAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if (!HasAttribute(attribute))
                {
                    return false;
                }
            }
            return true;
        }
        public IReadOnlyDictionary<WeaponAttribute, int> GetAllAttributes()
        {
            return _container;
        }
        public void AddStack(WeaponAttribute attribute,int count=1)
        {
            if(HasAttribute(attribute))
            {
                _container[attribute] += count;
            }
            else
            {
                Debug.LogWarning("没有该字段");
            }
        }
        public void RemoveStack(WeaponAttribute attribute, int count)
        {
            if (HasAttribute(attribute))
            {
                _container[attribute] -= count;
            }
            else
            {
                Debug.LogWarning("没有该字段");
            }
        }
    }
}
```
