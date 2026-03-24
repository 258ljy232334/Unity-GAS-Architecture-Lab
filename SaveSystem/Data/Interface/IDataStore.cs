using Data.Save;

namespace Data
{
    public interface IDataStore
    {
        void Save(SaveBlob blob, string key);
        SaveBlob Load(string key);
    }
}
