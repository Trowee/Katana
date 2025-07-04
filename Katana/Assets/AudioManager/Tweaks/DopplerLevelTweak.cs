using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class DopplerLevelTweak : IAppliable<AudioSource>
    {
        [Range(0, 5)]
        public float DopplerLevel = 1;
        
        public void Apply(AudioSource source) => source.dopplerLevel = DopplerLevel;
    }
}