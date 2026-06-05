using Inventory.Data;
using Inventory.Enum;
using Inventory.Interface;
using Item.Information;
using Item.Instance;
using System;

namespace Inventory.Factory {
    public class FoodFactory : IItemFactory
    {
        

        public bool CanCreateItem(ItemInformation info)
        {
            return info != null && info is FoodInformation;
        }

        public ItemInstance CreateItem(ItemInformation information, int count = 1)
        {
            return new FoodInstance(information, count,Guid.NewGuid().ToString());
        }

        public ItemInstance DataToInstance(ItemInformation info, ItemData data)
        {
            FoodInstance food = new FoodInstance(info, data.Count, data.Guid);
            food.FromData(data);
            return food;
        }
    }
}
