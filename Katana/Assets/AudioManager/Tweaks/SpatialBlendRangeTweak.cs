using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpatialBlendRangeTweak : Tweak
    {
        [MinMax(0, 1, "")]
        public Vector2 SpatialBlendRange;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.spatialBlend = Random.Range(SpatialBlendRange.x, SpatialBlendRange.y);
            return source;
        }
    }
}