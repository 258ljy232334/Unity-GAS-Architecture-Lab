
using Unity.Entities;

namespace FSM.Blob
{
    public struct FSMRuntimeBlob
    {
        public BlobArray<StateNodeBlob> AllStates;
    }
}