
using Inventory.Enum;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Inventory.Catalog
{
    [CreateAssetMenu(fileName ="Containers",
        menuName ="SO/Inventory/Containers")]
    public class InventoryContainerCatalog : ScriptableObjectInstaller<InventoryContainerCatalog>
    {
        public List<InventoryContainer> Containers;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
        public bool TryGet(ContainerType type, out InventoryContainer cfg)
        {
            foreach (var c in Containers)
            {
                if (c.ContainerType == type) 
                { 
                    cfg = c;
                    return true; 
                }
            }
            cfg = null;
            return false;
        }
    }
    [System.Serializable]
    public class InventoryContainer
    {
        public ContainerType ContainerType;
        public int DefaultCapacity;
        public int AddOnceCapacity;
        public int MaxCapacity;
    }
}

