using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [Serializable]
    public struct CannonAnimationKey
    {
        public Vector2 DurationRange;
        public AnimationCurve Curve;
        [FoldoutGroup("Cannon")]
        public Vector3 CannonPositionOffsetMin;
        [FoldoutGroup("Cannon")]
        public Vector3 CannonPositionOffsetMax;
        [FoldoutGroup("Cannon")]
        public bool TranslateCannonPosition;

        [FoldoutGroup("Barrel")]
        public Vector3 BarrelPositionOffsetMin;
        [FoldoutGroup("Barrel")]
        public Vector3 BarrelPositionOffsetMax;
        [FoldoutGroup("Barrel")]
        public bool TranslateBarrelPosition;
        [FoldoutGroup("Barrel")]
        public Vector3 BarrelRotationOffsetMin;
        [FoldoutGroup("Barrel")]
        public Vector3 BarrelRotationOffsetMax;
    }
}
