using Inventory.Data;
using Inventory.Interface;
using Item.Information;
using Item.Instance;
using System.Collections.Generic;
using Zenject;

namespace Inventory.Factory
{
    /// <summary>
    /// 物品工厂实例
    /// </summary>
    public class InventoryFactory:IInventoryFactory
    {
        [Inject] 
        private IInventoryQueryService _query;
        [Inject]
        private List<IItemFactory> _itemFactorys;
       

       
       
        public ItemInstance CreateItem(ItemInformation info, int count = 1)
        {
            IItemFactory itemFactory = FindFactory(info);
            if(itemFactory == null)
            {
                return null;
            }
            return itemFactory.CreateItem(info, count);
        }

        public ItemInstance DataToInstance(ItemData data)
        {
            ItemInformation info = _query.GetItemInformation(data.ItemId);
            if (info == null)
            {
                return null;
            }
            IItemFactory itemFactory = FindFactory(info);
            if(itemFactory == null)
            {
                return null;
            }
            return itemFactory.DataToInstance(info,data);
        }
        public ItemData InstanceToData(ItemInstance instance)
        {
            ItemData data = instance.ToData();
            return data;
        }
        private IItemFactory FindFactory(ItemInformation info)
        {
            foreach (var itemFactory in _itemFactorys)
            {
                if(itemFactory.CanCreateItem(info))
                {
                    return itemFactory;
                }
            }
            return null;
        }
    }
}
