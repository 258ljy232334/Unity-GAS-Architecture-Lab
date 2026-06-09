using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
namespace ObjectPool
{
    public class GameObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private int _spawnCount = 10000;
        [Inject]
        private IObjectPool _pool;
        private const int MAX_SPAWN_COUNT = 20000;

        [Button("Spawn WithOut Pool")]
        public void SpawnWithOutPool()
        {
            if (_prefab == null)
            {
                return;
            }
            Debug.Log("Spawn WithOut Pool");
            for (int i = 0; i < Mathf.Min(MAX_SPAWN_COUNT,_spawnCount); i++)
            {
                GameObject obj = Instantiate(_prefab);
                obj.AddComponent<Rigidbody>();
                Destroy(obj);
            }
        }
        [Button("Spawn With Pool")]
        public void SpawnWithPool()
        {
            if (_prefab == null)
            {
                return;
            }
            for (int i = 0; i < Mathf.Min(MAX_SPAWN_COUNT, _spawnCount); i++)
            {
                //TODO
            }
        }

    }

}