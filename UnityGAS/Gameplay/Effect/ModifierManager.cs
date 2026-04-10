
using Gameplay.Attribute;
using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Effect
{
    public class ModifierManager : MonoBehaviour
    {
        private AttributeSet _attributes;
        //用于缓存需要修改的属性，避免重复计算
        private HashSet<GameplayTag> _cacheAttributes;
        private readonly Dictionary<GameplayTag, List<ActiveModifier>> _modifiers = new();
        public void Initialize(AttributeSet attributes)
        {
            _cacheAttributes = new HashSet<GameplayTag>();
            _attributes = attributes;
        }
        public void AddModifiers(List<ModifierConfig> permanentModifiers, EffectInstance effect)
        {
            _cacheAttributes.Clear();
            //添加永久属性，永久属性会一直存在，直到被移除
            foreach (ModifierConfig mod in permanentModifiers)
            {
                _cacheAttributes.Add(mod.AttributeTag);
                if (!_modifiers.TryGetValue(mod.AttributeTag, out List<ActiveModifier> activeModifiers))
                {
                    // 不存在：创建新列表
                    activeModifiers = new List<ActiveModifier>();
                    _modifiers.Add(mod.AttributeTag, activeModifiers);
                }
                // 无论是否存在，都直接 Add 即可
                activeModifiers.Add(new ActiveModifier()
                {
                    Config = mod,
                    SourceEffect = effect,
                    AssetTag = effect.SourceTag,
                });
            }
            Recalculate(_cacheAttributes);
        }
        public void RemoveModifiersByEffect(EffectInstance effect)
        {
            _cacheAttributes.Clear();
            foreach(var mod in effect.Config.PermanentModifiers)
            {
                if(_modifiers.TryGetValue(mod.AttributeTag,out List<ActiveModifier> activeModifiers))
                {
                    for(int i = activeModifiers.Count - 1; i >= 0; i--)
                    {
                        if (activeModifiers[i].SourceEffect == effect)
                        {
                            activeModifiers.RemoveAt(i);
                        }
                    }
                    _cacheAttributes.Add(mod.AttributeTag);
                }
            }
            Recalculate(_cacheAttributes);
        }
        private void Recalculate(HashSet<GameplayTag> attributes)
        {
            foreach(var attribute in attributes)
            {
                RecalculateAttribute(attribute);
            }
        }
        //重新计算属性值，考虑所有的Modifier
        private void RecalculateAttribute(GameplayTag attribute)
        {
            if(_modifiers.TryGetValue(attribute,out List<ActiveModifier> activeModifiers))
            {
                var current = _attributes.GetBase(attribute);
                float sumAdd = 0;
                float sumMultiply = 1;
                float overrideValue=float.NaN;
                foreach (var mod in activeModifiers)
                {
                    var value = mod.Config.Value * mod.SourceEffect.StackCount;
                   
                    switch (mod.Config.Operation)
                    {
                        case ModifierType.Add:
                            sumAdd += value;
                            break;
                        case ModifierType.Multiply:
                            sumMultiply += value;
                            break;
                        case ModifierType.Override:
                            overrideValue = value;
                            break;
                    }
                }
                float finalValue = float.IsNaN(overrideValue) ? current *(1+ sumMultiply) + sumAdd : overrideValue;
                _attributes.SetCurrent(attribute, finalValue);
            }
        }

        public void UpdateModifiersStack(EffectInstance effect)
        {
            _cacheAttributes.Clear();
            foreach (var mod in effect.Config.PermanentModifiers)
            {
                _cacheAttributes.Add(mod.AttributeTag);
            }
            Recalculate(_cacheAttributes);
        }
    }
}