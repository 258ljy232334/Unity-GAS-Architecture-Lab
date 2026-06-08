using Gameplay.Tag;

namespace Gameplay.Attribute
{
    [System.Serializable]
    public class AttributeValue
    {
        public GameplayTag Tag;
        public float BaseValue;
        public float CurrentValue;
        public float MaxValue { get; private set; }
        public float MinValue { get; private set; }
        public GameplayTag LimitTag { get; private set; }
        public AttributeValue(GameplayTag name,float value,float min=float.MinValue,float max=float.MaxValue,GameplayTag limit=GameplayTag.None)
        {
            Tag = name;
            BaseValue = value;
            CurrentValue = value;
            MaxValue = max;
            MinValue = min;
            LimitTag = limit;
        }
    }
}