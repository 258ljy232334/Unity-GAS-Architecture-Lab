using Gameplay.Tag;

namespace Gameplay.Effect
{
    [System.Serializable]
    public struct ModifierConfig
    {
        public GameplayTag AttributeTag;    //要修改的对应属性
        public ModifierType Operation;      //加，乘或者覆盖
        public float Value;
    }
}