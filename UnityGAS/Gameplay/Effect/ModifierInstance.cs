using Gameplay.Tag;

namespace Gameplay.Effect
{
    public struct ModifierInstance
    {
        public GameplayTag AssetTag;
        public EffectInstance SourceEffect;
        public ModifierConfig Config; 
    }
}