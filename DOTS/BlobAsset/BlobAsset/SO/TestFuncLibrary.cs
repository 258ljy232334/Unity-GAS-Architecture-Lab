using Unity.Burst;

namespace BlobAsset.Func
{
    public delegate bool TestDelegate(int current, int target);
    [BurstCompile]
    public static class TestFuncLibrary
    {
        [BurstCompile]
        public static bool CheckNotEquals(int current, int target)
        {
            return current!=target; 
        }
        [BurstCompile]
        public static bool CheckEquals(int current, int target)
        {
            return current == target;
        }
    }
}