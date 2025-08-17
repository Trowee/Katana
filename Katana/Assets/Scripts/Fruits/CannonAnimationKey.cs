using System;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [Serializable]
    public struct CannonAnimationKey
    {
        public Vector2 DurationRange;
        public AnimationCurve Curve;
        public Vector3 CannonPositionOffsetMin;
        public Vector3 CannonPositionOffsetMax;
        public bool TranslateCannonPosition;
        public Vector3 BarrelPositionOffsetMin;
        public Vector3 BarrelPositionOffsetMax;
        public bool TranslateBarrelPosition;
        public Vector3 BarrelRotationOffsetMin;
        public Vector3 BarrelRotationOffsetMax;
    }
}
