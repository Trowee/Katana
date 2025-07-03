using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class PitchTweak : ITweak<AudioSource>
    {
        [Range(-3, 3)]
        public float Pitch = 1;
        
        public void Apply(AudioSource source) => source.pitch = Pitch;
    }
}