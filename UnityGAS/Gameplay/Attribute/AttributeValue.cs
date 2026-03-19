

namespace Gameplay.Attribute
{
    [System.Serializable]
    public class AttributeValue
    {
        public string Name;
        public float BaseValue;
        public float CurrentValue;
        public AttributeValue(string name,float value)
        {
            Name = name;
            BaseValue = value;
            CurrentValue = value;
        }
    }
}