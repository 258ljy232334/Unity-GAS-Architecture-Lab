```
namespace Item.Information
{
    /// <summary>
    /// 物品的基础信息
    /// </summary>
    [System.Serializable]
    public class ItemInformation
    {
        public int Id;
        public string Name;
        public string Description;
        public ItemFlags Flags;
        public ItemCategory Category;
        public bool CanStack;
        public int MaxStackCount;
        public int Price;
        public Sprite Icon;
    }
}

```
