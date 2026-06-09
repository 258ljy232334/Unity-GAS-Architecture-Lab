using PlayerInput.Interface;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace PlayerInput.Module
{
    public class InputModule : IInputModule, IInitializable, IDisposable
    {
        public Vector2 MoveInput => Get<Vector2>("Move");
        // �¼�ӳ�䣺Input Action ����Ӧ�·��¼�
        public event Action OpenBag, Jump, Fire, E_Skill, Q_Skill, ChangeView, Interact;
        public event Action ChangeWeapon1, ChangeWeapon2, ChangeWeapon3;
        public event Action<int> OnWeaponScroll;

        private Player_Input _input;
        private readonly Dictionary<string, InputAction> _actions = new();

        private enum InputSubscriptionPhase
        {
            Performed,
            Started
        }

        private readonly List<(InputAction action, Action<InputAction.CallbackContext> handler, InputSubscriptionPhase phase)> _bindings = new();

        public void Initialize()
        {
            _input = new Player_Input();
            _input.Enable();

            // ֵ���� Action ����
            CacheActions("Move");
            // ��ť��performed ����
            BindButton("OpenBag", () => OpenBag?.Invoke());
            BindButton("Fire", () => Fire?.Invoke());
            BindButton("Jump", () => Jump?.Invoke());
            BindButton("Skill1", () => E_Skill?.Invoke());
            BindButton("Skill2", () => Q_Skill?.Invoke());
            BindButton("ChangeView", () => ChangeView?.Invoke());
            BindButton("ChangeWeapon1", () => ChangeWeapon1?.Invoke());
            BindButton("ChangeWeapon2", () => ChangeWeapon2?.Invoke());
            BindButton("ChangeWeapon3", () => ChangeWeapon3?.Invoke());
            BindButton("Interact", () => Interact?.Invoke());

            BindScroll("WeaponScroll");
        }

        public void Dispose()
        {
            foreach (var (action, handler, phase) in _bindings)
            {
                if (phase == InputSubscriptionPhase.Started)
                    action.started -= handler;
                else
                {
                    action.performed -= handler;
                    action.canceled -= handler;
                }
            }
            _bindings.Clear();
            _input?.Dispose();
        }

        private T Get<T>(string name) where T : struct =>
            _actions.TryGetValue(name, out var action) ? action.ReadValue<T>() : default;

        private void CacheActions(params string[] names)
        {
            foreach (var name in names)
                _actions[name] = _input.FindAction(name);
        }

        private void BindButton(string actionName, Action eventAction)
        {
            var action = _input.FindAction(actionName);
            Action<InputAction.CallbackContext> handler = _ => eventAction?.Invoke();
            action.performed += handler;
            _bindings.Add((action, handler, InputSubscriptionPhase.Performed));
        }

        private void BindScroll(string actionName)
        {
            var action = _input.FindAction(actionName);
            Action<InputAction.CallbackContext> handler = ctx =>
            {
                float value = ctx.ReadValue<float>();
                int dir = value > 0 ? 1 : -1;
                OnWeaponScroll?.Invoke(dir);
            };
            action.started += handler;
            _bindings.Add((action, handler, InputSubscriptionPhase.Started));
        }

        public void OnEnableInput() => _input?.Enable();
        public void OnDisableInput() => _input?.Disable();
    }
}
