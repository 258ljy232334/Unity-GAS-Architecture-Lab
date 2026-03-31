
using BlobAsset.Func;
using Unity.Burst;
using Unity.Entities;

namespace BlobAsset.Blob
{
    public struct TestConfigBlob
    {
        public int TestInt;
        public BlobString TestString;
        public BlobArray<float> FloatList;
        public FunctionPointer<TestDelegate> LogicFunc;
        public BlobArray<SubDataBlob> SubDataList;
    }
    public struct SubDataBlob // 扁平结构体
    {
        public int SubId;
        public float SubValue;
    }
}
