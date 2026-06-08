using Gameplay.Ability;
using Gameplay.Attribute;
using Gameplay.Effect;
using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.AbilitySystem
{
    public class AbilitySystemComponent : MonoBehaviour
    {
        [SerializeField] private GameplayEffectManager _effectManager;
        [SerializeField] private AttributeSet _attributeSet;
        [SerializeField] private ModifierManager _modifierManager;

        private GameplayTagContainer _tags;
        private AbilityActorInfo _actorInfo;

        private readonly Dictionary<GameplayTag, AbilitySpec> _grantedAbilities = new();
        private readonly Dictionary<GameplayTag, AbilityInstance> _activeInstances = new();
        [SerializeField]
        private List<GameplayAbility> _readyToGrantAbilities;
        private readonly List<GameplayTag> _endedTags = new();
        void Awake()
        {
            _effectManager ??= GetComponent<GameplayEffectManager>();
            _attributeSet ??= GetComponent<AttributeSet>();
            _modifierManager ??= GetComponent<ModifierManager>();

            _tags = new GameplayTagContainer();

            _attributeSet.Initialize();
            _modifierManager.Initialize(_attributeSet);

            _effectManager.Initialize(_attributeSet, _tags, _modifierManager);

            _actorInfo = new AbilityActorInfo(
                owner: gameObject,
                avatar: gameObject,
                effectManager: _effectManager,
                asc: this
            );
            foreach (var ability in _readyToGrantAbilities)
            {
                GrantAbility(ability);
            }
        }
        

        void Update()
        {
            foreach (var kvp in _activeInstances)
            {
                kvp.Value.Tick();
            }

            _endedTags.Clear();
            foreach (var kvp in _activeInstances)
            {
                if (kvp.Value.CurrentState == AbilityInstance.State.Ended)
                {
                    _endedTags.Add(kvp.Key);
                }
            }
            foreach (var tag in _endedTags)
            {
                _activeInstances.Remove(tag);
            }
        }

        
        public void GrantAbility(GameplayAbility ability, int level = 1)
        {
            _grantedAbilities[ability.AssetTag] = new AbilitySpec(ability, level);
        }

        public bool TryActivateAbility(GameplayTag assetTag,
            Vector3? targetPoint = null, GameObject targetActor = null)
        {
            if (!_grantedAbilities.TryGetValue(assetTag, out var spec))
            {
                return false;
            }
            if (_activeInstances.ContainsKey(assetTag) &&
                _activeInstances[assetTag].CurrentState != AbilityInstance.State.Ended)
            {
                return false;
            }
            var instance = new AbilityInstance(spec, _actorInfo,
                targetPoint ?? Vector3.zero, targetActor);
            if (!instance.TryActivate())
            {
                return false;
            }

            _activeInstances[assetTag] = instance;
            return true;
        }

        public bool TryActivateAbility(GameplayTag abilityTag) =>
            TryActivateAbility(abilityTag, null, null);

        // ȡ������
        public void CancelAbility(GameplayTag abilityTag)
        {
            if (_activeInstances.TryGetValue(abilityTag, out var instance))
            {
                instance.CancelAbility();
            }
        }

        // ��ѯ
        public bool HasTag(GameplayTag tag) => _tags.HasTag(tag);
        public AbilitySpec GetSpec(GameplayTag tag) => _grantedAbilities.GetValueOrDefault(tag);

        /// <summary>
        /// �����Ƿ��ڽ����У������С����������У����ѽ�����δ��ʵ��������Ϊ false��
        /// </summary>
        public bool IsAbilityActive(GameplayTag tag)
        {
            if (!_activeInstances.TryGetValue(tag, out var instance))
                return false;
            var s = instance.CurrentState;
            return s != AbilityInstance.State.None && s != AbilityInstance.State.Ended;
        }

        // �ڲ�����
        public GameplayEffectManager GetEffectManager() => _effectManager;
        public GameplayTagContainer GetTagContainer() => _tags;
    }
}
