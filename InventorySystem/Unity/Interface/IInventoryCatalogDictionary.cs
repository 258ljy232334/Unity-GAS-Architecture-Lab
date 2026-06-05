using Item.Information;

namespace Inventory.Interface
{
    /// <summary>
    /// ¿â´æÄ¿Â¼×Öµä½Ó¿Ú
    /// </summary>
    public interface IInventoryCatalogDictionary
    {
        ItemInformation GetItemInformation(int itemId);
        bool TryGetItemInformation(int itemId, out ItemInformation info);
    }
}
