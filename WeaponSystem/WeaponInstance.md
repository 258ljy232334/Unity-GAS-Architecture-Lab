```
namespace Weapon.Instance
{
    public class WeaponInstance 
    {
        public readonly WeaponData Data;
       
        public float LastAttackTime { get; private set; }
        private WeaponAttributeContainer _container;
        public WeaponInstance(WeaponData data)
        {
            Data = data;
            _container = new WeaponAttributeContainer(data.AttributeConfigs);
        }
        public void CommitAttack()
        {
            LastAttackTime=Time.time;
        }
        public int GetAttribute(WeaponAttribute attribute)
        {
            return _container.GetAttribute(attribute);
        }
        public void AddStack(WeaponAttribute attribute,int count)
        {
            _container.AddStack(attribute, count);
        }
        public void RemoveStack(WeaponAttribute attribute,int count)
        {
            _container.RemoveStack(attribute, count);
        }
        public T GetExtra<T>() where T : WeaponExtra
        {
            foreach(var extra in Data.Extras)
            {
                if(extra is T)
                {
                    return (T)extra;
                }
            }
            return null;
        }
    }
}

```
