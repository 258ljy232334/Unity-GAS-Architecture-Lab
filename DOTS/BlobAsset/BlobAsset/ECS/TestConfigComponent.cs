using BlobAsset.Blob;
using Unity.Entities;
namespace BlobAsset.ECS
{
    public struct TestConfigComponent : IComponentData
    {
        public BlobAssetReference<TestConfigBlob> BlobRef;
    }
}