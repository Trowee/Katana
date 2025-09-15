using System;
using NnUtils.Modules.Easings;
using UnityEngine;

[Serializable]
public class CameraAnimationSettings
{
    public float Duration;
    public EasingType Easing;
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

    public CameraAnimationSettings(float duration = 0, EasingType easing = EasingType.Linear)
    {
        Duration = duration;
        Easing = easing;
        Curve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    public CameraAnimationSettings(float duration, EasingType easing, AnimationCurve curve)
    {
        Duration = duration;
        Easing = easing;
        Curve = curve;
    }
}