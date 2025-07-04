using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class VolumeTweak : IAppliable<AudioSource>
    {
        [Range(0, 1)]
        public float Volume = 1;
        
        public void Apply(AudioSource source) => source.volume = Volume;
    }
}