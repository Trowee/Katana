using System;
using NnUtils.Modules.Easings;

namespace Assets.Scripts.TimeScale
{
    [Serializable]
    public struct TimeScaleKey
    {
        public float TimeScale;
        public float Time;
        public EasingType Easing;

        public TimeScaleKey(float timeScale = 1, float time = 0, EasingType easing = EasingType.Linear)
        {
            TimeScale = timeScale;
            Time = time;
            Easing = easing;
        }
    }
}
