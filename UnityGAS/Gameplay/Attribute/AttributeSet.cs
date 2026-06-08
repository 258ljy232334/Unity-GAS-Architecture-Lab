using Gameplay.Tag;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Attribute
{
    public  class AttributeSet :MonoBehaviour
    {
        [SerializeField]
        private AttributeSetConfig _config;
        
        public event Action<GameplayTag, float, float> OnAttributeChanged;
        protected Dictionary<GameplayTag, AttributeValue> _attributeDic = new Dictionary<GameplayTag, AttributeValue>();
        public void Initialize()
        {
            foreach(var data in _config.AttributeValues)
            {
                AddAttribute(data);
            }
        }
        public float GetBase(GameplayTag tag)
        {
            if (_attributeDic.ContainsKey(tag))
            {
                return _attributeDic[tag].BaseValue;
            }
            return 0f;
        }
        public float GetCurrent(GameplayTag tag)
        {
            if (_attributeDic.ContainsKey(tag))
            {
                return _attributeDic[tag].CurrentValue;
            }
            return 0f;
        }
        public void SetBase(GameplayTag tag, float value)
        {
            if (_attributeDic.ContainsKey(tag))
            {
                _attributeDic[tag].BaseValue =value;
            }
        }
        //子类可以重写这个方法来实现不同的设置当前值的逻辑
        //比如触发事件或者进行额外的计算
        public virtual void SetCurrent(GameplayTag tag, float value)
        {
            if (_attributeDic.TryGetValue(tag, out var attribute))
            {
                float old = attribute.CurrentValue;
                float max = attribute.MaxValue;
                if (attribute.LimitTag!=GameplayTag.None&&_attributeDic.TryGetValue(attribute.LimitTag, out var limit))
                {
                    max = limit.CurrentValue;
                }
                float finalValue = Mathf.Clamp(value, attribute.MinValue, max);
                attribute.CurrentValue = finalValue;
                OnAttributeChanged?.Invoke(tag, old, finalValue);
            }
        }
        protected void AddAttribute(AttributeValueData data)
        {
            if(data.AttributeTag==GameplayTag.None)
            {
                throw new ArgumentException(nameof(data), "not set the attribute");
            }
            if(data.AttributeTag==data.LimitAttributeTag)
            {
                throw new ArgumentException(nameof(data.AttributeTag), "this attribute can't limit itself");
            }
            AttributeValue attributeValue = new AttributeValue(data.AttributeTag,data.InitializeValue,data.MinValue,data.MaxValue,data.LimitAttributeTag);
            _attributeDic.Add(data.AttributeTag, attributeValue);
        }
    }
       
}