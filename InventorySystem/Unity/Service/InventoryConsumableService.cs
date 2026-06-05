using Consumable;
using Inventory.Interface;
using Item.Information;
using Item.Instance;
using UnityEngine;
using Zenject;

namespace Inventory.Service
{
    public class InventoryConsumableService : IInventoryConsumableService
    {
        [Inject(Id = "Player")]
        private GameObject _player;

        public bool TryConsumeItem(ItemInstance instance, int count = 1)
        {
            if (instance == null)
            {
                return false;
            }
            if (instance.Information is not ConsumableInformation consumableInformation)
            {
                return false;
            }
            if (consumableInformation.Strategy == null)
            {
                Debug.LogWarning("消耗品未配置 Strategy。");
                return false;
            }
            if (_player == null)
            {
                Debug.LogWarning("未注入 Id 为 Player 的 GameObject，无法使用消耗品。");
                return false;
            }
            return consumableInformation.Strategy.Consume(instance, new UseContext(_player, null)).Success;
        }
    }
}
