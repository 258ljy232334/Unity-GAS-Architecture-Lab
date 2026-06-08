
using Gameplay.Tag;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.Attribute
{
    [CreateAssetMenu(fileName = "AttributeSetConfig",
        menuName = "SO/Gameplay/AttributeSetConfig")]
    public class AttributeSetConfig : ScriptableObject
    {
        public List<AttributeValueData> AttributeValues;
    }
    [System.Serializable]
    public struct AttributeValueData
    {
        public GameplayTag AttributeTag;
        public float MinValue;
        public float MaxValue;
        public float InitializeValue;
        public GameplayTag LimitAttributeTag;
    }
}