using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpatialBlendRangeTweak : ITweak<AudioSource>
    {
        [MinMax(0, 1, "")]
        public Vector2 SpatialBlendRange;

        public void Apply(AudioSource source) =>
            source.spatialBlend = Random.Range(SpatialBlendRange.x, SpatialBlendRange.y);
    }
}