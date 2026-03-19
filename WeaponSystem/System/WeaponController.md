```
using PlayerInput.Interface;
using System;
using UnityEngine;
using Weapon.Attribute;
using Weapon.Context;
using Weapon.Instance;
using Weapon.Signal;
using Zenject;

namespace Weapon.Manager
{
    public class WeaponController:IInitializable,IDisposable
    {
        [Inject]
        private SignalBus _bus;
        [Inject]
        private IInputModule _input;
        private WeaponInstance _currentWeapon;

        public void Initialize()
        {
            _bus.Subscribe<OnCurrentWeaponChangedSignal>(OnChangeWeapon);
            _input.Fire += OnFire;
        }
        public void Dispose()
        {
            _bus.Unsubscribe<OnCurrentWeaponChangedSignal>(OnChangeWeapon);
            _input.Fire -= OnFire;
        }
        //攻击事件
        private void OnFire()
        {
            if (_currentWeapon?.Data?.Stragety == null)
            {
                Debug.LogWarning("武器数据为空");
                return;
            }
            if (!_currentWeapon.Data.Stragety.CanAttack(_currentWeapon))
            {
                Debug.LogWarning("当前不能攻击");
                return;
            }
            AttackContext context = new AttackContext();
            _currentWeapon.Data.Stragety.Attack(_currentWeapon,context);
            _bus.Fire(new OnWeaponAttackSignal(_currentWeapon, context));
            Debug.Log(_currentWeapon.GetAttribute(WeaponAttribute.BulletCount));
        }

        private void OnChangeWeapon(OnCurrentWeaponChangedSignal signal)
        {
            if (_currentWeapon != null)
            {
                if (_currentWeapon.Data != null&&
                    _currentWeapon.Data.Stragety!=null)
                {
                    _currentWeapon.Data.Stragety.OnUnequipped(_currentWeapon);
                }
            }
            _currentWeapon = signal.CurrentWeapon;
            if (_currentWeapon != null)
            {
                if (_currentWeapon.Data != null &&
                    _currentWeapon.Data.Stragety != null)
                {
                    _currentWeapon.Data.Stragety.OnEquiped(_currentWeapon);
                }
            }
        }

        
    }
}
```
