using Data.Save;
using Scene;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Data.Facade
{
    public class DataManager : MonoBehaviour, IDataManager, IInitializable
    {
        [Inject]
        private IDataStore _store;
        [Inject]
        private IDataRepo _repo;
        [Inject]
        private IDataServe _serve;
        [Inject]
        private ISceneFacade _sceneFacade;
        private SaveBlob _saveBlob = new SaveBlob();
        //初始化加载
        public void Initialize()
        {
            string archive = _sceneFacade.GetArchiveName();
            Load(archive); //初始化执行加载方法
        }

        public IReadOnlyList<T> GetRoList<T>() where T : class, IDatable
        {
            return _repo.GetRoList<T>();
        }

        public void Save()
        {
            _repo.SaveBlobData(_saveBlob);
            string archive = _sceneFacade.GetArchiveName();
            _store.Save(_saveBlob, archive);
        }
        public void Load(string path)
        {
            _saveBlob = _store.Load(path);
            _repo.Initialize(_saveBlob);
        }

        public List<T> GetWoList<T>() where T : class, IDatable
        {
            return _repo.GetWoList<T>();
        }

        public T GetRoSingle<T>() where T : class, IDatable
        {
            return _repo.GetRoSingle<T>();
        }

        public T GetWoSingle<T>() where T : class, IDatable
        {
            return _repo.GetWoSingle<T>();
        }
    }
}

   

