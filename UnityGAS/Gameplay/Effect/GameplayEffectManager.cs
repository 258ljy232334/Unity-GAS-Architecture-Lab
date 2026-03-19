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
        //该字典的键为Effect或Ability的AssetTag
        private readonly Dictionary<GameplayTag, EffectInstance> _effects = new();
        private readonly List<ActiveModifier> _modifiers = new();
        public void Initialize(AttributeSet set,GameplayTagContainer container)
        {
            _attributes = set;
            _tags = container;
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
                    ExecuteInstant(effect);
                    effect.NextPeriodTime = Time.time + effect.Spec.Period;
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
            if (effect.DurationType == DurationType.Instant)
            {
                ExecuteInstant(new EffectInstance(effect, sourceTag));
                return;
            }
            if (_effects.TryGetValue(effect.AssetTag, out var existing))
            {
                if (effect.CanStack && existing.StackCount < effect.StackLimit)
                {
                    existing.StackCount++;
                    existing.ExpireTime = Time.time + effect.Duration;
                    // 手动收集属性，不用Linq
                    var attrs = new List<string>();
                    foreach (var mod in effect.Modifiers)
                    {
                        if (!attrs.Contains(mod.Attribute))
                        {
                            attrs.Add(mod.Attribute);
                        }
                    }
                    Recalculate(attrs);
                }
                return;
            }

            var instance = new EffectInstance(effect, sourceTag);
            _effects[effect.AssetTag] = instance;

            foreach (var mod in effect.Modifiers)
            {
                _modifiers.Add(new ActiveModifier
                {
                    AssetTag = effect.AssetTag,
                    Attribute = mod.Attribute,
                    Type = mod.Type,
                    Value = mod.Value
                });
            }
            foreach (var tag in effect.GrantedTags)
            {
                _tags.AddTag(tag);
            }
            // 手动收集属性
            var affectedAttrs = new List<string>();
            foreach (var mod in effect.Modifiers)
            {
                if (!affectedAttrs.Contains(mod.Attribute))
                {
                    affectedAttrs.Add(mod.Attribute);
                }
            }
            Recalculate(affectedAttrs);
        }

        public void RemoveEffect(GameplayTag assetTag)
        {
            if (!_effects.TryGetValue(assetTag, out var effect)) return;

            // 收集受影响属性（去重）
            var attrs = new List<string>();
            foreach (var mod in _modifiers)
            {
                if (mod.AssetTag == assetTag && !attrs.Contains(mod.Attribute))
                {
                    attrs.Add(mod.Attribute);
                }
            }

            // 移除修饰器
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i].AssetTag == assetTag)
                {
                    _modifiers.RemoveAt(i);
                }
            }

            foreach (var tag in effect.Spec.GrantedTags)
            { 
            _tags.RemoveTag(tag);
            }
            _effects.Remove(assetTag);
            Recalculate(attrs);
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
        private void ExecuteInstant(EffectInstance instance)
        {
            foreach (var mod in instance.Spec.Modifiers)
            {
                var value = mod.Value * instance.StackCount;
                var current = _attributes.GetCurrent(mod.Attribute);
                var newValue = mod.Type switch
                {
                    ModifierType.Add => current + value,
                    ModifierType.Multiply => current * value,
                    ModifierType.Override => value,
                    _ => current
                };
                _attributes.SetCurrent(mod.Attribute, newValue);
            }
        }
        private void Recalculate(List<string> attrs)
        {
            foreach (var attr in attrs)
            {
                var baseVal = _attributes.GetBase(attr);
                float add = 0, mul = 1, over = float.NaN;

                foreach (var mod in _modifiers)
                {
                    if (mod.Attribute != attr) continue;

                    switch (mod.Type)
                    {
                        case ModifierType.Add: add += mod.Value; break;
                        case ModifierType.Multiply: mul *= mod.Value; break;
                        case ModifierType.Override: over = mod.Value; break;
                    }
                }
                var result = float.IsNaN(over) ? (baseVal + add) * mul : over;
                _attributes.SetCurrent(attr, result);
            }
        }
        public int GetStackCount(GameplayTag assetTag) =>
            _effects.TryGetValue(assetTag, out var e) ? e.StackCount : 0;
        public bool HasEffect(GameplayTag assetTag) => _effects.ContainsKey(assetTag);

       
    }
}
