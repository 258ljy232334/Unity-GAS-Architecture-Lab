
using Data.Save;
using System.Collections.Generic;
namespace Data
{
    public interface IDataRepo
    {
        void Initialize(SaveBlob blob);
        void SaveBlobData(SaveBlob blob);
        // 只读遍历
        IReadOnlyList<T> GetRoList<T>() where T : class, IDatable;

        // 可写
        List<T> GetWoList<T>() where T : class, IDatable;
        T GetRoSingle<T>() where T : class, IDatable;
        T GetWoSingle<T>() where T : class, IDatable;
    }
}
