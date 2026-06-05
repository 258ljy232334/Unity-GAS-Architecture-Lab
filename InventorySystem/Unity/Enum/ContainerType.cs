namespace Inventory.Enum
{

    /// <summary>
    /// 容器类型,仅用于依赖注入区分
    /// </summary>
    public enum ContainerType
    {
        None,
        Bag,            // 背包
        WeaponSlot,     // 武器槽
    }
}
