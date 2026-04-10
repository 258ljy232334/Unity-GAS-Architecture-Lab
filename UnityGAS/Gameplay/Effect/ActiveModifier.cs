using Gameplay.Tag;

namespace Gameplay.Effect
{
    public struct ActiveModifier
    {
        public GameplayTag AssetTag;
        public EffectInstance SourceEffect;
        public ModifierConfig Config; 
    }
}