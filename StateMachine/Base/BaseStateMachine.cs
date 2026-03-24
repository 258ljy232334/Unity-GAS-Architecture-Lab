using FSM.Interfaces;
using System;
using UnityEngine;
namespace FSM.Base
{
    public abstract class BaseStateMachine : MonoBehaviour 
    {
        [SerializeField]
        protected BaseStateMachineAsset _stateMachineAsset;
        protected BaseState _currentState;
        public BaseBlackboard _currentBlackboard;
        public event Action<BaseState,BaseState> OnStateChanged;
        protected virtual void Start()
        {
            CreateStateValue();
            Initialize();
        }
        protected void Update()
        {
            _currentState.OnUpdate(_currentBlackboard);
            var list = _stateMachineAsset.GetTranslations(_currentState.Type);
            if (list == null) return;
            
            foreach (var t in list)
            {
                if(t.Condition.CanTranslate(_currentBlackboard))
                {
                    _currentState.Exit(_currentBlackboard);
                    _currentState = t.To;
                    _currentState.Enter(_currentBlackboard);
                    OnStateChanged?.Invoke(t.From, t.To);
                    break;
                }
            }
        }

        protected void FixedUpdate()
        {
            if(_currentState is IPhysicsState physics)
            {
                physics.OnFixedUpdate(_currentBlackboard);
            }
        }
        protected abstract void CreateStateValue();
        protected void Initialize()
        {
            if (_stateMachineAsset == null)
            {
                Debug.LogError("资源为空");
            }
            if(_stateMachineAsset.InitialState==null)
            {
                Debug.LogError("初始状态为空");
            }
            else
            {
                _currentState = _stateMachineAsset.InitialState;
                _currentState.Enter(_currentBlackboard);
                OnStateChanged?.Invoke(null,_currentState);
            }
        }
        public BaseBlackboard GetStateValue()=> _currentBlackboard;
        //强制切换当前状态
        public void ForceToChangeState(BaseState newState)
        {
            // 可以在这里加一个防御：如果已经在该状态，是否重复进入？
            if (_currentState == newState) return;
            //TODO
            
        }
    }

}