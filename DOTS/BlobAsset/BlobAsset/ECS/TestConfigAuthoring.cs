using BlobAsset.Blob;
using BlobAsset.Func;
using BlobAsset.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
namespace BlobAsset.ECS
{
    public class TestConfigAuthoring : MonoBehaviour
    {
        public TestConfigSO ConfigSO;

        private class Baker : Baker<TestConfigAuthoring>
        {
            public override void Bake(TestConfigAuthoring authoring)
            {
                if (authoring == null || authoring.ConfigSO == null) return;

                var entity = GetEntity(TransformUsageFlags.None);
                BlobAssetReference<TestConfigBlob> blobRef;

                using (var builder = new BlobBuilder(Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<TestConfigBlob>();
                    
                    root.TestInt = authoring.ConfigSO.TestInt;
                    
                    var builderRef = builder;
                    builderRef.AllocateString(ref root.TestString, authoring.ConfigSO.TestString);

                    var floatArrayBuilder = builder.Allocate(ref root.FloatList, authoring.ConfigSO.FloatList.Count);
                    for (int i = 0; i < authoring.ConfigSO.FloatList.Count; i++)
                    {
                        floatArrayBuilder[i] = authoring.ConfigSO.FloatList[i];
                    }
                    var subDataArrayBuilder = builder.Allocate(ref root.SubDataList, authoring.ConfigSO.SubDataList.Count);
                    for (int i = 0; i < authoring.ConfigSO.SubDataList.Count; i++)
                    {
                        var source = authoring.ConfigSO.SubDataList[i];

                        // 将类中的数据赋值给结构体
                        subDataArrayBuilder[i] = new SubDataBlob
                        {
                            SubId = source.SubId,
                            SubValue = source.SubValue
                        };
                    }
                    if(authoring.ConfigSO.IsEqual)
                    {
                        root.LogicFunc = BurstCompiler.CompileFunctionPointer<TestDelegate>(TestFuncLibrary.CheckEquals);
                    }
                    else
                    {
                        root.LogicFunc = BurstCompiler.CompileFunctionPointer<TestDelegate>(TestFuncLibrary.CheckNotEquals);
                    }
                        blobRef = builder.CreateBlobAssetReference<TestConfigBlob>(Allocator.Persistent);
                } 
                AddBlobAsset(ref blobRef, out _);
                AddComponent(entity, new TestConfigComponent { BlobRef = blobRef });
            }
        }
    }
}
