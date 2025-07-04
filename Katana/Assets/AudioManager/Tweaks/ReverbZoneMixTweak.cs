using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class ReverbZoneMixTweak : IAppliable<AudioSource>
    {
        [Range(0, 1.1f)]
        public float ReverbZoneMix = 1;
        
        public void Apply(AudioSource source) => source.reverbZoneMix = ReverbZoneMix;
    }
}