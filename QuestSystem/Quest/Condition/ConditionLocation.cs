using Quest.Instance;

namespace Quest.Condition
{
    public struct ConditionLocation
    {
        public TaskInstance Task;
        public int ConditionIndex;
        public ConditionLocation(TaskInstance instance,int index)
        {
            Task=instance;
            ConditionIndex=index;
        }
    }
}
