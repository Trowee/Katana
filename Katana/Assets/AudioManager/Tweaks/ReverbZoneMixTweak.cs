using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class ReverbZoneMixTweak : Tweak
    {
        [Range(0, 1.1f)]
        public float ReverbZoneMix = 1;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.reverbZoneMix = ReverbZoneMix;
            return source;
        }
    }
}