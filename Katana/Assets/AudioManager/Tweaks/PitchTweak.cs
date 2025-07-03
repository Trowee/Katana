using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class PitchTweak : Tweak
    {
        [Range(-3, 3)]
        public float Pitch = 1;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.pitch = Pitch;
            return source;
        }
    }
}