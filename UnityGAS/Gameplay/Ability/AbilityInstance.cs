using Gameplay.Attribute;
using Gameplay.Effect;
using Gameplay.Tag;
using UnityEngine;

namespace Gameplay.Ability
{
    public class AbilityInstance 
    {
        public GameplayTag AssetTag { get; }
        public AbilitySpec Spec { get; }
        public AbilityActorInfo ActorInfo { get; }
        public Vector3 TargetPoint { get; }
        public GameObject TargetActor { get; }
        public enum State { None, Activating, Active, Ended }
        public State CurrentState { get; private set; } = State.None;


        public AbilityInstance(AbilitySpec spec, AbilityActorInfo info,
            Vector3 point, GameObject target)
        {
            Spec = spec;
            ActorInfo = info;
            TargetPoint = point;
            TargetActor = target;
            AssetTag = spec.Ability.AssetTag;
        }

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
            CurrentState = State.Activating;

            ApplyCosts();
            ApplyCooldown();
            OnActivate(); 

            // 瞬发技能直接结束，不进入Active
            if (Spec.Ability.IsInstant)
            {
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
            OnTick(); //�������ܵ��߼�
            if (ShouldEndAbility())
            {
                EndAbility();
            }
        }
        public virtual void CancelAbility()
        {
            if (CurrentState is State.Activating or State.Active)
            {
                OnCancel();
                EndAbility();
            }
        }

        protected virtual void OnActivate()
        {
            // Ĭ�ϣ�Ӧ�����õ�Ч��
            ApplyConfiguredEffects();
            GenerateConfiguredPrefab();
            ExecuteExtraFunc();
        }
        protected virtual void OnTick() { }

        protected virtual bool ShouldEndAbility()
        {
            return true;
        }
        protected virtual void OnCancel() { }
        protected virtual void OnEnd() { }


        private void EndAbility()
        {
            if (CurrentState == State.Ended) return;

            CurrentState = State.Ended;
            OnEnd();
        }

        private void ApplyConfiguredEffects()
        {
            var ability = Spec.Ability;
            // ����Ч��
            foreach (var effect in ability.ApplyToSelf)
            {
                ActorInfo.EffectManager.ApplyEffect(effect, AssetTag);
            }

            // Ŀ��Ч��
            if (TargetActor != null)
            {
                var targetEffectManager = TargetActor.GetComponent<GameplayEffectManager>();
                if (targetEffectManager != null)
                {
                    foreach (var effect in ability.ApplyToTarget)
                    {
                        targetEffectManager.ApplyEffect(effect, AssetTag);
                    }
                }
            }
        }
        //����Ԥ����
        private void GenerateConfiguredPrefab()
        {
            if (Spec.Ability.Prefab == null)
                return;

            // �Խ�ɫ Avatar Ϊ���ɻ��㣨û�о��� Owner��
            Transform spawnTransform = ActorInfo.Avatar != null
                ? ActorInfo.Avatar.transform
                : ActorInfo.Owner.transform;

            // ������������λ��
            Vector3 spawnPos = spawnTransform.position + spawnTransform.TransformDirection(Spec.Ability.PrefabOffset);

            // ����Ԥ���壨�̳н�ɫ����
            GameObject spawnedObj =GameObject.Instantiate(
                Spec.Ability.Prefab,
                spawnPos,
                spawnTransform.rotation
            );
        }
        //ִ�ж��ⷽ��
        private void ExecuteExtraFunc()
        {
            if (Spec.Ability.ExtraFunc != null && Spec.Ability.ExtraFunc.Count > 0)
            {
                for (int i = 0; i < Spec.Ability.ExtraFunc.Count; i++)
                {
                    Spec.Ability.ExtraFunc[i].ExecuteFunc(this);
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
