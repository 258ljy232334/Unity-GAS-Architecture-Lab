using UnityEngine;
//实现序列化的方式
namespace Data.Serializer
{
    public class JsonUtilitySerializer : ISaveSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public string Serialize(object obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}
