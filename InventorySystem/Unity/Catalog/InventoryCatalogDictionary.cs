using Inventory.Interface;
using Item.Information;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Inventory.Catalog
{
    /// <summary>
    /// 库存目录字典
    /// </summary>

    public sealed class InventoryCatalogDictionary :IInventoryCatalogDictionary,IInitializable
    {
        [Inject]
        private InventoryItemCatalog _catalog;
        
        private Dictionary<int, ItemInformation> _infoDic = new Dictionary<int, ItemInformation>();

        public void Initialize()
        {
            BuildDictionary();
        }
        /// <summary>
        /// 重新构建字典（编辑器与运行时复用）
        /// </summary>
        private void BuildDictionary()
        {
            _infoDic.Clear();
            if (_catalog == null)
            {
                return;
            }
            // 逐个列表添加，AddList 负责 null 与项为 null 的检查
            AddList(_catalog.Foods);
        }

        private void AddList<T>(List<T> list) where T : ItemInformation
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item == null) continue;
                _infoDic[item.Id] = item;
            }
        }

        public ItemInformation GetItemInformation(int itemId)
        {
            _infoDic.TryGetValue(itemId, out var info);
            return info;
        }

        /// <summary>
        /// 更高效的查询方式，避免外部再进行 null 判断
        /// </summary>
        public bool TryGetItemInformation(int itemId, out ItemInformation info)
        {
            return _infoDic.TryGetValue(itemId, out info);
        }
    }
}
