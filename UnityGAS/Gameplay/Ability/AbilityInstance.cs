using Gameplay.Attribute;
using Gameplay.Effect;
using Gameplay.Tag;
using System;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityInstance 
    {
        public AbilitySpec Spec { get; }
        public AbilityActorInfo ActorInfo { get; }
        public GameplayTag ActivationTag { get; }

        public Vector3 TargetPoint { get; }
        public GameObject TargetActor { get; }
        public enum State { None, Activating, Active, Ending, Ended }
        public State CurrentState { get; private set; } = State.None;

        public event Action OnEnded;

        public AbilityInstance(AbilitySpec spec, AbilityActorInfo info,
            Vector3 point, GameObject target)
        {
            Spec = spec;
            ActorInfo = info;
            TargetPoint = point;
            TargetActor = target;
            ActivationTag = spec.Ability.AbilityTag;
        }

        // ========== 核心流程（可重写） ==========

        public virtual bool TryActivate()
        {
            if (CurrentState != State.None)
            {
                return false;
            }
            if (Spec.IsOnCooldown)
            {
                return false;
            }
            if (!CheckCosts())
            {
                return false;
            }
            if (!CanActivate())
            {
                return false; // 额外检查点
            }
            CurrentState = State.Activating;

            ApplyCosts();
            ApplyCooldown();
            OnActivate(); // 可重写：动画、特效等

            // 瞬发直接结束，持续技能进入Active
            if (Spec.Ability.IsInstant)
            {
                CurrentState = State.Ending;
                EndAbility();
            }
            else
            {
                CurrentState = State.Active;
            }
            return true;
        }
        public virtual void Tick()
        {
            if (CurrentState != State.Active) return;
            OnTick(); // 可重写：持续技能逻辑
            if (ShouldEndAbility())
            {
                CurrentState = State.Ending;
                EndAbility();
            }
        }
        public virtual void CancelAbility()
        {
            if (CurrentState is State.Activating or State.Active)
            {
                CurrentState = State.Ending;
                OnCancel(); // 可重写：取消回调
                EndAbility();
            }
        }

        // ========== 可重写方法（子类扩展） ==========

        /// <summary>激活前额外检查</summary>
        protected virtual bool CanActivate() => true;

        /// <summary>激活时执行（动画、特效、初始效果）</summary>
        protected virtual void OnActivate()
        {
            // 默认：应用配置的效果
            ApplyConfiguredEffects();
        }

        /// <summary>每帧更新（持续技能用）</summary>
        protected virtual void OnTick() { }

        protected virtual bool ShouldEndAbility()
        {
            return true;
        }
        protected virtual void OnCancel() { }
        protected virtual void OnEnd() { }

        // ========== 内部方法 ==========

        private void EndAbility()
        {
            if (CurrentState == State.Ended) return;

            OnEnd();
            CurrentState = State.Ended;
            OnEnded?.Invoke();
        }

        private void ApplyConfiguredEffects()
        {
            var ability = Spec.Ability;
            // 自身效果
            foreach (var effect in ability.ApplyToSelf)
            {
                ActorInfo.EffectManager.ApplyEffect(effect, ActivationTag);
            }

            // 目标效果
            if (TargetActor != null)
            {
                var targetEffectManager = TargetActor.GetComponent<GameplayEffectManager>();
                if (targetEffectManager != null)
                {
                    foreach (var effect in ability.ApplyToTarget)
                    {
                        targetEffectManager.ApplyEffect(effect, ActivationTag);
                    }
                }
            }
        }

        private bool CheckCosts()
        {
            var attrs = ActorInfo.ASC.GetComponent<AttributeSet>();
            foreach (var cost in Spec.Ability.Costs)
            {
                if (attrs.GetCurrent(cost.Attribute) < cost.Value)
                    return false;
            }
            return true;
        }

        private void ApplyCosts()
        {
            var attrs = ActorInfo.ASC.GetComponent<AttributeSet>();
            foreach (var cost in Spec.Ability.Costs)
            {
                var current = attrs.GetCurrent(cost.Attribute);
                attrs.SetCurrent(cost.Attribute, current - cost.Value);
            }
        }

        private void ApplyCooldown()
        {
            Spec.CooldownEndTime = Time.time + Spec.Ability.CooldownTime;
        }
    }
}
