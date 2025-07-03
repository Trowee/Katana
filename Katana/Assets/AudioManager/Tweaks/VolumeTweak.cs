using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class VolumeTweak : Tweak
    {
        [Range(0, 1)]
        public float Volume = 1;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.volume = Volume;
            return source;
        }
    }
}