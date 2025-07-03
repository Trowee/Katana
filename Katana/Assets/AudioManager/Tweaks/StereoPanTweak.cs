using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class StereoPanTweak : Tweak
    {
        [Range(-1, 1)]
        public float StereoPan;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.panStereo = StereoPan;
            return source;
        }
    }
}