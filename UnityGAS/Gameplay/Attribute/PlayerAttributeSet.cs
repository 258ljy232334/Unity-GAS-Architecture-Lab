namespace Gameplay.Attribute
{
    public class PlayerAttributeSet : AttributeSet
    {
        public override void Initialize()
        {
            AddAttribute("Health", 1);
            AddAttribute("MaxHealth", 100);
        }
    }
}