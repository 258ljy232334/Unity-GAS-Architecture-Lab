using Inventory.Enum;

namespace Inventory.Signal
{
    public readonly struct InventoryContainerCapacityChangedSignal 
    {
        public readonly ContainerType ContainerType;
        public readonly int CurrentCapacity;
        public InventoryContainerCapacityChangedSignal(ContainerType type,int capacity)
        {
            ContainerType = type;
            CurrentCapacity = capacity;
        }
    }
}
