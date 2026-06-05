using Inventory.Enum;
using Item.Instance;
namespace Inventory.Signal
{
    public readonly struct InventoryItemChangedSignal
    {
        public readonly ContainerType ContainerType;
        public readonly int Index;
        public readonly ItemInstance Instance;
        public InventoryItemChangedSignal(ContainerType type,int index,ItemInstance instance)
        {
            ContainerType = type;
            Index = index;
            Instance = instance;
        }
    }
}
