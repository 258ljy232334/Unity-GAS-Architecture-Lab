using Gameplay.Tag;

namespace Gameplay.Effect
{
    [System.Serializable]
    public struct ModifierConfig
    {
        public GameplayTag AttributeTag;
        public ModifierType Operation;
        public float Value;
    }
}