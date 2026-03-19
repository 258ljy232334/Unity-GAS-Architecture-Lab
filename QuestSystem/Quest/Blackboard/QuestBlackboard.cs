using Quest.Enum;
using Quest.Interface;
using Quest.Signal;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zenject;

namespace Quest.Blackboard
{
    public class QuestBlackboard : IQuestBlackboard
    {
        [Inject]
        private SignalBus _bus;
        private Dictionary<(BlackboardKeyType, int), ValueUnion> _valueDic = new Dictionary<(BlackboardKeyType, int), ValueUnion>();

        public int GetInt(BlackboardKeyType key, int instanceId)
        {
            if (_valueDic.TryGetValue((key, instanceId), out var value))
            {
                return value.IntValue;
            }
            return -1;
        }
        public float GetFloat(BlackboardKeyType key, int instanceId)
        {
            if (_valueDic.TryGetValue((key, instanceId), out var value))
            {
                return value.FloatValue;
            }
            return -1f;
        }
        public bool GetBool(BlackboardKeyType key, int instanceId)
        {
            if (_valueDic.TryGetValue((key, instanceId), out var value))
            {
                return value.BoolValue;
            }
            return false;
        }

        public void SetInt(BlackboardKeyType key, int instanceId, int value)
        {
            var temp = new ValueUnion(value);
            _valueDic[(key, instanceId)] = temp;
            FireSignal(key, instanceId, temp);
        }
        public void SetFloat(BlackboardKeyType key, int instanceId, float value)
        {
            var temp = new ValueUnion(value);
            _valueDic[(key, instanceId)] = temp;
            FireSignal(key, instanceId, temp);
        }
        public void SetBool(BlackboardKeyType key, int instanceId, bool value)
        {
            var temp = new ValueUnion(value);
            _valueDic[(key, instanceId)] = temp;
            FireSignal(key, instanceId,temp);
        }
        private void FireSignal(BlackboardKeyType key, int instanceId,ValueUnion value) =>_bus.Fire(new OnBlackboardValueChangedSignal(key, instanceId,value));

    }

    //利用内存头对齐
    [StructLayout(LayoutKind.Explicit)]
    public struct ValueUnion
    {
        [FieldOffset(0)] public int IntValue;
        [FieldOffset(0)] public float FloatValue;
        [FieldOffset(0)] public bool BoolValue;

        public ValueUnion(int value) : this() => IntValue = value;
        public ValueUnion(float value) : this() => FloatValue = value;
        public ValueUnion(bool value) : this() => BoolValue = value;
    }
}
