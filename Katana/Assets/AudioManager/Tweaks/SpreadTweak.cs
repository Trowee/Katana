using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class SpreadTweak : Tweak
    {
        [Range(0, 360)]
        public float Spread;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.spread = Spread;
            return source;
        }
    }
}