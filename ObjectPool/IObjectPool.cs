using UnityEngine;

namespace ObjectPool
{
    public interface IObjectPool
    {
        GameObject Get(GameObject prefab);
        void Return(GameObject obj);
    }
}