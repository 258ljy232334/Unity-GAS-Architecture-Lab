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

        private GameplayTagContainer _tags;
        private AbilityActorInfo _actorInfo;

        // 授予的技能
        private readonly Dictionary<GameplayTag, AbilitySpec> _grantedAbilities = new();
        // 活跃的技能实例
        private readonly Dictionary<GameplayTag, AbilityInstance> _activeInstances = new();
        [SerializeField]
        private List<GameplayAbility> _readyToGrantAbilities;
        void Awake()
        {
            _effectManager ??= GetComponent<GameplayEffectManager>();
            _attributeSet ??= GetComponent<AttributeSet>();

            _tags = new GameplayTagContainer();

            _attributeSet.Initialize();
            _effectManager.Initialize(_attributeSet, _tags);

            _actorInfo = new AbilityActorInfo(
                owner: gameObject,
                avatar:gameObject,
                effectManager: _effectManager,
                asc: this
            );
        }
        private void Start()
        {
            foreach (var ability in _readyToGrantAbilities)
            {
                GrantAbility(ability);
            }
        }
        void Update()
        {
            // 驱动持续技能
            foreach (var kvp in _activeInstances)
            {
                kvp.Value.Tick();
            }
        }

        // 授予技能
        public void GrantAbility(GameplayAbility ability, int level = 1)
        {
            _grantedAbilities[ability.AbilityTag] = new AbilitySpec(ability, level);
        }

        // 激活技能
        public bool TryActivateAbility(GameplayTag abilityTag,
            Vector3? targetPoint = null, GameObject targetActor = null)
        {
            // 检查是否已授予
            if (!_grantedAbilities.TryGetValue(abilityTag, out var spec))
            {
                Debug.LogWarning("还没有授予该能力");
                return false;
            }
            // 检查是否已激活（限制并发）
            if (_activeInstances.ContainsKey(abilityTag)&&
                _activeInstances[abilityTag].CurrentState != AbilityInstance.State.Ended)
            {
                Debug.LogWarning("该能力已经持有且未结束");
                return false;           
            }
            // 创建并激活
            var instance = new AbilityInstance(spec, _actorInfo,
                targetPoint ?? Vector3.zero, targetActor);
            instance.OnEnded += () =>
            {
                _activeInstances.Remove(abilityTag);
            };
            if (!instance.TryActivate())
            {
                Debug.LogWarning("该能力激活失败");
                return false;
            }
            // 记录并订阅结束事件
            _activeInstances[abilityTag] = instance;
            
            return true;
        }

        public bool TryActivateAbility(GameplayTag abilityTag) =>
            TryActivateAbility(abilityTag, null, null);

        // 取消技能
        public void CancelAbility(GameplayTag abilityTag)
        {
            if (_activeInstances.TryGetValue(abilityTag, out var instance))
            {
                instance.CancelAbility();
            }
        }

        // 查询
        public bool HasTag(GameplayTag tag) => _tags.HasTag(tag);
        public AbilitySpec GetSpec(GameplayTag tag) => _grantedAbilities.GetValueOrDefault(tag);
        public bool IsAbilityActive(GameplayTag tag) => _activeInstances.ContainsKey(tag);

        // 内部访问
        internal GameplayEffectManager GetEffectManager() => _effectManager;
        internal GameplayTagContainer GetTagContainer() => _tags;
    }
}
