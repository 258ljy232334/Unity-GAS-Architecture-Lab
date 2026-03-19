```
using PlayerInput.Interface;
using System;
using Weapon.Interface;
using Weapon.Signal;
using Zenject;

namespace Weapon.Manager
{
    public class WeaponSwitcher:IInitializable,IDisposable
    {
        [Inject]
        private IInputModule _input;
        [Inject]
        private IWeaponSlot _weaponSlot;
        [Inject]
        private SignalBus _bus;

        private int _currentIndex = 0;
        // 保存委托引用
        private Action _onWeapon1;
        private Action _onWeapon2;
        private Action _onWeapon3;
        private Action<int> _onScroll;

        public void Initialize()
        {
            // 创建并缓存委托
            _onWeapon1 = () => SelectWeapon(0);
            _onWeapon2 = () => SelectWeapon(1);
            _onWeapon3 = () => SelectWeapon(2);
            _onScroll=OnWeaponScroll;
            // 订阅
            _input.ChangeWeapon1 += _onWeapon1;
            _input.ChangeWeapon2 += _onWeapon2;
            _input.ChangeWeapon3 += _onWeapon3;
            _input.OnWeaponScroll += _onScroll;

            SelectWeapon(0);
        }
        public void Dispose()
        {
            // 取消订阅
            _input.ChangeWeapon1 -= _onWeapon1;
            _input.ChangeWeapon2 -= _onWeapon2;
            _input.ChangeWeapon3 -= _onWeapon3;
            _input.OnWeaponScroll -= _onScroll;
        }
        

        private void SelectWeapon(int index)
        {
            if(_weaponSlot!=null)
            {
                var weapon=  _weaponSlot.GetInstance(index);
                _currentIndex=index;
                _bus.Fire(new OnCurrentWeaponChangedSignal(weapon, index));
            }
        }
        private void OnWeaponScroll(int delta)
        {
            int realIndex = (_currentIndex + delta + _weaponSlot.GetSlotCount()) % _weaponSlot.GetSlotCount();
            if (_weaponSlot.GetInstance(realIndex) != null)
            {
                var weapon = _weaponSlot.GetInstance(realIndex);
                _currentIndex = realIndex;
                _bus.Fire(new OnCurrentWeaponChangedSignal(weapon, realIndex));
            }
        }
    }
}
```
