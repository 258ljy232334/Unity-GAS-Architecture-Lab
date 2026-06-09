using System;
using UnityEngine;

namespace PlayerInput.Interface
{
    public interface IInputModule
    {

        //移动相关
        Vector2 MoveInput { get; }
       

        //单次按下事件
        event Action OpenBag;
        event Action Interact;
        event Action Jump;
        event Action Fire;
        event Action E_Skill;
        event Action Q_Skill;
        event Action ChangeView;
        event Action ChangeWeapon1;
        event Action ChangeWeapon2;
        event Action ChangeWeapon3;
        event Action<int> OnWeaponScroll;


        void OnEnableInput();
        void OnDisableInput();
        
    }
}
