using Gameplay.Tag;
using UnityEngine;

namespace Gameplay.Effect
{
    public class EffectInstance
    {
        public readonly GameplayEffect Spec;
        public readonly GameplayTag SourceTag;
        public int StackCount;
        //失效的时间
        public float ExpireTime;
        //下一次周期触发的时间
        public float NextPeriodTime;
        //是否失效了
        public bool IsExpired => Spec.DurationType == DurationType.HasDuration &&
            Time.time >= ExpireTime;
        public bool ShouldTriggerPeriod => (Spec.DurationType == DurationType.Infinite
            || Spec.DurationType == DurationType.HasDuration)
            && Spec.Period > 0
            && Time.time >= NextPeriodTime;
        public EffectInstance(GameplayEffect spec,GameplayTag tag)
        {
            Spec = spec;
            SourceTag = tag;
            StackCount = 1;
            var now=Time.time;
            ExpireTime=Spec.DurationType==DurationType.HasDuration?
                now+Spec.Duration:float.MaxValue;
            NextPeriodTime=Spec.Period>0?now+Spec.Period:float.MaxValue;
        }
    }
}