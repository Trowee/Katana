using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class ReverbZoneMixRangeTweak : Tweak
    {
        [MinMax(0, 1.1f, "")]
        public Vector2 ReverbZoneMixRange = Vector2.one;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.reverbZoneMix = Random.Range(ReverbZoneMixRange.x, ReverbZoneMixRange.y);
            return source;
        }
    }
}