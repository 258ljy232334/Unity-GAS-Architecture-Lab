
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Attribute
{
    public abstract class AttributeSet :MonoBehaviour
    {
        public abstract void Initialize();
        protected Dictionary<string, AttributeValue> _attributeDic = new Dictionary<string, AttributeValue>();
        public float GetBase(string name)
        {
            if (_attributeDic.ContainsKey(name))
            {
                return _attributeDic[name].BaseValue;
            }
            return 0f;
        }
        public float GetCurrent(string name)
        {
            if (_attributeDic.ContainsKey(name))
            {
                return _attributeDic[name].CurrentValue;
            }
            return 0f;
        }
        public void SetBase(string name, float value)
        {
            if (_attributeDic.ContainsKey(name))
            {
                _attributeDic[name].BaseValue = value;
            }
        }
        public void SetCurrent(string name, float value)
        {
            if (_attributeDic.ContainsKey(name))
            {
                _attributeDic[name].CurrentValue = value;
            }
        }
        protected void AddAttribute(string name, float value)
        {
            AttributeValue attributeValue = new AttributeValue(name,value);
            _attributeDic.Add(name, attributeValue);
        }
       
    }
       
}