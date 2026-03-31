using Unity.Burst;
using Unity.Entities;
namespace BlobAsset.ECS
{
    partial struct TestBlobCheckSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TestConfigComponent>();
            
        }

        
        public void OnUpdate(ref SystemState state)
        {
            
            state.Enabled = false;

            // 既然是单例配置，推荐直接用 GetSingleton
            var config = SystemAPI.GetSingleton<TestConfigComponent>();

            // 获取只读引用
            ref var blob = ref config.BlobRef.Value;

            // 1. 验证基础数据
            UnityEngine.Debug.Log($"[ECS Config] ID: {blob.TestInt}, Name: {blob.TestString.ToString()}");

            // 2. 验证 float 列表
            UnityEngine.Debug.Log($"[ECS Config] FloatList Count: {blob.FloatList.Length}");

            // 3. 验证你加的“嵌套扁平结构体”
            for (int i = 0; i < blob.SubDataList.Length; i++)
            {
                ref var sub = ref blob.SubDataList[i];
                UnityEngine.Debug.Log($"--- SubItem[{i}]: ID={sub.SubId}, Value={sub.SubValue}");
            }
            if(blob.LogicFunc.Invoke(10,20))
            {
                UnityEngine.Debug.Log("你好");
            }
            else
            {
                UnityEngine.Debug.Log("你坏");
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
