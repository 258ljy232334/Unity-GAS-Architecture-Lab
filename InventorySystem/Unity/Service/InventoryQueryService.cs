using Inventory.Container;
using Inventory.Enum;
using Inventory.Interface;
using Item.Information;
using Item.Instance;
using System;
using Zenject;

namespace Inventory.Service
{
    public class InventoryQueryService : IInventoryQueryService
    {
        [Inject]
        private IInventoryRepository _repository;
        [Inject]
        private IInventoryCatalogDictionary _catalog;

        public ItemInformation GetItemInformation(int itemId)
        {
            return _catalog.GetItemInformation(itemId);
        }

        public ItemInstance GetItemInstance(ContainerType type, int index)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            return container.GetInstance(index);
        }

        private bool TryGetContainer(ContainerType type, out InventoryContainerBase container)
        {
            container = _repository.GetContainer(type);
            if (container == null)
            {
                return false;
            }
            return true;
        }
    }
}
