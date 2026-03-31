using FSM.FuncLibrary;
using Unity.Burst;
using Unity.Entities;

namespace FSM.Blob
{
    public struct StateNodeBlob
    {
        public int StateID;
        public FunctionPointer<FSMActionDelegate> ExecuteFunc;
        public BlobArray<StateTranslationBlob> Translations;
    }
}
