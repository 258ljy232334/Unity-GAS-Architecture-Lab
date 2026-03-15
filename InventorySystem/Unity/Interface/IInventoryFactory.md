```
namespace Inventory.Interface
{
    /// <summary>
    /// 物品工厂接口,用于创建和还原物品实例
    /// </summary>

    public interface IInventoryFactory
    {
        //创建物品实例
        ItemInstance CreateItem(ItemInformation information, int count = 1);
        //还原物品实例
        ItemInstance DataToInstance(ItemData data);

        ItemData InstanceToData(ItemInstance instance);
    }
}
```
