using Quest.Blackboard;
using Quest.Config;
using Quest.Enum;
using Quest.Interface;
using System;

namespace Quest.Instance {
    public class TaskInstance
    {
        public TaskConfig Config => _config;

        private readonly TaskConfig _config;

        private ValueUnion[] _previousValues;
        private bool[] _conditionStates;
        private int _remainingCount;
        public TaskInstance(TaskConfig config,IQuestBlackboard blackboard)
        {
            _config = config;
            
            _conditionStates=new bool[config.Conditions.Count];
            _remainingCount = config.Conditions.Count;
            if(config.IsPreviousValue)
            {
                int n=config.Conditions.Count;
                _previousValues = new ValueUnion[n];
                for (int i = 0; i < n; i++)
                {
                    var condition=config.Conditions[i];
                    switch (condition.ValueType)
                    {
                        case BlackboardValueType.Int:
                            _previousValues[i] = new ValueUnion(blackboard.GetInt(condition.KeyType, condition.InstanceId));
                            break;
                        case BlackboardValueType.Float:
                            _previousValues[i]=new ValueUnion(blackboard.GetFloat(condition.KeyType, condition.InstanceId));
                            break;
                        case BlackboardValueType.Bool:
                            _previousValues[i] = new ValueUnion(blackboard.GetBool(condition.KeyType, condition.InstanceId));
                            break;
                    }
                }
            }
        }
        public bool MarkConditionComplete(int index)
        {
            if (index >= 0 && index < _conditionStates.Length && !_conditionStates[index])
            {
                _conditionStates[index] = true;
                _remainingCount--;
                return _remainingCount <= 0;
            }
            return false;
        }
        
        public void SetPreValue(int index, ValueUnion value)
        {
            _previousValues[index]=value;
        }

        public ValueUnion GetPreValue(int index)
        {
            if (_previousValues != null && index < _previousValues.Length)
            {
                return _previousValues[index];
            }
            return default;
        }
    }
}
