using Inventory.Enum;
using Item.Enum;
using Item.Information;
using Item.Instance;

namespace Inventory.Container
{
    /// <summary>
    /// ������λ������������ <see cref="ItemCategory.Equipment"/>���Ҿܾ�����Ʒ���ݣ�<see cref="ConsumableInformation"/> �������ࣩ��
    /// </summary>
    public sealed class WeaponSlotContainer : InventoryContainerBase
    {
        public override ContainerType ContainerType => ContainerType.WeaponSlot;

        public override bool CanAccept(ItemInstance instance)
        {
            if (instance == null)
                return true;
            if (instance.Information == null)
                return false;
            if (instance.Information is ConsumableInformation)
                return false;
            return instance.Information.Category == ItemCategory.Equipment;
        }

        public override bool TryAddInstance(ItemInstance instance)
        {
            return false;
        }
    }
}
