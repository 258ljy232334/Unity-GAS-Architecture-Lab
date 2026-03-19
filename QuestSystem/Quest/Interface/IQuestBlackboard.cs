using Quest.Enum;

namespace Quest.Interface
{
    public interface IQuestBlackboard
    {
        int GetInt(BlackboardKeyType key, int instanceId);
        float GetFloat(BlackboardKeyType key, int instanceId);
        bool GetBool(BlackboardKeyType key, int instanceId);
        void SetInt(BlackboardKeyType key, int instanceId, int value);
        void SetFloat(BlackboardKeyType key, int instanceId, float value);
        void SetBool(BlackboardKeyType key, int instanceId, bool value);
    }
}