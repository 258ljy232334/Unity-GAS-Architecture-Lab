using Quest.Config;

namespace Quest.Signal
{
    public readonly struct OnTaskFinishedSignal
    {
        public readonly TaskConfig Config;
        public OnTaskFinishedSignal(TaskConfig config)
        {
            Config= config;
        }
    }
}