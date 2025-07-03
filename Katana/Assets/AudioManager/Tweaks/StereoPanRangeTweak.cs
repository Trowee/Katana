using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class StereoPanRangeTweak : Tweak
    {
        [MinMax(-1, 1, "")]
        public Vector2 StereoPanRange;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.panStereo = Random.Range(StereoPanRange.x, StereoPanRange.y);
            return source;
        }
    }
}