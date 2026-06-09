using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace ObjectPool
{
    public class Pool : IObjectPool,IInitializable
    {
        private Dictionary<string, Stack<GameObject>> _dic;

        public void Initialize()
        {
            _dic=new Dictionary<string, Stack<GameObject>>();
        }
        public GameObject Get(GameObject prefab)
        {
            throw new System.NotImplementedException();
        }
        public void Return(GameObject obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
