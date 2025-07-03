using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class DopplerLevelTweak : Tweak
    {
        [Range(0, 5)]
        public float DopplerLevel = 1;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.dopplerLevel = DopplerLevel;
            return source;
        }
    }
}