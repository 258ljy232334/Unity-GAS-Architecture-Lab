using Inventory.Enum;
using Item.Information;
using Item.Instance;

namespace Inventory.Interface
{
    public interface IInventoryQueryService 
    {
        ItemInstance GetItemInstance(ContainerType type, int index);
        ItemInformation GetItemInformation(int itemId);
    }
}
