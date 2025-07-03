using System;
using AudioManager.MinMax;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class StereoPanRangeTweak : ITweak<AudioSource>
    {
        [MinMax(-1, 1, "")]
        public Vector2 StereoPanRange;
        
        public void Apply(AudioSource source) => source.panStereo = Random.Range(StereoPanRange.x, StereoPanRange.y);
    }
}