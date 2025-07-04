using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class StereoPanTweak : IAppliable<AudioSource>
    {
        [Range(-1, 1)]
        public float StereoPan;
        
        public void Apply(AudioSource source) => source.panStereo = StereoPan;
    }
}