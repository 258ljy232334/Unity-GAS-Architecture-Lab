using Inventory.Container;
using Inventory.Enum;
using Inventory.Interface;
using System;
using Zenject;

namespace Inventory.Initialize
{
    /// <summary>
    /// ???????? <see cref="IInventoryRepository"/>?
    /// </summary>
    public sealed class InventoryInitialize : IInitializable, IDisposable
    {
        [Inject]
        private IInventoryRepository _repo;
        [Inject(Id = ContainerType.Bag)]
        private BagContainer _bag;

        [Inject(Id = ContainerType.WeaponSlot)]
        private WeaponSlotContainer _weaponSlot;

        public void Initialize()
        {
            BindContainers();
        }

        public void Dispose()
        {
            UnbindContainers();
        }

        private void BindContainers()
        {
            _repo.RegisterContainer(ContainerType.Bag, _bag);
            _repo.RegisterContainer(ContainerType.WeaponSlot, _weaponSlot);
        }

        private void UnbindContainers()
        {
            _repo.UnregisterContainer(ContainerType.Bag);
            _repo.UnregisterContainer(ContainerType.WeaponSlot);
        }
    }
}
