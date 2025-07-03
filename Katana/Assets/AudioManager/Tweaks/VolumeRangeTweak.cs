using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class VolumeRangeTweak : Tweak
    {
        [MinMax(0, 1, "")]
        public Vector2 VolumeRange = Vector2.one;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.volume = Random.Range(VolumeRange.x, VolumeRange.y);
            return source;
        }
    }
}