using NnUtils.Modules.Easings;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class Utils
    {
        public static float Tween(ref float lerpPos, float lerpTime = 1,
                                  Easings.Type easingType = Easings.Type.Linear,
                                  bool scaled = true, float multiplier = 1)
        {
            if (lerpTime == 0) lerpPos = 1;
            else
                lerpPos = Mathf.Clamp01(lerpPos +=
                                            ((scaled ? Time.deltaTime : Time.unscaledDeltaTime) /
                                             lerpTime) * multiplier);
            return Easings.Ease(lerpPos, easingType);
        }
    }
}