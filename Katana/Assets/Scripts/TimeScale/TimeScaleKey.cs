using System;
using NnUtils.Modules.Easings;
using NnUtils.Scripts;

namespace Assets.Scripts.TimeScale
{
    [Serializable]
    public struct TimeScaleKey
    {
        public float TimeScale;
        public float Time;
        public Easings.Type Easing;

        public TimeScaleKey(float timeScale = 1, float time = 0, Easings.Type easing = Easings.Type.Linear)
        {
            TimeScale = timeScale;
            Time = time;
            Easing = easing;
        }
    }
}