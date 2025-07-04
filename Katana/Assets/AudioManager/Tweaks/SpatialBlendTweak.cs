using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpatialBlendTweak : IAppliable<AudioSource>
    {
        [Range(0, 1)]
        public float SpatialBlend;
        
        public void Apply(AudioSource source) => source.spatialBlend = SpatialBlend;
    }
}