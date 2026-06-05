using Inventory.Catalog;
using Inventory.Container;
using Inventory.Enum;
using Inventory.Factory;
using Inventory.Initialize;
using Inventory.Interface;
using Inventory.Repository;
using Inventory.Service;
using Inventory.Signal;
using System;
using Zenject;
namespace Inventory.Zenject
{
    public sealed class InventorySubInstaller : Installer<InventorySubInstaller>
    {
        [Inject]
        private InventoryContainerCatalog _catalog;
        public override void InstallBindings()
        {
            //绑定单例
            Container.BindInterfacesAndSelfTo<InventoryCatalogDictionary>().AsSingle();
            
            Container.Bind<IInventoryCommandService>().To<InventoryCommandService>().AsSingle();
            Container.Bind<IInventoryQueryService>().To<InventoryQueryService>().AsSingle();
            Container.Bind<IInventoryConsumableService>().To<InventoryConsumableService>().AsSingle();

            Container.Bind<IInventoryRepository>().To<InventoryRepository>().AsSingle();
           
            //绑定监听器
          
           
            
            Container.Bind<IItemFactory>().To<FoodFactory>().AsSingle();
           
            Container.BindInterfacesAndSelfTo<InventoryFactory>().AsSingle();

            //绑定容器
            BindContainer<BagContainer>(ContainerType.Bag);
            BindContainer<WeaponSlotContainer>(ContainerType.WeaponSlot);
          
            //绑定事件
            Container.DeclareSignal<InventoryItemChangedSignal>();
            Container.DeclareSignal<InventoryContainerCapacityChangedSignal>();

            //绑定初始化
            Container.BindInterfacesAndSelfTo<InventoryInitialize>().AsSingle();
           
        }
        public void BindContainer<TContainer>(ContainerType type) where TContainer : InventoryContainerBase
        {
            
            if (!_catalog.TryGet(type, out var cfg))
                throw new ArgumentException($"ContainerProfile missing for {type}");

            Container.Bind<TContainer>()
                     .WithId(type)                       // 用枚举当 ID
                     .AsSingle()                     
                     .OnInstantiated<TContainer>((ctx, container) =>
                         container.Initialize(cfg.DefaultCapacity,
                                              cfg.AddOnceCapacity,
                                              cfg.MaxCapacity));
        }
    }
}