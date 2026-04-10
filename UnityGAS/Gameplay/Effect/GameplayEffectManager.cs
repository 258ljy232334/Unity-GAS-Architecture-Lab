using Gameplay.Attribute;
using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Effect
{
    public class GameplayEffectManager : MonoBehaviour
    {
        private AttributeSet _attributes;
        private GameplayTagContainer _tags;
        private ModifierManager _modifierManager;
        //该字典的键为Effect或Ability的AssetTag
        private readonly Dictionary<GameplayTag, EffectInstance> _effects = new();
        public void Initialize(AttributeSet set,GameplayTagContainer container,ModifierManager modifierManager)
        {
            _attributes = set;
            _tags = container;
            _modifierManager = modifierManager;
        }
        private void Update()
        {
            if(_attributes==null||_tags==null)
            {
                return;
            }

            var toRemove = new List<GameplayTag>();
            foreach (var kvp in _effects)
            {
                var effect = kvp.Value;
                if (effect.ShouldTriggerPeriod)
                {
                    ExecutePeriod(effect);
                }
                if (effect.IsExpired)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var tag in toRemove)
            {
                RemoveEffect(tag);
            }
        }
        
        public void ApplyEffect(GameplayEffect effect, GameplayTag sourceTag)
        {
            if (effect.EffectDurationType == DurationType.Instant)
            {
                ExecuteInstant(new EffectInstance(effect, sourceTag));
                return;
            }
            if (_effects.TryGetValue(effect.AssetTag, out var existing))
            {
                if (effect.StackType==StackType.CanStack && existing.StackCount < effect.MaxStackCount)
                {
                    existing.StackCount++;
                    existing.ExpireTime = Time.time + effect.Duration;
                    _modifierManager.UpdateModifiersStack(existing);
                }
                else if(effect.StackType == StackType.Refresh
                    &&existing.Config.EffectDurationType
                    ==DurationType.HasDuration)
                {
                    existing.ExpireTime = Time.time + effect.Duration;
                }
                return;
            }

            var instance = new EffectInstance(effect, sourceTag);
            _effects[effect.AssetTag] = instance;
            //执行瞬时效果，添加周期效果和永久效果的修饰器，添加标签
            ExecuteInstant(instance);
            _modifierManager.AddModifiers(effect.PermanentModifiers,instance);
            foreach (var tag in effect.GrantedTags)
            {
                _tags.AddTag(tag);
            }
        }
        public void RemoveEffect(GameplayTag assetTag)
        {
            if (!_effects.TryGetValue(assetTag, out var effect)) return;

            // 收集受影响属性（去重）
            _modifierManager.RemoveModifiersByEffect(effect);
            foreach (var tag in effect.Config.GrantedTags)
            { 
            _tags.RemoveTag(tag);
            }
            _effects.Remove(assetTag);
        }

        public void RemoveEffectsBySource(GameplayTag sourceTag)
        {
            var toRemove = new List<GameplayTag>();
            foreach (var kvp in _effects)
            {
                if (kvp.Value.SourceTag == sourceTag)
                    toRemove.Add(kvp.Key);
            }
            foreach (var tag in toRemove)
            {
                RemoveEffect(tag);
            }
        }
        //执行周期效果，更新下一次触发时间
        private void ExecutePeriod(EffectInstance instance)
        {
            instance.NextPeriodTime = Time.time + instance.Config.Period;
            foreach(var mod in instance.Config.PeriodModifiers)
            {
                var value = mod.Value * instance.StackCount;
                var current = _attributes.GetCurrent(mod.AttributeTag);
                var newValue = mod.Operation switch
                {
                    ModifierType.Add => current + value,
                    ModifierType.Multiply => current * value,
                    ModifierType.Override => value,
                    _ => current
                };
                _attributes.SetCurrent(mod.AttributeTag, newValue);
            }
        }
        //执行瞬时效果
        private void ExecuteInstant(EffectInstance instance)
        {
            foreach (var mod in instance.Config.InstantModifiers)
            {
                var value = mod.Value * instance.StackCount;
                var current = _attributes.GetCurrent(mod.AttributeTag);
                var newValue = mod.Operation switch
                {
                    ModifierType.Add => current + value,
                    ModifierType.Multiply => current * value,
                    ModifierType.Override => value,
                    _ => current
                };
                _attributes.SetCurrent(mod.AttributeTag, newValue);
            }
        }
        
        public int GetStackCount(GameplayTag assetTag) =>
            _effects.TryGetValue(assetTag, out var e) ? e.StackCount : 0;
        public bool HasEffect(GameplayTag assetTag) => _effects.ContainsKey(assetTag);

       
    }
}
