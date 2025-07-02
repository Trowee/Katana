using UnityEngine;

namespace AudioManager
{
    public static class Utils
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
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();
            return component;
        }
    }
}