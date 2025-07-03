using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpatialBlendTweak : Tweak
    {
        [Range(0, 1)]
        public float SpatialBlend;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.spatialBlend = SpatialBlend;
            return source;
        }
    }
}