using Gameplay.Tag;
using UnityEngine;

namespace Gameplay.Effect
{
    public class EffectInstance
    {
        public readonly GameplayEffect Config;
        public readonly GameplayTag SourceTag;
        public int StackCount;
        //失效的时间
        public float ExpireTime;
        //下一次周期触发的时间
        public float NextPeriodTime;
        //是否失效了
        public bool IsExpired => Config.EffectDurationType == DurationType.HasDuration &&
            Time.time >= ExpireTime;
        public bool ShouldTriggerPeriod => (Config.EffectDurationType == DurationType.Infinite
            || Config.EffectDurationType == DurationType.HasDuration)
            && Config.Period > 0
            && Time.time >= NextPeriodTime;
        public EffectInstance(GameplayEffect spec,GameplayTag tag)
        {
            Config = spec;
            SourceTag = tag;
            StackCount = 1;
            var now=Time.time;
            ExpireTime=Config.EffectDurationType==DurationType.HasDuration?
                now+Config.Duration:float.MaxValue;
            NextPeriodTime=Config.Period>0?now+Config.Period:float.MaxValue;
        }
    }
}