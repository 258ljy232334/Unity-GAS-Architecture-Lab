using FSM.Blob;
using Unity.Entities;
namespace FSM.ECS
{
    public struct FSMComponent : IComponentData
    {
        public BlobAssetReference<FSMRuntimeBlob> BlobRef;
        public int CurrentStateIndex;
        public float Timer;
    }
}
