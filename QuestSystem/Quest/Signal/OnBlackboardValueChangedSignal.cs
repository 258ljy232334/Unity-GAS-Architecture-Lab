using Quest.Blackboard;
using Quest.Enum;

namespace Quest.Signal
{
    public readonly struct OnBlackboardValueChangedSignal
    {
        public readonly BlackboardKeyType KeyType;
        public readonly int InstanceID;
        public readonly ValueUnion Value;
        public OnBlackboardValueChangedSignal(BlackboardKeyType keyType,int id,ValueUnion value)
        {
            KeyType = keyType;
            InstanceID = id;
            Value = value;
        }
    }
}