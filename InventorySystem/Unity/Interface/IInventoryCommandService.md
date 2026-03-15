```
namespace Inventory.Interface
{
    /// <summary>
    /// 库存系统增删物品的命令服务接口
    /// </summary>
    public interface IInventoryCommandService
    {
        bool TryAddItem(ContainerType type,int itemId,int count=1);
        bool TryReduceItem(ContainerType type, int index,int count=1);
        bool TryAddItem(ContainerType type, ItemInstance instance);
        bool TrySetItem(ContainerType type, int index, ItemInstance instance);
        bool TrySetItem(ContainerType type, int index,int itemId,int count=1);
        bool TryClearItem(ContainerType type,int index);
        bool TrySwapItem(ContainerType type,int indexA,int indexB);
        bool TryMoveItem(ContainerType containerA,ContainerType containerB,
            int indexA,int indexB);
       
    }
}

```
