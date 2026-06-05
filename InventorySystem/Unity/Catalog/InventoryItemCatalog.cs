using Item.Information;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Inventory.Catalog
{
    /// <summary>
    /// ¿â´æÄ¿Â¼
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryCatalog", menuName =
        "SO/Inventory/InventoryCatalog", order = 1)]
    public sealed class InventoryItemCatalog : ScriptableObjectInstaller<InventoryItemCatalog>
    {
        public List<FoodInformation> Foods;
        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
       
    }
}
