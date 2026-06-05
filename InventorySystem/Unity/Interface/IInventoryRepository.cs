using Inventory.Container;
using Inventory.Enum;
using System.Collections.Generic;

namespace Inventory.Interface

{
    /// <summary>
    /// ¿â´æ²Ö¿â½Ó¿Ú
    /// </summary>
    public interface IInventoryRepository
    {
        /* ---------- 1. ÄÃÈÝÆ÷ ---------- */
        InventoryContainerBase GetContainer(ContainerType containerId);
        IReadOnlyList<InventoryContainerBase> GetContainers();

        /* ---------- 2. ×¢²á/Ð¶ÔØ ---------- */
        void RegisterContainer(ContainerType type, InventoryContainerBase container);
        void UnregisterContainer(ContainerType type);
    }
}
