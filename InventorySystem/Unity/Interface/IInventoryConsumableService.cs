using Item.Instance;
namespace Inventory.Interface
{
    public interface IInventoryConsumableService 
    {
        bool TryConsumeItem(ItemInstance instance,int count=1);
    }
}
