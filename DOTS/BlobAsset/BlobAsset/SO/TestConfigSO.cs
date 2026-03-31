

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlobAsset.SO {
    [CreateAssetMenu(fileName ="TestConfig",
        menuName ="SO/Test/Config")]
    public class TestConfigSO :ScriptableObject
    {
        public int TestInt;
        public string TestString;
        public List<float> FloatList;
        public List<SubData> SubDataList;
        public bool IsEqual;
    }
    [Serializable]
    public class SubData // 自定义类
    {
        public int SubId;
        public float SubValue;
    }
}
