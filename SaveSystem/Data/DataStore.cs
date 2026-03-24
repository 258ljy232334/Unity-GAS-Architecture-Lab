using Data.Save;
using Data.Serializer;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
namespace Data.Store
{
    public class DataStore : IDataStore
    {
        private ISaveSerializer _serializer = new JsonUtilitySerializer();

        private string GetPath(string key)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{key}.txt");
            return path;
        }

        //保存和加载不会同时发生
        public async void Save(SaveBlob blob, string key)
        {
            var path = GetPath(key);

            var tmp = path + ".tmp";

            // 1. 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            // 2. 先写完临时文件
            await Task.Run(() =>
            {
                var json = _serializer.Serialize(blob);
                File.WriteAllText(tmp, json);
            }).ConfigureAwait(false);

            // 3. 如果这是第一次存档，path 还不存在，直接 Move 即可
            if (!File.Exists(path))
            {
                File.Move(tmp, path);          // 第一次保存，无旧文件可替换
            }
            else
            {
                File.Replace(tmp, path, null); // 后续覆盖，原子替换
            }
        }

        public SaveBlob Load(string key)
        {
            string path = GetPath(key);
            if (!File.Exists(path))
            {
                Debug.Log($"[DataStore] No file → {path} , returning empty blob");
                return new SaveBlob();          // 首次运行给个空壳
            }

            string json = File.ReadAllText(path);
            SaveBlob blob = _serializer.Deserialize<SaveBlob>(json);
            //if (blob._version.Count != 1)
            //{
            //    Debug.LogWarning("当前版本号不对,旧存档已经报废");
            //    return new SaveBlob();
            //}
            Debug.Log($"[DataStore] Loaded ← {path}");
            return blob;
        }
    }
}
