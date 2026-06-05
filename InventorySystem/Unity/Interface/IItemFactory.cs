using Inventory.Data;
using Inventory.Enum;
using Item.Information;
using Item.Instance;
namespace Inventory.Interface
{
    public interface IItemFactory 
    {
        bool CanCreateItem(ItemInformation info);
        ItemInstance CreateItem(ItemInformation information, int count = 1);
        //还原物品实例
        ItemInstance DataToInstance(ItemInformation info,ItemData data);
       
    }
}