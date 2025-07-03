using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpreadTweak : ITweak<AudioSource>
    {
        [Range(0, 360)]
        public float Spread;
        
        public void Apply(AudioSource source) => source.spread = Spread;
    }
}