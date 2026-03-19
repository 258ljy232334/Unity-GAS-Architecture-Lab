using Gameplay.Tag;

namespace Gameplay.Effect
{
    public struct ActiveModifier
    {
        public GameplayTag AssetTag;
        public string Attribute;
        public ModifierType Type;
        public float Value;
    }
}