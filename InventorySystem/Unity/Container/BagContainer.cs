using Inventory.Enum;
using Item.Instance;

namespace Inventory.Container
{
    /// <summary>
    /// ???????
    /// </summary>
    public sealed class BagContainer : InventoryContainerBase
    {
        public override ContainerType ContainerType => ContainerType.Bag;

        public override bool CanAccept(ItemInstance instance)
        {
            return true;
        }
    }
}
