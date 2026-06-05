using Inventory.Container;
using Inventory.Enum;
using Inventory.Interface;
using Item.Information;
using Item.Instance;
using System;
using UnityEngine;
using Zenject;

namespace Inventory.Service
{
    /// <summary>
    /// ?????????????
    /// </summary>
    public sealed class InventoryCommandService : IInventoryCommandService
    {
        [Inject]
        private IInventoryRepository _repository;
        [Inject]
        private IInventoryCatalogDictionary _catalog;
        [Inject]
        private IInventoryFactory _factory;

        public bool TryAddItem(ContainerType type, int itemId, int count = 1)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            if (count <= 0)
            {
                return false;
            }
            ItemInstance instance = CreateItem(itemId, count);
            if (instance == null)
            {
                return false;
            }
            return container.TryAddInstance(instance);
        }

        public bool TrySetItem(ContainerType type, int index, ItemInstance instance)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            return container.SetInstance(index, instance);
        }

        public bool TrySetItem(ContainerType type, int index, int itemId, int count = 1)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            if (count <= 0)
            {
                return false;
            }
            ItemInstance instance = CreateItem(itemId, count);
            if (instance == null)
            {
                return false;
            }
            return container.SetInstance(index, instance);
        }

        public bool TryAddItem(ContainerType type, ItemInstance instance)
        {
            if (instance == null)
            {
                return false;
            }
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            return container.TryAddInstance(instance);
        }

        public bool TryReduceItem(ContainerType type, int index, int count = 1)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            if (count <= 0)
            {
                return false;
            }
            return container.TryReduceInstance(index, count);
        }

        /// <summary>
        /// ???????????
        /// </summary>
        public bool TryClearItem(ContainerType type, int index)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            return container.TryRemoveInstance(index);
        }

        public bool TrySwapItem(ContainerType type, int indexA, int indexB)
        {
            if (!TryGetContainer(type, out var container))
            {
                throw new InvalidOperationException($"?? {type} ???");
            }
            return container.SwapInstance(indexA, indexB);
        }

        public bool TryMoveItem(ContainerType typeA, ContainerType typeB, int indexA, int indexB)
        {
            if (!TryGetContainer(typeA, out var containerA) || !TryGetContainer(typeB, out var containerB))
            {
                throw new InvalidOperationException("?????");
            }

            ItemInstance instanceA = containerA.GetInstance(indexA);
            ItemInstance instanceB = containerB.GetInstance(indexB);
            if (instanceA == null && instanceB == null)
            {
                return false;
            }
            if ((instanceA != null && !containerB.CanAccept(instanceA))
                || (instanceB != null && !containerA.CanAccept(instanceB)))
            {
                return false;
            }

            containerA.SetInstance(indexA, instanceB);
            containerB.SetInstance(indexB, instanceA);
            return true;
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

        private ItemInstance CreateItem(int itemId, int count)
        {
            ItemInformation info = _catalog.GetItemInformation(itemId);
            if (info == null)
            {
                Debug.Log("?? ID ??");
                return null;
            }
            return _factory.CreateItem(info, count);
        }
    }
}
