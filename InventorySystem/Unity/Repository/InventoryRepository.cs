using Inventory.Container;
using Inventory.Enum;
using Inventory.Interface;
using System;
using System.Collections.Generic;

namespace Inventory.Repository
{
    /// <summary>
    /// ????????
    /// </summary>
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly Dictionary<ContainerType, InventoryContainerBase> _containers = new();

        public InventoryContainerBase GetContainer(ContainerType containerId)
        {
            return _containers.TryGetValue(containerId, out var container) ? container : null;
        }

        public IReadOnlyList<InventoryContainerBase> GetContainers()
        {
            var res = new List<InventoryContainerBase>();
            foreach (var container in _containers.Values)
            {
                res.Add(container);
            }
            return res.AsReadOnly();
        }

        public void RegisterContainer(ContainerType type, InventoryContainerBase container)
        {
            _containers[type] = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void UnregisterContainer(ContainerType containerId)
        {
            _containers.Remove(containerId);
        }
    }
}
