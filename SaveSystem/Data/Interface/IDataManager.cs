using System.Collections.Generic;
namespace Data
{
    public interface IDataManager
    {
        IReadOnlyList<T> GetRoList<T>() where T : class, IDatable;

        // 可写
        List<T> GetWoList<T>() where T : class, IDatable;
        T GetRoSingle<T>() where T : class, IDatable;
        T GetWoSingle<T>() where T : class, IDatable;
        void Load(string key);
        void Save();
    }
}
