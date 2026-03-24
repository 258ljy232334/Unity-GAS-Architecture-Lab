namespace Data
{
    //实现序列化的接口
    public interface ISaveSerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }
}
